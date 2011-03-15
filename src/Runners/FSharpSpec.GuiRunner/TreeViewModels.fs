namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel
open System.Collections.Generic
open System.Reflection
open System.Diagnostics
open System.Linq

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


type TreeViewModel (name, specsRunResult) =
   inherit ViewModelBase ()
   
   let mutable _isExpanded = false
   let mutable _isSelected = false
   let mutable _state = NotRunYet

   member x.Name with get () = name
  
   member x.IsExpanded 
     with get() = _isExpanded
     and set value = 
       _isExpanded <- value 
       base.OnPropertyChanged("IsExpanded")
       x.OnExpanded ()

   abstract member OnExpanded : unit -> unit
   default x.OnExpanded () = ()

   member x.IsSelected 
     with get() = _isSelected
     and set value = 
       _isSelected <- value 
       base.OnPropertyChanged("IsSelected")
       x.OnSelected ()

   abstract member OnSelected : unit -> unit
   default x.OnSelected () = ()

   member x.State 
    with get ()       = _state
    and  set (value : SpecState)  = 
      _state <- value
      base.OnPropertyChanged("State")

   member x.SpecsRunResult with get () : SpecsRunResult = specsRunResult

   static member aggregatedResults (treeViewModels : seq<TreeViewModel>) =
      match treeViewModels with
      | xs when xs |> Seq.exists (fun s -> s.State = SpecState.Failed)        -> SpecState.Failed
      | xs when xs |> Seq.exists (fun s -> s.State = SpecState.Inconclusive)  -> SpecState.Inconclusive
      | xs when xs |> Seq.exists (fun s -> s.State = SpecState.NotRunYet)     -> SpecState.NotRunYet
      | otherwise                                                             -> SpecState.Passed

   override x.ToString() = x.Name

type SpecViewModel (specInfo : (string * SpecDelegate), specsRunResult, buildContextAndResolveSpecs, getFullNameOfSpec) =
  inherit TreeViewModel (fst specInfo, specsRunResult)

  let _spec = snd specInfo

  
  let mutable _specRunResult = getFullNameOfSpec (fst specInfo) |> SpecRunResultViewModel.NotRunYet  

  member private x._runSpecCommand = ActionCommand ((fun _ -> x.runSpec), (fun _ -> true))
  member private x._debugSpecCommand = ActionCommand ((fun _ -> x.debugSpec), (fun _ -> true))
 
  member x.runSpec = 
    try
      let outcome = _spec.Method.Invoke(_spec.Target, null) :?> AssertionResult  
      x.State <- outcome |> toSpecState
      _specRunResult <- SpecRunResultViewModel (x.State, getFullNameOfSpec (fst specInfo))
      if x.IsSelected then x.OnSelected ()
    with
      ex              -> x.State <- SpecState.Failed

  /// Re-evaluates the Context after launching the debugger in order to hit all possible breakpoints relevant to the specification
  member x.debugSpec = 
    Debugger.Launch() |> ignore
    buildContextAndResolveSpecs () |> ignore
    x.runSpec
 
  member x.RunSpecCommand with get () = x._runSpecCommand :> ICommand
  member x.DebugSpecCommand with get () = x._debugSpecCommand :> ICommand
  

  override x.OnSelected () = 
    x.SpecsRunResult.Items.Clear()
    x.SpecsRunResult.Items.Add _specRunResult
      
  member x.IsDummySpec = x.Name = SpecViewModel.DummySpecName

  static member DummySpecName = "___DummySpecToShowTreeExpander___GUID:0D46D658-A328-466C-873F-B4BA1E394E5D"
  static member Dummy = SpecViewModel ((SpecViewModel.DummySpecName, SpecDelegate(fun () -> AssertionResult.Inconclusive)), new SpecsRunResult(), (fun () -> []), (fun _ -> null))
  
 
type SpecContainerViewModel (specs : SpecInfo, context, specRunResults) =
  inherit TreeViewModel (specs.Name |> removeLeadingGet, specRunResults)
  
  let _instantiatedSpecs = ObservableCollection<SpecViewModel>([ SpecViewModel.Dummy ])
  let getFullNameOfSpec = getFullSpecName context specs.Method.Name
  

  let extractSpecs () = 
    match _instantiatedSpecs with
    | xs when xs.Count > 0 && (not _instantiatedSpecs.[0].IsDummySpec)  -> ()
    | otherwise             ->
        _instantiatedSpecs.Clear()
        let buildContextAndResolveSpecs () =
          let instantiatedContext = context.Clazz |> instantiate
          specs.Method.Invoke(instantiatedContext, null) :?> (string * SpecDelegate) list
        
        buildContextAndResolveSpecs ()
        |> List.iter (fun spec -> _instantiatedSpecs.Add <| SpecViewModel(spec, specRunResults, buildContextAndResolveSpecs, getFullNameOfSpec)) 

  member x.runSpecs = 
    extractSpecs ()
    _instantiatedSpecs |> Seq.iter (fun s -> s.runSpec)
    x.State <- _instantiatedSpecs.Cast<TreeViewModel>() |> TreeViewModel.aggregatedResults 

  member private x._runSpecsCommand = ActionCommand ((fun _ -> x.runSpecs), (fun _ -> true))

  member x.Name with get() =  specs.Name |> removeLeadingGet
  member x.Specifications with get() = _instantiatedSpecs

  override x.OnExpanded () = extractSpecs ()

  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

type ContextViewModel (node : Node, specsRunResult : SpecsRunResult) = 
  inherit TreeViewModel (getNodeName node, specsRunResult)

  let _node = node
  let _childContexts = List<ContextViewModel>()
  let _specContainers = List<SpecContainerViewModel>()
  let mutable _isExpanded = false

  do
    _node.Children
    |> Seq.iter (fun c -> _childContexts.Add <| ContextViewModel(c, specsRunResult))
    
    _node.Context.SpecLists
    |> Seq.map (fun mi -> { Name = mi.Name; Method = mi }) 
    |> Seq.iter (fun si -> _specContainers.Add <| SpecContainerViewModel(si, _node.Context, specsRunResult))
    

  member x.Name 
    with get() = 
      match _node.Context.Clazz.Name with
      | x when x <> "Object"  -> x
      | otherwise             -> "Specifications"

   member x.runSpecs = 
    x.ChildContexts |> Seq.iter (fun (c : ContextViewModel) -> c.runSpecs)
    x.SpecContainers |> Seq.iter (fun (c : SpecContainerViewModel) -> c.runSpecs)
    x.State <- x.Children |> TreeViewModel.aggregatedResults 

  member private x._runSpecsCommand = ActionCommand ((fun _ -> x.runSpecs), (fun _ -> x.Children.Count > 0))
  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  member x.ChildContexts with get() = _childContexts
  member x.SpecContainers with get() = _specContainers
  member x.IsExpanded 
    with get ()       = _isExpanded
    and  set (value)  = _isExpanded <- value

  member x.Children
    with get () : List<TreeViewModel> =
      let lst = new List<TreeViewModel>()
      _childContexts.ForEach(fun x -> lst.Add x)
      _specContainers.ForEach(fun x -> lst.Add x)
      lst

