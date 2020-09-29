using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Onion.Web.IntegrationTests.Controllers
{
    public class MetaControllerTests : ControllerTest<InMemoryDatabaseAppFactory>
    {
        public MetaControllerTests(InMemoryDatabaseAppFactory fixture)
            : base(fixture)
        {
        }


        [Fact, Trait("Category", "Integration")]
        public async Task MetaController_Info_ReturnsCorrelationIdHeader()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/api/info");
            string content = await ReadResponseContent(response);

            // Assert
            response.Headers.Any(h => h.Key == "CorrelationId").Should().BeTrue();
        }
    }
}