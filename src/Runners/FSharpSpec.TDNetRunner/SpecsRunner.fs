namespace FSharpSpec.TDNetRunner

open System.Reflection
open TestDriven.Framework;

type TestDrivenNetRunner() =
    interface ITestRunner with
        member x.RunAssembly(listener : ITestListener , assembly : Assembly) = TestRunState.NoTests
        member x.RunMember(listener : ITestListener , assembly : Assembly, memberInfo : MemberInfo) = TestRunState.NoTests
        member x.RunNamespace(listener : ITestListener , assembly : Assembly, nameSpace : string) = TestRunState.NoTests

    

    