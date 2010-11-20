namespace FSharpSpec.Katas  

open FSharpSpec

type ``StringCalculator Specs``() = 
    let sut = new StringCalculator()

    member x.``adding empty string`` = 
        [ it "always returns 0" (sut.Add "") should.equal 0 ]
    
    member x.``single numbers`` =
        [ 
            it "adding '0' returns 0" (sut.Add "0") should.equal 0
            it "adding '1' returns 1" (sut.Add "1") should.equal 1
        ] 


