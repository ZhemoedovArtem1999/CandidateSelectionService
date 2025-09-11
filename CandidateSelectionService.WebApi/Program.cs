namespace CandidateSelectionService.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureServices(builder.Configuration);

            builder.Services.ConfigureUserServices(builder.Configuration);

            builder.Services.ConfigureRepositories(builder.Configuration);

            var app = builder.Build();

            Startup.Configure(app, app.Environment);
            Startup.Migrate(app, app.Services);

            await app.RunAsync();
        }
    }
}
