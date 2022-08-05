//using TechTalk.SpecFlow;
//using TechTalk.SpecFlow.UnitTestProvider;

//namespace BBlog.Tests.Steps;

//[Binding, Scope(Tag = "Manual")]
//public class ManualSteps
//{
//    private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider;

//    public ManualSteps(IUnitTestRuntimeProvider unitTestRuntimeProvider)
//    {
//        _unitTestRuntimeProvider = unitTestRuntimeProvider;
//    }

//    [Given(".*"), When(".*"), Then(".*")]
//    public void EmptyStep()
//    {
//        Ignore();
//    }

//    [Given(".*"), When(".*"), Then(".*")]
//    public void EmptyStep(string multiLineStringParam)
//    {
//        Ignore();
//    }

//    [Given(".*"), When(".*"), Then(".*")]
//    public void EmptyStep(Table tableParam)
//    {
//        Ignore();
//    }

//    private void Ignore()
//    {
//        _unitTestRuntimeProvider.TestIgnore("manual test");
//    }
//}