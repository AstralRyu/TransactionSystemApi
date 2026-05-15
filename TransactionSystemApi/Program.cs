using Microsoft.EntityFrameworkCore;
using TransactionSystemApi;
using TransactionSystemApi.Infrastructure;
using TransactionSystemApi.Repositories;
using TransactionSystemApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddControllers();  // registers controller services
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddHttpClient<ITreasuryApiClient, TreasuryApiClient>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exception = context.Features
        .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

    (int statusCode, string message) = exception switch
    {
        ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
        KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
        InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
    };

    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { error = message });
}));

// app.UseHttpsRedirection();
app.MapControllers();  

app.Run();