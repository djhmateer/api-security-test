using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.Configure<KestrelServerOptions>(options =>
//{
//    options.ConfigureHttpsDefaults(options =>
//        options.ClientCertificateMode = ClientCertificateMode.RequireCertificate);
//});

builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {
        options.Events = new CertificateAuthenticationEvents
        {
            OnAuthenticationFailed = context =>
              {
                  return Task.CompletedTask;
              },

            OnCertificateValidated = context =>
            {
                var claims = new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        context.ClientCertificate.Subject,
                        ClaimValueTypes.String, context.Options.ClaimsIssuer),
                    new Claim(
                        ClaimTypes.Name,
                        context.ClientCertificate.Subject,
                        ClaimValueTypes.String, context.Options.ClaimsIssuer)
                };

                context.Principal = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, context.Scheme.Name));
                context.Success();

                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseAuthentication();

app.MapGet("/", () => "Hello World2!");

app.MapGet("/anon", () => "Anon endpoint2").AllowAnonymous();

app.MapGet("/auth", () => "This endpoint requires authorization").RequireAuthorization();

app.Run();