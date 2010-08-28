(* 
    #I @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug";;
    #r "FSharpSpec.dll";;

*)
namespace FSharpSpec.Runner 

open System
open System.Reflection
open System.Collections.Generic 

open FSharpSpec

module SpecsExtractor =
    let specsPath = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"

    let assemblyFor fullPath = 
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

    let instantiated (ty : Type) =  Activator.CreateInstance(ty)

    let asm  = assemblyFor specsPath
    let allPublicTypes = getAllPublicTypes asm
 
    let ctx0 = allPublicTypes |> Array.find(fun ty -> ty.FullName.Equals("FSharpSpec.Specs.ctx0"))
    let ctx1 = allPublicTypes |> Array.find(fun ty -> ty.FullName.Equals("FSharpSpec.Specs.ctx1"))
    let ctx2 = allPublicTypes |> Array.find(fun ty -> ty.FullName.Equals("FSharpSpec.Specs.ctx2"))

    let ctx0SpecLists = ctx0 |> getSpecLists
    let ctx1SpecLists = ctx1 |> getSpecLists
    let ctx2SpecLists = ctx2 |> getSpecLists

    let c0 = instantiated ctx0
    let c1 = instantiated ctx1
    let c2 = instantiated ctx2