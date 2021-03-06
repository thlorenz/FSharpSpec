﻿namespace FSharpSpec.GuiRunner

open System.Diagnostics
open System.Windows.Input
open System.ComponentModel
open FSharpSpec

type SpecViewModel (specInfo : (string * SpecDelegate), controller : IGuiController, buildContextAndResolveSpecs : unit -> (string * SpecDelegate) list option * exn * string, getFullNameOfSpec) =
  inherit TreeViewModel (fst specInfo, controller)

  let _spec = snd specInfo

  let mutable _specRunResult = getFullNameOfSpec (fst specInfo) |> SpecRunResultViewModel.NotRunYet  
  let mutable _inconclusiveException = null

  let getResult state = SpecRunResultViewModel (state, getFullNameOfSpec (fst specInfo))
  let getResult1 state exn = SpecRunResultViewModel (state, getFullNameOfSpec (fst specInfo), exn)
  let getInconclusiveResult excep = SpecRunResultViewModel(SpecState.Inconclusive, getFullNameOfSpec (fst specInfo), excep)
 
  let resetAndRegisterSpec () = 
    controller.ResetResults ()
    controller.RegisterSpecs [specInfo]

  let runSpecification () = 
    try
      // System.Threading.Thread.Sleep(50)
      (_spec.Method.Invoke(_spec.Target, null) :?> AssertionResult, null :> exn )
    with
      ex               -> (Failed, ex)
  
  member private x.reportRunResult result =
    let outcome, excep =  result
  
    x.AsITreeViewModel.State <- outcome |> toSpecState
    
    match outcome with
    | Inconclusive -> _specRunResult <- getInconclusiveResult x.InconclusiveException
    | _            -> 
      match excep with
      | ex when ex = null       -> _specRunResult <- getResult x.AsITreeViewModel.State 
      | ex                      -> _specRunResult <- getResult1 x.AsITreeViewModel.State ex

    x.AsITreeViewModel.SpecsRunResult.Clear()
    x.AsITreeViewModel.SpecsRunResult.Add _specRunResult

    controller.ReportResult outcome
    
    if x.IsSelected then x.OnSelected ()


  member private x._runSpecCommand = 
    ActionCommand ((fun _ -> 
      resetAndRegisterSpec ()
      x.runSpec (fun () -> ())
      x.IsSelected <- true), 
      (fun _ -> true))

  member private x._debugSpecCommand = 
    ActionCommand ((fun _ -> 
      resetAndRegisterSpec ()
      x.debugSpec
      x.IsSelected <- true), 
      (fun _ -> true))
 
  member x.Spec with get () = specInfo

  member x.runSpec completed = 
      use wkr = new BackgroundWorker()
     
      wkr.DoWork.Add (fun args -> args.Result <- runSpecification ())
      wkr.RunWorkerCompleted.Add(fun args -> 
        let runResult = args.Result :?> AssertionResult * exn
        x.reportRunResult runResult
        completed ())
      
      wkr.RunWorkerAsync() 


  /// Re-evaluates the Context after launching the debugger in order to hit all possible breakpoints relevant to the specification
  member x.debugSpec = 
    Debugger.Launch() |> ignore
    buildContextAndResolveSpecs () |> ignore
    x.runSpec (fun () -> ())
 
  member x.RunSpecCommand with get () = x._runSpecCommand :> ICommand
  member x.DebugSpecCommand with get () = x._debugSpecCommand :> ICommand
  
  member x.IsDummySpec = x.AsITreeViewModel.Name = SpecViewModel.DummySpecName
  member x.InconclusiveException with get () = _inconclusiveException and set (v) = _inconclusiveException <- v
  
  static member DummySpecName = "___DummySpecToShowTreeExpander___GUID:0D46D658-A328-466C-873F-B4BA1E394E5D"
  static member Dummy = SpecViewModel ((SpecViewModel.DummySpecName, SpecDelegate(fun () -> AssertionResult.Inconclusive)), Unchecked.defaultof<IGuiController> , (fun () -> (None, null, null)), (fun _ -> null))
  static member Inconclusive controller msg excep = 
    let specVm = SpecViewModel((msg, SpecDelegate(fun () -> AssertionResult.Inconclusive)), controller , (fun () -> (None, excep, msg)), (fun _ -> msg))
    specVm.InconclusiveException <- excep
    specVm