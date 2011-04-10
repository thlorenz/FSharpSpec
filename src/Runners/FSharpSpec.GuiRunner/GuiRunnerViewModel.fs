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
 
  
  interface IGuiRunnerViewModel with 
    override x.UpdateSpecsRunResult results = 
      _specsRunResults.Clear()
      results |> Seq.iter _specsRunResults.Add
    override x.Root with get () = contextRoot  
      
  member private x._addAssemblyCommand = ActionCommand ((fun _ -> x.addAssembly ()), (fun _ -> true))
  member private x.addAssembly () = x.loadAssembly

  member x.AsIGuiRunnerViewModel = x :> IGuiRunnerViewModel
  
  member x.SpecsRunResults with get () = _specsRunResults
  member x.AddAssemblyCommand with get () = x._addAssemblyCommand :> ICommand

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

  member x.Root with get () = x.AsIGuiRunnerViewModel.Root