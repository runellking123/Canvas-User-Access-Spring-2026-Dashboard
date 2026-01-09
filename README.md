# Canvas User Access Spring 2026 Dashboard

A Power BI dashboard for tracking student engagement and activity in Canvas LMS for Wiley University.

## Overview

This dashboard connects to the Canvas LMS API to pull user access data and course activity logs, providing insights into:
- Student enrollment and login activity
- Content engagement (views, participations)
- At-risk student identification
- Course-level analytics

## Prerequisites

- **Power BI Desktop** (latest version)
- **Tabular Editor 3** (for running C# scripts)
- **Canvas LMS API Token** (with appropriate permissions)
- **Canvas Reports** enabled for:
  - Last User Access
  - User Course Access Log

## Project Structure

```
├── PowerQuery/              # Power Query M code for data connections
│   ├── 1_CanvasConfig.txt
│   ├── 2_LastUserAccess.txt
│   ├── 3_UserCourseAccessLog.txt
│   └── 4_TriggerNewReports.txt
├── TabularEditor/           # Tabular Editor 3 C# scripts
│   ├── TE3_Capitalize_Columns.csx
│   ├── TE3_Create_Relationships.csx
│   ├── TE3_Fixed_All_Measures.csx
│   ├── TE3_Add_Measure_Descriptions.csx
│   ├── TE3_Add_Table_Column_Descriptions.csx
│   └── TE3_Add_Copilot_Instructions.csx
├── CopilotAI/               # Power BI Copilot instructions and prompts
│   ├── Canvas_PrepDataForAI_Instructions.txt
│   ├── Canvas_Copilot_Dashboard_Prompts.txt
│   ├── Canvas_QA_Questions_By_Folder.txt
│   └── Canvas_UserAccess_Copilot_Instructions.txt
├── Theme/                   # Power BI theme file
│   └── WileyUniversity_PowerBI_Theme.json
└── SampleExports/           # Sample data exports
    ├── Active_Students_January_2026.csv
    └── Inactive_Students_Spring2026.csv
```

## Setup Instructions

### Step 1: Configure Canvas API Connection

1. Open Power BI Desktop
2. Go to **Home** > **Transform data** > **Advanced Editor**
3. Create a new blank query and paste the contents of `PowerQuery/1_CanvasConfig.txt`
4. Replace `YOUR_CANVAS_DOMAIN` with your Canvas instance URL (e.g., `wileyc.instructure.com`)
5. Replace `YOUR_API_TOKEN` with your Canvas API token

### Step 2: Add Data Queries

Create the following queries by pasting the Power Query code:

| Order | Query Name | File |
|-------|------------|------|
| 1 | CanvasConfig | `PowerQuery/1_CanvasConfig.txt` |
| 2 | LastUserAccess | `PowerQuery/2_LastUserAccess.txt` |
| 3 | UserCourseAccessLog | `PowerQuery/3_UserCourseAccessLog.txt` |
| 4 | TriggerNewReports | `PowerQuery/4_TriggerNewReports.txt` |

### Step 3: Apply Tabular Editor Scripts

Run these scripts in order using Tabular Editor 3:

| Order | Script | Purpose |
|-------|--------|---------|
| 1 | `TE3_Capitalize_Columns.csx` | Capitalize column names |
| 2 | `TE3_Create_Relationships.csx` | Create table relationships |
| 3 | `TE3_Fixed_All_Measures.csx` | Create all 31 DAX measures |
| 4 | `TE3_Add_Measure_Descriptions.csx` | Add descriptions for Copilot |
| 5 | `TE3_Add_Table_Column_Descriptions.csx` | Add table/column descriptions |
| 6 | `TE3_Add_Copilot_Instructions.csx` | Add model-level Copilot instructions |

**To run a script in Tabular Editor 3:**
1. Open your Power BI model in Tabular Editor 3
2. Go to **C# Script** > **New from file**
3. Select the script file
4. Press **F5** to run
5. Save the model (Ctrl+S)

### Step 4: Apply Theme

1. In Power BI Desktop, go to **View** > **Themes** > **Browse for themes**
2. Select `Theme/WileyUniversity_PowerBI_Theme.json`

### Step 5: Configure Copilot AI

1. Go to **File** > **Options** > **Preview features** > Enable **Copilot**
2. Go to **Prep data for Copilot AI**
3. Paste the contents of `CopilotAI/Canvas_PrepDataForAI_Instructions.txt`

## Measures Reference

### Enrollment (5 measures)
| Measure | Description |
|---------|-------------|
| Student Count | Total unique students |
| Instructor Count | Total unique instructors |
| TA Count | Total teaching assistants |
| Observer Count | Total observers |
| Designer Count | Total course designers |

### Student Activity (8 measures)
| Measure | Description |
|---------|-------------|
| Active Students 7 Days | Students logged in within 7 days |
| Active Students 30 Days | Students logged in within 30 days |
| Inactive Students 30 Days | Students not logged in for 30+ days |
| Never Accessed Students | Students who never logged in |
| Pct Active 7 Days | % students active in 7 days |
| Pct Active 30 Days | % students active in 30 days |
| Pct Inactive 30 Days | % students inactive 30+ days |
| Pct Never Accessed | % students never logged in |

### Course Metrics (2 measures)
| Measure | Description |
|---------|-------------|
| Total Courses | Total unique courses |
| Total Sections | Total unique sections |

### Engagement (11 measures)
| Measure | Description |
|---------|-------------|
| Total Views | Total content views by students |
| Total Participations | Total participations by students |
| Avg Views Per Student | Average views per student |
| Avg Participations Per Student | Average participations per student |
| Assignment Views | Views of assignments |
| Discussion Views | Views of discussions |
| Quiz Views | Views of quizzes |
| Module Views | Views of modules |
| Page Views | Views of pages |
| File Views | Views of files |
| Announcement Views | Views of announcements |

### Engagement Levels (8 measures)
| Measure | Description | Threshold |
|---------|-------------|-----------|
| High Engagement Students | Highly active students | 50+ views |
| Medium Engagement Students | Moderately active students | 10-49 views |
| Low Engagement Students | Minimally active students | 1-9 views |
| Zero Engagement Students | At-risk students | 0 views |
| Pct High Engagement | % highly engaged | |
| Pct Medium Engagement | % moderately engaged | |
| Pct Low Engagement | % low engagement | |
| Pct Zero Engagement | % at risk | |

## Copilot Prompts

Use the prompts in `CopilotAI/Canvas_Copilot_Dashboard_Prompts.txt` to quickly build dashboard pages:

**Example:**
```
Create a report page called Executive Summary with these visuals:
Four card visuals in a row: Student Count, Active Students 30 Days, Total Courses, Instructor Count from _Measures table
A donut chart showing Pct High Engagement, Pct Medium Engagement, Pct Low Engagement, Pct Zero Engagement from _Measures table
A clustered bar chart showing Assignment Views, Quiz Views, Discussion Views, Module Views from _Measures table sorted descending
```

## Q&A Questions

Use natural language questions with Power BI Q&A:

```
what is the student count
how many active students 30 days
show engagement percentages as donut chart
show content views as bar chart
top courses by total views
```

## Theme Colors (Wiley University Brand)

| Color | Hex | Usage |
|-------|-----|-------|
| Wildcat Purple | #3D2C68 | Primary |
| Wiley Purple | #65538F | Secondary |
| Carbon | #414042 | Text |
| Gray | #595959 | Labels |
| Silver | #B1B6C1 | Accents |
| Light Stone | #E2E2E2 | Backgrounds |

**Font:** Open Sans

## Data Model

```
┌─────────────────────┐
│   LastUserAccess    │
│ (One row per user)  │
│                     │
│ • user id (PK)      │
│ • user name         │
│ • last access at    │
└─────────┬───────────┘
          │
          │ user id
          │
┌─────────▼───────────┐
│ UserCourseAccessLog │
│ (Activity details)  │
│                     │
│ • user id (FK)      │
│ • course name       │
│ • enrollment type   │
│ • content type      │
│ • times viewed      │
│ • times participated│
└─────────────────────┘
```

## Important Notes

1. **All columns are imported as TEXT** - The DAX measures handle text-to-number and text-to-date conversions automatically.

2. **Activity measures are STUDENT-ONLY** - All activity measures filter to `enrollment type = "student"` to avoid inflating numbers with instructor activity.

3. **Date format from Canvas API** - Dates come in ISO 8601 format (`2026-01-08T13:49:06-06:00`). Measures parse this format automatically.

4. **Hidden tables** - `CanvasConfig` and `TriggerNewReports` are configuration tables and should be hidden from report view.

## Troubleshooting

### Error: "Cannot convert value '' of type Text to type Date"
The date column contains empty values. The measures use `IFERROR` to handle this, but ensure you're using `TE3_Fixed_All_Measures.csx`.

### Error: "The function SUM cannot work with values of type String"
The `times viewed` and `times participated` columns are text. Use `SUMX` with `VALUE()` conversion as shown in the fixed measures script.

### Measure shows wrong count
Ensure the measure filters to `enrollment type = "student"`. The `Active Students` measures should never exceed `Student Count`.

## License

Internal use only - Wiley University

## Author

Created with Claude Code assistance for Wiley University Institutional Research.
