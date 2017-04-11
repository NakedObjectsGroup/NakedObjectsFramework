webpackJsonp([2,4],{

/***/ 12:
/***/ (function(module, exports) {

/*
	MIT License http://www.opensource.org/licenses/mit-license.php
	Author Tobias Koppers @sokra
*/
// css base code, injected by the css-loader
module.exports = function() {
	var list = [];

	// return the list of modules as css string
	list.toString = function toString() {
		var result = [];
		for(var i = 0; i < this.length; i++) {
			var item = this[i];
			if(item[2]) {
				result.push("@media " + item[2] + "{" + item[1] + "}");
			} else {
				result.push(item[1]);
			}
		}
		return result.join("");
	};

	// import a list of modules into the list
	list.i = function(modules, mediaQuery) {
		if(typeof modules === "string")
			modules = [[null, modules, ""]];
		var alreadyImportedModules = {};
		for(var i = 0; i < this.length; i++) {
			var id = this[i][0];
			if(typeof id === "number")
				alreadyImportedModules[id] = true;
		}
		for(i = 0; i < modules.length; i++) {
			var item = modules[i];
			// skip already imported module
			// this implementation is not 100% perfect for weird media query combinations
			//  when a module is imported multiple times with different media queries.
			//  I hope this will never occur (Hey this way we have smaller bundles)
			if(typeof item[0] !== "number" || !alreadyImportedModules[item[0]]) {
				if(mediaQuery && !item[2]) {
					item[2] = mediaQuery;
				} else if(mediaQuery) {
					item[2] = "(" + item[2] + ") and (" + mediaQuery + ")";
				}
				list.push(item);
			}
		}
	};
	return list;
};


/***/ }),

/***/ 1270:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "/*Global fonts*/\n.gemini, table, a, pre, textarea, input, button { \n    font-family: 'Segoe UI', 'Open Sans', Verdana, Arial, Helvetica, sans-serif;\n}\n\n@font-face {\n    font-family: \"iconFont\";\n    src: url(" + __webpack_require__(671) + ");\n    src: url(" + __webpack_require__(671) + "?#iefix) format('embedded-opentype'), url(" + __webpack_require__(1320) + "#iconFont) format('svg'), url(" + __webpack_require__(1510) + ") format('woff'), url(" + __webpack_require__(1509) + ") format('truetype');\n    font-weight: normal;\n    font-style: normal;\n}\n\n/* Override unwanted defaults */\n.gemini * {\n    /*Specifies that padding & border are added INSIDE the specified width*/\n    box-sizing: border-box;\n}\n\npre {\n    white-space: pre-wrap; /* CSS 3 */\n    white-space: -moz-pre-wrap; /* Mozilla, since 1999 */\n    white-space: -pre-wrap; /* Opera 4-6 */\n    white-space: -o-pre-wrap; /* Opera 7 */\n    word-wrap: break-word; /* Internet Explorer 5.5+ */\n}\n\n\nhtml, #main, body, .background, .single, .split {\n    margin: 0px;\n    height: 100%; /*fill the vertical space*/\n    background-color: black;\n}\n\n.background {\n    overflow-y: auto;\n}\n\n.single, .split {\n    overflow-x: auto;\n    padding-bottom: 60px;\n}\n.single {\n    width: 100%;\n}\n\n.split { \n   float: left;\n    width: 50%;\n}\n\n/*0. Black */\n.object-color0 {\n    background-color: #000000;\n    color: white;\n}\n\n.link-color0 {\n    background-color: #404040;\n    color: white;\n}\n/*1. Blue*/\n.object-color1 {\n    background-color: #2b5797;\n    color: white;\n}\n\n.link-color1 {\n    background-color: #1b4787;\n    color: white;\n}\n/*2. Green*/\n.object-color2 {\n    background-color: #1e7145;\n    color: white;\n}\n\n.link-color2 {\n    background-color: #0e6135;\n    color: white;\n}\n/*3. Red*/\n.object-color3 {\n    background-color: #ee2121;\n    color: white;\n}\n\n.link-color3 {\n    background-color: #ce0101;\n    color: white;\n}\n/*4. Yellow*/\n.object-color4 {\n    background-color: #ffc41d;\n    color: white;\n}\n\n.link-color4 {\n    background-color: #efb40d;\n    color: white;\n}\n/*5. Purple*/\n.object-color5 {\n    background-color: #603cba;\n    color: white;\n}\n\n.link-color5 {\n    background-color: #502caa;\n    color: white;\n}\n/*6. Orange*/\n.object-color6 {\n    background-color: #da532c;\n    color: white;\n}\n\n.link-color6 {\n    background-color: #ca431c;\n    color: white;\n}\n/*7. Mauve*/\n.object-color7 {\n    background-color: #9f10a7;\n    color: white;\n}\n\n.link-color7 {\n    background-color: #8f0097;\n    color: white;\n}\n/*8. Teal*/\n.object-color8 {\n    background-color: #10aba9;\n    color: white;\n}\n\n.link-color8 {\n    background-color: #009b99;\n    color: white;\n}\n/*9. Cherry*/\n.object-color9 {\n    background-color: #b91d47;\n    color: white;\n}\n\n.link-color9 {\n    background-color: #a90d37;\n    color: white;\n}\n/*10. Grey*/\n.object-color10 {\n    background-color: #525252;\n    color: white;\n}\n\n.link-color10 {\n    background-color: #424242;\n    color: white;\n}\n/*11. Yellow ochre*/\n.object-color11 {\n    background-color: #e3a21a;\n    color: white;\n}\n\n.link-color11 {\n    background-color: #d3920a;\n    color: white;\n}\n/*Fuschia*/\n.object-color12 {\n    background-color: #ff2097;\n    color: white;\n}\n\n.link-color12 {\n    background-color: #df0077;\n    color: white;\n}\n\n/* Auto-complete -  overriding standard Angular/Materials styling */\n.mat-input-placeholder-wrapper,.mat-input-underline  {\n    display: none;\n}\n\n.mat-autocomplete-panel {\n    background: #fff;\n}\n\n.mat-option {\n    white-space: nowrap;\n    overflow-x: hidden;\n    text-overflow: ellipsis;\n    display: block;\n    font-size: 10pt;\n    padding: 0 5px;\n    text-align: start;\n    text-decoration: none;\n    position: relative;\n    cursor: pointer;\n    outline: 0;\n}\n.mat-option:hover {\n    display: block;\n   background-color: grey;\n   color: white;\n}\n\nnof-edit-parameter .mat-input-wrapper, nof-edit-property .mat-input-wrapper  {\n    margin: 0;\n    padding-bottom: 0;\n}\n.cdk-overlay-pane {\n    position: absolute;\n    pointer-events: auto;\n    box-sizing: border-box;\n    z-index: 1000;\n}\n.cdk-global-overlay-wrapper, .cdk-overlay-container {\n    pointer-events: none;\n    top: 0;\n    left: 0;\n    height: 100%;\n    width: 100%;\n}", ""]);

// exports


/***/ }),

/***/ 1320:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__.p + "iconFont.507ceb31df41f6b52dc8.svg";

/***/ }),

/***/ 1502:
/***/ (function(module, exports) {

/*
	MIT License http://www.opensource.org/licenses/mit-license.php
	Author Tobias Koppers @sokra
*/
var stylesInDom = {},
	memoize = function(fn) {
		var memo;
		return function () {
			if (typeof memo === "undefined") memo = fn.apply(this, arguments);
			return memo;
		};
	},
	isOldIE = memoize(function() {
		return /msie [6-9]\b/.test(self.navigator.userAgent.toLowerCase());
	}),
	getHeadElement = memoize(function () {
		return document.head || document.getElementsByTagName("head")[0];
	}),
	singletonElement = null,
	singletonCounter = 0,
	styleElementsInsertedAtTop = [];

module.exports = function(list, options) {
	if(typeof DEBUG !== "undefined" && DEBUG) {
		if(typeof document !== "object") throw new Error("The style-loader cannot be used in a non-browser environment");
	}

	options = options || {};
	// Force single-tag solution on IE6-9, which has a hard limit on the # of <style>
	// tags it will allow on a page
	if (typeof options.singleton === "undefined") options.singleton = isOldIE();

	// By default, add <style> tags to the bottom of <head>.
	if (typeof options.insertAt === "undefined") options.insertAt = "bottom";

	var styles = listToStyles(list);
	addStylesToDom(styles, options);

	return function update(newList) {
		var mayRemove = [];
		for(var i = 0; i < styles.length; i++) {
			var item = styles[i];
			var domStyle = stylesInDom[item.id];
			domStyle.refs--;
			mayRemove.push(domStyle);
		}
		if(newList) {
			var newStyles = listToStyles(newList);
			addStylesToDom(newStyles, options);
		}
		for(var i = 0; i < mayRemove.length; i++) {
			var domStyle = mayRemove[i];
			if(domStyle.refs === 0) {
				for(var j = 0; j < domStyle.parts.length; j++)
					domStyle.parts[j]();
				delete stylesInDom[domStyle.id];
			}
		}
	};
}

function addStylesToDom(styles, options) {
	for(var i = 0; i < styles.length; i++) {
		var item = styles[i];
		var domStyle = stylesInDom[item.id];
		if(domStyle) {
			domStyle.refs++;
			for(var j = 0; j < domStyle.parts.length; j++) {
				domStyle.parts[j](item.parts[j]);
			}
			for(; j < item.parts.length; j++) {
				domStyle.parts.push(addStyle(item.parts[j], options));
			}
		} else {
			var parts = [];
			for(var j = 0; j < item.parts.length; j++) {
				parts.push(addStyle(item.parts[j], options));
			}
			stylesInDom[item.id] = {id: item.id, refs: 1, parts: parts};
		}
	}
}

function listToStyles(list) {
	var styles = [];
	var newStyles = {};
	for(var i = 0; i < list.length; i++) {
		var item = list[i];
		var id = item[0];
		var css = item[1];
		var media = item[2];
		var sourceMap = item[3];
		var part = {css: css, media: media, sourceMap: sourceMap};
		if(!newStyles[id])
			styles.push(newStyles[id] = {id: id, parts: [part]});
		else
			newStyles[id].parts.push(part);
	}
	return styles;
}

function insertStyleElement(options, styleElement) {
	var head = getHeadElement();
	var lastStyleElementInsertedAtTop = styleElementsInsertedAtTop[styleElementsInsertedAtTop.length - 1];
	if (options.insertAt === "top") {
		if(!lastStyleElementInsertedAtTop) {
			head.insertBefore(styleElement, head.firstChild);
		} else if(lastStyleElementInsertedAtTop.nextSibling) {
			head.insertBefore(styleElement, lastStyleElementInsertedAtTop.nextSibling);
		} else {
			head.appendChild(styleElement);
		}
		styleElementsInsertedAtTop.push(styleElement);
	} else if (options.insertAt === "bottom") {
		head.appendChild(styleElement);
	} else {
		throw new Error("Invalid value for parameter 'insertAt'. Must be 'top' or 'bottom'.");
	}
}

function removeStyleElement(styleElement) {
	styleElement.parentNode.removeChild(styleElement);
	var idx = styleElementsInsertedAtTop.indexOf(styleElement);
	if(idx >= 0) {
		styleElementsInsertedAtTop.splice(idx, 1);
	}
}

function createStyleElement(options) {
	var styleElement = document.createElement("style");
	styleElement.type = "text/css";
	insertStyleElement(options, styleElement);
	return styleElement;
}

function createLinkElement(options) {
	var linkElement = document.createElement("link");
	linkElement.rel = "stylesheet";
	insertStyleElement(options, linkElement);
	return linkElement;
}

function addStyle(obj, options) {
	var styleElement, update, remove;

	if (options.singleton) {
		var styleIndex = singletonCounter++;
		styleElement = singletonElement || (singletonElement = createStyleElement(options));
		update = applyToSingletonTag.bind(null, styleElement, styleIndex, false);
		remove = applyToSingletonTag.bind(null, styleElement, styleIndex, true);
	} else if(obj.sourceMap &&
		typeof URL === "function" &&
		typeof URL.createObjectURL === "function" &&
		typeof URL.revokeObjectURL === "function" &&
		typeof Blob === "function" &&
		typeof btoa === "function") {
		styleElement = createLinkElement(options);
		update = updateLink.bind(null, styleElement);
		remove = function() {
			removeStyleElement(styleElement);
			if(styleElement.href)
				URL.revokeObjectURL(styleElement.href);
		};
	} else {
		styleElement = createStyleElement(options);
		update = applyToTag.bind(null, styleElement);
		remove = function() {
			removeStyleElement(styleElement);
		};
	}

	update(obj);

	return function updateStyle(newObj) {
		if(newObj) {
			if(newObj.css === obj.css && newObj.media === obj.media && newObj.sourceMap === obj.sourceMap)
				return;
			update(obj = newObj);
		} else {
			remove();
		}
	};
}

var replaceText = (function () {
	var textStore = [];

	return function (index, replacement) {
		textStore[index] = replacement;
		return textStore.filter(Boolean).join('\n');
	};
})();

function applyToSingletonTag(styleElement, index, remove, obj) {
	var css = remove ? "" : obj.css;

	if (styleElement.styleSheet) {
		styleElement.styleSheet.cssText = replaceText(index, css);
	} else {
		var cssNode = document.createTextNode(css);
		var childNodes = styleElement.childNodes;
		if (childNodes[index]) styleElement.removeChild(childNodes[index]);
		if (childNodes.length) {
			styleElement.insertBefore(cssNode, childNodes[index]);
		} else {
			styleElement.appendChild(cssNode);
		}
	}
}

function applyToTag(styleElement, obj) {
	var css = obj.css;
	var media = obj.media;

	if(media) {
		styleElement.setAttribute("media", media)
	}

	if(styleElement.styleSheet) {
		styleElement.styleSheet.cssText = css;
	} else {
		while(styleElement.firstChild) {
			styleElement.removeChild(styleElement.firstChild);
		}
		styleElement.appendChild(document.createTextNode(css));
	}
}

function updateLink(linkElement, obj) {
	var css = obj.css;
	var sourceMap = obj.sourceMap;

	if(sourceMap) {
		// http://stackoverflow.com/a/26603875
		css += "\n/*# sourceMappingURL=data:application/json;base64," + btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap)))) + " */";
	}

	var blob = new Blob([css], { type: "text/css" });

	var oldSrc = linkElement.href;

	linkElement.href = URL.createObjectURL(blob);

	if(oldSrc)
		URL.revokeObjectURL(oldSrc);
}


/***/ }),

/***/ 1509:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__.p + "iconFont.0a1778b9b8be9e01db68.ttf";

/***/ }),

/***/ 1510:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__.p + "iconFont.2f87c9f2aedf8f582535.woff";

/***/ }),

/***/ 1515:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(857);


/***/ }),

/***/ 671:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__.p + "iconFont.61e31f00a5b39f5a3786.eot";

/***/ }),

/***/ 857:
/***/ (function(module, exports, __webpack_require__) {

// style-loader: Adds some css to the DOM by adding a <style> tag

// load the styles
var content = __webpack_require__(1270);
if(typeof content === 'string') content = [[module.i, content, '']];
// add the styles to the DOM
var update = __webpack_require__(1502)(content, {});
if(content.locals) module.exports = content.locals;
// Hot Module Replacement
if(false) {
	// When the styles change, update the <style> tags
	if(!content.locals) {
		module.hot.accept("!!../node_modules/css-loader/index.js?{\"sourceMap\":false,\"importLoaders\":1}!../node_modules/postcss-loader/index.js!./styles.css", function() {
			var newContent = require("!!../node_modules/css-loader/index.js?{\"sourceMap\":false,\"importLoaders\":1}!../node_modules/postcss-loader/index.js!./styles.css");
			if(typeof newContent === 'string') newContent = [[module.id, newContent, '']];
			update(newContent);
		});
	}
	// When the module is disposed, remove the <style> tags
	module.hot.dispose(function() { update(); });
}

/***/ })

},[1515]);
//# sourceMappingURL=styles.bundle.js.map