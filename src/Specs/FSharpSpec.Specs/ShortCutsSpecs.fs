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
  
  let _eventRaiser = EventRaiser()
  let eventWasRaised = watchEvent _eventRaiser.StructEvent 
  
  member x.eventRaiser = _eventRaiser

  member x.initially =
    it "it returns an eventWasRaised reference that is false" !eventWasRaised should.be false

  member x.``and i raise it`` =
    x.eventRaiser.RaiseStructEvent 0
    it "sets the eventWasRaised reference to true" !eventWasRaised should.be true
    
type ``and want to know about the event args that are struct`` () =
  inherit ``when i watch an event`` ()
  
  let args = 1
  let eventWasRaised, eventArgs = watchEvent2 base.eventRaiser.StructEvent 
  
  member x.``and i raise it with 1`` =
    x.eventRaiser.RaiseStructEvent args
    [
      it "sets the eventWasRaised reference to true" !eventWasRaised should.be true
      it "sets the eventArgs to 1" !eventArgs should.equal args
    ]  

type ``and want to know about the event args that are reference type`` () =
  inherit ``when i watch an event`` ()

  let args = "hello"
  let eventWasRaised, eventArgs = watchEvent1 base.eventRaiser.ClassEvent
  
  member x.``and i raise it with 'hello'`` =
    x.eventRaiser.RaiseClassEvent args
    [
      it "sets the eventWasRaised reference to true" !eventWasRaised should.be true
      it "sets the eventArgs to 'hello'" !eventArgs should.equal args
    ]  