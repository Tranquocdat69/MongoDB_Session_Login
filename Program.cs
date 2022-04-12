using MongoDB_Session_Login.Extensions;
using MongoDB_Session_Login.Models.SessionLogin;
using MongoDB_Session_Login.Services;
using System.Net;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("https://client.fpts.com.vn:5500", "http://client.fpts.com.vn:5500")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
        //.AllowAnyOrigin();
    });
});

builder.Services.Configure<SessionLoginDatabaseSettings>(builder.Configuration.GetSection("SessionLoginDatabase"));
builder.Services.AddSingleton<SessionLoginService>();
builder.Services.AddSingleton<HashService>();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("session_login")
    .AddCookie("session_login", options =>
    {
        options.LoginPath = "/api/SessionLogin/RequireLogin";
        options.AccessDeniedPath = "/api/SessionLogin/RequireLogin";
        options.Cookie.Name = "session_login";
        options.Cookie.Domain = "fpts.com.vn";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.HttpOnly = true;
        options.Cookie.Path = "/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);

      /*  options.Events = new CookieAuthenticationEvents()
        {
            OnSignedIn = context =>
            {
                DateTimeOffset? expiresUtc = context.Properties.ExpiresUtc;
                return Task.CompletedTask;

               *//* context.HttpContext.Response.Cookies.Delete("session_token", CustomCookieOptions.option);
                await context.HttpContext.SignOutAsync();*//*
            }
        };*/
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CheckSessionToken", configPolicyBuilder =>
    {
        configPolicyBuilder.RequireCustomCheckSessionToken();
    });

    options.AddPolicy("CheckLogout", configPolicyBuilder =>
    {
        configPolicyBuilder.RequireCustomCheckLogout();
    });
});
builder.Services.AddDbContext<TAuthContext>(options =>
                 options.UseOracle(connString));
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
            ;
}));

builder.Services.Configure<SessionLoginDatabaseSettings>(builder.Configuration.GetSection("SessionLoginDatabase"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, CheckSessionTokenRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CheckSessionLogoutRequirementHandler>();
builder.Services.AddSingleton<SessionLoginService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger();

app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.UseSession();

//app.UseCheckSessionToken();

app.Run();


