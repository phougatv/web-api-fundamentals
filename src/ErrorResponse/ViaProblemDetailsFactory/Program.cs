using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApi.ErrorResponse.ViaProblemDetailsFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<ProblemDetailsFactory, MyProblemDetailsFactory>();    //<-- Register custom ProblemDetailsFactory

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	//app.UseExceptionHandler("/error-development");
	app.UseExceptionHandler("/error");
}
else
{
	app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
