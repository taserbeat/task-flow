using Web.IntegrationTests.Environments;
using Xunit;

namespace Web.IntegrationTests.Collections
{
    [CollectionDefinition(CollectionDefinitionNames.Integration)]
    public class IntegrationCollection : ICollectionFixture<TestEnvironment>
    {
    }
}