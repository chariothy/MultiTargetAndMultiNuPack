MultiTargetAndMultiNuPack
============
Just a demonstrantion of how to build C# project targeting multi-framework and then pack them all within a nuget package.

## MultiTarget

All things I done is in MultiTarget.csproj:

#### Modify default target's output path
```
<OutputPath>bin\$(Configuration)\$(TargetFrameworkVersion.Replace("v", "NET").Replace(".", ""))\</OutputPath>
```
There's nothing under bin\Debug or bin\Release but folders named after framework, such as NET35, NET40 ...

#### Add user defined PropertyGroups
```
<!-- ################################# Begin of MultiTarget ############################################## -->
  <PropertyGroup>
    <Framework Condition=" '$(Framework)' == '' ">$(TargetFrameworkVersion.Replace("v", "NET").Replace(".", "_"))</Framework>
  </PropertyGroup>
  <PropertyGroup Condition=" $(Framework.Replace('NET', '').Replace('_', '.')) &lt; $(TargetFrameworkVersion.Replace('v', '')) and '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <TargetFrameworkVersion>$(Framework.Replace("NET", "v").Replace("_", "."))</TargetFrameworkVersion>
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\$(Framework.Replace("_", ""))\</OutputPath>
    <DefineConstants>TRACE;DEBUG;TEST</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <Prefer32Bit>false</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup Condition=" $(Framework.Replace('NET', '').Replace('_', '.')) &lt; $(TargetFrameworkVersion.Replace('v', '')) and '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <TargetFrameworkVersion>$(Framework.Replace("NET", "v").Replace("_", "."))</TargetFrameworkVersion>
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\$(Framework.Replace("_", ""))\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <Prefer32Bit>false</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup>
    <!-- Adding a custom constant will auto-magically append a comma and space to the pre-built constants.    -->
    <!-- Move the comma delimiter to the end of each constant and remove the trailing comma when we're done.  -->
    <DefineConstants Condition=" !$(DefineConstants.Contains(';NET')) ">$(DefineConstants);$(TargetFrameworkVersion.Replace("v", "NET").Replace(".", ""))</DefineConstants>
    <DefineConstants Condition=" $(TargetFrameworkVersion.Replace('v', '')) &gt;= 2.0 ">$(DefineConstants);NET20_OR_GREATER</DefineConstants>
    <DefineConstants Condition=" $(TargetFrameworkVersion.Replace('v', '')) &gt;= 3.5 ">$(DefineConstants);NET35_OR_GREATER</DefineConstants>
    <DefineConstants Condition=" $(TargetFrameworkVersion.Replace('v', '')) &gt;= 4.0 ">$(DefineConstants);NET40_OR_GREATER</DefineConstants>
    <DefineConstants Condition=" $(TargetFrameworkVersion.Replace('v', '')) &gt;= 4.5 ">$(DefineConstants);NET45_OR_GREATER</DefineConstants>
    <DefineConstants Condition=" $(TargetFrameworkVersion.Replace('v', '')) &gt;= 4.5.1 ">$(DefineConstants);NET451_OR_GREATER</DefineConstants>
    <DefineConstants Condition=" $(TargetFrameworkVersion.Replace('v', '')) &gt;= 4.5.2 ">$(DefineConstants);NET452_OR_GREATER</DefineConstants>
  </PropertyGroup>
<!-- ################################# End of MultiTarget ############################################## -->
```
Two PropertyGroups for msbuild added above. The condition:
```
Condition=" $(Framework.Replace('NET', '').Replace('_', '.')) &lt; $(TargetFrameworkVersion.Replace('v', '')) and '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "
```
set there to avoid TargetFrameworkVersion value be changed by selecting other target framework in project property setting dialog box.
```
<TargetFrameworkVersion>$(Framework.Replace("NET", "v").Replace("_", "."))</TargetFrameworkVersion>
```
Serval DefineConstants have been added. So we can use these value in program to distinguish different target framework version:
```
#if NET40_OR_GREATER
using System.Threading.Tasks;
#endif
```
#### Add AfterBuild Targets
```
<Target Name="AfterBuild">
    <!-- ################################# Begin of MultiTarget ############################################## -->
    <!-- This just allows us to drop a note in the build output -->
    <Message Text="Enter After Build TargetFrameworkVersion:$(TargetFrameworkVersion) Framework:$(Framework)" Importance="high" />
    <!-- This is the key to the whole process. The second build happens here.  We set our 'Framework' variable allowing the above PropertyGroups to run more frameworks.  -->
    <MSBuild Condition=" '$(Framework)' == 'NET4_5_2'" Projects="$(MSBuildProjectFile)" Properties="Framework=NET4_5_1" RunEachTargetSeparately="true" />
    <MSBuild Condition=" '$(Framework)' == 'NET4_5_1'" Projects="$(MSBuildProjectFile)" Properties="Framework=NET4_5" RunEachTargetSeparately="true" />
    <MSBuild Condition=" '$(Framework)' == 'NET4_5'" Projects="$(MSBuildProjectFile)" Properties="Framework=NET4_0" RunEachTargetSeparately="true" />
    <MSBuild Condition=" '$(Framework)' == 'NET4_0'" Projects="$(MSBuildProjectFile)" Properties="Framework=NET3_5" RunEachTargetSeparately="true" />
    <!--
    <MSBuild Condition=" '$(Framework)' == 'NET3_5'" Projects="$(MSBuildProjectFile)" Properties="Framework=NET2_0" RunEachTargetSeparately="true" />
    -->
    <!-- You could repeat the above node again here and target another framework if there was a property group that would evaluate to true-->
    <!-- Just more logging -->
    <Message Text="Exiting After Build TargetFrameworkVersion:$(TargetFrameworkVersion) Framework:$(Framework)" Importance="high" />
    <!-- ################################# End of MultiTarget ############################################## -->
</Target>
```
By doing this, Visual Studio can automatically build all target framework version less than current version. Say, if current version is NET 4.5, after press F6, NET4.0 and NET3.5 will be built automatically.

## MultiNuPack
#### Modify nuget spec file
Nuget will not do below things only if you tell it to do it.

1. Pack all target framework.
2. Copy those thing do not included in the lib folder.
3. Set app.config or web.config.


So we must specify them in nuget spec file:
```
<files>
    <file src="bin\Release\**\*.*" target="lib" />
    <file src="NativeBinaries\**\*.*" target="NativeBinaries" />
    <file src="Content\**\*.*" target="Content" />
    <file src="Tools\**\*.*" target="Tools" />
    <file src="Resource\**\*.*" target="Resource" />
</files>
```
#### Add transform files under Content folder
Nuget will add contents in app.config.transform to app.setting and web.config.transform to web.config after package installed.

#### Add install scripts under Tools
Scripts telling nuget what and where files will be copies to.

#### Prefer Release configuration
By default, nuget will pack binaries under Debug folder.
```
nuget pack MultiTarget.csproj -Prop Configuration=Release -Symbols
```

#That's all
Have fun!