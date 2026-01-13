# Canvas Data Fetch Script
# This script fetches Canvas LMS reports and saves them as CSV files
# Schedule this to run before your Power BI refresh

param(
    [string]$OutputFolder = "C:\Users\ruking\CanvasDataFetch\Data"
)

# Configuration - Same as your Power BI CanvasConfig
$Config = @{
    BaseUrl = "https://wileyc.instructure.com"
    ApiToken = "4289~99fhGuex8n9Vt9BAEnWaPQAQwYJYfwUr8Ly7aQKyMUNG22Q76TuUx9RFykQMrLYA"
    AccountId = "1"
    TermId = "341"
}

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
        [int]$MaxWaitSeconds = 300
    )

    $StartTime = Get-Date
    $Status = "running"

    while ($Status -eq "running" -or $Status -eq "created") {
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
    # ============================================
    # Fetch LastUserAccess Report - ALWAYS TRIGGER FRESH
    # ============================================
    Write-Log "Triggering fresh LastUserAccess report..."

    $TriggerUrl = "$($Config.BaseUrl)/api/v1/accounts/$($Config.AccountId)/reports/last_user_access_csv"
    $TriggerBody = @{
        parameters = @{
            enrollment_term_id = $Config.TermId
        }
    } | ConvertTo-Json

    $TriggerResult = Invoke-RestMethod -Uri $TriggerUrl -Headers $Headers -Method Post -Body $TriggerBody
    Write-Log "Triggered LastUserAccess report. ID: $($TriggerResult.id), Status: $($TriggerResult.status)"

    # Wait for report to complete
    $ReportStatusUrl = "$($Config.BaseUrl)/api/v1/accounts/$($Config.AccountId)/reports/last_user_access_csv/$($TriggerResult.id)"
    $FileUrl = Wait-ForReport -ReportUrl $ReportStatusUrl

    if ($FileUrl) {
        Write-Log "Downloading LastUserAccess from: $FileUrl"
        $LastUserAccessPath = Join-Path $OutputFolder "LastUserAccess.csv"
        Invoke-WebRequest -Uri $FileUrl -Headers $Headers -OutFile $LastUserAccessPath
        Write-Log "LastUserAccess saved to: $LastUserAccessPath"
    } else {
        Write-Log "ERROR: Could not get LastUserAccess report"
    }

    # ============================================
    # Fetch UserCourseAccessLog Report - ALWAYS TRIGGER FRESH
    # ============================================
    Write-Log "Triggering fresh UserCourseAccessLog report..."

    $StartDate = (Get-Date).AddDays(-30).ToString("yyyy-MM-ddT00:00:00.000Z")
    $TriggerUrl = "$($Config.BaseUrl)/api/v1/accounts/$($Config.AccountId)/reports/user_course_access_log_csv"
    $TriggerBody = @{
        parameters = @{
            enrollment_term_id = $Config.TermId
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
        $UserCourseAccessPath = Join-Path $OutputFolder "UserCourseAccessLog.csv"
        Invoke-WebRequest -Uri $FileUrl -Headers $Headers -OutFile $UserCourseAccessPath
        Write-Log "UserCourseAccessLog saved to: $UserCourseAccessPath"
    } else {
        Write-Log "ERROR: Could not get UserCourseAccessLog report"
    }

    # ============================================
    # Create metadata file with last update time
    # ============================================
    $MetadataPath = Join-Path $OutputFolder "metadata.json"
    $Metadata = @{
        LastUpdated = (Get-Date -Format "yyyy-MM-ddTHH:mm:ss")
        TermId = $Config.TermId
        AccountId = $Config.AccountId
    } | ConvertTo-Json

    Set-Content -Path $MetadataPath -Value $Metadata
    Write-Log "Metadata saved to: $MetadataPath"

    Write-Log "=== Fetch completed successfully ==="

} catch {
    Write-Log "ERROR: $($_.Exception.Message)"
    Write-Log "Stack Trace: $($_.ScriptStackTrace)"
    throw
}
