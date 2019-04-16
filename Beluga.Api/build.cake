#tool nuget:?package=GitVersion&verion=3.6.5

var target = Argument("Target","Version");
var configuration = Argument("Configuration","Release");

var packageVersion = "0.1.0";
Task("Restore")
	.Does(()=>{
		
		NuGetRestore("../SendeoApi.sln");
		
		});


Task("Build")
	.IsDependentOn("Restore")
	.Does(()=>{

		DotNetBuild("../SendeoApi.sln",
		settings => settings.SetConfiguration(configuration).WithTarget("Build")
		);
		}
	  );


 Task("Version")
 .IsDependentOn("Build")
	.Does(()=>{
       var version= GitVersion();
	   Information($"Calculated semantic version {version.SemVer}");
	   packageVersion = version.NuGetVersion;
	   Information($"Corresponding package verion {packageVersion}");

		if(BuildSystem.IsLocalBuild){

			   GitVersion(new GitVersionSettings
			   {
				 OutputType = GitVersionOutput.BuildServer,
				 UpdateAssemblyInfo =true
		 
				 });
			}
	   });

	  RunTarget(target);