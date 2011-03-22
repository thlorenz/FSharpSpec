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