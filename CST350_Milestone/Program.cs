var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    // Set the session timeout
    option.IdleTimeout = TimeSpan.FromMinutes(30);
    // Make the session cookies HTTP only
    option.Cookie.HttpOnly = true;
    // Make the session coolie essential
    option.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
