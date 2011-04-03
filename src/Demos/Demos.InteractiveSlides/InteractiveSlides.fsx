#I @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\"
#r @"PresentationCore.dll"
#r "PresentationFramework.dll"
#r "WindowsBase.dll"
#r "System.Xaml"
#r "Microsoft.CSharp"

#r @"C:\dev\FSharp\FSharpSpec\src\Demos\Demos.InteractiveSlides\bin\Debug\Demos.InteractiveSlides.dll"
    
let show = SlideShow(@"C:\dev\FSharp\FSharpSpec\src\")

let transitionHeadLine = transitionHeadLine show
let expandNormal alignment  = expandNormal alignment show

"Why FSharpSpec"  |> show.WriteUsing 0 Headline

transitionHeadLine "What we have so far"
transitionHeadLine "NUnit, MSTest if you are old school"
transitionHeadLine "MSpec if you are more BDD oriented"
show.Slides.Clear()
show.RevealStyle <- FromLeft
show.LeftMargin <- 200
show.TextStyle <-  Normal
show.Write  "+ HelloWorld"
show.Write  "+ HelloWorld"
show.Write  "+ HelloWorld"



"NUnitSample.txt" |> show.ReadFile |> expandNormal Bottom



show.End  
  