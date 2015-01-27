using System;
namespace NakedObjects {

    /// <summary>
    ///     Interface for the utility class created by the IDomainObjectContainer#NewTitleBuilder 
    ///     to help produce titles for objects without having to add lots of guard
    ///     code. 
    ///     
    ///     It provides two basic method: one to concatenate a title to the buffer; 
    ///     another to append a title with a joiner string, taking care adding in necessary
    ///     spaces. The benefits of using this class is that <c>null</c> references are 
    ///     safely ignored (rather than appearing as 'null'), and joiners (a space by default) are only
    ///     added when needed
    /// </summary>
    public interface ITitleBuilder {
        ITitleBuilder Append(object obj);
        ITitleBuilder Append(object obj, string format, string defaultValue);
        ITitleBuilder Append(string joiner, object obj);
        ITitleBuilder Append(string joiner, object obj, string format, string defaultTitle);
        ITitleBuilder Append(string joiner, string text);
        ITitleBuilder Append(string text);
        ITitleBuilder AppendSpace();
        ITitleBuilder Concat(object obj);
        ITitleBuilder Concat(object obj, string format, string defaultValue);
        ITitleBuilder Concat(string joiner, object obj);
        ITitleBuilder Concat(string joiner, object obj, string format, string defaultValue);
        ITitleBuilder Concat(string text);
        string ToString();
        ITitleBuilder Truncate(int noWords);
    }
}
