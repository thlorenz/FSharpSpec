namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel

open FSharpSpec

type SpecState = | NotRunYet | Passed | Pending | Failed | Inconclusive 
 
type SpecRunResultViewModel =
  inherit ViewModelBase

  val _state : SpecState
  val _fullSpecName : string
  val _exceptionMessage : string

  new () = SpecRunResultViewModel (NotRunYet, null)

  new (state, fullSpecName) = SpecRunResultViewModel (state, fullSpecName, null)
  
  /// FullSpecName includes Contexts and Inheritance Chain
  new  (state, fullSpecName, exceptionMessage) =
    { _state = state; _fullSpecName = fullSpecName; _exceptionMessage = exceptionMessage }

  member x.State with get () = x._state
    
  member x.StateDisplay 
    with get () =  
      match x.State with
      | NotRunYet     -> "Not Run Yet"
      | Passed        -> "Passed"
      | Pending       -> "Pending"
      | Failed        -> "Failed"
      | Inconclusive  -> "Inconclusive"
  
     
  member x.FullSpecName with get () = x._fullSpecName
  member x.ExceptionMessage with get () = x._exceptionMessage

  static member NotRunYet fullSpecName = SpecRunResultViewModel(NotRunYet, fullSpecName, null)

type SpecsRunResult () =
  inherit ViewModelBase ()
 
  let _items = ObservableCollection<SpecRunResultViewModel>()
  
  member x.Items with get () = _items