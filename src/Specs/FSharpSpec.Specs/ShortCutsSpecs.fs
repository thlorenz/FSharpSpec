module ShortCutsSpecs 

open System
open FSharpSpec

type EventRaiser () =
  let event = new Event<_>()
  [<CLIEvent>]
  member x.RaisedEvent = event.Publish
  member x.Raise () = event.Trigger 1

type ``when i watch an event`` () =
  
  let eventRaiser = EventRaiser()
  let eventWasRaised = watchEvent eventRaiser.RaisedEvent 

  member x.initially =
    it "it returns an eventWasRaised reference that is false" !eventWasRaised should.be false

  member x.``and i raise it`` =
    eventRaiser.Raise()
    it "sets the eventWasRaised reference to true" !eventWasRaised should.be true
    

