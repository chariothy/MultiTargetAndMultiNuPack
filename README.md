MultiTargetFrameworkDemo
============
A demonstrantion of how to build Visual Studio projects targeting multi-framework and then pack them all within a nuget package.

## MultiTarget

All things I done is in MultiTarget.csproj:

#### Modify default target's output path
```
<OutputPath>bin\$(Configuration)\$(TargetFrameworkVersion.Replace("v", "NET").Replace(".", ""))\</OutputPath>
```
There's nothing will be created bin\Debug or bin\Release root folder but subfolders named after framework, such as NET35, NET40 ...

#### Add user defined PropertyGroups
```
<!-- ################################# Begin of MultiTarget ############################################## -->
  <PropertyGroup>
    <FrameworkNumber>$(TargetFrameworkVersion.Replace("v", "").Replace(".", ""))</FrameworkNumber>
    <DefineConstants Condition=" !$(DefineConstants.Contains(';NET')) ">$(DefineConstants);$(TargetFrameworkVersion.Replace("v", "NET").Replace(".", ""))</DefineConstants>
    <Framework Condition=" '$(Framework)' == '' ">$(TargetFrameworkVersion.Replace("v", "NET").Replace(".", "_"))</Framework>
    <DefineConstants Condition=" $(FrameworkNumber) &gt;= 20 ">$(DefineConstants);NET20_OR_ABOVE</DefineConstants>
    <DefineConstants Condition=" $(FrameworkNumber) &gt;= 35 ">$(DefineConstants);NET35_OR_ABOVE</DefineConstants>
    <DefineConstants Condition=" $(FrameworkNumber) &gt;= 40 ">$(DefineConstants);NET40_OR_ABOVE</DefineConstants>
    <DefineConstants Condition=" $(FrameworkNumber) &gt;= 45 ">$(DefineConstants);NET45_OR_ABOVE</DefineConstants>
    <DefineConstants Condition=" $(FrameworkNumber) &gt;= 451 ">$(DefineConstants);NET451_OR_ABOVE</DefineConstants>
    <DefineConstants Condition=" $(FrameworkNumber) &gt;= 452 ">$(DefineConstants);NET452_OR_ABOVE</DefineConstants>
  </PropertyGroup>
<!-- ################################# End of MultiTarget ############################################## -->
```
Serval DefineConstants have been added. So we can use these value in program to distinguish different target framework version:
```
#if NET35_OR_ABOVE
using System.Linq;
#endif

#if NET40_OR_ABOVE
using System.Threading.Tasks;
#endif
```
#### Modify HintPath for different framework version
```
<Reference Include="log4net">
      <HintPath>..\packages\log4net.2.0.3\lib\net40-full\log4net.dll</HintPath>
</Reference>
```
So log4net in net40 can be referenced by NET40_OR_ABOVE.

#### Add RequiredTargetFramework
```
<Reference Include="System.Core">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
</Reference>
```

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
