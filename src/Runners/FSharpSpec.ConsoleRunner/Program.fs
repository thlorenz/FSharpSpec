namespace FSharpSpec.ConsoleRunner

open System
open System.IO
open System.Diagnostics
open System.Reflection

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils

module main = 
   
  let helpFlag = ("--help", "-h")
  let silentFlag = ("--silent", "-s")
  let debugFlag = ("--debug", "-d")
    
  let helpText = "
Usage: FsharpSpec.ConsoleRunner [options] [Path1 Path2 ...]

  Path: The full or relative path to the assembly containing the specifications to run

  Options:
      --help:      prints this help
      --silent:    supresses printing out of the detailed specification tree

"
  let getFlags (args : string list) = 
    let validFlags = [helpFlag; silentFlag; debugFlag]
    let flags = args |> List.filter(fun arg -> arg.StartsWith("--"))
    let shortFlags = args |> List.filter(fun arg -> arg.StartsWith("-") && arg.Length = 2)
    let allFlags = List.concat [flags; shortFlags]

    let invalidFlags = 
      allFlags
      |> List.filter(fun flag -> not (validFlags |> List.exists(fun f -> fst f = flag || snd f = flag)))
            
    match invalidFlags with
    | []    -> allFlags
    | x     -> failwithf "\n\nFound invalid option(s), use --help for more information. \nInvalid option(s): %A\n\n" x
        
  let getPaths (args : string list) =
    let paths = 
      args 
      |> List.filter(fun arg -> not (arg.StartsWith("-")))
      |> List.map (fun path -> 
          match path with
          | p when File.Exists(p) && Path.IsPathRooted(p) -> p 
          | p                                             -> Path.Combine(Environment.CurrentDirectory, p)
          )
   
    let invalidPaths = paths |> List.filter(fun path -> File.Exists(path) |> not)
          
    match invalidPaths with
    | []    -> paths
    | x     -> failwithf "\n\nUnable to find the following path(s) \n%A\n\n" x

  let extraxtAllContexts args = 
    let getContexts path =
      let asm = getAssembly path 
      hookAssemblyResolve asm
    
      let ctx = asm |> getAllContexts
        
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
      @"C:\dev\FSharp\FSharpSpec\src\Specs\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"
      @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.FSharpSampleSpecs\bin\Debug\FSharpSpec.FSharpSampleSpecs.dll"
      @"C:\dev\FSharp\FSharpSpec\src\Samples\Katas\FSharpSpec.Katas.StringCalculator\bin\Debug\FSharpSpec.Katas.dll"
      @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.GuiRunner.Specs\bin\Debug\FSharpSpec.GuiRunner.Specs.dll"
      @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug\FSharpSpec.dll"
    ]
    *)
    

  [<EntryPoint>]
  let main(args:string[]) =
    let printer = writeToConsole
        
    let containFlag flag flags =
      flags |> List.exists(fun f -> f = fst flag || f = snd flag)

    try
      let args = args |> Array.toList |> List.map(fun arg -> arg.Trim())
      
      (*
      Environment.CurrentDirectory <- @"C:\dev\FSharp\FSharpSpec\src\Specs\FSharpSpec.Specs\bin\Debug"
      let args = ["FSharpSpec.Specs.dll"]
      *)

      (*
      let args = [
       
       @"C:\dev\FSharp\FSharpSpec\src\Specs\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"
       @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.FSharpSampleSpecs\bin\Debug\FSharpSpec.FSharpSampleSpecs.dll"
       @"C:\dev\FSharp\FSharpSpec\src\Samples\Katas\FSharpSpec.Katas.StringCalculator\bin\Debug\FSharpSpec.Katas.dll"
       @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.GuiRunner.Specs\bin\Debug\FSharpSpec.GuiRunner.Specs.dll"
       @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug\FSharpSpec.dll"
      ]
      *)
         
      let flags = args |> getFlags
      let needHelp = flags |> containFlag helpFlag || args |> List.isEmpty
      let isSilent = flags |> containFlag silentFlag
      let isDebug = flags |> containFlag debugFlag

      if needHelp then 
        helpText |> printer
        0
      else
        let allContexts, allWarnings = args |> extraxtAllContexts
        
        let tree = allContexts |> getContextTreeOfContexts
                
        if (isDebug) then
          printfn "Attaching debugger - Just click yes in the dialog."
          Debugger.Launch() |> ignore
          Debugger.Log(0, "FSharpSpec", "Successfully attached Debugger.\n")
                
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
        | []    ->  // Console.ReadLine() |> ignore
                    0
        | x     ->  sprintf "\nWarnings: \n%A\n\n" x |> printer
                    // Console.ReadLine() |> ignore
                    1
      with
      | ex    -> ex.Message |> printer
                 "Use -help for more information\n" |> printer 
                 // Console.ReadLine() |> ignore
                 1

      