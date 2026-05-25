# CASigned

Este diretorio contem o fluxo para emissao de certificado via certificadora.

O script `GerarCertificadoCSR.bat` gera apenas:

- `Certnew.key`: chave privada
- `Certnew.csr`: requisicao de assinatura (CSR)
- `GerarCertificadoPEM.bat`: converte o retorno `.p7b` em `.pem`
- `GerarCertificadoPFX.bat`: gera `.pfx` a partir de `.key` e `.pem`

Uso esperado:

1. Execute `GerarCertificadoCSR.bat`.
2. Preencha os dados do subject conforme o padrao da Nuclea.
3. Envie `Certnew.csr` para a certificadora.
4. Guarde `Certnew.key` em ambiente seguro.
5. Receba da certificadora o certificado assinado ou a cadeia em formato `p7b`.
6. Execute `GerarCertificadoPEM.bat` para converter o `p7b` em `pem`.
7. Execute `GerarCertificadoPFX.bat` para gerar o `pfx`, se necessario.

Observacoes:

- O fluxo `CASigned` nao gera `Certnew.cer` autoassinado.
- A validade final do certificado sera definida pela certificadora.
- A chave privada nao deve ser compartilhada junto com o `CSR` ou com o `p7b`.

Guia de instalação do certificado:

https://certificados.serpro.gov.br/instalador/ajuda/html/demo_1.html (Serpro)
