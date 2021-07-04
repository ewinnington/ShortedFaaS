using System;
using System.IO; 
using System.Text; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http; 
using Microsoft.Extensions.Hosting;
using CliWrap; 
using CliWrap.Buffered;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/f/{name}", async context => {

    var name = context.Request.RouteValues["name"];
    await using var input = File.OpenRead(@"C:\Repos\ShortedFaaS\src\deploy\jq\test.json"); 

    var result = await Cli.Wrap(@$"C:\Repos\ShortedFaaS\src\deploy\{name}\jq.exe")
        .WithArguments(".")
        .WithWorkingDirectory("")
        .WithValidation(CommandResultValidation.None)
        .WithEnvironmentVariables(env => env
            .Set("ENV_VAR_1", "Data1")
            .Set("ENV_VAR_1", "Data2"))
        .WithStandardInputPipe(PipeSource.FromStream(input))
        .ExecuteBufferedAsync(); 

    if (result.ExitCode == 0) 
        {
            await context.Response.WriteAsync(result.StandardOutput); 
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(result.StandardError);
        }
    }
);

app.MapPost("/f/{name}", async context => {

    var name = context.Request.RouteValues["name"];

    using StreamReader input = new StreamReader(context.Request.Body, Encoding.UTF8);
    string rawinput = await input.ReadToEndAsync(); 

    var result = await Cli.Wrap(@$"C:\Repos\ShortedFaaS\src\deploy\{name}\jq.exe")
        .WithArguments(".")
        .WithWorkingDirectory("")
        .WithValidation(CommandResultValidation.None)
        .WithEnvironmentVariables(env => env
            .Set("ENV_VAR_1", "Data1")
            .Set("ENV_VAR_1", "Data2"))
        .WithStandardInputPipe(PipeSource.FromString(rawinput))
        .ExecuteBufferedAsync(); 

    if (result.ExitCode == 0) 
        {
            await context.Response.WriteAsync(result.StandardOutput); 
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(result.StandardError);
        }
    }
);

app.Run();
