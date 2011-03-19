namespace FSharpSpec.GuiRunner
open System.Diagnostics
type TreeViewModel (name, specsRunResult) =
   inherit ViewModelBase ()
   
   let mutable _isExpanded = false
   let mutable _isSelected = false
   let mutable _state = NotRunYet

   abstract member Name : string
   default x.Name with get () = name

   abstract member Children : seq<TreeViewModel>
   default x.Children with get () = Seq.empty
      
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
      Debug.WriteLine("{0} -> {1}", x.Name, x.State |> toSpecStateDisplay)
      x.OnPropertyChanged("State")

   member x.SpecsRunResult with get () : SpecsRunResult = specsRunResult

   abstract Reset : unit -> unit
   default x.Reset () = 
     Debug.WriteLine("Resetting: " + x.Name)
     x.State <- NotRunYet
     x.Children |> Seq.iter(fun c -> c.Reset ())
     
  
   static member aggregatedResults (treeViewModels : seq<TreeViewModel>) =
      match treeViewModels with
      | xs when xs |> Seq.exists (fun s -> s.State = SpecState.Failed)        -> SpecState.Failed
      | xs when xs |> Seq.exists (fun s -> s.State = SpecState.Inconclusive)  -> SpecState.Inconclusive
      | xs when xs |> Seq.exists (fun s -> s.State = SpecState.NotRunYet)     -> SpecState.NotRunYet
      | otherwise                                                             -> SpecState.Passed

   override x.ToString() = x.Name



