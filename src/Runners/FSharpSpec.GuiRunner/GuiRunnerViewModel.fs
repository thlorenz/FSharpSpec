namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel

type GuiRunnerViewModel (contextRoot, controller) =
  let mutable _specsRunResults = ObservableCollection<SpecRunResultViewModel>()
  
  interface IGuiRunnerViewModel with 
    override x.UpdateSpecsRunResult results = 
      _specsRunResults.Clear()
      results |> Seq.iter _specsRunResults.Add
      
  
  member x.AsIGuiRunnerViewModel = x :> IGuiRunnerViewModel
  member x.Root with get () = contextRoot
  member x.SpecsRunResults with get () = _specsRunResults
