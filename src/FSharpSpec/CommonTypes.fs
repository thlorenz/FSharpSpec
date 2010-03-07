namespace FSharpSpec

open System

type AssertionResult = | Passed | Failed | Inconclusive

type SpecDelegate = delegate of unit -> AssertionResult 

exception SpecFailedException of string
exception ExceptionNotRaisedException of string
exception DidNotFailException

type ThrowDelegate = delegate of unit -> unit