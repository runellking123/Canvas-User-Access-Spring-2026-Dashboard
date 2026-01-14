# Canvas Data Fetch Script
# This script fetches Canvas LMS reports and saves them as CSV files
# Schedule this to run before your Power BI refresh

param(
    [string]$OutputFolder = "C:\Users\ruking\CanvasDataFetch\Data"
)

# Configuration - Same as your Power BI CanvasConfig
$Config = @{
    BaseUrl = "https://wileyc.instructure.com"
    ApiToken = "YOUR_CANVAS_API_TOKEN"  # Replace with your Canvas API token
    AccountId = "1"
}

# Multiple terms to fetch - Traditional, Graduate I, and Adult Degree Completion Program A
$Terms = @(
    @{ Id = "341"; Name = "2025-2026 Spring Traditional" },
    @{ Id = "347"; Name = "2025-2026 Spring Graduate I" },
    @{ Id = "345"; Name = "2025-2026 Spring Adult Degree Completion Program A" }
)

# Create output folder if it doesn't exist
if (-not (Test-Path $OutputFolder)) {
    New-Item -ItemType Directory -Path $OutputFolder -Force | Out-Null
}

# Set up headers
$Headers = @{
    "Authorization" = "Bearer $($Config.ApiToken)"
    "Content-Type" = "application/json"
}

# Log file
$LogFile = Join-Path $OutputFolder "fetch_log.txt"
$Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
Add-Content -Path $LogFile -Value "`n=== Fetch started at $Timestamp ==="

function Write-Log {
    param([string]$Message)
    $LogTimestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $LogMessage = "[$LogTimestamp] $Message"
    Write-Host $LogMessage
    Add-Content -Path $LogFile -Value $LogMessage
}

function Wait-ForReport {
    param(
        [string]$ReportUrl,
        [int]$MaxWaitSeconds = 600
    )

    $StartTime = Get-Date
    $Status = "running"

    while ($Status -eq "running" -or $Status -eq "created" -or $Status -eq "compiling") {
        Start-Sleep -Seconds 5

        $Elapsed = ((Get-Date) - $StartTime).TotalSeconds
        if ($Elapsed -gt $MaxWaitSeconds) {
            Write-Log "WARNING: Report generation timed out after $MaxWaitSeconds seconds"
            return $null
        }

        try {
            $ReportStatus = Invoke-RestMethod -Uri $ReportUrl -Headers $Headers -Method Get
            $Status = $ReportStatus.status
            Write-Log "Report status: $Status (elapsed: $([math]::Round($Elapsed))s)"

            if ($Status -eq "complete" -and $ReportStatus.attachment) {
                return $ReportStatus.attachment.url
            }
        } catch {
            Write-Log "Error checking report status: $($_.Exception.Message)"
        }
    }

    return $null
}

try {
    # Initialize arrays to hold combined data
    $AllLastUserAccess = @()
    $AllUserCourseAccessLog = @()
    $TermIds = @()

    # Process each term
    foreach ($Term in $Terms) {
        Write-Log "============================================"
        Write-Log "Processing Term: $($Term.Name) (ID: $($Term.Id))"
        Write-Log "============================================"

        $TermIds += $Term.Id

        # ============================================
        # Fetch LastUserAccess Report for this term
        # ============================================
        Write-Log "Triggering fresh LastUserAccess report for $($Term.Name)..."

        $TriggerUrl = "$($Config.BaseUrl)/api/v1/accounts/$($Config.AccountId)/reports/last_user_access_csv"
        $TriggerBody = @{
            parameters = @{
                enrollment_term_id = $Term.Id
            }
        } | ConvertTo-Json

        $TriggerResult = Invoke-RestMethod -Uri $TriggerUrl -Headers $Headers -Method Post -Body $TriggerBody
        Write-Log "Triggered LastUserAccess report. ID: $($TriggerResult.id), Status: $($TriggerResult.status)"

        # Wait for report to complete
        $ReportStatusUrl = "$($Config.BaseUrl)/api/v1/accounts/$($Config.AccountId)/reports/last_user_access_csv/$($TriggerResult.id)"
        $FileUrl = Wait-ForReport -ReportUrl $ReportStatusUrl

        if ($FileUrl) {
            Write-Log "Downloading LastUserAccess from: $FileUrl"
            $TempPath = Join-Path $OutputFolder "LastUserAccess_$($Term.Id).csv"
            Invoke-WebRequest -Uri $FileUrl -Headers $Headers -OutFile $TempPath

            # Read and add term column
            $Data = Import-Csv -Path $TempPath
            $Data | ForEach-Object {
                $_ | Add-Member -NotePropertyName "TermId" -NotePropertyValue $Term.Id -Force
                $_ | Add-Member -NotePropertyName "TermName" -NotePropertyValue $Term.Name -Force
            }
            $AllLastUserAccess += $Data
            Write-Log "Added $($Data.Count) records from $($Term.Name)"
            Remove-Item -Path $TempPath -Force
        } else {
            Write-Log "WARNING: Could not get LastUserAccess report for $($Term.Name)"
        }

        # ============================================
        # Fetch UserCourseAccessLog Report for this term
        # ============================================
        Write-Log "Triggering fresh UserCourseAccessLog report for $($Term.Name)..."

        $StartDate = (Get-Date).AddDays(-30).ToString("yyyy-MM-ddT00:00:00.000Z")
        $TriggerUrl = "$($Config.BaseUrl)/api/v1/accounts/$($Config.AccountId)/reports/user_course_access_log_csv"
        $TriggerBody = @{
            parameters = @{
                enrollment_term_id = $Term.Id
                start_at = $StartDate
            }
        } | ConvertTo-Json

        $TriggerResult = Invoke-RestMethod -Uri $TriggerUrl -Headers $Headers -Method Post -Body $TriggerBody
        Write-Log "Triggered UserCourseAccessLog report. ID: $($TriggerResult.id), Status: $($TriggerResult.status)"

        # Wait for report to complete
        $ReportStatusUrl = "$($Config.BaseUrl)/api/v1/accounts/$($Config.AccountId)/reports/user_course_access_log_csv/$($TriggerResult.id)"
        $FileUrl = Wait-ForReport -ReportUrl $ReportStatusUrl

        if ($FileUrl) {
            Write-Log "Downloading UserCourseAccessLog from: $FileUrl"
            $TempPath = Join-Path $OutputFolder "UserCourseAccessLog_$($Term.Id).csv"
            Invoke-WebRequest -Uri $FileUrl -Headers $Headers -OutFile $TempPath

            # Read and add term column
            $Data = Import-Csv -Path $TempPath
            $Data | ForEach-Object {
                $_ | Add-Member -NotePropertyName "TermId" -NotePropertyValue $Term.Id -Force
                $_ | Add-Member -NotePropertyName "TermName" -NotePropertyValue $Term.Name -Force
            }
            $AllUserCourseAccessLog += $Data
            Write-Log "Added $($Data.Count) records from $($Term.Name)"
            Remove-Item -Path $TempPath -Force
        } else {
            Write-Log "WARNING: Could not get UserCourseAccessLog report for $($Term.Name)"
        }
    }

    # ============================================
    # Save combined data to CSV files
    # ============================================
    Write-Log "============================================"
    Write-Log "Saving combined data files..."

    $LastUserAccessPath = Join-Path $OutputFolder "LastUserAccess.csv"
    $AllLastUserAccess | Export-Csv -Path $LastUserAccessPath -NoTypeInformation -Force
    Write-Log "LastUserAccess saved: $($AllLastUserAccess.Count) total records to $LastUserAccessPath"

    $UserCourseAccessPath = Join-Path $OutputFolder "UserCourseAccessLog.csv"
    $AllUserCourseAccessLog | Export-Csv -Path $UserCourseAccessPath -NoTypeInformation -Force
    Write-Log "UserCourseAccessLog saved: $($AllUserCourseAccessLog.Count) total records to $UserCourseAccessPath"

    # ============================================
    # Create metadata file with last update time
    # ============================================
    $MetadataPath = Join-Path $OutputFolder "metadata.json"
    $Metadata = @{
        LastUpdated = (Get-Date -Format "yyyy-MM-ddTHH:mm:ss")
        TermIds = $TermIds
        TermNames = ($Terms | ForEach-Object { $_.Name })
        AccountId = $Config.AccountId
        TotalLastUserAccessRecords = $AllLastUserAccess.Count
        TotalUserCourseAccessLogRecords = $AllUserCourseAccessLog.Count
    } | ConvertTo-Json

    Set-Content -Path $MetadataPath -Value $Metadata
    Write-Log "Metadata saved to: $MetadataPath"

    Write-Log "=== Fetch completed successfully ==="

} catch {
    Write-Log "ERROR: $($_.Exception.Message)"
    Write-Log "Stack Trace: $($_.ScriptStackTrace)"
    throw
}
