namespace FSharpSpec.Rx

open System
open System.Collections.Generic
open System.Reactive
open System.Reactive.Testing

type rx() =

    static member OnNext ticks (value :'a) = 
        Recorded<Notification<'a>>(
            ticks, 
            Notification<'a>.OnNext(value)
        )
    
    static member OnNextSpan(time : TimeSpan) (value :'a) = 
        Recorded<Notification<'a>>(
            time.Ticks, 
            Notification<'a>.OnNext(value)
        )

    static member Value(time : TimeSpan) (value :'a) = 
        Recorded<Notification<'a>>(
            time.Ticks, 
            Notification<'a>.OnNext(value)
        )

    static member OnCompleted ticks  = 
        Recorded<Notification<'a>>(
            ticks, 
            Notification<'a>.OnCompleted()
        )

    static member OnError ticks  excep = 
        Recorded<Notification<'a>>(
            ticks, 
            Notification<'a>.OnError(excep)
        )

    static member Subscribe start  = Subscription(start)

    static member Subscribe start finish = Subscription(start, finish)
