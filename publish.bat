dotnet publish src -c release -o published 
xcopy src\deploy\ published\deploy /E/Y
set ShortedFaaS_Deploy=C:\Repos\ShortedFaaS\published\deploy\
.\published\ShortedFaas.exe