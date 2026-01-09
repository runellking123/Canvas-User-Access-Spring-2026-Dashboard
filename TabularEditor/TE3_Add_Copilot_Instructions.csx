// ============================================
// TABULAR EDITOR 3 - ADD POWER BI COPILOT INSTRUCTIONS
// Adds model-level instructions for Copilot AI
// ============================================

// Set the model description with detailed Copilot instructions
Model.Description = @"
=== CANVAS LMS USER ACCESS DASHBOARD ===

PURPOSE:
This Power BI model analyzes user engagement and activity in Canvas Learning Management System (LMS) for Wiley University. It tracks student and faculty login patterns, course engagement, content usage, and identifies at-risk students based on engagement levels.

=== DATA SOURCES ===

1. LastUserAccess Table:
   - Contains one row per Canvas user
   - Shows when each user last logged into Canvas
   - Primary key: [user id]
   - Use for: login tracking, inactive user identification, platform adoption metrics

2. UserCourseAccessLog Table:
   - Contains detailed course activity records
   - Multiple rows per user (one per content item accessed)
   - Foreign key: [user id] links to LastUserAccess
   - Use for: engagement analysis, content usage, participation tracking

=== KEY MEASURES (use these for all calculations) ===

USER COUNTS:
- [Total Users]: All unique users in Canvas
- [Active Users 7 Days]: Users who logged in within 7 days
- [Active Users 30 Days]: Users who logged in within 30 days
- [Inactive Users 30 Days]: Users who haven't logged in for 30+ days
- [Never Accessed Users]: Users who have never logged in

ENGAGEMENT:
- [Total Views]: Sum of all content views
- [Total Participations]: Sum of all active participation actions
- [Student Total Views]: Views by students only
- [Student Total Participations]: Participations by students only

ENROLLMENT COUNTS:
- [Student Count]: Number of unique students
- [Teacher Count]: Number of unique teachers/instructors
- [Observer Count]: Number of observers (parents/advisors)
- [TA Count]: Number of teaching assistants
- [Designer Count]: Number of course designers

ENGAGEMENT LEVELS (students only):
- [High Engagement Students]: 50+ views (highly active)
- [Medium Engagement Students]: 10-49 views (moderately active)
- [Low Engagement Students]: 1-9 views (minimally active)
- [Zero Engagement Students]: 0 views (at risk)

=== IMPORTANT COLUMN INFORMATION ===

ENROLLMENT TYPES (filter on [enrollment type]):
- 'student' = enrolled students
- 'teacher' = instructors/faculty
- 'ta' = teaching assistants
- 'observer' = parents, advisors, auditors
- 'designer' = course designers

CONTENT TYPES (filter on [content type]):
- 'assignments' = homework, projects, submissions
- 'discussion_topics' = discussion boards
- 'quizzes' = tests, assessments
- 'modules' = course content containers
- 'pages' = wiki/content pages
- 'attachments' = files, PDFs, documents
- 'announcements' = instructor communications

=== COPILOT QUERY GUIDELINES ===

WHEN ASKED ABOUT STUDENTS:
- Always filter [enrollment type] = 'student' unless otherwise specified
- Use [Student Count] for student totals
- Use [Student Total Views] and [Student Total Participations] for student engagement

WHEN ASKED ABOUT ENGAGEMENT:
- High engagement = 50+ views
- Medium engagement = 10-49 views
- Low engagement = 1-9 views
- Zero/No engagement = 0 views
- Use the pre-built engagement level measures

WHEN ASKED ABOUT ACTIVITY:
- 'Views' means content was loaded/displayed
- 'Participations' means active action (submit, post, comment)
- 'Active users' means logged in within specified timeframe

WHEN ASKED ABOUT COURSES:
- Use [course name] for display
- Use [course id] for counting unique courses
- Use [Total Courses] measure for course counts

WHEN ASKED ABOUT TIME PERIODS:
- 7 days = weekly activity
- 30 days = monthly activity
- Use [last access at] from LastUserAccess for login times
- Use [last viewed] from UserCourseAccessLog for content access times

=== DO NOT USE ===
- CanvasConfig table (hidden, API configuration only)
- TriggerNewReports table (hidden, API trigger only)
- Raw columns for calculations (use pre-built measures instead)

=== EXAMPLE QUERIES AND RESPONSES ===

Q: 'How many students are active?'
A: Use [Active Users 30 Days] filtered to students, or [Student Count] for total enrolled students.

Q: 'Which students are at risk?'
A: Use [Zero Engagement Students] or [Low Engagement Students] measures. These identify students with minimal or no course activity.

Q: 'What content is most viewed?'
A: Group by [content type] and sum [Total Views]. Assignments, quizzes, and modules typically have highest views.

Q: 'Show me engagement by course'
A: Group by [course name], show [Student Count], [Total Views], [Student Avg Views].

Q: 'What percentage of students are engaged?'
A: Use [Pct High Engagement] + [Pct Medium Engagement] for engaged students, or [Pct Zero Engagement] for disengaged.
";

Info("Added comprehensive Copilot instructions to model description.");
Info("These instructions will help Power BI Copilot understand your data model and respond accurately.");