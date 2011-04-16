namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel
open Microsoft.Win32
open System.Windows.Input
open System.Diagnostics

open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils

type GuiRunnerViewModel (contextRoot : ITreeViewModel, controller : IGuiController) =
  inherit ViewModelBase()

  let mutable _specsRunResults = ObservableCollection<SpecRunResultViewModel>()
  let mutable _registeredSpecs = 0
  let mutable _finishedSpecs = 0
  let mutable _passedSpecs = 0
  let mutable _inconclusiveSpecs = 0
  let mutable _pendingSpecs = 0
  let mutable _failedSpecs = 0
  let mutable _finishedSpecs = 0

  
  interface IGuiRunnerViewModel with 
    override x.UpdateSpecsRunResult results = 
      _specsRunResults.Clear()
      results |> Seq.iter _specsRunResults.Add
    override x.Root with get () = contextRoot 
   
    override x.ResetSpecs () = 
      x.RegisteredSpecs     <- 0
      x.PassedSpecs         <- 0
      x.InconclusiveSpecs   <- 0
      x.PendingSpecs        <- 0
      x.FailedSpecs         <- 0 
      x.FinishedSpecs       <- 0

    override x.RegisterSpecs specs  = x.RegisteredSpecs <- x.RegisteredSpecs + specs

    override x.PassedSpec          () = x.PassedSpecs <- x.PassedSpecs + 1 
                                        x.FinishedSpecs <- x.GetFinishedSpecs ()

    override x.InconclusiveSpec    () = x.InconclusiveSpecs <- x.InconclusiveSpecs + 1 
                                        x.FinishedSpecs <- x.GetFinishedSpecs ()

    override x.PendingSpec         () = x.PendingSpecs <- x.PendingSpecs + 1 
                                        x.FinishedSpecs <- x.GetFinishedSpecs ()

    override x.FailedSpec          () = x.FailedSpecs <- x.FailedSpecs + 1 
                                        x.FinishedSpecs <- x.GetFinishedSpecs ()
      
  member private x._addAssemblyCommand = ActionCommand ((fun _ -> x.addAssembly ()), (fun _ -> true))
  member private x.addAssembly () = x.loadAssembly

  member x.AsIGuiRunnerViewModel = x :> IGuiRunnerViewModel
  
  member x.SpecsRunResults with get () = _specsRunResults
  member x.AddAssemblyCommand with get () = x._addAssemblyCommand :> ICommand

  
  member x.Root with get () = x.AsIGuiRunnerViewModel.Root

  member x.RegisteredSpecs 
    with get () = _registeredSpecs 
    and set (v) = _registeredSpecs <- v; 
                  Debug.WriteLine (sprintf "Registered %d" _registeredSpecs)
                  x.OnPropertyChanged("RegisteredSpecs")

  member x.PassedSpecs 
    with get () = _passedSpecs 
    and set (v) = _passedSpecs <- v; 
                  x.OnPropertyChanged("PassedSpecs")

  member x.InconclusiveSpecs 
    with get () = _inconclusiveSpecs 
    and set (v) = _inconclusiveSpecs <- v; 
                  x.OnPropertyChanged("InconclusiveSpecs")

  member x.PendingSpecs 
    with get () = _pendingSpecs 
    and set (v) = _pendingSpecs <- v; 
                  x.OnPropertyChanged("PendingSpecs")

  member x.FailedSpecs 
    with get () = _failedSpecs 
    and set (v) = _failedSpecs <- v; 
                  x.OnPropertyChanged("FailedSpecs")
  
  member x.FinishedSpecs 
    with get () = _finishedSpecs
    and set (v) = _finishedSpecs <- v
                  x.OnPropertyChanged("FinishedSpecs")

  member private x.GetFinishedSpecs () = 
    _passedSpecs + _pendingSpecs + _inconclusiveSpecs + _failedSpecs

  member private x.loadAssembly  = 
  
    let openAssembly = 
      let dlg = OpenFileDialog(DefaultExt = ".dll,.exe", Filter = "Assemblies (*.dll, *.exe)|*.dll;*.exe|All files (*.*)|*.*")
      let result = dlg.ShowDialog()
      match result with
      | r when r.HasValue && r.Value  -> Some(dlg.FileName)
      | _                             -> None 

    match openAssembly with
    | Some(path)   -> contextRoot.Add (AssemblyViewModel(path |> getAssembly, controller))
    | None        ->  Debug.WriteLine("Canceled") |> ignore

    