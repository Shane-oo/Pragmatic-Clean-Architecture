using Bookify.Domain.Abstractions;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Domain;

[TestClass]
public class DomainTests: BaseTest
{
    [TestMethod]
    public void DomainEvent_Should_Have_DomainEventSuffix()
    {
        var result = Types.InAssembly(DomainAssembly)
                          .That()
                          .ImplementInterface(typeof(IDomainEvent))
                          .Should()
                          .HaveNameEndingWith("DomainEvent")
                          .GetResult();
        
        Assert.IsTrue(result.IsSuccessful);
    }
}
