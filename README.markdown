FSharpSpec
======================================================================
FSharpSpec is a BDD framework that leverages the succinctness of F# to create specifications without the noise.

## Why do I care? 
Well, lets wet your appetite with some samples which can be found in their entirety [here](https://github.com/thlorenz/FSharpSpec/tree/master/src/Samples):

Assuming we have a system under test:
<pre>let sut = new StringCalculator()</pre>
###Write a simple spec:
<pre>
member x.``adding empty string`` = 
        [ it "returns 0" (sut.Add "") should.equal 0 ]
</pre>
###Produce specs however you like
The fact that FSharpSpec verifies a list of specifications returned by an F# property, we can produce this list however we like which allows us to achieve super compact code and maximum code reuse among other things. 

We can use this helper function:
<pre>
 let testAdding (nums:string) expected = 
        let specName = (sprintf "adding '%s' returns %d" nums expected)
        it specName (sut.Add nums) should.equal expected
</pre>
to write our specs:
<pre>
member x.``handles \n separator`` = [
        testAdding "1\n1" 2
        testAdding "1\n2\n3\n4" 10
    ]
</pre>
Additionally this allows us to use F#s capabilities to do all kind of other neat things - the possibilities are endless (did I just say that?). For instance ...

###RowTests 
By mapping values to specifications:
<pre>
 member x.``single numbers`` =
        ["0", 0; "1", 1; "5", 5; "10", 10; "99", 99; "100", 100; "999", 999 ] 
        |> List.map (fun (num, r) -> 
            it (sprintf "adding '%s' returns %d" num r) (sut.Add num) should.equal r)
</pre>
###Multiple assertions without the price
In case you wondered if all specifications get evaluated, even if one of them fails ...

Each *'it'* is wrapped and executed independently from all other *'it's*, and therefore if one fails, **all the others still are evaluated**.

No catch? - well, non-spec related errors (like null references) can make it impossible to create the spec list in the first place, but even that case can be dealt with.

###BDD oriented output
The FSharp spec runner takes context inheritance into account to produce a tree-like output that visualizes this information. This makes it much easier to see under which circumstances specifications are failing and provides more documentation of our code.

An example:
<pre>
+ Given a TestType(0)

|      + named ContextName
|      |
|         - when I increment by 1
|            -  should have Value 1

|      + named ContextName

|      |      + that has been incremented
              |
|      |         - when I increment by 1
|      |            -  should have Value 2

|      |      + that has been incremented

|      |      |      + and was incremented again
|      |      |      |
|      |      |         - when I increment by 1
|      |      |            -  should have Value 3

</pre>
###Tons of more features
 Further more FSharpSpec has support for dealing with exceptions, string and list queries and more, as explained in the more detailed documentation (further down).

Sold? Read on ...

## Getting Started with FSharpSpec

### Downloading and Building FSharpSpec

#### Build from Source
The best way to obtain the source code for FSharpSpec is to [clone its Git repository](http://www.book.git-scm.com/documentation) available via:

`git clone git://github.com/thlorenz/FSharpSpec.git`

##Test Runners
###Native FSharpSpec Runner
FSharpSpec comes with a built in runner that can be invoked from the command line or from within Visual Studio via an external command to run specification assemblies.
Debugging specifications is not supported when they are run this way.

Of course there is nothing keeping you from directly using the [FSharpSpec.RunnerUtils](https://github.com/thlorenz/FSharpSpec/tree/master/src/Runners/FSharpSpec.RunnerUtils/) in order to create a short F# program that runs your tests and thus enables debugging. It's quite simple - really, as can be seen from the built in [FSharpSpec Runner] (https://github.com/thlorenz/FSharpSpec/blob/master/src/Runners/FSharpSpec.ConsoleRunner/Program.fs)

###TD.Net Support
At this point TD.Net can be used to run entire specification assemblies in normal and debug mode.
Unfortunately TD.Net seems to have problems identifying F# namespaces and types, so those cannot be run individually.

###ReSharper Support
In the works (when ever I get around to implement their huge interface).

## Writing Specifications
### Equality Assertions
### List Assertions
### String Assertions
### Type Assertions
### Expected Exceptions
### Using functions to isolate 'it's

