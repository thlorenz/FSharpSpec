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
open System.Collections.Generic
open System.Reflection

open System.Windows
open System.Windows.Controls
open System.Windows.Input

open System.Windows.Markup
open System.Windows.Media
open System.Xml

open GuiCore

type SpecViewModel (spec : (string * SpecDelegate)) =
  inherit ViewModelBase ()

  let _name = fst spec
  let _spec = snd spec

  let mutable _specResult = None
  let runSpec () = _specResult <- Some(_spec.Invoke())
  let debugSpec () = 
    Debugger.Launch() |> ignore
    _specResult <- Some(_spec.Invoke())
  let _runSpecCommand = ActionCommand ((fun _ -> runSpec()), (fun _ -> true))
  let _debugSpecCommand = ActionCommand ((fun _ -> runSpec()), (fun _ -> true))
  
  member x.Name = _name
  member x.RunSpecCommand with get () = _runSpecCommand :> ICommand
  member x.DebugSpecCommand with get () = _debugSpecCommand :> ICommand
  member x.SpecResult with get () = _specResult.Value

  override x.ToString() = x.Name

type SpecContainerViewModel (specs, clazz) =
  inherit ViewModelBase ()
  let _specs = specs
  let _clazz = clazz
  
  let mutable _isExpanded = false
  let _instantiatedSpecs = ObservableCollection<SpecViewModel>()
  
  let extractSpecs () = 
    match _instantiatedSpecs with
    | xs when xs.Count > 0  -> ()
    | otherwise             ->
        let instantiatedContext = _clazz |> instantiate
        _specs.Method.Invoke(instantiatedContext, null) :?> (string * SpecDelegate) list
        |> List.iter (fun spec -> _instantiatedSpecs.Add <| SpecViewModel(spec)) 

  member x.Name with get() =  _specs.Name |> removeLeadingGet
  member x.Specifications with get() = _instantiatedSpecs

  member x.IsExpanded 
    with get() = _isExpanded
    and set value = 
      _isExpanded <- value 
      extractSpecs()

  override x.ToString() = x.Name

type ContextViewModel (node : Node) = 
  inherit ViewModelBase ()
  let _node = node
  let _childContexts = List<ContextViewModel>()
  let _specContainers = List<SpecContainerViewModel>()
  let mutable _isExpanded = false

  do
    _node.Children
    |> Seq.iter (fun c -> _childContexts.Add <| ContextViewModel(c))
 
    _node.Context.SpecLists
    |> Seq.map (fun mi -> { Name = mi.Name; Method = mi }) 
    |> Seq.iter (fun si -> _specContainers.Add <| SpecContainerViewModel(si, _node.Context.Clazz))
  
  member x.Name 
    with get() = 
      match _node.Context.Clazz.Name with
      | x when x <> "Object"  -> x
      | otherwise             -> "Specifications"

  member x.ChildContexts with get() = _childContexts
  member x.SpecContainers with get() = _specContainers
  member x.IsExpanded 
    with get ()       = _isExpanded
    and  set (value)  = _isExpanded <- value

  //member x.Items with get () = _childContexts @ _specContainers

  override x.ToString() = x.Name

let tree = allContexts |> getContextTreeOfContexts
let asmRoot = ContextViewModel(tree)

let loadGuiRunnerView (fileName : string) =
  let reader = XmlReader.Create fileName
  XamlReader.Load reader :?> UserControl


let view () = 
  let userControl = loadGuiRunnerView @"C:\dev\FSharp\FSharpSpec\src\Runners\FSharpSpec.GuiRunner\GuiRunnerView.xaml" 
  userControl.DataContext <- asmRoot
  userControl

let win = Window( Topmost = true, Width=300.0, Height = 200.0, Content = view)
win.Show()

win.Content <- view ()



let r = asmRoot.ChildContexts.[1]
let c = r.SpecContainers.[0]
c.IsExpanded <- true
let s = c.Specifications.[0]
s.RunSpecCommand.Execute(null)
s.DebugSpecCommand.Execute(null)

