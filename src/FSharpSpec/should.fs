namespace FSharpSpec

open System

type should() = 
    
    static member equal<'a when 'a : equality>(actual : 'a, expected : 'a) = 
        match (actual, expected) with
        | a, e when a <> e  -> String.Format("Expected [{0}], but was [{1}]!", e, a) 
                               |> SpecFailedException 
                               |> raise
        | _, _              -> Passed
       
    static member be (actual:bool, expected:bool) = should.equal (actual, expected)
    
    static member be<'a when 'a : not struct and 'a : equality> (actual : 'a , expected : 'a) = should.equal (actual, expected)
        
    static member be(actual, expectedType : Type) = should.equal (actual.GetType(), expectedType)
    
    static member beSameAs<'a when 'a : not struct>(actual : 'a, expected : 'a) =
        match (actual, expected) with
        | a, e when Object.ReferenceEquals(a, e)    -> Passed
        | a, e                                      -> String.Format("Expected [{1}] to be the same as [{0}], but it wasn't.", e, a) 
                                                       |> SpecFailedException 
                                                       |> raise
   
    static member failWith(codeBlock : (unit -> unit), expectedType : Type) =
        try 
          (new ThrowDelegate(codeBlock)).Invoke()
         
          String.Format("Expected exception of type {0}, but was never raised!", expectedType)
          |> ExceptionNotRaisedException 
          |> raise
        with 
          | ex  when (ex.GetType() <> expectedType) -> 
                String.Format("Expected exception of type {0}, but instead exception of type {1} was raised!", 
                               expectedType, ex.GetType())
                |> ExceptionNotRaisedException 
                |> raise
          | _                                       -> Passed
   
    static member contain (container:string, contained:string) =
        match container, contained with
        |  cr, cd when cr.Contains(cd)  -> Passed
        |  cr, cd                       -> String.Format("[{0}] was expected to contain [{1}] but didn't.", cr, cd)
                                           |> SpecFailedException
                                           |> raise
          
    static member contain<'a when 'a: equality>(items : 'a seq, item : 'a) =
        match items, item with
        | xs, x when xs |> Seq.exists (fun i -> i = x) -> Passed
        | xs, x                                         -> String.Format("[{0}] was expected to contain [{1}] but didn't.", items, item)
                                                           |> SpecFailedException
                                                           |> raise
  