# 🔐 Certificados Digitais – SLC (Núclea)

Neste primeiro momento, em função das adequações regulatórias relacionadas à Resolução 522, este repositório tem como objetivo consolidar as orientações recebidas da Núclea referentes à emissão e utilização de certificados digitais no contexto de integração com o SLC, incluindo o script (“robozinho”) desenvolvido pela própria Núclea para geração dos certificados.

O material original foi consolidado na pasta **SelfSigned/docs**, e o conteúdo do script foi materializado no arquivo **GerarCertificado.bat**.

Para executar corretamente o script de geração de certificado, é necessário que o OpenSSL esteja instalado e configurado no ambiente Windows. Siga atentamente os passos abaixo.

## 1. Verificar se o OpenSSL está instalado

Abra o Prompt de Comando ou PowerShell.
Execute o comando:

```
openssl version
```

**Resultado esperado:**

* ✅ Se o comando retornar a versão do OpenSSL, o software já está instalado e configurado.
  → Prossiga para execução do script.
* ❌ Se aparecer a mensagem *“'openssl' não é reconhecido como um comando interno ou externo”*, o OpenSSL não está instalado ou não está no PATH.
  → Continue para a seção de instalação.

## 2. Instalar o OpenSSL no Windows

### 2.1 Download do OpenSSL

Acesse o site oficial:
[https://slproweb.com/products](https://slproweb.com/products)

### 2.2 Instalação

* Execute o instalador baixado
* Mantenha o caminho padrão:
  `C:\Program Files\OpenSSL-Win64`
* Conclua a instalação

### 2.3 Configurar variável de ambiente (PATH)

Adicionar:

```
C:\Program Files\OpenSSL-Win64\bin
```

Depois:

* Feche e reabra o terminal
* Valide novamente com:

```
openssl version
```

## 3. Execução do Script

Execute o arquivo:

```
GerarCertificado.bat
```

Durante a execução, o script irá solicitar o preenchimento de informações necessárias para geração do certificado.

📌 **Orientação:**
Preencha os campos conforme os prompts exibidos no terminal, respeitando os padrões definidos pela Núclea (ex.: CN, OU, ambiente, etc.).