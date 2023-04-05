using ServiceProviderRatingsAndNotification.Rating;
using ServiceProviderRatingsAndNotification.ServiceProvider;
using ServiceProviderRatingsAndNotification.ServiceProviderNotification;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IServiceProviderRepository>(_ => 
// in real word application this should come from appsettings.json, but there the only purpose is to run locally.
    new ServiceProviderRepository("Server=localhost;Database=spratingsdb;User Id=sa;Password=SuperPass92!")
);
builder.Services.AddSingleton<IServiceProviderNotifier>(_ => new ServiceProviderNotifierWithRabbitMq("localhost"));
builder.Services.AddTransient<RatingService>();
builder.Services.AddTransient<ServiceProviderService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// this is removed for this assignment for the sake of simplicity, making this app runnable from docker container
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();