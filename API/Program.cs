using Application.Handlers.CommandHandlers;
using Domain.Entities;
using Domain.Repositories;
using FluentValidation.AspNetCore;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICommandRepository<ClinicalTrial>, ClinicalTrialCommandRepository>();
builder.Services.AddScoped<IQueryRepository<ClinicalTrial>, ClinicalTrialQueryRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SaveTrialHandler>());
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UploadJsonFileValidator>());
//builder.Services.AddValidatorsFromAssemblyContaining<UploadJsonFileValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
