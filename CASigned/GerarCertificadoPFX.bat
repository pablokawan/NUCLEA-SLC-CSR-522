@echo off
setlocal DisableDelayedExpansion
cd /d "%~dp0"

echo === Gerar PFX a partir de KEY e PEM ===
echo === Os arquivos do certificado serao solicitados a seguir. ===
echo.

set /p "KEY_FILE=Informe o arquivo .key de entrada: "
set /p "PEM_FILE=Informe o arquivo .pem de entrada: "
set "PFX_FILE=__DEFAULT_PFX__"
set /p "PFX_FILE=Informe o nome do arquivo .pfx de saida [padrao: mesmo nome da .key]: "
set "FRIENDLY_NAME=__DEFAULT_NAME__"
set /p "FRIENDLY_NAME=Informe o friendly name [padrao: mesmo nome da .key]: "

set "KEY_FILE=%KEY_FILE:"=%"
set "PEM_FILE=%PEM_FILE:"=%"
set "PFX_FILE=%PFX_FILE:"=%"
set "FRIENDLY_NAME=%FRIENDLY_NAME:"=%"

if not defined KEY_FILE goto missing_field
if not defined PEM_FILE goto missing_field
if "%PFX_FILE%"=="__DEFAULT_PFX__" set "PFX_FILE="
if "%FRIENDLY_NAME%"=="__DEFAULT_NAME__" set "FRIENDLY_NAME="

if not exist "%KEY_FILE%" (
echo [ERRO] Arquivo .key nao encontrado: %KEY_FILE%
goto end
)

if not exist "%PEM_FILE%" (
echo [ERRO] Arquivo .pem nao encontrado: %PEM_FILE%
goto end
)

if not defined PFX_FILE (
for %%I in ("%KEY_FILE%") do set "PFX_FILE=%%~nI.pfx"
)

if not defined FRIENDLY_NAME (
for %%I in ("%KEY_FILE%") do set "FRIENDLY_NAME=%%~nI"
)

echo.
echo [Resumo]
echo   Arquivo KEY        : %KEY_FILE%
echo   Arquivo PEM        : %PEM_FILE%
echo   Arquivo PFX        : %PFX_FILE%
echo   Friendly Name      : %FRIENDLY_NAME%
echo.

openssl version >nul 2>&1
if errorlevel 1 (
echo [ERRO] OpenSSL nao encontrado no PATH. Verifique a instalacao ou o PATH.
goto end
)

echo [1/1] Gerando PFX a partir de KEY e PEM
echo Comando: openssl pkcs12 -export -out "%PFX_FILE%" -inkey "%KEY_FILE%" -in "%PEM_FILE%" -name "%FRIENDLY_NAME%"
echo.
echo O OpenSSL vai solicitar:
echo 1. A senha da chave privada, se a .key estiver protegida.
echo 2. A senha do PFX.
echo.
pause

openssl pkcs12 -export -out "%PFX_FILE%" -inkey "%KEY_FILE%" -in "%PEM_FILE%" -name "%FRIENDLY_NAME%"

if errorlevel 1 (
echo [ERRO] Falha ao gerar o arquivo PFX.
goto end
) else (
echo [OK] PFX gerado: %PFX_FILE%
)

echo.
echo ===== Artefatos =====
if exist "%KEY_FILE%" echo    Chave privada: %KEY_FILE%
if exist "%PEM_FILE%" echo    Certificado  : %PEM_FILE%
if exist "%PFX_FILE%" echo    PFX          : %PFX_FILE%
echo ======================
echo.
echo [IMPORTANTE] A chave privada (.key) e o arquivo .pfx devem ser armazenados em ambiente seguro.
echo Concluido.
goto end

:missing_field
echo.
echo [ERRO] Os arquivos .key e .pem de entrada sao obrigatorios.
echo Execute novamente o script e informe os arquivos corretamente.

:end
endlocal
pause
