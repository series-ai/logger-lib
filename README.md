# Padoru Logger Lib

## Overview
Standalone lib for logging, packaged as a DLL for use in various other projects at series.


## Build Instructions

1. Run a 'Release' config from the logger-lib project in your IDE
2. Replace the older logger-lib DLL in the target project (eg core-lib)


## How to update .net versions

Ocasionally the versions of Unity and .net may change over time, causing mismatch like below:
![image](https://github.com/user-attachments/assets/8cdf2554-c236-4414-883d-fc777bc1c9b5)

In this case locate the latest Unity DLL locally on your machine:
1. On Mac you do this by locating the Unity folder in applications in finder, navigate to the application and right click 'show package contents':
<img width="1219" alt="unity-app-location" src="https://github.com/user-attachments/assets/41291559-d3db-45da-9fc6-d1620f252e4b">

3. Navigate to the dll of your current editor and copy the UnityEngine.dll
<img width="736" alt="unity-dll-location" src="https://github.com/user-attachments/assets/9169ea27-08e2-4099-a8a4-943384f632b8">

4. Paste the file to replace the dll used by logger-lib [PadoruLogger/Unity/UnityEngine.dll](https://github.com/series-ai/logger-lib/blob/jpersons/RHO-4290_fix-dll-versions/PadoruLogger/Unity/UnityEngine.dll) file

5. Get the .net version from the Unity editor

6. Change the target .net version
<img width="848" alt="target-dotnet-version" src="https://github.com/user-attachments/assets/37ca30ae-74b2-4835-82e4-1a2ada0e6b26">

7. Push your changes and make sure no build errors/warnings for .net when generating the logger-lib dll


Here's an example PR: https://github.com/series-ai/logger-lib/pull/4/files#diff-f5557ccc861749f22de1bbfe64b7c77fd5be7518644fba92cee111f959843690R4

