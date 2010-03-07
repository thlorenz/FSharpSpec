﻿namespace FSharpSpec

open System

type shouldn't() =
    static member equal (actual, expected) = 
        match (actual, expected) with
        | a, e when a <> e  -> Passed
        | a, e              -> String.Format("Expected  [{0}] not equal [{1}], but it was!", e, a)
                               |> SpecFailedException 
                               |> raise
       
    static member be (actual:bool, expected:bool) = shouldn't.equal (actual, expected)
    
    static member be<'a when 'a : not struct and 'a : equality> (actual : 'a , expected : 'a) = shouldn't.equal (actual, expected)
    
    static member beSameAs<'a when 'a : not struct>(actual : 'a, expected : 'a) =
        match (actual, expected) with
        | a, e when Object.ReferenceEquals(a, e)  -> String.Format("Expected [{1}] to not be the same as [{0}], but it was.", e, a) 
                                                     |> SpecFailedException 
                                                     |> raise
        | _, _              -> Passed
        
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