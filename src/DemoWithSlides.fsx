#I @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\"
#r @"PresentationCore.dll"
#r "PresentationFramework.dll"
#r "WindowsBase.dll"
#r "System.Xaml"
#r "Microsoft.CSharp"

#r @"C:\dev\FSharp\FSharpSpec\src\Demos\Demos.InteractiveSlides\bin\Debug\Demos.InteractiveSlides.dll"
    
let show = SlideShow(@"C:\dev\FSharp\FSharpSpec\src\")

let transitionHeadLine = transition show Headline
let transitionLarge = transition show Large
let transitionMediumLarge = transition show MediumLarge
let expandNormal alignment  = expandNormal alignment show

"FSharpSpec" |> transitionHeadLine
"Why do we need it?" |> transitionHeadLine
"What's out there?" |> transitionHeadLine
"XUnit Frameworks" |> transitionHeadLine
show |> clear; show.ReadFile("NUnitSample.txt") |> expandNormal Middle
"MSpec" |> transitionHeadLine
show |> clear; show.ReadFile("MSpecSample.txt") |> expandNormal Middle
"FSharpSpec" |> transitionHeadLine
show |> clear; show.ReadFile("FSharpSpecSample.txt") |> expandNormal Middle

"But Seriously" |> transitionHeadLine
"Let's Compare" |> transitionHeadLine

show.ResetStyle()
show |> clear; show.AlignV Top;  show.AlignH Left;
show.RevealStyle <- FromLeft; show.TextStyle <- MediumLarge; show.LeftMargin <- 100

"XUnit Frameworks"    |> show.WriteUsing 20 Headline
"Advantages"          |> show.WriteUsing 50 Large
"● can write tests"             |> show.Write
"Disadvantages"       |> show.WriteUsing 50 Large
"● only safe to have ONE assertion PER test" |> show.Write
"● lots of noise"               |> show.Write
"● verbose"                     |> show.Write
"● hard to inherit contexts"    |> show.Write
"● hard to re-use assertions"   |> show.Write

show |> clear; "MSpec"               |> show.WriteUsing 20 Headline
"Advantages"          |> show.WriteUsing 50 Large
"● less noise"                  |> show.Write
"● multiple assertions per test" |> show.Write
"● easier to inherit contexts"  |> show.Write
"Disadvantages"       |> show.WriteUsing 50 Large
"● everything is static"                                            |> show.Write
"● only limited assertion re-use via behaviors"                   |> show.Write
"● limited by C# language"                                          |> show.Write
"● structure of result output doesn't reflect context inheritance"  |> show.Write

show |> clear; "FSharpSpec"          |> show.WriteUsing 20 Headline
"Advantages"          |> show.WriteUsing 50 Large
"● least noise"                 |> show.Write
"● uses class inheritance to build up contexts" |> show.Write
"● non static list of assertions returned by property" |> show.Write
"● assertions re-usable (RowTests on steroids)" |> show.Write
"● less reflection -> superfast test runner" |> show.Write
"● structure of result output reflects context inheritance" |> show.Write
"Disadvantages"       |> show.WriteUsing 50 Large
"● need some familiarity with F#"                           |> show.Write
"● failure of assertion setup affects other assertions" |> show.Write
"● cannot nest types in F#" |> show.Write

"Why F#?"           |> transitionHeadLine
"Super succinct"    |> transitionHeadLine
"No need for Parentheses" |> transitionHeadLine
"Type Inference"    |> transitionHeadLine

"Statically typed"  |> transitionHeadLine
"● Intellisense"    |> show.Write
"● Refactoring Support" |> show.Write

"FSharpSpec Features" |> transitionHeadLine
"● TestDriven.Net support (on type level)" |> show.Write
"● Console Runner" |> show.Write
"● Gui Runner" |> show.Write
"● Debugging Support" |> show.Write
"● run assertions interactively" |> show.Write

"Writing a specification" |> transitionHeadLine
"it \"adds 1 + 1 to 2\"         (1 + 1) should.equal 2" |> transitionMediumLarge
"Or interactively:" |> show.WriteUsing 0 MediumLarge
"\"adds 1 + 1 to 2\" |> run  (1 + 1) should.equal 2" |> show.WriteUsing 0 MediumLarge
"run2  (1 + 1) should.equal 2" |> show.WriteUsing 320 MediumLarge


#r @"FSharpSpec\bin\Debug\FSharpSpec.dll"
open FSharpSpec
open System

"Comparisons" |> transitionHeadLine
show.Hide()

"1 + 1 = 2"   |> run (1 + 1) should.equal 2
"1 + 1 <> 2"  |> run (1 + 1) shouldn't.equal 2               // fails

"1 > 0"       |> run 1 should.beGreaterThan 0
"1 > 2"       |> run 1 should.beGreaterThan 2                // fails

show.Show(); "should.be" |> transitionHeadLine
show.Hide()
"'' is empty" |> run "" should.be Empty
"1 is an int" |> run 1 should.be typeof<int>
"'a string' is a string" |> run "a string" should.be1 typeof<string>

show.Show(); "Lists and Strings" |> transitionHeadLine
show.Hide()

run2 "Content" should.be Empty               //fails
run2 [1] shouldn't.be Empty
run2 [] shouldn't.be Empty                   // fails

run2 "Hello World" should.contain "Hello"
run2 "Hello" should.contain "Hello World"    // fails

run2 [1;2] should.contain 1
run2 [1;2] should.contain 3                  // fails

run2 [1;2] should.contain1 [1;2]  
run2 [1;2;3] should.contain1 [1;2;4]        // fails

run2 [1;2;3] should.containOnly [1;2;3]
run2 [1;2;3] should.containOnly [1;2]       // fail

show.Show(); "Exceptions" |> transitionHeadLine
show.Hide()
let exn = catch (fun () -> 1 / 0)
run2 exn should.be typeof<DivideByZeroException>
run2 exn.Message should.contain "divide by zero"          

run2 (fun () -> 1 /0) should.failWith typeof<DivideByZeroException>
run2 (fun () -> 1 /0) should.failWithMessageContaining "divide by zero"
run2 (fun () -> 1 /0) should.failWithMessageNotContaining "argument"

show.Show(); "Contexts" |> transitionHeadLine
show.Hide() // FSharpSpec.FSharpSampleSpecs/BDD.fs

show.Show(); "Specifications via Functions" |> transitionHeadLine
show.Hide()   // FSharpSpec.FSharpSampleSpecs/RowTests.fs
              // FSharpSpec.Katas.StringCalculator/StringCalculatorSpecs.fs
              // FSharpSpec.GuiRunner.Specs/GuiRunnerViewModelSpecs.fs

show.Show(); "Multiple Assertions" |> transitionHeadLine
show.Hide() // FSharpSpec.FSharpSampleSpecs/MultipleAssertions.fs

show.Show(); "Mocking with FSharpSpec" |> transitionHeadLine
show.Hide()  // FSharp.Interop.NSubstitute/NSubstitute.fsx

show.Show(); "Scripting with FSharpSpec" |> transitionHeadLine
show.Hide()  // FSharpSpec.FSharpSampleSpecs/SpecScript.fsx

