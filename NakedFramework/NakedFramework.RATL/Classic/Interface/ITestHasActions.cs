﻿namespace NakedFramework.RATL.Classic.Interface;

public interface ITestHasActions : ITestNaked
{
    ITestAction[] Actions { get; }
    ITestAction GetAction(string name);

    /// <summary>
    ///     It is not necessary to specify the parameter types unless you need to disambiguate overloaded action methods
    /// </summary>
    ITestAction GetAction(string name, params Type[] parameterTypes);

    ITestAction GetAction(string name, string subMenu);

    /// <summary>
    ///     It is not necessary to specify the parameter types unless you need to disambiguate overloaded action methods
    /// </summary>
    ITestAction GetAction(string name, string subMenu, params Type[] parameterTypes);

    string GetObjectActionOrder();

    /// <summary>
    ///     Test action order against string of form: "Action1, Action2"
    ///     or "Action1, (SubMenu1:Action1, Action2)"
    /// </summary>
    /// <param name="order"></param>
    /// <returns>The current object</returns>
    ITestHasActions AssertActionOrderIs(string order);
}