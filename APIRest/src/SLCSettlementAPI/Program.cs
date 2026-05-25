using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SLCSettlementAPI;
using System.Net.Mime;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<NucleaSettings>(
    builder.Configuration.GetSection(NucleaSettings.SectionName));

builder.Services.Configure<KestrelServerOptions>(options =>
    options.AddServerHeader = false);

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<NucleaSettings>>().Value;

    if (string.IsNullOrEmpty(settings.CertificatePath) || string.IsNullOrEmpty(settings.CertificatePassword))
    {
        throw new InvalidOperationException("Caminho do certificado ou senha nao foram configurados no 'NucleaSettings'.");
    }

    var storageFlags = X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet;

    try
    {
        return new X509Certificate2(settings.CertificatePath, settings.CertificatePassword, storageFlags);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Falha ao carregar o certificado SLC no caminho: {settings.CertificatePath}", ex);
    }
});

builder.Services.AddSingleton<IJwsSignatureProvider, JwsSignatureProvider>();
builder.Services.AddTransient<JwsSignatureHandler>();

builder.Services.AddHttpClient<IPSCLClient, NucleaClient>((provider, client) =>
{
    var settings = provider.GetRequiredService<IOptions<NucleaSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
})
.ConfigurePrimaryHttpMessageHandler(provider =>
{
    var crypto = provider.GetRequiredService<IJwsSignatureProvider>();
    var handler = new HttpClientHandler();

    handler.ClientCertificates.Add(crypto.GetClientCertificate());
    handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;

    return handler;
})
.AddHttpMessageHandler<JwsSignatureHandler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("eco", async ([FromServices] IPSCLClient slcClient) =>
{
    using var response = await slcClient.TestarConectividadeEcoAsync();

    return await CreateTextResponseAsync(response);
})
.WithName("SLC0800")
.WithOpenApi();

app.MapGet("relatorios/consulta-cadastro-em-arranjo", async ([FromServices] IPSCLClient slcClient, string ispb) =>
{
    using var response = await slcClient.ConsultarCadastroArranjoAsync(ispb);

    return await CreateTextResponseAsync(response);
})
.WithName("SLC0910")
.WithOpenApi();

app.MapPost("/liquidacoes-credito", async ([FromServices] IPSCLClient slcClient, [FromBody] SolicitacaoLiquidacaoRequest solicitacao) =>
{
    var validationError = SolicitacaoLiquidacaoValidator.ValidarCredito(solicitacao);

    if (validationError is not null)
    {
        return Results.BadRequest(new { message = validationError });
    }

    using var response = await slcClient.SolicitarLiquidacaoCreditoAsync(solicitacao);

    return await CreateTextResponseAsync(response);
})
.WithName("SLC0914")
.WithOpenApi(operation =>
{
    AddJsonExamples(operation, new()
    {
        ["CreditoVisaSiloc"] = new()
        {
            Summary = "Visa credito normal via SILOC",
            Description = "Exemplo de credito sem uso de registro informacional.",
            Value = OpenApiExampleParser.Parse(SolicitacaoLiquidacaoExamples.CreditoVisaSilocJson)
        }
    });

    return operation;
});

app.MapPost("/liquidacoes-debito", async ([FromServices] IPSCLClient slcClient, [FromBody] SolicitacaoLiquidacaoRequestDebito solicitacao) =>
{
    var validationError = SolicitacaoLiquidacaoValidator.ValidarDebito(solicitacao);

    if (validationError is not null)
    {
        return Results.BadRequest(new { message = validationError });
    }

    using var response = await slcClient.SolicitarLiquidacaoDebitoAsync(solicitacao);

    return await CreateTextResponseAsync(response);
})
.WithName("SLC0913")
.WithOpenApi(operation =>
{
    AddJsonExamples(operation, new()
    {
        ["DebitoMaestroSiloc"] = new()
        {
            Summary = "Maestro debito normal via SILOC",
            Description = "Exemplo de debito com produto 03 e sem registro informacional.",
            Value = OpenApiExampleParser.Parse(SolicitacaoLiquidacaoExamples.DebitoMaestroSilocJson)
        }
    });

    return operation;
});

app.MapPost("/liquidacoes-antecipacao", async ([FromServices] IPSCLClient slcClient, [FromBody] SolicitacaoLiquidacaoRequestAntecipacao solicitacao) =>
{
    var validationError = SolicitacaoLiquidacaoValidator.ValidarAntecipacao(solicitacao);

    if (validationError is not null)
    {
        return Results.BadRequest(new { message = validationError });
    }

    using var response = await slcClient.SolicitarLiquidacaoAntecipacaoAsync(solicitacao);

    return await CreateTextResponseAsync(response);
})
.WithName("SLC0912")
.WithOpenApi(operation =>
{
    AddJsonExamples(operation, new()
    {
        ["AntecipacaoVisaInformacional"] = new()
        {
            Summary = "Visa antecipacao informacional",
            Description = "Exemplo de antecipacao usando registro informacional, unico fluxo em que esta forma de transferencia eh aceita.",
            Value = OpenApiExampleParser.Parse(SolicitacaoLiquidacaoExamples.AntecipacaoVisaInformacionalJson)
        }
    });

    return operation;
});

app.Run();

static async Task<IResult> CreateTextResponseAsync(HttpResponseMessage response)
{
    var bytes = await response.Content.ReadAsByteArrayAsync();

    var content = Encoding.Latin1.GetString(bytes);

    return Results.Text(content, MediaTypeNames.Application.Json, Encoding.UTF8, statusCode: (int)response.StatusCode);
}

static void AddJsonExamples(OpenApiOperation operation, Dictionary<string, OpenApiExample> examples)
{
    if (operation.RequestBody.Content.TryGetValue(MediaTypeNames.Application.Json, out var mediaType))
    {
        mediaType.Examples = examples;
    }
}
