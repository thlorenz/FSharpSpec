
module BeEmptySpecs 

open FSharpSpec

type BeEmpty () =
  
  member x.strings = [
    it "'' should.be Empty" "" should.be Empty
    it "'a' shouldn't.be Empty" "a" shouldn't.be Empty
    it "'aa' shouldn't.be Empty" "aa" shouldn't.be Empty

    ass "'a' should.be Empty will fail" (it "" "a" should.be Empty) will.fail
    ass "null should.be Empty will fail" (it "" null should.be Empty) will.fail
  ]

  member x.lists = [
    it "[] should.be Empty" [] should.be Empty
    it "[1] shouldn't.be Empty" [1] shouldn't.be Empty
    it "[1;2] shouldn't.be Empty" [1;2] shouldn't.be Empty
    
    ass "[1] should.be Empty will fail" (it "" [1] should.be Empty) will.fail
  ]



