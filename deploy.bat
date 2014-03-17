@echo off

SET DIR=%~d0%~p0%
SET SOURCEDIR=%DIR%\src
SET BINARYDIR="%DIR%build_output"
SET DEPLOYDIR="%DIR%ReleaseBinaries"

IF NOT EXIST %BINARYDIR% (
  mkdir %BINARYDIR%
) ELSE (
  del %BINARYDIR%\* /Q
)

%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %SOURCEDIR%\BellyRub\BellyRub.cspro /property:OutDir=%BINARYDIR%\;Configuration=Release /target:rebuild

IF NOT EXIST %DEPLOYDIR% (
  mkdir %DEPLOYDIR%
) ELSE (
  del %DEPLOYDIR%\* /Q
)

copy %BINARYDIR%\BellyRub.dll %DEPLOYDIR%\
copy %BINARYDIR%\Nancy.dll %DEPLOYDIR%\
copy %BINARYDIR%\Nancy.Hosting.Self.dll %DEPLOYDIR%\
copy %BINARYDIR%\websocket-sharp.dll %DEPLOYDIR%\
copy %BINARYDIR%\Newtonsoft.Json.dll %DEPLOYDIR%\
