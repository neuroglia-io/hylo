using Hylo.Api.Core.Infrastructure;
using Hylo.Api.Core.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Certificate;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

builder.Services.AddSingleton<AuthenticationManager>();
builder.Services.AddSingleton<IAuthenticationManager>(provider => provider.GetRequiredService<AuthenticationManager>());
builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<AuthenticationManager>());

builder.Services.AddSingleton<IdentityRoleManager>();
builder.Services.AddSingleton<IIdentityRoleManager>(provider => provider.GetRequiredService<IdentityRoleManager>());
builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<IdentityRoleManager>());

builder.Services.AddSingleton<IdentityClaimsPrincipalFactory>();
builder.Services.AddSingleton<IIdentityClaimsPrincipalFactory>(provider => provider.GetRequiredService<IdentityClaimsPrincipalFactory>());
builder.Services.AddSingleton<IIdentityClaimsPrincipalFactory>(provider => provider.GetRequiredService<IdentityClaimsPrincipalFactory>());

builder.Services.AddAuthentication()
    .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(BasicAuthenticationDefaults.AuthenticationScheme, options => { })
    .AddCertificate(options =>
    {
        options.Events = new()
        {
            OnCertificateValidated = async (CertificateValidatedContext context) =>
            {
                var authenticationManager = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationManager>();
                var result = await authenticationManager.AuthenticateAsync(CertificateAuthenticationDefaults.AuthenticationScheme, new CertificateAuthenticationProperties(context.ClientCertificate));
                if (!result.Succeeded)
                {
                    context.Fail(result.Failure!);
                    return;
                }
                context.Principal = result.Principal;
                context.Success();
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
