using System.Text.Json.Serialization;

namespace SLCSettlementAPI;

// SLC0913 - Solicitacao Liquidacao Debito
public sealed class SolicitacaoLiquidacaoRequestDebito : SolicitacaoLiquidacaoRequestBase<GrupoCentralizadoraDebito>
{
    [JsonPropertyName("grupoSLC0913Centrlz")]
    public GrupoCentralizadoraDebito GrupoCentralizadora { get; set; } = new();
}
