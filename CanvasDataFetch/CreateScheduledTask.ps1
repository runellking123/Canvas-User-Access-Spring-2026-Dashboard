# Create Scheduled Task for Canvas Data Fetch
# Run this script once as Administrator to set up the scheduled task

$TaskAction = New-ScheduledTaskAction -Execute 'powershell.exe' -Argument '-ExecutionPolicy Bypass -WindowStyle Hidden -File "C:\Users\ruking\CanvasDataFetch\Fetch-CanvasData.ps1"'

$TaskTrigger = New-ScheduledTaskTrigger -Daily -At '5:00AM'

$TaskSettings = New-ScheduledTaskSettingsSet -StartWhenAvailable -DontStopIfGoingOnBatteries -AllowStartIfOnBatteries

Register-ScheduledTask -TaskName 'CanvasDataFetch' -Action $TaskAction -Trigger $TaskTrigger -Settings $TaskSettings -Description 'Fetches Canvas LMS data for Power BI dashboard' -Force

Write-Host "Scheduled task 'CanvasDataFetch' created successfully!"
Write-Host "The task will run daily at 5:00 AM."
Write-Host "You can modify the schedule in Task Scheduler if needed."
