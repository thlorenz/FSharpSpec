namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel
open System

open FSharpSpec
open FSharpSpec.RunnerUtils


open SpecsRunnerUtils

type SpecState = | NotRunYet | Passed | Pending | Failed | Inconclusive 

[<AutoOpen>]
module GuiReportingHelpers =

  let toSpecState assertionResult = 
      match assertionResult with
      | AssertionResult.Passed        -> Passed 
      | AssertionResult.Pending       -> Pending 
      | AssertionResult.Failed        -> Failed 
      | AssertionResult.Inconclusive  -> Inconclusive

  let toSpecStateDisplay state =
    match state with
    | NotRunYet     -> "Not Run Yet"
    | Passed        -> "Passed"
    | Pending       -> "Pending"
    | Failed        -> "Failed"
    | Inconclusive  -> "Inconclusive"

type SpecRunResultViewModel =
  inherit ViewModelBase

  val _state : SpecState
  val _fullSpecName : string
   val _exception : Exception

  new () = SpecRunResultViewModel (NotRunYet, null)

  new (state, fullSpecName) = SpecRunResultViewModel (state, fullSpecName, null)
  
  /// FullSpecName includes Contexts and Inheritance Chain
  new  (state, fullSpecName, exn : Exception) =
    { _state = state; _fullSpecName = fullSpecName; _exception = exn }

  member x.State with get () = x._state
  
  member x.StateDisplay with get () = toSpecStateDisplay x.State
       
  member x.FullSpecName with get () = x._fullSpecName
  member x.ExceptionMessage with get () = getExceptionMessage x._exception "\t"
  member x.FullException 
    with get () = 
      match x._exception with
      | null    -> ""
      | excep   -> excep.ToString()

  static member NotRunYet fullSpecName = SpecRunResultViewModel(NotRunYet, fullSpecName, null)

type SpecsRunResult () =
  inherit ViewModelBase ()
 
  let _items = ObservableCollection<SpecRunResultViewModel>()
  
  member x.Items with get () = _items

 

  