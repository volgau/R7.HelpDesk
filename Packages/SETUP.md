# Initial setup of packaging project

This template uses MSBuild to create install and source packages,
but due to template engine limitations it will require some changes 
to be made manually in a project file.

1. Install *MSBuild.Community.Tasks* extension from https://github.com/loresoft/msbuildtasks/releases

2. Open `Packages.csproj` in text editor or add it to project files.

3. Just after last `<Import Project="..." />` directive insert this code:

```
<!-- Begin snippet -->
<Import Project="$(MSBuildExtensionsPath)\MSBuildCommunityTasks\MSBuild.Community.Tasks.Targets" />
<PropertyGroup>
	<PackageExtension>zip</PackageExtension>
	<PackageName>R7.HelpDesk</PackageName>
	<PackageOutputPath>output</PackageOutputPath>
	<MSBuildDnnBinPath Condition="'$(MSBuildDnnBinPath)' == ''">..\..\..\bin</MSBuildDnnBinPath>
</PropertyGroup>
<Import Project="Install.targets" />
<Import Project="Source.targets" />
<Target Name="AfterBuild" DependsOnTargets="MakeInstallPackage;MakeSourcePackage" />
<!-- End snippet -->
```
    
4. Now switch to "Release" configuration and execute "Build All" command.

5. After this in a `Packages\output` folder you should find two `.zip` archives - 
one for install and one for source packages. 

6. Now you can install extension from `R7.HelpDesk-01.00.00-Install.zip` package  
through *DNN > Host > Extensions*, as usual.

# Extending solution

Current scripts automatically able to add all DNN projects inside solution into a single source or install package. 
The general idea behind that is that all projects in the solution is very dependant.

If you add new DNN extension to your solution using *R7.DnnTemplates*, you probably only need to:

1. Name your project like *R7.HelpDesk_MyNewExtension*. If not, just add another `<InstallBinaryFiles>` entry
to the `Install.targets` for new extension binary file (.dll) too.

2. Move Packages Project/EndProject entry in the `R7.HelpDesk.sln` file to the bottom, to ensure
that Packages project will build last. 

3. Merge new project's `.SqlDataProvider` files, `.dnn` manifest file (and all files, referenced by manifest -
generally, it's `License.txt` and `ReleaseNotes.txt`) into the similar files from first extension.

# General tips

* After external `.sln`, `.csproj` and `.targets` file changes you probably need to reload solution. 


