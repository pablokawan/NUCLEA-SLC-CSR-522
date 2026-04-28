# SelfSigned

Este diretorio contem os artefatos de certificado autoassinado e o processo para gerar um arquivo `PFX`.

**Observação importante sobre a senha da `.key`:**
Na versão original do script, senhas contendo `!` podiam ser corrompidas pelo `cmd` por causa de `setlocal enabledelayedexpansion`.
Isso afeta caracteres de exclamação `!`, especialmente quando a senha contém um ou mais `!`.
Exemplo anonimizado: `SenhaForte!123` podia ser gravada sem o `!`.
O ajuste feito no script evita esse problema e preserva a senha exatamente como foi digitada.

## Arquivos atuais

- `Certnew.cer`: certificado
- `Certnew.key`: chave privada
- `Certnew.csr`: CSR
- `Certnew.pfx`: pacote PKCS#12 gerado a partir do `.cer` e `.key`
- `GerarCertificado.bat`: gera `.key`, `.cer` e `.csr`
- `GerarPfx.bat`: gera `.pfx` a partir do `.cer` e `.key`

## Geracao do PFX

Foi gerado o arquivo `Certnew.pfx` usando:

- certificado: `Certnew.cer`
- chave privada: `Certnew.key`
- friendly name: `Certnew`

## Como gerar o PFX com o batch

O `GerarPfx.bat` foi feito para usar automaticamente os arquivos da pasta atual:

- `Certnew.cer`
- `Certnew.key`

Fluxo:

1. Execute:

```bat
GerarPfx.bat
```

2. O script valida se `Certnew.cer` e `Certnew.key` existem na pasta.
3. O proprio `OpenSSL` pede a senha da chave privada, se a `Certnew.key` estiver protegida.
4. O proprio `OpenSSL` pede a senha de protecao do `Certnew.pfx`.
5. O arquivo `Certnew.pfx` e gerado na mesma pasta.

O script nao pede caminho de arquivo nem `friendly name`. Esses valores ja ficam fixos no processo:

- certificado: `Certnew.cer`
- chave: `Certnew.key`
- saida: `Certnew.pfx`
- friendly name: `Certnew`

Observacao:

- se quiser usar a mesma senha para a chave e para o `PFX`, informe a mesma senha nas duas perguntas do `OpenSSL`
- esse formato e `full .bat`, sem chamar `PowerShell`

## Como gerar o PFX manualmente com OpenSSL

```bat
openssl pkcs12 -export ^
  -out Certnew.pfx ^
  -inkey Certnew.key ^
  -in Certnew.cer ^
  -name "Certnew"
```

Nesse modo, o proprio `OpenSSL` vai pedir interativamente:

- a senha da chave privada, se houver
- a senha do `PFX`

## Validacao

Para validar o PFX:

```bat
openssl pkcs12 -in Certnew.pfx -info -noout -passin pass:SENHA
```

Se o comando retornar a estrutura PKCS#12 sem erro, o `PFX` esta consistente.

## Seguranca

- Nao mantenha senha em texto puro por mais tempo do que o necessario.
- Restrinja o acesso a `Certnew.key`, `Certnew.pfx` e arquivos de apoio.
- Prefira armazenar a senha em cofre de segredos quando esse material sair do ambiente local.
