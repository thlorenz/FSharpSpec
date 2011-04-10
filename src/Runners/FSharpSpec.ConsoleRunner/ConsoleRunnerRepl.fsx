
open System
open System.Diagnostics
open System.Reflection

let asm = Assembly.LoadFrom @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.GuiRunner.Specs\bin\Debug\FSharpSpec.GuiRunner.Specs.dll"
asm.GetReferencedAssemblies()
