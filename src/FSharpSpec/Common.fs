namespace FSharpSpec

open System
open System.Reflection

type AssertionResult = | Passed | Pending | Failed | Inconclusive

type SpecDelegate = delegate of unit -> AssertionResult 

type Characteristic = | Empty 

exception SpecFailedException of string
exception DidNotFailException

module CommonUtils =
  let notContainedButShould source = ( fun containedItem -> 
    source 
    |> Seq.exists(fun sourceItem -> sourceItem = containedItem) 
    |> not )  
  
  let containedButShouldn't containedItems = ( fun sourceItem -> 
    containedItems 
    |> Seq.exists(fun containedItem -> containedItem = sourceItem) 
    |> not ) 

  let evaluate (result, msg) =
    match result with
      | Passed        -> Passed
      | Failed        -> msg |> SpecFailedException |> raise
      | otherwise     -> ArgumentException(sprintf "Did not expect result %A here" result)
                         |> raise