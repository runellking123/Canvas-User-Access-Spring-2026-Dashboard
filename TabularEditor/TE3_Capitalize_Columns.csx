// Tabular Editor 3 C# Script
// Capitalizes the first letter of each word in all column names

foreach (var column in Model.AllColumns)
{
    var words = column.Name.Split(' ');
    var capitalizedWords = words.Select(word =>
        string.IsNullOrEmpty(word) ? word :
        char.ToUpper(word[0]) + word.Substring(1).ToLower()
    );
    column.Name = string.Join(" ", capitalizedWords);
}