# Canvas User Access Spring 2026 Dashboard

A Power BI dashboard for tracking student engagement and activity in Canvas LMS for Wiley University. Supports multiple enrollment terms including Traditional, Graduate, and Adult Degree Completion students.

## Overview

This dashboard pulls Canvas LMS user access data and course activity logs, providing insights into:
- Student enrollment and login activity across all terms
- Content engagement (views, participations)
- At-risk student identification
- Course-level analytics
- Term-based filtering (Traditional, Graduate, Adult Degree Completion)

## Project Structure

```
├── CanvasDataFetch/
│   ├── Fetch-CanvasData.ps1       # Main script - fetches data from Canvas API
│   └── CreateScheduledTask.ps1    # Creates Windows scheduled tasks
├── PowerQuery/
│   ├── 1_CanvasConfig.txt         # Configuration (reference only)
│   ├── 2_LastUserAccess.txt       # Power Query for LastUserAccess table
│   └── 3_UserCourseAccessLog.txt  # Power Query for UserCourseAccessLog table
└── Canvas User Access Spring 2026 Dashboard.pbix  # Power BI Dashboard file
```

## Important: Data Flow Architecture

This dashboard uses a **local-file-only** approach for Power BI Service compatibility:

```
Canvas API → PowerShell Script → Local CSV Files → Power BI Gateway → Power BI Service
```

**Why this approach?**
- Power BI Service cannot authenticate directly to Canvas API (Bearer token not supported)
- Using local files allows the On-Premises Gateway to handle data access
- The PowerShell script handles all Canvas API authentication

## Data Sources

The dashboard fetches data from three Canvas enrollment terms:

| Term ID | Term Name | Description |
|---------|-----------|-------------|
| 341 | 2025-2026 Spring Traditional | Undergraduate students |
| 347 | 2025-2026 Spring Graduate I | Graduate students |
| 345 | 2025-2026 Spring Adult Degree Completion Program A | Adult degree students |

## Output Files

The PowerShell script generates these CSV files in `C:\Users\ruking\CanvasDataFetch\Data\`:

| File | Description | Records |
|------|-------------|---------|
| LastUserAccess.csv | Student enrollment and last login data | ~1,064 |
| UserCourseAccessLog.csv | Detailed course activity (last 30 days) | ~91,568 |
| metadata.json | Fetch metadata and record counts | - |

## Setup Instructions

### Prerequisites

- Windows 10/11
- PowerShell 5.1+
- Power BI Desktop
- Power BI Personal Gateway (for scheduled refresh)
- Canvas LMS API Token with report access

### Step 1: Configure the PowerShell Script

Edit `CanvasDataFetch/Fetch-CanvasData.ps1` and update if needed:

```powershell
$Config = @{
    BaseUrl = "https://wileyc.instructure.com"
    ApiToken = "YOUR_API_TOKEN"
    AccountId = "1"
}
```

### Step 2: Run Initial Data Fetch

```powershell
powershell -ExecutionPolicy Bypass -File "C:\Users\ruking\CanvasDataFetch\Fetch-CanvasData.ps1"
```

This will:
1. Trigger fresh reports for each term in Canvas
2. Wait for reports to complete (can take 5-10 minutes)
3. Download and combine data from all terms
4. Add TermId and TermName columns to each record
5. Save combined CSV files

### Step 3: Set Up Scheduled Tasks

The scheduled tasks run at **6:00 AM** and **3:00 PM** daily:

```powershell
# Creates two scheduled tasks
schtasks /Create /TN "Canvas Data Fetch" /TR "powershell.exe -ExecutionPolicy Bypass -WindowStyle Hidden -File \"C:\Users\ruking\CanvasDataFetch\Fetch-CanvasData.ps1\"" /SC DAILY /ST 06:00
schtasks /Create /TN "Canvas Data Fetch 3PM" /TR "powershell.exe -ExecutionPolicy Bypass -WindowStyle Hidden -File \"C:\Users\ruking\CanvasDataFetch\Fetch-CanvasData.ps1\"" /SC DAILY /ST 15:00
```

### Step 4: Configure Power BI

The Power Query in Power BI reads from local CSV files:

**LastUserAccess Query:**
```
let
    FilePath = "C:\Users\ruking\CanvasDataFetch\Data\LastUserAccess.csv",
    CsvContent = File.Contents(FilePath),
    CsvData = Csv.Document(CsvContent, [Delimiter=",", Encoding=65001, QuoteStyle=QuoteStyle.Csv]),
    PromotedHeaders = Table.PromoteHeaders(CsvData, [PromoteAllScalars=true])
in
    PromotedHeaders
```

**UserCourseAccessLog Query:**
```
let
    FilePath = "C:\Users\ruking\CanvasDataFetch\Data\UserCourseAccessLog.csv",
    CsvContent = File.Contents(FilePath),
    CsvData = Csv.Document(CsvContent, [Delimiter=",", Encoding=65001, QuoteStyle=QuoteStyle.Csv]),
    PromotedHeaders = Table.PromoteHeaders(CsvData, [PromoteAllScalars=true])
in
    PromotedHeaders
```

### Step 5: Install Personal Gateway

1. Go to Power BI Service → Your Workspace → Dataset Settings
2. Under Gateway connection, click "Install gateway"
3. Download and install **Personal mode**
4. Sign in with your Power BI account

### Step 6: Configure Scheduled Refresh in Power BI Service

1. Go to Dataset Settings → Scheduled refresh
2. Enable scheduled refresh
3. Add refresh times: **6:30 AM** and **3:30 PM** (30 min after script runs)
4. Click Apply

## Automation Flow

```
6:00 AM  → PowerShell fetches fresh Canvas data → Saves to CSV
6:30 AM  → Power BI Service refreshes from CSV files

3:00 PM  → PowerShell fetches fresh Canvas data → Saves to CSV
3:30 PM  → Power BI Service refreshes from CSV files
```

## Dashboard Features

### Filtering by Term

Use the **TermName** slicer to filter data:
- 2025-2026 Spring Traditional (Undergrad)
- 2025-2026 Spring Graduate I
- 2025-2026 Spring Adult Degree Completion Program A

### Key Metrics

| Category | Metrics |
|----------|---------|
| Enrollment | Student Count, Instructor Count, TA Count |
| Activity | Active Students (7/30 days), Inactive Students, Never Accessed |
| Engagement | Total Views, Total Participations, Views by Content Type |
| Risk | High/Medium/Low/Zero Engagement percentages |

## Data Columns

### LastUserAccess.csv

| Column | Description |
|--------|-------------|
| user id | Canvas user ID |
| user name | Student name |
| last access at | Last login timestamp |
| TermId | Term identifier (341, 347, 345) |
| TermName | Term display name |

### UserCourseAccessLog.csv

| Column | Description |
|--------|-------------|
| user id | Canvas user ID |
| user name | Student name |
| course name | Course title |
| enrollment type | student, teacher, ta, etc. |
| asset category | content type (assignments, quizzes, etc.) |
| times viewed | View count |
| times participated | Participation count |
| TermId | Term identifier |
| TermName | Term display name |

## Troubleshooting

### Script times out on UserCourseAccessLog

The script waits up to 600 seconds (10 minutes) for large reports. If it still times out, increase `MaxWaitSeconds` in the `Wait-ForReport` function.

### Power BI refresh fails

1. Ensure your computer is on and connected at refresh time
2. Verify Personal Gateway is running (check system tray)
3. Check that CSV files exist in the Data folder
4. Verify file paths in Power Query match actual file locations

### "Invalid credentials for Web source" error

If you see an error like `Failed to update data source credentials: The credentials provided for the Web source are invalid`, your Power BI file contains queries that try to access Canvas API directly.

**Fix this by removing Web-based queries:**

1. Open the `.pbix` file in Power BI Desktop
2. Go to **Transform Data** (Power Query Editor)
3. In the Queries pane, look for any queries that reference:
   - `Web.Contents`
   - `https://wileyc.instructure.com`
   - `TriggerNewReports` or similar names
4. Delete these queries (right-click → Delete)
5. Keep ONLY queries that use `File.Contents` for local CSV files
6. Click **Close & Apply**
7. Re-publish to Power BI Service

The dashboard should only have these data sources:
- `C:\Users\ruking\CanvasDataFetch\Data\LastUserAccess.csv`
- `C:\Users\ruking\CanvasDataFetch\Data\UserCourseAccessLog.csv`

### Scheduled task not running

1. Open Task Scheduler and verify task status
2. Check task history for errors
3. Ensure "Run whether user is logged on or not" is configured if needed

## Manual Data Refresh

To manually fetch fresh data:

```powershell
cd C:\Users\ruking\CanvasDataFetch
powershell -ExecutionPolicy Bypass -File "Fetch-CanvasData.ps1"
```

Check the log file for status:
```powershell
Get-Content "C:\Users\ruking\CanvasDataFetch\Data\fetch_log.txt" -Tail 50
```

## License

Internal use only - Wiley University

## Author

Created for Wiley University Institutional Research
