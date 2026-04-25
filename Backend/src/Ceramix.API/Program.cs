using Ceramix.API.Middleware;
using Ceramix.Application.Interfaces;
using Ceramix.Application.Services;
using Ceramix.Domain.Entities;
using Ceramix.Domain.Interfaces;
using Ceramix.Infrastructure.Data;
using Ceramix.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CeramixDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("Ceramix.Infrastructure")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<WorkshopRepository>();
builder.Services.AddScoped<EnrollmentRepository>();
builder.Services.AddScoped<ScheduleRepository>();


builder.Services.AddScoped<IWorkshopService,    WorkshopService>();
builder.Services.AddScoped<IInstructorService,  InstructorService>();
builder.Services.AddScoped<IStudentService,     StudentService>();
builder.Services.AddScoped<IEnrollmentService,  EnrollmentService>();
builder.Services.AddScoped<IScheduleService,    ScheduleService>();
builder.Services.AddScoped<IPaymentService,     PaymentService>();
builder.Services.AddScoped<IReportService,      ReportService>();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Ceramix API", Version = "v1",
        Description = "API de gestión para talleres de cerámica." });
});

builder.Services.AddCors(opt => opt.AddPolicy("CeramixFrontend", policy =>
    policy.WithOrigins("http://localhost:5173")
          .AllowAnyHeader()
          .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CeramixDbContext>();
    await db.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(db);
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ceramix API v1"));
}

app.UseHttpsRedirection();
app.UseCors("CeramixFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();