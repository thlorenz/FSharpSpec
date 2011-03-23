module ShortCutsSpecs 

open System
open FSharpSpec

type EventRaiser () =
  let event1 = new Event<_>()
  let event2 = new Event<_>()

  member x.StructEvent = event1.Publish
  member x.RaiseStructEvent args = event1.Trigger args

  member x.ClassEvent = event2.Publish
  member x.RaiseClassEvent args = event2.Trigger args


type ``when i watch an event`` () =
  
  let eventRaiser = EventRaiser()
  let eventWasRaised = watchEvent eventRaiser.StructEvent 

  member x.initially =
    it "it returns an eventWasRaised reference that is false" !eventWasRaised should.be false

  member x.``and i raise it`` =
    eventRaiser.RaiseStructEvent 0
    it "sets the eventWasRaised reference to true" !eventWasRaised should.be true
    
type ``when i watch an event and want to know about the event args that are struct`` () =
  
  let args = 1
  let eventRaiser = EventRaiser()
  let eventWasRaised, eventArgs = watchEvent2 eventRaiser.StructEvent 
  
  member x.``and i raise it with 1`` =
    eventRaiser.RaiseStructEvent args
    [
      it "sets the eventWasRaised reference to true" !eventWasRaised should.be true
      it "sets the eventArgs to 1" !eventArgs should.equal args
    ]  

type ``when i watch an event and want to know about the event args that are reference type`` () =
  
  let args = "hello"
  let eventRaiser = EventRaiser()
  let eventWasRaised, eventArgs = watchEvent1 eventRaiser.ClassEvent
  
  member x.``and i raise it with 'hello'`` =
    eventRaiser.RaiseClassEvent args
    [
      it "sets the eventWasRaised reference to true" !eventWasRaised should.be true
      it "sets the eventArgs to 'hello'" !eventArgs should.equal args
    ]  