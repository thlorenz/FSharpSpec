namespace FSharpSpec.ConsoleRunner

open FSharpSpec.RunnerUtils

type ConsoleLogger () =
    let mutable passed = 0
    let mutable pending = 0
    let mutable failed = 0
    

    interface ISpecsResultsLogger with
        member x.SpecPassed specName =
            passed <- passed + 1
      
        member x.SpecPending specName =
            pending <- pending + 1
       
        member x.SpecFailed specName message stackTrace =
            failed <- failed + 1
    
    member x.Report = 
        let totalSpecs = passed + pending + failed
        sprintf "\n-----------------------\nSpecifications: %d\nPassed: %d, Pending: %d, Failed: %d\n\n" 
            totalSpecs passed pending failed