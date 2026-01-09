// ============================================
// TABULAR EDITOR 3 - ADD TABLE AND COLUMN DESCRIPTIONS
// For Power BI Copilot optimization
// ============================================

// ============================================
// TABLE DESCRIPTIONS
// ============================================

if (Model.Tables.Contains("LastUserAccess"))
{
    var t = Model.Tables["LastUserAccess"];
    t.Description =
        "Contains the last login/access information for each user in Canvas LMS. " +
        "Each row represents one unique user with their most recent access timestamp. " +
        "Use this table to analyze user login patterns, identify inactive users, and track platform adoption. " +
        "Primary key: user id. Grain: one row per user.";
}

if (Model.Tables.Contains("UserCourseAccessLog"))
{
    var t = Model.Tables["UserCourseAccessLog"];
    t.Description =
        "Contains detailed course activity logs for users in Canvas LMS. " +
        "Each row represents a user's interaction with specific content in a course. " +
        "Use this table to analyze engagement patterns, content usage, and participation by course, section, or user. " +
        "Grain: one row per user per course per content item. Related to LastUserAccess via user id.";
}

if (Model.Tables.Contains("CanvasConfig"))
{
    var t = Model.Tables["CanvasConfig"];
    t.Description =
        "Configuration table containing Canvas API connection settings. " +
        "This is a helper table used for data refresh and should not be used in reports. " +
        "Do not include this table in any visualizations or analysis.";
    t.IsHidden = true;
}

if (Model.Tables.Contains("TriggerNewReports"))
{
    var t = Model.Tables["TriggerNewReports"];
    t.Description =
        "Helper table used to trigger new Canvas report generation. " +
        "This table initiates API calls to Canvas and should not be used in reports. " +
        "Do not include this table in any visualizations or analysis.";
    t.IsHidden = true;
}

if (Model.Tables.Contains("_Measures"))
{
    var t = Model.Tables["_Measures"];
    t.Description =
        "Container table for all DAX measures in this model. " +
        "This table holds calculated measures organized into display folders. " +
        "Use the measures from this table for all calculations and KPIs in reports.";
}

// ============================================
// LastUserAccess COLUMN DESCRIPTIONS
// ============================================

if (Model.Tables.Contains("LastUserAccess"))
{
    var t = Model.Tables["LastUserAccess"];

    Action<string, string> SetCol = (colName, desc) =>
    {
        if (t.Columns.Contains(colName))
            t.Columns[colName].Description = desc;
    };

    SetCol("user id",
        "Unique Canvas identifier for the user. " +
        "This is the primary key for users in Canvas LMS. " +
        "Use this column to join with UserCourseAccessLog table or filter to specific users.");

    SetCol("user sis id",
        "Student Information System (SIS) identifier for the user. " +
        "This links the Canvas user to your institution's student/employee records. " +
        "May be a student ID number or employee ID. Can be blank if not synced from SIS.");

    SetCol("user name",
        "Full display name of the user in Canvas LMS. " +
        "Typically formatted as 'First Last' or 'Last, First'. " +
        "Use this column to display user names in reports and tables.");

    SetCol("last access at",
        "Timestamp of when the user last accessed Canvas LMS. " +
        "Format: ISO 8601 datetime (YYYY-MM-DDTHH:MM:SSZ). " +
        "Blank/null means the user has never logged in. Use to calculate days since last access.");

    SetCol("last ip",
        "IP address from which the user last accessed Canvas LMS. " +
        "Can be used for security analysis or geographic insights. " +
        "May be blank for users who have never logged in.");
}

// ============================================
// UserCourseAccessLog COLUMN DESCRIPTIONS
// ============================================

if (Model.Tables.Contains("UserCourseAccessLog"))
{
    var t = Model.Tables["UserCourseAccessLog"];

    Action<string, string> SetCol = (colName, desc) =>
    {
        if (t.Columns.Contains(colName))
            t.Columns[colName].Description = desc;
    };

    // Section columns
    SetCol("section id",
        "Unique Canvas identifier for the course section. " +
        "Sections are subdivisions of courses (e.g., different class periods). " +
        "Use to analyze engagement at the section level.");

    SetCol("section sis id",
        "Student Information System (SIS) identifier for the section. " +
        "Links to your institution's scheduling system. " +
        "Format varies by institution.");

    SetCol("section name",
        "Display name of the course section. " +
        "Often includes course code and section number. " +
        "Use this column to display section names in reports.");

    // Course columns
    SetCol("course id",
        "Unique Canvas identifier for the course. " +
        "Use to filter or group data by specific courses. " +
        "Multiple sections can belong to one course.");

    SetCol("course sis id",
        "Student Information System (SIS) identifier for the course. " +
        "Links to your institution's course catalog. " +
        "Format varies by institution.");

    SetCol("course name",
        "Display name of the course in Canvas LMS. " +
        "Typically includes course title and sometimes term/section info. " +
        "Use this column to display course names in reports and filters.");

    // Term columns
    SetCol("term id",
        "Unique Canvas identifier for the enrollment term. " +
        "Terms represent academic periods (Fall 2025, Spring 2026, etc.). " +
        "Use to filter data by academic term.");

    SetCol("term sis id",
        "Student Information System (SIS) identifier for the term. " +
        "Links to your institution's academic calendar. " +
        "Format varies by institution.");

    SetCol("term name",
        "Display name of the enrollment term. " +
        "Examples: 'Fall 2025', 'Spring 2026', '2025-2026 Traditional'. " +
        "Use this column for term-based filtering and grouping.");

    // User columns
    SetCol("enrollment type",
        "The role of the user in this course enrollment. " +
        "Values: 'student', 'teacher', 'ta', 'observer', 'designer'. " +
        "Use to filter analysis by user role (e.g., students only).");

    SetCol("user id",
        "Unique Canvas identifier for the user. " +
        "Foreign key to LastUserAccess table. " +
        "Use to join tables or count unique users.");

    SetCol("user sis id",
        "Student Information System (SIS) identifier for the user. " +
        "Links to your institution's student/employee records. " +
        "Use to connect Canvas data with institutional data.");

    SetCol("user name",
        "Full display name of the user in Canvas LMS. " +
        "Use this column to display user names in reports. " +
        "For student privacy, consider hiding in shared reports.");

    // Content columns
    SetCol("content type",
        "The type of Canvas content that was accessed. " +
        "Values: 'assignments', 'discussion_topics', 'quizzes', 'modules', 'pages', 'attachments', 'announcements'. " +
        "Use to analyze which content types are most viewed.");

    SetCol("content",
        "The name or title of the specific content item accessed. " +
        "Examples: 'Week 1 Assignment', 'Chapter 1 Quiz', 'Syllabus'. " +
        "Use to identify specific high-traffic or low-traffic content items.");

    // Activity columns
    SetCol("times viewed",
        "Number of times the user viewed this content item. " +
        "A view is recorded each time the content is loaded. " +
        "Higher values indicate more engagement with this content. Numeric value stored as text.");

    SetCol("times participated",
        "Number of times the user actively participated with this content. " +
        "Participations include: submitting, posting, commenting, taking quizzes. " +
        "Higher values indicate active engagement beyond passive viewing. Numeric value stored as text.");

    SetCol("last viewed",
        "Timestamp of when the user last viewed this content item. " +
        "Format: ISO 8601 datetime. " +
        "Use to identify recently accessed vs. stale content.");
}

// ============================================
// SUMMARY
// ============================================

Info("Added descriptions to all tables and columns for Power BI Copilot optimization.");
Info("Hidden helper tables: CanvasConfig, TriggerNewReports");