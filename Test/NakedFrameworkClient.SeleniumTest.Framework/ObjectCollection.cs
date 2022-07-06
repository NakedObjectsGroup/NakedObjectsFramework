using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public class ObjectCollection : SubView {
    public ObjectCollection(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    //TODO local collection actions

    public ObjectCollection AssertDetails(string expected) {
        var actual = helper.WaitForChildElement(element, ".summary .details").Text;
        Assert.AreEqual(expected, actual);
        return this;
    }

    public ObjectCollection AssertNoOfRowsIs(int rows) => throw new NotImplementedException();

    public ObjectCollection AssertIsClosed() => throw new NotImplementedException();

    public ObjectCollection ClickCloseCollection() => throw new NotImplementedException();

    public ObjectCollection AssertIsOpenAsList() => throw new NotImplementedException();

    public ObjectCollection ClickListView() {
        helper.WaitForCss(".icon.list").Click();
        Thread.Sleep(500);
        helper.WaitForChildElement(element, "tbody tr");
        return this;
    }

    public ObjectCollection AssertIsOpenAsTable() {
        helper.WaitForChildElement(element, "thead tr th");
        return this;
    }

    public ObjectCollection ClickTableView() {
        helper.WaitForCss(".icon.table").Click();
        Thread.Sleep(500);
        helper.WaitForChildElement(element, "thead tr th");
        return this;
    }

    public ObjectCollection AssertTableHeaderHasColumns(params string[] expectedHeaders) {
        var actualHeaders = element.FindElements(By.CssSelector("thead tr th")).Select(el => el.Text).ToArray();
        CollectionAssert.AreEqual(expectedHeaders, actualHeaders);
        return this;
    }

    //Row number counts from zero
    public TableRow GetRowFromTable(int rowNumber) {
        helper.wait.Until(dr => element.FindElements(By.CssSelector("tbody tr")).Count > rowNumber);
        var row = element.FindElements(By.CssSelector("tbody tr")).ElementAt(rowNumber);
        return new TableRow(row, helper, enclosingView);
    }

    public TableRow GetLastRowFromTable() {
        var last = element.FindElements(By.CssSelector("tbody tr")).Count - 1;
        return GetRowFromTable(last);
    }

    //Row number counts from zero
    public Reference GetRowFromList(int rowNumber) {
        helper.wait.Until(dr => element.FindElements(By.CssSelector("tbody tr td.reference")).Count > rowNumber);
        var row = element.FindElements(By.CssSelector("tbody tr td.reference"))[rowNumber];
        return new Reference(row, helper, enclosingView);
    }

    public Reference GetLastRowFromList() {
        var last = element.FindElements(By.CssSelector("tbody tr td.reference")).Count - 1;
        return GetRowFromList(last);
    }

    public ActionWithDialog GetActionWithDialog(string actionName) {
        var actionSelector = $"nof-action input[value=\"{actionName}\"]";
        var act = helper.wait.Until(d => element.FindElement(By.CssSelector(actionSelector)));
        //TODO: test that it generates a dialog - information not currently available in HTML see #292
        return new ActionWithDialog(act, helper, enclosingView);
    }

    public ActionWithoutDialog GetActionWithoutDialog(string actionName) {
        var actionSelector = $"nof-action input[value=\"{actionName}\"]";
        var act = helper.wait.Until(d => element.FindElement(By.CssSelector(actionSelector)));
        //TODO: test that it does not generate a dialog - information not currently available in HTML see #292
        return new ActionWithoutDialog(act, helper, enclosingView);
    }

    public ObjectCollection SelectCheckBoxOnRow(int rowNumber) {
        helper.wait.Until(dr => element.FindElements(By.CssSelector("tbody tr")).Count > rowNumber);
        var row = element.FindElements(By.CssSelector("tbody tr")).ElementAt(rowNumber);
        var checkbox = row.FindElement(By.CssSelector("td.checkbox input"));
        checkbox.Click();
        helper.wait.Until(dr => checkbox.Selected);
        return this;
    }
}