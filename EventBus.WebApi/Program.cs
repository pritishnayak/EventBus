using EventBus.Library.Sender;
using EventBus.WebApi.Processors;

var builder = WebApplication.CreateBuilder(args);

// Register Typed Sender
builder.Services.AddTransient(typeof(IMessageSender<>), typeof(MessageSender<>));

// Register generic Sender
builder.Services.AddTransient<IMessageSender>(x => ActivatorUtilities.CreateInstance<MessageSender>(x, "Generic"));

// Add services to the container.
builder.Services.RegisterMessageProcessor<AntProcessor>();
builder.Services.RegisterMessageProcessor<BatProcessor>();

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

app.MapGet("/" => "Hello");

app.MapControllers();

app.Run();
