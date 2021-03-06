﻿namespace FSharpSpec

open System
open CommonUtils

type shouldn't() =
    static member equal (actual : obj, expected : obj) = 
        match (actual, expected) with
        | a, e when a <> e  -> Passed
        | null, null        -> String.Format("Expected  [null] not equal [null], but it was!")
                               |> SpecFailedException 
                               |> raise
        | a, e              -> String.Format("Expected  [{0}] not equal [{1}], but it was!", e, a)
                               |> SpecFailedException 
                               |> raise
     
    static member equal<'a when 'a : equality>(actual : 'a, expected : 'a) = 
        match (actual, expected) with
        | a, e when a <> e  -> Passed
        | a, e              -> String.Format("Expected  [{0}] not equal [{1}], but it was!", e, a) 
                               |> SpecFailedException 
                               |> raise                           
       
    static member be (actual:bool, expected:bool) = shouldn't.equal (actual, expected)
    
    static member be (actual: seq<'a>, characteristic:Characteristic) =
      let testForNotEmpty source = 
        match source with
        | null                          -> (Failed, 
                                            sprintf "value was expected to be not empty but was [null]!")
        | s when not (s |> Seq.isEmpty) -> (Passed, "")
        | otherwise                     -> (Failed, 
                                            sprintf "%A was expected to be not empty but wasn't!" source)
    
      let (result, msg) =
        match characteristic with
        | Empty   -> testForNotEmpty actual
    
      evaluate (result, msg)

    static member be(actual : obj, expectedType : Type) = shouldn't.equal (actual.GetType(), expectedType)

    static member beSameAs<'a when 'a : not struct>(actual : 'a, expected : 'a) =
        match (actual, expected) with
        | a, e when Object.ReferenceEquals(a, e)  -> String.Format("Expected [{1}] to not be the same as [{0}], but it was.", e, a) 
                                                     |> SpecFailedException 
                                                     |> raise
        | _, _              -> Passed
        
    static member contain (container : string, contained : string) =
        match container, contained with
        |  cr, cd when cr.Contains(cd)  ->  String.Format("[{0}] was expected to  not contain [{1}], but did.", container, contained)
                                            |> SpecFailedException
                                            |> raise
        | _ , _                         ->  Passed 
        
    static member contain<'a when 'a: equality>(items : 'a seq, item : 'a) =
        match items, item with
        | xs, x when xs |> Seq.exists (fun i -> i = x) -> String.Format("[{0}] was expected to not contain [{1}] but did.", items, item)
                                                           |> SpecFailedException
                                                           |> raise
        | _, _                                         -> Passed                                                                   


    // Risk-friendly overloads
    static member equal<'a>(riskyCode : (unit -> 'a), expected) = shouldn't.equal(riskyCode (), expected)
    static member be<'a>(riskyCode : (unit -> 'a), expected) = shouldn't.be(riskyCode (), expected)
    static member beSameAs<'a when 'a : not struct>(riskyCode : (unit -> 'a), expected) = shouldn't.beSameAs(riskyCode (), expected)
   