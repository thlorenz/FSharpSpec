namespace FSharpSpec.Specs

open FSharpSpec

type ``given a number that is zero`` () =
    let number = 0
    member x.``when i add one to it`` =  
        let result = number + 1
        [ 
            it "results in one" result should.equal 1 
            it "does not change the number" number should.equal 0
        ]
 
    member x.``when i add two to it`` =  
        let result = number + 2
        it "results in two" result should.equal 2
