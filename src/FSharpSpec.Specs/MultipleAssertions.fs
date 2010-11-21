namespace FSharpSpec.Specs

open FSharpSpec

type MultipleAssertions () =

    member x.Specs = 
        [
          lazy(it "spec1" (0 + 1) should.equal 1)
          lazy(it "spec2" (1 /0) should.equal 1)
          lazy(it "spec3" (2 - 1) should.equal 1)
        ]

