﻿module TreeViewModelSpecs

open System
open FSharpSpec
open FSharpSpec.GuiRunner
open FSharp.Interop

type TreeViewModelSpecs () = 
  let _controllerMock = fake<IGuiController>
  let _name = "some Name"
  let _sut = TreeViewModel(_name, _controllerMock)
 
  member x.name with get () = _name 
  member x.controllerMock with get () = _controllerMock
  member x.sut with get () = _sut

  member x.``when the user selects it`` = 
    x.sut.IsSelected <- true
    lazy (x.controllerMock |> received).Selected x.sut |> verify "tells the controller that it was selected" 
  
  member x.``when the user de-selects it`` =
    x.sut.IsSelected <- false
    lazy (x.controllerMock |> didNotReceive).Selected x.sut |> verify "doesn't tell the controller that it was selected" 
     




