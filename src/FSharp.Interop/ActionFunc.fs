namespace FSharp.Interop

open System

[<AutoOpen>]
module ActionFunc =

  /// converts a lambda from unit to unit to a System.Action
  let action f = Action(f)
  
  /// converts a lambda from a 'a to unit to a System.Action<'a>
  let action1 f = Action<_>(f)
  /// converts a lambda a System.Action
  let action2 f = Action<_,_>(f)
  /// converts a lambda a System.Action
  let action3 f = Action<_,_,_>(f)
  /// converts a lambda a System.Action
  let action4 f = Action<_,_,_,_>(f)
   /// converts a lambda a System.Action
  let action5 f = Action<_,_,_,_,_>(f)
  /// converts a lambda a System.Action
  let action6 f = Action<_,_,_,_,_,_>(f)
  /// converts a lambda a System.Action
  let action7 f = Action<_,_,_,_,_,_,_>(f)
  
  /// converts a lambda a System.Action
  let actionExn f = Action<exn>(f)
  
  /// converts a lambda from unit to 'a to System.Func<'a>
  let func f = Func<_>(f)
  /// converts a lambda from 'a to 'b to System.Func<'a,'b>
  let func1 f = Func<_,_>(f)
  /// converts a lambda with given params to and output 'x to a System.Func that returns 'x
  let func2 f = Func<_,_,_>(f)
  /// converts a lambda with given params to and output 'x to a System.Func that returns 'x
  let func3 f = Func<_,_,_,_>(f)
  /// converts a lambda with given params to and output 'x to a System.Func that returns 'x
  let func4 f = Func<_,_,_,_,_>(f)
  /// converts a lambda with given params to and output 'x to a System.Func that returns 'x
  let func5 f = Func<_,_,_,_,_,_>(f)
  /// converts a lambda with given params to and output 'x to a System.Func that returns 'x
  let func6 f = Func<_,_,_,_,_,_,_>(f)
  /// converts a lambda with given params to and output 'x to a System.Func that returns 'x
  let func7 f = Func<_,_,_,_,_,_,_,_>(f)

