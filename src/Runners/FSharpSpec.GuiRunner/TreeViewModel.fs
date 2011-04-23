namespace FSharpSpec.GuiRunner
open System.Diagnostics
open System
open System.Collections.ObjectModel

type TreeViewModel (name, controller : IGuiController) =
  inherit ViewModelBase ()
   
  let mutable _isExpanded = false
  let mutable _isSelected = false
  let mutable _state = NotRunYet
  let mutable _specsRunResult = ObservableCollection ([ SpecRunResultViewModel (NotRunYet, name) ])
  let mutable _children = ObservableCollection()
  member x.AsITreeViewModel with get () = x :> ITreeViewModel
  
  interface ITreeViewModel with
    override x.Name with get () = name
    override x.Children with get () = _children
    
    override x.State with get () = x.State and set (v : SpecState) = x.State <- v

    override x.SpecsRunResult 
      with get () = _specsRunResult
  
    override x.ResetResults () = 
      x.AsITreeViewModel.State <- NotRunYet
      x.AsITreeViewModel.Children |> Seq.iter(fun c -> c.ResetResults ())
      
      match controller with
      | c when c = Unchecked.defaultof<IGuiController>    -> ()
      | c                                                 -> c.ResetResults ()
    
    override x.ResolveSpecs () = ()
    override x.RunSpecs completed = completed ()
     
  member x.aggregateStates =
    match x.AsITreeViewModel.Children with
    | xs when xs |> Seq.exists (fun s -> s.State = SpecState.Failed)        -> SpecState.Failed
    | xs when xs |> Seq.exists (fun s -> s.State = SpecState.Inconclusive)  -> SpecState.Inconclusive
    | xs when xs |> Seq.exists (fun s -> s.State = SpecState.NotRunYet)     -> SpecState.NotRunYet
    | otherwise                                                             -> SpecState.Passed
  
  abstract member OnExpanded : unit -> unit
  default x.OnExpanded () = () 
  
  abstract member OnSelected : unit -> unit
  default x.OnSelected () = ()
 
  member x.IsSelected 
      with get() = _isSelected
      and set value = 
        _isSelected <- value 
        x.refreshResults ()
        if (x.IsSelected) then x.OnSelected ()
        base.OnPropertyChanged("IsSelected")
  
  member x.IsExpanded 
    with get() = _isExpanded
    and set value = 
      _isExpanded <- value 
      base.OnPropertyChanged("IsExpanded")
      x.OnExpanded ()

  member x.aggregateResults = x.AsITreeViewModel.Children |> Seq.collect(fun c -> c.SpecsRunResult) 
  member x.refreshResults () = if _isSelected then controller.Selected x.AsITreeViewModel
  
  override x.ToString() = x.AsITreeViewModel.Name

  // Need these to ensure proper DataBinding
  member x.Children with get () = x.AsITreeViewModel.Children 
  member x.Name with get () = x.AsITreeViewModel.Name 
  member x.State 
    with get ()               = _state
    and  set (v : SpecState)  = 
      _state <- v
      x.OnPropertyChanged("State")
  

