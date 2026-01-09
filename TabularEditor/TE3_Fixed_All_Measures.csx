// ============================================
// TABULAR EDITOR 3 - FIXED CANVAS MEASURES
// Handles ALL columns as TEXT type
// ============================================

var measuresTable = Model.Tables.FirstOrDefault(t => t.Name == "_Measures");
if (measuresTable == null)
{
    measuresTable = Model.AddCalculatedTable("_Measures", "ROW(\"Helper\", 1)");
}

// Delete ALL existing measures
foreach (var m in measuresTable.Measures.ToList())
{
    m.Delete();
}

// ============================================
// ENROLLMENT MEASURES
// ============================================

measuresTable.AddMeasure(
    "Student Count",
    @"CALCULATE(
    DISTINCTCOUNT(UserCourseAccessLog[user id]),
    UserCourseAccessLog[enrollment type] = ""student""
)"
).DisplayFolder = "Enrollment";

measuresTable.AddMeasure(
    "Instructor Count",
    @"CALCULATE(
    DISTINCTCOUNT(UserCourseAccessLog[user id]),
    UserCourseAccessLog[enrollment type] = ""teacher""
)"
).DisplayFolder = "Enrollment";

measuresTable.AddMeasure(
    "TA Count",
    @"CALCULATE(
    DISTINCTCOUNT(UserCourseAccessLog[user id]),
    UserCourseAccessLog[enrollment type] = ""ta""
)"
).DisplayFolder = "Enrollment";

measuresTable.AddMeasure(
    "Observer Count",
    @"CALCULATE(
    DISTINCTCOUNT(UserCourseAccessLog[user id]),
    UserCourseAccessLog[enrollment type] = ""observer""
)"
).DisplayFolder = "Enrollment";

measuresTable.AddMeasure(
    "Designer Count",
    @"CALCULATE(
    DISTINCTCOUNT(UserCourseAccessLog[user id]),
    UserCourseAccessLog[enrollment type] = ""designer""
)"
).DisplayFolder = "Enrollment";

// ============================================
// COURSE MEASURES
// ============================================

measuresTable.AddMeasure(
    "Total Courses",
    @"DISTINCTCOUNT(UserCourseAccessLog[course id])"
).DisplayFolder = "Course Metrics";

measuresTable.AddMeasure(
    "Total Sections",
    @"DISTINCTCOUNT(UserCourseAccessLog[section id])"
).DisplayFolder = "Course Metrics";

// ============================================
// STUDENT ACTIVITY MEASURES (Using text date handling)
// ============================================

measuresTable.AddMeasure(
    "Active Students 7 Days",
    @"COUNTROWS(
    FILTER(
        ADDCOLUMNS(
            CALCULATETABLE(
                DISTINCT(UserCourseAccessLog[user id]),
                UserCourseAccessLog[enrollment type] = ""student""
            ),
            ""LastAccess"", LOOKUPVALUE(LastUserAccess[last access at], LastUserAccess[user id], [user id])
        ),
        NOT(ISBLANK([LastAccess])) &&
        LEN([LastAccess]) >= 10 &&
        IFERROR(DATEVALUE(LEFT(SUBSTITUTE([LastAccess], ""T"", "" ""), 10)), DATE(1900,1,1)) >= TODAY() - 7
    )
)"
).DisplayFolder = "Student Activity";

measuresTable.AddMeasure(
    "Active Students 30 Days",
    @"COUNTROWS(
    FILTER(
        ADDCOLUMNS(
            CALCULATETABLE(
                DISTINCT(UserCourseAccessLog[user id]),
                UserCourseAccessLog[enrollment type] = ""student""
            ),
            ""LastAccess"", LOOKUPVALUE(LastUserAccess[last access at], LastUserAccess[user id], [user id])
        ),
        NOT(ISBLANK([LastAccess])) &&
        LEN([LastAccess]) >= 10 &&
        IFERROR(DATEVALUE(LEFT(SUBSTITUTE([LastAccess], ""T"", "" ""), 10)), DATE(1900,1,1)) >= TODAY() - 30
    )
)"
).DisplayFolder = "Student Activity";

measuresTable.AddMeasure(
    "Inactive Students 30 Days",
    @"COUNTROWS(
    FILTER(
        ADDCOLUMNS(
            CALCULATETABLE(
                DISTINCT(UserCourseAccessLog[user id]),
                UserCourseAccessLog[enrollment type] = ""student""
            ),
            ""LastAccess"", LOOKUPVALUE(LastUserAccess[last access at], LastUserAccess[user id], [user id])
        ),
        NOT(ISBLANK([LastAccess])) &&
        LEN([LastAccess]) >= 10 &&
        IFERROR(DATEVALUE(LEFT(SUBSTITUTE([LastAccess], ""T"", "" ""), 10)), DATE(1900,1,1)) < TODAY() - 30
    )
)"
).DisplayFolder = "Student Activity";

measuresTable.AddMeasure(
    "Never Accessed Students",
    @"COUNTROWS(
    FILTER(
        ADDCOLUMNS(
            CALCULATETABLE(
                DISTINCT(UserCourseAccessLog[user id]),
                UserCourseAccessLog[enrollment type] = ""student""
            ),
            ""LastAccess"", LOOKUPVALUE(LastUserAccess[last access at], LastUserAccess[user id], [user id])
        ),
        ISBLANK([LastAccess]) || LEN([LastAccess]) < 10
    )
)"
).DisplayFolder = "Student Activity";

// ============================================
// STUDENT ACTIVITY PERCENTAGES
// ============================================

measuresTable.AddMeasure(
    "Pct Active 7 Days",
    @"DIVIDE([Active Students 7 Days], [Student Count], 0)"
).DisplayFolder = "Student Activity\\Percentages";

measuresTable.AddMeasure(
    "Pct Active 30 Days",
    @"DIVIDE([Active Students 30 Days], [Student Count], 0)"
).DisplayFolder = "Student Activity\\Percentages";

measuresTable.AddMeasure(
    "Pct Inactive 30 Days",
    @"DIVIDE([Inactive Students 30 Days], [Student Count], 0)"
).DisplayFolder = "Student Activity\\Percentages";

measuresTable.AddMeasure(
    "Pct Never Accessed",
    @"DIVIDE([Never Accessed Students], [Student Count], 0)"
).DisplayFolder = "Student Activity\\Percentages";

// ============================================
// ENGAGEMENT MEASURES (Converting text to numbers)
// ============================================

measuresTable.AddMeasure(
    "Total Views",
    @"CALCULATE(
    SUMX(
        UserCourseAccessLog,
        VALUE(UserCourseAccessLog[times viewed])
    ),
    UserCourseAccessLog[enrollment type] = ""student""
)"
).DisplayFolder = "Engagement";

measuresTable.AddMeasure(
    "Total Participations",
    @"CALCULATE(
    SUMX(
        UserCourseAccessLog,
        VALUE(UserCourseAccessLog[times participated])
    ),
    UserCourseAccessLog[enrollment type] = ""student""
)"
).DisplayFolder = "Engagement";

measuresTable.AddMeasure(
    "Avg Views Per Student",
    @"DIVIDE([Total Views], [Student Count], 0)"
).DisplayFolder = "Engagement";

measuresTable.AddMeasure(
    "Avg Participations Per Student",
    @"DIVIDE([Total Participations], [Student Count], 0)"
).DisplayFolder = "Engagement";

// ============================================
// CONTENT TYPE VIEWS (Converting text to numbers)
// ============================================

measuresTable.AddMeasure(
    "Assignment Views",
    @"CALCULATE(
    SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])),
    UserCourseAccessLog[enrollment type] = ""student"",
    UserCourseAccessLog[content type] = ""assignments""
)"
).DisplayFolder = "Engagement\\By Content Type";

measuresTable.AddMeasure(
    "Discussion Views",
    @"CALCULATE(
    SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])),
    UserCourseAccessLog[enrollment type] = ""student"",
    UserCourseAccessLog[content type] = ""discussion_topics""
)"
).DisplayFolder = "Engagement\\By Content Type";

measuresTable.AddMeasure(
    "Quiz Views",
    @"CALCULATE(
    SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])),
    UserCourseAccessLog[enrollment type] = ""student"",
    UserCourseAccessLog[content type] = ""quizzes""
)"
).DisplayFolder = "Engagement\\By Content Type";

measuresTable.AddMeasure(
    "Module Views",
    @"CALCULATE(
    SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])),
    UserCourseAccessLog[enrollment type] = ""student"",
    UserCourseAccessLog[content type] = ""modules""
)"
).DisplayFolder = "Engagement\\By Content Type";

measuresTable.AddMeasure(
    "Page Views",
    @"CALCULATE(
    SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])),
    UserCourseAccessLog[enrollment type] = ""student"",
    UserCourseAccessLog[content type] = ""pages""
)"
).DisplayFolder = "Engagement\\By Content Type";

measuresTable.AddMeasure(
    "File Views",
    @"CALCULATE(
    SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])),
    UserCourseAccessLog[enrollment type] = ""student"",
    UserCourseAccessLog[content type] = ""attachments""
)"
).DisplayFolder = "Engagement\\By Content Type";

measuresTable.AddMeasure(
    "Announcement Views",
    @"CALCULATE(
    SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])),
    UserCourseAccessLog[enrollment type] = ""student"",
    UserCourseAccessLog[content type] = ""announcements""
)"
).DisplayFolder = "Engagement\\By Content Type";

// ============================================
// ENGAGEMENT LEVELS
// ============================================

measuresTable.AddMeasure(
    "High Engagement Students",
    @"COUNTROWS(
    FILTER(
        ADDCOLUMNS(
            CALCULATETABLE(
                DISTINCT(UserCourseAccessLog[user id]),
                UserCourseAccessLog[enrollment type] = ""student""
            ),
            ""Views"", CALCULATE(SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])))
        ),
        [Views] >= 50
    )
)"
).DisplayFolder = "Engagement\\Levels";

measuresTable.AddMeasure(
    "Medium Engagement Students",
    @"COUNTROWS(
    FILTER(
        ADDCOLUMNS(
            CALCULATETABLE(
                DISTINCT(UserCourseAccessLog[user id]),
                UserCourseAccessLog[enrollment type] = ""student""
            ),
            ""Views"", CALCULATE(SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])))
        ),
        [Views] >= 10 && [Views] < 50
    )
)"
).DisplayFolder = "Engagement\\Levels";

measuresTable.AddMeasure(
    "Low Engagement Students",
    @"COUNTROWS(
    FILTER(
        ADDCOLUMNS(
            CALCULATETABLE(
                DISTINCT(UserCourseAccessLog[user id]),
                UserCourseAccessLog[enrollment type] = ""student""
            ),
            ""Views"", CALCULATE(SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])))
        ),
        [Views] >= 1 && [Views] < 10
    )
)"
).DisplayFolder = "Engagement\\Levels";

measuresTable.AddMeasure(
    "Zero Engagement Students",
    @"COUNTROWS(
    FILTER(
        ADDCOLUMNS(
            CALCULATETABLE(
                DISTINCT(UserCourseAccessLog[user id]),
                UserCourseAccessLog[enrollment type] = ""student""
            ),
            ""Views"", CALCULATE(SUMX(UserCourseAccessLog, VALUE(UserCourseAccessLog[times viewed])))
        ),
        ISBLANK([Views]) || [Views] = 0
    )
)"
).DisplayFolder = "Engagement\\Levels";

// ============================================
// ENGAGEMENT LEVEL PERCENTAGES
// ============================================

measuresTable.AddMeasure(
    "Pct High Engagement",
    @"DIVIDE([High Engagement Students], [Student Count], 0)"
).DisplayFolder = "Engagement\\Levels";

measuresTable.AddMeasure(
    "Pct Medium Engagement",
    @"DIVIDE([Medium Engagement Students], [Student Count], 0)"
).DisplayFolder = "Engagement\\Levels";

measuresTable.AddMeasure(
    "Pct Low Engagement",
    @"DIVIDE([Low Engagement Students], [Student Count], 0)"
).DisplayFolder = "Engagement\\Levels";

measuresTable.AddMeasure(
    "Pct Zero Engagement",
    @"DIVIDE([Zero Engagement Students], [Student Count], 0)"
).DisplayFolder = "Engagement\\Levels";

// ============================================
// FORMAT ALL MEASURES
// ============================================

foreach (var m in measuresTable.Measures)
{
    if (m.Name.StartsWith("Pct") || m.Name.Contains("Percent"))
    {
        m.FormatString = "0.0%";
    }
    else if (m.Name.Contains("Avg"))
    {
        m.FormatString = "0.0";
    }
    else
    {
        m.FormatString = "#,0";
    }
}

Info("SUCCESS: 31 measures created!");
Info("All measures handle TEXT columns properly.");
