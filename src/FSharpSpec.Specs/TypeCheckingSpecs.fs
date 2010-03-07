namespace FSharpSpec.Specs

open System
open FSharpSpec

type TypeChecking () =
    member x.Structs = [
        it "1 should.be typeof<int>" 1 should.be typeof<int>
        
        ass "1 should.be typeof<double> will.fail" (it "" 1 should.be typeof<double>) will.fail
    ]
        
        