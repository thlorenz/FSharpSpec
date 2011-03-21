namespace FSharpSpec.GuiRunner
open System.Diagnostics

type TreeViewModel (name, controller : IGuiController) =
  inherit ViewModelBase ()
   
  let mutable _isExpanded = false
  let mutable _isSelected = false
  let mutable _state = NotRunYet
  let mutable _specsRunResult = Seq.empty
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
  
    override x.Reset () = 
      x.AsITreeViewModel.State <- NotRunYet
      x.AsITreeViewModel.Children |> Seq.iter(fun c -> c.Reset ())

     
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
        if _isSelected then controller.Selected x.AsITreeViewModel
        x.OnSelected ()
        base.OnPropertyChanged("IsSelected")
  
  member x.IsExpanded 
    with get() = _isExpanded
    and set value = 
      _isExpanded <- value 
      base.OnPropertyChanged("IsExpanded")
      x.OnExpanded ()

  member x.aggregateResults = x.AsITreeViewModel.Children |> Seq.collect(fun c -> c.SpecsRunResult) 

  override x.ToString() = x.AsITreeViewModel.Name

  // Need these to ensure proper DataBinding
  member x.Children with get () = x.AsITreeViewModel.Children 
  member x.Name with get () = x.AsITreeViewModel.Name 
  member x.State with get () = x.AsITreeViewModel.State
  

