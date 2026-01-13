# Find gateway installation and services
Write-Host "=== Gateway Services ==="
Get-Service | Where-Object { $_.DisplayName -match 'gateway' -or $_.Name -match 'PBI' } | Format-Table Name, DisplayName, Status

Write-Host "`n=== Program Files Directories ==="
Get-ChildItem "C:\Program Files" -Directory | Where-Object { $_.Name -match 'gateway' -or $_.Name -match 'Power BI' } | ForEach-Object { Write-Host $_.FullName }

Write-Host "`n=== Checking specific paths ==="
$paths = @(
    "C:\Program Files\On-premises data gateway\EnterpriseGatewayConfigurator.exe",
    "C:\Program Files\On-premises data gateway\Microsoft.PowerBI.EnterpriseGateway.exe"
)
foreach ($p in $paths) {
    if (Test-Path $p) {
        Write-Host "FOUND: $p"
    }
}

Write-Host "`n=== All PBI Services ==="
Get-Service | Where-Object { $_.Name -like '*PBI*' } | Format-Table Name, DisplayName, Status -AutoSize
