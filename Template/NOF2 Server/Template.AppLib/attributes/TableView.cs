using NakedFramework;

[AttributeUsage(AttributeTargets.Property|AttributeTargets.Method)]
public class TableViewAttribute : Attribute,ITableViewAttribute
{
    public TableViewAttribute(bool showTitle, params string[] columns)
    {
        TableTitle = showTitle;
        TableColumns = columns;
    }
    public bool TableTitle { get; set; }
    public string[] TableColumns { get; set; }
}