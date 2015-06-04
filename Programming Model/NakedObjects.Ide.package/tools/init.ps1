param($rootPath, $toolsPath, $package, $project)

	# Had problems wih old NuGet versions and only 1.5+ is tested
	if ([NuGet.PackageManager].Assembly.GetName().Version -lt 1.5) 
	{
		throw "Naked Objects requires NuGet (Package Manager Console) 1.5 or later"
	} 

	# install snippets and templates 

	$nakedObjectsFolder = "Naked Objects"
	$CSSnippets = "$toolsPath\C#\*.snippet"

	$CSTemplates = "$toolsPath\C#\*.zip"
	$vsVersions = @("2013","2012","2010")

	Function copyFiles ($toParent, $toDir,  $files) {

		if (Test-Path $toParent) {
			$destination = "$toParent$toDir"
			
			if (!(Test-Path $destination)) {			
				New-Item $destination -itemType "directory"	
			}	

			"Installing to $destination for Visual Studio $vsVersion"
			Copy-Item $files $destination			
		}		
	}


	Foreach ($vsVersion in $vsVersions) {
		
		$docRoot = [System.Environment]::GetFolderPath("MyDocuments")
		$CSSnippetsFolder = "$docRoot\Visual Studio $vsVersion\Code Snippets\Visual C#\My Code Snippets\"
		$CSItemsFolder = "$docRoot\Visual Studio $vsVersion\Templates\ItemTemplates\Visual C#\"

		$docRoot 
		$CSSnippetsFolder 
		$CSItemsFolder 

		copyFiles $CSSnippetsFolder $nakedObjectsFolder $CSSnippets
	    copyFiles $CSItemsFolder $nakedObjectsFolder $CSTemplates
	}

	# helper functions for build and package versioning 

	<#
	.SYNOPSIS

	Do a clean build of the Naked Objects Framework including running tests.  
	.DESCRIPTION

	Do a clean build of the Naked Objects Framework including running tests. This cmd must be called from a solution at the same level as Naked Objects build 
	scripts. It will call msbuild on: 
	
	build.ide.proj /t:Clean
	build.pm.proj /t:Clean
	build.core.proj /t:Clean
	build.facade.proj /t:Clean
	build.ro.proj /t:Clean
	build.mvc.proj /t:Clean
	build.batch.proj /t:Clean
	
	build.ide.proj 
	build.pm.proj 
	build.core.proj 
	build.facade.proj 
	build.ro.proj 
	build.mvc.proj
	build.batch.proj 

	.EXAMPLE

	New-NakedObjectsCleanBuildTest
	#>
	function global:New-NakedObjectsCleanBuildTest()
	{		
		Function build( $project, $target ){
			& "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $project $target
			if ($LastExitCode -eq 1) {exit $LastExitCode}
		}

		build build.ide.proj /t:Clean
		build build.pm.proj /t:Clean
		build build.core.proj /t:Clean
		build build.facade.proj /t:Clean
		build build.ro.proj /t:Clean
		build build.mvc.proj /t:Clean
		build build.batch.proj /t:Clean
		
		build build.ide.proj 
		build build.pm.proj 
		build build.core.proj 
		build build.facade.proj 
		build build.ro.proj 
		build build.mvc.proj 
		build build.batch.proj
	}

	<#
	.SYNOPSIS

	Do a clean build of the Naked Objects Framework without running tests.  
	.DESCRIPTION

	Do a clean build of the Naked Objects Framework without running tests. This cmd must be called from a solution at the same level as Naked Objects build 
	scripts. It will call msbuild on: 
	
	build.ide.proj /t:Clean
	build.pm.proj /t:Clean
	build.core.proj /t:Clean
	build.facade.proj /t:Clean
	build.ro.proj /t:Clean
	build.mvc.proj /t:Clean
	build.batch.proj /t:Clean
	
	build.ide.proj 
	build.pm.proj 
	build.core.proj /t:FrameworkPackageNoTest
	build.facade.proj 
	build.ro.proj /t:RestfulObjectsPackageNoTest
	build.mvc.proj /t:MvcPackageNoTest
	build.batch.proj

	.EXAMPLE

	New-NakedObjectsCleanBuildNoTest
	#>
	function global:New-NakedObjectsCleanBuildNoTest()
	{		
		Function build( $project, $target ){
			& "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $project $target
			if ($LastExitCode -eq 1) {exit $LastExitCode}
		}

		build build.ide.proj /t:Clean
		build build.pm.proj /t:Clean
		build build.core.proj /t:Clean
		build build.facade.proj /t:Clean
		build build.ro.proj /t:Clean
		build build.mvc.proj /t:Clean
		build build.batch.proj /t:Clean
		

		build build.ide.proj 
		build build.pm.proj 
		build build.core.proj /t:FrameworkPackageNoTest
		build build.facade.proj 
		build build.ro.proj /t:RestfulObjectsPackageNoTest
		build build.mvc.proj /t:MvcPackageNoTest
		build build.batch.proj
	}

	<#
	.SYNOPSIS

	Display the versions of the NakedObjects packages and also save them into nof-package-versions.txt.  
	.DESCRIPTION

	The versions of all the packages as stored in all the .nuspec files under the solution will be saved into the file "nof-package-versions.txt".
	Packages in the 'build' directory will be ignored. Contents of the file are displayed.  

	.EXAMPLE

	Get-NakedObjectsAllPackageVersions
	#>
	function global:Get-NakedObjectsAllPackageVersions() {

		$dependentFiles = Get-ChildItem -Filter ("*.nuspec") -Recurse  -Include *.nuspec | ?{ $_.fullname -notmatch "\\build\\?" }

		"" > nof-package-versions.txt

		foreach($dependentFile in $dependentFiles) {
			[xml]$x = Get-Content $dependentFile
	
			$x.package.metadata.id.PadRight(30) + "`t" + $x.package.metadata.version | out-file "nof-package-versions.txt"  -Append 
		}

		type "nof-package-versions.txt"
	}

	<#
	.SYNOPSIS

	Reads the new versions from nof-package-versions.txt and calls Update-NakedObjectsPackageVersion for each one.
	.DESCRIPTION

	A file called nof-package-versions.txt, in the format produced by the Get-NakedObjectsAllPackageVersions cmdlet, is expected in the solution directory. 
	Update-NakedObjectsPackageVersion will then be called for each package in the file. The file format is, for example 
	
	NakedObjects.Batch            	6.0.0
	NakedObjects.Authorisation.Wif	6.0.0
	etc 

	.EXAMPLE

	Update-NakedObjectsAllPackageVersions
	#>
	function global:Update-NakedObjectsAllPackageVersions() {
		$versionsFile = Get-ChildItem -Filter "nof-package-versions.txt"
		$versions = @(Get-Content $versionsFile) 

		foreach($version in $versions) {
			if ($version) {
				$idAndVersion = $version.Split("`t")
				$id = $idAndVersion[0].Trim()
				$ver= $idAndVersion[1].Trim()

				Update-NakedObjectsPackageVersion -Package $id -NewVersion $ver
			}
		}
	}

	<#
	.SYNOPSIS

	Update the version of a single NakedObjects package in its NuSpec, and update references to it in other NuSpecs.
	.DESCRIPTION

	Updates the version of a NakedObjects Package.The version is updated in the package's .nuspec file and also any dependent packages' .nuspec files. 
	.PARAMETER Package

	Mandatory. The name of the package. The following are valid.   
	nakedobjects.batch         
	nakedobjects.authorisation.wif
	nakedobjects.core    
	nakedobjects.xat         
	nakedobjects.mvc-assemblies
	nakedobjects.mvc-filetemplates
	nakedobjects.ide            
	nakedobjects.programmingmodel
	restfulobjects.mvc          
	restfulobjects.server      
	nakedobjects.sample.icons
	nakedobjects.facade.impl 
	nakedobjects.facade        
	nakedobjects.mvc.selenium   

	.PARAMETER NewVersion

	Mandatory. The new version for the package. Any string is valid.  
	.EXAMPLE

	Update-NakedObjectsPackageVersion nakedobjects.batch 6.0.0-beta3
	#>
	function global:Update-NakedObjectsPackageVersion($Package, $NewVersion) {

		"Calling Update-NakedObjectsPackageVersion -Package " + $Package  + " -NewVersion " + $NewVersion

		# validate parms 

		$supportedPackages = "nakedobjects.batch",           
							 "nakedobjects.authorisation.wif",
							 "nakedobjects.core",        
							 "nakedobjects.xat",              
							 "nakedobjects.mvc-assemblies",   
							 "nakedobjects.mvc-filetemplates",
							 "nakedobjects.ide",              
							 "nakedobjects.programmingmodel", 
							 "restfulobjects.mvc",            
							 "restfulobjects.server",         
							 "nakedobjects.sample.icons",     
							 "nakedobjects.facade.impl",     
							 "nakedobjects.facade",          
							 "nakedobjects.mvc.selenium"     
	
		if (!($Package -is [string])){
			return "package must be string";
		}

		if (!($supportedPackages.Contains($Package.ToLower()))){	
			return "no such naked objects package " + $Package;
		}

		if (!($NewVersion -is [string])){
			return "version must be string";
		}

		# update nuspec files 

		$nuspecFile = Get-ChildItem -Filter ($Package + ".nuspec") -Recurse  -Include *.nuspec | ?{ $_.fullname -notmatch "\\build\\?" }
	
		[xml]$nuspecContent =  Get-Content $nuspecFile 

		$nuspecContent.package.metadata.version = $NewVersion

		$nuspecContent.Save($nuspecFile.FullName);

		# update dependencies 

		# determine valid version range for dependent packages (i.e. exclude next major version)
		[string] $major = $NewVersion.Substring(0,1)
		[int] $majorNext = ([int] $major) + 1
		[string] $range = "["+$NewVersion+ ", "+$majorNext+")"

		$dependentFiles = Get-ChildItem -Filter ("*.nuspec") -Recurse  -Include *.nuspec | ?{ $_.fullname -notmatch "\\build\\?" }

		foreach($dependentFile in $dependentFiles) {
			[xml]$x = Get-Content $dependentFile
			$upd = ""
	
			foreach($dependency in $x.package.metadata.dependencies.dependency) {
				if ($dependency.id -eq $Package) {
					$dependency.version = $range
					$upd = "updated"
					"Updated dependency on "+$dependency.id+" to " + $Range
				}
			} 

			if ($upd -eq "updated"){
				"Saving " + $dependentFile.FullName
				$x.Save($dependentFile.FullName)
			}
		} 
	}

	<#
	.SYNOPSIS

	Update all packages.config files and all .csproj files to refer to the specified new version of the package
	.DESCRIPTION

	The package version is updated in each packages.config file that it appears in. In addition the hint paths in all *proj files in the solution that 
	use the package are updated. 
	.PARAMETER PackageName

	Mandatory. The name of the package. And string is valid
	.PARAMETER NewVersion

	Mandatory. The new version. Any string is valid. 
	.PARAMETER verbose

	A flag to output additional info during processing.   
	.EXAMPLE

	Update-NakedObjectsPackageConfigAndProjectRefs nakedobjects.batch 6.0.0-beta3

	.EXAMPLE
	Update-NakedObjectsPackageConfigAndProjectRefs nakedobjects.batch 6.0.0-beta3 -verbose
	#>
	function global:Update-NakedObjectsPackageConfigAndProjectRefs($PackageName, $NewVersion, [switch] $verbose) {
    
		if (!($PackageName -is [string])){
			return "package is mandatory and must be a string";
		}

		if (!($NewVersion -is [string])){
			return "version is mandatory and must be a string";
		}

		if ($verbose) {
			"updating package.config files"
		}

		$packageConfigFiles = Get-ChildItem -Recurse  -Include packages.config

		foreach($packageConfigFile in $packageConfigFiles) {

		    if ($verbose) {
				"Processing " + $packageConfigFile.FullName
			}

			[xml]$cfgAsXml = Get-Content $packageConfigFile
			$upd = ""

			foreach($package in $cfgAsXml.packages.package) {

				if ($package.id -eq $PackageName) {
					$package.version = $NewVersion
					$upd = "updated"

					if ($verbose) {
						"Updated package " + $PackageName +  "  to " + $NewVersion + " in " + $packageConfigFile.FullName
					}
				}
			} 

			if ($upd -eq "updated"){
				"Updated and saved " + $packageConfigFile.FullName
				$cfgAsXml.Save($packageConfigFile.FullName)
			}		
		}

		if ($verbose) {
			"updating proj files"
		}

		$projectFiles = Get-ChildItem -Recurse  -Include *.*proj

		foreach($projectFile in $projectFiles) {

			if ($verbose) {
				"Processing " + $projectFile.FullName
			}

			[xml]$projAsXml = Get-Content $projectFile
			$upd = ""

			foreach($itemGroup in $projAsXml.Project.ItemGroup) {
				foreach($ref in $itemGroup.Reference) {

					$hintPath = $ref.HintPath

					if ($hintPath) {
						if ($verbose) {
							"Processing path " + $hintPath
						}
						
						if ($hintPath -match "\\" + $PackageName + "\.[0-9]+\.[0-9]+\..*\\lib" ) {

							if ($verbose){
								"Found path" + $hintPath
							}

							$newPath = $hintPath -replace "($PackageName).*\\lib", ('$1.' + $newVersion + "\lib")

							$ref.HintPath = $newPath 

							$upd = "updated"

							if ($verbose) {
								"Updated path to " + $newPath  + " in " + $projectFile.FullName
							}
						}
					}
				} 
			}

			if ($upd -eq "updated"){
				"Updated and saved " + $projectFile.FullName
				$projAsXml.Save($projectFile.FullName)
			}		
		}

	}