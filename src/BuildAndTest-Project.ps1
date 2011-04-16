param ($projectPath)
    $DOTNETVERSION = "4.0.30319" 
    $msbuild = Join-Path $env:windir "Microsoft.Net\Framework\v$DOTNETVERSION\msbuild.exe"
    
    $fullProjectPath = Resolve-Path $projectPath
    $binPath = Join-Path $fullProjectPath "bin\Debug"

    $project = gci "$fullProjectPath\*" -include "*.fsproj"
    Write-Host "Building and testing`n`t$project`n"

    pushd
    cd $fullProjectPath
    & $msbuild /v:quiet
    popd

    $projectDllName = $project.Name.Replace('fsproj', 'dll')

    $runnerRoot = Resolve-Path "C:\dev\FSharp\FSharpSpec\src\Runners\FSharpSpec.ConsoleRunner\bin\Debug"
    $specRunner = Join-Path $runnerRoot "FSharpSpec.ConsoleRunner.exe"

    & $specRunner "$binPath\$projectDllName"
