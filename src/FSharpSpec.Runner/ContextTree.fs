namespace FSharpSpec.Runner 
open System
open System.Text
open System.Reflection

open FSharpSpec

[<AutoOpen>]
module ContextTree = 
   type FailureInfo = { FullSpecName : string; Exception : Exception; }
    
   let instantiate (ty : Type) =  Activator.CreateInstance(ty)
  
   let removeLeadingGet (propertyName : string) = 
        match propertyName with
        | pn when pn.StartsWith("get_")  -> pn.Substring(4)
        | pn                             -> pn

   let getFullSpecName context specName = 
     let toInheritanceChain (parentClasses : Type list) = 
        parentClasses
        |> List.fold (fun acc clazz -> acc + clazz.Name + ", ")  ("")  
    
     (new StringBuilder())
      .AppendLine(context.Clazz.Namespace)
      .AppendLine((context.ParentContexts |> toInheritanceChain) + context.Clazz.Name + "  » " + specName)
      .ToString()
   
   let runContainedSpecs context (specMethod : MethodInfo) isFirstMethod instantiatedContext (indent : string) = 
        let results = new StringBuilder()
        let mutable failures : FailureInfo list = []
        let specs = specMethod.Invoke(instantiatedContext, null) :?> list<(string * SpecDelegate)>
        
        if isFirstMethod then 
           results.AppendLine(indent + "+ " + context.Clazz.Name).AppendLine(indent + "|") |> ignore
       
        results.AppendLine(indent  + "   - " + (specMethod.Name |> removeLeadingGet) ) |> ignore   
        
        for (specName, specDelegate)  in specs do
            results.Append(indent  + "      »  "  + specName) |> ignore 
            try
                let outcome = specDelegate.Method.Invoke(specDelegate.Target, null) :?> AssertionResult
                
                if outcome = Pending then 
                    results.AppendLine("  -  <<< Pending >>>")
                else
                    results.AppendLine() 
                |> ignore
            with
            | ex -> do 
                     failures <- failures @  
                        [{ FullSpecName = (getFullSpecName context specName); Exception = ex } ]  
                     results.AppendLine(" - <<< Failed >>>") |> ignore 
        
        (results.ToString() + "\n", failures)
   
   let unableToSetupContextResult context ex (indent :string) =
      let sb = new StringBuilder()
      let mutable failures : FailureInfo list = []

      sb.AppendLine(indent + "+ " + context.Clazz.Name)
        .AppendLine(indent + "Unable to setup context !!!") |> ignore
      
      (sb.ToString(), [{ FullSpecName = (getFullSpecName context "Exception while setting up context"); Exception = ex } ]  )  

   let getContextInfoForEmptyContext context (indent :string) = 
      (indent + "+ " + context.Clazz.Name + "\n", [])
 
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
     
     member x.RunSpecs() : (string * FailureInfo list) list =
        let sb = new StringBuilder()
        let mutable results : (string * FailureInfo list) list = []
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



