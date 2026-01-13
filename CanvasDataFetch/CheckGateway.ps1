# Check for gateway processes and configuration
Write-Host "=== Checking for Gateway Processes ==="
Get-Process | Where-Object { $_.ProcessName -like '*gateway*' -or $_.ProcessName -like '*PBIEgw*' } | Select-Object ProcessName, Id

Write-Host "`n=== Looking for Gateway Installation ==="
$gatewayPaths = @(
    "C:\Program Files\On-premises data gateway",
    "C:\Program Files (x86)\On-premises data gateway",
    "$env:LOCALAPPDATA\Microsoft\On-premises data gateway"
)

foreach ($path in $gatewayPaths) {
    if (Test-Path $path) {
        Write-Host "Found gateway at: $path"
        Get-ChildItem $path -Filter "*.exe" | Select-Object Name
    }
}

Write-Host "`n=== Gateway Configuration Files ==="
$configPath = "$env:LOCALAPPDATA\Microsoft\On-premises data gateway"
if (Test-Path $configPath) {
    Get-ChildItem $configPath -Filter "*.config" | Select-Object Name
}
