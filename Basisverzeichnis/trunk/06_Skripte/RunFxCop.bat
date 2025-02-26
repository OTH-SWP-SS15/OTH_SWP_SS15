@echo off
SETLOCAL EnableDelayedExpansion

call "%~dp0\SetEnv.bat" > NUL

REM ===========================================================================================
REM Script description
REM ===========================================================================================
set "-Script.Name=%~nx0"
set "-Script.Path=%~dp0"
set "-Script.Usage="^
Usage for !-Script.Name!: ^
Wrapper for FxCopCmd ^(static code analysis^) application.^
""

set "!-Script.Name!.Errorlevel=0"

REM configure ParseArguments
set "OptionDefaults=-target:"" -outDir:"""


REM ===========================================================================================
REM configure ParseOptions.bat GUI interface
REM ===========================================================================================
if not defined -target.Usage              set "-target.Usage="Select target *.dll/*.exe which should be analyzed.""
if not defined -target.Necessity          set "-target.Necessity="Required""
if not defined -target.Multiplicity       set "-target.Multiplicity="Single""
if not defined -target.GuiEntryType       set "-target.GuiEntryType="File""
if not defined -target.GuiDefaultValues   set "-target.GuiDefaultValues="%SWP_BUILD_ROOT%/bin""

if not defined -outDir.Usage              set "-outDir.Usage="Select root out directory for xml file. This directory will be expanded by target name.""
if not defined -outDir.Necessity          set "-outDir.Necessity="Required""
if not defined -outDir.GuiEntryType       set "-outDir.GuiEntryType="Folder""
if not defined -outDir.Multiplicity       set "-outDir.Multiplicity="Single""
if not defined -outDir.GuiDefaultValues   set "-outDir.GuiDefaultValues="%SWP_BUILD_ROOT%/StaticCodeAnalysis/FxCop""

call %SWP_PARSEARGUMENTS_GUI_BAT% %*
if errorlevel 1 exit /b %ERRORLEVEL%

rem output selected options
echo !-Script.Name!
for %%A in (%OptionDefaults%) do for /f "tokens=1,* delims=:" %%B in ("%%A") do (
  set name=!%%B!
  if /I "!%%B!" NEQ "" echo %%B=!name!
)
echo.

REM get plain target name
for %%F in ("!-target!") do set "targetName=%%~nF"

if not exist "!-outDir!" (
  echo Create !-outDir!.
  mkdir "!-outDir!"
)


REM create outFile for static code analysis result
set -outFile=!-outDir!/%SWP_LOCALTIME_DATESTAMP%_!targetName!.xml

echo Call: %SWP_FXCOP_CMD_EXE% /out:!-outFile! /file:!-target!
call %SWP_FXCOP_CMD_EXE% /out:!-outFile! /file:!-target!
if errorlevel 1 ( 
  echo %SWP_FXCOP_CMD_EXE% failed.
  set !-Script.Name!.Errorlevel=100
) else (
  echo %SWP_FXCOP_CMD_EXE% succedded.
  
)

IF DEFINED CALLED_FROM_EXPLORER pause
exit /b !%-Script.Name%.Errorlevel!
