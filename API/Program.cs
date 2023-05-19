using API.DTOs;
using API.Extensions;
using API.Middleware;
using API.Services;
using API.Soap;
using Application.Core;
using Application.Plants;
using Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt => {
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(opt =>  {
    // opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    // opt.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
    opt.UseMySQL(builder.Configuration.GetConnectionString("Default"));
});
// builder.Services.AddTransient<MySqlConnection>(_ =>
//     new MySqlConnection(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMediatR(typeof(List.Handler));
builder.Services.AddAutoMapper(typeof(MappingPlants).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Create>();

// Identity (Authnetication and Authorization) configuration file
builder.Services.AddIdentityServices(builder.Configuration);

// Email Configuration : used MailKit Package
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

//forgot password token generation config
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromHours(1));

builder.Services.AddHttpContextAccessor();

builder.Services.AddSoapCore();
builder.Services.TryAddScoped<ISampleService, SampleService>();
builder.Services.TryAddScoped<IGenerateUrlsService, GenerateUrlsService>();
// builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// plant Login api(soap api) endpoint
app.UseSoapEndpoint<ISampleService>("/plantLogin.asmx", new SoapEncoderOptions());
app.UseSoapEndpoint<IGenerateUrlsService>("/GenerateUrls.asmx", new SoapEncoderOptions());

app.MapControllers();

// To save migrations automatically
// using var scope = app.Services.CreateScope();
// var services = scope.ServiceProvider;
// try
// {
//     var context = services.GetRequiredService<DataContext>();
//     var userManager = services.GetRequiredService<UserManager<User>>();
//     await context.Database.MigrateAsync();
//     await Seed.SeedData(context,userManager);
// }
// catch (Exception ex)
// {
//     var logger = services.GetRequiredService<ILogger<Program>>();
//     logger.LogError(ex,"An error from Migration");
// }

app.Run();
