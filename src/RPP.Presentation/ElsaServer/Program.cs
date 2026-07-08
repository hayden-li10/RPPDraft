using RPP.Orchestration.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

var services = builder.Services;
var configuration = builder.Configuration;

services.AddOrchestration(configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//UseHttpsRedirection must be disabled to allow Docker HTTP-only network
app.UseHttpsRedirection();// diable this for local distributed environment

app.MapStaticAssets();
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseOrchestration(); 

app.MapFallbackToPage("/_Host");

app.Run();