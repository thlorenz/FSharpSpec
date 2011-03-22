module GuiControllerSpecs

open System
open FSharpSpec
open FSharpSpec.GuiRunner
open FSharp.Interop

type GuiControllerSpecs () =
  let _treeViewModelStub = fake<ITreeViewModel>
  let _guiViewModelMock = fake<IGuiRunnerViewModel>
  let _sut = GuiController( GuiRunnerViewModel = _guiViewModelMock ) :> IGuiController

  member x.treeViewModelStub with get () = _treeViewModelStub
  member x.guiRunnerViewModelMock with get () = _guiViewModelMock
  member x.sut with get () = _sut

type ``when a tree viewmodel has specs run results`` () =
  inherit GuiControllerSpecs ()
  let firstResult = SpecRunResultViewModel(Passed, "some spec name")
  let secondResult = SpecRunResultViewModel(Failed, "some other spec name")

  let specsRunResult = [ firstResult; secondResult ] |> List.toSeq
  do base.treeViewModelStub.SpecsRunResult |> returns specsRunResult

  member x.``and the user selects it`` =
    x.sut.Selected x.treeViewModelStub
    [
      verify "updates the spec run results of the gui viewmodel"
        <| lazy (x.guiRunnerViewModelMock |> received).UpdateSpecsRunResult specsRunResult
      
    ]