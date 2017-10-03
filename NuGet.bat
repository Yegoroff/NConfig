@set DEVENV="%programfiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe"

@set NUGET="Lib\NuGet.CommandLine.1.6.0\tools\NuGet.exe"

@echo ==========================
@echo Building NConfig.
@rmdir bin /s /q
%DEVENV% NConfig.sln /build Release
@if errorlevel 1 goto error

@echo ==========================
@echo Copying NConfig assemblies.
@rmdir NuGet\lib /s /q
@mkdir NuGet\lib
xcopy bin\Release NuGet\lib /s /y
@if errorlevel 0 goto nuget
@goto error

:nuget
@echo ==========================
@echo NuGet package creation.
@%NUGET% pack NuGet\nconfig.nuspec -basePath NuGet -o NuGet
@if not errorlevel 0 goto error

@echo NConfig build sucessfull.
@goto end

:error
@echo Error occured during NConfig build.

:end
@pause