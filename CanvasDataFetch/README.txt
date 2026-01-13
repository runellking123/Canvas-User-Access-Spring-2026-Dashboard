CANVAS DATA FETCH - POWER BI DYNAMIC DATA SOURCE FIX
=====================================================

This solution fixes the Power BI "dynamic data source" error that prevents
scheduled refresh of the Canvas User Access Dashboard.

WHAT WAS CREATED:
-----------------
1. Fetch-CanvasData.ps1 - PowerShell script that fetches Canvas reports
2. UpdatePowerBIQueries.csx - Tabular Editor script to update Power BI queries
3. CreateScheduledTask.ps1 - Script to create Windows Scheduled Task
4. Data folder - Contains the CSV files for Power BI

FILES LOCATION:
---------------
C:\Users\ruking\CanvasDataFetch\
├── Fetch-CanvasData.ps1      (main data fetch script)
├── UpdatePowerBIQueries.csx  (Tabular Editor script)
├── CreateScheduledTask.ps1   (scheduled task setup)
├── README.txt                (this file)
└── Data\
    ├── LastUserAccess.csv         (Canvas report data)
    ├── UserCourseAccessLog.csv    (Canvas report data)
    ├── metadata.json              (last update timestamp)
    └── fetch_log.txt              (execution log)

STEP-BY-STEP INSTRUCTIONS TO COMPLETE SETUP:
--------------------------------------------

STEP 1: Apply the Power BI Query Changes (ONE-TIME)

   a. Open Tabular Editor 3

   b. Connect to your Power BI dataset:
      - File > Open > From Database
      - Server: powerbi://api.powerbi.com/v1.0/myorg/[YOUR_WORKSPACE_NAME]
      - Database: Canvas User Access Spring 2026 Dashboard
      (or use the XMLA endpoint you used before)

   c. Load the C# script:
      - File > Open Script (or Ctrl+Shift+O)
      - Navigate to: C:\Users\ruking\CanvasDataFetch\UpdatePowerBIQueries.csx
      - Click Open

   d. Run the script:
      - Press F5 (or click the Run button)
      - Check the output panel for success messages

   e. Save the changes:
      - Press Ctrl+S (or File > Save)
      - Wait for confirmation that changes were saved

STEP 2: Configure Power BI Gateway (ONE-TIME)

   a. Open Power BI Service (app.powerbi.com)

   b. Go to Settings > Manage Gateways

   c. Add a new data source connection:
      - Data Source Type: Folder
      - Path: C:\Users\ruking\CanvasDataFetch\Data

   d. Go to your dataset settings:
      - Settings > Datasets > Canvas User Access Spring 2026 Dashboard
      - Under "Gateway connection", select your gateway
      - Map the data source to the folder path

STEP 3: Verify Scheduled Task (ALREADY DONE)

   The scheduled task "CanvasDataFetch" has been created and will run
   daily at 5:00 AM. To verify or modify:

   a. Open Task Scheduler (search "Task Scheduler" in Windows)
   b. Look for "CanvasDataFetch" in the task list
   c. Right-click to Run, modify, or view history

STEP 4: Test the Refresh

   a. Manually run the fetch script first:
      - Open PowerShell
      - Run: powershell -ExecutionPolicy Bypass -File "C:\Users\ruking\CanvasDataFetch\Fetch-CanvasData.ps1"

   b. In Power BI Service, trigger a manual refresh

   c. Verify the refresh completes without the dynamic data source error

SCHEDULE RECOMMENDATIONS:
-------------------------
- Data Fetch Script: Daily at 5:00 AM (already configured)
- Power BI Refresh: Daily at 6:00 AM (configure in Power BI Service)

This ensures fresh Canvas data is downloaded before Power BI refreshes.

TROUBLESHOOTING:
----------------
1. If refresh still fails:
   - Verify the gateway is running on your machine
   - Check that the CSV files exist in the Data folder
   - Verify the gateway data source path matches exactly

2. If Canvas data is stale:
   - Check fetch_log.txt for errors
   - Manually run the fetch script to see error messages
   - Verify the Canvas API token is still valid

3. If Tabular Editor script fails:
   - Ensure you're connected to the correct dataset
   - Check that table names match (LastUserAccess, UserCourseAccessLog)
   - Try running commands individually if needed

MANUAL EXECUTION:
-----------------
To manually fetch new Canvas data at any time:
   powershell -ExecutionPolicy Bypass -File "C:\Users\ruking\CanvasDataFetch\Fetch-CanvasData.ps1"

=====================================================
Solution created: January 13, 2026
