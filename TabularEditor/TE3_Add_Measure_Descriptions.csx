// ============================================
// TABULAR EDITOR 3 - ADD MEASURE DESCRIPTIONS
// For Power BI Copilot optimization
// ============================================

var measuresTableName = "_Measures";

if (!Model.Tables.Contains(measuresTableName))
{
    Error("_Measures table not found. Run TE3_Create_Canvas_Measures.csx first.");
    return;
}

var measuresTable = Model.Tables[measuresTableName];

// Helper function to set measure description
Action<string, string> SetDescription = (measureName, description) =>
{
    if (measuresTable.Measures.Contains(measureName))
    {
        measuresTable.Measures[measureName].Description = description;
    }
};

// ============================================
// USER ACTIVITY DESCRIPTIONS
// ============================================

SetDescription("Total Users",
    "The total count of unique users in the Canvas LMS system. " +
    "This includes all user types: students, teachers, observers, TAs, and designers. " +
    "Use this measure to understand the overall user population size.");

SetDescription("Active Users 7 Days",
    "The count of unique users who have logged into Canvas LMS within the last 7 days. " +
    "A user is considered active if their last access date is within 7 days of today. " +
    "Use this measure to track recent engagement and identify weekly active users.");

SetDescription("Active Users 30 Days",
    "The count of unique users who have logged into Canvas LMS within the last 30 days. " +
    "A user is considered active if their last access date is within 30 days of today. " +
    "Use this measure to track monthly active users and overall platform adoption.");

SetDescription("Inactive Users 30 Days",
    "The count of unique users who have NOT logged into Canvas LMS in over 30 days. " +
    "These users had previous access but have not returned recently. " +
    "Use this measure to identify users who may need outreach or support.");

SetDescription("Never Accessed Users",
    "The count of unique users who have never logged into Canvas LMS. " +
    "These users exist in the system but have no recorded access date. " +
    "Use this measure to identify users who may need onboarding assistance.");

SetDescription("Avg Days Since Access",
    "The average number of days since users last accessed Canvas LMS. " +
    "Calculated as the mean of days between each user's last access date and today. " +
    "Lower values indicate more recent overall engagement. Measured in days.");

// ============================================
// PERCENTAGE DESCRIPTIONS
// ============================================

SetDescription("Pct Active 7 Days",
    "The percentage of total users who have been active in the last 7 days. " +
    "Calculated as Active Users 7 Days divided by Total Users. " +
    "Displayed as a percentage (e.g., 0.75 = 75%). Use to measure weekly engagement rate.");

SetDescription("Pct Active 30 Days",
    "The percentage of total users who have been active in the last 30 days. " +
    "Calculated as Active Users 30 Days divided by Total Users. " +
    "Displayed as a percentage (e.g., 0.85 = 85%). Use to measure monthly engagement rate.");

SetDescription("Pct Inactive 30 Days",
    "The percentage of total users who have been inactive for over 30 days. " +
    "Calculated as Inactive Users 30 Days divided by Total Users. " +
    "Displayed as a percentage. Use to identify the proportion of disengaged users.");

SetDescription("Pct Never Accessed",
    "The percentage of total users who have never accessed Canvas LMS. " +
    "Calculated as Never Accessed Users divided by Total Users. " +
    "Displayed as a percentage. Use to measure onboarding completion rate.");

// ============================================
// ENGAGEMENT DESCRIPTIONS
// ============================================

SetDescription("Total Views",
    "The total sum of all content views across all users in Canvas LMS. " +
    "Each time a user views any content item (assignment, quiz, page, etc.), it counts as one view. " +
    "Use this measure to understand overall platform usage volume.");

SetDescription("Total Participations",
    "The total sum of all participation actions across all users in Canvas LMS. " +
    "Participations include submitting assignments, posting to discussions, taking quizzes, etc. " +
    "This measures active engagement beyond passive viewing.");

SetDescription("Unique Users With Activity",
    "The count of unique users who have any recorded course activity in Canvas LMS. " +
    "This counts users from the UserCourseAccessLog table. " +
    "Use to compare against Total Users to see how many users have course engagement.");

SetDescription("Avg Views Per User",
    "The average number of content views per user. " +
    "Calculated as Total Views divided by Unique Users With Activity. " +
    "Use this measure to understand typical user viewing behavior.");

SetDescription("Avg Participations Per User",
    "The average number of participation actions per user. " +
    "Calculated as Total Participations divided by Unique Users With Activity. " +
    "Use this measure to understand typical user engagement level.");

// ============================================
// CONTENT TYPE DESCRIPTIONS
// ============================================

SetDescription("Assignment Views",
    "The total number of times assignment content was viewed in Canvas LMS. " +
    "Includes views of assignment instructions and submission pages. " +
    "Use to measure student engagement with assignments.");

SetDescription("Discussion Views",
    "The total number of times discussion topics were viewed in Canvas LMS. " +
    "Includes views of discussion boards and individual discussion threads. " +
    "Use to measure engagement with collaborative discussions.");

SetDescription("Quiz Views",
    "The total number of times quiz content was viewed in Canvas LMS. " +
    "Includes views of quiz instructions and quiz attempt pages. " +
    "Use to measure student engagement with assessments.");

SetDescription("Module Views",
    "The total number of times module content was viewed in Canvas LMS. " +
    "Modules are containers that organize course content. " +
    "Use to measure how students navigate through course structure.");

SetDescription("Page Views",
    "The total number of times wiki/content pages were viewed in Canvas LMS. " +
    "Pages contain instructional content, resources, and information. " +
    "Use to measure engagement with course reading materials.");

SetDescription("File Views",
    "The total number of times file attachments were viewed in Canvas LMS. " +
    "Includes PDFs, documents, images, and other uploaded files. " +
    "Use to measure engagement with downloadable resources.");

SetDescription("Announcement Views",
    "The total number of times announcements were viewed in Canvas LMS. " +
    "Announcements are course-wide communications from instructors. " +
    "Use to measure how well students are receiving course communications.");

// ============================================
// ENROLLMENT DESCRIPTIONS
// ============================================

SetDescription("Student Count",
    "The count of unique users enrolled as students in Canvas LMS courses. " +
    "Only counts users with enrollment type 'student'. " +
    "Use this measure for student-specific analysis and reporting.");

SetDescription("Teacher Count",
    "The count of unique users enrolled as teachers/instructors in Canvas LMS courses. " +
    "Only counts users with enrollment type 'teacher'. " +
    "Use this measure for faculty-specific analysis and reporting.");

SetDescription("Observer Count",
    "The count of unique users enrolled as observers in Canvas LMS courses. " +
    "Observers are typically parents, advisors, or auditors who can view but not participate. " +
    "Only counts users with enrollment type 'observer'.");

SetDescription("TA Count",
    "The count of unique users enrolled as teaching assistants in Canvas LMS courses. " +
    "TAs typically have grading and limited administrative permissions. " +
    "Only counts users with enrollment type 'ta'.");

SetDescription("Designer Count",
    "The count of unique users enrolled as course designers in Canvas LMS courses. " +
    "Designers can edit course content but typically cannot grade. " +
    "Only counts users with enrollment type 'designer'.");

// ============================================
// COURSE METRICS DESCRIPTIONS
// ============================================

SetDescription("Total Courses",
    "The count of unique courses in Canvas LMS with recorded activity. " +
    "Each course is counted once regardless of how many sections or users it has. " +
    "Use this measure to understand the scope of course offerings.");

SetDescription("Total Sections",
    "The count of unique course sections in Canvas LMS with recorded activity. " +
    "Sections are subdivisions of courses, often representing different class periods or groups. " +
    "Use this measure to understand course section distribution.");

SetDescription("Avg Views Per Course",
    "The average number of content views per course. " +
    "Calculated as Total Views divided by Total Courses. " +
    "Use to compare engagement levels across courses.");

SetDescription("Avg Students Per Course",
    "The average number of students enrolled per course. " +
    "Calculated as Student Count divided by Total Courses. " +
    "Use to understand typical course enrollment sizes.");

SetDescription("Avg Participations Per Course",
    "The average number of participation actions per course. " +
    "Calculated as Total Participations divided by Total Courses. " +
    "Use to compare active engagement levels across courses.");

// ============================================
// STUDENT ENGAGEMENT DESCRIPTIONS
// ============================================

SetDescription("Student Total Views",
    "The total sum of all content views by students only. " +
    "Excludes views by teachers, TAs, observers, and designers. " +
    "Use this measure for student-specific engagement analysis.");

SetDescription("Student Total Participations",
    "The total sum of all participation actions by students only. " +
    "Excludes participations by teachers, TAs, observers, and designers. " +
    "Use this measure for student-specific active engagement analysis.");

SetDescription("Student Avg Views",
    "The average number of content views per student. " +
    "Calculated as Student Total Views divided by Student Count. " +
    "Use to understand typical student viewing behavior.");

SetDescription("Student Avg Participations",
    "The average number of participation actions per student. " +
    "Calculated as Student Total Participations divided by Student Count. " +
    "Use to understand typical student active engagement.");

// ============================================
// ENGAGEMENT LEVEL DESCRIPTIONS
// ============================================

SetDescription("High Engagement Students",
    "The count of students with 50 or more total content views. " +
    "These students are highly active in viewing course materials. " +
    "Use to identify your most engaged student population.");

SetDescription("Medium Engagement Students",
    "The count of students with 10 to 49 total content views. " +
    "These students have moderate engagement with course materials. " +
    "Use to identify students with average engagement levels.");

SetDescription("Low Engagement Students",
    "The count of students with 1 to 9 total content views. " +
    "These students have minimal engagement with course materials. " +
    "Use to identify students who may need additional support or outreach.");

SetDescription("Zero Engagement Students",
    "The count of students with zero content views. " +
    "These students are enrolled but have not viewed any course content. " +
    "Use to identify students at risk of falling behind or dropping out.");

SetDescription("Pct High Engagement",
    "The percentage of students classified as high engagement (50+ views). " +
    "Calculated as High Engagement Students divided by Student Count. " +
    "Displayed as a percentage. Target: higher is better.");

SetDescription("Pct Medium Engagement",
    "The percentage of students classified as medium engagement (10-49 views). " +
    "Calculated as Medium Engagement Students divided by Student Count. " +
    "Displayed as a percentage.");

SetDescription("Pct Low Engagement",
    "The percentage of students classified as low engagement (1-9 views). " +
    "Calculated as Low Engagement Students divided by Student Count. " +
    "Displayed as a percentage. Target: lower is better.");

SetDescription("Pct Zero Engagement",
    "The percentage of students classified as zero engagement (0 views). " +
    "Calculated as Zero Engagement Students divided by Student Count. " +
    "Displayed as a percentage. Target: 0% is ideal.");

// ============================================
// SUMMARY
// ============================================

int count = 0;
foreach (var m in measuresTable.Measures)
{
    if (!string.IsNullOrEmpty(m.Description)) count++;
}

Info("Added descriptions to " + count + " measures for Power BI Copilot optimization.");