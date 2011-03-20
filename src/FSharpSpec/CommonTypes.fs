namespace FSharpSpec

open System
open System.Reflection

type AssertionResult = | Passed | Pending | Failed | Inconclusive

type SpecDelegate = delegate of unit -> AssertionResult 

exception SpecFailedException of string
exception DidNotFailException
