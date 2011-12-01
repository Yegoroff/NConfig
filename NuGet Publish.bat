@set NUGET="Lib\NuGet.CommandLine.1.5.21005.9019\tools\NuGet.exe"

@echo ==========================
@echo NuGet package publishing.
@%NUGET% Push NuGet\NConfig.1.0.7.nupkg
@if not errorlevel 0 goto error

@echo NConfig publishing sucessfull.
@goto end

:error
@echo Error occured during NConfig publishing.
@pause

:end
@pause