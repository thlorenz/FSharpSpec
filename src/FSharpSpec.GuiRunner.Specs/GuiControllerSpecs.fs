module GuiControllerSpecs

open System
open FSharpSpec
open FSharpSpec.GuiRunner
open FSharp.Interop

type GuiControllerSpecs () =
  let _treeViewModelStub = fake<ITreeViewModel>
  let _sut = GuiController() :> IGuiController

  member x.treeViewModelStub with get () = _treeViewModelStub
  member x.sut with get () = _sut

type ``when a tree viewmodel has specs run results`` () =
  inherit GuiControllerSpecs ()
  
  let specsRunResult = [ SpecRunResultViewModel(Passed, "some spec name") ]
  do base.treeViewModelStub.SpecsRunResult <- specsRunResult

//  member x.``the user selects it`` =
//    x.sut.Selected x.treeViewModelStub
//    it "the spec run results contain only the results of the selected tree view model"
//      x.sut.SpecRunResults should.con
