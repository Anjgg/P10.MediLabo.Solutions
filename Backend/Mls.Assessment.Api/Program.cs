using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi;
using Mls.Assessment.Api.Calculators;
using Mls.Assessment.Api.Clients;
using Mls.Assessment.Api.Services;
using Mls.Library.Authentication;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IPatientsApiClient, PatientsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:PatientsApiBaseUrl"]!);
    var username = builder.Configuration["Services:Username"];
    var password = builder.Configuration["Services:Password"];
    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
});

builder.Services.AddHttpClient<INotesApiClient, NotesApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:NotesApiBaseUrl"]!);
    var username = builder.Configuration["Services:Username"];
    var password = builder.Configuration["Services:Password"];
    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
});

builder.Services.AddScoped<IRiskAssessmentCalculator, RiskAssessmentCalculator>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Basic",
        In = ParameterLocation.Header,
        Description = "Authentification HTTP Basic (nom d'utilisateur / mot de passe)."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Basic", document)] = []
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
