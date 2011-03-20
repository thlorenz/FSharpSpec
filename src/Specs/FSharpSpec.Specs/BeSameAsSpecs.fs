module BeSameAsSpecs

open System
open FSharpSpec

type ``BeSameAs - Strings`` () =
  member x.``when s1=s2 and s3 is another string`` = 
      let s1 = "string1"
      let s2 = s1
      let s3 = "string3"
      [
          it "s1 should.beSameAs s2" s1 should.beSameAs s2
          it "s1 shouldn't.beSameAs s3" s1 shouldn't.beSameAs s3
            
          ass "s1 should.beSameAs s3 will fail" (it "" s1 should.beSameAs s3) will.fail
          ass "s1 shouldn't.beSameAs s2 will fail" (it "" s1 shouldn't.beSameAs s2) will.fail
      ]

type ``BeSameAs - Objects`` () =        
  member x.``when obj1=obj2 and obj3 is another object`` =
      let obj1 = new Object()
      let obj2 = obj1
      let obj3 = new Object()
      [
          it "obj1 should.beSameAs obj2" obj1 should.beSameAs obj2
          it "obj1 shouldn't.beSameAs obj3" obj1 shouldn't.beSameAs obj3
            
          ass "obj1 should.beSameAs obj3 will.fail" (it "" obj1 should.beSameAs obj3) will.fail
          ass "obj1 shouldn't.beSameAs obj2 will. fail" (it "" obj1 shouldn't.beSameAs obj2) will.fail
      ]