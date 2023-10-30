using NetArchTest.Rules;

namespace Bookify.ArchitectureTests;

[TestClass]
public class LayerTests: BaseTest
{
    [TestMethod]
    public void DomainLayer_Should_NotHaveDependencyOn_ApplicationLayer()
    {
        var result = Types.InAssembly(DomainAssembly)
                          .Should()
                          .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
                          .GetResult();
        
        Assert.IsTrue(result.IsSuccessful);
    }

    [TestMethod]
    public void DomainLayer_Should_NotHaveDependencyOn_InfrastructureLayer()
    {
        var result = Types.InAssembly(DomainAssembly)
                          .Should()
                          .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
                          .GetResult();
        
        Assert.IsTrue(result.IsSuccessful);
    }

    [TestMethod]
    public void ApplicationLayer_Should_NotHaveDependencyOn_InfrastructureLayer()
    {
        var result = Types.InAssembly(ApplicationAssembly)
                          .Should()
                          .NotHaveDependencyOn(Infrastructure.GetName().Name)
                          .GetResult();
        
        Assert.IsTrue(result.IsSuccessful);
    }
}
