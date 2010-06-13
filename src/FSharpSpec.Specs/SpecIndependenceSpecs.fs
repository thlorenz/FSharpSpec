namespace FSharpSpec.Specs

open System
open FSharpSpec

type ``Spec Independence`` () =
    let constructedO1 = new TestType(0)
    
    member x.``Value is originally 0 and both specs increment value during execution`` =
        let o1 = new TestType(0)                                                                                              
        [
          it "Incrementing o1 with 1 returns 1" (o1.IncrementValue()) should.equal 1  
          it "Incrementing o1 with 1 again returns 2" (o1.IncrementValue()) should.equal 2  
        ]
    
    member x.``Object is returned from a method, its value is originally 0 and both specs increment value during execution`` =
        let sut() = new TestType(0)
        [
          it "Incrementing sut() with 1 returns 1" (sut().IncrementValue()) should.equal 1  
          it "Incrementing sut() with 1 again returns 1"  (sut().IncrementValue()) should.equal 1  
        ]
        
    member x.``Object is initialized in constructor and used for this concern`` = [
          it "Incrementing constructedO1 with 1 returns 1" (constructedO1.IncrementValue()) should.equal 1  
          it "Incrementing constructedO1 with 1 again returns 2" (constructedO1.IncrementValue()) should.equal 2  
        ]
            
    member x.``Object is initialized in constructor and used again for another concern`` = [
          it "Incrementing constructedO1 with 1 returns 1" (constructedO1.IncrementValue()) should.equal 1  
          it "Incrementing constructedO1 with 1 again returns 2" (constructedO1.IncrementValue()) should.equal 2  
        ]
        