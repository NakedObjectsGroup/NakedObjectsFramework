SET nugetPackagePath=C:\NakedObjectsPackages

dotnet build NakedFramework.ProgrammingModel.sln -c Debug

dotnet pack "Programming Model\NakedFramework\NakedFramework.ProgrammingModel.Package\NakedFramework.ProgrammingModel.Package.csproj"  --include-symbols --include-source

XCOPY ".\Programming Model\NakedFramework\NakedFramework.ProgrammingModel.Package\bin\Debug\*.nupkg" %nugetPackagePath% /y

dotnet build NakedObjects.ProgrammingModel.sln -c Debug
dotnet build NakedFunctions.ProgrammingModel.sln -c Debug

dotnet pack "Programming Model\NakedObjects\NakedObjects.ProgrammingModel.Package\NakedObjects.ProgrammingModel.Package.csproj"  --include-symbols --include-source
dotnet pack "Programming Model\NakedFunctions\NakedFunctions.ProgrammingModel.Package\NakedFunctions.ProgrammingModel.Package.csproj"  --include-symbols --include-source

XCOPY ".\Programming Model\NakedObjects\NakedObjects.ProgrammingModel.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
XCOPY ".\Programming Model\NakedFunctions\NakedFunctions.ProgrammingModel.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y

dotnet build NakedObjects.Server.sln -c Debug
dotnet build NakedFunctions.Server.sln -c Debug

dotnet pack NakedObjects\NakedObjects.Server.Package\NakedObjects.Server.Package.csproj --include-symbols --include-source
dotnet pack NakedFunctions\NakedFunctions.Server.Package\NakedFunctions.Server.Package.csproj --include-symbols --include-source

XCOPY ".\NakedObjects\NakedObjects.Server.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
XCOPY ".\NakedFunctions\NakedFunctions.Server.Package\bin\Debug\*.nupkg"  %nugetPackagePath% /y
