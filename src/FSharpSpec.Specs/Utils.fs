namespace FSharpSpec.Specs

open System
open FSharpSpec

[<AutoOpen>]
module Utils =
    
    exception DidNotFailException
   
    type will () = 
        static member fail(spec :(string * SpecDelegate)) =
            try
               (snd (spec)).Invoke() |> ignore
               
               DidNotFailException |> raise
            with
              | :? DidNotFailException -> 
                        String.Format("Expected to fail but didn't")
                        |> ExceptionNotRaisedException
                        |> raise
              | _    -> Passed   
    
    let ass (specName:string) actual assertion = 
        let specDelegate = new SpecDelegate(fun () -> assertion (actual))
        (specName, specDelegate)
        
        
    type TestType =
        val mutable _value : int
        val mutable _name : string
        
        new (value : int) =  { 
            _value = value 
            _name="DefaultName" 
        }
        
        member x.Value with get()         = x._value
        
        member x.Name with get()          = x._name
                      and  set value      = x._name <- value  
                      
        member public x.IncrementValue () =
                
                x._value <- x._value + 1
                x._value
            
        override x.Equals(o : obj)  =
            match o with
            | :? TestType as other  -> (other._value = x._value)
            | _                     -> false
            
        override x.GetHashCode() = x._value    