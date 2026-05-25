namespace SLCSettlementAPI;

public static class SolicitacaoLiquidacaoValidator
{
    public static string? ValidarCredito(SolicitacaoLiquidacaoRequest request)
    {
        return ValidarPontosVenda(
            request.GrupoCentralizadora.GrupoPontoVenda,
            permiteRegistroInformacional: false,
            operacao: "credito",
            produtosPermitidos: new HashSet<TipoProdutoLiquidacao>
            {
                TipoProdutoLiquidacao.CartaoCredito,
                TipoProdutoLiquidacao.AjusteCredito
            });
    }

    public static string? ValidarDebito(SolicitacaoLiquidacaoRequestDebito request)
    {
        return ValidarPontosVenda(
            request.GrupoCentralizadora.GrupoPontoVenda,
            permiteRegistroInformacional: false,
            operacao: "debito",
            produtosPermitidos: new HashSet<TipoProdutoLiquidacao>
            {
                TipoProdutoLiquidacao.CartaoDebito
            });
    }

    public static string? ValidarAntecipacao(SolicitacaoLiquidacaoRequestAntecipacao request)
    {
        return ValidarPontosVenda(
            request.GrupoCentralizadora.GrupoPontoVenda,
            permiteRegistroInformacional: true,
            operacao: "antecipacao",
            produtosPermitidos: new HashSet<TipoProdutoLiquidacao>
            {
                TipoProdutoLiquidacao.CartaoCredito,
                TipoProdutoLiquidacao.AjusteCredito
            });
    }

    private static string? ValidarPontosVenda(
        IReadOnlyList<GrupoPontoVenda> pontosVenda,
        bool permiteRegistroInformacional,
        string operacao,
        IReadOnlySet<TipoProdutoLiquidacao> produtosPermitidos)
    {
        if (pontosVenda.Count == 0)
        {
            return "Informe ao menos um ponto de venda na solicitacao.";
        }

        if (!permiteRegistroInformacional && pontosVenda.Any(pv => pv.IndicadorFormaTransferencia == FormaTransferencia.RegistroInformacional))
        {
            return $"A forma de transferencia informacional so eh permitida para antecipacao. Ajuste a solicitacao de {operacao}.";
        }

        var pontoVendaComProdutoInvalido = pontosVenda.FirstOrDefault(pv => !produtosPermitidos.Contains(pv.TipoProdutoLiquidacao));
        if (pontoVendaComProdutoInvalido is not null)
        {
            return $"O produto de liquidacao informado para o ponto de venda '{pontoVendaComProdutoInvalido.CodigoPontoVenda}' nao eh compativel com a operacao de {operacao}.";
        }

        return null;
    }
}
