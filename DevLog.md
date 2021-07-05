# Development log

## 2021.7.4 

Initial commit of ShortedFaaS, managed to get jq to consume the input from the stdin with CliWrap nuget and setup the basic structure. Direct out of process functions would work, but the command line is completely fixed.

## 2021.7.5

The FunctionsRegister has been added and functions can be registed in code at the beginning of the program. This could be enough for some use cases. The calling code of the functions has now been made generic. As proof that ShortedFaaS can run multiple functions, Python and JavaScript (node) functions have been added.

This means that the **Full code out of Process** hosting is complete. 

Out of process / Native is done with JQ as an example and Out of Process / Launched / Interpreted is done with Python and Javascript (node). These must be installed and running on your local machine for ShortedFaaS to work. Currently I have a Node v10.16.3 and Python 3.7.8 installed. 

The next step for this is automatically loading new functions are they are added to the deploy folder, I plan to scan at startup and have a filesystem watcher. One point of the design that I'm questioning is if the execution information of the function (command line, arguments, environment) should be inside the function's folder or should it be a level higher in the deploy folder? 

- deploy
    - jq.json    <- ShortedFaaS declaration 
    - jq
        - jq.exe 

The advantage of having the declaration out of the folder is that the working folder is totally "clean" with only the deployable code, nothing extra. But it's more work for the filesystem watcher and it's not as clean to deploy since you have to deploy 2 elements at once. 

- deploy
    - jq
        - jq.exe 
        - run.json    <- ShortedFaaS declaration 

This was my initial concept and I'll probably stick with it.

Next steps: 
    - add run.json support
    - add file system watcher for dynamic addition and removal of functions
    - add in-process function with dynamic loads 
        - iron-python 
        - octave.net 


## 2021.7.6

Added the environment variable for the Deploy folder "ShortedFaaS_Deploy". 
Added out of process Rust and out of process Wasm from the Rust.