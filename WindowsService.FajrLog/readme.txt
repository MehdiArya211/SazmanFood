1. open powershell as administrator
2. go to publish directory
3. enter bellow command to create service:
    sc.exe create "SERVICE_NAME" binpath="SERVICE_PATH\WindowsService.FajrLog.exe"
4.enter bellow command to start Service:
    sc.exe start "SERVICE_NAME"


5. for stop service:
    sc.exe stop "SERVICE_NAME"
6. for remove service:
    sc.exe delete "SERVICE_NAME"
