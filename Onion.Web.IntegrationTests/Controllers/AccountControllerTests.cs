using Xunit;

namespace Onion.Web.IntegrationTests.Controllers
{
    [CollectionDefinition("AccountController", DisableParallelization = true)]
    public partial class AccountControllerTests : ControllerTest<InMemoryDatabaseAppFactory>
    {
        public AccountControllerTests(InMemoryDatabaseAppFactory fixture)
            : base(fixture)
        {
        }
    }
}