module TreeViewModelSpecs

open NSubstitute
open System
open System.Linq.Expressions
open FSharpSpec
open FSharpSpec.GuiRunner

type TreeViewModelSpecs () = 
  
  member x.name = "some Name"
  member x.guiControllerMock = Substitute.For<IGuiController>()
  member x.sut = TreeViewModel(x.name, x.guiControllerMock)

  member x.``when the user selects it`` =
    
    SubstituteExtensions.Received(x.guiControllerMock.Selected(TreeViewModel(x.name, x.guiControllerMock) ) ) |> ignore
    it "did not fail" true should.be true
 
type ``when the node has specs run results`` () =
  inherit TreeViewModelSpecs ()
  do base.sut.AsITreeViewModel.SpecsRunResult <- [ SpecRunResultViewModel(Passed, "some spec name") ] 


