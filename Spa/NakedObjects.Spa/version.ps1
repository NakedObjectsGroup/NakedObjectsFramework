[xml]$xmlDoc = Get-Content   ..\..\NakedObjects.Spa.package\NakedObjects.Spa.nuspec
$ver = $xmlDoc.package.metadata.version

Set-Content ..\Scripts\nakedobjects.version.ts "module NakedObjects { export const version = `"$ver`" }"


