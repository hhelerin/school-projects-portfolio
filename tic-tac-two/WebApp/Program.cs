using DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
//                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var connectionString =$"Data Source={FileHelper.BasePath}app.db";
//connectionString = connectionString.Replace("<%location%>", FileHelper.BasePath);

// register "how to create a db when somebody asks for it"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IConfigRepository, ConfigRepositoryDataBase>();
builder.Services.AddScoped<IGameRepository, GameRepositoryDataBase>();


builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();