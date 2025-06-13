using Microsoft.AspNetCore.Diagnostics;
using SponsorshipApp.API.Background;
using SponsorshipApp.Application.Interfaces;
using SponsorshipApp.Application.Services;
using SponsorshipApp.Infrastructure.Interfaces;
using SponsorshipApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICommunityProjectRepository, InMemoryCommunityProjectRepository>();
builder.Services.AddSingleton<ISponsorshipPlanRepository, InMemorySponsorshipPlanRepository>();
builder.Services.AddSingleton<ICommunityProjectService, CommunityProjectService>();
builder.Services.AddSingleton<ISponsorshipPlanService, SponsorshipPlanService>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddHostedService<SponsorshipScheduler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

//Global Exception Handler (Improved Formatting)
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = exceptionHandlerPathFeature?.Error;

        context.Response.StatusCode = ex switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException or InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var error = new
        {
            message = ex?.Message,
            type = ex?.GetType().Name
        };

        await context.Response.WriteAsJsonAsync(error);
    });
});

app.MapControllers();

app.Run();
