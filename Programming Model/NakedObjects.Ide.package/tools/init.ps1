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

	
	function global:New-NakedObjectsCleanBuildTest()
	{		
		Function build( $project, $target ){
			& "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $project $target
			if ($LastExitCode -eq 1) {exit $LastExitCode}
		}

		build build.ide.proj /t:Clean
		build build.pm.proj /t:Clean
		build build.core.proj /t:Clean
		build build.sf.proj /t:Clean
		build build.wif.proj /t:Clean
		build build.ro.proj /t:Clean
		build build.mvc.proj /t:Clean
		
		build build.ide.proj 
		build build.pm.proj 
		build build.core.proj 
		build build.sf.proj 
		build build.wif.proj 
		build build.ro.proj 
		build build.mvc.proj 
	}

	function global:New-NakedObjectsCleanBuildNoTest()
	{		
		Function build( $project, $target ){
			& "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $project $target
			if ($LastExitCode -eq 1) {exit $LastExitCode}
		}

		build build.ide.proj /t:Clean
		build build.pm.proj /t:Clean
		build build.core.proj /t:Clean
		build build.sf.proj /t:Clean
		build build.wif.proj /t:Clean
		build build.ro.proj /t:Clean
		build build.mvc.proj /t:Clean
		

		build build.ide.proj 
		build build.pm.proj 
		build build.core.proj /t:FrameworkPackageNoTest
		build build.sf.proj 
		build build.wif.proj /t:WifPackageNoTest
		build build.ro.proj /t:RestfulObjectsPackageNoTest
		build build.mvc.proj /t:MvcPackageNoTest
	}

	function global:Get-NakedObjectsPackageVersions() {

		$dependentFiles = Get-ChildItem -Filter ("*.nuspec") -Recurse  -Include *.nuspec | ?{ $_.fullname -notmatch "\\build\\?" }

		"" > nog-packages.txt

		foreach($dependentFile in $dependentFiles) {
			[xml]$x = Get-Content $dependentFile
	
			$x.package.metadata.id.PadRight(30) + "`t" + $x.package.metadata.version | out-file "nog-packages.txt"  -Append 
		}

		type "nog-packages.txt"
	}

	function global:Set-NakedObjectsPackageVersions() {
		$versionsFile = Get-ChildItem -Filter "nog-packages.txt"
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
							 "nakedobjects.surface.nof4",     
							 "nakedobjects.surface",          
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

		$nuspecFile = Get-ChildItem -Filter ($Package + "*") -Recurse  -Include *.nuspec | ?{ $_.fullname -notmatch "\\build\\?" }
	
		[xml]$nuspecContent =  Get-Content $nuspecFile 

		$nuspecContent.package.metadata.version = $NewVersion

		$nuspecContent.Save($nuspecFile.FullName);

		# update dependencies 

		$dependentFiles = Get-ChildItem -Filter ("*.nuspec") -Recurse  -Include *.nuspec | ?{ $_.fullname -notmatch "\\build\\?" }

		foreach($dependentFile in $dependentFiles) {
			[xml]$x = Get-Content $dependentFile
			$upd = ""
	
			foreach($dependency in $x.package.metadata.dependencies.dependency) {
				if ($dependency.id -eq $Package) {
					$dependency.version = $NewVersion
					$upd = "updated"
					"Updated dependency to " + $NewVersion + " in " + $dependency.id
				}
			} 

			if ($upd -eq "updated"){
				"Saving " + $dependentFile.FullName
				$x.Save($dependentFile.FullName)
			}
		} 
	}

	function global:Update-PackageConfig($PackageName, $NewVersion, [switch] $verbose) {
    
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

						if ($hintPath -like "*\" + $PackageName + ".*" ) {

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