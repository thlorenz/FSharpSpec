namespace FSharpSpec.RunnerUtils 

open System
open System.IO
open System.Reflection

[<AutoOpen>]
module AppDomainUtils =
  let mutable specsAsmFolder = null

  let assemblyResolve = ResolveEventHandler(fun _ args -> 
     
    let referencedAssemblyName = args.Name.Split([| ',' |]).[0]  
     
    printf "*** Trying to resolve %s\n" referencedAssemblyName
      
    let fullPath = 
      let dllFile = sprintf @"%s\%s.dll" specsAsmFolder referencedAssemblyName
      let exeFile = sprintf @"%s\%s.exe" specsAsmFolder referencedAssemblyName
      match (dllFile, exeFile) with
      | (dll, exe) when File.Exists dll -> dll
      | (dll, exe) when File.Exists exe -> exe
      | (dll, exe)                      -> let msg = sprintf "Cannot find \n\t%s or \n\t%s\n\tand therefore cannot resolve this required assembly!" dll exe
                                           printf "Fatal: %s\n" msg 
                                           failwith msg
                                            
    printf "Loading %s\n\n" fullPath

    Assembly.LoadFrom(fullPath)
  )

  let hookAssemblyResolve (specsAsm : Assembly) = 
    AppDomain.CurrentDomain.remove_AssemblyResolve(assemblyResolve)
    
    specsAsmFolder <- (FileInfo(specsAsm.Location)).DirectoryName
    
    AppDomain.CurrentDomain.add_AssemblyResolve(assemblyResolve)
    
