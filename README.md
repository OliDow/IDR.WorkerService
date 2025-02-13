**Project Contains** 

IntraDayReport Service for processing Trade Aggregates      

**To run**

 cd .\IDR.WorkerService   
 dotnet run

 **Deploy as Service**

 Can be deployed as a service manually by navigating to bin/Debug/net6.0
 and running New-Service -Name "IDRWorkerService" -BinaryPathName .\IDR.WorkerService.exe
 _Not this will require admin_

**_Warning_**

This project is using a legacy version of .Net which is out of support (net6)  
https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core
