function createTestHtml(tag, type) {
	if (type) {
		return "<div><" + tag + " id='test1' type='" + type + "'/><" + tag + " id='test2' type='" + type + "'/></div>";
	}
	else {
		return "<div><" + tag + " id='test1'/><" + tag + " id='test2'/></div>";
	}
}

function setupForFocusOnFirst(tag, type) {
	var hadFocus = new Object();
	hadFocus.test1 = false;
	$("div#testHtml > div").replaceWith($(createTestHtml(tag, type)));
	$("#test1").focus(function () { hadFocus.test1 = true; });
	$("#test2").focus(function () { ok(false, "don't expect focus on test2"); });
	return hadFocus;
}

function setupForFocusOnFirstWithForm(formClass, buttonClass, inputClass) {
    var hadFocus = new Object();
    hadFocus.test1 = false;
    hadFocus.test2 = false;
    $("div#testHtml > div").replaceWith($("<div><form class='" + formClass + "'><input id='test1' type='text'><input id='test2' class='" + inputClass + "'   type='text'></input><button class='" + buttonClass + "'/></form></div>"));
    $("#test1").focus(function () { hadFocus.test1 = true; });
    $("#test2").focus(function () { hadFocus.test2 = true; });
    return hadFocus;
}


function getEvent(name, value) {
    var event = jQuery.Event("submit");
    var button = $("button#redisplayCollection")[0];
    button.name = name;
    button.value = value;
    $(button).addClass('lastClicked'); 
	event.liveFired = { activeElement: button };
	return event;
}


function checkSubmitsEnabledAndDisabled(expected) {
     ok(!nakedObjects.getDisabledSubmits(), "submits should have been cleared");    
}

function redisplayCollectionList(event) {
	stop();

	var summaryhtml = $("<div><form action='/redisplayCollection/list/test' method='post'>" +
										"<div class='nof-property' id='Items-Items'><label>Items:</label>" +
											"<div class='nof-collection-summary'>" +
												"<div class='nof-object' title=''>3 Items</div>" +
												"<button class='nof-summary' name='Redisplay' title='Summary' type='submit' value='Items=summary'>Summary</button>" +
												"<button class='nof-list nof-lastClicked' name='Redisplay' title='List' type='submit' value='Items=list'>List</button>" +
												"<button class='nof-table' name='Redisplay' title='Table' type='submit' value='Items=table'>Table</button>" +
											"</div>" +
										"</div>" +
									"</form></div>");

	$("div#testHtml > div").replaceWith(summaryhtml);
	$("div#testHtml form").submit(nakedObjects.redisplayProperty);

	$("div#testHtml form").trigger(event);

	setTimeout(function () {
	    start();
	    expect(2);
	    ok($("div#testHtml div.nof-collection-list").length == 1, "new html should be list");
	    checkSubmitsEnabledAndDisabled(3); 
	}, 1000);
};

function redisplayCollectionTable(event) {
	stop();

	var summaryhtml = $("<div><form action='/redisplayCollection/table/test' method='post'>" +
										"<div class='nof-property' id='Items-Items'><label>Items:</label>" +
											"<div class='nof-collection-summary'>" +
												"<div class='nof-object' title=''>3 Items</div>" +
												"<button class='nof-summary' name='Redisplay' title='Summary' type='submit' value='Items=summary'>Summary</button>" +
												"<button class='nof-list' name='Redisplay' title='List' type='submit' value='Items=list'>List</button>" +
												"<button class='nof-table nof-lastClicked' name='Redisplay' title='Table' type='submit' value='Items=table'>Table</button>" +
											"</div>" +
										"</div>" +
									"</form></div>");

	$("div#testHtml > div").replaceWith(summaryhtml);
	$("div#testHtml form").submit(nakedObjects.redisplayProperty);

	$("div#testHtml form").trigger(event);

	setTimeout(function () {
		start();
		expect(2);
		ok($("div#testHtml div.nof-collection-table").length == 1, "new html should be table");
		checkSubmitsEnabledAndDisabled(3); 
	}, 1000);
};

function redisplayCollectionSummary(event) {
	stop();

	var summaryhtml = $("<div><form action='/redisplayCollection/summary/test' method='post'>" +
										"<div class='nof-property' id='Items-Items'><label>Items:</label>" +
											"<div class='nof-collection-list'>" +
												"<table class='AbstractExpenseItem'>" +
													"<tr><td><div class='nof-object'><img alt='Taxi' src='' /><a href=''>Taxi</a></div></td>  </tr>" +
													"<tr><td><div class='nof-object'><img alt='General Expense' src='' /><a href=''>Meal</a></div></td></tr>" +
													"<tr><td><div class='nof-object'><img alt='Taxi' src='' /><a href=''>Taxi</a></div></td>  </tr>" +
												"</table>" +
												"<button class='nof-summary nof-lastClicked' name='Redisplay' title='Summary' type='submit' value='Items=summary'>Summary</button>" +
												"<button class='nof-list' name='Redisplay' title='List' type='submit' value='Items=list'>List</button>" +
												"<button class='nof-table' name='Redisplay' title='Table' type='submit' value='Items=table'>Table</button>" +
											"</div>" +
										"</div>" +
									"</form></div>");

	$("div#testHtml > div").replaceWith(summaryhtml);
	$("div#testHtml form").submit(nakedObjects.redisplayProperty);

	$("div#testHtml form").trigger(event);

	setTimeout(function () {
		start();
		expect(2);
		ok($("div#testHtml div.nof-collection-summary").length == 1, "new html should be summary");
		checkSubmitsEnabledAndDisabled(3); 
	}, 1000);
};

function updatePageFromActionTest(event) {
	stop();

	var page = $("<div><div id='main' class='main-content'><form class='' action='/updatePageFromAction/action/test' method='post'><div id='oldmain'></div><button class='nof-lastClicked' id='redisplayCollection' name='InvokeAction' type='submit' value='action=action'/></form></div></div>");

	$("div#testHtml > div").replaceWith(page);
	$("div#testHtml form").submit(nakedObjects.updatePageFromAction);

	$("div#testHtml form").trigger(event);

	setTimeout(function () {
		start();
		expect(2);
		ok($("div#testHtml div#newmain").length == 1, "not new html");
		checkSubmitsEnabledAndDisabled(1); 
	}, 1000);
};

function updateTestwithSubmenu(buttonName, event) {
	stop();

	var page = $("<div><form action='/updatePageFromAction/finder/test' class='nof-edit'>" +
							"<div class='Outside'/>" +
							"<div class='nof-property' id='Property-Id'><label>AProperty:</label>" +
								"<div class='nof-object'>" +
									"<div class='nof-menu' id='Property-Find'><div class='nof-menuname'>Find</div>" +
									   "<div class='nof-menuitems'>" +
											"<button class='nof-action nof-lastClicked' name='" + buttonName + "' title='Recently Viewed'  type='submit' value='aValue'>Recently Viewed</button>" +
										"</div>" +
									"</div>" +
								"</div>" +
							"</div>" +
						"</form></div>");

	$("div#testHtml > div").replaceWith(page);
	$("div#testHtml form").submit(nakedObjects.updatePageFromAction);

//	var event = jQuery.Event("submit");
//	event.originalEvent = { explicitOriginalTarget: { name: buttonName, value: "aValue"} };

	$("div#testHtml form").trigger(event);

	setTimeout(function () {
		start();
		expect(3);
		ok($("div#testHtml div.Outside").length == 1, "too much html changed");
		ok($("div#testHtml div.NewProperty").length == 1, "not new html");
		checkSubmitsEnabledAndDisabled(1); 
	}, 1000);
};

function updateDialogwithSubmenu(buttonName, event) {
	stop();

	var page = $("<div><form action='/updateDialogFromAction/finder/test' class='nof-dialog'>" +
							"<div class='Outside'/>" +
							"<div class='nof-parameter' id='Property-Id'><label>AProperty:</label>" +
								"<div class='nof-object'>" +
									"<div class='nof-menu' id='Property-Find'><div class='nof-menuname'>Find</div>" +
									   "<div class='nof-menuitems'>" +
											"<button class='nof-action nof-lastClicked' name='" + buttonName + "' title='Recently Viewed'  type='submit' value='aValue'>Recently Viewed</button>" +
										"</div>" +
									"</div>" +
								"</div>" +
							"</div>" +
						"</form></div>");

	$("div#testHtml > div").replaceWith(page);
	$("div#testHtml form").submit(nakedObjects.updatePageFromAction);

//	var event = jQuery.Event("submit");
//	event.originalEvent = { explicitOriginalTarget: { name: buttonName, value: "aValue"} };

	$("div#testHtml form").trigger(event);

	setTimeout(function () {
	    start();
	    expect(3);
		ok($("div#testHtml div.Outside").length == 1, "too much html changed");
		ok($("div#testHtml div.NewProperty").length == 1, "not new html");
		checkSubmitsEnabledAndDisabled(1); 
	}, 1000);
};

$.mockjax({
	url: '/clearHistory/test',
	contentType: 'text/plain',
	responseText: "<div><div class='nof-history'><div class='nof-object'><img alt='Obj3' src='' /><a href=''>Obj3 Title</a></div><div class='nof-object'><img alt='Obj4' src='' /><a href=''>Obj4 Title</a></div></div></div>"
});

$.mockjax({
	url: '/redisplayCollection/list/test',
	contentType: 'text/plain',
	responseText: "<div><form action='/redisplayCollection/list/test' method='post'>" +
								"<div class='nof-property' id='Items-Items'><label>Items:</label>" +
									"<div class='nof-collection-list'>" +
										"<table class='AbstractExpenseItem'>" +
											"<tr><td><div class='nof-object'><img alt='Taxi' src='' /><a href=''>Taxi</a></div></td>  </tr>" +
											"<tr><td><div class='nof-object'><img alt='General Expense' src='' /><a href=''>Meal</a></div></td></tr>" +
											"<tr><td><div class='nof-object'><img alt='Taxi' src='' /><a href=''>Taxi</a></div></td>  </tr>" +
										"</table>" +
										"<button class='nof-summary' name='Redisplay' title='Summary' type='submit' value='Items=summary'>Summary</button>" +
										"<button class='nof-list' name='Redisplay' title='List' type='submit' value='Items=list'>List</button>" +
										"<button class='nof-table' name='Redisplay' title='Table' type='submit' value='Items=table'>Table</button>" +
									"</div>" +
								"</div>" +
							"</form></div>"
});

$.mockjax({
	url: '/redisplayCollection/summary/test',
	contentType: 'text/plain',
	responseText: "<div><form action='/redisplayCollection/list/test' method='post'>" +
								"<div class='nof-property' id='Items-Items'><label>Items:</label>" +
									"<div class='nof-collection-summary'>" +
										"<div class='nof-object' title=''>3 Items</div>" +
										"<button class='nof-summary' name='Redisplay' title='Summary' type='submit' value='Items=summary'>Summary</button>" +
										"<button class='nof-list' name='Redisplay' title='List' type='submit' value='Items=list'>List</button>" +
										"<button class='nof-table' name='Redisplay' title='Table' type='submit' value='Items=table'>Table</button>" +
									"</div>" +
								"</div>" +
							"</form></div>"
});

$.mockjax({
	url: '/redisplayCollection/table/test',
	contentType: 'text/plain',
	responseText: "<div><form action='/redisplayCollection/list/test' method='post'>" +
								"<div class='nof-property' id='Items-Items'><label>Items:</label>" +
									 "<div class='nof-collection-table'>" +
									   "<table class='AbstractExpenseItem'>" +
											"<tr><th></th>" +
												"<th>Date Incurred</th>" +
												"<th>Description</th>" +
												"<th>Amount</th>" +
												"<th>Project Code</th>" +
												"<th>Status</th>" +
												"<th>Comment</th></tr>" +
											"<tr><td><div class='nof-object'><img alt='Taxi' src=''/><a href=''>Taxi</a></div></td>" +
												"<td><div class='nof-value' title=''>28-Mar-07</div></td>" +
												"<td><div class='nof-value' title=''>Euston - Mayfair</div></td>" +
												"<td><div class='nof-value' title=''>£8.50</div></td>" +
												"<td><div class='nof-object' title=''><img alt='Project Code' src=''/>001 Marketing</div></td>" +
												"<td><div class='nof-object' title=''><img alt='Expense Item Status' src=''/>New - Complete</div></td>" +
												"<td><div class='nof-value' title=''></div></td></tr>" +
										"</table>" +
										"<button class='nof-summary' name='Redisplay' title='Summary' type='submit' value='Items=summary'>Summary</button>" +
										"<button class='nof-list' name='Redisplay' title='List' type='submit' value='Items=list'>List</button>" +
										"<button class='nof-table' name='Redisplay' title='Table' type='submit' value='Items=table'>Table</button>" +
									"</div>" +
								"</div>" +
							"</form></div>"
});


$.mockjax({
	url: '/updatePageFromAction/action/test',
	contentType: 'text/plain',
	responseText: "<title>Action Test</title><div><div id='main' class='main-content'><form action='/updatePageFromAction/action/test' method='post'><div id='newmain'></div></form></div></div>"
});

$.mockjax({
	url: '/updatePageFromAction/finder/test',
	contentType: 'text/plain',
	responseText: "<title>Page Finder Test</title><div><form action='/updatePageFromAction/finder/test' class='nof-edit'>" +
							"<div class='nof-property' id='Property-Id'><label>AProperty:</label>" +
								"<div class='nof-object'>" +
								   "<div class='NewProperty'/>" +
									"<div class='nof-menu' id='Property-Find'><div class='nof-menuname'>Find</div>" +
									   "<div class='nof-menuitems'>" +
											"<button class='nof-action' name='Finder' title='Recently Viewed'  type='submit' value='avalue'>Recently Viewed</button>" +
										"</div>" +
									"</div>" +
								"</div>" +
							"</div>" +
						"</form></div>"
});

$.mockjax({
	url: '/updateDialogFromAction/finder/test',
	contentType: 'text/plain',
	responseText: "<title>Dialog Finder Test</title><div><form action='/updateDialogFromAction/finder/test' class='Dialog'>" +
							"<div class='nof-parameter' id='Property-Id'><label>AProperty:</label>" +
								"<div class='nof-object'>" +
								   "<div class='NewProperty'/>" +
									"<div class='nof-menu' id='Property-Find'><div class='nof-menuname'>Find</div>" +
									   "<div class='nof-menuitems'>" +
											"<button class='nof-action' name='Finder' title='Recently Viewed'  type='submit' value='avalue'>Recently Viewed</button>" +
										"</div>" +
									"</div>" +
								"</div>" +
							"</div>" +
						"</form></div>"
});


function getJsonPrefix() {
    // rubbish way to do this but didn't find another way to get the prefix

    var fs = [];

    for (v in this) { 
        if (v.length > 10 &&  v.substr(0, 6) === "jQuery"){
            fs.push(v); 
        }
    }

    // return last
    return fs.pop(); 
}

$.mockjax({
    url: 'Ajax/ValidateProperty',
    contentType: 'text/json',
    response: function (settings) {
        //  this.responseText = getJsonPrefix() + "(true)";
        this.responseText = "true";
    }
});

$.mockjax({
    url: 'Ajax/ValidateParameter',
    contentType: 'text/json',
    response: function (settings) {
        //this.responseText = getJsonPrefix() + "(true)";
        this.responseText = "true";
    }
});

$.mockjax({
    url: 'Ajax/ValidatePropertyFail',
    contentType: 'text/json',
    response: function (settings) {
        //this.responseText = getJsonPrefix() + "('failure string')";
        this.responseText = "'failure string'";
    }
});

$.mockjax({
    url: 'Ajax/ValidateParameterFail',
    contentType: 'text/json',
    response: function (settings) {
        //this.responseText = getJsonPrefix() + "('failure string')";
        this.responseText = "'failure string'";
    }
});

$.mockjax({
    url: '/Ajax/GetPropertyChoices',
    contentType: 'text/json',
    response: function (settings) {
        equal(settings.data['Address-CountryRegion-Select0'], "0", "expect value 0");
       // this.responseText = getJsonPrefix() + "( { 'Address-CountryRegion-Select' :  [['1', '2'], ['1', '2']],  'Address-StateProvince-Select' :  [['3', '4'], ['3', '4']]})";

        this.responseText = {'Address-CountryRegion-Select' :  [['1', '2'], ['1', '2']],  'Address-StateProvince-Select' :  [['3', '4'], ['3', '4']]};
    }
});

$.mockjax({
    url: '/Ajax/GetPropertyChoicesUnchanged',
    contentType: 'text/json',
    response: function (settings) {
        //this.responseText = getJsonPrefix() + "( { 'Address-CountryRegion-Select' :  [['0'], ['0']],  'Address-StateProvince-Select' :  [['0'], ['0']]})";
        this.responseText =  { 'Address-CountryRegion-Select' :  [['0'], ['0']],  'Address-StateProvince-Select' :  [['0'], ['0']]};
    }
});

$.mockjax({
    url: '/Ajax/GetPropertyChoicesEmpty',
    contentType: 'text/json',
    response: function (settings) {
        //this.responseText = getJsonPrefix() + "({})";
        this.responseText = {};
    }
});
