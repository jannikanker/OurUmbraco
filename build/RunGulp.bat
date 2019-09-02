@ECHO OFF
SETLOCAL
	:: SETLOCAL is on, so changes to the path not persist to the actual user's path

SET release=%1
ECHO Installing Npm NuGet Package

SET nuGetFolder=%CD%\..\packages\
ECHO Configured packages folder: %nuGetFolder%
ECHO Current folder: %CD%

%CD%\..\.nuget\NuGet.exe install Npm.js -OutputDirectory %nuGetFolder%  -Verbosity quiet

for /f "delims=" %%A in ('dir %nuGetFolder%node.js.* /b') do set "nodePath=%nuGetFolder%%%A\"
for /f "delims=" %%A in ('dir %nuGetFolder%npm.js.* /b') do set "npmPath=%nuGetFolder%%%A\tools\"

REM Ensures that we look for the just downloaded NPM, not whatever the user has installed on their machine
path=%npmPath%;%nodePath%

ECHO %path%

SET buildFolder=%CD%

ECHO Change directory to %CD%\..\OurUmbraco.Client\
CD %CD%\..\OurUmbraco.Client\

ECHO Do npm install and the gulp build
call npm install
call npm install -g install gulp -g --quiet
call gulp

ECHO Move back to the build folder
CD %buildFolder% 