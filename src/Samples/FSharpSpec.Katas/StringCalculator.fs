namespace FSharpSpec.Katas  

type StringCalculator () =
    member x.Add numbers =
        match numbers with 
        | ""        -> failwith "Hello"
        | _         -> 0
