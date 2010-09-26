namespace FSharpSpec.RunnerUtils
open System
open System.Text
open System.Reflection

open FSharpSpec

[<AutoOpen>]
module ContextTree = 
   type FailureInfo = { FullSpecName : string; Exception : Exception; }
    
   let instantiate (ty : Type) =  Activator.CreateInstance(ty)
  
   let removeLeadingGet = function
        | (pn : string) when pn.StartsWith("get_")  -> pn.Substring(4)
        | pn                                        -> pn

   let getFullSpecName context specMethodName specName = 
     let toInheritanceChain = 
         List.fold (fun acc (clazz : Type) -> acc + clazz.Name + ", ")  ("")  
    
     (new StringBuilder())
      .AppendLine(context.Clazz.Namespace)
      .AppendLine((context.ParentContexts |> toInheritanceChain) + context.Clazz.Name + ": " + specMethodName + "  » " + specName)
      .ToString()
   
   let runContainedSpecs context (specMethod : MethodInfo) isFirstMethod instantiatedContext (indent : string) = 
        
        let specs = specMethod.Invoke(instantiatedContext, null) :?> list<(string * SpecDelegate)>
        
        let rec getSpecResults (specs : (string * SpecDelegate) list) =
            let runSpec specName (specDelegate : SpecDelegate) =
                let fullSpecName = getFullSpecName context (removeLeadingGet specMethod.Name) specName
                try
                    let outcome = specDelegate.Method.Invoke(specDelegate.Target, null) :?> AssertionResult
                    match outcome with
                    | Passed        ->    (fullSpecName, indent  + "      »  "  + specName + "\n",
                                                    None, Passed)

                    | Pending       ->    (fullSpecName, indent  + "      »  "  + specName + " - <<< Pending >>>" + "\n",
                                                    None, Pending)

                    | Failed        ->    (fullSpecName, "Should have thrown exception",
                                                    None, Failed)  
                    | Inconclusive  ->    (fullSpecName, indent  + "      »  "  + specName + " - <<< Inconclusive >>>" + "\n",
                                                    None, Inconclusive) 
                with
                    ex              ->    (fullSpecName, indent  + "      »  "  + specName + " - <<< Failed >>>" + "\n",
                                                    Some({ FullSpecName = fullSpecName; Exception = ex }), Failed) 

            match specs with
            | []                    -> []
            | (specName, specDelegate) :: xs    -> [(runSpec specName specDelegate)] @ getSpecResults xs
        
        let specResults =         
            specs
            |> getSpecResults
        
        let rec getFailures = function
            | []                                                        -> []
            | (s, m, f : FailureInfo option, o) :: xs  when f.IsSome    -> [f.Value] @ getFailures xs
            | _ :: xs                                                   -> getFailures xs
        
        let passes   = specResults |> List.filter(fun (s,m,f,o) -> o = Passed) |> List.map(fun (s,m,f,o) -> s + "\n")
        let failures = specResults |> List.filter(fun (s,m,f,o) -> o = Failed) |> getFailures
        let pending  = specResults |> List.filter(fun (s,m,f,o) -> o = Pending)|> List.map(fun (s,m,f,o) -> s + "\n")
        let messages = specResults |> List.map(fun (s,m,f,o) -> m)
        
        let combinedMessage = (messages |> List.reduce (fun acc s -> acc + s)) + "\n"
        
        let clazzName = indent + "+ " + context.Clazz.Name + "\n" + indent + "|" + "\n"
        let specMethodName = indent  + "   - " + (specMethod.Name |> removeLeadingGet) + "\n"

        match isFirstMethod with
        | true  -> (clazzName + specMethodName + combinedMessage, passes, failures, pending)       
        | false -> (specMethodName + combinedMessage, passes, failures, pending)        

   
   let unableToSetupContextResult context ex (indent :string) =
      let sb = new StringBuilder()
      let mutable failures : FailureInfo list = []

      sb.AppendLine(indent + "+ " + context.Clazz.Name)
        .AppendLine(indent + "Unable to setup context !!!") |> ignore
      
      (sb.ToString(), [], [{ FullSpecName = (getFullSpecName context "" "Exception while setting up context"); Exception = ex } ], [sprintf "%s inconclusive" context.Clazz.Name]  )  

   let getContextInfoForEmptyContext context (indent :string) = 
      (indent + "+ " + context.Clazz.Name + "\n", [], [], [])
 
   let typeWasNotAddedBefore (ty :Type) (typesAddedBefore : Type list) = 
     typesAddedBefore
     |> List.exists (fun t -> t = ty)
     |> not
        
  type Node (context : Context, ply : int) =
     let _context = context
     let  _ply = ply
     
     let mutable _children : Node list = []
     let mutable _contexts : Context list = []

     member x.Context with get() = _context
     member x.Ply with get() = _ply
     member x.Children with get() = _children
        
     member x.getContextNode(context : Context) = x.addToChildrenIfNotFoundAndReturnReference context
     
     member x.RunSpecs() =
        let sb = new StringBuilder()
        let mutable results = []
        let mutable addedType = []
        
        if context.SpecLists.Length = 0 then
            let addEmptyContextToResults = function
              | ctx when ctx.Clazz = typeof<obj>  -> []
              | ctx                               -> [getContextInfoForEmptyContext ctx x.Indent]
            
            results <- (addEmptyContextToResults context)

        else
          try
            let specMethods = x.Context.SpecLists
            for specMethod in specMethods do 
                let instantiatedContext = context.Clazz |> instantiate
                let isFirstMethod = Array.IndexOf(specMethods, specMethod) = 0
                let result = runContainedSpecs context specMethod isFirstMethod instantiatedContext x.Indent
                results <- results @ [result]
          with
            | ex -> results <- results @ [unableToSetupContextResult context ex x.Indent]
        
        for childNode in x.Children do
            results <- results @ childNode.RunSpecs()
       
        results

     member private x.hasNodeWith(contextToFind : Context) : Option<Node> =
        let rec foundIn (nodes : Node list) = 
            match nodes with
            | x::xs when (x.Context = contextToFind) -> Some(x)
            | x::xs                                                 -> foundIn xs
            | []                                                    -> None
        
        foundIn _children
        
     member private x.addNode(child : Node) = 
        _children <- _children @ [child]
        child
     
     member private x.addToChildrenIfNotFoundAndReturnReference(context : Context) =
        let existingNode = x.hasNodeWith context
        match existingNode with
        | None         -> x.addNode(new Node(context, _ply + 1))
        | _            -> existingNode.Value
     
     member private x.Indent =
          let rec getIndentFor = function
                | 0   -> ""
                | ply -> "|      " + getIndentFor (ply - 1)
          getIndentFor _ply   

     override x.ToString() = 
        let sb = new StringBuilder()
        sb.AppendLine(x.Indent + "+ " + x.Context.Clazz.Name) |> ignore
        
        for specMethod in x.Context.SpecLists do 
            sb.AppendLine(x.Indent + "|") |> ignore
            sb.AppendLine(x.Indent + "|   » " + specMethod.Name) |> ignore
        
        for childNode in x.Children do
            sb.AppendLine(x.Indent + "|") |> ignore
            sb.Append(childNode.ToString()) |> ignore
        
        sb.ToString()



