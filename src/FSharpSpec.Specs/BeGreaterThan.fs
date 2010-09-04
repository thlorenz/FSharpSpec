namespace FSharpSpec.Specs

open System
open FSharpSpec

type ``BeGreaterThan`` () =
    static member ``Structs`` = [
        it "1 should.beGreaterThan 0" 1 should.beGreaterThan 2
        it "0 should.beGreaterThan -1" 0 should.beGreaterThan 1
        
        ass "1 should.beGreaterThan 1 will fail" (it "" 1 should.beGreaterThan 1) will.fail
        ass "1 should.beGreaterThan 2 will fail" (it "" 1 should.beGreaterThan 2) will.fail
      ]  