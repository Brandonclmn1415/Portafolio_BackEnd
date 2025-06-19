using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNetlify", policy =>
    { 
        policy.WithOrigins("https://portafoliobrandonclmn1415.netlify.app")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseCors("AllowNetlify");
}

app.UseHttpsRedirection();


var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".webmanifest"] = "application/manifest+json";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.Use(async (context, next) =>
    {
        await next();

        if (context.Response.StatusCode == 404 &&
            !context.Request.Path.Value.StartsWith("./api") &&
            !Path.HasExtension(context.Request.Path.Value))
        {
            context.Request.Path = "/index.html";
            await next();
        }
    });

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=()";

    await next();
});


app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();