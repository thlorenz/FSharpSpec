namespace FSharpSpec.GuiRunner

type GuiController () =
  
  [<DefaultValue>]
  val mutable guiRunnerViewModel : IGuiRunnerViewModel
  
  member x.GuiRunnerViewModel  
    with get () = x.guiRunnerViewModel 
    and set (value) = x.guiRunnerViewModel <- value

  interface IGuiController with
    override x.Selected treeViewModel =
      x.GuiRunnerViewModel.UpdateSpecsRunResult treeViewModel.SpecsRunResult 

