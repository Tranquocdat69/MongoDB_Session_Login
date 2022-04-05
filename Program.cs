using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using MongoDB_Session_Login.AuthorizationRequirements;
using MongoDB_Session_Login.Extensions;
using MongoDB_Session_Login.Models.SessionLogin;
using MongoDB_Session_Login.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("https://client.fpts.com.vn:5500")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
        //.AllowAnyOrigin();
    });
});
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("session_login")
    .AddCookie("session_login", options =>
    {
        options.Cookie.Name = "session_login";
        options.LoginPath = "/api/SessionLogin/RequireLogin";
        options.AccessDeniedPath = "/api/SessionLogin/RequireLogin";
        options.Cookie.Domain = "fpts.com.vn";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = false;
    });

/*builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(config => {
    config.Cookie.Name = "session_login";
    config.IdleTimeout = new TimeSpan(0, 0, 30);
});*/

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CheckSessionToken", configPolicyBuilder =>
    {
        configPolicyBuilder.RequireCustomCheckSessionToken();
    });
});

builder.Services.Configure<SessionLoginDatabaseSettings>(builder.Configuration.GetSection("SessionLoginDatabase"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, CheckSessionTokenRequirementHandler>();
builder.Services.AddSingleton<SessionLoginService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  /*  app.UseSwagger();
    app.UseSwaggerUI();*/
}
app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.UseSession();

//app.UseCheckSessionToken();

app.Run();


