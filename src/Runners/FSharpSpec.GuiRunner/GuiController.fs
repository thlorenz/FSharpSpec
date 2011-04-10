namespace FSharpSpec.GuiRunner
open System.Diagnostics

type GuiController () =
  
  [<DefaultValue>]
  val mutable guiRunnerViewModel : IGuiRunnerViewModel
  
  member x.GuiRunnerViewModel  
    with get () = x.guiRunnerViewModel 
    and set (value) = x.guiRunnerViewModel <- value

  interface IGuiController with
    override x.Selected treeViewModel =
      x.GuiRunnerViewModel.UpdateSpecsRunResult treeViewModel.SpecsRunResult 

    override x.AddAssembly asm =
      Debug.WriteLine("Adding " + asm)
     
    override x.RemoveAssembly asm =
      Debug.WriteLine("Remove " + asm)