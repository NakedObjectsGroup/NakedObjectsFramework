dotnet build NakedObjects.ProgrammingModel.sln -c Debug
dotnet build NakedFunctions.ProgrammingModel.sln -c Debug

dotnet pack "Programming Model\NakedObjects\NakedObjects.ProgrammingModel.Package\NakedObjects.ProgrammingModel.Package.csproj"  --include-symbols --include-source

dotnet pack "Programming Model\NakedFunctions\NakedFunctions.ProgrammingModel.Package\NakedFunctions.ProgrammingModel.Package.csproj"  --include-symbols --include-source

dotnet build NakedObjects.Server.sln -c Debug -p:DefineConstants=APPVEYOR
dotnet build NakedFunctions.Server.sln -c Debug -p:DefineConstants=APPVEYOR

dotnet pack NakedObjects\NakedObjects.Server.Package\NakedObjects.Server.Package.csproj --include-symbols --include-source

dotnet pack NakedFunctions\NakedFunctions.Server.Package\NakedFunctions.Server.Package.csproj --include-symbols --include-source