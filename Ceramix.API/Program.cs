using Ceramix.Infrastructure.Persistence;
using Ceramix.Application.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CeramixDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<PieceService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<FiringService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();