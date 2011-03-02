module MultipleAssertions

open FSharpSpec

// Demonstrates two ways to achieve specfication independence
// In both cases spec1 and spec3 will still be run even though resolving (1 / 0) inside spec2 throws an exception
// Delaying these 'risky' codeblocks is necessary, because otherwise they get resolved, whenever FSharpSpec
// tries to resolve the list of specs inside the property.
// If an error occurs at that point no specifications can be resolved from the
type MultipleAssertions () =
  (* 
      (1 / 0) is evaluated once spec2 is resolved, which allows FSharpSpec to handle the exception and still run the other specs.
      By using lazy, we delay the creation of the specification delegate and thus they can be resolved separately.
      Since spec2 cannot be resolved, FSharpSpec will be unable to resolve its name.
      Uncommenting the offender results in the following output:
      - Isolating specifications via 'lazy' keyword
            »  spec1
            »  Unresolvable - <<< Threw Exception >>>
            »  spec3
      FSharpSpec.Specs
          MultipleAssertions: Isolating specifications via 'lazy' keyword  » (Specification name could not be resolved)
		      Attempted to divide by zero.
    *)
  member x.``Isolating specifications via 'lazy' keyword`` = [
    lazy(it "spec1" (0 + 1) should.equal 1)
          
    // Uncomment the below specification to see it work
    // lazy(it "spec2" (1 /0) should.equal 1)
          
    lazy(it "spec3" (2 - 1) should.equal 1)
  ]
    
  (*
      By placing (1 / 0) inside a lambda, we delay its evaluation to a point, were FSharpSpec can deal with it
      without affecting the execution of the other specifications.
      Uncommenting the offender results in the following output:
        - Isolating specifications by placing 'risky' code inside a lambda
            »  spec1
            »  spec2 - <<< Failed >>>
            »  spec3
      FSharpSpec.Specs
          MultipleAssertions: Isolating specifications by placing 'risky' code inside a lambda  » spec2
		      Attempted to divide by zero.
  *)
  member x.``Isolating specifications by placing 'risky' code inside a lambda`` = [
    it "spec1" (0 + 1) should.equal 1
          
    // Uncomment the below specification to see it work
    // it "spec2" (fun () -> 1 /0) should.equal 1
         
    it "spec3" (2 - 1) should.equal 1
  ]

  (*
      To compare lets see what happens, if we don't isolate the specifications from each other
      When FSharpSpec tries to obtain the list of specifications of the below property, the
      (1 / 0) is evaluated right away and throws an exception.
      Thus FSharpSpec is unable to obtain any specifications at all from the list.
      Uncommenting the offender results in the following output:
      - If specifications were not isolated, none can be resolved if one of them throws during property access
          Unable to resolve specifications !!!
      FSharpSpec.Specs
          MultipleAssertions: If specifications were not isolated, none can be resolved if one of them throws during property access  » Exception while setting up specification
		      Attempted to divide by zero.
  *)
  member x.``If specifications were not isolated, none can be resolved if one of them throws during property access`` = [
    it "spec1" (0 + 1) should.equal 1
          
    // Uncomment the below specification to see that no specifications can be resolved
    // it "spec2" (1 /0) should.equal 1
          
    it "spec3" (2 - 1) should.equal 1
  ]
