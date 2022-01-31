SET nugetPackagePath=C:\NakedObjectsPackages

DEL ".\Programming Model\NakedFramework\NakedFramework.ProgrammingModel.Package\bin\Debug\*.nupkg"
DEL ".\Programming Model\NakedObjects\NakedObjects.ProgrammingModel.Package\bin\Debug\*.nupkg"
DEL ".\Programming Model\NakedFunctions\NakedFunctions.ProgrammingModel.Package\bin\Debug\*.nupkg"
DEL ".\Programming Model\NOF2\NOF2.ProgrammingModel.Package\bin\Debug\*.nupkg"

DEL ".\NakedFramework\NakedFramework.Package\bin\Debug\*.nupkg"
DEL ".\NakedObjects\NakedObjects.Server.Package\bin\Debug\*.nupkg"
DEL ".\NakedFunctions\NakedFunctions.Server.Package\bin\Debug\*.nupkg"
DEL ".\NOF2\NOF2.Server.Package\bin\Debug\*.nupkg"

dotnet build NakedFramework.ProgrammingModel.sln -c Debug

dotnet pack "Programming Model\NakedFramework\NakedFramework.ProgrammingModel.Package\NakedFramework.ProgrammingModel.Package.csproj"  --include-symbols --include-source

XCOPY ".\Programming Model\NakedFramework\NakedFramework.ProgrammingModel.Package\bin\Debug\*.nupkg" %nugetPackagePath% /y

dotnet build NakedObjects.ProgrammingModel.sln -c Debug
dotnet build NakedFunctions.ProgrammingModel.sln -c Debug
dotnet build NOF2.ProgrammingModel.sln -c Debug

dotnet pack "Programming Model\NakedObjects\NakedObjects.ProgrammingModel.Package\NakedObjects.ProgrammingModel.Package.csproj"  --include-symbols --include-source
dotnet pack "Programming Model\NakedFunctions\NakedFunctions.ProgrammingModel.Package\NakedFunctions.ProgrammingModel.Package.csproj"  --include-symbols --include-source
dotnet pack "Programming Model\NOF2\NOF2.ProgrammingModel.Package\NOF2.ProgrammingModel.Package.csproj"  --include-symbols --include-source

XCOPY ".\Programming Model\NakedObjects\NakedObjects.ProgrammingModel.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
XCOPY ".\Programming Model\NakedFunctions\NakedFunctions.ProgrammingModel.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
XCOPY ".\Programming Model\NOF2\NOF2.ProgrammingModel.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y

dotnet build NakedFramework.sln -c Debug

dotnet pack NakedFramework\NakedFramework.Package\NakedFramework.Package.csproj --include-symbols --include-source

XCOPY ".\NakedFramework\NakedFramework.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y

dotnet build NakedObjects.Server.sln -c Debug
dotnet build NakedFunctions.Server.sln -c Debug
dotnet build NOF2.Server.sln -c Debug

dotnet pack NakedObjects\NakedObjects.Server.Package\NakedObjects.Server.Package.csproj --include-symbols --include-source
dotnet pack NakedFunctions\NakedFunctions.Server.Package\NakedFunctions.Server.Package.csproj --include-symbols --include-source
dotnet pack NOF2\NOF2.Server.Package\NOF2.Server.Package.csproj --include-symbols --include-source

XCOPY ".\NakedObjects\NakedObjects.Server.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
XCOPY ".\NakedFunctions\NakedFunctions.Server.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
XCOPY ".\NOF2\NOF2.Server.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y