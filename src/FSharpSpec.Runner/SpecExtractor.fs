(* 
    #I @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug";;
    #r "FSharpSpec.dll";;

*)
namespace FSharpSpec.Runner 

open System
open System.Reflection
open System.Collections.Generic 

open FSharpSpec
open FSharpSpec.Runner

module SpecsExtractor =
    let getAssembly fullPath = 
        let loadAssembly fullPath = Reflection.Assembly.LoadFile fullPath
        loadAssembly fullPath  
    
    let getAllPublicTypes (asm : Assembly) = asm.GetExportedTypes()

    let getSpecLists (ty : Type) = 
        let isSpecList (mi : MethodInfo) = 
            match mi.ReturnType with
            |ty when ty  = typeof<list<(string * SpecDelegate)>> -> true
            | _                                                  -> false 
        
        let isDeclaredDirectlyOnThisType(mi : MethodInfo) = mi.DeclaringType = ty
       
        ty.GetMethods() 
        |> Array.filter isSpecList
        |> Array.filter isDeclaredDirectlyOnThisType

    let getParentContexts (ty : Type) = 
        let rec getInheritanceChain (ty : Type) =
            match ty with
            | t when t = typeof<Object>      -> []
            | t                              -> getInheritanceChain t.BaseType @ [t]
        ty.BaseType
        |> getInheritanceChain 

    let toPotentialContextWithParents (ty : Type) =
        { Clazz = ty; SpecLists = ty |> getSpecLists; ParentContexts = ty |> getParentContexts }

    let isContext (ctx : Context) = ctx.SpecLists.Length > 0

    let getAllContexts specsDllPath  = 
        specsDllPath 
        |> getAssembly
        |> getAllPublicTypes
        |> Array.map toPotentialContextWithParents
        |> Array.filter isContext
 
    let getContextTree specsPath =
        let emptySpecLists :  MethodInfo[] = getSpecLists typeof<obj>
        let rootNode = new Node( {Clazz = typeof<obj>; SpecLists = emptySpecLists; ParentContexts = [] }, 0)

        for context in  specsPath |> getAllContexts do
            let mutable lastNode : Node =  rootNode
        
            for parentType in context.ParentContexts do
                let parentContext =  { Clazz = parentType; SpecLists = emptySpecLists; ParentContexts = []  } 
                let newNode = lastNode.getContextNode(parentContext)
                lastNode <- newNode
      
            lastNode.getContextNode(context) |> ignore
         
        rootNode    

    