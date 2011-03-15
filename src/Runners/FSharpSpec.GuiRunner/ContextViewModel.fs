namespace FSharpSpec.GuiRunner

open System.Diagnostics
open System.Windows.Input

open System.Collections.ObjectModel
open System.Collections.Generic

open System.Linq

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils  

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
