using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IFileService, FileDetailsService>();
builder.Services.AddSingleton<IVirtualFolderService, VirtualFolderService>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
