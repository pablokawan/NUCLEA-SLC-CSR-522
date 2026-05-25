namespace SLCSettlementAPI;

public interface IPSCLClient
{
    Task<HttpResponseMessage> SolicitarLiquidacaoCreditoAsync(SolicitacaoLiquidacaoRequest request);
    Task<HttpResponseMessage> SolicitarLiquidacaoDebitoAsync(SolicitacaoLiquidacaoRequestDebito request);
    Task<HttpResponseMessage> SolicitarLiquidacaoAntecipacaoAsync(SolicitacaoLiquidacaoRequestAntecipacao request);
    Task<HttpResponseMessage> ConsultarSituacaoAsync(string numCtrlCip);
    Task<HttpResponseMessage> TestarConectividadeEcoAsync();
    Task<HttpResponseMessage> ConsultarRelatorioNuLiquidAsync(string nuLiquid);
    Task<HttpResponseMessage> ConsultarCadastroArranjoAsync(string ispbContraparte);
}

public class NucleaSettings
{
    public const string SectionName = "Nuclea";
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public string IspbPrincipal { get; set; } = string.Empty;
    public string CertificatePath { get; set; } = string.Empty;
    public string CertificatePassword { get; set; } = string.Empty;
}

public class NucleaClient : IPSCLClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NucleaClient> _logger;

    public NucleaClient(HttpClient httpClient, ILogger<NucleaClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public Task<HttpResponseMessage> SolicitarLiquidacaoCreditoAsync(SolicitacaoLiquidacaoRequest request)
    {
        const string endpoint = "v1/liquidacoes-credito";
        return PostSolicitacaoAsync(endpoint, request);
    }

    public Task<HttpResponseMessage> SolicitarLiquidacaoDebitoAsync(SolicitacaoLiquidacaoRequestDebito request)
    {
        const string endpoint = "v1/liquidacoes-debito";
        return PostSolicitacaoAsync(endpoint, request);
    }

    public Task<HttpResponseMessage> SolicitarLiquidacaoAntecipacaoAsync(SolicitacaoLiquidacaoRequestAntecipacao request)
    {
        const string endpoint = "v1/liquidacoes-antecipacao";
        return PostSolicitacaoAsync(endpoint, request);
    }

    public Task<HttpResponseMessage> ConsultarSituacaoAsync(string numCtrlCip)
    {
        const string endpointTemplate = "v1/liquidacoes/{numCtrlCip}/processamento";
        var route = endpointTemplate.Replace("{numCtrlCip}", numCtrlCip);
        return GetAsync(route);
    }

    public Task<HttpResponseMessage> TestarConectividadeEcoAsync()
    {
        const string route = "v1/ferramentas/credenciadoras/eco?msg=testesuccess";
        return GetAsync(route);
    }

    public Task<HttpResponseMessage> ConsultarRelatorioNuLiquidAsync(string nuLiquid)
    {
        const string endpoint = "v1/credenciadoras/relatorios/detalhado-por-nuliquid";
        var route = $"{endpoint}?NULiquid={nuLiquid}";
        return GetAsync(route);
    }

    public Task<HttpResponseMessage> ConsultarCadastroArranjoAsync(string ispbContraparte)
    {
        const string endpoint = "v1/credenciadoras/relatorios/consulta-cadastro-em-arranjo";
        var route = $"{endpoint}?ISPBContraparte={ispbContraparte}";
        return GetAsync(route);
    }

    private async Task<HttpResponseMessage> PostSolicitacaoAsync<TRequest>(string endpoint, TRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, request);

            _logger.LogInformation("SLC POST {Endpoint} respondeu {StatusCode}", endpoint, response.StatusCode);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha crítica na chamada POST SLC: {Endpoint}", endpoint);
            throw;
        }
    }

    private async Task<HttpResponseMessage> GetAsync(string route)
    {
        try
        {
            var response = await _httpClient.GetAsync(route);

            _logger.LogInformation("SLC GET {Route} respondeu {StatusCode}", route, response.StatusCode);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha crítica na chamada GET SLC: {Route}", route);
            throw;
        }
    }
}
