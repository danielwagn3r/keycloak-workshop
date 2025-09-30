using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
using RefreshClient;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext())
        .ConfigureServices((hostContext, services) =>
        {
            // default cache
            services.AddDistributedMemoryCache();

            services.AddClientCredentialsTokenManagement();
            services.AddSingleton(new DiscoveryCache(hostContext.Configuration["Sts:Authority"]));
            // services.AddSingleton<IConfigureOptions<ClientCredentialsClient>, ClientCredentialsClientConfigureOptions>();

            // Configure OAuth access Token management
            services.AddClientCredentialsTokenManagement()
                .AddClient("sts", client =>
                {
                    var sp = services.BuildServiceProvider();

                    var _cache = sp.GetService<DiscoveryCache>();

                    client.TokenEndpoint = new Uri(_cache.GetAsync().GetAwaiter().GetResult().TokenEndpoint);
                    client.ClientId = ClientId.Parse(hostContext.Configuration["Sts:ClientId"]);
                    client.ClientSecret = ClientSecret.Parse(hostContext.Configuration["Sts:ClientSecret"]);

                    client.Scope = Scope.Parse("calc:double");

                    client.Parameters = new Parameters
                    {
                        new("audience", hostContext.Configuration["Api:Audience"])
                    };
                });

            // Configure http client
            services.AddHttpClient("client",
                    client => { client.BaseAddress = new Uri(hostContext.Configuration["Api:BaseAddress"]); })
                .AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse("sts"));

            // services.AddTransient<IClientAssertionService, ClientAssertionService>();

            services.AddHostedService<Worker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}
