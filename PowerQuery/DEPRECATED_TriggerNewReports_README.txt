================================================================================
WHY THIS QUERY IS DEPRECATED
================================================================================

The TriggerNewReports Power Query was designed to trigger Canvas API reports
directly from Power BI. However, this causes authentication problems in
Power BI Service because:

1. Power BI Service cannot handle Bearer token authentication for Web sources
2. Canvas API requires token-based authentication that doesn't map to
   Power BI's credential options (Anonymous, Basic, OAuth2, etc.)
3. This creates "invalid credentials" errors during scheduled refresh

SOLUTION:
---------
Use the PowerShell script (Fetch-CanvasData.ps1) instead. It:
- Properly authenticates with Canvas API using Bearer token
- Fetches data from all three enrollment terms
- Combines and saves data to local CSV files
- Can be scheduled via Windows Task Scheduler

The Power BI dashboard should ONLY read from the local CSV files:
- C:\Users\ruking\CanvasDataFetch\Data\LastUserAccess.csv
- C:\Users\ruking\CanvasDataFetch\Data\UserCourseAccessLog.csv

This way, Power BI Service only needs to access local files via the
On-Premises Data Gateway, which works reliably.

================================================================================
RECOMMENDED WORKFLOW
================================================================================

1. Run Fetch-CanvasData.ps1 (manually or via scheduled task)
2. Open Power BI Desktop and refresh the dashboard
3. Publish to Power BI Service
4. Power BI Service refreshes via gateway (local files only)

================================================================================
