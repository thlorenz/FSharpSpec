﻿namespace FSharpSpec.GuiRunner

open System
open System.Collections.ObjectModel
open Microsoft.Win32
open System.Windows.Input
open System.Diagnostics
open System.Collections.Generic
open System.IO

open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils

type GuiRunnerViewModel (contextRoot : ITreeViewModel, controller : IGuiController) =
  inherit ViewModelBase()

  let mutable _specsRunResults = ObservableCollection<SpecRunResultViewModel>()
  let mutable _selectedResult = null
  let mutable _registeredSpecs = 0
  let mutable _finishedSpecs = 0
  let mutable _passedSpecs = 0
  let mutable _inconclusiveSpecs = 0
  let mutable _pendingSpecs = 0
  let mutable _failedSpecs = 0
  let mutable _finishedSpecs = 0
  let mutable _overallState = NotRunYet

  let mutable _loadedAssemblies = List<string>()

  
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
      x.UpdateProgress ()

    override x.RegisterSpecs specs  = x.RegisteredSpecs <- x.RegisteredSpecs + specs
    override x.PassedSpec          () = x.PassedSpecs <- x.PassedSpecs + 1 
                                        x.UpdateProgress ()

    override x.InconclusiveSpec    () = x.InconclusiveSpecs <- x.InconclusiveSpecs + 1 
                                        x.UpdateProgress ()

    override x.PendingSpec         () = x.PendingSpecs <- x.PendingSpecs + 1 
                                        x.UpdateProgress ()

    override x.FailedSpec          () = x.FailedSpecs <- x.FailedSpecs + 1 
                                        x.UpdateProgress ()
      
  member private x._addAssemblyCommand = ActionCommand ((fun _ -> x.addAssembly ()), (fun _ -> true))
  member private x.addAssembly () = x.loadAssembly

  member x.AsIGuiRunnerViewModel = x :> IGuiRunnerViewModel
  
  member x.SpecsRunResults with get () = _specsRunResults
  member x.AddAssemblyCommand with get () = x._addAssemblyCommand :> ICommand

  
  member x.Root with get () = x.AsIGuiRunnerViewModel.Root

  member x.RegisteredSpecs 
    with get () = _registeredSpecs 
    and set (v) = _registeredSpecs <- v; 
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

  member x.OverallState
    with get () = _overallState
    and set (v) = _overallState <- v
                  x.OnPropertyChanged("OverallState")

  member x.SelectedResult 
    with get () = _selectedResult 
    and set (v) = _selectedResult <- v
                  x.OnPropertyChanged("SelectedResult")
 
  member x.RunAllSpecsCommand with get () = x._runAllSpecsCommand
    
  member private x._runAllSpecsCommand = 
    ActionCommand((fun _ -> 
      contextRoot.Children 
      |> Seq.iter (fun child -> child |> resetResolveAndRunSpecs)),
      (fun _ -> Seq.length contextRoot.Children  > 0))

  member x.ReloadAssembliesCommand with get () = x._reloadAssembliesCommand

  member private x._reloadAssembliesCommand =
    ActionCommand((fun _ -> x.ReloadAssemblies ()), (fun _ -> Seq.length _loadedAssemblies > 0))

  member private x.GetFinishedSpecs () = 
    _passedSpecs + _pendingSpecs + _inconclusiveSpecs + _failedSpecs
  
  member private x.GetOverallState () =
    match (x.PassedSpecs, x.InconclusiveSpecs, x.FailedSpecs) with
    | p, i, f when f > 0  -> Failed
    | p, i, f when i > 0  -> Inconclusive
    | p, i, f when p > 0  -> Passed
    | _                   -> NotRunYet
  
  member private x.UpdateProgress () =
    x.FinishedSpecs <- x.GetFinishedSpecs ()
    x.OverallState <- x.GetOverallState ()

  /// Copies the specs dlls folder recursively to a temp folder and returns the path to the copied specs dll
  member private x.makeShadowCopy path =
    let destination file targetDir = 
        let name = Path.GetFileName file
        Path.Combine (targetDir, name)

    let rec copyFolder sourceDir targetDir =
     
      Directory.GetFiles(sourceDir)
      |> Array.iter (fun file -> 
        let dest = destination file targetDir
        File.Copy(file, dest))
     
      Directory.GetDirectories(sourceDir)
      |> Array.iter(fun dir ->
        let dest = destination dir targetDir
        copyFolder sourceDir dest)
    
    let tempPath = Path.GetTempPath()
    let fileInfo = FileInfo(path)
    let sourceDir = fileInfo.DirectoryName
    let targetDir = Path.Combine(tempPath, sprintf "FSharpSpec.GuiRunner_%s" <| Guid.NewGuid().ToString())
    
    let target = Directory.CreateDirectory targetDir
   
    copyFolder sourceDir targetDir

    Debug.WriteLine (sprintf "Copied from %s to %s" sourceDir targetDir)

    Path.Combine(targetDir, fileInfo.Name)

  member private x.loadAndAddAssembly path = 
    let copiedSpecsDll = x.makeShadowCopy path
    contextRoot.Children.Add (AssemblyViewModel(copiedSpecsDll |> getAssembly, controller))

  member private x.loadAssembly  = 
  
    let openAssembly = 
      let dlg = OpenFileDialog(DefaultExt = ".dll,.exe", Filter = "Assemblies (*.dll, *.exe)|*.dll;*.exe|All files (*.*)|*.*")
      let result = dlg.ShowDialog()
      match result with
      | r when r.HasValue && r.Value  -> Some(dlg.FileName)
      | _                             -> None 

    match openAssembly with
    | Some(path)   -> x.loadAndAddAssembly path
                      _loadedAssemblies.Add path
                      
    | None        ->  Debug.WriteLine("Canceled") |> ignore
  


  member private x.ReloadAssemblies () =
    controller.ResetResults ()
    contextRoot.Children.Clear()
    _loadedAssemblies
    |> Seq.iter(fun path -> x.loadAndAddAssembly path)
    
    