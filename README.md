# ShortedFaaS

A no-isolation function as a service provider written to explore what is needed to run a FaaS. 

It’s called Shorted because if you run current through a conductor with no insulation, you’ll get a short any time something else comes into contact with the conductor. 

Do not use in production. No security guarantees and no isolation. 

The host OS must explicitly provide the runners (Python, Node, Lua, …) for the various languages to run if interpreted mode is used. 

## How does ShortedFaaS work?

Packaged functions are deployed as a folder to the /deploy folder. ShortedFaaS reads the run.json file to understand how to launch the function. By default, the route for the functions is /f/{folder_name}. A file system watcher checks the /deploy folder for additions and removals of folders.

### Sample run files

- run.json for an executable

    { “type”:”executable”, 
    “language”:”x86-64”, “command”:”jq.exe”, “arguments”:”.”}

- run.json for a Python file
 
     { “type”:”interpreted”, 
     “language”:”python”, “command”:”python”, “arguments”:”lambda.py”}

Exposed functions respond to a HTTP Post request. The entire body is passed to the function and any output is returned. Headers are used for authentication {Authorization: SharedSecret xxxxx, Authorization: Bearer xxxxx, …}. Auth is dealt with before calling the function. 

Direct Executables can be deployed. The stdin will be filled with the Body of the Post Request. The stdout will be returned to the caller with an HTTP code of 200 as long as the exit code of the process is 0, otherwise an HTTP 500 code is returned with no body.

For interpreted or hosted languages a standard function definition lambda(input, environment) is expected. In NodeJS, this file will be .require and then run. In Python, the file will be loaded as a module. A launcher base in the language is present in ShortedFaaS that will take care of reading stdin and passing it dynamically to the lambda function and writing any output to the stdout. For this to run successfully, the host operating system must have the exact required interpreter and libraries installed on the host and available on the path. The default return value is an HTTP 200, unless an error or exception occurs, then a 500 will be returned if the launcher base is able to trap the exception. A function can also set environment.code (default 200 / 202 if called async).

A production level FaaS would create isolation with containers which include the runtime and libraries necessary to run. Here, the host must provide everything for simplicity.

Asynchronous calls to the functions are not planned to be supported in the first iteration but the design is as follows: The path to an asynchronous function is /a/{folder_name} and returns an HTTP 202 result with an id. The result can be collected from /r/{id} when finished (HTTP 200 on success with body, 500 on failure), if called before a result is available then HTTP 102 is the reply.

ShortedFaaS provides:

- /list which returns the listing of functions loaded
- /history which returns the last executed functions and their return code (probably in memory only - last X invocations available)
- /history/{id} when logging all data, the inputs, outputs, errors, exit codes and HTTP response are returned (probably in memory only - last X invocations available)

Ideally, the lambda definition for hosted languages should as close as possible to AWS, Azure and Google functions definitions. The type could even be specified in run.json to support all three.

## Why shorted?

Because I want to see what is needed for a FaaS provider. 

## Will this progress beyond a prototype?

Probably not, if you have a use case for a function as a service provider use OpenFaaS, fassd, kNative, OpenWhisk, Fn or any other of the cloud provider function as a service services (Azure, AWS, Google Cloud, …).

## Why doesn’t the lambda receive a json object instead of a raw string?

Because I don’t want to implement any type of logic in ShortedFaaS, so the body of the request is passed in as-is. Also means you can receive xml, csv, base64 or whatever you want. Ideally, run.json would tell ShortedFaaS what it accepts, but that’s a feature I’m not (yet) planning on. It would be implemented as a list of Accept mime types and ShortedFaaS would reject any invocation that doesn’t pass the right mime type header (no payload validation) and no conversion. Up to the lambda to read the input mime type from the environment and do the conversion there. Similarly for the output, the accept mime type header would be passed to the environment, up to the lambda to respect it or not. 

## How does the Authentication work?

If enabled at ShortedFaaS host level all functions require authentication. In an I deal world, the auth functions could be implemented and deployed in /auth as functions. For this version 0.1, it is planned to support only a shared secret that is present in an environment variable of the host. 

Environment variable: ShortedFaaS_SharedSecret 

## When can I see some code? 

When I’ve written it. ShortedFaaS is a late night sketch. 

## Something Something Containers / Docker / Kubernetes

Yes, that's how a FaaS should work ideally. Isolated functions. 
