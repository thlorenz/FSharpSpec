namespace FSharpSpec.RunnerUtils

type ISpecsResultsLogger = 
    abstract SpecPassed : string -> unit
    abstract SpecPending : string -> unit
    abstract SpecFailed : string -> string -> string -> unit