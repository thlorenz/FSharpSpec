module GuiRunnerViewModelSpecs

open System
open FSharpSpec
open FSharpSpec.GuiRunner
open FSharp.Interop

type GuiRunnerViewModelSpecs () =
 
  let _controllerMock = fake<IGuiController>
  let _rootViewModelStub = fake<ITreeViewModel>
  let _sut = GuiRunnerViewModel(_rootViewModelStub, _controllerMock)
 
  member x.controllerMock with get () = _controllerMock
  member x.rootViewModelStub with get () = _rootViewModelStub
  member x.sut with get () = _sut
  member x.suti with get () = _sut.AsIGuiRunnerViewModel

  member x.initially = [
    it "has no specs run results" x.sut.SpecsRunResults should.be Empty
  ]

type ``when there are two specs run results`` () =
  inherit GuiRunnerViewModelSpecs () 
  
  let firstResult = SpecRunResultViewModel(Failed, "some message")
  let secondResult = SpecRunResultViewModel(Failed, "some other message")
  let specsRunResults = [ firstResult; secondResult ]
  
  let raisedResultsChanged = watchEvent base.sut.SpecsRunResults.CollectionChanged

  member x.``and it is updated with them`` = 
    x.sut.AsIGuiRunnerViewModel.UpdateSpecsRunResult specsRunResults
    
    [
      it "contains only the two results it was updated with" 
        x.sut.SpecsRunResults should.containOnly specsRunResults
      it "lets me know that its results changed" !raisedResultsChanged should.be true
    ]

type ``when i register 8 specs`` () =
  inherit GuiRunnerViewModelSpecs ()
  
  let showsProgress registered passed pending inconclusive failed finished (sut : GuiRunnerViewModel) = 
    [
      it (sprintf "has %d registered spec(s)" registered)           sut.RegisteredSpecs should.equal registered
      it (sprintf "has %d passed spec(s)" passed)                   sut.PassedSpecs should.equal passed
      it (sprintf "has %d pending spec(s)" pending)                 sut.PendingSpecs should.equal pending
      it (sprintf "has %d inconclusive spec(s)" inconclusive)       sut.InconclusiveSpecs should.equal inconclusive
      it (sprintf "has %d failed spec(s)" failed)                   sut.FailedSpecs should.equal failed
      it (sprintf "has %d finished spec(s)" finished)               sut.FinishedSpecs should.equal finished
    ]

  do
    base.suti.RegisterSpecs 8

  member x.initially = x.sut |> showsProgress 8 0 0 0 0 0
  
  member x.``and i reset the specs`` = 
    x.suti.ResetSpecs ()
    x.sut |> showsProgress 0 0 0 0 0 0

  member x.``and 1 spec passes`` =
    x.suti.PassedSpec ()
    (it "overall state is passed" x.sut.OverallState should.equal Passed) :: 
    (x.sut |> showsProgress 8 1 0 0 0 1)
    
  member x.``and 1 spec was pending, 3 were inconclusive and 2 specs fail`` =
    x.suti.PendingSpec ()
    x.suti.InconclusiveSpec (); x.suti.InconclusiveSpec (); x.suti.InconclusiveSpec ()
    x.suti.FailedSpec (); x.suti.FailedSpec ()
    (it "overall state is failed" x.sut.OverallState should.equal Failed) :: 
    (x.sut |> showsProgress 8 0 1 3 2 6)
 
  member x.``and 1 spec passed and 1 was inconclusive`` =
    x.suti.PassedSpec ()
    x.suti.InconclusiveSpec ()
    (it "overall state is inconclusive" x.sut.OverallState should.equal Inconclusive) :: 
    (x.sut |> showsProgress 8 1 0 1 0 2)