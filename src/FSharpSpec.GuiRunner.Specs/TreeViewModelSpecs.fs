module TreeViewModelSpecs

open System
open System.Linq.Expressions
open FSharpSpec
open FSharpSpec.GuiRunner
open FSharp.Interop

type TreeViewModelSpecs () = 
  
  member x.name = "some Name"
  member x.guiControllerMock = fake<IGuiController>
  member x.sut = TreeViewModel(x.name, x.guiControllerMock)

  member x.``when the user selects it`` = 
    verify "tells the controller" <| lazy (x.guiControllerMock |> received).Selected(x.sut)

type ``when the node has specs run results`` () =
  inherit TreeViewModelSpecs ()
  do base.sut.AsITreeViewModel.SpecsRunResult <- [ SpecRunResultViewModel(Passed, "some spec name") ] 


