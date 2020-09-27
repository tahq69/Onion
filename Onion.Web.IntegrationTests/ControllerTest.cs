using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Onion.Web.IntegrationTests
{
    public abstract class ControllerTest<TFixture> : IClassFixture<TFixture>
        where TFixture : class
    {
        protected ControllerTest(TFixture fixture)
        {
            Factory = fixture;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// Gets the client application factory.
        /// </summary>
        public TFixture Factory { get; }

        /// <summary>
        /// Reads the content of the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Content response string.</returns>
        protected async Task<string> ReadResponseContent(HttpResponseMessage response)
        {
            using Stream stream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Reads the content of the response.
        /// </summary>
        /// <typeparam name="T">Type of the JSON content.</typeparam>
        /// <param name="response">The response.</param>
        /// <returns>Deserialized content response.</returns>
        protected async Task<T> ReadResponseContent<T>(HttpResponseMessage response)
        {
            string content = await ReadResponseContent(response);

            content.Should().NotBeNullOrWhiteSpace("Response content is empty.");

            return JsonConvert.DeserializeObject<T>(content);
        }

        protected HttpContent ToJsonContent(object value)
        {
            string json = JsonConvert.SerializeObject(value);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}