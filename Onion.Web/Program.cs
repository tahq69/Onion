namespace Onion.Web
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The application entry class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The entry arguments.</param>
        public static void Main(string[] args)
        {
            Program.CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the host builder instance for web application.
        /// </summary>
        /// <param name="args">The entry arguments.</param>
        /// <returns>Application host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}