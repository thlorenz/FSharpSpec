let validFlags = ["-silent"; "-help"]

let args = 
        ["-silent"; "-help";
         @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll   ";
         @"  C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.FSharpSampleSpecs\bin\Debug\FSharpSpec.FSharpSampleSpecs.dll";
         @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.Katas\bin\Debug\FSharpSpec.Katas.dll"]
        |> List.map(fun arg -> arg.Trim())


let flags = args |> List.filter(fun arg -> arg.StartsWith("-"))
let invalidFlags = flags |> List.filter(fun flag -> not (validFlags |> List.exists(fun f -> f = flag)))

match invalidFlags with
| []    -> printfn "Options: %A" flags
| x     -> failwithf "Found invalid options, use -help for more information. Invalid options: %A" x