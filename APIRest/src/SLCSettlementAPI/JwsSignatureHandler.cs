using System.Net.Http.Headers;
using System.Net.Mime;

namespace SLCSettlementAPI;

public class JwsSignatureHandler : DelegatingHandler
{
    private readonly IJwsSignatureProvider _cryptoProvider;

    public JwsSignatureHandler(IJwsSignatureProvider cryptoProvider)
    {
        _cryptoProvider = cryptoProvider;
    }

    private const string JwsSignatureHeaderName = "x-jws-signature";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Gera dados de referência para assinatura
        var requestId = Guid.NewGuid().ToString("N"); // TODO : utilizar um identificador de correlação em produção
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd");

        // Gera o header x-jws-signature
        string jsonBody = "";

        if (request.Content != null)
        {
            await request.Content.LoadIntoBufferAsync();

            jsonBody = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        var jwsHeader = _cryptoProvider.GenerateDetachedSignature(jsonBody, requestId, timestamp);

        // Injeta no request
        request.Headers.Add(JwsSignatureHeaderName, jwsHeader);

        if (request.Content != null && request.Content.Headers.ContentType == null)
        {
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(
                MediaTypeNames.Application.Json);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}