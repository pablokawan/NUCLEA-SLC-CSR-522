@echo off
setlocal
cd /d "%~dp0"

set "CERT_FILE=Certnew.cer"
set "KEY_FILE=Certnew.key"
set "PFX_FILE=Certnew.pfx"
set "FRIENDLY_NAME=Certnew"

echo === Geracao de PFX ===
echo.

openssl version >nul 2>&1
if errorlevel 1 (
    echo [ERRO] OpenSSL nao encontrado no PATH.
    exit /b 1
)

if not exist "%CERT_FILE%" (
    echo [ERRO] Arquivo nao encontrado: %CERT_FILE%
    exit /b 1
)

if not exist "%KEY_FILE%" (
    echo [ERRO] Arquivo nao encontrado: %KEY_FILE%
    exit /b 1
)

echo Certificado localizado: %CERT_FILE%
echo Chave privada localizada: %KEY_FILE%
echo Saida do PFX: %PFX_FILE%
echo.
echo O OpenSSL vai solicitar:
echo 1. A senha da chave privada, se a .key estiver protegida.
echo 2. A senha do PFX.
echo.
echo Se quiser usar a mesma senha nos dois casos, informe a mesma senha quando solicitado.
echo.

openssl pkcs12 -export -out "%PFX_FILE%" -inkey "%KEY_FILE%" -in "%CERT_FILE%" -name "%FRIENDLY_NAME%"

if errorlevel 1 (
    echo.
    echo [ERRO] Nao foi possivel gerar o PFX.
    exit /b 1
)

echo.
echo [OK] PFX gerado: %PFX_FILE%
echo Concluido.
exit /b 0
