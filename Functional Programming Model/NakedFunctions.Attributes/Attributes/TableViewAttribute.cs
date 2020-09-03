using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Indicate whether  the annotated object should have a title column when displayed in a table.
    ///     Also provide a list of the properties of the object to be shown as columns when the object is displayed in a table.
    ///     The columns will be displayed in the same order as the list of properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class TableViewAttribute : Attribute
    {
        public TableViewAttribute(bool title, params string[] columns)
        {
            Title = title;
            Columns = columns;
        }

        public string[] Columns { get; private set; }
        public bool Title { get; private set; }
    }
}
