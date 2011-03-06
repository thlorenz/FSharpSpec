$pluginDir = "$($env:APPDATA)\JetBrains\ReSharper\v5.1\vs10.0\Plugins"
$pluginFiles = gci src\Runners\FSharpSpec.ResharperRunner.5.1\bin\Debug\FSharpSpec*

ps devenv | kill

cp $pluginFiles $pluginDir -Force

Write-Host "Copied $pluginFiles"

ii .\src\FSharpSpec.sln
