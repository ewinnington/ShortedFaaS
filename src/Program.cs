using System;
using System.IO; 
using System.Text; 
using System.Collections.Generic;
using System.Collections.Concurrent; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http; 
using Microsoft.Extensions.Hosting;
using CliWrap; 
using CliWrap.Buffered;
using ewinnington.ShortedFaas; 

string deploy_root = @"C:\Repos\ShortedFaaS\src\deploy\"; 

var HostedFunctions = new ConcurrentDictionary<string, HostedFunction>(); 
HostedFunctions["jq"] = new HostedFunction("jq", @"jq.exe", new List<string>{"."}, new Dictionary<string, string>());
HostedFunctions["py-hi"] = new HostedFunction("py-hi", @"python", new List<string>(){@"py-hi.py"}, new Dictionary<string, string>());
HostedFunctions["py-json"] = new HostedFunction("py-json", @"python", new List<string>(){@"py-json.py"}, new Dictionary<string, string>());
HostedFunctions["js-hi"] = new HostedFunction("js-hi", @"node", new List<string>(){@"js-hi.js"}, new Dictionary<string, string>());
HostedFunctions["js-json"] = new HostedFunction("js-json", @"node", new List<string>(){@"js-json.js"}, new Dictionary<string, string>());


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapPost("/f/{name}", async context => {

    var name = context.Request.RouteValues["name"];
    var target = HostedFunctions[name.ToString()]; 

    var result = await Cli.Wrap(target.Cli)
        .WithArguments(target.Arguments)
        .WithWorkingDirectory(Path.Combine(deploy_root, name.ToString()))
        .WithValidation(CommandResultValidation.None)
        .WithEnvironmentVariables(target.Environment)
        .WithStandardInputPipe(PipeSource.FromStream(context.Request.Body))
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
