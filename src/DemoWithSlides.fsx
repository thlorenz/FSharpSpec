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

"FSharpSpec" |> transitionHeadLine
"Why do we need it?" |> transitionHeadLine
"What's out there now" |> transitionHeadLine
"XUnit Frameworks" |> transitionHeadLine
show |> clear; show.ReadFile("NUnitSample.txt") |> expandNormal Top
show.LoadStyle()
"MSpec" |> transitionHeadLine
// MSpec Code here
// FSharpSpec Code here


"But Seriously !" |> transitionHeadLine
"Let's Compare" |> transitionHeadLine

show.ResetStyle()
show |> clear; show.AlignV Top;  show.AlignH Left;
show.RevealStyle <- FromLeft; show.TextStyle <- Normal; show.LeftMargin <- 120

"XUnit\n"             |> show.WriteUsing 150 Large
"Advantages\n"        |> show.WriteUsing 100 MediumLarge
"----------"          |> show.WriteUsing 100 MediumLarge
"● can write tests"   |> show.Write
"Disadvantages\n"     |> show.WriteUsing 100 MediumLarge
"● can write tests"   |> show.Write

