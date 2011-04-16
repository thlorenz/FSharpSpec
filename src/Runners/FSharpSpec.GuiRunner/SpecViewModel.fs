namespace FSharpSpec.GuiRunner

open System.Diagnostics
open System.Windows.Input

open FSharpSpec

type SpecViewModel (specInfo : (string * SpecDelegate), controller : IGuiController, buildContextAndResolveSpecs : unit -> (string * SpecDelegate) list, getFullNameOfSpec) =
  inherit TreeViewModel (fst specInfo, controller)

  let _spec = snd specInfo

  let mutable _specRunResult = getFullNameOfSpec (fst specInfo) |> SpecRunResultViewModel.NotRunYet  
  let getResult state = SpecRunResultViewModel (state, getFullNameOfSpec (fst specInfo))
  let getResult1 state exn = SpecRunResultViewModel (state, getFullNameOfSpec (fst specInfo), exn)

  member private x._runSpecCommand = ActionCommand ((fun _ -> x.runSpec; x.IsSelected <- true), (fun _ -> true))
  member private x._debugSpecCommand = ActionCommand ((fun _ -> x.debugSpec), (fun _ -> true))
 
  member x.Spec with get () = specInfo

  member x.runSpec = 
    try
      let outcome = _spec.Method.Invoke(_spec.Target, null) :?> AssertionResult  
      x.AsITreeViewModel.State <- outcome |> toSpecState
      _specRunResult <- getResult x.AsITreeViewModel.State
      x.AsITreeViewModel.SpecsRunResult <- [_specRunResult]
      controller.ReportResult outcome
      
    with
      exn              -> 
        x.AsITreeViewModel.State <- SpecState.Failed
        _specRunResult <- getResult1 x.AsITreeViewModel.State exn
        x.AsITreeViewModel.SpecsRunResult <- [_specRunResult]
        controller.ReportResult Failed
    
    if x.IsSelected then x.OnSelected ()


  /// Re-evaluates the Context after launching the debugger in order to hit all possible breakpoints relevant to the specification
  member x.debugSpec = 
    Debugger.Launch() |> ignore
    buildContextAndResolveSpecs () |> ignore
    x.runSpec
 
  member x.RunSpecCommand with get () = x._runSpecCommand :> ICommand
  member x.DebugSpecCommand with get () = x._debugSpecCommand :> ICommand
  
  member x.IsDummySpec = x.AsITreeViewModel.Name = SpecViewModel.DummySpecName
  
  static member DummySpecName = "___DummySpecToShowTreeExpander___GUID:0D46D658-A328-466C-873F-B4BA1E394E5D"
  static member Dummy = SpecViewModel ((SpecViewModel.DummySpecName, SpecDelegate(fun () -> AssertionResult.Inconclusive)), Unchecked.defaultof<IGuiController> , (fun () -> []), (fun _ -> null))
