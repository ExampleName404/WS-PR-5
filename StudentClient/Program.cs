using StudentClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("StudentApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["StudentApi:BaseAddress"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<IStudentApiService, StudentApiService>();

var app = builder.Build();

app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
