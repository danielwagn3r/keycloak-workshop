using Serilog;
using System.Security.Claims;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

try {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = builder.Configuration["Sts:Authority"];
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidAudiences = new[] { builder.Configuration["Api:Audience"] };
            options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
            
            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents()
                    {
                        OnTokenValidated = async (context) =>
                        {
                            if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
                            {
                                var scopeClaims = claimsIdentity.FindFirst("scope");
                                if (scopeClaims is not null)
                                {
                                    claimsIdentity.RemoveClaim(scopeClaims);
                                    claimsIdentity.AddClaims(scopeClaims.Value.Split(' ').Select(scope => new Claim("scope", scope)));
                                }
                            }
                            await Task.CompletedTask;
                        }
                    };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("calc:double", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "calc:double");
        });

        options.AddPolicy("calc:square", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "calc:square");
        });

    });

    var app = builder.Build();

    // Configure Serilog request logging.
    app.UseSerilogRequestLogging();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    // app.MapControllers().RequireAuthorization("ApiScope");
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}