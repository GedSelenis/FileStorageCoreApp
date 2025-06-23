using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFileService, FileDetailsService>();
builder.Services.AddScoped<IVirtualFolderService, VirtualFolderService>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddDbContext<ApplicationDbContext>
    (options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
