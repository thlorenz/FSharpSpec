#I @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug"
#I @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Runner\bin\Debug"
#r "FSharpSpec.dll"
#r "FSharpSpec.Runner.exe"

open System
open System.Reflection
open System.Collections.Generic 

open FSharpSpec
open FSharpSpec.Runner
open FSharpSpec.Runner.ContextTree

type ContextWithParents  =  { Clazz : Type; SpecLists : MethodInfo[]; ParentContexts : Type list }

let specsPath = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"

let getAssembly fullPath = 
    let loadAssembly fullPath = Reflection.Assembly.LoadFile fullPath
    loadAssembly fullPath  
    
let getAllPublicTypes (asm : Assembly) = asm.GetExportedTypes()

let getSpecLists (ty : Type) = 
    let isSpecList (mi : MethodInfo) = 
        match mi.ReturnType with
        |ty when ty  = typeof<list<(string * SpecDelegate)>> -> true
        | _                                                                      -> false 
        
    let isDeclaredDirectlyOnThisType(mi : MethodInfo) = mi.DeclaringType = ty
       
    ty.GetMethods() 
    |> Array.filter isSpecList
    |> Array.filter isDeclaredDirectlyOnThisType

let getParentContexts (ty : Type) = 
    let rec getInheritanceChain (ty : Type) =
        match ty with
        | t when t = typeof<Object>      -> [t]
        | t                                          -> getInheritanceChain t.BaseType @ [t]
    ty.BaseType
    |> getInheritanceChain 

let toPotentialContextWithParents (ty : Type) =
    { Clazz = ty; SpecLists = ty |> getSpecLists; ParentContexts = ty |> getParentContexts }

let isContext (ctx : ContextWithParents) = ctx.SpecLists.Length > 0

let instantiate (ty : Type) =  Activator.CreateInstance(ty)

let getAllContextsWithParents specsDllPath  = 
    specsDllPath 
    |> getAssembly
    |> getAllPublicTypes
    |> Array.map toPotentialContextWithParents
    |> Array.filter isContext
 
let allContextsWithParents = specsPath |> getAllContextsWithParents

let getContextTree specsPath =
    let emptySpecLists :  MethodInfo[] = getSpecLists typeof<obj>
    let rootNode = new Node( {Clazz = typeof<obj>; SpecLists = emptySpecLists }, 0)

    for context in  specsPath |> getAllContextsWithParents do
        let mutable lastNode : Node =  rootNode
        
        for parentType in context.ParentContexts do
            let parentContext =  { Context.Clazz = parentType; Context.SpecLists = emptySpecLists } 
            let newNode = lastNode.getContextNode(parentContext)
            lastNode <- newNode
      
        lastNode.getContextNode({ Context.Clazz = context.Clazz; Context.SpecLists = context.SpecLists }) |> ignore
         
    rootNode    

let tree = specsPath |> getContextTree

let ctx2 = allContextsWithParents |> Array.find(fun ctx -> ctx.Clazz.FullName.Equals("FSharpSpec.Specs.ctx2"))

let ctx2SpecLists = ctx2.SpecLists

let c2 = instantiate ctx2.Clazz

let met = ctx2.SpecLists.[0]
let specs =  met.Invoke(c2, null) :?> list<(string * SpecDelegate)>

let s0 = snd specs.[0]
let m0 = s0.Method
let t0 = s0.Target
let r = m0.Invoke(t0, null)

