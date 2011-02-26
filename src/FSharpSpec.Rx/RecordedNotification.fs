namespace FSharpSpec.Rx

open System
open System.Collections.Generic
open System.Reactive
open System.Reactive.Testing

[<AutoOpen>]
module RecordedNotification = 

  let onNext ticks (value :'a) = 
      Recorded<Notification<'a>>(
          ticks, 
          Notification<'a>.OnNext(value)
      )
    
  let onNextSpan(time : TimeSpan) (value :'a) = 
      Recorded<Notification<'a>>(
          time.Ticks, 
          Notification<'a>.OnNext(value)
      )

  let onCompleted ticks  = 
      Recorded<Notification<'a>>(
          ticks, 
          Notification<'a>.OnCompleted()
      )

  let onError ticks  excep = 
      Recorded<Notification<'a>>(
          ticks, 
          Notification<'a>.OnError(excep)
      )

  let subscribeForever start  = Subscription(start)

  let subscribe start finish = Subscription(start, finish)
