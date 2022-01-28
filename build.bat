SET nugetPackagePath=C:\NakedObjectsPackages

DEL ".\Programming Model\NakedFramework\NakedFramework.ProgrammingModel.Package\bin\Debug\*.nupkg"
DEL ".\Programming Model\NakedObjects\NakedObjects.ProgrammingModel.Package\bin\Debug\*.nupkg"
DEL ".\Programming Model\NakedFunctions\NakedFunctions.ProgrammingModel.Package\bin\Debug\*.nupkg"
DEL ".\Programming Model\NOF2\NOF2.ProgrammingModel.Package\bin\Debug\*.nupkg"

DEL ".\NakedFramework\NakedFramework.Server.Package\bin\Debug\*.nupkg"
DEL ".\NakedObjects\NakedObjects.Reflector.Package\bin\Debug\*.nupkg"
DEL ".\NakedFunctions\NakedFunctions.Reflector.Package\bin\Debug\*.nupkg"
DEL ".\NOF2\NOF2.Reflector.Package\bin\Debug\*.nupkg"

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

dotnet build NakedFramework.Server.sln -c Debug

dotnet pack NakedFramework\NakedFramework.Server.Package\NakedFramework.Server.Package.csproj --include-symbols --include-source

XCOPY ".\NakedFramework\NakedFramework.Server.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y

dotnet build NakedObjects.Reflector.sln -c Debug
dotnet build NakedFunctions.Reflector.sln -c Debug
dotnet build NOF2.Reflector.sln -c Debug

dotnet pack NakedObjects\NakedObjects.Reflector.Package\NakedObjects.Reflector.Package.csproj --include-symbols --include-source
dotnet pack NakedFunctions\NakedFunctions.Reflector.Package\NakedFunctions.Reflector.Package.csproj --include-symbols --include-source
dotnet pack NOF2\NOF2.Reflector.Package\NOF2.Reflector.Package.csproj --include-symbols --include-source

XCOPY ".\NakedObjects\NakedObjects.Reflector.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
XCOPY ".\NakedFunctions\NakedFunctions.Reflector.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
XCOPY ".\NOF2\NOF2.Reflector.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y