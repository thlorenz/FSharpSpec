
@echo off & if not "%ECHO%"=="" echo %ECHO%

setlocal
set LOCALDIR=%~dp0

echo Windows Registry Editor Version 5.00 > FSharpSpecTDNet.reg
echo [HKEY_CURRENT_USER\Software\MutantDesign\TestDriven.NET\TestRunners\FSharpSpec] >> FSharpSpecTDNet.reg
echo "Application"="" >> FSharpSpecTDNet.reg
echo "AssemblyPath"="%LOCALDIR:\=\\%FSharpSpec.TDNetRunner.dll" >> FSharpSpecTDNet.reg
echo "TargetFrameworkAssemblyName"=FSharpSpec" >> FSharpSpecTDNet.reg
echo "TypeName"="FSharpSpec.TDNetRunner.SpecsRunner" >> FSharpSpecTDNet.reg
echo @="6" >> FSharpSpecTDNet.reg

regedit FSharpSpecTDNet.reg

del FSharpSpecTDNet.reg

