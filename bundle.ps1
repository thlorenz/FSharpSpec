rm bundle -R -fo
mkdir bundle
cp .\src\Runners\FSharpSpec.ConsoleRunner\bin\Debug\* bundle
cp .\src\Runners\FSharpSpec.TDNetRunner\bin\Debug\* bundle
cp .\src\Runners\FSharpSpec.GuiRunner\bin\Debug\* bundle
cp .\src\FSharp.Interop.NSubstitute\bin\Debug\* bundle
cp .\src\FSharpSpec.Rx\bin\Debug\* bundle
cd bundle
. 7z.exe a FSharpSpec.zip *.*
cd .. 
