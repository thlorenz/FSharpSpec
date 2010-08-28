namespace FSharpSpec.Runner 

open System
open System.Reflection
open System.Collections.Generic 

open FSharpSpec

module SpecsExtractor =
    type TestClassInfo = { Namespace : string; Hierarchy : string list }
    
    let loadAssembly fullPath = Reflection.Assembly.LoadFile fullPath

    let getAllPublicClasses (asm:Assembly) = asm.GetExportedTypes()
    
    let getContextMethods (c : Type) = 
        let isSpecMethod (mi : MethodInfo) = 
            match mi.ReturnType with
            |ty when ty  = typeof<list<(string * SpecDelegate)>> -> true
            | _                                                  -> false 
        
        let isDeclaredDirectlyOnThisType(mi : MethodInfo) = mi.DeclaringType = c
       
        c.GetMethods() 
        |> Array.filter isSpecMethod
        |> Array.filter isDeclaredDirectlyOnThisType
                                      
    
    
    let getClassInfo (clazz : Type) = 
        let rec hierarchy (clazz : Type) =
            match clazz with
            | c when c = typeof<Object>      -> []
            | c                              -> hierarchy c.BaseType @ [c]
        
        let hierarchyNames (classes : Type list) =
            classes 
            |> List.map(function clazz -> clazz.Name)    
            
        { Namespace = clazz.Namespace;
          Hierarchy = clazz |> hierarchy |> hierarchyNames }
    
    let flattenedClassInfo (inf: TestClassInfo) =
        inf.Hierarchy 
        |> List.reduce(fun acc clazz -> acc + ", " + clazz)
        
         


    let getAllContextMethods (classes : Type[]) = 
        [ for c in classes -> 
            getContextMethods c ] 
        |> List.reduce (fun met mets -> Array.append met mets)

    let getAllContextsInDll (dllPath:string) =
        loadAssembly dllPath
        |> getAllPublicClasses
        |> getAllContextMethods
        |> Array.map (fun specMethod -> (getClassInfo specMethod.ReflectedType |> flattenedClassInfo, 
                                         specMethod.Name, 
                                         specMethod.Invoke(Activator.CreateInstance(specMethod.ReflectedType), null) :?> list<(string * SpecDelegate)>))
        
    let allContextsSorted dllPath = 
        let unsortedContexts = getAllContextsInDll dllPath        
        let dic = new Dictionary<string, List<(string * ((string * SpecDelegate) list))>>()
        
        for (key : string, concern : string, specs : ((string * SpecDelegate) list)) in unsortedContexts do
            if not <| dic.ContainsKey(key) then dic.Add(key, new List<(string * ((string * SpecDelegate) list))>())
            dic.[key].Add(concern, specs)
        dic    
            