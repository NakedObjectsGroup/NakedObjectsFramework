param($rootPath, $toolsPath, $package, $project)

# Had problems wih old NuGet versions and only 1.5+ is tested
if ([NuGet.PackageManager].Assembly.GetName().Version -lt 1.5) 
{
	throw "Naked Objects requires NuGet (Package Manager Console) 1.5 or later"
} 