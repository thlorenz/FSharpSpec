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
    verify "updates the spec run results of the gui viewmodel"
      <| lazy (x.guiRunnerViewModelMock |> received).UpdateSpecsRunResult specsRunResult
  
type ``when spec run progresses`` () =
  inherit GuiControllerSpecs ()

  member x.``with resetting results`` =
    x.sut.ResetResults ()
    verify "resets the specs on the gui runner viewmodel"
      <| lazy (x.guiRunnerViewModelMock |> received).ResetSpecs ()
  
  member x.``with registering 3 specs`` () =
    [ 
      ("1", SpecDelegate (fun () -> AssertionResult.Passed)) 
      ("2", SpecDelegate (fun () -> AssertionResult.Failed)) 
      ("3", SpecDelegate (fun () -> AssertionResult.Pending)) 
    ]
    |> List.toSeq
    |> x.sut.RegisterSpecs 

    verify "tells the gui runner viewmodel that 3 specs were registered" 
      <| lazy (x.guiRunnerViewModelMock |> received).RegisterSpecs 3

  member x.``with passed spec`` =
    x.sut.ReportResult AssertionResult.Passed
    verify "tells gui runner viewmodel that a spec passed"
      <| lazy (x.guiRunnerViewModelMock |> received).PassedSpec ()

  member x.``with pending spec`` =
    x.sut.ReportResult AssertionResult.Pending
    verify "tells gui runner viewmodel that a spec was pending"
      <| lazy (x.guiRunnerViewModelMock |> received).PendingSpec ()

  member x.``with inconclusive spec`` =
    x.sut.ReportResult AssertionResult.Inconclusive
    verify "tells gui runner viewmodel that a spec was inconclusive"
      <| lazy (x.guiRunnerViewModelMock |> received).InconclusiveSpec ()

  member x.``with failed spec`` =
    x.sut.ReportResult AssertionResult.Failed
    verify "tells gui runner viewmodel that a spec failed"
      <| lazy (x.guiRunnerViewModelMock |> received).FailedSpec ()