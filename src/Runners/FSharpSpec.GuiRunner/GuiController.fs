namespace FSharpSpec.GuiRunner
open System.Diagnostics
open FSharpSpec

type GuiController () =
  
  let mutable _guiRunnerViewModel : IGuiRunnerViewModel = Unchecked.defaultof<IGuiRunnerViewModel>
  
  member x.GuiRunnerViewModel  
    with get () = _guiRunnerViewModel 
    and set (v) = _guiRunnerViewModel <- v

  interface IGuiController with
    override x.Selected treeViewModel =
      x.GuiRunnerViewModel.UpdateSpecsRunResult treeViewModel.SpecsRunResult 

    override x.AddAssembly asm =
      Debug.WriteLine("Adding " + asm)
     
    override x.RemoveAssembly asm =
      Debug.WriteLine("Remove " + asm)

    override x.ResetResults () = 
      x.GuiRunnerViewModel.ResetSpecs ()

    override x.RegisterSpecs specs = 
      specs |> Seq.length |> x.GuiRunnerViewModel.RegisterSpecs

    override x.ReportResult result = 
      match result with
      | Passed        -> x.GuiRunnerViewModel.PassedSpec ()
      | Pending       -> x.GuiRunnerViewModel.PendingSpec ()
      | Inconclusive  -> x.GuiRunnerViewModel.InconclusiveSpec ()
      | Failed        -> x.GuiRunnerViewModel.FailedSpec ()
