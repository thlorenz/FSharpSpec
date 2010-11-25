namespace FSharpSpec.Katas.StringCalculator  

open FSharpSpec
open System

type ``StringCalculator Specs``() = 
    let sut = new StringCalculator()

    // Helper Function to archieve more succinct specifications
    let addsTo expected (nums:string) = 
        
        let specName = (sprintf "adding \"%s\" returns %d" nums expected)
        it specName (sut.Add nums) should.equal expected

    // When only one assertion is made it can be returned as is (instead of inside a list)
    member x.``adding empty string`` = 
        it "returns 0" (sut.Add "") should.equal 0 
    
    // Uses 'Row test' approach by mapping inputs and expected outputs to specifications
    member x.``single numbers`` =
        ["0", 0; "1", 1; "5", 5; "10", 10; "99", 99; "100", 100; "999", 999 ] 
        |> List.map (fun (num, r) -> 
            it (sprintf "adding \"%s\" returns %d" num r) (sut.Add num) should.equal r)
    
    // The fact that we can use a function to return specifications allows to use helper functions easily
    member x.``adding two numbers`` =  [ 
        "0,1"       |> addsTo 1 
        "0,2"       |> addsTo 2 
        "1,1"       |> addsTo 2 
        "99,100"    |> addsTo 199
    ]
    
    member x.``adding more than two numbers`` = [
        "1,1,1,1"           |> addsTo 4 
        "1,2,3,4"           |> addsTo 10 
        "2,2,2,2,2,2,2,2"   |> addsTo 16 
    ]

    member x.``handles \n separator`` = [
        "1\n1"          |> addsTo 2 
        "1\n2\n3\n4"    |> addsTo 10 
    ]

    member x.``handles \n separator mixed with ',' separator`` = [
        "1\n2,3\n4" |> addsTo 10 
    ]

    member x.``handles any separator specified via //[delimiter]\n[numbers…]`` = [
        "//&\n1&1"      |> addsTo 2 
        "//^\n1^2^3^4"  |> addsTo 10 
    ]

    member x.``handles any separator specified via //[delimiter]\n[numbers…] and mixed with ',' and '\n'`` = [
        "//^\n1^2,3\n4" |> addsTo 10 
    ]

    // The following specification demonstrates catching an expected exception and then querying its properties
    // As can be seen, any setup code (in this case catching the exception) can be put inside the property
    // before the list of specifications is returned
    member x.``when given string that includes negative numbers`` = 
        let ex = catch (fun () -> sut.Add("-1,2,-3") |> ignore)
        [
            // should.be has a special overload that compares the type of a given object to a System.Type
            it "raises an ArgumentException" ex should.be typeof<ArgumentException>
            it "the exception message contains -1" ex.Message should.contain "-1"
            it "the exception message does not contain 2" ex.Message shouldn't.contain "2"
            it "the exception message contains -3" ex.Message should.contain "-3"
        ]

    // The following specification demonstrates how to implement the above specifications, by catching and querting the exception directly.
    // One disadvantage here is the rather verbose (fun -> ... ) syntax, but for simple exception assertions this is usefull.
    // Additionally in this case, the add operation is performed 4 times vs. 1 time when using catch and thus will run somewhat slower.
    member x.``when given string that includes negative numbers inline`` = 
        [
            it "raises an ArgumentException" (fun () -> sut.Add "-1,2,-3") should.failWith typeof<ArgumentException>
            it "the exception message contains -1" (fun () -> sut.Add "-1,2,-3") should.failWithMessageContaining "-1"
            it "the exception does not message contain 2" (fun () -> sut.Add "-1,2,-3") should.failWithMessageNotContaining "2"
            it "the exception message contains -3" (fun () -> sut.Add "-1,2,-3") should.failWithMessageContaining "-3"
        ]

    member x.``when given string that includes numbers > 10000 they will be ignored in the calculation`` = [
        "1001,1"            |> addsTo 1 
        "1, 2, 9999, 3, 4"  |> addsTo 10 
    ]


   


