using Passwordless;
using Passwordless.MultiTenancy.Example.Passwordless;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Multi-tenancy: For accessing the current HTTP context
builder.Services.AddHttpContextAccessor();

// Multi-tenancy: To build the PasswordlessClient
builder.Services.AddSingleton<IPasswordlessClientBuilder, PasswordlessClientBuilder>();

// Multi-tenancy: To get the multi-tenant api secrets from our configuration
builder.Services.AddOptions<PasswordlessMultiTenancyConfiguration>().BindConfiguration("Passwordless");

// Multi-tenancy: Integrate the multi-tenant PasswordlessClient with the HttpClient
builder.Services.AddHttpClient<IPasswordlessClient, PasswordlessClient>((http, sp) =>
{
    
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var host = httpContextAccessor.HttpContext!.Request.Host;

    // gameofthrones or thewalkingdead tenant
    var tenant = host.Host.Split('.')[0];

    var clientBuilder = sp.GetRequiredService<IPasswordlessClientBuilder>();
    clientBuilder.WithTenant(tenant);

    var passwordlessClient = clientBuilder.Build();
    return passwordlessClient;
});

builder.Services.AddScoped(sp => (PasswordlessClient)sp.GetRequiredService<IPasswordlessClient>());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();