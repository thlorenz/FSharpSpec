namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel
open System.Collections.Generic
open System.Reflection
open System.Diagnostics

open System.Windows
open System.Windows.Controls
open System.Windows.Input

open System.Windows.Markup
open System.Windows.Media
open System.Xml

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils

type SpecState = | NotRunYet | Passed | Pending | Failed | Inconclusive 

type TreeViewModel (name) =
   inherit ViewModelBase ()
   
   let _name = name
   let mutable _isExpanded = false
   let mutable _isSelected = false
   
   member x.Name with get () = _name
  
   member x.IsExpanded 
    with get() = _isExpanded
    and set value = 
      _isExpanded <- value 
      base.OnPropertyChanged("IsExpanded")
      x.OnExpanded ()

   abstract member OnExpanded : unit -> unit
   default x.OnExpanded () = ()

   override x.ToString() = x.Name

type SpecViewModel (spec : (string * SpecDelegate), buildContextAndResolveSpecs) =
  inherit TreeViewModel (fst spec)

  let _spec = snd spec

  let mutable _state = NotRunYet

  member private x._runSpecCommand = ActionCommand ((fun _ -> x.runSpec _spec), (fun _ -> true))
  member private x._debugSpecCommand = ActionCommand ((fun _ -> x.debugSpec _spec), (fun _ -> true))

  member private x.toSpecState assertionResult = 
    match assertionResult with
    | AssertionResult.Passed -> Passed | AssertionResult.Pending -> Pending | AssertionResult.Failed -> Failed | AssertionResult.Inconclusive -> Inconclusive
 
  member x.runSpec specToRun = 
    try
      let outcome = _spec.Method.Invoke(specToRun.Target, null) :?> AssertionResult  
      x.State <- outcome |> x.toSpecState
    with
      ex              -> x.State <- Failed

  /// Re-evaluates the Context after launching the debugger in order to hit all possible breakpoints relevant to the specification
  member x.debugSpec specToDebug = 
    Debugger.Launch() |> ignore
    buildContextAndResolveSpecs () |> ignore
    x.runSpec specToDebug
 
  member x.RunSpecCommand with get () = x._runSpecCommand :> ICommand
  member x.DebugSpecCommand with get () = x._debugSpecCommand :> ICommand
  member x.State 
    with get ()       = _state
    and  set (value)  = 
      _state <- value
      base.OnPropertyChanged("State")
  
  member x.IsDummySpec = x.Name = SpecViewModel.DummySpecName

  static member DummySpecName = "___###DummySpecToShowTreeExpander###___"
  static member Dummy = SpecViewModel ((SpecViewModel.DummySpecName, SpecDelegate(fun () -> AssertionResult.Inconclusive)), (fun () -> []))
  
 
type SpecContainerViewModel (specs : SpecInfo, clazz) =
  inherit TreeViewModel (specs.Name |> removeLeadingGet)
  let _specs = specs
  let _clazz = clazz
  
  let _instantiatedSpecs = ObservableCollection<SpecViewModel>([ SpecViewModel.Dummy ])
  
  let extractSpecs () = 
    match _instantiatedSpecs with
    | xs when xs.Count > 0 && (not _instantiatedSpecs.[0].IsDummySpec)  -> ()
    | otherwise             ->
        _instantiatedSpecs.Clear()
        let buildContextAndResolveSpecs () =
          let instantiatedContext = _clazz |> instantiate
          _specs.Method.Invoke(instantiatedContext, null) :?> (string * SpecDelegate) list
        
        buildContextAndResolveSpecs ()
        |> List.iter (fun spec -> _instantiatedSpecs.Add <| SpecViewModel(spec, buildContextAndResolveSpecs)) 

  member x.Name with get() =  _specs.Name |> removeLeadingGet
  member x.Specifications with get() = _instantiatedSpecs

  override x.OnExpanded () = extractSpecs ()


type ContextViewModel (node : Node) = 
  inherit TreeViewModel (getNodeName node)

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

  member x.Children 
    with get () =
      let lst = new List<TreeViewModel>()
      _childContexts.ForEach(fun x -> lst.Add x)
      _specContainers.ForEach(fun x -> lst.Add x)
      lst

