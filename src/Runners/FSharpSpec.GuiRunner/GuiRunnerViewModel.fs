namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel

type GuiRunnerViewModel (contextRoot, controller) =
  let mutable _specsRunResults  = ObservableCollection<SpecRunResultViewModel>()
  
  interface IGuiRunnerViewModel with 
    override x.UpdateSpecsRunResult results = ()

  member x.Root with get () = contextRoot
  member x.SpecsRunResults with get () = _specsRunResults
