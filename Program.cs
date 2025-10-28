var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

//app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/health", () =>
{
    return new { status = "ok" };
})
.WithName("health")
.WithOpenApi();

app.MapGet("/sum", (int a, int b) =>
{
    return new { a, b, sum = a + b };
})
.WithName("GetSum")
.WithOpenApi();

app.MapGet("/divide", (int a, int b) =>
{
    if (b == 0)
    {
        return Results.BadRequest(new { error = "Division by zero is not allowed" });
    }
    return Results.Ok(new { a, b, result = (double)a / b });
})
.WithName("GetDivide")
.WithOpenApi();

app.MapGet("/loadbalance", () =>
{
    var hostname = System.Environment.MachineName;
    var ipAddress = System.Net.Dns.GetHostEntry(hostname).AddressList
        .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "N/A";
    
    return new
    {
        podName = hostname,
        podIp = ipAddress,
        timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
        message = "Load balancing verification endpoint"
    };
})
.WithName("GetLoadBalance")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
