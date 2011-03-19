namespace FSharpSpec.GuiRunner
open System.Diagnostics

type TreeViewModel (name, controller : IGuiController) =
  inherit ViewModelBase ()
   
  let mutable _isExpanded = false
  let mutable _isSelected = false
  let mutable _state = NotRunYet
  let mutable _specsRunResult = Seq.empty
  member x.AsI with get () = x :> ITreeViewModel
  
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
      x.AsI.State <- NotRunYet
      x.AsI.Children |> Seq.iter(fun c -> c.Reset ())

    
     
  member x.aggregateStates =
    match x.AsI.Children with
    | xs when xs |> Seq.exists (fun s -> s.State = SpecState.Failed)        -> SpecState.Failed
    | xs when xs |> Seq.exists (fun s -> s.State = SpecState.Inconclusive)  -> SpecState.Inconclusive
    | xs when xs |> Seq.exists (fun s -> s.State = SpecState.NotRunYet)     -> SpecState.NotRunYet
    | otherwise                                                             -> SpecState.Passed
 
  abstract member OnExpanded : unit -> unit
  default x.OnExpanded () = () 
  
  abstract member OnSelected : unit -> unit
  default x.OnSelected () = ()
  
  member x.Children with get () = x.AsI.Children 
  member x.Name with get () = x.AsI.Name 
  member x.State with get () = x.AsI.State
  
  member x.IsSelected 
      with get() = _isSelected
      and set value = 
        _isSelected <- value 
        base.OnPropertyChanged("IsSelected")
        x.OnSelected ()
  
  member x.IsExpanded 
    with get() = _isExpanded
    and set value = 
      _isExpanded <- value 
      base.OnPropertyChanged("IsExpanded")
      x.OnExpanded ()

  member x.aggregateResults = x.AsI.Children |> Seq.collect(fun c -> c.SpecsRunResult) 

  override x.ToString() = x.AsI.Name



