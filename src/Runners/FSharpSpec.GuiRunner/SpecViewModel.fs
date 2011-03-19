namespace FSharpSpec.GuiRunner

open System.Diagnostics
open System.Windows.Input

open FSharpSpec



type SpecViewModel (specInfo : (string * SpecDelegate), controller, buildContextAndResolveSpecs : unit -> (string * SpecDelegate) list, getFullNameOfSpec) =
  inherit TreeViewModel (fst specInfo, controller)

  let _spec = snd specInfo

  let mutable _specRunResult = getFullNameOfSpec (fst specInfo) |> SpecRunResultViewModel.NotRunYet  
  let getResult state = SpecRunResultViewModel (state, getFullNameOfSpec (fst specInfo))

  member private x._runSpecCommand = ActionCommand ((fun _ -> x.runSpec), (fun _ -> true))
  member private x._debugSpecCommand = ActionCommand ((fun _ -> x.debugSpec), (fun _ -> true))
 
  member x.runSpec = 
    try
      let outcome = _spec.Method.Invoke(_spec.Target, null) :?> AssertionResult  
      x.AsI.State <- outcome |> toSpecState
      _specRunResult <- getResult x.AsI.State
      x.AsI.SpecsRunResult <- [_specRunResult]
      if x.IsSelected then x.OnSelected ()
    with
      ex              -> 
        x.AsI.State <- SpecState.Failed
        _specRunResult <- getResult x.AsI.State

  /// Re-evaluates the Context after launching the debugger in order to hit all possible breakpoints relevant to the specification
  member x.debugSpec = 
    Debugger.Launch() |> ignore
    buildContextAndResolveSpecs () |> ignore
    x.runSpec
 
  member x.RunSpecCommand with get () = x._runSpecCommand :> ICommand
  member x.DebugSpecCommand with get () = x._debugSpecCommand :> ICommand
  
  member x.IsDummySpec = x.AsI.Name = SpecViewModel.DummySpecName
  
  static member DummySpecName = "___DummySpecToShowTreeExpander___GUID:0D46D658-A328-466C-873F-B4BA1E394E5D"
  static member Dummy = SpecViewModel ((SpecViewModel.DummySpecName, SpecDelegate(fun () -> AssertionResult.Inconclusive)), GuiController() :> IGuiController , (fun () -> []), (fun _ -> null))
