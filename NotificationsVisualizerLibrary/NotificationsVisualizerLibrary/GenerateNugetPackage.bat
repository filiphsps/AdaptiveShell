@echo off

set files=bin\Release\NotificationsVisualizerLibrary.winmd
set files=%files% bin\Release\NotificationsVisualizerLibrary.pri
set files=%files% bin\Release\NotificationsVisualizerLibrary.pdb
set files=%files% bin\Release\NotificationsVisualizerLibrary.xml
set files=%files% bin\Release\NotificationsVisualizerLibrary.xr.xml

FOR %%f IN (%files%) DO IF NOT EXIST %%f call :file_not_found %%f


echo Here are the current timestamps on the DLL's...
echo.

FOR %%f IN (%files%) DO ECHO %%~tf %%f

echo.

PAUSE



echo Welcome, let's create a new NuGet package for NotificationsVisualizerLibrary!
echo.

set /p version="Enter Version Number (ex. 1.0.0): "

if not exist "NugetPackages" mkdir "NugetPackages"

"C:\Program Files (x86)\NuGet\nuget.exe" pack NotificationsVisualizerLibrary.nuspec -Version %version% -OutputDirectory "NugetPackages"

PAUSE

explorer NugetPackages




exit
:file_not_found

echo File not found: %1
PAUSE
exit