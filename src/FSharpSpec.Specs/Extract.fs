namespace FSharpSpec.Specs

open System
open FSharpSpec

type ctx0 () =
     let _sut = new TestType(0)
     
     member x.sut = _sut
          
type ctx1 () = 
    inherit ctx0 ()    
    
    do 
      base.sut.Name <- "ContextName"
        
    member x.``when I increment by 1`` = 
        
        x.sut.IncrementValue() |> ignore
        
        [
            it "should have name ContextName" x.sut.Name should.equal "ContextName"
            it "should have Value 1" x.sut.Value should.equal 1
        ]

    member x.``when I increment twice`` = 
        x.sut.IncrementValue() |>ignore
        x.sut.IncrementValue() |>ignore
        [
            it "should have name ContextName" x.sut.Name should.equal "ContextName"
            it "should have Value 2" x.sut.Value should.equal 2
        ]
 
 type ctx2 () = 
    inherit ctx1 ()
    
    do 
      base.sut.IncrementValue() |> ignore
        
    member x.initially = 
        [
            it "should have name ContextName" x.sut.Name should.equal "ContextName"
            it "should have Value 1" x.sut.Value should.equal 1
        ]

    member x.``when I increment by 1`` = 
        x.sut.IncrementValue() |>ignore
        [
            it "should have name ContextName" x.sut.Name should.equal "ContextName"
            it "should have Value 2" x.sut.Value should.equal 2
        ]
 
 type ctx3 () = 
    inherit ctx2 ()
    
    do 
      base.sut.IncrementValue() |> ignore
        
    member x.initially = 
        [
            it "should have name ContextName" x.sut.Name should.equal "ContextName"
            it "should have Value 2" x.sut.Value should.equal 2
        ]

    member x.``when I increment by 1`` = 
        x.sut.IncrementValue() |>ignore
        [
            it "should have name ContextName" x.sut.Name should.equal "ContextName"
            it "should have Value 3" x.sut.Value should.equal 3
        ]
