using Microsoft.EntityFrameworkCore;
using MongoDB_Session_Login.Data;
using MongoDB_Session_Login.Extensions;
using MongoDB_Session_Login.Models.SessionLogin;
using MongoDB_Session_Login.Services;

var builder = WebApplication.CreateBuilder(args);
string connString = builder.Configuration.GetConnectionString("Oracle");
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.Configure<SessionLoginDatabaseSettings>(builder.Configuration.GetSection("SessionLoginDatabase"));
builder.Services.AddSingleton<SessionLoginService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(config => {
    config.Cookie.Name = "session_login";
    config.IdleTimeout = new TimeSpan(0, 0, 30);
});
builder.Services.AddDbContext<OracleContext>(options =>
                 options.UseOracle(connString));
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
            ;
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("MyPolicy");
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSession();

app.UseCheckSessionToken();

app.Run();


