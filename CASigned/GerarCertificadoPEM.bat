@echo off
setlocal DisableDelayedExpansion
cd /d "%~dp0"

echo === Gerar PEM a partir de arquivo P7B ===
echo === Os arquivos do certificado serao solicitados a seguir. ===
echo.

set /p "P7B_FILE=Informe o arquivo .p7b de entrada: "
set "PEM_FILE=__DEFAULT_PEM__"
set /p "PEM_FILE=Informe o nome do arquivo .pem de saida [padrao: mesmo nome do .p7b]: "

set "P7B_FILE=%P7B_FILE:"=%"
set "PEM_FILE=%PEM_FILE:"=%"

if not defined P7B_FILE goto missing_field
if "%PEM_FILE%"=="__DEFAULT_PEM__" set "PEM_FILE="

if not exist "%P7B_FILE%" (
echo [ERRO] Arquivo .p7b nao encontrado: %P7B_FILE%
goto end
)

if not defined PEM_FILE (
for %%I in ("%P7B_FILE%") do set "PEM_FILE=%%~nI.pem"
)

echo.
echo [Resumo]
echo   Arquivo P7B        : %P7B_FILE%
echo   Arquivo PEM        : %PEM_FILE%
echo.

openssl version >nul 2>&1
if errorlevel 1 (
echo [ERRO] OpenSSL nao encontrado no PATH. Verifique a instalacao ou o PATH.
goto end
)

echo [1/1] Convertendo P7B em PEM
echo Comando: openssl pkcs7 -print_certs -in "%P7B_FILE%" -out "%PEM_FILE%"
pause

openssl pkcs7 -print_certs -in "%P7B_FILE%" -out "%PEM_FILE%"

if errorlevel 1 (
echo [ERRO] Falha ao gerar o arquivo PEM.
goto end
) else (
echo [OK] PEM gerado: %PEM_FILE%
)

echo.
echo ===== Artefatos =====
if exist "%P7B_FILE%" echo    Entrada: %P7B_FILE%
if exist "%PEM_FILE%" echo    Saida  : %PEM_FILE%
echo ======================
echo.
echo Concluido.
goto end

:missing_field
echo.
echo [ERRO] O arquivo .p7b de entrada e obrigatorio.
echo Execute novamente o script e informe o arquivo corretamente.

:end
endlocal
pause
