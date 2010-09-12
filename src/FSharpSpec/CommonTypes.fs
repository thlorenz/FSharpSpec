namespace FSharpSpec

open System
open System.Reflection

type AssertionResult = | Passed | Pending | Failed | Inconclusive

type SpecDelegate = delegate of unit -> AssertionResult 
type Context  =  { Clazz : Type; SpecLists : MethodInfo[]; ParentContexts : Type list  }

exception SpecFailedException of string
exception ExceptionNotRaisedException of string
exception DidNotFailException

type ThrowDelegate = delegate of unit -> unit