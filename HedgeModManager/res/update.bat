REM 
@echo off
REM Clears the Screen
cls
REM Sets the title
title Updating...
REM 
echo Waiting 3 seconds to ensure process is closed...
REM Sleeps for 3 seconds
powershell start-sleep 3
echo Killing Remaining Processes
REM Kill HedgeModManager if its still running
taskkill /F /IM "HedgeModManager*"
REM 
echo Applying update over the currectly installed version of the application...
REM Copies all new files from the temporary folder
xcopy /s /y ".\updateTemp" ".\" >nul
REM Backup the log file
echo Removing Old Logs
if exist "HedgeModManager_prev.log" (
del HedgeModManager_prev.log
)
echo Backuping up Current Log
if exist "HedgeModManager.log" (
ren HedgeModManager.log HedgeModManager_prev.log
)
echo Cleaning Temp Files
REM Deletes the updatetemp folder
rmdir /S /Q ".\updateTemp"
REM Sleeps for 1 second
powershell start-sleep 1
REM 
echo Done, Starting application...
REM Starts the Mod Manager without waiting
start HedgeModManager.exe
REM deletes the current script
del update.bat >nul