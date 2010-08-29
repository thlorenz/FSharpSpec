namespace FSharpSpec.Runner 
open System
open System.Text
open System.Reflection

open FSharpSpec

[<AutoOpen>]
module ContextTree = 
  type Node (context : Context, ply : int) =
     let _context = context
     let  _ply = ply
     
     let mutable _children : Node list = []
     let mutable _contexts : Context list = []

     let instantiate (ty : Type) =  Activator.CreateInstance(ty)
  
     let removeLeadingGet (propertyName : string) = 
        match propertyName with
        | pn when pn.StartsWith("get_")  -> pn.Substring(4)
        | pn                                        -> pn
     
     member x.Context with get() = _context
     member x.Ply with get() = _ply
     member x.Children with get() = _children
        
     member x.getContextNode(context : Context) = x.addToChildrenIfNotFoundAndReturnReference context
     
     member x.RunSpecs() : string =
        let sb = new StringBuilder()
        sb.AppendLine(x.Indent + "+ " + x.Context.Clazz.Name) |> ignore
           
        let runContainedSpecs (specMethod : MethodInfo, instantiatedContext : obj) = 
           let specs = specMethod.Invoke(instantiatedContext, null) :?> list<(string * SpecDelegate)>
           
           for (specName, specDelegate)  in specs do
               sb.Append(x.Indent + "|      »  "  + specName) |> ignore
               try
                 specDelegate.Method.Invoke(specDelegate.Target, null) |> ignore
                 sb.AppendLine() |> ignore
               with
                | ex -> sb.AppendLine(" - FAILED") |> ignore
    
        try
            let instantiatedContext = context.Clazz |> instantiate
            for specMethod in x.Context.SpecLists do 
                sb.AppendLine(x.Indent + "|") |> ignore
                sb.AppendLine(x.Indent + "| - " + (specMethod.Name |> removeLeadingGet) ) |> ignore
                runContainedSpecs (specMethod, instantiatedContext)
        with
            | ex -> sb.Append(x.Indent)
                          .AppendLine("    Unable to setup context !!!") |> ignore 
        
        for childNode in x.Children do
            sb.AppendLine(x.Indent + "|") |> ignore
            sb.Append(childNode.RunSpecs()) |> ignore
        
        sb.ToString()
        

     member private x.hasNodeWith(contextToFind : Context) : Option<Node> =
        let rec foundIn (nodes : Node list) = 
            match nodes with
            | x::xs when (x.Context = contextToFind) -> Some(x)
            | x::xs                                                 -> foundIn xs
            | []                                                     -> None
        
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
                | ply -> "|   " + getIndentFor (ply - 1)
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



