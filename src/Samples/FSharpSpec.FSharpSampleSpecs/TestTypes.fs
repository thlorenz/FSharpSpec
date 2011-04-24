module TestTypes

type TestType =
  val mutable _value : int
  val mutable _name : string
        
  new (value : int) =  { 
      _value = value 
      _name="DefaultName" 
  }
        
  member x.Value with get()         = x._value
        
  member x.Name with get()          = x._name
                and  set value      = x._name <- value  
                      
  member public x.IncrementValue () =
          x._value <- x._value + 1
          x._value
            
  override x.Equals(o : obj)  =
      match o with
      | :? TestType as other  -> (other._value = x._value)
      | _                     -> false
            
  override x.GetHashCode() = x._value    