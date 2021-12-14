using System;

namespace AdventureWorksModel.Attr; 

[AttributeUsage(AttributeTargets.Property)]
public class AWNotCountedAttribute : Attribute { }