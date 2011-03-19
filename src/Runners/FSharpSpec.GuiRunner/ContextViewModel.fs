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

  let _childContexts = List<ContextViewModel>()
  let _specContainers = List<SpecContainerViewModel>()
  let mutable _isExpanded = false

  do
    node.Children
    |> Seq.iter (fun c -> _childContexts.Add <| ContextViewModel(c, specsRunResult))
    
    node.Context.SpecLists
    |> Seq.map (fun mi -> { Name = mi.Name; Method = mi }) 
    |> Seq.iter (fun si -> _specContainers.Add <| SpecContainerViewModel(si, node.Context, specsRunResult))

  let _children = Seq.cast<TreeViewModel>(_childContexts) |> Seq.append(Seq.cast<TreeViewModel>(_specContainers))
    
  member x.runSpecs = 
    x.Reset ()
    x.ChildContexts |> Seq.iter (fun (c : ContextViewModel) -> c.runSpecs)
    x.SpecContainers |> Seq.iter (fun (c : SpecContainerViewModel) -> c.runSpecs)
    
    
    x.State <- x.Children |> TreeViewModel.aggregatedResults 

  member private x._runSpecsCommand = ActionCommand ((fun _ -> x.runSpecs), (fun _ -> Seq.length x.Children > 0))
  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  member x.ChildContexts with get() = _childContexts
  member x.SpecContainers with get() = _specContainers
  member x.IsExpanded 
    with get ()       = _isExpanded
    and  set (value)  = _isExpanded <- value

  override x.Children with get () = _children
      

  
