namespace FSharpSpec.ConsoleRunner

open System.IO

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils

module main = 
   
    let helpFlag = "-help"
    let silentFlag = "-silent"
    
    let helpText = "
Usage: FsharpSpec.ConsoleRunner [options] [FullPath1 FullPath2 ...]

    FullPath: The full path to the assembly containing the specifications to run

    Options:
        -help:      prints this help
        -silent:    supresses printing out of the detailed specification tree

"
   
    let getFlags (args : string list) = 
        let validFlags = ["-silent"; "-help"]
        let flags = args |> List.filter(fun arg -> arg.StartsWith("-"))

        let invalidFlags = flags |> List.filter(fun flag -> not (validFlags |> List.exists(fun f -> f = flag)))
            
        match invalidFlags with
        | []    -> flags
        | x     -> failwithf "\n\nFound invalid option(s), use -help for more information. \nInvalid option(s): %A\n\n" x
        
    let getPaths (args : string list) =
        let paths = args |> List.filter(fun arg -> not (arg.StartsWith("-")))
        let invalidPaths = paths |> List.filter(fun path -> File.Exists(path) |> not)
          
        match invalidPaths with
        | []    -> paths
        | x     -> failwithf "\n\nUnable to find the following path(s) \n%A\n\n" x

    let extraxtAllContexts args = 
        let getContexts path = 
            let ctx = 
                getAssembly path
                |> getAllContexts
        
            match ctx.Length with
            | 0   -> 
                let warnings = [sprintf "No contexts/specifications found in %s" path]
                (Array.empty, warnings)
            | _   -> (ctx, [])

        let mutable allWarnings = List.empty
        let mutable allContexts = Array.empty
        for path in args |> getPaths do
            let result = getContexts path
            allContexts <- Array.append allContexts (fst result)
            allWarnings <- List.append  allWarnings (snd result)
        (allContexts, allWarnings)
    
     
     (*
     [
        @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll";
        @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.FSharpSampleSpecs\bin\Debug\FSharpSpec.FSharpSampleSpecs.dll";
        @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.Katas\bin\Debug\FSharpSpec.Katas.dll";

        @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.dll";
     ]
     *)
    [<EntryPoint>]
    let main(args:string[]) =
        let printer = writeToConsole
        
        try
            let args = args |> Array.toList |> List.map(fun arg -> arg.Trim())
        
            let flags = args |> getFlags
            let needHelp = args |> List.isEmpty || flags |> List.exists(fun f -> f = helpFlag) 
            let isSilent = flags |> List.exists(fun f -> f = silentFlag)
           

            if needHelp then 
                helpText |> printer
                0
            else
                let allContexts, allWarnings = args |> extraxtAllContexts
        
                let tree = allContexts |> getContextTreeOfContexts
                let results = tree.RunSpecs() 
                       
                if (not isSilent) then printResultTree results printer
                printFailureDetails results printer
                printPendingSummary results printer
                printFailureSummary results printer

                let logger = new ConsoleLogger() :> ISpecsResultsLogger

                results
                |> List.iter(fun (msg, passes, failures, pendings) -> 
                    passes    |> List.iter(fun passedSpec -> logger.SpecPassed passedSpec)
                    failures  |> List.iter(fun failedSpec -> 
                                    logger.SpecFailed failedSpec.FullSpecName (
                                        getFailureMessage failedSpec) (getFailureStackTrace failedSpec))
                    pendings  |> List.iter(fun pendingSpec -> logger.SpecPending pendingSpec))  
            
                (logger :?> ConsoleLogger).Report |> printer

                match allWarnings with
                | []    ->  0
                | x     ->  sprintf "\nWarnings: \n%A\n\n" x |> printer
                            1
        with
        | ex    -> ex.Message |> printer
                   "Use -help for more information\n" |> printer 
                   1
