[xml]$xmlDoc = Get-Content   ..\..\NakedObjects.Spa.package\NakedObjects.Spa.nuspec
$ver = $xmlDoc.package.metadata.version

$content = Get-Content ..\Scripts\nakedobjects.app.ts

if (!($content -like "*export const version*")){
	Add-Content ..\Scripts\nakedobjects.app.ts "`nmodule NakedObjects { export const version = `"$ver`" }"
}

