namespace FSharpSpec.GuiRunner
open System.Diagnostics
open System

type TreeViewModel (name, controller : IGuiController) =
  inherit ViewModelBase ()
   
  let mutable _isExpanded = false
  let mutable _isSelected = false
  let mutable _state = NotRunYet
  let mutable _specsRunResult = [ SpecRunResultViewModel (NotRunYet, name) ] |> List.toSeq
  member x.AsITreeViewModel with get () = x :> ITreeViewModel
  
  interface ITreeViewModel with
    override x.Name with get () = name
    override x.Children with get () : seq<ITreeViewModel> = Seq.empty
    
    override x.State 
      with get ()       = _state
      and  set (value : SpecState)  = 
        _state <- value
        x.OnPropertyChanged("State")

    override x.SpecsRunResult 
      with get () :  SpecRunResultViewModel seq = _specsRunResult 
      and set (value) = _specsRunResult <- value
  
    override x.ResetResults () = 
      x.AsITreeViewModel.State <- NotRunYet
      x.AsITreeViewModel.Children |> Seq.iter(fun c -> c.ResetResults ())
      
      match controller with
      | c when c = Unchecked.defaultof<IGuiController>    -> ()
      | c                                                 -> c.ResetResults ()
    
    override x.ResolveSpecs () = ()
    override x.RunSpecs () = ()

    override x.Add child = NotImplementedException("Need to override Add to use it.") |> raise

     
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
        x.OnSelected ()
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
  member x.State with get () = x.AsITreeViewModel.State
  

