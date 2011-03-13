#I @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug"
#r "FSharpSpec.dll"
#I @"C:\dev\FSharp\FSharpSpec\src\Runners\FSharpSpec.RunnerUtils\bin\Debug"
#r "FSharpSpec.RunnerUtils.dll"

#I @"bin\Debug\"
#r "FSharpSpec.GuiRunner.exe"

#I @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\"
#r @"PresentationCore.dll"
#r "PresentationFramework.dll"
#r "WindowsBase.dll"
#r "System.Xaml"
#r "Microsoft.CSharp"

open System
open System.IO
open System.Linq
open System.Diagnostics

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils
       
let path = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"

let asm = path |> getAssembly
let allContexts = asm |> getAllContexts
let getName (asmFullName : string) = (asmFullName.Split(',')).[0]

open System.Windows
open System.Windows.Controls
open System.Windows.Input

open System.Windows.Markup
open System.Windows.Media
open System.Xml

open FSharpSpec.GuiRunner

let tree = allContexts |> getContextTreeOfContexts
let asmRoot = ContextViewModel(tree)

let loadGuiRunnerView (fileName : string) =
  let reader = XmlReader.Create fileName
  XamlReader.Load reader :?> UserControl

let view () = 
  let userControl = loadGuiRunnerView @"C:\dev\FSharp\FSharpSpec\src\Runners\FSharpSpec.GuiRunner\GuiRunnerView.xaml" 
  userControl.DataContext <- asmRoot
  userControl

let win = Window( Topmost = true, Width=300.0, Height = 200.0, Content = view ())
win.Show()

win.Content <- view ()



let r = asmRoot.ChildContexts.[1]
let c = r.SpecContainers.[0]
c.IsExpanded <- true
let s = c.Specifications.[0]
s.RunSpecCommand.Execute(null)
s.DebugSpecCommand.Execute(null)

