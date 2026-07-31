using Frontend.Services;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IPatientApiClient, PatientApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayBaseUrl"]!);

    var username = builder.Configuration["ApiSettings:Username"];
    var password = builder.Configuration["ApiSettings:Password"];
    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
});

builder.Services.AddHttpClient<INotesApiClient, NotesApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayBaseUrl"]!);

    var username = builder.Configuration["ApiSettings:Username"];
    var password = builder.Configuration["ApiSettings:Password"];
    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
});

builder.Services.AddHttpClient<IAssessmentApiClient, AssessmentApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayBaseUrl"]!);

    var username = builder.Configuration["ApiSettings:Username"];
    var password = builder.Configuration["ApiSettings:Password"];
    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
