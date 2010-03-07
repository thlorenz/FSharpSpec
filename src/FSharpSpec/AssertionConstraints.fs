namespace FSharpSpec

open System

[<AutoOpen>]
module Assertions =
    type AssertionResult = | Passed | Failed | Inconclusive

    type SpecDelegate = delegate of unit -> AssertionResult 

    exception SpecFailedException of string
    exception ExceptionNotRaisedException of string
    exception DidNotFailException

    type ThrowDelegate = delegate of unit -> unit

    type should() = 
        
        static member equal<'a when 'a : equality>(actual : 'a, expected : 'a) = 
            match (actual, expected) with
            | a, e when a <> e  -> String.Format("Expected [{0}], but was [{1}]!", e, a) 
                                   |> SpecFailedException 
                                   |> raise
            | _, _              -> Passed
           
        static member be (actual:bool, expected:bool) = should.equal (actual, expected)
        
        static member be<'a when 'a : not struct and 'a : equality> (actual : 'a , expected : 'a) = should.equal (actual, expected)
            
        static member be(actual, expectedType : Type) = should.equal (actual.GetType(), expectedType)
       
        static member failWith(codeBlock : (unit -> unit), expectedType : Type) =
            try 
              (new ThrowDelegate(codeBlock)).Invoke()
             
              String.Format("Expected exception of type {0}, but was never raised!", expectedType)
              |> ExceptionNotRaisedException 
              |> raise
            with 
              | ex  when (ex.GetType() <> expectedType) -> 
                    String.Format("Expected exception of type {0}, but instead exception of type {1} was raised!", 
                                   expectedType, ex.GetType())
                    |> ExceptionNotRaisedException 
                    |> raise
              | _                                       -> Passed
       
        static member contain (container:string, contained:string) =
            match container, contained with
            |  cr, cd when cr.Contains(cd)  -> Passed
            |  cr, cd                       -> String.Format("[{0}] was expected to contain [{1}] but didn't.", cr, cd)
                                               |> SpecFailedException
                                               |> raise
              
        static member contain<'a when 'a: equality>(items : 'a seq, item : 'a) =
            match items, item with
            | xs, x when xs |> Seq.exists (fun i -> i = x) -> Passed
            | xs, x                                         -> String.Format("[{0}] was expected to contain [{1}] but didn't.", items, item)
                                                               |> SpecFailedException
                                                               |> raise
      
    type shouldn't() =
        static member equal (actual, expected) = 
            match (actual, expected) with
            | a, e when a <> e  -> Passed
            | a, e              -> String.Format("Expected  [{0}] not equal [{1}], but it was!", e, a)
                                   |> SpecFailedException 
                                   |> raise
           
        static member be (actual:bool, expected:bool) = shouldn't.equal (actual, expected)
        
        static member be<'a when 'a : not struct and 'a : equality> (actual : 'a , expected : 'a) = shouldn't.equal (actual, expected)
        
        static member contain (container:string, contained:string) =
            match container, contained with
            |  cr, cd when cr.Contains(cd)  ->  String.Format("[{0}] was expected to  not contain [{1}] but did.", container, contained)
                                                |> SpecFailedException
                                                |> raise
            | _ , _                         ->  Passed 
            
        static member contain<'a when 'a: equality>(items : 'a seq, item : 'a) =
            match items, item with
            | xs, x when xs |> Seq.exists (fun i -> i = x) -> String.Format("[{0}] was expected to not contain [{1}] but did.", items, item)
                                                               |> SpecFailedException
                                                               |> raise
            | _, _                                         -> Passed                                                                   