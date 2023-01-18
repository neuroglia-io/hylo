using Hylo.Api.Admission.Infrastructure.Configuration;
using Hylo.Api.Authorization.Core.Configuration;
using Hylo.Api.Authorization.Infrastructure.Configuration;
using Hylo.Api.Commands.Resources;
using Hylo.Api.Configuration;
using Hylo.Api.Core.Infrastructure.Services;
using Hylo.Api.Server.Controllers;
using MediatR;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var authentication = builder.Services.AddAuthentication();
var authorization = builder.Services.AddAuthorization();
var mvc = builder.Services.AddControllers();

var hyloApiBuilder = new HyloApiBuilder(builder.Environment, builder.Configuration, builder.Services);
hyloApiBuilder.UseCoreApi();
hyloApiBuilder.UseAuthorizationApi();
hyloApiBuilder.UseAdmissionApi();
hyloApiBuilder.Build();

builder.Services.AddMediatR(typeof(V1CreateResourceCommand).Assembly);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(ResourcesEndpoint).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("redis")!);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<RedisServiceBus>();
builder.Services.AddSingleton<IServiceBus>(provider => provider.GetRequiredService<RedisServiceBus>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<RedisServiceBus>());
builder.Services.AddScoped<IResourceRepository, RedisResourceRepository>();

builder.Services.AddSingleton<ResourceEventBus>();
builder.Services.AddSingleton<IResourceEventBus>(provider => provider.GetRequiredService<ResourceEventBus>());
builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<ResourceEventBus>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
