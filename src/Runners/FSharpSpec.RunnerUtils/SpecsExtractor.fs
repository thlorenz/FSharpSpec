(* 
    #I @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug";;
    #r "FSharpSpec.dll";;
*)

namespace FSharpSpec.RunnerUtils 

open System
open System.Reflection
open System.Collections.Generic 
open FSharpSpec

module SpecsExtractor =
    
    let getAssembly fullPath = 
        let loadAssembly fullPath = Reflection.Assembly.LoadFile fullPath
        loadAssembly fullPath  
    
    let getAllPublicTypes (asm : Assembly) = asm.GetExportedTypes()

    let filterOutInvalidTypes (types : Type[]) = types |> Array.filter(fun ty -> not ty.IsAbstract)
        
    let getSpecLists (ty : Type) = 
        let isReturningSpecs (mi : MethodInfo) = 
            match mi.ReturnType with
            | ty when ty  = typeof<(string * SpecDelegate)>             -> true    
            | ty when ty  = typeof<(string * SpecDelegate) list>        -> true
            | ty when ty  = typeof<Lazy<string * SpecDelegate> list>    -> true
            | _                                                         -> false 
        
        let isDeclaredDirectlyOnThisType(mi : MethodInfo) = mi.DeclaringType = ty
       
        ty.GetMethods() 
        |> Array.filter isReturningSpecs
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

    let getAllContexts asm  = 
        asm
        |> getAllPublicTypes
        |> filterOutInvalidTypes
        |> Array.map toPotentialContextWithParents
        |> Array.filter isContext
 
    let getContextTreeOfContexts contexts =
        let emptySpecLists :  MethodInfo[] = getSpecLists typeof<obj>
        let rootNode = new Node( {Clazz = typeof<obj>; SpecLists = emptySpecLists; ParentContexts = [] }, -1)

        for context in contexts do
            let mutable lastNode : Node =  rootNode
        
            for parentType in context.ParentContexts do
                let parentContext =  { Clazz = parentType; SpecLists = emptySpecLists; ParentContexts = []  } 
                let newNode = lastNode.getContextNode(parentContext)
                lastNode <- newNode
      
            lastNode.getContextNode(context) |> ignore
         
        rootNode    

    let getContextTreeOfAssembly asm =
      asm 
      |> getAllContexts
      |> getContextTreeOfContexts

    let getContextTree specsDllPath =
      specsDllPath 
      |> getAssembly
      |> getContextTreeOfAssembly

    let getContextTreeOfTypesInAssembly memberInfo asm =
    
      let getContextsInType (memberInfo : MemberInfo) contexts = 
        
        let  getTypeEqualsRunTimeTypeOrDeclaringType (ty : Type) (memberInfo : MemberInfo) =
          ty.FullName.Equals(memberInfo.ToString()) || 
          (ty.DeclaringType <> null && ty.DeclaringType.Name.Equals(memberInfo.ToString()))
        
        contexts 
        |> Array.filter (fun (c : Context) -> 
            let contextClass = c.Clazz
          
            match contextClass with
            | null  -> false
            | x     -> getTypeEqualsRunTimeTypeOrDeclaringType x memberInfo
            )
         
      asm 
      |> getAllContexts
      |> getContextsInType memberInfo
      |> getContextTreeOfContexts

    
  
    