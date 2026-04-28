@echo off

:: Skip if cl.exe is already available
where cl >nul 2>nul && exit /b 0

:: Locate VS 2022 using vswhere
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if exist "%VSWHERE%" (
  for /f "usebackq tokens=*" %%i in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
    set "VSINSTALLDIR=%%i"
    goto :found
  )
)

:: Fallback: Try default install path
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat" (
  set "VSINSTALLDIR=C:\Program Files\Microsoft Visual Studio\2022\Community"
  goto :found
)

echo Error: Visual Studio 2022 with MSBuild not found.
exit /b 1

:found
echo Found Visual Studio at: %VSINSTALLDIR%
call "%VSINSTALLDIR%\Common7\Tools\VsDevCmd.bat"
if errorlevel 1 (
  echo Failed to initialize Developer Command Prompt.
  exit /b 1
)
