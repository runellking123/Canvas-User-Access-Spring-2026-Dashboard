// Tabular Editor C# Script to Update Power BI Queries
// This script updates the Canvas dashboard to read from static CSV files
// instead of using dynamic API calls
//
// How to use:
// 1. Open Tabular Editor 3
// 2. Connect to your Power BI dataset via XMLA endpoint
// 3. Go to File > Open Script (or press Ctrl+Shift+O)
// 4. Select this file
// 5. Press F5 to run the script
// 6. Save the model (Ctrl+S)

// Define the data folder path
var dataFolder = @"C:\Users\ruking\CanvasDataFetch\Data";

// ============================================
// Update LastUserAccess table
// ============================================
var lastUserAccessTable = Model.Tables["LastUserAccess"];
if (lastUserAccessTable != null)
{
    var partition = lastUserAccessTable.Partitions[0];
    partition.Expression = $@"let
    Source = Csv.Document(File.Contents(""{dataFolder}\LastUserAccess.csv""), [Delimiter="","", Columns=5, Encoding=65001, QuoteStyle=QuoteStyle.None]),
    #""Promoted Headers"" = Table.PromoteHeaders(Source, [PromoteAllScalars=true]),
    #""Changed Type"" = Table.TransformColumnTypes(#""Promoted Headers"", {{{{""user_id"", Int64.Type}}, {{""user_sis_id"", type text}}, {{""user_name"", type text}}, {{""last_access_at"", type datetimezone}}, {{""current_login_at"", type datetimezone}}}})
in
    #""Changed Type""";

    Info("Updated LastUserAccess table to read from static CSV file.");
}
else
{
    Warning("LastUserAccess table not found!");
}

// ============================================
// Update UserCourseAccessLog table
// ============================================
var userCourseAccessTable = Model.Tables["UserCourseAccessLog"];
if (userCourseAccessTable != null)
{
    var partition = userCourseAccessTable.Partitions[0];
    partition.Expression = $@"let
    Source = Csv.Document(File.Contents(""{dataFolder}\UserCourseAccessLog.csv""), [Delimiter="","", Columns=9, Encoding=65001, QuoteStyle=QuoteStyle.None]),
    #""Promoted Headers"" = Table.PromoteHeaders(Source, [PromoteAllScalars=true]),
    #""Changed Type"" = Table.TransformColumnTypes(#""Promoted Headers"", {{{{""user_name"", type text}}, {{""course_name"", type text}}, {{""sis_id"", type text}}, {{""total_access_count"", Int64.Type}}, {{""total_access_time"", type number}}, {{""first_access"", type datetimezone}}, {{""last_access"", type datetimezone}}, {{""asset_name"", type text}}, {{""asset_type"", type text}}}})
in
    #""Changed Type""";

    Info("Updated UserCourseAccessLog table to read from static CSV file.");
}
else
{
    Warning("UserCourseAccessLog table not found!");
}

// ============================================
// Remove or disable TriggerNewReports and CanvasConfig
// since they're no longer needed
// ============================================
var triggerTable = Model.Tables.FirstOrDefault(t => t.Name == "TriggerNewReports");
if (triggerTable != null)
{
    triggerTable.IsHidden = true;
    // Or to delete: triggerTable.Delete();
    Info("Hidden TriggerNewReports table (no longer needed).");
}

var configTable = Model.Tables.FirstOrDefault(t => t.Name == "CanvasConfig");
if (configTable != null)
{
    configTable.IsHidden = true;
    // Or to delete: configTable.Delete();
    Info("Hidden CanvasConfig table (no longer needed).");
}

Info("Script completed! Remember to save the model (Ctrl+S) and verify the changes.");
Info("After saving, the scheduled refresh should work without dynamic data source errors.");
