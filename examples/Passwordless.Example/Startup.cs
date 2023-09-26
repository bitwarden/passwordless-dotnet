namespace Passwordless.Example;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        // add support for routing to controllers
        services.AddControllers();

        // Inject the Passwordless SDK
        services.AddPasswordlessSdk(options =>
        {
            Configuration.GetRequiredSection("Passwordless").Bind(options);
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        // add support for serving index.html
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}