using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

namespace SLCSettlementAPI;

public interface IJwsSignatureProvider
{
    string GenerateDetachedSignature(string jsonPayload, string requestId, string dataReferencia);
    X509Certificate2 GetClientCertificate();
}

/// <summary>
/// Jws em concordância com a RFC-7515
/// </summary>
/// <remarks>
/// https://www.rfc-editor.org/rfc/rfc7515.html
/// </remarks>
public class JwsSignatureProvider : IJwsSignatureProvider
{
    private readonly NucleaSettings _settings;
    private readonly X509Certificate2 _certificate;

    public JwsSignatureProvider(IOptions<NucleaSettings> settings)
    {
        _settings = settings.Value;

        // Carrega o certificado uma única vez (Singleton recommended)
        // Em produção, considere carregar do Azure KeyVault ou Windows Store
        _certificate = new X509Certificate2(_settings.CertificatePath, _settings.CertificatePassword);
    }

    public X509Certificate2 GetClientCertificate() => _certificate;

    public string GenerateDetachedSignature(string jsonPayload, string requestId, string dataReferencia)
    {
        // 1. Montar o JOSE Header com as Claims Privadas Obrigatórias 
        var headerData = new Dictionary<string, object>
        {
            { "alg", "RS256" },
            { "x5t#S256", GetCertificateThumbprintSha256() },
            { "kid", GetCertificateSerialNumberHex() },
            { "http://www.cip-bancos.org.br/data-referencia", dataReferencia },
            { "http://www.cip-bancos.org.br/identificador-requisicao", requestId },
            { "http://www.cip-bancos.org.br/identificador-emissor-principal", _settings.IspbPrincipal },
            { "http://www.cip-bancos.org.br/identificador-emissor-administrado", _settings.IspbPrincipal }
        };

        var jsonHeader = JsonSerializer.Serialize(headerData);

        // 2. Codificar Header e Payload em Base64Url
        var protectedHeader = Base64UrlEncode(Encoding.UTF8.GetBytes(jsonHeader));

        // O Node usa payload vazio para o endpoint de ECO. 
        // Se jsonPayload for null ou vazio, o encodedPayload será uma string vazia.
        var encodedPayload = string.IsNullOrEmpty(jsonPayload)
            ? "" : Base64UrlEncode(Encoding.UTF8.GetBytes(jsonPayload));

        // 3. Assinar (Header + "." + Body) - Se Body for vazio, fica "Header."
        var stringToSign = $"{protectedHeader}.{encodedPayload}";

        using var rsa = _certificate.GetRSAPrivateKey();

        if (rsa == null) throw new InvalidOperationException("Certificado sem chave privada.");

        var signatureBytes = rsa.SignData(
            Encoding.UTF8.GetBytes(stringToSign),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        var encodedSignature = Base64UrlEncode(signatureBytes);

        // 4. Retornar formato Detached (Header + ".." + Signature)
        return $"{protectedHeader}..{encodedSignature}";
    }

    private string GetCertificateThumbprintSha256()
    {
        var hash = SHA256.HashData(_certificate.RawData);

        return Base64UrlEncode(hash);
    }

    private string GetCertificateSerialNumberHex()
    {
        var serialBytes = _certificate.GetSerialNumber();

        Array.Reverse(serialBytes);

        return Convert.ToHexString(serialBytes).PadLeft(32, '0');
    }

    private static string Base64UrlEncode(byte[] input)
    {
        var output = Convert.ToBase64String(input);

        return output.Split('=')[0].Replace('+', '-').Replace('/', '_');
    }
}