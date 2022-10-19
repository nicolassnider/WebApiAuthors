using WebApiAuthors;

var builder = WebApplication.CreateBuilder(args);

//Instances a startup
var startup = new Startup(builder.Configuration);
// Add services to the container.
startup.ConfigureServices(builder.Services);

var app = builder.Build();
var serviceLogger = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));
startup.Configure(app, app.Environment,serviceLogger);

app.Run();
