namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel

type GuiController () =
  let _specRunResults = ObservableCollection<SpecRunResultViewModel>()
  
  interface IGuiController with
    override x.Selected t = ()  
    override x.SpecRunResults with get () = _specRunResults

