module MultipleAssertions

open FSharpSpec

// Demonstrates three ways to achieve specfication independence
// In both cases spec1 and spec3 will still be run even though resolving (1 / 0) inside spec2 throws an exception
// Delaying these 'risky' codeblocks is necessary, because otherwise they get resolved, whenever FSharpSpec
// tries to resolve the list of specs inside the property.
// If an error occurs at that point no specifications can be resolved from the
type MultipleAssertions () =
  
  /// By placing (1 / 0) inside a lambda, we delay its evaluation to a point, were FSharpSpec can deal with it
  /// without affecting the execution of the other specifications.
  member x.``Isolating specifications by placing 'risky' code inside a lambda`` = [
    it "spec1" (0 + 1) should.equal 1
          
    // This specification fails, but the other specs and the spec name can be evaluated before the failing code rusn
    it "spec2" (fun () -> 1/0) should.equal 1
         
    it "spec3" (2 - 1) should.equal 1
  ]

  /// This is a more explicit way to do the same as above:
  /// Note that this is using the it1 overload to make sure that the lazy value is properly evaluated.
  /// It's a matter of taste which of them (isolating via lambda or lazy you prefer)
  /// By placing (1 / 0) inside a lazy, we delay its evaluation to a point, were FSharpSpec can deal with it
  /// without affecting the execution of the other specifications.
  member x.``Isolating specifications by placing 'risky' code inside a lazy`` = [
    it "spec1" (0 + 1) should.equal 1
          
    // This specification fails, but the other specs and the spec name can be evaluated before the failing code runs
    it1 "spec2" ( lazy  (1/0) ) should.equal 1
         
    it "spec3" (2 - 1) should.equal 1
  ]
  
  /// To compare lets see what happens, if we don't isolate the specifications from each other
  /// When FSharpSpec tries to obtain the list of specifications of the below property, the
  /// (1 / 0) is evaluated right away and throws an exception.
  /// Thus FSharpSpec is unable to obtain any specifications at all from the list.
  member x.``If specifications were not isolated, none can be resolved if one of them throws during property access`` = [
    it "spec1" (0 + 1) should.equal 1
          
    /// This specification fails, and the other specs and spec name can NOT be evaluated before the failing code runs
    it "spec2" (1 /0) should.equal 1
          
    it "spec3" (2 - 1) should.equal 1
  ]
