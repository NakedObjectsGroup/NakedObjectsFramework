namespace NakedFramework.RATL.Classic.Interface;

public interface ITestMenuItem {
    ITestMenuItem AssertNameEquals(string name);

    ITestMenuItem AssertSubMenuEquals(string name);
    ITestMenuItem AssertIsAction();
    ITestAction AsAction();
    ITestMenuItem AssertIsSubMenu();
    ITestMenu AsSubMenu();
}