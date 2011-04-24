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

type ContextViewModel (node : Node, controller) = 
  inherit TreeViewModel (getNodeName node, controller)

  let _childContexts = List<ContextViewModel>()
  let _specContainers = List<SpecContainerViewModel>()
  let mutable _isExpanded = false
  let children = base.Children

  do
    node.Children
    |> Seq.iter (fun c -> _childContexts.Add <| ContextViewModel(c, controller))
    
    node.Context.SpecLists
    |> Seq.map (fun mi -> { Name = mi.Name; Method = mi }) 
    |> Seq.iter (fun si -> _specContainers.Add <| SpecContainerViewModel(si, node.Context, controller))

    Seq.cast<ITreeViewModel>(_childContexts) 
    |> Seq.append(Seq.cast<ITreeViewModel>(_specContainers))
    |> Seq.iter (fun child -> 
        match children |> Seq.filter (fun (c : ITreeViewModel) -> c.Name.Equals(child.Name)) with
        | dups when Seq.isEmpty dups      -> children.Add child
        | dups                            -> child.Children |> Seq.iter (fun c -> dups.First().Children.Add c)
        ) 
    
  member private x._runSpecsCommand = 
    ActionCommand ((fun _ ->
      x.AsITreeViewModel |> resetResolveAndRunSpecs
      x.IsSelected <- true), 
      (fun _ -> Seq.length x.AsITreeViewModel.Children > 0))

  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  member x.ChildContexts with get() = _childContexts
  member x.SpecContainers with get() = _specContainers
  
  interface ITreeViewModel with
    override x.ResolveSpecs () = x.Children |> Seq.iter(fun c -> c.ResolveSpecs ())
   
    override x.RunSpecs completed = 
      x.Children |> Seq.iter(fun c -> c.RunSpecs (fun () ->
        x.AsITreeViewModel.State <- x.aggregateStates
        x.AsITreeViewModel.SpecsRunResult.Clear()
        x.aggregateResults |> Seq.iter(fun r -> x.AsITreeViewModel.SpecsRunResult.Add r)
        completed ())) 

  
