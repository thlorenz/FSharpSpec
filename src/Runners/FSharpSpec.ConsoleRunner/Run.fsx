#I @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug"
#r "FSharpSpec.dll"
#I @"C:\dev\FSharp\FSharpSpec\src\Runners\FSharpSpec.RunnerUtils\bin\Debug"
#r "FSharpSpec.RunnerUtils.dll"

open System.IO
open System.Text

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils


let validFlags = ["-silent"; "-help"]

let args = 
        ["-silent"; "-help";
         @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll";
         @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.FSharpSampleSpecs\bin\Debug\FSharpSpec.FSharpSampleSpecs.dll";
         @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.Katas\bin\Debug\FSharpSpec.Katas.dll";

         @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.dll";
        ]
        |> List.map(fun arg -> arg.Trim())


let flags = args |> List.filter(fun arg -> arg.StartsWith("-"))
let invalidFlags = flags |> List.filter(fun flag -> not (validFlags |> List.exists(fun f -> f = flag)))

match invalidFlags with
| []    -> printfn "Options: %A" flags
| x     -> failwithf "Found invalid options, use -help for more information. Invalid options: %A" x

let paths = args |> List.filter(fun arg -> not (arg.StartsWith("-")))
let invalidPaths = paths |> List.filter(fun path -> File.Exists(path) |> not)

match invalidPaths with
| []    -> printfn "Loading assemblies ..."
| x     -> failwithf "Unable to find the following path(s) %A" x

let getContexts path = 
    let ctx = 
        getAssembly path
        |> getAllContexts
        
    match ctx.Length with
    | 0   -> 
        let warnings = [sprintf "No contexts/specifications found in %s" path]
        (Array.empty, warnings)
    | _   -> (ctx, [])

let mutable allWarnings = []
let mutable allContexts = Array.empty
paths 
|> List.iter(fun path -> 
    let result = getContexts path
    allContexts <- Array.append allContexts (fst result)
    allWarnings <- List.append  allWarnings (snd result))

printfn "Warnings: %A" allWarnings
allContexts

let tree =
    allContexts
    |> getContextTreeOfContexts





    

  