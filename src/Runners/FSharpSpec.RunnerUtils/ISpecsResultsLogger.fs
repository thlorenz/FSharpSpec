namespace FSharpSpec.RunnerUtils

type ISpecsResultsLogger = 
    abstract SpecPassed : string -> unit
    abstract SpecIgnored : string -> unit
    abstract SpecFailed : string -> string -> string -> unit