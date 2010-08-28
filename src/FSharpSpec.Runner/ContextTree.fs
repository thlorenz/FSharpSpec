namespace FSharpSpec.Runner 
open System
open System.Text
open System.Reflection

open FSharpSpec

module ContextTree = 
  type Context  =  { Clazz : Type; SpecLists : MethodInfo[] }

  type Node (context : Context, ply : int) =
     let _context = context
     let  _ply = ply
     
     let mutable _children : Node list = []
     let mutable _contexts : Context list = []
     
     member x.Context with get() = _context
     member x.Ply with get() = _ply
     member x.Children with get() = _children
        
     member x.getContextNode(context : Context) = x.addToChildrenIfNotFoundAndReturnReference context
     
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
     
     
     override x.ToString() = 
        let indent =
            let rec getIndentFor = function
                | 0   -> ""
                | ply -> "|   " + getIndentFor (ply - 1)
            getIndentFor _ply    
            
        let sb = new StringBuilder()
        sb.AppendLine(indent + "+ " + x.Context.Clazz.Name) |> ignore
        
        for specMethod in x.Context.SpecLists do 
            sb.AppendLine(indent + "|") |> ignore
            sb.AppendLine(indent + "|   » " + specMethod.Name) |> ignore
        
        for childNode in x.Children do
            sb.AppendLine(indent + "|") |> ignore
            sb.Append(childNode.ToString()) |> ignore
        
        sb.ToString()

