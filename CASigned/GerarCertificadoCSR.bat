@echo off
setlocal DisableDelayedExpansion

echo === Gerar key e CSR para emissao via certificadora ===
echo === Os dados do certificado serao solicitados a seguir. ===
echo.

:: --------- Coletar dados do cliente --------- 
set /p "CN=Informe o CN (ex.: teste.hext.com.br): " 
set /p "OU=Informe a OU (Nome Institucional): " 
set /p "ISPB=Informe o numero do ISPB (8 primerios digitos do CNPJ): " 
:: Sigla e codigo que compoem o campo O dinamico (ex.: RRC T001, DUP T001, SCC T001) 
set /p "SIGLA=Informe a sigla (ex.: CCC, CMP, CTC, MCB, PCA, RRC, SCC, SLC, RRC, SEC): " 
set /p "CODIGO=Informe o codigo (ex.: T001 para homologacacao ou P001 para producao): " 
set /p "L=Informe a Cidade (L) [padrao: Sao Paulo]: " 
set /p "ST=Informe o Estado (ST) [padrao: SP]: " 
set /p "C=Informe o Pais (C) [padrao: BR]: " 

:: --------- Defaults se o usuario deixar em branco ---------
if not defined L set "L=Aracaju"
if not defined ST set "ST=SE"
if not defined C set "C=BR"

:: --------- Validacao basica ---------
if not defined CN goto missing_field
if not defined OU goto missing_field
if not defined ISPB goto missing_field
if not defined SIGLA goto missing_field
if not defined CODIGO goto missing_field

:: --------- Perguntar senha da chave (opcional) ---------
echo.
echo [Opcional] Protecao da chave privada com senha
echo - Se desejar criptografar a .key, informe uma senha forte.
echo - Se deixar em branco, a chave sera gerada SEM senha.
set "PASS=__EMPTY_PASS__"
set /p "PASS=Informe a senha da chave (ou deixe em branco): "

:: --------- Sanitizacao simples (remover aspas) ---------
set "CN=%CN:"=%"
set "OU=%OU:"=%"
set "ISPB=%ISPB:"=%"
set "SIGLA=%SIGLA:"=%"
set "CODIGO=%CODIGO:"=%"
set "L=%L:"=%"
set "ST=%ST:"=%"
set "C=%C:"=%"
set "PASS=%PASS:"=%"
if "%PASS%"=="__EMPTY_PASS__" set "PASS="

:: --------- Montar SUBJECT ---------
set "SUBJ=/CN=%CN%/OU=%OU%/OU=%ISPB%/OU=%SIGLA% %CODIGO%/OU=ICP-Brasil/L=%L%/ST=%ST%/C=%C%"

echo.
echo [Resumo]
echo   CN                : %CN%
echo   OU                : %OU%
echo   O (ISPB)          : %ISPB%
echo   O (Sigla+Codigo)  : %SIGLA% %CODIGO%
echo   O                 : ICP-Brasil
echo   L/S/C             : %L% / %ST% / %C%
if defined PASS (
echo   Chave             : SERA GERADA COM SENHA
) else (
echo   Chave             : SERA GERADA SEM SENHA
)
echo   Subject           : %SUBJ%
echo.

echo [AVISO] A chave privada (.key) e confidencial: armazene com seguranca,
echo nunca compartilhe por e-mail/mensageria e mantenha backups criptografados.
echo.

:: -------- Checar OpenSSL no PATH --------
openssl version >nul 2>&1
if errorlevel 1 (
echo [ERRO] OpenSSL nao encontrado no PATH. Verifique a instalacao ou o PATH.
goto end
)

echo [1/2] Gerando chave privada e CSR para envio a certificadora
if defined PASS (
echo Comando: openssl req -new -newkey rsa:2048 -sha256 -keyout Certnew.key -out Certnew.csr -subj "%SUBJ%" -passout pass:********
) else (
echo Comando: openssl req -new -newkey rsa:2048 -sha256 -nodes -keyout Certnew.key -out Certnew.csr -subj "%SUBJ%"
)
pause
if defined PASS (
openssl req -new -newkey rsa:2048 -sha256 -keyout Certnew.key -out Certnew.csr -subj "%SUBJ%" -passout pass:%PASS%
) else (
openssl req -new -newkey rsa:2048 -sha256 -nodes -keyout Certnew.key -out Certnew.csr -subj "%SUBJ%"
)
if errorlevel 1 (
echo [ERRO] Falha ao gerar a chave privada e a CSR.
goto end
) else (
echo [OK] Gerados: Certnew.key e Certnew.csr
)
echo.

echo [2/2] Verificando se a key e a CSR correspondem (comparando modulus)...
if defined PASS (
openssl rsa -in Certnew.key -noout -modulus -passin pass:%PASS% | openssl md5 > key.md5
) else (
openssl rsa -in Certnew.key -noout -modulus | openssl md5 > key.md5
)
openssl req -in Certnew.csr -noout -modulus | openssl md5 > csr.md5

set "KMD5="
set "SMD5="
for /f "usebackq delims=" %%i in ("key.md5") do set "KMD5=%%i"
for /f "usebackq delims=" %%i in ("csr.md5") do set "SMD5=%%i"

if /I "%KMD5%"=="%SMD5%" (
echo [OK] Modulus conferem: key == csr
) else (
echo [ALERTA] Modulus NAO conferem. Verifique se os arquivos foram gerados com a mesma chave.
)

del /q key.md5 csr.md5 >nul 2>&1
echo.
echo ===== Artefatos =====
if exist Certnew.key echo    Chave privada: Certnew.key
if exist Certnew.csr echo    CSR          : Certnew.csr
echo ======================
echo.
echo [PROXIMO PASSO] Envie o arquivo Certnew.csr para a certificadora.
echo O retorno esperado para a cadeia/certificado assinado pode vir em formato .p7b.
echo Mantenha a Certnew.key armazenada com seguranca para uso posterior.
echo.
echo Concluido.
goto end

:missing_field
echo.
echo [ERRO] CN, OU, ISPB, SIGLA e CODIGO sao obrigatorios.
echo Execute novamente o script e informe todos os campos.

:end
endlocal
pause