namespace FSharp.Interop

open NSubstitute

[<AutoOpen>]
module NSubstitute =

  let any<'a> = Arg.Any<'a>()
  let is<'a> (value : 'a) = Arg.Is<'a>(value)

  let clearReceivedCalls substitute = SubstituteExtensions.ClearReceivedCalls(substitute)

  let received substitute = SubstituteExtensions.Received(substitute)
  let didNotReceive substitute = SubstituteExtensions.DidNotReceive(substitute)

  let returns (arg : 'a) call = SubstituteExtensions.Returns(call, arg)
  
  let whenReceived f substitute = SubstituteExtensions.When(substitute, action1(f))