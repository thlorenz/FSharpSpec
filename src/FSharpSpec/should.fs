namespace FSharpSpec

open System

type should() = 
    
    static member equal(actual : obj, expected : obj) = 
        match (actual, expected) with
        | a, e when a = e   -> Passed
        | null, e           -> String.Format("Expected [{0}], but was [null]!", e) 
                               |> SpecFailedException 
                               |> raise   
        | a, null           -> String.Format("Expected [null], but was [{0}]!", a) 
                               |> SpecFailedException 
                               |> raise                          
        | a, e              -> String.Format("Expected [{0}], but was [{1}]!", e, a) 
                               |> SpecFailedException 
                               |> raise
            
    static member equal<'a when 'a : equality>(actual : 'a, expected : 'a) = 
        match (actual, expected) with
        | a, e when a = e   -> Passed
        | a, e              -> String.Format("Expected [{0}], but was [{1}]!", e, a) 
                               |> SpecFailedException 
                               |> raise
         
    static member be (actual:bool, expected:bool) = should.equal (actual, expected)
    
    static member be(actual : 'a, expectedType : Type) = should.equal (actual.GetType(), expectedType)
    
    static member beSameAs<'a when 'a : not struct>(actual : 'a, expected : 'a) =
        match (actual, expected) with
        | a, e when Object.ReferenceEquals(a, e)    -> Passed
        | a, e                                      -> String.Format("Expected [{1}] to be the same as [{0}], but it wasn't.", e, a) 
                                                       |> SpecFailedException 
                                                       |> raise
    
    static member beGreaterThan<'a when 'a : comparison>(actual : 'a, smaller : 'a) =
        match (actual, smaller) with
        | a, s when a > s -> Passed
        | a, s            -> String.Format("Expected [{1}] to be greater than [{0}], but it wasn't.", s, a) 
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
                String.Format("Expected exception of type {0}, but instead exception of type {1} was raised.", 
                               expectedType, ex.GetType())
                |> ExceptionNotRaisedException 
                |> raise
          | _                                       -> Passed
   
    static member failWithMessage(codeBlock : (unit -> unit), expectedMessage : string) =
        try 
          (new ThrowDelegate(codeBlock)).Invoke()
         
          String.Format("Expected exception with message {0}, but was never raised!", expectedMessage)
          |> ExceptionNotRaisedException 
          |> raise
        with 
          | ex  when (ex.Message <> expectedMessage) -> 
                String.Format("Expected exception with message [{0}], but instead exception with message [{1}] was raised.", 
                               expectedMessage, ex.Message)
                |> ExceptionNotRaisedException 
                |> raise
          | _                                       -> Passed
          
    static member failWithMessageContaining(codeBlock : (unit -> unit), containedMessage : string) =
        try 
          (new ThrowDelegate(codeBlock)).Invoke()
         
          String.Format("Expected exception with message {0}, but was never raised!", containedMessage)
          |> ExceptionNotRaisedException 
          |> raise
        with 
          | ex  when (ex.Message.Contains containedMessage) -> Passed
          | ex                                              -> 
                String.Format("Expected exception with message containing [{0}], but instead exception with message [{1}] was raised.", 
                               containedMessage, ex.Message)
                |> ExceptionNotRaisedException 
                |> raise
                
    static member contain (container : string, contained : string) =
        match container, contained with
        |  cr, cd when cr.Contains(cd)  -> Passed
        |  cr, cd                       -> String.Format("[{0}] was expected to contain [{1}] but didn't.", cr, cd)
                                           |> SpecFailedException
                                           |> raise
          
    static member contain<'a when 'a: equality>(items : 'a seq, item : 'a) =
        match items, item with
        | xs, x when xs |> Seq.exists (fun i -> i = x) -> Passed
        | xs, x                                        -> String.Format("[{0}] was expected to contain [{1}] but didn't.", items, item)
                                                           |> SpecFailedException
                                                           |> raise
  