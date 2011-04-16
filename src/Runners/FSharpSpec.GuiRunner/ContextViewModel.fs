﻿namespace FSharpSpec.GuiRunner

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

  do
    node.Children
    |> Seq.iter (fun c -> _childContexts.Add <| ContextViewModel(c, controller))
    
    node.Context.SpecLists
    |> Seq.map (fun mi -> { Name = mi.Name; Method = mi }) 
    |> Seq.iter (fun si -> _specContainers.Add <| SpecContainerViewModel(si, node.Context, controller))

  let _children = Seq.cast<ITreeViewModel>(_childContexts) |> Seq.append(Seq.cast<ITreeViewModel>(_specContainers))
    
  member private x._runSpecsCommand = 
    ActionCommand ((fun _ ->
      x.AsITreeViewModel |> resetResolveAndRunSpecs
      x.IsSelected <- true), 
      (fun _ -> Seq.length x.AsITreeViewModel.Children > 0))

  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  member x.ChildContexts with get() = _childContexts
  member x.SpecContainers with get() = _specContainers
  
  interface ITreeViewModel with
    override x.Children with get () = _children
    
    override x.ResolveSpecs () = x.Children |> Seq.iter(fun c -> c.ResolveSpecs ())
   
    override x.RunSpecs () = 
      x.Children |> Seq.iter(fun c -> c.RunSpecs ())
      x.AsITreeViewModel.State <- x.aggregateStates
      x.AsITreeViewModel.SpecsRunResult <- x.aggregateResults 

  
