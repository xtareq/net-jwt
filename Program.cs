using njwt.Middlewares;
using njwt.Services;
using njwt.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddControllers();

builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("JWT"));

// configure DI for application Services


builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IHashUtils, HashUtils>();
builder.Services.AddScoped<IUserService, UserService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseMiddleware<JwtMiddleware>();


app.MapControllers();

app.Run();
