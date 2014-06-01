function Get-ScriptDirectory
{
   $Invocation = (Get-Variable MyInvocation -Scope 1).Value
   Split-Path $Invocation.MyCommand.Path
}

$nuget = Join-Path (Get-ScriptDirectory) src\.nuget\NuGet.exe

# ensure all packages are installed

$config = Join-Path (Get-ScriptDirectory) src\Reactive.EventAggregator\packages.config
$solution_dir = Join-Path (Get-ScriptDirectory) src

. $nuget restore .\src\Reactive.EventAggregator.sln

# build the solution from scratch
$version = "v4.0.30319"
$sln = Join-Path (Get-ScriptDirectory) src\Reactive.EventAggregator.sln

. $env:windir\Microsoft.NET\Framework\$version\MSBuild.exe $sln /t:Rebuild /p:Configuration=Release /m /v:q

# package it up

$nuspec = Join-Path (Get-ScriptDirectory) src\Reactive.EventAggregator\Reactive.EventAggregator.nuspec

$nugetVersion = $env:APPVEYOR_BUILD_NUMBER
if ($nugetVersion -eq "")
{
   $nugetVersion = "1.1.0"
}

. $nuget pack $nuspec -Version $env:APPVEYOR_BUILD_NUMBER