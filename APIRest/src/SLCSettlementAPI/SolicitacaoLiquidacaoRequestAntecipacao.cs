using System.Text.Json.Serialization;

namespace SLCSettlementAPI;

// SLC0912 - Solicitacao Liquidacao Antecipacao
public sealed class SolicitacaoLiquidacaoRequestAntecipacao : SolicitacaoLiquidacaoRequestBase<GrupoCentralizadoraAntecipacao>
{
    [JsonPropertyName("grupoSLC0912Centrlz")]
    public GrupoCentralizadoraAntecipacao GrupoCentralizadora { get; set; } = new();
}
