namespace Onion.Web.IntegrationTests.Controllers
{
    public partial class AccountControllerTests : ControllerTest<InMemoryDatabaseAppFactory>
    {
        public AccountControllerTests(InMemoryDatabaseAppFactory fixture)
            : base(fixture)
        {
        }
    }
}