namespace FSharpSpec.Runner 

open System
open System.Reflection
open System.Collections.Generic 

open FSharpSpec

module SpecsExtractor =

    let loadAssembly fullPath = Reflection.Assembly.LoadFile fullPath

    let getAllPublicClasses (asm:Assembly) = asm.GetExportedTypes()

    let getContextMethods (c : Type) = 
            c.GetMethods() 
            |> Array.filter (fun m -> m.ReturnType = typeof<list<(string * SpecDelegate)>>)
            

    let getAllContextMethods (classes : Type[]) = 
            [ for c in classes -> getContextMethods c ] 
            |> List.reduce (fun met mets -> Array.append met mets)

    let getAllContextsInDll (dllPath:string) =
            loadAssembly dllPath
            |> getAllPublicClasses
            |> getAllContextMethods
            |> Array.map (fun specMethod -> (specMethod.ReflectedType.Name, 
                                             specMethod.Name, 
                                             specMethod.Invoke(Activator.CreateInstance(specMethod.ReflectedType), null) :?> list<(string * SpecDelegate)>))
            
    let allContextsSorted dllPath = 
        let unsortedContexts = getAllContextsInDll dllPath        
        let dic = new Dictionary<string, List<(string * ((string * SpecDelegate) list))>>()
        
        for (key : string, concern : string, specs : ((string * SpecDelegate) list)) in unsortedContexts do
            if not <| dic.ContainsKey(key) then dic.Add(key, new List<(string * ((string * SpecDelegate) list))>())
            dic.[key].Add(concern, specs)
        dic    
            