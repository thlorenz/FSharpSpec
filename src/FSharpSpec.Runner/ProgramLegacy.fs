namespace FSharpSpec.Runner

open System
open System.Collections.Generic
open SpecsExtractorLegacy 
open System.Linq
open System.Reflection

exception WrongSecond of int

module mainLegacy = 
     
    //[<EntryPoint>]
    let main_legacy(args:string[]) =
        
        let args = [ @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"; "-verbose" ].ToArray()
        
        if args.Length = 0 
        then 
            printfn "usage: FSharpSpec.Runner \"fullPathToTests1.dll\" \"fullPathToTests2.dll\" [-verbose]"
        else    
            try
                let dllPath = args.[0] 
                let verbose = Array.Exists(args, fun arg -> arg.Equals("-verbose"))
                let errorReports = new Dictionary<string, Exception>() 
                
                printf "Testing: %s" dllPath
                let allSpecs = allContextsSorted dllPath
                  
                for concern in allSpecs.Keys do
                    printfn "\n\n%s" concern
                    for _ in 1 .. concern.Length do printf "--"
                    printfn ""
                    for (context, specifications) in allSpecs.[concern] do
                        let cleanContext = 
                            match context with
                            | s when s.StartsWith("get_")  -> s.Substring(4)
                            | s                            -> s
                            
                        printf "\n  %s:" cleanContext
                        for(specName, specDelegate) in specifications do 
                            try  
                              specDelegate.Invoke() |> ignore
                              if verbose then printf "\n\t -> %s" specName 
                            with   
                              | ex -> printf "\n\t -> %s (Failed)" specName 
                                      errorReports.Add(String.Format("Concern: \"{0}\"\nContext: \"{1}\" \n  -> {2}", concern, cleanContext, specName), ex)
                        printfn ""
                        
                printfn "\n____________________________________________________________" 
                for entry in errorReports do
                    printfn "\n\n%s: \n%A\n\nFull Stacktrace: \n%s" entry.Key entry.Value (entry.Value.ToString())
                    printfn "==========================================================="
                
                match errorReports.Count with
                | 0         -> printfn "No Failures found -> GREEN"
                | _         -> printfn "\n\nFound the following errors (full stacktraces above):\n"
                               for entry in errorReports do
                                   printfn "%s: %A\n" entry.Key entry.Value
            with
            | ex -> printfn "usage: FSharpSpec.Runner \"fullPathToTests1.dll\" \"fullPathToTests2.dll\" [-verbose]\n Here the error: \n %A" ex                        
           
        let key = Console.ReadLine()
        0
        
