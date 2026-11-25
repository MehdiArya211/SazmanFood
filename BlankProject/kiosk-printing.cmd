@echo off
setlocal

rem Close all existing Chrome windows before running this launcher.
rem Set the thermal printer as the Windows default printer first.
rem Optional argument: the kiosk login URL for this device.
set "KIOSK_URL=%~1"
if not defined KIOSK_URL set "KIOSK_URL=http://localhost:44340/Authentication/IndexZP"

set "CHROME_EXE=%ProgramFiles%\Google\Chrome\Application\chrome.exe"
if not exist "%CHROME_EXE%" set "CHROME_EXE=%ProgramFiles(x86)%\Google\Chrome\Application\chrome.exe"
if not exist "%CHROME_EXE%" set "CHROME_EXE=%LocalAppData%\Google\Chrome\Application\chrome.exe"
if not exist "%CHROME_EXE%" (
    echo Chrome was not found. Install Google Chrome on this kiosk.
    pause
    exit /b 1
)

start "" "%CHROME_EXE%" --kiosk --kiosk-printing "%KIOSK_URL%"
endlocal
