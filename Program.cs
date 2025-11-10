using MunicipalServicesMVC.Models;
using MunicipalServicesMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IEventCatalog, InMemoryEventCatalog>();

// ✅ Required for Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Chat.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Chatbot services (fully qualified to avoid using/namespace issues)
builder.Services.AddSingleton<MunicipalServicesMVC.Services.Chatbot.FaqService>();
builder.Services.AddSingleton<MunicipalServicesMVC.Services.Chatbot.IssueService>();

// Your existing singleton
builder.Services.AddSingleton<IssueStore>();  // ✅

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Must be BEFORE Authorization
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();