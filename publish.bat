dotnet publish src -c release -o published 
xcopy src\deploy\ published\deploy /E/Y