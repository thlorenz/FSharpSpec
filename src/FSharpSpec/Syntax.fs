namespace FSharpSpec

open System

[<AutoOpen>]
module Syntax = 
    
    type ThrowDelegate = delegate of unit -> unit

    type soon() =
        static member it (specName : string) =
            let specDelegate = new SpecDelegate(fun () -> Pending)
            (specName, specDelegate)

    let it (specName : string) actual assertion expected = 
        let specDelegate = new SpecDelegate(fun () -> assertion (actual, expected))
        (specName, specDelegate)
  
        
    let catch(throwingCode:(unit -> unit)) =
        try 
           (new ThrowDelegate(throwingCode)).Invoke()
           null
        with
           | excep  ->  excep     
