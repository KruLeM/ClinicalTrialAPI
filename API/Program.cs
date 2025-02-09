using Application.Handlers.CommandHandlers;
using Domain.Entities;
using Domain.Repositories;
using FluentValidation.AspNetCore;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using API.Validation.Validators;
using API.Exceptions;
using System.Reflection;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Clinical Trial Service",
        Description = "Tehnical task for 4Create interview",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Mirko Kruscic",
            Email = "krule988@gmail.com"
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddScoped<ICommandRepository<ClinicalTrial>, ClinicalTrialCommandRepository>();
builder.Services.AddScoped<IQueryRepository<ClinicalTrial>, ClinicalTrialQueryRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddTrialHandler>());
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionDocker")));
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UploadJsonFileValidator>());
builder.Services.AddValidatorsFromAssemblyContaining<PaginationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetTrialByStatusValidator>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception)
    {
    }
}

app.Run();
