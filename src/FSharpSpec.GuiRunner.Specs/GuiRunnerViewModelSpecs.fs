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

  member x.initially =
    [// TODO: need should.beEmpty and shouldn't.beEmpty
    it "has no specs run results" x.sut.SpecsRunResults should.be Empty
  ]