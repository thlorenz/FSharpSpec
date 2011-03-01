module EqualitySpecs

  open System
  
  open FSharpSpec

  type Equality () =
       member x.Boolean = [
          it "true should.be true" true should.be true
          it "false should.be false" false should.be false
          it "false shouldn't.be true" false shouldn't.be true
          it "true shouldn't.be false" true shouldn't.be false
        
          ass "\"false should.be true\" will fail" (it "" false should.be true)  will.fail
          ass "\"true should.be false\" will fail" (it "" true should.be false) will.fail
         
          ass "\"true shouldn't.be true\" will fail" (it "" true shouldn't.be true) will.fail
          ass "\"false shouldn't.be false\" will fail " (it "" false shouldn't.be false) will.fail
       ]  
    
       member x.equalNull = 
           let initializedObject = new Object()
           let (nullObject : Object) = null
           [   
              it "null should.equal null" (null : obj) should.equal null
              it "nullObject should.equal null" nullObject should.equal null
              it "initializedObject shouldn't.equal null" initializedObject shouldn't.be null
              it "null shouldn't.equal initializedObject" null shouldn't.equal initializedObject
            
              ass "initializedObject should.equal null will fail" (it "" initializedObject should.equal null) will.fail
              ass "nullObject shouldn't.be null will fail" (it "" nullObject shouldn't.be null) will.fail
           ] 
     
       member x.Structs = [
          it "1 should.equal 1" 1 should.equal 1
          it "1 shouldn't.equal 2" 1 shouldn't.equal 2
        
          ass "\"1 should.equal 2\" will fail" (it "" 1 should.equal 2) will.fail
          ass "\"1 shouldn't.equal 1\" will fail" (it "" 1 shouldn't.equal 1) will.fail
       ]
     
       member x.Strings = [
          it "\"hello\" should.equal \"hello\"" "hello" should.equal "hello"        
          it "\"hello\" shouldn't.equal \"world\"" "hello" shouldn't.equal "world"
        
          ass "\"\"hello\" should.equal \"world\"\" will fail" (it "" "hello" should.equal "world") will.fail
          ass "\"hello\" shouldn't.equal \"hello\" will fail" (it "" "hello" shouldn't.equal "hello") will.fail        
       ] 
        
       member x.``Objects, when o1 = o3 and o1 <> o2`` =
        let o1 = new TestType(1)
        let o2 = new TestType(2)
        let o3 = new TestType(1)
        [
          it "o1 shouldn't.equal o2" o1 shouldn't.equal o2
          it "o1 should.equal o3" o1 should.equal o3
          it "o2 shouldn't.equal o3" o2 shouldn't.equal o3
          it "o3 should.equal o1" o3 should.equal o1
        
          ass "\"o1 should.equal o2\" will fail" (it "" o1 should.equal o2) will.fail
          ass "\"o1 shouldn't.equal o3\" will fail" (it "" o1 shouldn't.equal o3) will.fail
       ]