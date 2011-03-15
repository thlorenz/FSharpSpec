namespace FSharpSpec.GuiRunner

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



