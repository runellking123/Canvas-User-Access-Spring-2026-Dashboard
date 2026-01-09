// ============================================
// TABULAR EDITOR 3 - CREATE CANVAS RELATIONSHIPS
// ============================================

// Define table and column names
var lastUserAccessTable = "LastUserAccess";
var userCourseAccessLogTable = "UserCourseAccessLog";
var userIdColumn = "user id";

// Track created relationships
int relationshipsCreated = 0;
int relationshipsSkipped = 0;

// Helper function to check if relationship exists
Func<string, string, string, string, bool> RelationshipExists = (fromTable, fromCol, toTable, toCol) =>
{
    foreach (var rel in Model.Relationships)
    {
        if ((rel.FromTable.Name == fromTable && rel.FromColumn.Name == fromCol &&
             rel.ToTable.Name == toTable && rel.ToColumn.Name == toCol) ||
            (rel.FromTable.Name == toTable && rel.FromColumn.Name == toCol &&
             rel.ToTable.Name == fromTable && rel.ToColumn.Name == fromCol))
        {
            return true;
        }
    }
    return false;
};

// Helper function to create relationship safely
Action<string, string, string, string, string> CreateRelationship = (fromTableName, fromColumnName, toTableName, toColumnName, description) =>
{
    // Check if tables exist
    if (!Model.Tables.Contains(fromTableName))
    {
        Warning("Table '" + fromTableName + "' not found. Skipping relationship: " + description);
        relationshipsSkipped++;
        return;
    }
    if (!Model.Tables.Contains(toTableName))
    {
        Warning("Table '" + toTableName + "' not found. Skipping relationship: " + description);
        relationshipsSkipped++;
        return;
    }

    var fromTable = Model.Tables[fromTableName];
    var toTable = Model.Tables[toTableName];

    // Check if columns exist
    if (!fromTable.Columns.Contains(fromColumnName))
    {
        Warning("Column '" + fromColumnName + "' not found in '" + fromTableName + "'. Skipping relationship: " + description);
        relationshipsSkipped++;
        return;
    }
    if (!toTable.Columns.Contains(toColumnName))
    {
        Warning("Column '" + toColumnName + "' not found in '" + toTableName + "'. Skipping relationship: " + description);
        relationshipsSkipped++;
        return;
    }

    // Check if relationship already exists
    if (RelationshipExists(fromTableName, fromColumnName, toTableName, toColumnName))
    {
        Info("Relationship already exists: " + description);
        relationshipsSkipped++;
        return;
    }

    // Create the relationship
    var fromColumn = fromTable.Columns[fromColumnName];
    var toColumn = toTable.Columns[toColumnName];

    var relationship = Model.AddRelationship();
    relationship.FromColumn = fromColumn;
    relationship.ToColumn = toColumn;
    relationship.CrossFilteringBehavior = CrossFilteringBehavior.OneDirection;
    relationship.IsActive = true;

    Info("Created relationship: " + description);
    relationshipsCreated++;
};

// ============================================
// CREATE RELATIONSHIPS
// ============================================

// Primary relationship: LastUserAccess to UserCourseAccessLog on user id
// LastUserAccess = One side (lookup/dimension - one row per user)
// UserCourseAccessLog = Many side (fact - multiple rows per user)
CreateRelationship(
    userCourseAccessLogTable, userIdColumn,  // Many side (From)
    lastUserAccessTable, userIdColumn,        // One side (To)
    "UserCourseAccessLog[user id] -> LastUserAccess[user id] (Many to One)"
);

// ============================================
// SUMMARY
// ============================================

Info("========================================");
Info("Relationships Created: " + relationshipsCreated);
Info("Relationships Skipped: " + relationshipsSkipped);
Info("========================================");

if (relationshipsCreated > 0)
{
    Info("Remember to save the model to persist changes.");
}