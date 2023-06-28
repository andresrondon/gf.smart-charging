using Microsoft.Azure.Cosmos;

// New instance of CosmosClient class
using CosmosClient client = new(
    accountEndpoint: Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")!,
    authKeyOrResourceToken: Environment.GetEnvironmentVariable("COSMOS_KEY")!
);

// Database reference with creation if it does not already exist
Database database = client.GetDatabase(id: "SmartChargingDB");

Console.WriteLine($"New database:\t{database.Id}");

// Container reference with creation if it does not alredy exist
Container container = await database.CreateContainerIfNotExistsAsync(
    id: "Group",
    partitionKeyPath: "/categoryId",
    throughput: 400
);

Console.WriteLine($"New container:\t{container.Id}");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
