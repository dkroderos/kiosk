using Kiosk.Api;
using Kiosk.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseHttpsRedirection();

var connection = builder.Configuration.GetConnectionString("DefaultConnection") ??
                 throw new InvalidOperationException("No Connection");
var repo = new QueueRepository(connection);

app.MapGet("/queue", async () =>
{
    var queues = await repo.GetAllAsync();

    return Results.Ok(queues);
});

app.MapGet("/queue/{id:int}", async (int id) =>
{
    var queue = await repo.GetByIdAsync(id);
    return queue is null ? Results.NotFound("No queue") : Results.Ok(queue);
});

app.MapPost("/queue", async (CreateQueueDto createQueueDto) =>
{
    var queue = await repo.CreateAsync(createQueueDto);
    return queue is null ? Results.BadRequest("Cannot create") : Results.Ok(queue);
});

app.MapDelete("/queue/{id:int}", async (int id) =>
{
    var success = await repo.DeleteByIdAsync(id);
    return success ? Results.Ok() : Results.BadRequest($"Cannot delete {id}");
});

app.Run();