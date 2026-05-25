using Microsoft.OpenApi.Any;
using System.Text.Json;

namespace SLCSettlementAPI;

public static class SolicitacaoLiquidacaoExamples
{
    public const string CreditoVisaSilocJson = """
    {
      "cnpjBaseCreddr": "10773370",
      "cnpjCreddr": "10773370000115",
      "ispbIfDevdr": "54403563",
      "ispbIfCredr": "54403563",
      "agCreddr": "0001",
      "ctCreddr": 12345670,
      "nomCreddr": "SERVELOJA INSTITUICAO DE PAGAMENTO LTDA",
      "grupoSLC0914Centrlz": {
        "numCtrlCreddrCentrlz": "20260508000000000001",
        "tpPessoaCentrlz": "J",
        "cnpjCpfCentrlz": "10773370000115",
        "codCentrlz": "00000000000000048927",
        "tpCt": "CC",
        "agCentrlz": "1234",
        "ctCentrlz": 12345670,
        "ctPgtoCentrlz": null,
        "grupoSLC0914PontoVenda": [
          {
            "numCtrlCreddrPontoVenda": "20260508000000000002",
            "ispbIfLiquidPontoVenda": "54403563",
            "codPontoVenda": "2026050800000000000048927",
            "nomePontoVenda": "ESTABELECIMENTO VISA CREDITO",
            "tpPessoaPontoVenda": "J",
            "cnpjCpfPontoVenda": "10773370000115",
            "codInstitdrArrajPgto": "004",
            "tpProdLiquidCarts": "01",
            "indrFormaTransf": "3",
            "codMoeda": "001",
            "tpPontoVenda": "EC",
            "tpVlrPgto": "MP",
            "dtPgto": "2026-05-08",
            "vlrPgto": 1.99,
            "formaPgto": null,
            "numCtrlPgto": null
          }
        ]
      }
    }
    """;

    public const string DebitoMaestroSilocJson = """
    {
      "cnpjBaseCreddr": "10773370",
      "cnpjCreddr": "10773370000115",
      "ispbIfDevdr": "54403563",
      "ispbIfCredr": "54403563",
      "agCreddr": "0001",
      "ctCreddr": 12345670,
      "nomCreddr": "SERVELOJA INSTITUICAO DE PAGAMENTO LTDA",
      "grupoSLC0913Centrlz": {
        "numCtrlCreddrCentrlz": "20260508000000000003",
        "tpPessoaCentrlz": "J",
        "cnpjCpfCentrlz": "10773370000115",
        "codCentrlz": "00000000000000048927",
        "tpCt": "CC",
        "agCentrlz": "1234",
        "ctCentrlz": 12345670,
        "ctPgtoCentrlz": null,
        "grupoSLC0913PontoVenda": [
          {
            "numCtrlCreddrPontoVenda": "20260508000000000004",
            "ispbIfLiquidPontoVenda": "54403563",
            "codPontoVenda": "2026050800000000000048928",
            "nomePontoVenda": "ESTABELECIMENTO MAESTRO DEBITO",
            "tpPessoaPontoVenda": "J",
            "cnpjCpfPontoVenda": "10773370000115",
            "codInstitdrArrajPgto": "025",
            "tpProdLiquidCarts": "03",
            "indrFormaTransf": "3",
            "codMoeda": "001",
            "tpPontoVenda": "EC",
            "tpVlrPgto": "MP",
            "dtPgto": "2026-05-08",
            "vlrPgto": 1.88,
            "formaPgto": null,
            "numCtrlPgto": null
          }
        ]
      }
    }
    """;

    public const string AntecipacaoVisaInformacionalJson = """
    {
      "cnpjBaseCreddr": "10773370",
      "cnpjCreddr": "10773370000115",
      "ispbIfDevdr": "54403563",
      "ispbIfCredr": "23527532",
      "agCreddr": "0001",
      "ctCreddr": 12345670,
      "nomCreddr": "SERVELOJA INSTITUICAO DE PAGAMENTO LTDA",
      "grupoSLC0912Centrlz": {
        "numCtrlCreddrCentrlz": "20260508000000000005",
        "tpPessoaCentrlz": "J",
        "cnpjCpfCentrlz": "10773370000115",
        "codCentrlz": "00000000000000048927",
        "tpCt": "CC",
        "agCentrlz": "1234",
        "ctCentrlz": 12345670,
        "ctPgtoCentrlz": null,
        "grupoSLC0912PontoVenda": [
          {
            "numCtrlCreddrPontoVenda": "20260508000000000006",
            "ispbIfLiquidPontoVenda": "54403563",
            "codPontoVenda": "2026050800000000000048929",
            "nomePontoVenda": "ESTABELECIMENTO VISA ANTECIPACAO",
            "tpPessoaPontoVenda": "J",
            "cnpjCpfPontoVenda": "10773370000115",
            "codInstitdrArrajPgto": "004",
            "tpProdLiquidCarts": "01",
            "indrFormaTransf": "6",
            "codMoeda": "001",
            "tpPontoVenda": "EC",
            "tpVlrPgto": "MP",
            "dtPgto": "2026-05-08",
            "vlrPgto": 1.93,
            "formaPgto": "3",
            "numCtrlPgto": "PIX202605080000000000000000000000001"
          }
        ]
      }
    }
    """;
}

public static class OpenApiExampleParser
{
    public static IOpenApiAny Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return ToOpenApiAny(document.RootElement);
    }

    private static IOpenApiAny ToOpenApiAny(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => ToOpenApiObject(element),
            JsonValueKind.Array => ToOpenApiArray(element),
            JsonValueKind.String => new OpenApiString(element.GetString()),
            JsonValueKind.Number when element.TryGetInt64(out var longValue) => new OpenApiLong(longValue),
            JsonValueKind.Number => new OpenApiDouble(element.GetDouble()),
            JsonValueKind.True => new OpenApiBoolean(true),
            JsonValueKind.False => new OpenApiBoolean(false),
            JsonValueKind.Null => new OpenApiNull(),
            _ => new OpenApiNull()
        };
    }

    private static OpenApiObject ToOpenApiObject(JsonElement element)
    {
        var value = new OpenApiObject();

        foreach (var property in element.EnumerateObject())
        {
            value[property.Name] = ToOpenApiAny(property.Value);
        }

        return value;
    }

    private static OpenApiArray ToOpenApiArray(JsonElement element)
    {
        var value = new OpenApiArray();

        foreach (var item in element.EnumerateArray())
        {
            value.Add(ToOpenApiAny(item));
        }

        return value;
    }
}
