namespace FSharpSpec

open System

[<AutoOpen>]
module Syntax = 
    
    type soon() =
        static member it (specName : string) =
            let specDelegate = new SpecDelegate(fun () -> Pending)
            (specName, specDelegate)

    let it (specName : string) actual assertion expected = 
        let specDelegate = new SpecDelegate(fun () -> assertion (actual, expected))
        (specName, specDelegate)
  
        
    let catch<'a>(throwingCode:(unit -> 'a)) =
        try 
           (new RiskDelegate<'a>(throwingCode)).Invoke() |> ignore
           null
        with
           | excep  ->  excep     
