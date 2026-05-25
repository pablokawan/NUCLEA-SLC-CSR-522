using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SLCSettlementAPI;

public interface IGrupoCentralizadoraComPontosVenda
{
    List<GrupoPontoVenda> GrupoPontoVenda { get; set; }
}

public abstract class SolicitacaoLiquidacaoRequestBase<TGrupoCentralizadora>
    where TGrupoCentralizadora : GrupoCentralizadoraBase
{
    [JsonPropertyName("cnpjBaseCreddr")]
    public string CnpjBaseCredenciador { get; set; } = string.Empty;

    [JsonPropertyName("cnpjCreddr")]
    public string CnpjCredenciador { get; set; } = string.Empty;

    [JsonPropertyName("ispbIfDevdr")]
    public string IspbIfDevedor { get; set; } = string.Empty;

    [JsonPropertyName("ispbIfCredr")]
    public string IspbIfCredora { get; set; } = string.Empty;

    [JsonPropertyName("agCreddr")]
    public string? AgenciaCredenciador { get; set; }

    [JsonPropertyName("ctCreddr")]
    public long? ContaCredenciador { get; set; }

    [JsonPropertyName("nomCreddr")]
    public string NomeCredenciador { get; set; } = string.Empty;
}

// SLC0914 - Solicitacao Liquidacao Credito
public sealed class SolicitacaoLiquidacaoRequest : SolicitacaoLiquidacaoRequestBase<GrupoCentralizadoraCredito>
{
    [JsonPropertyName("grupoSLC0914Centrlz")]
    public GrupoCentralizadoraCredito GrupoCentralizadora { get; set; } = new();
}

public abstract class GrupoCentralizadoraBase
{
    [JsonPropertyName("numCtrlCreddrCentrlz")]
    public string NumCtrlCredenciadorCentralizadora { get; set; } = string.Empty;

    [JsonPropertyName("tpPessoaCentrlz")]
    public TipoPessoa TipoPessoaCentralizadora { get; set; }

    [JsonPropertyName("cnpjCpfCentrlz")]
    public string CnpjCpfCentralizadora { get; set; } = string.Empty;

    [JsonPropertyName("codCentrlz")]
    public string CodigoCentralizadora { get; set; } = string.Empty;

    [JsonPropertyName("tpCt")]
    public TipoConta? TipoConta { get; set; }

    [JsonPropertyName("agCentrlz")]
    public string? AgenciaCentralizadora { get; set; }

    [JsonPropertyName("ctCentrlz")]
    public long? ContaCentralizadora { get; set; }

    [JsonPropertyName("ctPgtoCentrlz")]
    public decimal? ContaPagamentoCentralizadora { get; set; }
}

public sealed class GrupoCentralizadoraCredito : GrupoCentralizadoraBase, IGrupoCentralizadoraComPontosVenda
{
    [JsonPropertyName("grupoSLC0914PontoVenda")]
    public List<GrupoPontoVenda> GrupoPontoVenda { get; set; } = [];
}

public sealed class GrupoCentralizadoraDebito : GrupoCentralizadoraBase, IGrupoCentralizadoraComPontosVenda
{
    [JsonPropertyName("grupoSLC0913PontoVenda")]
    public List<GrupoPontoVenda> GrupoPontoVenda { get; set; } = [];
}

public sealed class GrupoCentralizadoraAntecipacao : GrupoCentralizadoraBase, IGrupoCentralizadoraComPontosVenda
{
    [JsonPropertyName("grupoSLC0912PontoVenda")]
    public List<GrupoPontoVenda> GrupoPontoVenda { get; set; } = [];
}

public class GrupoPontoVenda
{
    [JsonPropertyName("numCtrlCreddrPontoVenda")]
    public string NumCtrlCredenciadorPontoVenda { get; set; } = string.Empty;

    [JsonPropertyName("ispbIfLiquidPontoVenda")]
    public string IspbIfLiquidacaoPontoVenda { get; set; } = string.Empty;

    [JsonPropertyName("codPontoVenda")]
    public string CodigoPontoVenda { get; set; } = string.Empty;

    [JsonPropertyName("nomePontoVenda")]
    public string NomePontoVenda { get; set; } = string.Empty;

    [JsonPropertyName("tpPessoaPontoVenda")]
    public TipoPessoa TipoPessoaPontoVenda { get; set; }

    [JsonPropertyName("cnpjCpfPontoVenda")]
    public string CnpjCpfPontoVenda { get; set; } = string.Empty;

    [JsonPropertyName("codInstitdrArrajPgto")]
    public InstituidorArranjoPagamento CodigoInstituidorArranjo { get; set; }

    [JsonPropertyName("tpProdLiquidCarts")]
    public TipoProdutoLiquidacao TipoProdutoLiquidacao { get; set; }

    [JsonPropertyName("indrFormaTransf")]
    public FormaTransferencia IndicadorFormaTransferencia { get; set; }

    [JsonPropertyName("codMoeda")]
    public string CodigoMoeda { get; set; } = "001";

    [JsonPropertyName("tpPontoVenda")]
    public TipoPontoVenda TipoPontoVenda { get; set; }

    [JsonPropertyName("tpVlrPgto")]
    public TipoValorPagamento TipoValorPagamento { get; set; }

    [JsonPropertyName("dtPgto")]
    public string DataPagamento { get; set; } = string.Empty;

    [JsonPropertyName("vlrPgto")]
    public decimal ValorPagamento { get; set; }

    [JsonPropertyName("formaPgto")]
    public FormaPagamento? FormaPagamento { get; set; }

    [JsonPropertyName("numCtrlPgto")]
    public string? NumeroControlePagamento { get; set; }
}

[JsonConverter(typeof(CodigoEnumJsonConverter<TipoPessoa>))]
public enum TipoPessoa
{
    [EnumMember(Value = "F")]
    Fisica = 1,

    [EnumMember(Value = "J")]
    Juridica = 2
}

[JsonConverter(typeof(CodigoEnumJsonConverter<TipoConta>))]
public enum TipoConta
{
    [EnumMember(Value = "CC")]
    ContaCorrente = 1
}

[JsonConverter(typeof(CodigoEnumJsonConverter<InstituidorArranjoPagamento>))]
public enum InstituidorArranjoPagamento
{
    [EnumMember(Value = "003")]
    MastercardCredito = 3,

    [EnumMember(Value = "004")]
    VisaCredito = 4,

    [EnumMember(Value = "021")]
    HipercardCredito = 21,

    [EnumMember(Value = "025")]
    MaestroDebito = 25,

    [EnumMember(Value = "026")]
    VisaElectronDebito = 26,

    [EnumMember(Value = "027")]
    EloDebito = 27
}

[JsonConverter(typeof(CodigoEnumJsonConverter<TipoProdutoLiquidacao>))]
public enum TipoProdutoLiquidacao
{
    [EnumMember(Value = "01")]
    CartaoCredito = 1,

    [EnumMember(Value = "02")]
    AjusteCredito = 2,

    [EnumMember(Value = "03")]
    CartaoDebito = 3
}

[JsonConverter(typeof(CodigoEnumJsonConverter<FormaTransferencia>))]
public enum FormaTransferencia
{
    [EnumMember(Value = "3")]
    Siloc = 3,

    [EnumMember(Value = "4")]
    DebitoEmConta = 4,

    [EnumMember(Value = "5")]
    Str = 5,

    [EnumMember(Value = "6")]
    RegistroInformacional = 6
}

[JsonConverter(typeof(CodigoEnumJsonConverter<TipoPontoVenda>))]
public enum TipoPontoVenda
{
    [EnumMember(Value = "EC")]
    EstabelecimentoComercial = 1
}

[JsonConverter(typeof(CodigoEnumJsonConverter<TipoValorPagamento>))]
public enum TipoValorPagamento
{
    [EnumMember(Value = "MP")]
    MontantePagamento = 1
}

[JsonConverter(typeof(CodigoEnumJsonConverter<FormaPagamento>))]
public enum FormaPagamento
{
    [EnumMember(Value = "1")]
    TransferenciaEntreContas = 1,

    [EnumMember(Value = "2")]
    DepositoEmConta = 2,

    [EnumMember(Value = "3")]
    Pix = 3,

    [EnumMember(Value = "4")]
    Ted = 4
}

public sealed class CodigoEnumJsonConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private static readonly Dictionary<string, TEnum> CodeToEnum = BuildCodeToEnum();
    private static readonly Dictionary<TEnum, string> EnumToCode = BuildEnumToCode();

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"O valor de {typeof(TEnum).Name} deve ser uma string.");
        }

        var code = reader.GetString();

        if (code is not null && CodeToEnum.TryGetValue(code, out var value))
        {
            return value;
        }

        throw new JsonException($"O valor '{code}' nao eh valido para {typeof(TEnum).Name}.");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (!EnumToCode.TryGetValue(value, out var code))
        {
            throw new JsonException($"Nao foi encontrado um codigo para {typeof(TEnum).Name}.{value}.");
        }

        writer.WriteStringValue(code);
    }

    private static Dictionary<string, TEnum> BuildCodeToEnum()
    {
        var map = new Dictionary<string, TEnum>(StringComparer.Ordinal);

        foreach (var member in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = member.GetCustomAttribute<EnumMemberAttribute>();
            var code = attribute?.Value ?? member.Name;
            map[code] = (TEnum)member.GetValue(null)!;
        }

        return map;
    }

    private static Dictionary<TEnum, string> BuildEnumToCode()
    {
        var map = new Dictionary<TEnum, string>();

        foreach (var member in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = member.GetCustomAttribute<EnumMemberAttribute>();
            var code = attribute?.Value ?? member.Name;
            var value = (TEnum)member.GetValue(null)!;
            map[value] = code;
        }

        return map;
    }
}
