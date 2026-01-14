# Create Scheduled Task for Canvas Data Fetch
# Run this script once as Administrator to set up the scheduled task

$taskName = "Canvas Data Fetch"

# Remove existing task if it exists
Unregister-ScheduledTask -TaskName $taskName -Confirm:$false -ErrorAction SilentlyContinue

# Define the action
$action = New-ScheduledTaskAction -Execute "powershell.exe" -Argument '-ExecutionPolicy Bypass -WindowStyle Hidden -File "C:\Users\ruking\CanvasDataFetch\Fetch-CanvasData.ps1"'

# Define triggers for 6:00 AM and 3:00 PM daily
$trigger1 = New-ScheduledTaskTrigger -Daily -At "6:00AM"
$trigger2 = New-ScheduledTaskTrigger -Daily -At "3:00PM"

# Define settings
$settings = New-ScheduledTaskSettingsSet -StartWhenAvailable -DontStopOnIdleEnd -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries

# Register the task with both triggers
Register-ScheduledTask -TaskName $taskName -Action $action -Trigger $trigger1, $trigger2 -Settings $settings -Description "Fetches Canvas LMS data for Power BI dashboard (runs at 6AM and 3PM daily)" -RunLevel Highest -Force

Write-Host "Scheduled task '$taskName' created successfully!"
Write-Host "Triggers: 6:00 AM and 3:00 PM daily"
