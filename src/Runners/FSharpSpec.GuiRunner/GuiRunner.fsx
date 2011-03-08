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

open System.Collections.ObjectModel

open System.Windows
open System.Windows.Controls

open CoreGui

type SpecViewModel (spec : (string * SpecDelegate)) =
  inherit ViewModelBase ()

  let _name = fst spec
  let _delegate = snd spec

  member x.Name = _name


type SpecContainerViewModel () =
  inherit ViewModelBase ()
  
  let _specifications = ObservableCollection<SpecViewModel>();
  member x.Specifications = _specifications

type ContextViewModel () = 
  inherit ViewModelBase ()

  let _childContexts = ObservableCollection<ContextViewModel>()
  let _specContainers = ObservableCollection()
  
  member x.ChildContexts = _childContexts
  member x.SpecContainers = _specContainers

  member public x.Explore (tree : Node) = ()




