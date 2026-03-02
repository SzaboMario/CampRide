using CampRide.Components;
using CampRide.Data;
using CampRide.Seeder;
using CampRide.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=campride.db");
});

builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<CaravanService>();

var app = builder.Build();

var imagesRootPath = Path.Combine(app.Environment.WebRootPath, "images", "caravans");

if(!Directory.Exists(imagesRootPath))
{
    Directory.CreateDirectory(imagesRootPath);
}

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbInitializer.Seed(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
