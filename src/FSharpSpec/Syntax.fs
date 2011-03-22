namespace FSharpSpec

open System

[<AutoOpen>]
module Syntax = 
  
  /// Use for not yet implemented specifications.  
  type soon() =
    /// Use for not yet implemented specifications.  
    static member it (specName : string) =
      let specDelegate = new SpecDelegate(fun () -> Pending)
      (specName, specDelegate)
  
  /// Use for straight forward specifications
  let it (specName : string) actual assertion expected = 
    let specDelegate = new SpecDelegate(fun () -> assertion (actual, expected))
    (specName, specDelegate)
  
  /// Use for specifications whose evaluation of the actual value may throw an error.
  /// It is lazily evaluated and thus the name of the specification can be extracted first
  /// in order to give a proper error message. 
  /// Sample: it "0 / 1 equals 1" ( lazy (0/1) ) should.equal 1
  let it1 (specName : string) (actual : Lazy<'a>) assertion (expected : 'a) = 
    let specDelegate = new SpecDelegate(fun () -> assertion (actual.Value, expected))
    (specName, specDelegate)

  /// Use in order to verify that a codeblock executes without errors.
  /// The number one use case is to test mock expectations. 
  /// Sample: verify "was told to scream hello" <| lazy (m |> received).Scream("Hello") 
  let verify (specName : string) (verifyCall : Lazy<unit>) = 
    let specDelegate = SpecDelegate(fun () -> verifyCall.Value ; AssertionResult.Passed)
    (specName, specDelegate)

   /// Use to catch an exception in order to then assert about its type, message etc.      
  let catch<'a>(throwingCode:(unit -> 'a)) =
    try 
      throwingCode () |> ignore
      null
    with
      | excep  ->  excep     

[<AutoOpen>]
module Shortcuts =

  let watchEvent (event : IObservable<_>) =
    let eventWasRaised = ref false
    event.Add ( fun _ -> eventWasRaised := true)
    eventWasRaised