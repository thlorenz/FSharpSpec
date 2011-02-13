mkdir bundle
cp .\src\Runners\FSharpSpec.ConsoleRunner\bin\Debug\* bundle
cp .\src\Runners\FSharpSpec.TDNetRunner\bin\Debug\* bundle
cd bundle
. 7z.exe a FSharpSpec.zip *.*
cd .. 
