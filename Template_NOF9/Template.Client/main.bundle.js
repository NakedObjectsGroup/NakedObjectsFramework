webpackJsonp([1,4],{

/***/ 1011:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__action_action_component__ = __webpack_require__(122);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__ = __webpack_require__(604);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ActionBarComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var ActionBarComponent = (function () {
    function ActionBarComponent() {
    }
    Object.defineProperty(ActionBarComponent.prototype, "menuHolder", {
        set: function (mhvm) {
            var menuItems = mhvm.menuItems;
            var avms = _.flatten(_.map(menuItems, function (mi) { return mi.actions; }));
            this.actions = _.map(avms, function (a) { return __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__action_action_component__["b" /* wrapAction */])(a); });
        },
        enumerable: true,
        configurable: true
    });
    ActionBarComponent.prototype.focusOnFirstAction = function (actions) {
        if (actions) {
            // until first element returns true
            _.some(actions.toArray(), function (i) { return i.focus(); });
        }
    };
    ActionBarComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        this.focusOnFirstAction(this.actionChildren);
        this.actionChildren.changes.subscribe(function (ql) { return _this.focusOnFirstAction(ql); });
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Array)
    ], ActionBarComponent.prototype, "actions", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__["IMenuHolderViewModel"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__["IMenuHolderViewModel"]) === 'function' && _a) || Object), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__["IMenuHolderViewModel"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__["IMenuHolderViewModel"]) === 'function' && _b) || Object])
    ], ActionBarComponent.prototype, "menuHolder", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])(__WEBPACK_IMPORTED_MODULE_1__action_action_component__["a" /* ActionComponent */]), 
        __metadata('design:type', (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _c) || Object)
    ], ActionBarComponent.prototype, "actionChildren", void 0);
    ActionBarComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-action-bar',
            template: __webpack_require__(1339),
            styles: [__webpack_require__(1274)]
        }), 
        __metadata('design:paramtypes', [])
    ], ActionBarComponent);
    return ActionBarComponent;
    var _a, _b, _c;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/action-bar.component.js.map

/***/ }),

/***/ 1012:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__action_action_component__ = __webpack_require__(122);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__ = __webpack_require__(604);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ActionListComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var ActionListComponent = (function () {
    function ActionListComponent() {
        var _this = this;
        this.actionHolders = [];
        this.hasActions = function (menuItem) {
            var actions = menuItem.actions;
            return actions && actions.length > 0;
        };
        this.hasItems = function (menuItem) {
            var items = menuItem.menuItems;
            return items && items.length > 0;
        };
        this.menuName = function (menuItem) { return menuItem.name; };
        this.menuItems = function (menuItem) { return menuItem.menuItems; };
        this.menuActions = function (menuItem, index) {
            if (!_this.actionHolders[index]) {
                _this.actionHolders[index] = _this.getActionHolders(menuItem);
            }
            return _this.actionHolders[index];
        };
        this.toggleCollapsed = function (menuItem, index) { return menuItem.toggleCollapsed(); };
        this.navCollapsed = function (menuItem) { return menuItem.navCollapsed; };
        this.displayClass = function (menuItem) { return ({ collapsed: menuItem.navCollapsed, open: !menuItem.navCollapsed, rootMenu: !menuItem.name }); };
        this.previousActionChildrenNames = [];
    }
    Object.defineProperty(ActionListComponent.prototype, "menuHolder", {
        get: function () {
            return this.holder;
        },
        set: function (mh) {
            this.holder = mh;
            this.actionHolders = []; // clear cache;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ActionListComponent.prototype, "items", {
        get: function () {
            return this.menuHolder.menuItems;
        },
        enumerable: true,
        configurable: true
    });
    ActionListComponent.prototype.getActionHolders = function (menuItem) {
        return _.map(menuItem.actions, function (a) { return __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__action_action_component__["b" /* wrapAction */])(a); });
    };
    ActionListComponent.prototype.focusFromIndex = function (actions, index) {
        if (index === void 0) { index = 0; }
        var toFocus = actions.toArray().slice(index);
        if (toFocus && toFocus.length > 0) {
            // until first element returns true
            _.some(toFocus, function (i) { return i.focus(); });
        }
    };
    ActionListComponent.prototype.focus = function (actions) {
        if (actions && actions.length > 0) {
            var actionChildrenNames = _.map(actions.toArray(), function (a) { return a.action.value; });
            var newActions = _.difference(actionChildrenNames, this.previousActionChildrenNames);
            var index = 0;
            if (newActions && newActions.length > 0) {
                var firstAction_1 = _.first(newActions);
                index = _.findIndex(actions.toArray(), function (a) { return a.action.value === firstAction_1; });
                index = index < 0 ? 0 : index;
            }
            this.previousActionChildrenNames = actionChildrenNames;
            this.focusFromIndex(actions, index);
        }
    };
    ActionListComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        this.focus(this.actionChildren);
        this.actionChildren.changes.subscribe(function (ql) { return _this.focus(ql); });
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__["IMenuHolderViewModel"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__["IMenuHolderViewModel"]) === 'function' && _a) || Object), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__["IMenuHolderViewModel"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_models_imenu_holder_view_model__["IMenuHolderViewModel"]) === 'function' && _b) || Object])
    ], ActionListComponent.prototype, "menuHolder", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])(__WEBPACK_IMPORTED_MODULE_1__action_action_component__["a" /* ActionComponent */]), 
        __metadata('design:type', (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _c) || Object)
    ], ActionListComponent.prototype, "actionChildren", void 0);
    ActionListComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-action-list',
            template: __webpack_require__(1340),
            styles: [__webpack_require__(1275)]
        }), 
        __metadata('design:paramtypes', [])
    ], ActionListComponent);
    return ActionListComponent;
    var _a, _b, _c;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/action-list.component.js.map

/***/ }),

/***/ 1013:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__home_home_component__ = __webpack_require__(595);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__recent_recent_component__ = __webpack_require__(600);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__application_properties_application_properties_component__ = __webpack_require__(583);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__attachment_attachment_component__ = __webpack_require__(584);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__multi_line_dialog_multi_line_dialog_component__ = __webpack_require__(597);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__dynamic_object_dynamic_object_component__ = __webpack_require__(590);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__dynamic_list_dynamic_list_component__ = __webpack_require__(589);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__dynamic_error_dynamic_error_component__ = __webpack_require__(588);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__cicero_cicero_component__ = __webpack_require__(586);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__auth_service__ = __webpack_require__(147);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RoutingModule; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};













var routes = [
    {
        path: '',
        redirectTo: '/gemini/home',
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        pathMatch: 'full'
    },
    {
        path: 'gemini/home',
        component: __WEBPACK_IMPORTED_MODULE_2__home_home_component__["a" /* HomeComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single" },
        children: [
            { path: "home", component: __WEBPACK_IMPORTED_MODULE_2__home_home_component__["a" /* HomeComponent */], data: { pane: 2, paneType: "split" } },
            { path: "object", component: __WEBPACK_IMPORTED_MODULE_7__dynamic_object_dynamic_object_component__["a" /* DynamicObjectComponent */], data: { pane: 2, paneType: "split" } },
            { path: "list", component: __WEBPACK_IMPORTED_MODULE_9__dynamic_list_dynamic_list_component__["a" /* DynamicListComponent */], data: { pane: 2, paneType: "split" } },
            { path: "attachment", component: __WEBPACK_IMPORTED_MODULE_5__attachment_attachment_component__["a" /* AttachmentComponent */], data: { pane: 2, paneType: "split" } },
            { path: "recent", component: __WEBPACK_IMPORTED_MODULE_3__recent_recent_component__["a" /* RecentComponent */], data: { pane: 2, paneType: "split" } }
        ]
    },
    {
        path: 'gemini/object',
        component: __WEBPACK_IMPORTED_MODULE_7__dynamic_object_dynamic_object_component__["a" /* DynamicObjectComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single", dynamicType: __WEBPACK_IMPORTED_MODULE_8__route_data__["c" /* ViewType */].Object },
        children: [
            { path: "home", component: __WEBPACK_IMPORTED_MODULE_2__home_home_component__["a" /* HomeComponent */], data: { pane: 2, paneType: "split" } },
            { path: "object", component: __WEBPACK_IMPORTED_MODULE_7__dynamic_object_dynamic_object_component__["a" /* DynamicObjectComponent */], data: { pane: 2, paneType: "split" } },
            { path: "list", component: __WEBPACK_IMPORTED_MODULE_9__dynamic_list_dynamic_list_component__["a" /* DynamicListComponent */], data: { pane: 2, paneType: "split" } },
            { path: "attachment", component: __WEBPACK_IMPORTED_MODULE_5__attachment_attachment_component__["a" /* AttachmentComponent */], data: { pane: 2, paneType: "split" } },
            { path: "recent", component: __WEBPACK_IMPORTED_MODULE_3__recent_recent_component__["a" /* RecentComponent */], data: { pane: 2, paneType: "split" } }
        ]
    },
    {
        path: 'gemini/list',
        component: __WEBPACK_IMPORTED_MODULE_9__dynamic_list_dynamic_list_component__["a" /* DynamicListComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single" },
        children: [
            { path: "home", component: __WEBPACK_IMPORTED_MODULE_2__home_home_component__["a" /* HomeComponent */], data: { pane: 2, paneType: "split" } },
            { path: "object", component: __WEBPACK_IMPORTED_MODULE_7__dynamic_object_dynamic_object_component__["a" /* DynamicObjectComponent */], data: { pane: 2, paneType: "split" } },
            { path: "list", component: __WEBPACK_IMPORTED_MODULE_9__dynamic_list_dynamic_list_component__["a" /* DynamicListComponent */], data: { pane: 2, paneType: "split" } },
            { path: "attachment", component: __WEBPACK_IMPORTED_MODULE_5__attachment_attachment_component__["a" /* AttachmentComponent */], data: { pane: 2, paneType: "split" } },
            { path: "recent", component: __WEBPACK_IMPORTED_MODULE_3__recent_recent_component__["a" /* RecentComponent */], data: { pane: 2, paneType: "split" } }
        ]
    },
    {
        path: 'gemini/attachment',
        component: __WEBPACK_IMPORTED_MODULE_5__attachment_attachment_component__["a" /* AttachmentComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single" },
        children: [
            { path: "home", component: __WEBPACK_IMPORTED_MODULE_2__home_home_component__["a" /* HomeComponent */], data: { pane: 2, paneType: "split" } },
            { path: "object", component: __WEBPACK_IMPORTED_MODULE_7__dynamic_object_dynamic_object_component__["a" /* DynamicObjectComponent */], data: { pane: 2, paneType: "split" } },
            { path: "list", component: __WEBPACK_IMPORTED_MODULE_9__dynamic_list_dynamic_list_component__["a" /* DynamicListComponent */], data: { pane: 2, paneType: "split" } },
            { path: "attachment", component: __WEBPACK_IMPORTED_MODULE_5__attachment_attachment_component__["a" /* AttachmentComponent */], data: { pane: 2, paneType: "split" } },
            { path: "recent", component: __WEBPACK_IMPORTED_MODULE_3__recent_recent_component__["a" /* RecentComponent */], data: { pane: 2, paneType: "split" } }
        ]
    },
    {
        path: 'gemini/recent',
        component: __WEBPACK_IMPORTED_MODULE_3__recent_recent_component__["a" /* RecentComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single" },
        children: [
            { path: "home", component: __WEBPACK_IMPORTED_MODULE_2__home_home_component__["a" /* HomeComponent */], data: { pane: 2, paneType: "split" } },
            { path: "object", component: __WEBPACK_IMPORTED_MODULE_7__dynamic_object_dynamic_object_component__["a" /* DynamicObjectComponent */], data: { pane: 2, paneType: "split" } },
            { path: "list", component: __WEBPACK_IMPORTED_MODULE_9__dynamic_list_dynamic_list_component__["a" /* DynamicListComponent */], data: { pane: 2, paneType: "split" } },
            { path: "attachment", component: __WEBPACK_IMPORTED_MODULE_5__attachment_attachment_component__["a" /* AttachmentComponent */], data: { pane: 2, paneType: "split" } },
            { path: "recent", component: __WEBPACK_IMPORTED_MODULE_3__recent_recent_component__["a" /* RecentComponent */], data: { pane: 2, paneType: "split" } }
        ]
    },
    {
        path: 'gemini/error',
        component: __WEBPACK_IMPORTED_MODULE_10__dynamic_error_dynamic_error_component__["a" /* DynamicErrorComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single" }
    },
    {
        path: 'gemini/applicationProperties',
        component: __WEBPACK_IMPORTED_MODULE_4__application_properties_application_properties_component__["a" /* ApplicationPropertiesComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single" }
    },
    {
        path: 'gemini/multiLineDialog',
        component: __WEBPACK_IMPORTED_MODULE_6__multi_line_dialog_multi_line_dialog_component__["a" /* MultiLineDialogComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single" }
    },
    {
        path: 'cicero/home',
        component: __WEBPACK_IMPORTED_MODULE_11__cicero_cicero_component__["a" /* CiceroComponent */],
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        data: { pane: 1, paneType: "single" }
    },
    {
        path: '**',
        redirectTo: '/gemini/home',
        canActivate: [__WEBPACK_IMPORTED_MODULE_12__auth_service__["AuthService"]],
        pathMatch: 'full'
    }
];
var RoutingModule = (function () {
    function RoutingModule() {
    }
    RoutingModule = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["NgModule"])({
            imports: [__WEBPACK_IMPORTED_MODULE_1__angular_router__["c" /* RouterModule */].forRoot(routes)],
            exports: [__WEBPACK_IMPORTED_MODULE_1__angular_router__["c" /* RouterModule */]],
            providers: []
        }), 
        __metadata('design:paramtypes', [])
    ], RoutingModule);
    return RoutingModule;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/app-routing.module.js.map

/***/ }),

/***/ 1014:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__auth_service__ = __webpack_require__(147);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var AppComponent = (function () {
    function AppComponent(auth) {
        this.auth = auth;
    }
    AppComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'app-root',
            template: __webpack_require__(1342),
            styles: [__webpack_require__(1277)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__auth_service__["AuthService"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__auth_service__["AuthService"]) === 'function' && _a) || Object])
    ], AppComponent);
    return AppComponent;
    var _a;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/app.component.js.map

/***/ }),

/***/ 1015:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser__ = __webpack_require__(85);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_http__ = __webpack_require__(103);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__app_routing_module__ = __webpack_require__(1013);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__app_component__ = __webpack_require__(1014);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__footer_footer_component__ = __webpack_require__(1021);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__home_home_component__ = __webpack_require__(595);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__object_object_component__ = __webpack_require__(598);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__list_list_component__ = __webpack_require__(596);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__error_error_component__ = __webpack_require__(593);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__properties_properties_component__ = __webpack_require__(599);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__collections_collections_component__ = __webpack_require__(1018);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13__dialog_dialog_component__ = __webpack_require__(1019);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_14__parameters_parameters_component__ = __webpack_require__(364);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_15__edit_property_edit_property_component__ = __webpack_require__(592);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_16__view_property_view_property_component__ = __webpack_require__(1041);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_17__edit_parameter_edit_parameter_component__ = __webpack_require__(591);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_18__recent_recent_component__ = __webpack_require__(600);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_19__application_properties_application_properties_component__ = __webpack_require__(583);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_20__gemini_click_directive__ = __webpack_require__(1024);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_21__gemini_boolean_directive__ = __webpack_require__(1022);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_22__gemini_clear_directive__ = __webpack_require__(1023);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_23__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_24__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_25__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_26__click_handler_service__ = __webpack_require__(253);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_27__rep_loader_service__ = __webpack_require__(365);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_28__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_29__color_service__ = __webpack_require__(254);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_30__mask_service__ = __webpack_require__(256);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_31__collection_collection_component__ = __webpack_require__(1017);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_32_ng2_dnd__ = __webpack_require__(1330);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_33__attachment_attachment_component__ = __webpack_require__(584);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_34__multi_line_dialog_multi_line_dialog_component__ = __webpack_require__(597);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_35__view_parameter_view_parameter_component__ = __webpack_require__(1040);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_36__error_handler__ = __webpack_require__(1020);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_37__menu_bar_menu_bar_component__ = __webpack_require__(1027);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_38__action_action_component__ = __webpack_require__(122);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_39__dynamic_object_dynamic_object_component__ = __webpack_require__(590);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_40__custom_component_service__ = __webpack_require__(255);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_41__custom_component_config_service__ = __webpack_require__(587);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_42__dynamic_list_dynamic_list_component__ = __webpack_require__(589);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_43__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_44__logger_service__ = __webpack_require__(87);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_45__attachment_property_attachment_property_component__ = __webpack_require__(1016);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_46__dynamic_error_dynamic_error_component__ = __webpack_require__(588);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_47__cicero_cicero_component__ = __webpack_require__(586);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_48__cicero_command_factory_service__ = __webpack_require__(363);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_49__cicero_renderer_service__ = __webpack_require__(252);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_50__action_bar_action_bar_component__ = __webpack_require__(1011);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_51__action_list_action_list_component__ = __webpack_require__(1012);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_52__angular_material__ = __webpack_require__(965);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_53__row_row_component__ = __webpack_require__(1028);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_54__header_header_component__ = __webpack_require__(1025);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_55__auth_service__ = __webpack_require__(147);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_56_angular2_jwt__ = __webpack_require__(370);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_56_angular2_jwt___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_56_angular2_jwt__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_57__login_login_component__ = __webpack_require__(1026);
/* unused harmony export authHttpServiceFactory */
/* unused harmony export authServiceFactory */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppModule; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




























































function authHttpServiceFactory(http, configService, options) {
    if (configService.config.authenticate) {
        return new __WEBPACK_IMPORTED_MODULE_56_angular2_jwt__["AuthHttp"](new __WEBPACK_IMPORTED_MODULE_56_angular2_jwt__["AuthConfig"]({ tokenName: 'id_token' }), http, options);
    }
    else {
        return http;
    }
}
function authServiceFactory(configService, auth0AuthService, nullAuthService) {
    if (configService.config.authenticate) {
        return auth0AuthService;
    }
    else {
        return nullAuthService;
    }
}
var AppModule = (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["NgModule"])({
            declarations: [
                __WEBPACK_IMPORTED_MODULE_5__app_component__["a" /* AppComponent */],
                __WEBPACK_IMPORTED_MODULE_6__footer_footer_component__["a" /* FooterComponent */],
                __WEBPACK_IMPORTED_MODULE_7__home_home_component__["a" /* HomeComponent */],
                __WEBPACK_IMPORTED_MODULE_8__object_object_component__["a" /* ObjectComponent */],
                __WEBPACK_IMPORTED_MODULE_9__list_list_component__["a" /* ListComponent */],
                __WEBPACK_IMPORTED_MODULE_10__error_error_component__["a" /* ErrorComponent */],
                __WEBPACK_IMPORTED_MODULE_51__action_list_action_list_component__["a" /* ActionListComponent */],
                __WEBPACK_IMPORTED_MODULE_50__action_bar_action_bar_component__["a" /* ActionBarComponent */],
                __WEBPACK_IMPORTED_MODULE_11__properties_properties_component__["a" /* PropertiesComponent */],
                __WEBPACK_IMPORTED_MODULE_12__collections_collections_component__["a" /* CollectionsComponent */],
                __WEBPACK_IMPORTED_MODULE_13__dialog_dialog_component__["a" /* DialogComponent */],
                __WEBPACK_IMPORTED_MODULE_14__parameters_parameters_component__["a" /* ParametersComponent */],
                __WEBPACK_IMPORTED_MODULE_15__edit_property_edit_property_component__["a" /* EditPropertyComponent */],
                __WEBPACK_IMPORTED_MODULE_16__view_property_view_property_component__["a" /* ViewPropertyComponent */],
                __WEBPACK_IMPORTED_MODULE_17__edit_parameter_edit_parameter_component__["a" /* EditParameterComponent */],
                __WEBPACK_IMPORTED_MODULE_18__recent_recent_component__["a" /* RecentComponent */],
                __WEBPACK_IMPORTED_MODULE_19__application_properties_application_properties_component__["a" /* ApplicationPropertiesComponent */],
                __WEBPACK_IMPORTED_MODULE_20__gemini_click_directive__["a" /* GeminiClickDirective */],
                __WEBPACK_IMPORTED_MODULE_21__gemini_boolean_directive__["a" /* GeminiBooleanDirective */],
                __WEBPACK_IMPORTED_MODULE_22__gemini_clear_directive__["a" /* GeminiClearDirective */],
                __WEBPACK_IMPORTED_MODULE_31__collection_collection_component__["a" /* CollectionComponent */],
                __WEBPACK_IMPORTED_MODULE_33__attachment_attachment_component__["a" /* AttachmentComponent */],
                __WEBPACK_IMPORTED_MODULE_34__multi_line_dialog_multi_line_dialog_component__["a" /* MultiLineDialogComponent */],
                __WEBPACK_IMPORTED_MODULE_35__view_parameter_view_parameter_component__["a" /* ViewParameterComponent */],
                __WEBPACK_IMPORTED_MODULE_37__menu_bar_menu_bar_component__["a" /* MenuBarComponent */],
                __WEBPACK_IMPORTED_MODULE_38__action_action_component__["a" /* ActionComponent */],
                __WEBPACK_IMPORTED_MODULE_39__dynamic_object_dynamic_object_component__["a" /* DynamicObjectComponent */],
                __WEBPACK_IMPORTED_MODULE_42__dynamic_list_dynamic_list_component__["a" /* DynamicListComponent */],
                __WEBPACK_IMPORTED_MODULE_45__attachment_property_attachment_property_component__["a" /* AttachmentPropertyComponent */],
                __WEBPACK_IMPORTED_MODULE_46__dynamic_error_dynamic_error_component__["a" /* DynamicErrorComponent */],
                __WEBPACK_IMPORTED_MODULE_47__cicero_cicero_component__["a" /* CiceroComponent */],
                __WEBPACK_IMPORTED_MODULE_53__row_row_component__["a" /* RowComponent */],
                __WEBPACK_IMPORTED_MODULE_54__header_header_component__["a" /* HeaderComponent */],
                __WEBPACK_IMPORTED_MODULE_57__login_login_component__["a" /* LoginComponent */]
            ],
            entryComponents: [
                __WEBPACK_IMPORTED_MODULE_8__object_object_component__["a" /* ObjectComponent */],
                __WEBPACK_IMPORTED_MODULE_9__list_list_component__["a" /* ListComponent */],
                __WEBPACK_IMPORTED_MODULE_10__error_error_component__["a" /* ErrorComponent */]
            ],
            imports: [
                __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser__["a" /* BrowserModule */],
                __WEBPACK_IMPORTED_MODULE_32_ng2_dnd__["a" /* DndModule */].forRoot(),
                __WEBPACK_IMPORTED_MODULE_2__angular_forms__["a" /* FormsModule */],
                __WEBPACK_IMPORTED_MODULE_3__angular_http__["HttpModule"],
                __WEBPACK_IMPORTED_MODULE_4__app_routing_module__["a" /* RoutingModule */],
                __WEBPACK_IMPORTED_MODULE_2__angular_forms__["b" /* ReactiveFormsModule */],
                __WEBPACK_IMPORTED_MODULE_52__angular_material__["a" /* MaterialModule */]
            ],
            providers: [
                __WEBPACK_IMPORTED_MODULE_25__url_manager_service__["a" /* UrlManagerService */],
                __WEBPACK_IMPORTED_MODULE_26__click_handler_service__["a" /* ClickHandlerService */],
                __WEBPACK_IMPORTED_MODULE_24__context_service__["a" /* ContextService */],
                __WEBPACK_IMPORTED_MODULE_27__rep_loader_service__["a" /* RepLoaderService */],
                __WEBPACK_IMPORTED_MODULE_28__view_model_factory_service__["a" /* ViewModelFactoryService */],
                __WEBPACK_IMPORTED_MODULE_29__color_service__["a" /* ColorService */],
                __WEBPACK_IMPORTED_MODULE_23__error_service__["a" /* ErrorService */],
                __WEBPACK_IMPORTED_MODULE_30__mask_service__["a" /* MaskService */],
                __WEBPACK_IMPORTED_MODULE_40__custom_component_service__["a" /* CustomComponentService */],
                // to configure custom components create implementation of ICustomComponentConfigService and bind in here
                { provide: __WEBPACK_IMPORTED_MODULE_41__custom_component_config_service__["a" /* CustomComponentConfigService */], useClass: __WEBPACK_IMPORTED_MODULE_41__custom_component_config_service__["a" /* CustomComponentConfigService */] },
                __WEBPACK_IMPORTED_MODULE_44__logger_service__["a" /* LoggerService */],
                __WEBPACK_IMPORTED_MODULE_43__config_service__["a" /* ConfigService */],
                __WEBPACK_IMPORTED_MODULE_48__cicero_command_factory_service__["a" /* CiceroCommandFactoryService */],
                __WEBPACK_IMPORTED_MODULE_49__cicero_renderer_service__["a" /* CiceroRendererService */],
                __WEBPACK_IMPORTED_MODULE_55__auth_service__["Auth0AuthService"],
                __WEBPACK_IMPORTED_MODULE_55__auth_service__["NullAuthService"],
                { provide: __WEBPACK_IMPORTED_MODULE_1__angular_core__["ErrorHandler"], useClass: __WEBPACK_IMPORTED_MODULE_36__error_handler__["a" /* GeminiErrorHandler */] },
                { provide: __WEBPACK_IMPORTED_MODULE_1__angular_core__["APP_INITIALIZER"], useFactory: __WEBPACK_IMPORTED_MODULE_43__config_service__["b" /* configFactory */], deps: [__WEBPACK_IMPORTED_MODULE_43__config_service__["a" /* ConfigService */]], multi: true },
                { provide: __WEBPACK_IMPORTED_MODULE_1__angular_core__["LOCALE_ID"], useFactory: __WEBPACK_IMPORTED_MODULE_43__config_service__["c" /* localeFactory */], deps: [__WEBPACK_IMPORTED_MODULE_43__config_service__["a" /* ConfigService */]] },
                { provide: __WEBPACK_IMPORTED_MODULE_56_angular2_jwt__["AuthHttp"], useFactory: authHttpServiceFactory, deps: [__WEBPACK_IMPORTED_MODULE_3__angular_http__["Http"], __WEBPACK_IMPORTED_MODULE_43__config_service__["a" /* ConfigService */], __WEBPACK_IMPORTED_MODULE_3__angular_http__["RequestOptions"]] },
                { provide: __WEBPACK_IMPORTED_MODULE_55__auth_service__["AuthService"], useFactory: authServiceFactory, deps: [__WEBPACK_IMPORTED_MODULE_43__config_service__["a" /* ConfigService */], __WEBPACK_IMPORTED_MODULE_55__auth_service__["Auth0AuthService"], __WEBPACK_IMPORTED_MODULE_55__auth_service__["NullAuthService"]] }
            ],
            bootstrap: [__WEBPACK_IMPORTED_MODULE_5__app_component__["a" /* AppComponent */]]
        }), 
        __metadata('design:paramtypes', [])
    ], AppModule);
    return AppModule;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/app.module.js.map

/***/ }),

/***/ 1016:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_models_attachment_view_model__ = __webpack_require__(602);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__click_handler_service__ = __webpack_require__(253);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AttachmentPropertyComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var AttachmentPropertyComponent = (function () {
    function AttachmentPropertyComponent(error, urlManager, clickHandlerService, router) {
        var _this = this;
        this.error = error;
        this.urlManager = urlManager;
        this.clickHandlerService = clickHandlerService;
        this.router = router;
        this.doAttachmentClick = function (right) {
            if (_this.attachment.displayInline()) {
                _this.urlManager.setAttachment(_this.attachment.link, _this.clickHandlerService.pane(_this.attachment.onPaneId, right));
            }
            else {
                _this.attachment.downloadFile()
                    .then(function (blob) {
                    if (window.navigator.msSaveBlob) {
                        // internet explorer 
                        window.navigator.msSaveBlob(blob, _this.attachment.title);
                    }
                    else {
                        var burl = URL.createObjectURL(blob);
                        window.open(burl);
                    }
                })
                    .catch(function (reject) { return _this.error.handleError(reject); });
            }
        };
    }
    Object.defineProperty(AttachmentPropertyComponent.prototype, "attachment", {
        get: function () {
            return this.attach;
        },
        set: function (avm) {
            this.attach = avm;
            if (this.attach) {
                this.setup();
            }
        },
        enumerable: true,
        configurable: true
    });
    AttachmentPropertyComponent.prototype.setup = function () {
        this.title = this.attachment.title;
        if (this.attachment.displayInline()) {
            this.attachment.setImage(this);
        }
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__view_models_attachment_view_model__["a" /* AttachmentViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_models_attachment_view_model__["a" /* AttachmentViewModel */]) === 'function' && _a) || Object), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__view_models_attachment_view_model__["a" /* AttachmentViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_models_attachment_view_model__["a" /* AttachmentViewModel */]) === 'function' && _b) || Object])
    ], AttachmentPropertyComponent.prototype, "attachment", null);
    AttachmentPropertyComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-attachment-property',
            template: __webpack_require__(1344),
            styles: [__webpack_require__(1279)]
        }), 
        __metadata('design:paramtypes', [(typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__error_service__["a" /* ErrorService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_5__click_handler_service__["a" /* ClickHandlerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__click_handler_service__["a" /* ClickHandlerService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_3__angular_router__["b" /* Router */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__angular_router__["b" /* Router */]) === 'function' && _f) || Object])
    ], AttachmentPropertyComponent);
    return AttachmentPropertyComponent;
    var _a, _b, _c, _d, _e, _f;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/attachment-property.component.js.map

/***/ }),

/***/ 1017:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_collection_view_model__ = __webpack_require__(366);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__url_manager_service__ = __webpack_require__(22);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CollectionComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var CollectionComponent = (function () {
    function CollectionComponent(urlManager) {
        var _this = this;
        this.urlManager = urlManager;
        this.isSummary = function () { return _this.collection.currentState === __WEBPACK_IMPORTED_MODULE_1__route_data__["b" /* CollectionViewState */].Summary; };
        this.isList = function () { return _this.collection.currentState === __WEBPACK_IMPORTED_MODULE_1__route_data__["b" /* CollectionViewState */].List; };
        this.isTable = function () { return _this.collection.currentState === __WEBPACK_IMPORTED_MODULE_1__route_data__["b" /* CollectionViewState */].Table; };
        this.showActions = function () { return !_this.disableActions() && (_this.isTable() || _this.isList()); };
        this.showSummary = function () { return (_this.mayHaveItems || !_this.disableActions()) && (_this.isList() || _this.isTable()); };
        this.showList = function () { return (_this.mayHaveItems || !_this.disableActions()) && (_this.isTable() || _this.isSummary()); };
        this.showTable = function () { return _this.mayHaveItems && (_this.isList() || _this.isSummary()); };
        this.doSummary = function () { return _this.collection.doSummary(); };
        this.doList = function () { return _this.collection.doList(); };
        this.doTable = function () { return _this.collection.doTable(); };
        this.disableActions = function () { return _this.collection.disableActions(); };
        this.hasTableData = function () { return _this.collection.hasTableData(); };
    }
    Object.defineProperty(CollectionComponent.prototype, "currentState", {
        get: function () {
            return this.collection.currentState;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CollectionComponent.prototype, "state", {
        get: function () {
            return __WEBPACK_IMPORTED_MODULE_1__route_data__["b" /* CollectionViewState */][this.currentState].toString().toLowerCase();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CollectionComponent.prototype, "title", {
        get: function () {
            return this.collection.title;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CollectionComponent.prototype, "details", {
        get: function () {
            return this.collection.details;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CollectionComponent.prototype, "mayHaveItems", {
        get: function () {
            return this.collection.mayHaveItems;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CollectionComponent.prototype, "header", {
        get: function () {
            return this.collection.header;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CollectionComponent.prototype, "items", {
        get: function () {
            return this.collection.items;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(CollectionComponent.prototype, "message", {
        get: function () {
            return this.collection.getMessage();
        },
        enumerable: true,
        configurable: true
    });
    CollectionComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.paneRouteDataSub = this.urlManager.getPaneRouteDataObservable(this.collection.onPaneId)
            .subscribe(function (paneRouteData) {
            if (!paneRouteData.isEqual(_this.lastPaneRouteData)) {
                _this.lastPaneRouteData = paneRouteData;
                _this.currentOid = _this.currentOid || paneRouteData.objectId;
                // ignore if different object
                if (_this.currentOid === paneRouteData.objectId) {
                    _this.collection.reset(paneRouteData, false);
                    _this.collection.resetMessage();
                }
                _this.selectedDialogId = paneRouteData.dialogId;
            }
        });
    };
    CollectionComponent.prototype.ngOnDestroy = function () {
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__view_models_collection_view_model__["a" /* CollectionViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_models_collection_view_model__["a" /* CollectionViewModel */]) === 'function' && _a) || Object)
    ], CollectionComponent.prototype, "collection", void 0);
    CollectionComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-collection',
            template: __webpack_require__(1347),
            styles: [__webpack_require__(1282)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _b) || Object])
    ], CollectionComponent);
    return CollectionComponent;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/collection.component.js.map

/***/ }),

/***/ 1018:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CollectionsComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var CollectionsComponent = (function () {
    function CollectionsComponent() {
    }
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Array)
    ], CollectionsComponent.prototype, "collections", void 0);
    CollectionsComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-collections',
            template: __webpack_require__(1348),
            styles: [__webpack_require__(1283)]
        }), 
        __metadata('design:paramtypes', [])
    ], CollectionsComponent);
    return CollectionsComponent;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/collections.component.js.map

/***/ }),

/***/ 1019:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__rxjs_extensions__ = __webpack_require__(189);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__view_models_list_view_model__ = __webpack_require__(606);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__view_models_menu_view_model__ = __webpack_require__(607);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__view_models_domain_object_view_model__ = __webpack_require__(258);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__view_models_collection_view_model__ = __webpack_require__(366);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_14__parameters_parameters_component__ = __webpack_require__(364);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_15__view_models_helpers_view_models__ = __webpack_require__(41);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return DialogComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
















var DialogComponent = (function () {
    function DialogComponent(viewModelFactory, urlManager, activatedRoute, error, context, configService, formBuilder) {
        var _this = this;
        this.viewModelFactory = viewModelFactory;
        this.urlManager = urlManager;
        this.activatedRoute = activatedRoute;
        this.error = error;
        this.context = context;
        this.configService = configService;
        this.formBuilder = formBuilder;
        this.close = function () {
            if (_this.dialog) {
                _this.dialog.doCloseReplaceHistory();
            }
        };
    }
    Object.defineProperty(DialogComponent.prototype, "parent", {
        get: function () {
            return this.parentViewModel;
        },
        set: function (parent) {
            this.parentViewModel = parent;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(DialogComponent.prototype, "selectedDialogId", {
        get: function () {
            return this.currentDialogId;
        },
        set: function (id) {
            this.currentDialogId = id;
            this.getDialog();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(DialogComponent.prototype, "title", {
        get: function () {
            var dialog = this.dialog;
            return dialog ? dialog.title : "";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(DialogComponent.prototype, "message", {
        get: function () {
            var dialog = this.dialog;
            return dialog ? dialog.getMessage() : "";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(DialogComponent.prototype, "parameters", {
        get: function () {
            var dialog = this.dialog;
            return dialog ? dialog.parameters : "";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(DialogComponent.prototype, "tooltip", {
        get: function () {
            var dialog = this.dialog;
            return dialog ? dialog.tooltip() : "";
        },
        enumerable: true,
        configurable: true
    });
    DialogComponent.prototype.onSubmit = function (right) {
        var _this = this;
        if (this.dialog) {
            __WEBPACK_IMPORTED_MODULE_3_lodash__["forEach"](this.parms, function (p, k) {
                var newValue = _this.form.value[p.id];
                p.setValueFromControl(newValue);
            });
            this.dialog.doInvoke(right);
        }
    };
    DialogComponent.prototype.createForm = function (dialog) {
        var _this = this;
        (_a = __WEBPACK_IMPORTED_MODULE_15__view_models_helpers_view_models__["n" /* createForm */](dialog, this.formBuilder), this.form = _a.form, this.dialog = _a.dialog, this.parms = _a.parms, _a);
        this.form.valueChanges.subscribe(function (data) { return _this.onValueChanged(); });
        var _a;
    };
    DialogComponent.prototype.onValueChanged = function () {
        if (this.dialog) {
            // clear messages if dialog changes 
            this.dialog.resetMessage();
            this.context.clearMessages();
            this.context.clearWarnings();
        }
    };
    DialogComponent.prototype.closeExistingDialog = function () {
        if (this.dialog) {
            this.dialog.doCloseKeepHistory();
            this.context.clearMessages();
            this.context.clearWarnings();
            this.dialog = null;
        }
    };
    DialogComponent.prototype.getDialog = function () {
        // if it's the same dialog just return 
        var _this = this;
        if (this.parent && this.currentDialogId) {
            if (this.dialog && this.dialog.id === this.currentDialogId) {
                return;
            }
            var p = this.parent;
            var action = null;
            var actionViewModel_1 = null;
            if (p instanceof __WEBPACK_IMPORTED_MODULE_10__view_models_menu_view_model__["a" /* MenuViewModel */]) {
                action = p.menuRep.actionMember(this.currentDialogId, this.configService.config.keySeparator);
            }
            if (p instanceof __WEBPACK_IMPORTED_MODULE_11__view_models_domain_object_view_model__["a" /* DomainObjectViewModel */] && p.domainObject.hasActionMember(this.currentDialogId)) {
                action = p.domainObject.actionMember(this.currentDialogId, this.configService.config.keySeparator);
            }
            if (p instanceof __WEBPACK_IMPORTED_MODULE_9__view_models_list_view_model__["a" /* ListViewModel */]) {
                action = p.actionMember(this.currentDialogId);
                actionViewModel_1 = __WEBPACK_IMPORTED_MODULE_3_lodash__["find"](p.actions, function (a) { return a.actionRep.actionId() === _this.currentDialogId; }) || null;
            }
            if (p instanceof __WEBPACK_IMPORTED_MODULE_12__view_models_collection_view_model__["a" /* CollectionViewModel */] && p.hasMatchingLocallyContributedAction(this.currentDialogId)) {
                action = p.actionMember(this.currentDialogId, this.configService.config.keySeparator);
                actionViewModel_1 = __WEBPACK_IMPORTED_MODULE_3_lodash__["find"](p.actions, function (a) { return a.actionRep.actionId() === _this.currentDialogId; }) || null;
            }
            if (action) {
                this.context.getInvokableAction(action)
                    .then(function (details) {
                    // only if we still have a dialog (may have beenn removed while getting invokable action)
                    if (_this.currentDialogId) {
                        // must be a change 
                        _this.closeExistingDialog();
                        var dialogViewModel = _this.viewModelFactory.dialogViewModel(_this.parent.routeData, details, actionViewModel_1, false);
                        _this.createForm(dialogViewModel);
                    }
                })
                    .catch(function (reject) { return _this.error.handleError(reject); });
            }
            else {
                this.closeExistingDialog();
            }
        }
        else {
            this.closeExistingDialog();
        }
    };
    DialogComponent.prototype.focus = function (parms) {
        if (parms && parms.length > 0) {
            __WEBPACK_IMPORTED_MODULE_3_lodash__["some"](parms.toArray(), function (p) { return p.focus(); });
        }
    };
    DialogComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        this.parmComponents.changes.subscribe(function (ql) { return _this.focus(ql); });
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Object), 
        __metadata('design:paramtypes', [Object])
    ], DialogComponent.prototype, "parent", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', String), 
        __metadata('design:paramtypes', [String])
    ], DialogComponent.prototype, "selectedDialogId", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])(__WEBPACK_IMPORTED_MODULE_14__parameters_parameters_component__["a" /* ParametersComponent */]), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _a) || Object)
    ], DialogComponent.prototype, "parmComponents", void 0);
    DialogComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-dialog',
            template: __webpack_require__(1349),
            styles: [__webpack_require__(1284)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_4__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__angular_router__["d" /* ActivatedRoute */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_7__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__error_service__["a" /* ErrorService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_6__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__context_service__["a" /* ContextService */]) === 'function' && _f) || Object, (typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_13__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_13__config_service__["a" /* ConfigService */]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_8__angular_forms__["e" /* FormBuilder */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__angular_forms__["e" /* FormBuilder */]) === 'function' && _h) || Object])
    ], DialogComponent);
    return DialogComponent;
    var _a, _b, _c, _d, _e, _f, _g, _h;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/dialog.component.js.map

/***/ }),

/***/ 1020:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__models__ = __webpack_require__(15);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return GeminiErrorHandler; });



var GeminiErrorHandler = (function () {
    function GeminiErrorHandler() {
    }
    GeminiErrorHandler.prototype.handleError = function (error) {
        // todo make safer 
        var ec = error.context || (error.rejection && error.rejection.context);
        if (ec && ec.injector) {
            var urlManager = ec.injector.get(__WEBPACK_IMPORTED_MODULE_0__url_manager_service__["a" /* UrlManagerService */]);
            var context = ec.injector.get(__WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */]);
            var rp = new __WEBPACK_IMPORTED_MODULE_2__models__["a" /* ErrorWrapper */](__WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].ClientError, __WEBPACK_IMPORTED_MODULE_2__models__["c" /* ClientErrorCode */].SoftwareError, error.message);
            rp.stackTrace = error.stack.split("\n");
            context.setError(rp);
            urlManager.setError(__WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].ClientError, __WEBPACK_IMPORTED_MODULE_2__models__["c" /* ClientErrorCode */].SoftwareError);
        }
        else {
            console.error(error.message + "\n" + error.stack);
            throw error;
        }
    };
    return GeminiErrorHandler;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/error.handler.js.map

/***/ }),

/***/ 1021:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__(24);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_http__ = __webpack_require__(103);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__click_handler_service__ = __webpack_require__(253);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__rep_loader_service__ = __webpack_require__(365);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__auth_service__ = __webpack_require__(147);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return FooterComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};











var FooterComponent = (function () {
    function FooterComponent(authService, urlManager, context, clickHandler, error, repLoader, location, configService, http) {
        var _this = this;
        this.authService = authService;
        this.urlManager = urlManager;
        this.context = context;
        this.clickHandler = clickHandler;
        this.error = error;
        this.repLoader = repLoader;
        this.location = location;
        this.configService = configService;
        this.http = http;
        this.goHome = function (right) {
            var newPane = _this.clickHandler.pane(1, right);
            if (_this.configService.config.leftClickHomeAlwaysGoesToSinglePane && newPane === 1) {
                _this.urlManager.setHomeSinglePane();
            }
            else {
                _this.urlManager.setHome(newPane);
            }
        };
        this.goBack = function () {
            _this.location.back();
        };
        this.goForward = function () {
            _this.location.forward();
        };
        this.swapPanes = function () {
            if (!_this.swapDisabled()) {
                _this.context.swapCurrentObjects();
                _this.urlManager.swapPanes();
            }
        };
        this.swapDisabled = function () {
            return _this.urlManager.isMultiLineDialog() ? true : null;
        };
        this.singlePane = function (right) {
            _this.urlManager.singlePane(_this.clickHandler.pane(1, right));
        };
        this.logOff = function () {
            _this.context.getUser()
                .then(function (u) {
                if (window.confirm(__WEBPACK_IMPORTED_MODULE_8__user_messages__["_102" /* logOffMessage */](u.userName() || "Unknown"))) {
                    var serverLogoffUrl = _this.configService.config.logoffUrl;
                    if (serverLogoffUrl) {
                        var args = {
                            withCredentials: true
                        };
                        _this.http.post(_this.configService.config.logoffUrl, args);
                    }
                    _this.authService.logout();
                }
            })
                .catch(function (reject) { return _this.error.handleError(reject); });
        };
        this.applicationProperties = function () {
            _this.urlManager.applicationProperties();
        };
        this.recent = function (right) {
            _this.urlManager.setRecent(_this.clickHandler.pane(1, right));
        };
        this.cicero = function () {
            _this.urlManager.singlePane(_this.clickHandler.pane(1));
            _this.urlManager.cicero();
        };
    }
    Object.defineProperty(FooterComponent.prototype, "currentCopyColor", {
        get: function () {
            return this.copyViewModel.color;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(FooterComponent.prototype, "currentCopyTitle", {
        get: function () {
            return this.copyViewModel.draggableTitle();
        },
        enumerable: true,
        configurable: true
    });
    FooterComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.context.getUser().then(function (user) { return _this.userName = user.userName(); }).catch(function (reject) { return _this.error.handleError(reject); });
        this.repLoader.loadingCount$.subscribe(function (count) { return _this.loading = count > 0 ? __WEBPACK_IMPORTED_MODULE_8__user_messages__["_103" /* loadingMessage */] : ""; });
        this.context.warning$.subscribe(function (ws) { return _this.warnings = ws; });
        this.context.messages$.subscribe(function (ms) { return _this.messages = ms; });
        this.context.copiedViewModel$.subscribe(function (cvm) { return _this.copyViewModel = cvm; });
    };
    FooterComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-footer',
            template: __webpack_require__(1356),
            styles: [__webpack_require__(1291)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_10__auth_service__["AuthService"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_10__auth_service__["AuthService"]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_5__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__context_service__["a" /* ContextService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_4__click_handler_service__["a" /* ClickHandlerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__click_handler_service__["a" /* ClickHandlerService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_6__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__error_service__["a" /* ErrorService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_7__rep_loader_service__["a" /* RepLoaderService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__rep_loader_service__["a" /* RepLoaderService */]) === 'function' && _f) || Object, (typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_1__angular_common__["a" /* Location */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_common__["a" /* Location */]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */]) === 'function' && _h) || Object, (typeof (_j = typeof __WEBPACK_IMPORTED_MODULE_2__angular_http__["Http"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__angular_http__["Http"]) === 'function' && _j) || Object])
    ], FooterComponent);
    return FooterComponent;
    var _a, _b, _c, _d, _e, _f, _g, _h, _j;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/footer.component.js.map

/***/ }),

/***/ 1022:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_models_field_view_model__ = __webpack_require__(259);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return GeminiBooleanDirective; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var GeminiBooleanDirective = (function () {
    function GeminiBooleanDirective(el, renderer) {
        var _this = this;
        this.el = el;
        this.renderer = renderer;
        this.render = function () {
            var nativeEl = _this.el.nativeElement;
            switch (_this.model.value) {
                case true:
                    _this.renderer.setElementProperty(nativeEl, "indeterminate", false);
                    _this.renderer.setElementProperty(nativeEl, "checked", true);
                    break;
                case false:
                    _this.renderer.setElementProperty(nativeEl, "indeterminate", false);
                    _this.renderer.setElementProperty(nativeEl, "checked", false);
                    break;
                default:
                    _this.renderer.setElementProperty(nativeEl, "indeterminate", true);
            }
        };
        this.triStateClick = function () {
            switch (_this.model.value) {
                case false:
                    _this.model.value = true;
                    break;
                case true:
                    _this.model.value = null;
                    break;
                default:
                    _this.model.value = false;
            }
            _this.render();
        };
        this.twoStateClick = function () {
            _this.model.value = !_this.model.value;
            _this.render();
        };
    }
    Object.defineProperty(GeminiBooleanDirective.prototype, "viewModel", {
        set: function (vm) {
            this.model = vm;
            this.render();
        },
        enumerable: true,
        configurable: true
    });
    GeminiBooleanDirective.prototype.onClick = function () {
        var click = this.model.optional ? this.triStateClick : this.twoStateClick;
        click();
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])('geminiBoolean'), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__view_models_field_view_model__["a" /* FieldViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_models_field_view_model__["a" /* FieldViewModel */]) === 'function' && _a) || Object), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__view_models_field_view_model__["a" /* FieldViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_models_field_view_model__["a" /* FieldViewModel */]) === 'function' && _b) || Object])
    ], GeminiBooleanDirective.prototype, "viewModel", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('click'), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', []), 
        __metadata('design:returntype', void 0)
    ], GeminiBooleanDirective.prototype, "onClick", null);
    GeminiBooleanDirective = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({ selector: '[geminiBoolean]' }), 
        __metadata('design:paramtypes', [(typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"]) === 'function' && _d) || Object])
    ], GeminiBooleanDirective);
    return GeminiBooleanDirective;
    var _a, _b, _c, _d;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/gemini-boolean.directive.js.map

/***/ }),

/***/ 1023:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_field_view_model__ = __webpack_require__(259);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return GeminiClearDirective; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var GeminiClearDirective = (function () {
    function GeminiClearDirective(el, renderer) {
        this.el = el;
        this.renderer = renderer;
        this.nativeEl = this.el.nativeElement;
    }
    Object.defineProperty(GeminiClearDirective.prototype, "viewModel", {
        set: function (vm) {
            this.model = vm;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(GeminiClearDirective.prototype, "form", {
        set: function (fm) {
            this.formGroup = fm;
        },
        enumerable: true,
        configurable: true
    });
    GeminiClearDirective.prototype.ngOnInit = function () {
        var _this = this;
        this.onChange();
        this.formGroup.controls[this.model.id].valueChanges.subscribe(function (data) { return _this.onChange(); });
    };
    // not need the ngClass directive on element even though it doesn't do anything 
    // otherwise we lose all the classes added here 
    GeminiClearDirective.prototype.onChange = function () {
        this.nativeEl.classList.add("ng-clearable");
        if (this.formGroup.controls[this.model.id].value) {
            this.nativeEl.classList.add("ng-x");
        }
        else {
            this.nativeEl.classList.remove("ng-x");
        }
    };
    GeminiClearDirective.prototype.onMouseMove = function (event) {
        if (this.nativeEl.classList.contains("ng-x")) {
            var onX = this.nativeEl.offsetWidth - 18 < event.clientX - this.nativeEl.getBoundingClientRect().left;
            if (onX) {
                this.nativeEl.classList.add("ng-onX");
            }
            else {
                this.nativeEl.classList.remove("ng-onX");
            }
        }
    };
    GeminiClearDirective.prototype.onClick = function (event) {
        if (this.nativeEl.classList.contains("ng-onX")) {
            event.preventDefault();
            this.nativeEl.classList.remove("ng-x");
            this.nativeEl.classList.remove("ng-onX");
            this.model.clear();
            this.formGroup.controls[this.model.id].reset("");
        }
    };
    GeminiClearDirective.prototype.click = function (event) {
        this.onClick(event);
    };
    GeminiClearDirective.prototype.touchstart = function (event) {
        this.onClick(event);
    };
    GeminiClearDirective.prototype.mousemove = function (event) {
        this.onMouseMove(event);
    };
    GeminiClearDirective.prototype.input = function () {
        this.onChange();
    };
    GeminiClearDirective.prototype.change = function () {
        this.onChange();
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])('geminiClear'), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__view_models_field_view_model__["a" /* FieldViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_models_field_view_model__["a" /* FieldViewModel */]) === 'function' && _a) || Object), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__view_models_field_view_model__["a" /* FieldViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_models_field_view_model__["a" /* FieldViewModel */]) === 'function' && _b) || Object])
    ], GeminiClearDirective.prototype, "viewModel", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_1__angular_forms__["f" /* FormGroup */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_forms__["f" /* FormGroup */]) === 'function' && _c) || Object), 
        __metadata('design:paramtypes', [(typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_1__angular_forms__["f" /* FormGroup */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_forms__["f" /* FormGroup */]) === 'function' && _d) || Object])
    ], GeminiClearDirective.prototype, "form", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])("click", ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], GeminiClearDirective.prototype, "click", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])("touchstart", ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], GeminiClearDirective.prototype, "touchstart", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])("mousemove", ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], GeminiClearDirective.prototype, "mousemove", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])("input"), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', []), 
        __metadata('design:returntype', void 0)
    ], GeminiClearDirective.prototype, "input", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])("change"), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', []), 
        __metadata('design:returntype', void 0)
    ], GeminiClearDirective.prototype, "change", null);
    GeminiClearDirective = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({ selector: '[geminiClear]' }), 
        __metadata('design:paramtypes', [(typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"]) === 'function' && _f) || Object])
    ], GeminiClearDirective);
    return GeminiClearDirective;
    var _a, _b, _c, _d, _e, _f;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/gemini-clear.directive.js.map

/***/ }),

/***/ 1024:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return GeminiClickDirective; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var GeminiClickDirective = (function () {
    function GeminiClickDirective(el) {
        this.leftClick = new __WEBPACK_IMPORTED_MODULE_0__angular_core__["EventEmitter"]();
        this.rightClick = new __WEBPACK_IMPORTED_MODULE_0__angular_core__["EventEmitter"]();
        this.el = el.nativeElement;
    }
    GeminiClickDirective.prototype.onClick = function () {
        this.leftClick.emit("event");
        return false;
    };
    GeminiClickDirective.prototype.handleKey = function (event) {
        var enterKeyCode = 13;
        if (event.which === enterKeyCode) {
            var trigger = event.shiftKey ? this.rightClick : this.leftClick;
            trigger.emit("event");
            return false;
        }
        return true;
    };
    GeminiClickDirective.prototype.onEnter = function (event) {
        return this.handleKey(event);
    };
    GeminiClickDirective.prototype.onEnter1 = function (event) {
        return this.handleKey(event);
    };
    GeminiClickDirective.prototype.onContextMenu = function () {
        this.rightClick.emit("event");
        return false;
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Output"])(), 
        __metadata('design:type', Object)
    ], GeminiClickDirective.prototype, "leftClick", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Output"])(), 
        __metadata('design:type', Object)
    ], GeminiClickDirective.prototype, "rightClick", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('click'), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', []), 
        __metadata('design:returntype', void 0)
    ], GeminiClickDirective.prototype, "onClick", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('keydown', ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], GeminiClickDirective.prototype, "onEnter", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('keypress', ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], GeminiClickDirective.prototype, "onEnter1", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('contextmenu'), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', []), 
        __metadata('design:returntype', void 0)
    ], GeminiClickDirective.prototype, "onContextMenu", null);
    GeminiClickDirective = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Directive"])({ selector: '[geminiClick]' }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]) === 'function' && _a) || Object])
    ], GeminiClickDirective);
    return GeminiClickDirective;
    var _a;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/gemini-click.directive.js.map

/***/ }),

/***/ 1025:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__route_data__ = __webpack_require__(37);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return HeaderComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var HeaderComponent = (function () {
    function HeaderComponent() {
        var _this = this;
        this.allSelected = function () { return _this.collection.allSelected(); };
        this.selectAll = function () { return _this.collection.selectAll(); };
        this.itemId = function () { return ("" + _this.collection.name + _this.collection.onPaneId + "-all"); };
        this.showAllCheckbox = function () { return !(_this.collection.disableActions() || _this.noItems()); };
    }
    HeaderComponent.prototype.noItems = function () {
        return !this.collection.items || this.collection.items.length === 0;
    };
    Object.defineProperty(HeaderComponent.prototype, "header", {
        get: function () {
            return this.state === __WEBPACK_IMPORTED_MODULE_1__route_data__["b" /* CollectionViewState */].Table ? this.collection.header : null;
        },
        enumerable: true,
        configurable: true
    });
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Object)
    ], HeaderComponent.prototype, "collection", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__route_data__["b" /* CollectionViewState */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__route_data__["b" /* CollectionViewState */]) === 'function' && _a) || Object)
    ], HeaderComponent.prototype, "state", void 0);
    HeaderComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: '[nof-header]',
            template: __webpack_require__(1357),
            styles: [__webpack_require__(1292)]
        }), 
        __metadata('design:paramtypes', [])
    ], HeaderComponent);
    return HeaderComponent;
    var _a;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/header.component.js.map

/***/ }),

/***/ 1026:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__auth_service__ = __webpack_require__(147);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__config_service__ = __webpack_require__(21);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return LoginComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var LoginComponent = (function () {
    function LoginComponent(auth, configService) {
        this.auth = auth;
        this.configService = configService;
    }
    LoginComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-login',
            template: __webpack_require__(1360),
            styles: [__webpack_require__(1295)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__auth_service__["AuthService"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__auth_service__["AuthService"]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__config_service__["a" /* ConfigService */]) === 'function' && _b) || Object])
    ], LoginComponent);
    return LoginComponent;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/login.component.js.map

/***/ }),

/***/ 1027:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__action_action_component__ = __webpack_require__(122);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__url_manager_service__ = __webpack_require__(22);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return MenuBarComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var MenuBarComponent = (function () {
    function MenuBarComponent(urlManager) {
        this.urlManager = urlManager;
    }
    Object.defineProperty(MenuBarComponent.prototype, "menus", {
        set: function (links) {
            var _this = this;
            this.actions = _.map(links, function (link) { return ({
                value: link.title,
                doClick: function () {
                    var menuId = link.link.rel().parms[0].value;
                    _this.urlManager.setMenu(menuId, link.paneId);
                },
                doRightClick: function () { },
                show: function () { return true; },
                disabled: function () { return null; },
                tempDisabled: function () { return false; },
                title: function () { return link.title; },
                accesskey: null
            }); });
        },
        enumerable: true,
        configurable: true
    });
    MenuBarComponent.prototype.focusOnFirstMenu = function (menusList) {
        if (menusList) {
            // until first element returns true
            _.some(menusList.toArray(), function (i) { return i.focus(); });
        }
    };
    MenuBarComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        this.focusOnFirstMenu(this.actionComponents);
        this.actionComponents.changes.subscribe(function (ql) { return _this.focusOnFirstMenu(ql); });
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Array), 
        __metadata('design:paramtypes', [Array])
    ], MenuBarComponent.prototype, "menus", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])(__WEBPACK_IMPORTED_MODULE_1__action_action_component__["a" /* ActionComponent */]), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _a) || Object)
    ], MenuBarComponent.prototype, "actionComponents", void 0);
    MenuBarComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-menu-bar',
            template: __webpack_require__(1361),
            styles: [__webpack_require__(1296)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _b) || Object])
    ], MenuBarComponent);
    return MenuBarComponent;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/menu-bar.component.js.map

/***/ }),

/***/ 1028:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_models_item_view_model__ = __webpack_require__(367);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_helpers_view_models__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__view_models_recent_item_view_model__ = __webpack_require__(608);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RowComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var RowComponent = (function () {
    function RowComponent(context, renderer, element) {
        var _this = this;
        this.context = context;
        this.renderer = renderer;
        this.element = element;
        this.tableTitle = function () { return _this.item.tableRowViewModel ? _this.item.tableRowViewModel.title : _this.title; };
        this.hasTableTitle = function () { return (_this.item.tableRowViewModel && _this.item.tableRowViewModel.showTitle) || (_this.item instanceof __WEBPACK_IMPORTED_MODULE_4__view_models_recent_item_view_model__["a" /* RecentItemViewModel */] && _this.item.title); };
        this.tableProperties = function () { return _this.item.tableRowViewModel && _this.item.tableRowViewModel.properties; };
        this.propertyType = function (property) { return property.type; };
        this.propertyValue = function (property) { return property.value; };
        this.propertyFormattedValue = function (property) { return property.formattedValue; };
        this.propertyReturnType = function (property) { return property.returnType; };
        this.doClick = function (right) { return _this.item.doClick(right); };
    }
    Object.defineProperty(RowComponent.prototype, "id", {
        get: function () {
            return "" + (this.item.id || "item") + this.item.paneId + "-" + this.index;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RowComponent.prototype, "color", {
        get: function () {
            return this.item.color;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RowComponent.prototype, "selected", {
        get: function () {
            return this.item.selected;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RowComponent.prototype, "title", {
        get: function () {
            return this.item.title;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RowComponent.prototype, "friendlyName", {
        get: function () {
            return this.item instanceof __WEBPACK_IMPORTED_MODULE_4__view_models_recent_item_view_model__["a" /* RecentItemViewModel */] ? this.item.friendlyName : "";
        },
        enumerable: true,
        configurable: true
    });
    RowComponent.prototype.copy = function (event, item) {
        __WEBPACK_IMPORTED_MODULE_2__view_models_helpers_view_models__["l" /* copy */](event, item, this.context);
    };
    RowComponent.prototype.focus = function () {
        return !!this.element && __WEBPACK_IMPORTED_MODULE_2__view_models_helpers_view_models__["m" /* focus */](this.renderer, this.element);
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__view_models_item_view_model__["a" /* ItemViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_models_item_view_model__["a" /* ItemViewModel */]) === 'function' && _a) || Object)
    ], RowComponent.prototype, "item", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Number)
    ], RowComponent.prototype, "index", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Boolean)
    ], RowComponent.prototype, "withCheckbox", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Boolean)
    ], RowComponent.prototype, "isTable", void 0);
    RowComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: '[nof-row]',
            template: __webpack_require__(1367),
            styles: [__webpack_require__(1302)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]) === 'function' && _d) || Object])
    ], RowComponent);
    return RowComponent;
    var _a, _b, _c, _d;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/row.component.js.map

/***/ }),

/***/ 1029:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__rxjs_extensions__ = __webpack_require__(189);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return SimpleLruCache; });

var LruNode = (function () {
    function LruNode(key, value) {
        this.key = key;
        this.value = value;
        this.next = null;
        this.previous = null;
    }
    return LruNode;
}());
var SimpleLruCache = (function () {
    function SimpleLruCache(depth) {
        this.depth = depth;
        this.cache = {};
        this.count = 0;
        this.head = null;
        this.tail = null;
    }
    SimpleLruCache.prototype.unlinkNode = function (node) {
        var nodePrevious = node.previous;
        var nodeNext = node.next;
        if (nodePrevious) {
            nodePrevious.next = nodeNext;
        }
        else {
            // was head
            this.head = nodeNext;
        }
        if (nodeNext) {
            nodeNext.previous = nodePrevious;
        }
        else {
            //was tail
            this.tail = nodePrevious;
        }
        this.count--;
    };
    SimpleLruCache.prototype.moveNodeToHead = function (node) {
        var existingHead = this.head;
        node.previous = null;
        node.next = existingHead;
        if (existingHead) {
            existingHead.previous = node;
        }
        else {
            // no existing head so this is also tail
            this.tail = node;
        }
        this.head = node;
        this.count++;
    };
    SimpleLruCache.prototype.add = function (key, value) {
        if (this.cache[key]) {
            this.updateExistingEntry(key, value);
        }
        else {
            this.addNewEntry(key, value);
        }
    };
    SimpleLruCache.prototype.remove = function (key) {
        var node = this.cache[key];
        if (node) {
            this.unlinkNode(node);
            delete this.cache[key];
        }
    };
    SimpleLruCache.prototype.removeAll = function () {
        this.head = this.tail = null;
        this.cache = {};
        this.count = 0;
    };
    SimpleLruCache.prototype.getNode = function (key) {
        var node = this.cache[key];
        if (node) {
            this.unlinkNode(node);
            this.moveNodeToHead(node);
        }
        return node;
    };
    SimpleLruCache.prototype.get = function (key) {
        var node = this.getNode(key);
        return node ? node.value : null;
    };
    SimpleLruCache.prototype.updateExistingEntry = function (key, value) {
        var node = this.getNode(key);
        node.value = value;
    };
    SimpleLruCache.prototype.addNewEntry = function (key, value) {
        var newNode = new LruNode(key, value);
        this.cache[key] = newNode;
        this.moveNodeToHead(newNode);
        this.trimCache();
    };
    SimpleLruCache.prototype.trimCache = function () {
        while (this.count > this.depth) {
            var tail = this.tail;
            if (tail) {
                this.unlinkNode(tail);
                delete this.cache[tail.key];
            }
        }
    };
    return SimpleLruCache;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/simple-lru-cache.js.map

/***/ }),

/***/ 1030:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__ = __webpack_require__(41);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ActionViewModel; });





var ActionViewModel = (function () {
    function ActionViewModel(viewModelFactory, context, urlManager, error, clickHandler, actionRep, vm, routeData) {
        var _this = this;
        this.viewModelFactory = viewModelFactory;
        this.context = context;
        this.urlManager = urlManager;
        this.error = error;
        this.clickHandler = clickHandler;
        this.actionRep = actionRep;
        this.vm = vm;
        this.routeData = routeData;
        this.gotoResult = true;
        // form actions should never show dialogs
        this.showDialog = function () { return _this.actionRep.extensions().hasParams() && (_this.routeData.interactionMode !== __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Form); };
        this.incrementPendingPotentAction = function () {
            __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__["i" /* incrementPendingPotentAction */](_this.context, _this.invokableActionRep, _this.paneId);
        };
        this.decrementPendingPotentAction = function () {
            __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__["j" /* decrementPendingPotentAction */](_this.context, _this.invokableActionRep, _this.paneId);
        };
        this.invokeWithDialog = function (right) {
            // clear any previous dialog so we don't pick up values from it
            _this.context.clearDialogCachedValues(_this.paneId);
            _this.urlManager.setDialogOrMultiLineDialog(_this.actionRep, _this.paneId);
        };
        this.invokeWithoutDialogWithParameters = function (parameters, right) {
            _this.incrementPendingPotentAction();
            return parameters
                .then(function (pps) {
                return _this.execute(pps, right);
            })
                .then(function (actionResult) {
                _this.decrementPendingPotentAction();
                return actionResult;
            })
                .catch(function (reject) {
                _this.decrementPendingPotentAction();
                var display = function (em) { return _this.vm.setMessage(em.invalidReason() || em.warningMessage); };
                _this.error.handleErrorAndDisplayMessages(reject, display);
            });
        };
        this.invokeWithoutDialog = function (right) {
            _this.invokeWithoutDialogWithParameters(_this.parameters(), right).then(function (actionResult) {
                // if expect result and no warning from server generate one here
                if (actionResult.shouldExpectResult() && !actionResult.warningsOrMessages()) {
                    _this.context.broadcastWarning(__WEBPACK_IMPORTED_MODULE_2__user_messages__["_86" /* noResultMessage */]);
                }
            });
        };
        // open dialog on current pane always - invoke action goes to pane indicated by click
        // note this can be modified by decorators 
        this.doInvoke = this.showDialog() ? this.invokeWithDialog : this.invokeWithoutDialog;
        // note this is modified by decorators 
        this.execute = function (pps, right) {
            var parmMap = __WEBPACK_IMPORTED_MODULE_3_lodash__["zipObject"](__WEBPACK_IMPORTED_MODULE_3_lodash__["map"](pps, function (p) { return p.id; }), __WEBPACK_IMPORTED_MODULE_3_lodash__["map"](pps, function (p) { return p.getValue(); }));
            __WEBPACK_IMPORTED_MODULE_3_lodash__["forEach"](pps, function (p) { return _this.urlManager.setParameterValue(_this.actionRep.actionId(), p.parameterRep, p.getValue(), _this.paneId); });
            return _this.getInvokable()
                .then(function (details) { return _this.context.invokeAction(details, parmMap, _this.paneId, _this.clickHandler.pane(_this.paneId, right), _this.gotoResult); });
        };
        this.disabled = function () { return !!_this.actionRep.disabledReason(); };
        this.tempDisabled = function () { return _this.invokableActionRep &&
            _this.invokableActionRep.isPotent() &&
            _this.context.isPendingPotentActionOrReload(_this.paneId); };
        this.parameters = function () {
            if (_this.invokableActionRep) {
                var pps = _this.getParameters(_this.invokableActionRep);
                return Promise.resolve(pps);
            }
            return _this.context.getInvokableAction(_this.actionRep)
                .then(function (details) {
                _this.invokableActionRep = details;
                return _this.getParameters(details);
            });
        };
        this.makeInvokable = function (details) { return _this.invokableActionRep = details; };
        if (actionRep instanceof __WEBPACK_IMPORTED_MODULE_1__models__["B" /* ActionRepresentation */] || actionRep instanceof __WEBPACK_IMPORTED_MODULE_1__models__["C" /* InvokableActionMember */]) {
            this.invokableActionRep = actionRep;
        }
        this.paneId = routeData.paneId;
        this.title = actionRep.extensions().friendlyName();
        this.presentationHint = actionRep.extensions().presentationHint();
        this.menuPath = actionRep.extensions().menuPath() || "";
        this.description = this.disabled() ? actionRep.disabledReason() : actionRep.extensions().description();
    }
    ActionViewModel.prototype.getInvokable = function () {
        var _this = this;
        if (this.invokableActionRep) {
            return Promise.resolve(this.invokableActionRep);
        }
        return this.context.getInvokableAction(this.actionRep)
            .then(function (details) {
            _this.invokableActionRep = details;
            return details;
        });
    };
    ActionViewModel.prototype.getParameters = function (invokableAction) {
        var _this = this;
        var parameters = __WEBPACK_IMPORTED_MODULE_3_lodash__["pickBy"](invokableAction.parameters(), function (p) { return !p.isCollectionContributed(); });
        var parms = this.routeData.actionParams;
        return __WEBPACK_IMPORTED_MODULE_3_lodash__["map"](parameters, function (parm) { return _this.viewModelFactory.parameterViewModel(parm, parms[parm.id()], _this.paneId); });
    };
    return ActionViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/action-view-model.js.map

/***/ }),

/***/ 1031:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__constants__ = __webpack_require__(188);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ApplicationPropertiesViewModel; });

var ApplicationPropertiesViewModel = (function () {
    function ApplicationPropertiesViewModel(context, error, configService) {
        this.context = context;
        this.error = error;
        this.configService = configService;
        this.setUp();
    }
    ApplicationPropertiesViewModel.prototype.setUp = function () {
        var _this = this;
        this.context.getUser().
            then(function (u) { return _this.user = u.wrapped(); }).
            catch(function (reject) { return _this.error.handleError(reject); });
        this.context.getVersion().
            then(function (v) { return _this.serverVersion = v.wrapped(); }).
            catch(function (reject) { return _this.error.handleError(reject); });
        this.serverUrl = this.configService.config.appPath;
        this.clientVersion = __WEBPACK_IMPORTED_MODULE_0__constants__["g" /* clientVersion */];
        this.applicationName = this.configService.config.applicationName;
    };
    return ApplicationPropertiesViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/application-properties-view-model.js.map

/***/ }),

/***/ 1032:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_Subject__ = __webpack_require__(20);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_rxjs_Subject__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CiceroViewModel; });


var CiceroViewModel = (function () {
    function CiceroViewModel() {
        this.alert = ""; //Alert is appended before the output
        this.outputSource = new __WEBPACK_IMPORTED_MODULE_1_rxjs_Subject__["Subject"]();
        this.output$ = this.outputSource.asObservable();
    }
    CiceroViewModel.prototype.clearInput = function () {
        this.input = null;
    };
    ;
    CiceroViewModel.prototype.setOutputSource = function (text) {
        this.outputSource.next(__WEBPACK_IMPORTED_MODULE_0__models__["L" /* withUndefined */](text));
    };
    CiceroViewModel.prototype.outputMessageThenClearIt = function () {
        this.setOutputSource(this.message);
        this.message = null;
    };
    CiceroViewModel.prototype.clearInputRenderOutputAndAppendAlertIfAny = function (output) {
        this.clearInput();
        var text = output;
        if (this.alert) {
            text += this.alert;
            this.alert = "";
        }
        this.setOutputSource(text);
    };
    return CiceroViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/cicero-view-model.js.map

/***/ }),

/***/ 1033:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ErrorViewModel; });

var ErrorViewModel = (function () {
    function ErrorViewModel(error) {
        this.originalError = error;
        if (error) {
            this.title = error.title;
            this.description = error.description;
            this.errorCode = error.errorCode;
            this.message = error.message;
            var stackTrace = error.stackTrace;
            this.stackTrace = stackTrace && stackTrace.length !== 0 ? stackTrace : null;
            this.isConcurrencyError =
                error.category === __WEBPACK_IMPORTED_MODULE_0__models__["b" /* ErrorCategory */].HttpClientError &&
                    error.httpErrorCode === __WEBPACK_IMPORTED_MODULE_0__models__["q" /* HttpStatusCode */].PreconditionFailed;
        }
        this.description = this.description || "No description available";
        this.errorCode = this.errorCode || "No code available";
        this.message = this.message || "No message available";
        this.stackTrace = this.stackTrace || ["No stack trace available"];
    }
    return ErrorViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/error-view-model.js.map

/***/ }),

/***/ 1034:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return MenuItemViewModel; });
var MenuItemViewModel = (function () {
    function MenuItemViewModel(name, actions, menuItems) {
        var _this = this;
        this.name = name;
        this.actions = actions;
        this.menuItems = menuItems;
        this.toggleCollapsed = function () { return _this.navCollapsed = !_this.navCollapsed; };
        this.navCollapsed = !!this.name;
    }
    return MenuItemViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/menu-item-view-model.js.map

/***/ }),

/***/ 1035:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return MenusViewModel; });

var MenusViewModel = (function () {
    function MenusViewModel(viewModelFactory, menusRep, onPaneId) {
        var _this = this;
        this.viewModelFactory = viewModelFactory;
        this.menusRep = menusRep;
        this.items = __WEBPACK_IMPORTED_MODULE_0_lodash__["map"](this.menusRep.value(), function (link) { return _this.viewModelFactory.linkViewModel(link, onPaneId); });
    }
    return MenusViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/menus-view-model.js.map

/***/ }),

/***/ 1036:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return MultiLineDialogViewModel; });


var MultiLineDialogViewModel = (function () {
    function MultiLineDialogViewModel(color, context, viewModelFactory, urlManager, error, routeData, action, holder) {
        var _this = this;
        this.color = color;
        this.context = context;
        this.viewModelFactory = viewModelFactory;
        this.urlManager = urlManager;
        this.error = error;
        this.routeData = routeData;
        this.action = action;
        this.createRow = function (i) {
            var dialogViewModel = _this.viewModelFactory.dialogViewModel(_this.routeData, _this.action, null, true);
            dialogViewModel.actionViewModel.gotoResult = false;
            dialogViewModel.doCloseKeepHistory = function () { return dialogViewModel.submitted = true; };
            dialogViewModel.doCloseReplaceHistory = function () { return dialogViewModel.submitted = true; };
            dialogViewModel.doCompleteButLeaveOpen = function () { return dialogViewModel.submitted = true; };
            dialogViewModel.parameters.forEach(function (p) { return p.setAsRow(i); });
            return dialogViewModel;
        };
        this.objectFriendlyName = "";
        this.objectTitle = "";
        this.header = function () { return _this.dialogs.length === 0 ? [] : __WEBPACK_IMPORTED_MODULE_1_lodash__["map"](_this.dialogs[0].parameters, function (p) { return p.title; }); };
        this.invokeAndAdd = function (index) {
            _this.dialogs[index].doInvoke();
            _this.context.clearDialogCachedValues();
            return _this.add(index);
        };
        this.pushNewDialog = function () { return _this.dialogs.push(_this.createRow(_this.dialogs.length)) - 1; };
        this.add = function (index) {
            if (index === _this.dialogs.length - 1) {
                // if this is last dialog always add another
                return _this.pushNewDialog();
            }
            else if (__WEBPACK_IMPORTED_MODULE_1_lodash__["takeRight"](_this.dialogs)[0].submitted) {
                // if the last dialog is submitted add another 
                return _this.pushNewDialog();
            }
            return 0;
        };
        this.submittedCount = function () { return __WEBPACK_IMPORTED_MODULE_1_lodash__["filter"](_this.dialogs, function (d) { return d.submitted; }).length; };
        if (holder instanceof __WEBPACK_IMPORTED_MODULE_0__models__["o" /* DomainObjectRepresentation */]) {
            this.objectTitle = holder.title();
            this.objectFriendlyName = holder.extensions().friendlyName();
        }
        var initialCount = action.extensions().multipleLines() || 1;
        this.dialogs = __WEBPACK_IMPORTED_MODULE_1_lodash__["map"](__WEBPACK_IMPORTED_MODULE_1_lodash__["range"](initialCount), function (i) { return _this.createRow(i); });
        this.title = this.dialogs[0].title;
        this.action.parent.etagDigest = "*";
    }
    return MultiLineDialogViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/multi-line-dialog-view-model.js.map

/***/ }),

/***/ 1037:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RecentItemsViewModel; });

var RecentItemsViewModel = (function () {
    function RecentItemsViewModel(viewModelFactory, context, urlManager, onPaneId) {
        var _this = this;
        this.context = context;
        this.urlManager = urlManager;
        this.clear = function () {
            _this.context.clearRecentlyViewed();
            _this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
        };
        var items = __WEBPACK_IMPORTED_MODULE_0_lodash__["map"](this.context.getRecentlyViewed(), function (o, i) { return ({ obj: o, link: o.updateSelfLinkWithTitle(), index: i }); });
        this.items = __WEBPACK_IMPORTED_MODULE_0_lodash__["map"](items, function (i) { return viewModelFactory.recentItemViewModel(i.obj, i.link, onPaneId, false, i.index); });
    }
    return RecentItemsViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/recent-items-view-model.js.map

/***/ }),

/***/ 1038:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__choice_view_model__ = __webpack_require__(106);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__helpers_view_models__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TableRowColumnViewModel; });





var TableRowColumnViewModel = (function () {
    function TableRowColumnViewModel(id, propertyRep, mask) {
        this.id = id;
        if (propertyRep && mask) {
            this.title = propertyRep.extensions().friendlyName();
            if (propertyRep instanceof __WEBPACK_IMPORTED_MODULE_2__models__["A" /* CollectionMember */]) {
                var size = propertyRep.size();
                this.formattedValue = __WEBPACK_IMPORTED_MODULE_3__helpers_view_models__["b" /* getCollectionDetails */](size);
                this.value = "";
                this.type = "scalar";
                this.returnType = "string";
            }
            if (propertyRep instanceof __WEBPACK_IMPORTED_MODULE_2__models__["l" /* PropertyMember */]) {
                var isPassword = propertyRep.extensions().dataType() === "password";
                var value = propertyRep.value();
                this.returnType = propertyRep.extensions().returnType();
                if (propertyRep.isScalar()) {
                    this.type = "scalar";
                    __WEBPACK_IMPORTED_MODULE_3__helpers_view_models__["k" /* setScalarValueInView */](this, propertyRep, value);
                    var remoteMask = propertyRep.extensions().mask();
                    var localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
                    if (propertyRep.entryType() === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].Choices) {
                        var currentChoice_1 = new __WEBPACK_IMPORTED_MODULE_0__choice_view_model__["a" /* ChoiceViewModel */](value, id);
                        var choicesMap = propertyRep.choices();
                        var choices = __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](choicesMap, function (v, n) { return new __WEBPACK_IMPORTED_MODULE_0__choice_view_model__["a" /* ChoiceViewModel */](v, id, n); });
                        var choice = __WEBPACK_IMPORTED_MODULE_4_lodash__["find"](choices, function (c) { return c.valuesEqual(currentChoice_1); });
                        if (choice) {
                            this.value = choice.name;
                            this.formattedValue = choice.name;
                        }
                    }
                    else if (isPassword) {
                        this.formattedValue = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_93" /* obscuredText */];
                    }
                    else {
                        this.formattedValue = localFilter.filter(this.value);
                    }
                }
                else {
                    // is reference   
                    this.type = "ref";
                    this.formattedValue = value.isNull() ? "" : value.toString();
                }
            }
        }
        else {
            this.type = "scalar";
            this.value = "";
            this.formattedValue = "";
            this.title = "";
        }
    }
    return TableRowColumnViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/table-row-column-view-model.js.map

/***/ }),

/***/ 1039:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TableRowViewModel; });

var TableRowViewModel = (function () {
    function TableRowViewModel(viewModelFactory, properties, paneId, title) {
        var _this = this;
        this.viewModelFactory = viewModelFactory;
        this.paneId = paneId;
        this.title = title;
        this.showTitle = false;
        this.getPlaceHolderTableRowColumnViewModel = function (id) { return _this.viewModelFactory.propertyTableViewModel(id); }; // no property so place holder for column
        this.conformColumns = function (showTitle, columns) {
            _this.showTitle = showTitle;
            if (columns) {
                _this.properties = __WEBPACK_IMPORTED_MODULE_0_lodash__["map"](columns, function (c) { return __WEBPACK_IMPORTED_MODULE_0_lodash__["find"](_this.properties, function (tp) { return tp.id === c; }) || _this.getPlaceHolderTableRowColumnViewModel(c); });
            }
        };
        this.properties = __WEBPACK_IMPORTED_MODULE_0_lodash__["map"](properties, function (property, id) { return viewModelFactory.propertyTableViewModel(id, property); });
    }
    return TableRowViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/table-row-view-model.js.map

/***/ }),

/***/ 1040:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_models_parameter_view_model__ = __webpack_require__(368);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_dialog_view_model__ = __webpack_require__(257);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ViewParameterComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var ViewParameterComponent = (function () {
    function ViewParameterComponent() {
    }
    Object.defineProperty(ViewParameterComponent.prototype, "title", {
        get: function () {
            return this.parameter.title;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "parameterPaneId", {
        get: function () {
            return this.parameter.paneArgId;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "parameterType", {
        get: function () {
            return this.parameter.type;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "parameterReturnType", {
        get: function () {
            return this.parameter.returnType;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "formattedValue", {
        get: function () {
            return this.parameter.formattedValue;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "value", {
        get: function () {
            return this.parameter.value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "format", {
        get: function () {
            return this.parameter.format;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "isMultiline", {
        get: function () {
            return !(this.parameter.multipleLines === 1);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "multilineHeight", {
        get: function () {
            return this.parameter.multipleLines * 20 + "px";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewParameterComponent.prototype, "color", {
        get: function () {
            return this.parameter.color;
        },
        enumerable: true,
        configurable: true
    });
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__view_models_dialog_view_model__["a" /* DialogViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_models_dialog_view_model__["a" /* DialogViewModel */]) === 'function' && _a) || Object)
    ], ViewParameterComponent.prototype, "parent", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__view_models_parameter_view_model__["a" /* ParameterViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_models_parameter_view_model__["a" /* ParameterViewModel */]) === 'function' && _b) || Object)
    ], ViewParameterComponent.prototype, "parameter", void 0);
    ViewParameterComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-view-parameter',
            template: __webpack_require__(1368),
            styles: [__webpack_require__(1303)]
        }), 
        __metadata('design:paramtypes', [])
    ], ViewParameterComponent);
    return ViewParameterComponent;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/view-parameter.component.js.map

/***/ }),

/***/ 1041:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__view_models_property_view_model__ = __webpack_require__(369);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__view_models_helpers_view_models__ = __webpack_require__(41);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ViewPropertyComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var ViewPropertyComponent = (function () {
    function ViewPropertyComponent(router, error, context) {
        var _this = this;
        this.router = router;
        this.error = error;
        this.context = context;
        this.doClick = function (right) { return _this.property.doClick(right); };
    }
    // template listeners 
    ViewPropertyComponent.prototype.onEnter = function (event) {
        this.copy(event);
    };
    ViewPropertyComponent.prototype.onEnter1 = function (event) {
        this.copy(event);
    };
    Object.defineProperty(ViewPropertyComponent.prototype, "title", {
        // template API
        get: function () {
            return this.property.title;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "propertyType", {
        get: function () {
            return this.property.type;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "propertyRefType", {
        get: function () {
            return this.property.refType;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "propertyReturnType", {
        get: function () {
            return this.property.returnType;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "formattedValue", {
        get: function () {
            return this.property.formattedValue;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "value", {
        get: function () {
            return this.property.value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "format", {
        get: function () {
            return this.property.format;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "isBlob", {
        get: function () {
            return this.property.format === "blob";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "isMultiline", {
        get: function () {
            return !(this.property.multipleLines === 1);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "multilineHeight", {
        get: function () {
            return this.property.multipleLines * 20 + "px";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "color", {
        get: function () {
            return this.property.color;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ViewPropertyComponent.prototype, "attachment", {
        get: function () {
            return this.property.attachment;
        },
        enumerable: true,
        configurable: true
    });
    ViewPropertyComponent.prototype.copy = function (event) {
        var prop = this.property;
        if (prop) {
            __WEBPACK_IMPORTED_MODULE_5__view_models_helpers_view_models__["l" /* copy */](event, prop, this.context);
        }
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_4__view_models_property_view_model__["a" /* PropertyViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__view_models_property_view_model__["a" /* PropertyViewModel */]) === 'function' && _a) || Object)
    ], ViewPropertyComponent.prototype, "property", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('keydown', ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], ViewPropertyComponent.prototype, "onEnter", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('keypress', ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], ViewPropertyComponent.prototype, "onEnter1", null);
    ViewPropertyComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-view-property',
            template: __webpack_require__(1369),
            styles: [__webpack_require__(1304)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_router__["b" /* Router */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_router__["b" /* Router */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__error_service__["a" /* ErrorService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */]) === 'function' && _d) || Object])
    ], ViewPropertyComponent);
    return ViewPropertyComponent;
    var _a, _b, _c, _d;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/view-property.component.js.map

/***/ }),

/***/ 1042:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return environment; });
// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `angular-cli.json`.
var environment = {
    production: false
};
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/environment.js.map

/***/ }),

/***/ 106:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ChoiceViewModel; });
var ChoiceViewModel = (function () {
    function ChoiceViewModel(wrapped, id, name, searchTerm) {
        var _this = this;
        this.wrapped = wrapped;
        this.id = id;
        this.getValue = function () { return _this.wrapped; };
        this.equals = function (other) {
            return other instanceof ChoiceViewModel &&
                _this.id === other.id &&
                _this.name === other.name &&
                _this.wrapped.toValueString() === other.wrapped.toValueString();
        };
        this.valuesEqual = function (other) {
            if (other instanceof ChoiceViewModel) {
                var thisValue = _this.isEnum ? _this.wrapped.toValueString().trim() : _this.search.trim();
                var otherValue = _this.isEnum ? other.wrapped.toValueString().trim() : other.search.trim();
                return thisValue === otherValue;
            }
            return false;
        };
        this.toString = function () { return _this.name; };
        this.name = name || wrapped.toString();
        this.search = searchTerm || this.name;
        this.isEnum = !wrapped.isReference() && (this.name !== this.getValue().toValueString());
    }
    return ChoiceViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/choice-view-model.js.map

/***/ }),

/***/ 122:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_models_helpers_view_models__ = __webpack_require__(41);
/* harmony export (immutable) */ __webpack_exports__["b"] = wrapAction;
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ActionComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


function wrapAction(a) {
    return {
        value: a.title,
        doClick: function () { return a.doInvoke(); },
        doRightClick: function () { return a.doInvoke(true); },
        show: function () { return true; },
        disabled: function () { return a.disabled() ? true : null; },
        tempDisabled: function () { return a.tempDisabled(); },
        title: function () { return a.description; },
        accesskey: null
    };
}
var ActionComponent = (function () {
    function ActionComponent(renderer) {
        this.renderer = renderer;
    }
    ActionComponent.prototype.canClick = function () {
        return !(this.disabled() || this.tempDisabled());
    };
    ActionComponent.prototype.doClick = function () {
        if (this.canClick()) {
            this.action.doClick();
        }
    };
    ActionComponent.prototype.doRightClick = function () {
        if (this.canClick() && this.action.doRightClick) {
            this.action.doRightClick();
        }
    };
    ActionComponent.prototype.class = function () {
        return ({
            tempdisabled: this.tempDisabled()
        });
    };
    ActionComponent.prototype.show = function () {
        return this.action && this.action.show();
    };
    ActionComponent.prototype.disabled = function () {
        return this.action.disabled();
    };
    ActionComponent.prototype.tempDisabled = function () {
        return this.action.tempDisabled();
    };
    Object.defineProperty(ActionComponent.prototype, "value", {
        get: function () {
            return this.action.value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ActionComponent.prototype, "title", {
        get: function () {
            return this.action.title();
        },
        enumerable: true,
        configurable: true
    });
    ActionComponent.prototype.focus = function () {
        if (this.disabled()) {
            return false;
        }
        return !!(this.focusList && this.focusList.first) && __WEBPACK_IMPORTED_MODULE_1__view_models_helpers_view_models__["m" /* focus */](this.renderer, this.focusList.first);
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Object)
    ], ActionComponent.prototype, "action", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])("focus"), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _a) || Object)
    ], ActionComponent.prototype, "focusList", void 0);
    ActionComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-action',
            template: __webpack_require__(1341),
            styles: [__webpack_require__(1276)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"]) === 'function' && _b) || Object])
    ], ActionComponent);
    return ActionComponent;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/action.component.js.map

/***/ }),

/***/ 1274:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    display: block;\n}\n\nnof-action   {\n    outline: none;\n    margin-right: 20px;\n    display: block;\n    float: left;\n    padding: 5px;\n    margin-left: 1px; /*Else hover outline is hidden on the left-most menu*/\n    margin-right: 10px;\n    margin-bottom: 5px; /*Else hover outline is hidden on the bottom*/\n    font-weight: bolder; \n    font-size: 11pt;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1275:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    float: left;\n    margin-bottom: 20px;\n    display: block;\n}\n\n\nnof-action, .submenu  {\n    display: block;\n    cursor: pointer;\n    outline: none;\n    margin-right: 20px;\n    width: 150px;\n    font-size: 11pt;\n    font-weight: bolder;\n}\n.submenu {\n    padding: 5px;\n}\n.collapsed {\n    display: none;\n}\n\n.open {\n    display: block;\n    margin-left: 10px;\n}\n\n.open.rootMenu {\n    margin-left: 0px;\n}\n\n/*Icons*/\n.icon-expand:before {\n    content: \"\\E099\";\n}\n\n.icon-collapse:before {\n    content: \"\\E098\";\n}\n.icon-expand, .icon-collapse {\n        font-size: 8pt;\n}\n\n.icon-expand:hover, .icon-collapse:hover {\n    outline-color: white;\n    outline-style: solid;\n    outline-width: 1px;\n}\n\n[class^=\"icon-\"], [class*=\" icon-\"] {\n    font-family: \"iconFont\";\n    font-weight: normal;\n    font-style: normal;\n    text-decoration: inherit;\n    -webkit-font-smoothing: antialiased;\n    display: inline-block;\n    width: auto;\n    height: auto;\n    line-height: normal;\n    vertical-align: baseline;\n    background-image: none;\n    background-position: 0% 0%;\n    background-repeat: repeat;\n    margin-top: 0;\n    position: relative;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1276:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "\ninput {  /*To override user agent style sheet defaults*/\n    white-space: normal; /*Needed to force long names to wrap*/\n    text-align: left;\n    cursor: pointer;\n    background-color: transparent;\n    outline: none;\n    border: none;\n    font-size: inherit;\n    font-weight: inherit;\n    color: inherit;\n    padding: 5px;\n}\n\ninput:focus, input:hover   {\n    outline-color: white;\n    outline-style: solid;\n    outline-width: 1px;\n}\n\ninput:disabled {\n    color: grey;\n    outline: none;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1277:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1278:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    display: block;\n    margin-left: 20px;\n    color: white;\n}\n\n.header {\n    display: block;\n    font-size: 24pt; \n    font-weight: 200;\n    margin-left: 5px;\n}\n\n.properties {\n    display: block;\n    font-size: 11pt; \n    font-weight: 100;\n    width: 410px;\n    padding: 5px;\n    margin-bottom: 20px;\n}\n\n.property {\n    display: block;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1279:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ".reference {\n    cursor: pointer;\n}\n\nimg {\n    max-width: 245px;\n    max-height: 100px;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1280:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ".attachment {\n    -webkit-box-pack: center;\n        -ms-flex-pack: center;\n            justify-content: center;\n    -webkit-box-align: center;\n        -ms-flex-align: center;\n            align-items: center;\n    display: -webkit-box;\n    display: -ms-flexbox;\n    display: flex;\n    height: 100%;\n}\n\n img {\n        max-width: 100%;\n        height: auto;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1281:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ".cicero, .cicero input, .cicero pre {\n    background-color: black;\n    color: green;\n    font-size: 18pt;\n    font-family: Courier New, Courier, monospace;\n    font-weight: bold;\n}\n\n.cicero {\n    width: 100%;\n    border: none;\n}\n\n    .cicero .output {\n        margin: 27px;\n    }\n\n    .cicero input {\n        border-color: green;\n        border-width: 2px;\n        border: solid;\n        padding: 5px;\n        margin: 20px;\n        width: 95%;\n    }\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1282:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    display: block;\n}\n\n:host:not(:last-child) {\n    margin-bottom: 20px;\n}\n\n.summary {\n    display: block;\n    width: 450px;\n    font-size: 12pt;\n    margin-bottom: 10px;\n    overflow: hidden; /*To make div same depth as its floating children*/\n}\n\n.name, .details {\n    display: block;\n    float: left;\n    padding-left: 0px; /*Because it is a heading, so outdented*/\n    padding-right: 5px;\n}\n\n.name {\n    width: 150px;\n}\n\n.details {\n    width: 245px;\n}\n\n.icon {\n    display: block;\n    cursor: pointer;\n    float: right;\n    width: 20px;\n    height: 20px;\n    padding-left: 5px;\n    padding-top: 5px;\n    margin-right: 5px;\n   \n}\n\n.icon.summary {\n  background: url(" + __webpack_require__(1508) + ");\n   background-size: cover;  \n}\n.icon.list {\n background: url(" + __webpack_require__(850) + ");\n  background-size: cover;  \n} \n.icon.table {\n background: url(" + __webpack_require__(851) + ");\n  background-size: cover;  \n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1283:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    display: block;\n    float: left;\n    margin-bottom: 20px;\n    margin-right: 20px;\n}\n\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1284:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    display: block;\n    width: 410px;\n    margin-bottom: 20px;\n    overflow: hidden; /*Needed to force the floated divs inside this to expand the containing div in height*/\n    background-color: white;\n    font-family: 'Segoe UI', 'Open Sans', Verdana, Arial, Helvetica, sans-serif;\n    color: black;\n    font-size: 11pt; \n    font-weight: 400;\n    padding-left: 5px;\n    padding-right: 5px; \n}\n\n.dialog {\n    display: block; \n}\n\ninput[type=button],\ninput[type=reset],\ninput[type=submit] {\n    margin-left: 5px;\n    margin-top: 10px;\n    margin-right: 5px;\n    margin-bottom: 5px;\n    display: inline-block;\n    padding: 4px 12px;\n    text-align: center;\n    vertical-align: middle;\n    border: 1px transparent solid;\n    cursor: pointer;\n    width: auto;\n    *zoom: 1;\n    color: black;\n    background-color: white;\n    float: right;\n    outline: solid;\n    outline-color: lightgrey;\n    outline-width: 1px;\n}\n\ninput[type='button']:focus,\ninput[type='button']:hover,\ninput[type='submit']:focus,\ninput[type='submit']:hover {\n    outline: solid;\n    outline-width: 1px;\n    outline-color: black;\n}\n\n.ok:disabled {\n    color: grey;\n}\n\n.title {\n    display: block;\n    font-size: 12pt;        \n    margin-bottom: 10px;\n}\n\n.icon-cancel {\n        color: black;\n        float: right;\n    }\n\n.parameter .name {\n    width: 145px; /*To align input box with reference fields*/\n}\n\n .parameter {\n    overflow: hidden; /*To cope with long param/prop names*/\n    display: block;\n}\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1285:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1286:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ".list {\n    color: white;\n    padding-left: 20px;\n    height: 100%;\n    overflow-y: auto;\n    display: block;\n    font-weight: 100;\n}\n\n.header {\n    display: block;\n    margin-bottom: 20px;\n    overflow: hidden;\n}\n\n.title, .type {\n    position: relative;\n    font-weight: 100;\n    font-size: 24pt;\n    line-height: 38pt;\n    margin-left: 1px; /*Make room for focus outline*/\n    padding-left: 5px;\n    padding-right: 5px;\n    margin-right: 20px;\n    margin-top: 2px;\n    display: block;\n}\n\n.type {\n    float: left;\n    margin-right: 20px;\n    display: none; /*Change this to display type (e.g. for accessibility)*/\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1287:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1288:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "\n:host {\n    overflow: hidden; /*To cope with long prop names*/\n    display: block;\n}\n\n:host:not(:last-child) {\n    margin-bottom: 5px;\n}\n\ninput[type=text],  select {\n    border-style:solid;\n    border-color: grey;\n    border-width: 1px;\n}\n\ninput[type='text'] {\n    padding-left: 4px;\n    padding-right: 10px; /*To avoid text running over the 'x' (clear button)*/\n}\n\ninput:hover, input:focus {\n    outline-style: solid;\n    outline-color: black;\n    outline-width: 1px;\n}\n.name {\n    float: left;\n    width: 145px;\n    padding-right: 5px;\n}\n\n.reference, .value, collectionDetails {\n    width: 245px;\n    padding-left: 5px;\n    margin: 1px;\n    float: left;\n}\n\ninput:not([type='checkbox']), textarea {\n    width: 245px;\n    height: 20px;\n    padding-right:10px;\n}\n\ninput[type='text'] {\n    padding-left: 4px;\n}\n\n.validation {\n    display: block;\n    color: black;\n}\n\n/*MultiLine dialog*/\n :host.multilinedialog, :host.multilinedialog .co-validation, .multilinedialog .buttons {\n    display: inline-block;\n    vertical-align: top;\n}\n\n:host.multilinedialog .name {\n    display: none;\n}\n\n:host.multilinedialog .parameter {\n    width: 245px; /*To force (on IE) any validation message to be displayed underneath the field*/\n}\n\n:host.multilinedialog input:focus,:host.multilinedialog input:hover  {\n    outline-width: 1px;\n    outline-color: blue;\n}\n\n:host.multilinedialog .input-control input:not([type='checkbox']),\n:host.multilinedialog select {\n    height: 20px;\n    padding-top: 2px;\n}\n\n:host.multilinedialog .validation {\n    color: white;\n    margin-left: 5px;\n}\n\n\n/* Drag and drop*/\n.value.droppable {\n    border: solid;\n    border-width: 1px;\n    width: 245px;\n    height: 20px;\n}\n\n.value.droppable {\n    border-color: black;\n}\n\n.value.droppable.dropping.candrop {\n    border-color: lawngreen;\n    background-color: white;\n}\n\n.value.droppable.dropping {\n    border-color: red;\n}\n\n/*auto-complete*/\n.ui-autocomplete {\n    width: 245px;\n}\n\n.ui-autocomplete .ui-menu-item {\n        font-family: 'Segoe UI';\n        font-size: 10pt;\n}\n\ntextarea {\n    color: black;\n}\n\n.multiline {\n    overflow: auto;\n}\n\n/* clear button on non-empty input field*/\n.ng-clearable{\n  background-image: url(data:image/gif;base64,R0lGODlhgACAAPcAAAAAAAQEBAwMDBQUFBgYGCAgICQkJCgoKCwsLDQ0NDs7Ozw8PERERFBQUFRUVFhYWGBgYGhoaG5ubnBwcHR0dHh4eH9/fwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAEAAP8ALAAAAACAAIAAAAj/AP8JHEiwoMGDCBMqXMiwocOHECNKnEixosWLGDNq3Mixo8ePIAVWCEnSooWS/wQAmICypcMJAAiQVAlgpcubCGHWlPmRZk2bOIP+s/Bzp0efRVkKvVn0pwCOSJsqXVqyadGnGaNanUr1o9WmWC1q/cq168avVsNOHIu2rNmLaL+qhcg2rtu3E+Oinduwrt67eB/qjctXod/BgAMvDDA4bgCHhxsnVnyQcWPHDCNfnkx5oOXLmBNCAL25s8LPpNE+UOggNWLTlV3HdcCwtey2sAmivv2TdkPbvKXmThm8KQOIwIvX5LxU8+UFEpMrZ47TeWPoE6UXp97S+mDsFLUH/+c+U/lP8BXF8ybf03xN9BbV32YP1T0A+Bfly6afkYD9Ax7p5xp/FvnnHoAfCZgagRMZaB6CIClIGoMQOagchCFJCBqFDVlYHIYkaVjaRx4GB2JJIkrmUQH2GSBUiq9tdIB9PL1oH1AYzehejUvB+FeONOLlo10W6Wgej2YNidtERiqH5FtKkiVRk8U9KeSN7C1gX2GBRbmVQ1q6x6ViXgq3UJjmjUlZmUkphKZyanbG5k+AvVlcnKbNuZxBdgaHJ2x64vgPA1sOVxuWAun5mKGH2ieBoow6FKh9i0b6243mVWrppZjypummnHaa2qeghirqZaVmdyqqqaq6alytptL36lexxjdrUbXmdysAuWI0qVW9ZvTrTycF66uoHBpL0KTJKksQps06+w+y0lq0arS9zoptrLtRW61D3Z667abhXvvtabuaeW5B5d46Lmzt7vouZfGmOy9e3r16b1f5aittv+4qC/BXCjSAaK8DW4VdoPuilHBT6DEca4m8IWCQxKVSfNuJiR5sqcaycTwQxoyC7JrIy3qcm8mpoVwQyaaxTJrLF6usmMyg0XwQzHhRGZyVpk6nmM+8AS2pzUsRfZvRD/EclNKyMY0c0i5B7ZrUETnajZLVqWEdHdUhJWDfABGC7VGfvP0pq30ND4T2bWrbyjZJb8sWt65zf1S3a3cfmzdHe6fWt7BmTxQ4aYNrpLWrmSZZ+NHukWrj34wrJ3mPjycE6ZqZ1xw5oJ2PTOlwixs02ueGlj6QTu5Zqjrr5oEa6EgHUXBjqnOqWS9osZaJ5+6D5Rql2sCjFeyQdwOsLIyD59v2TSImTtyG1UoovUCaPT85YRgdpj3me2lU1/dUiXf9QWOR35V05yPkIQTr7uwUSA76Fn/N7S/kn/33m45SBP0LoAAHSMB1BQQAOw==);\n  background-repeat: no-repeat;\n  background-position: right -10px;\n  background-size: 8px;\n}\n.ng-clearable[class*=\"link-color\"] {\n    background-image: url(data:image/gif;base64,R0lGODlhgACAAPcAAAAAAP78/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAEAAP8ALAAAAACAAIAAAAj/AP8JHEiwoMGDCBMqXMiwocOHECNKnEixosWLGDNq3Mixo8ePIAOIDACypMWRJD+iFGmyJcSVKTnCZOmyZsKZMTPipGmzp8CdOU8CDerT5FCiEo/yLFpSKdKHTpcy7Rj1KcOqUqfqxJoUa1atFb1+vSl2LNiXZa0WTDvybFi2an/CNeuW7FyFc1HWhZrXbN+2exf+9Ts4bt3CVhEbBqvYbuHAaxs7HgxZLuKrkvcqXvxvs+bMDT2fFY32slbSEVH3VJ0adE3WXU2/dv1WtlHaQm2rxH0RtkbfGIHnftyUd23it3UPpzxb+UThHqFjdt5S+uS/p43j1e7SOkHvyZFP1xc/mrtl8m6hg19tfL1P2O6Lqo7PlDT9qZ7vZ9+MvjJC/sz5FxqAdwnIF4FlGdgaglwpuCCDSjn4HIRHSUgRhUBZeByGgGl4IYcdevghhyIGh2GJWzGI4m8IrrgRgS7KpF+M/5lH44E23igYjDqOyGOPpakIpEMgDjneiUbWCCJdPS65UpLfOfkklFLOlGSVOA2J5U5NbskljV4OFWOYFaJIZoQiijYje7at2R1tbhZHXZzRaUfni+bdaSJ1Bum5IXY4Brhffzvy+aah2yEaUo5KEiqno4ECptocpEEK+qilvTHqI6Z7UrqcpFRpGmlf4XH6KanViZqopywqOqCreMJ6HarvqTqQn6PmNaGsu/Hama2hyorrpqCuWmx6efo6qannHRuYb8DaxJuy9RHK6mfO/kqrh9k2m5aLuhb67ZhsETkumAnmWqaOYsUWlZENuruulk6daiWU2maYqb745ntvpzD1G+W/KU4p8K0BB6vXwQMzuSHDEEcs8cRQBgQAOw==);\n}\n.ng-clearable.ng-x  { background-position: right 2px center; }\n.ng-clearable.ng-onX{ cursor: pointer; }\ninput::-ms-clear {display: none;}\n\n/* date fields*/\n.input-control input.hasDatepicker,\n.input-control input.ui-timepicker-input {\n    width: 217px;\n}\n\n.input-control input.hasDatepicker,\n.input-control input.ui-timepicker-input {\n    width: 207px;\n    float: left;\n}\n\nimg.ui-datepicker-trigger {\n    margin-left: 5px;\n    margin-top: 2px;\n    width: 20px;\n}\n\nimg.ui-datepicker-trigger {\n    background-color: black;\n}\n\nselect {\n    width: 245px;\n    height: 20px;\n}\n\n    select[multiple] {\n        height: 68px; /*Issue with IE if just inherit the default height*/\n    }\n\n.validation {\n    margin-left: 155px;\n    font-size: 11pt;\n}\n\n.suggestions {\n    display: block;\n    color: black;\n    background-color: white;\n    font-size: 10pt;\n    width: 245px;\n    border-style: solid;\n    border-width: 1px;\n    border-color: black;\n}\n.suggestions ul {\n    display: block;\n    margin: 0px;\n    padding: 5px;\n}\n\n.suggestions ul:hover {\n        color: white;\n    background-color: Black;\n}\n.suggestions li {\n    list-style-type: none;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1289:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "\n:host {\n    overflow: hidden; /*To cope with long prop names*/\n    display: block;\n}\n\n:host:not(:last-child) {\n    margin-bottom: 5px;\n}\n\n.name {\n    display: block;\n    float: left;\n    width: 150px;\n    padding-right: 5px;\n}\n\n.field {\n    display: block;\n    float: left;\n    width: 245px;\n}\n\n.reference, .value, collectionDetails {\n    display: block;\n    float: left;\n    width: 245px;\n    padding-left: 5px;\n    padding-right: 5px;\n    margin: 1px;\n}\n\n.reference img {\n    display: block;\n    cursor: pointer;\n    float: left;\n    max-width: 245px;\n    max-height: 100px;\n}\n\n/* Drag and drop*/\n.value.droppable {\n    border: solid;\n    border-width: 1px;\n    width: 245px;\n    height: 20px;\n}\n\n.value.droppable {\n    border-color: black;\n}\n\n.value.droppable.dropping.candrop {\n    border-color: lawngreen;\n    background-color: white;\n}\n\n.value.droppable.dropping {\n    border-color: red;\n}\n\n/*auto-complete*/\n.ui-autocomplete {\n    width: 245px;\n}\n\n.ui-autocomplete .ui-menu-item {\n        font-family: 'Segoe UI';\n        font-size: 10pt;\n}\n\n\ntextarea {\n    color: black;\n}\n.multiline {\n    overflow: auto;\n}\n\n/* clear button on non-empty input field*/\n.ng-clearable{\n  background-image: url(data:image/gif;base64,R0lGODlhgACAAPcAAAAAAAQEBAwMDBQUFBgYGCAgICQkJCgoKCwsLDQ0NDs7Ozw8PERERFBQUFRUVFhYWGBgYGhoaG5ubnBwcHR0dHh4eH9/fwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAEAAP8ALAAAAACAAIAAAAj/AP8JHEiwoMGDCBMqXMiwocOHECNKnEixosWLGDNq3Mixo8ePIAVWCEnSooWS/wQAmICypcMJAAiQVAlgpcubCGHWlPmRZk2bOIP+s/Bzp0efRVkKvVn0pwCOSJsqXVqyadGnGaNanUr1o9WmWC1q/cq168avVsNOHIu2rNmLaL+qhcg2rtu3E+Oinduwrt67eB/qjctXod/BgAMvDDA4bgCHhxsnVnyQcWPHDCNfnkx5oOXLmBNCAL25s8LPpNE+UOggNWLTlV3HdcCwtey2sAmivv2TdkPbvKXmThm8KQOIwIvX5LxU8+UFEpMrZ47TeWPoE6UXp97S+mDsFLUH/+c+U/lP8BXF8ybf03xN9BbV32YP1T0A+Bfly6afkYD9Ax7p5xp/FvnnHoAfCZgagRMZaB6CIClIGoMQOagchCFJCBqFDVlYHIYkaVjaRx4GB2JJIkrmUQH2GSBUiq9tdIB9PL1oH1AYzehejUvB+FeONOLlo10W6Wgej2YNidtERiqH5FtKkiVRk8U9KeSN7C1gX2GBRbmVQ1q6x6ViXgq3UJjmjUlZmUkphKZyanbG5k+AvVlcnKbNuZxBdgaHJ2x64vgPA1sOVxuWAun5mKGH2ieBoow6FKh9i0b6243mVWrppZjypummnHaa2qeghirqZaVmdyqqqaq6alytptL36lexxjdrUbXmdysAuWI0qVW9ZvTrTycF66uoHBpL0KTJKksQps06+w+y0lq0arS9zoptrLtRW61D3Z667abhXvvtabuaeW5B5d46Lmzt7vouZfGmOy9e3r16b1f5aittv+4qC/BXCjSAaK8DW4VdoPuilHBT6DEca4m8IWCQxKVSfNuJiR5sqcaycTwQxoyC7JrIy3qcm8mpoVwQyaaxTJrLF6usmMyg0XwQzHhRGZyVpk6nmM+8AS2pzUsRfZvRD/EclNKyMY0c0i5B7ZrUETnajZLVqWEdHdUhJWDfABGC7VGfvP0pq30ND4T2bWrbyjZJb8sWt65zf1S3a3cfmzdHe6fWt7BmTxQ4aYNrpLWrmSZZ+NHukWrj34wrJ3mPjycE6ZqZ1xw5oJ2PTOlwixs02ueGlj6QTu5Zqjrr5oEa6EgHUXBjqnOqWS9osZaJ5+6D5Rql2sCjFeyQdwOsLIyD59v2TSImTtyG1UoovUCaPT85YRgdpj3me2lU1/dUiXf9QWOR35V05yPkIQTr7uwUSA76Fn/N7S/kn/33m45SBP0LoAAHSMB1BQQAOw==);\n  background-repeat: no-repeat;\n  background-position: right -10px;\n  background-size: 8px;\n}\n.ng-clearable[class*=\"link-color\"] {\n    background-image: url(data:image/gif;base64,R0lGODlhgACAAPcAAAAAAP78/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAEAAP8ALAAAAACAAIAAAAj/AP8JHEiwoMGDCBMqXMiwocOHECNKnEixosWLGDNq3Mixo8ePIAOIDACypMWRJD+iFGmyJcSVKTnCZOmyZsKZMTPipGmzp8CdOU8CDerT5FCiEo/yLFpSKdKHTpcy7Rj1KcOqUqfqxJoUa1atFb1+vSl2LNiXZa0WTDvybFi2an/CNeuW7FyFc1HWhZrXbN+2exf+9Ts4bt3CVhEbBqvYbuHAaxs7HgxZLuKrkvcqXvxvs+bMDT2fFY32slbSEVH3VJ0adE3WXU2/dv1WtlHaQm2rxH0RtkbfGIHnftyUd23it3UPpzxb+UThHqFjdt5S+uS/p43j1e7SOkHvyZFP1xc/mrtl8m6hg19tfL1P2O6Lqo7PlDT9qZ7vZ9+MvjJC/sz5FxqAdwnIF4FlGdgaglwpuCCDSjn4HIRHSUgRhUBZeByGgGl4IYcdevghhyIGh2GJWzGI4m8IrrgRgS7KpF+M/5lH44E23igYjDqOyGOPpakIpEMgDjneiUbWCCJdPS65UpLfOfkklFLOlGSVOA2J5U5NbskljV4OFWOYFaJIZoQiijYje7at2R1tbhZHXZzRaUfni+bdaSJ1Bum5IXY4Brhffzvy+aah2yEaUo5KEiqno4ECptocpEEK+qilvTHqI6Z7UrqcpFRpGmlf4XH6KanViZqopywqOqCreMJ6HarvqTqQn6PmNaGsu/Hama2hyorrpqCuWmx6efo6qannHRuYb8DaxJuy9RHK6mfO/kqrh9k2m5aLuhb67ZhsETkumAnmWqaOYsUWlZENuruulk6daiWU2maYqb745ntvpzD1G+W/KU4p8K0BB6vXwQMzuSHDEEcs8cRQBgQAOw==);\n}\n.ng-clearable.ng-x  { background-position: right 2px center; }\n.ng-clearable.ng-onX{ cursor: pointer; }\ninput::-ms-clear {display: none;}\n\n/* date fields*/\n.input-control input.hasDatepicker,\n.input-control input.ui-timepicker-input {\n    width: 217px;\n}\n\nimg.ui-datepicker-trigger {\n    margin-left: 5px;\n    margin-top: 2px;\n    width: 20px;\n}\n\nselect {\n    width: 245px;\n    height: 20px;\n}\n\n    select[multiple] {\n        height: 68px; /*Issue with IE if just inherit the default height*/\n    }\n\n.validation {\n    font-size: 11pt;\n    padding: 5px;\n    color: white;\n}\n\n.input-control input:not([type='checkbox']), textarea {\n    width: 245px;\n    height: 20px;\n    padding-left: 2px;\n}\ninput, select {\n    border-style: none;\n}\n\n.suggestions {\n    display: block;\n    color: black;\n    background-color: white;\n    font-size: 10pt;\n    font-weight: bolder;\n    width: 245px;\n}\n.suggestions ul {\n    display: block;\n    margin: 0px;\n    padding: 5px;\n}\n\n.suggestions ul:hover {\n        color: white;\n    background-color: Black;\n}\n.suggestions li {\n    list-style-type: none;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1290:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "\n:host {\n    display: block;\n    padding-left: 20px;\n    height: 100%;\n    overflow-y: auto;\n}\n\n.title, .message, .code, .description, .stacktrace{\n    display: block;\n    margin-bottom: 20px;\n}\n\n.title, .message {\n    font-size: 16pt;\n    color: red;\n}\n\n .code, .description, .stacktrace {\n    font-size: 12pt;\n    color: white;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1291:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    position: fixed;\n    bottom: 0;\n    left: 0;\n    right: 0;\n    background-color: black;\n}\n\n.icon {\n    display: block;\n    color: white;\n    font-size: 36pt;\n    padding: 5px;\n    margin-left: 10px;\n}\n\n    .icon.disabled {\n        display: none;\n    }\n\n    .icon:hover, .icon:focus {\n        outline-style: solid;\n        outline-width: 1px;\n        outline-color: white;\n    }\n\n    .icon:active {\n        outline-style: solid;\n        outline-width: 2px;\n        outline-color: white;\n    }\n\n.icon, .currentcopy {\n    float: left;\n}\n\n.currentcopy {\n    display: inline-block;\n    margin-left: 20px;\n}\n\n/*Warnings & Messages*/\n.messages, .warnings, .loading {\n    background-color: black;\n    font-size: 11pt;\n    color: white;\n}\n\n.warnings {\n    color: red;\n}\n\n/*Icons */\n.icon {\n    font-family: \"iconFont\";\n    font-weight: normal;\n    font-style: normal;\n    text-decoration: inherit;\n    -webkit-font-smoothing: antialiased;\n    display: inline-block;\n    width: auto;\n    height: auto;\n    line-height: normal;\n    vertical-align: baseline;\n    background-image: none;\n    background-position: 0% 0%;\n    background-repeat: repeat;\n    margin-top: 0;\n    position: relative;\n    cursor: pointer;\n}\n\n.home:before {\n    content: \"\\E000\";\n}\n\n.back:before {\n    content: \"\\E09F\";\n}\n\n.forward:before {\n    content: \"\\E09D\";\n}\n\n.swap:before {\n    content: \"\\E0A4\";\n}\n\n.full:before {\n    content: \"\\E08E\";\n}\n\n.recent:before {\n    content: \"\\E06B\";\n}\n\n.speech:before {\n    content: \"\\E036\";\n}\n\n.properties:before {\n    content: \"\\E048\";\n}\n\n.logoff:before {\n    content: \"\\E03B\";\n}\n\n/*Hide/show Cicero icon*/\n    .speech {\n        display: none;\n        /*display:    inline-block;*/\n    }", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1292:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "th {\n    font-size: 11pt;\n    font-weight: 100; /*Override browser default*/\n    text-align: left;\n    padding-left: 5px;\n    padding-right: 5px;\n    vertical-align: top;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1293:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ".home {\n    display: block;\n    padding-left: 20px;\n    height: 100%;\n    overflow-y: auto;\n    color: white;\n    font-size: 11pt; \n    font-weight: 100;\n}\n\n.header {\n    display: block;\n    margin-bottom: 20px;\n    overflow: hidden;\n}\n\n.title {\n    display: block;\n    position: relative;\n    font-weight: 200;\n    font-size: 24pt;\n    line-height: 38pt;\n    margin-left: 1px; /*Make room for focus outline */\n    padding-left: 5px;\n    padding-right: 5px;\n    margin-right: 20px;\n    margin-top: 2px;\n    display: block;\n}\n\n.main-column{\n    float: left;\n    margin-bottom: 20px;\n    margin-right: 20px;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1294:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ".list {\n    color: white;\n    padding-left: 20px;\n    height: 100%;\n    overflow-y: auto;\n    display: block;\n    font-weight: 100;\n}\n\n.header {\n    display: block;\n    margin-bottom: 20px;\n    overflow: hidden;\n}\n\n.title, .type {\n    position: relative;\n    font-weight: 100;\n    font-size: 24pt;\n    line-height: 38pt;\n    margin-left: 1px; /*Make room for focus outline*/\n    padding-left: 5px;\n    padding-right: 5px;\n    margin-right: 20px;\n    margin-top: 2px;\n    display: block;\n}\n\n.type {\n    float: left;\n    margin-right: 20px;\n    display: none; /*Change this to display type (e.g. for accessibility)*/\n}\n\n/* Menus */\n\n.menus {\n    display: block;\n}\n.menu, .header .action {\n    display: block;\n    float: left;\n    padding: 5px;\n    margin-top: 0px;\n    margin-left: 1px; /*Else hover outline is hidden on the left-most menu */\n    margin-right: 10px;\n    margin-bottom: 5px; /*Else hover outline is hidden on the bottom */\n    font-size: 16pt;\n}\n\n.menu:disabled, .ok:disabled {\n        color: grey;\n    }\n\n.summary {\n    display: block;\n    width: 450px;\n    font-size: 12pt;\n    margin-bottom: 10px;\n    overflow: hidden;\n}\n\n.details {\n    padding-left: 40px;\n    width: 400px;\n    float: left;\n}\n\n.icon {\n    display: block;\n    cursor: pointer;\n    float: right;\n    width: 20px;\n    height: 20px;\n    padding-top: 5px;\n    padding-left: 5px;\n    margin-right: 5px;\n}\n.icon.list {\n background: url(" + __webpack_require__(850) + ");\n  background-size: cover;  \n} \n.icon.table {\n background: url(" + __webpack_require__(851) + ");\n  background-size: cover;  \n}\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1295:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    display: block;\n    padding-left: 20px;\n    height: 100%;\n    overflow-y: auto;\n    color: white;\n    font-size: 11pt; \n    font-weight: 100;\n}\n\n\n\n.title {\n    display: block;\n    position: relative;\n    font-weight: 200;\n    font-size: 24pt;\n    line-height: 38pt;\n    margin-left: 1px; /*Make room for focus outline */\n    padding-left: 5px;\n    padding-right: 5px;\n    margin-right: 20px;\n    margin-top: 2px;\n    display: block;\n}\n\nbutton   {\n    color: white;\n    background-color: black;\n    border: none;\n    outline-style: solid;\n    outline-width: 1px;\n    outline-color: white;\n    margin-right: 20px;\n    display: block;\n    float: left;\n    margin-left: 1px; /*Else hover outline is hidden on the left-most menu*/\n    margin-right: 10px;\n    margin-bottom: 5px; /*Else hover outline is hidden on the bottom*/\n    font-size: 16pt;\n    font-weight: 200;\n}\n\nbutton:hover {\n    outline-width: 2px;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1296:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    display: block;\n}\n\nnof-action   {\n    outline: none;\n    margin-right: 20px;\n    display: block;\n    float: left;\n    margin-left: 1px; /*Else hover outline is hidden on the left-most menu*/\n    margin-right: 10px;\n    margin-bottom: 5px; /*Else hover outline is hidden on the bottom*/\n    font-size: 16pt;\n}\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1297:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    overflow-y: auto;\n    font-family: 'Segoe UI', 'Open Sans', Verdana, Arial, Helvetica, sans-serif;\n    color: white;\n    font-size: 11pt; \n    font-weight: 100;\n    display: block;\n    margin-left: 20px;\n    padding-bottom: 60px;\n}\nviewTitle {\n    display: inline-block; /*Otherwise focus outline takes full width*/\n}\n\n.title, header .type {\n    position: relative;\n    font-weight: 200;\n    font-size: 24pt;\n    line-height: 38pt;\n    margin-left: 1px; /*Make room for focus outline*/\n    padding-left: 5px;\n    padding-right: 5px;\n    margin-right: 20px;\n    margin-top: 2px;\n    display: block;\n}\n\nheader .type {\n    float: left;\n    margin-right: 20px;\n    display: none; /*Change this to display type (e.g. for accessibility)*/\n}\nnof-parameters {\n    display: inline-block;\n}\n.columnHeader {\n    display: inline-block;\n    vertical-align: top;\n    width: 247px;\n    padding-left: 10px;\n}\n\n\ninput.ok {\n    font-size: 9pt;\n    height: 20px;\n    margin-top: 1px;\n    margin-right: 0px;\n    margin-left: 2px;\n    margin-bottom: 5px;\n    padding-top: 1px;\n    padding-bottom: 0px;\n    padding-left: 2px;\n    padding-right: 2px;\n    vertical-align: top;\n    outline-color: lightgray;\n}\n\ninput.close {\n    float: left;\n    margin-left: 6px;\n    margin-right: 10px;\n}\n.count {\n    display: inline-block;\n    margin-top: 1px;\n}\n.co-validation {\n    display: inline-block;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1298:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    overflow-y: auto;\n    font-family: 'Segoe UI', 'Open Sans', Verdana, Arial, Helvetica, sans-serif;\n    color: white;\n    font-size: 11pt; \n    font-weight: 100;\n    display: block;\n    height: 100%;\n}\n\n.object {\n    height: 100%;\n    display: block;\n    padding-left: 20px;\n    overflow-y: auto;\n}\n\n.header {\n    display: block;\n    margin-bottom: 20px;\n    overflow: hidden;\n}\n\nviewTitle {\n    display: inline-block; /*Otherwise focus outline takes full width*/\n}\n\n.title, .type {\n    position: relative;\n    font-weight: 200;\n    font-size: 24pt;\n    line-height: 38pt;\n    margin-left: 1px; /*Make room for focus outline*/\n    padding-left: 5px;\n    padding-right: 5px;\n    margin-right: 20px;\n    margin-top: 2px;\n    display: inline-block;\n}\n\n.type {\n    float: left;\n    margin-right: 20px;\n    display: none; /*Change this to display type (e.g. for accessibility)*/\n}\n\n/* Menus */\n.menus {\n    display: block;\n}\n.menu, .header .action {\n    display: block;\n    float: left;\n    padding: 5px;\n    margin-top: 0px;\n    margin-left: 1px; /*Else hover outline is hidden on the left-most menu */\n    margin-right: 10px;\n    margin-bottom: 5px; /*Else hover outline is hidden on the bottom */\n    font:inherit;\n    font-size: 16pt;\n    color: white;\n    background-color: transparent;\n}\n.menu:disabled {\n        color: grey;\n}\n\n.title:hover, .title:focus   {\n    outline-color: white;\n    outline-style: solid;\n    outline-width: 1px;\n}\n\n/*Icons*/\n.icon-expand:before {\n    content: \"\\E099\";\n}\n\n.icon-collapse:before {\n    content: \"\\E098\";\n}\n\n\n.actions, .main-column{\n    float: left;\n    margin-bottom: 20px;\n    margin-right: 20px;\n}\n\n\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1299:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "host {\n    width: 410px;\n    padding: 5px;\n    margin-bottom: 20px;\n}\n\n.co-validation {\n    color: red;\n}\n\n:host.multilinedialog .co-validation {\n    vertical-align: top;\n    margin-top: 2px;\n    margin-left: 2px;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1300:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "host {\n    width: 410px;\n    padding: 5px;\n    margin-bottom: 20px;\n}\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1301:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    display: block;\n    color: white;\n    margin-left: 20px;\n    padding-left: 20px;\n    height: 100%;\n    overflow-y: auto;\n}\n\n.header {\n    display: block;\n    overflow: hidden;\n    font-size: 24pt; \n    font-weight: 200;\n}\n\n.collection {\n    display: block;\n}\n\ntable {\n    font-size: 11pt; \n    font-weight: 200;\n}\n\ntr {\n    cursor: pointer;\n}\n\nth, td {\n    font-weight: 100; /*Override browser default*/\n    text-align: left;\n    padding-left: 5px;\n    padding-right: 5px;\n    vertical-align: top;\n}\n\ntbody .number {\n    text-align:right;\n}\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1302:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "\n:host {\n    cursor:pointer;\n}\n\ntd {\n    font-size: 11pt;\n    font-weight: 100; /*Override browser default*/\n    text-align: left;\n    padding-left: 5px;\n    padding-right: 5px;\n    vertical-align: top;\n}\n\n.number {\n    text-align:right;\n}\n\n.reference {  /*i.e. in  List, not Table mode */\n    cursor: pointer;\n    font-size: 11pt; \n    padding-left: 5px;\n    padding-right: 5px;\n    margin: 1px;\n    width: 440px;\n}\n.reference:not(:last-child) {\n   margin-bottom: 5px;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1303:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, ":host {\n    width: 245px;\n    padding-left: 5px;\n    padding-right: 5px;\n    padding-top: 1px;\n    margin-top: 1px;\n    margin-bottom: 1px;\n}\n:host.multilinedialog {\n    padding-left: 10px;\n    width: 247px;\n    display: inline-block;\n    overflow: auto;\n}\n:host.multilinedialog .name{\n display: none;\n}\n\n.reference {\n    cursor: pointer;\n}", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1304:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(12)();
// imports


// module
exports.push([module.i, "\n:host {\n    overflow: hidden; /*To cope with long prop names*/\n    display: block;\n}\n\n:host:not(:last-child) {\n    margin-bottom: 5px;\n}\n\n.name {\n    display: block;\n    float: left;\n    width: 150px;\n    padding-right: 5px;\n}\n\n.reference, .value {\n    display: block;\n    float: left;\n    width: 245px;\n    padding-left: 5px;\n    padding-right: 5px;\n    margin: 1px;\n}\n\n.reference {\n    cursor: pointer;\n}\n\n.multiline {\n    overflow: auto;\n}\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 1328:
/***/ (function(module, exports, __webpack_require__) {

var map = {
	"./af": 674,
	"./af.js": 674,
	"./ar": 681,
	"./ar-dz": 675,
	"./ar-dz.js": 675,
	"./ar-kw": 676,
	"./ar-kw.js": 676,
	"./ar-ly": 677,
	"./ar-ly.js": 677,
	"./ar-ma": 678,
	"./ar-ma.js": 678,
	"./ar-sa": 679,
	"./ar-sa.js": 679,
	"./ar-tn": 680,
	"./ar-tn.js": 680,
	"./ar.js": 681,
	"./az": 682,
	"./az.js": 682,
	"./be": 683,
	"./be.js": 683,
	"./bg": 684,
	"./bg.js": 684,
	"./bn": 685,
	"./bn.js": 685,
	"./bo": 686,
	"./bo.js": 686,
	"./br": 687,
	"./br.js": 687,
	"./bs": 688,
	"./bs.js": 688,
	"./ca": 689,
	"./ca.js": 689,
	"./cs": 690,
	"./cs.js": 690,
	"./cv": 691,
	"./cv.js": 691,
	"./cy": 692,
	"./cy.js": 692,
	"./da": 693,
	"./da.js": 693,
	"./de": 696,
	"./de-at": 694,
	"./de-at.js": 694,
	"./de-ch": 695,
	"./de-ch.js": 695,
	"./de.js": 696,
	"./dv": 697,
	"./dv.js": 697,
	"./el": 698,
	"./el.js": 698,
	"./en-au": 699,
	"./en-au.js": 699,
	"./en-ca": 700,
	"./en-ca.js": 700,
	"./en-gb": 701,
	"./en-gb.js": 701,
	"./en-ie": 702,
	"./en-ie.js": 702,
	"./en-nz": 703,
	"./en-nz.js": 703,
	"./eo": 704,
	"./eo.js": 704,
	"./es": 706,
	"./es-do": 705,
	"./es-do.js": 705,
	"./es.js": 706,
	"./et": 707,
	"./et.js": 707,
	"./eu": 708,
	"./eu.js": 708,
	"./fa": 709,
	"./fa.js": 709,
	"./fi": 710,
	"./fi.js": 710,
	"./fo": 711,
	"./fo.js": 711,
	"./fr": 714,
	"./fr-ca": 712,
	"./fr-ca.js": 712,
	"./fr-ch": 713,
	"./fr-ch.js": 713,
	"./fr.js": 714,
	"./fy": 715,
	"./fy.js": 715,
	"./gd": 716,
	"./gd.js": 716,
	"./gl": 717,
	"./gl.js": 717,
	"./gom-latn": 718,
	"./gom-latn.js": 718,
	"./he": 719,
	"./he.js": 719,
	"./hi": 720,
	"./hi.js": 720,
	"./hr": 721,
	"./hr.js": 721,
	"./hu": 722,
	"./hu.js": 722,
	"./hy-am": 723,
	"./hy-am.js": 723,
	"./id": 724,
	"./id.js": 724,
	"./is": 725,
	"./is.js": 725,
	"./it": 726,
	"./it.js": 726,
	"./ja": 727,
	"./ja.js": 727,
	"./jv": 728,
	"./jv.js": 728,
	"./ka": 729,
	"./ka.js": 729,
	"./kk": 730,
	"./kk.js": 730,
	"./km": 731,
	"./km.js": 731,
	"./kn": 732,
	"./kn.js": 732,
	"./ko": 733,
	"./ko.js": 733,
	"./ky": 734,
	"./ky.js": 734,
	"./lb": 735,
	"./lb.js": 735,
	"./lo": 736,
	"./lo.js": 736,
	"./lt": 737,
	"./lt.js": 737,
	"./lv": 738,
	"./lv.js": 738,
	"./me": 739,
	"./me.js": 739,
	"./mi": 740,
	"./mi.js": 740,
	"./mk": 741,
	"./mk.js": 741,
	"./ml": 742,
	"./ml.js": 742,
	"./mr": 743,
	"./mr.js": 743,
	"./ms": 745,
	"./ms-my": 744,
	"./ms-my.js": 744,
	"./ms.js": 745,
	"./my": 746,
	"./my.js": 746,
	"./nb": 747,
	"./nb.js": 747,
	"./ne": 748,
	"./ne.js": 748,
	"./nl": 750,
	"./nl-be": 749,
	"./nl-be.js": 749,
	"./nl.js": 750,
	"./nn": 751,
	"./nn.js": 751,
	"./pa-in": 752,
	"./pa-in.js": 752,
	"./pl": 753,
	"./pl.js": 753,
	"./pt": 755,
	"./pt-br": 754,
	"./pt-br.js": 754,
	"./pt.js": 755,
	"./ro": 756,
	"./ro.js": 756,
	"./ru": 757,
	"./ru.js": 757,
	"./sd": 758,
	"./sd.js": 758,
	"./se": 759,
	"./se.js": 759,
	"./si": 760,
	"./si.js": 760,
	"./sk": 761,
	"./sk.js": 761,
	"./sl": 762,
	"./sl.js": 762,
	"./sq": 763,
	"./sq.js": 763,
	"./sr": 765,
	"./sr-cyrl": 764,
	"./sr-cyrl.js": 764,
	"./sr.js": 765,
	"./ss": 766,
	"./ss.js": 766,
	"./sv": 767,
	"./sv.js": 767,
	"./sw": 768,
	"./sw.js": 768,
	"./ta": 769,
	"./ta.js": 769,
	"./te": 770,
	"./te.js": 770,
	"./tet": 771,
	"./tet.js": 771,
	"./th": 772,
	"./th.js": 772,
	"./tl-ph": 773,
	"./tl-ph.js": 773,
	"./tlh": 774,
	"./tlh.js": 774,
	"./tr": 775,
	"./tr.js": 775,
	"./tzl": 776,
	"./tzl.js": 776,
	"./tzm": 778,
	"./tzm-latn": 777,
	"./tzm-latn.js": 777,
	"./tzm.js": 778,
	"./uk": 779,
	"./uk.js": 779,
	"./ur": 780,
	"./ur.js": 780,
	"./uz": 782,
	"./uz-latn": 781,
	"./uz-latn.js": 781,
	"./uz.js": 782,
	"./vi": 783,
	"./vi.js": 783,
	"./x-pseudo": 784,
	"./x-pseudo.js": 784,
	"./yo": 785,
	"./yo.js": 785,
	"./zh-cn": 786,
	"./zh-cn.js": 786,
	"./zh-hk": 787,
	"./zh-hk.js": 787,
	"./zh-tw": 788,
	"./zh-tw.js": 788
};
function webpackContext(req) {
	return __webpack_require__(webpackContextResolve(req));
};
function webpackContextResolve(req) {
	var id = map[req];
	if(!(id + 1)) // check for number
		throw new Error("Cannot find module '" + req + "'.");
	return id;
};
webpackContext.keys = function webpackContextKeys() {
	return Object.keys(map);
};
webpackContext.resolve = webpackContextResolve;
module.exports = webpackContext;
webpackContext.id = 1328;


/***/ }),

/***/ 1339:
/***/ (function(module, exports) {

module.exports = "<nof-action *ngFor=\"let action of actions\" [action]=\"action\"></nof-action>\n\n"

/***/ }),

/***/ 1340:
/***/ (function(module, exports) {

module.exports = "<ng-container *ngFor=\"let menu of items; let i = index\">\n\n    <div *ngIf=\"menuName(menu)\" (click)=\"toggleCollapsed(menu)\"  class=\"submenu\" [ngSwitch]=\"navCollapsed(menu)\">\n        {{menuName(menu)}}\n        <div *ngSwitchCase=\"true\" (keydown.enter)=\"toggleCollapsed(menu)\" class=\"icon-expand\" tabindex=\"0\"></div>\n        <div *ngSwitchCase=\"false\" (keydown.enter)=\"toggleCollapsed(menu)\" class=\"icon-collapse\" tabindex=\"0\"></div>\n    </div>\n    <div  *ngIf=\"!navCollapsed(menu)\"  class=\"menuitem\" [ngClass]=\"displayClass(menu)\">\n        <ng-container *ngIf=\"hasActions(menu)\">\n            <ng-container *ngFor=\"let action of menuActions(menu, i)\">\n                <nof-action [action]=\"action\"></nof-action>\n            </ng-container>\n        </ng-container>\n        <ng-container  *ngIf=\"hasItems(menu)\">\n            <nof-action-list [menuHolder]=\"menu\"></nof-action-list>\n        </ng-container>\n    </div>\n</ng-container>\n"

/***/ }),

/***/ 1341:
/***/ (function(module, exports) {

module.exports = "<input #focus tabindex=\"0\" type=\"button\" geminiClick (leftClick)=\"doClick()\" (rightClick)=\"doRightClick()\"  [value]=\"value\" [disabled]=\"disabled()\" *ngIf=\"show()\" [title]=\"title\" [ngClass]=\"class()\">"

/***/ }),

/***/ 1342:
/***/ (function(module, exports) {

module.exports = "<ng-container *ngIf=\"auth.authenticated()\">\n<router-outlet></router-outlet>\n<nof-footer></nof-footer>\n</ng-container>\n<ng-container *ngIf=\"!auth.authenticated()\">\n     <nof-login></nof-login>\n</ng-container>\n"

/***/ }),

/***/ 1343:
/***/ (function(module, exports) {

module.exports = "<div id=\"pane1\" class=\"single\">\n    <div class=\"properties\">\n        <div class=\"header\">\n            <div class=\"title\">Application Properties</div>\n        </div>\n        <div class=\"main-column\">\n            <div class=\"properties\">\n                <div class=\"property\">\n                    Application Name: {{applicationName}}\n                </div>\n                <div class=\"property\">\n                    User Name: {{userName}}\n                </div>\n                <div class=\"property\">\n                    Server Url: {{serverUrl}}\n                </div>\n                <div class=\"property\">\n                    Server version: {{implVersion}}\n                </div>\n                <div class=\"property\">\n                    Client version: {{clientVersion}}\n                </div>\n            </div>\n        </div>\n    </div>\n</div>\n"

/***/ }),

/***/ 1344:
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"attachment\" class=\"reference\" geminiClick (leftClick)=\"doAttachmentClick()\" (rightClick)=\"doAttachmentClick(true)\" tabindex=\"0\">\n    <div *ngIf=\"!image\">{{title}}</div>\n    <img *ngIf=\"image\" src=\"{{image}}\" alt=\"{{title}}\" />\n</div>\n<div *ngIf=\"!attachment\"><div>Attachment not yet supported on transient</div></div>"

/***/ }),

/***/ 1345:
/***/ (function(module, exports) {

module.exports = "<div [attr.id]=\"paneIdName\" [ngClass]=\"paneType\">\n    <div class=\"attachment view\">\n        <div class=\"reference\">\n            <img *ngIf=\"image\" src=\"{{image}}\" alt=\"{{title}}\" />\n        </div>\n    </div>\n</div>\n<router-outlet (activate)=\"onChild()\" (deactivate)=\"onChildless()\"></router-outlet>\n"

/***/ }),

/***/ 1346:
/***/ (function(module, exports) {

module.exports = "<div class=\"cicero\">\n    <div class=\"output\" aria-live=\"polite\"><pre>{{outputText}}</pre></div>\n    <input #inputField type=\"text\" [ngModel]=\"inputText\" \n     (keyup.enter)=\"parseInput(inputField.value)\"\n     (keydown.arrowup)=\"selectPreviousInput()\" \n     (keydown.arrowdown)=\"clearInput()\"\n     />\n</div>"

/***/ }),

/***/ 1347:
/***/ (function(module, exports) {

module.exports = "<div class=\"summary\">\n    <div class=\"name\">{{title}}:</div>\n    <div class=\"details\">{{details}}</div>\n    <div>\n        <div *ngIf=\"showSummary()\" class=\"icon summary\" (click)=\"doSummary()\"  title=\"Close Collection\" alt=\"Close Collection\"></div>\n        <div *ngIf=\"showList()\" class=\"icon list\" (click)=\"doList()\"  title=\"View as List\" alt=\"View as List\"></div>\n        <div *ngIf=\"showTable()\" class=\"icon table\" (click)=\"doTable()\"  title=\"View as Table\" alt=\"View as Table\"></div>\n    </div>\n</div>\n<div class=\"messages\">{{message}}</div>\n<nof-action-bar *ngIf=\"showActions()\" class=\"actions\" [menuHolder]=\"collection\"></nof-action-bar>\n<nof-dialog *ngIf=\"showActions()\" [parent]=\"collection\" [selectedDialogId]=\"selectedDialogId\"></nof-dialog>\n\n<table>\n    <thead>\n        <tr nof-header [collection]=\"collection\" [state]=\"currentState\"></tr>\n    </thead>\n    <tbody *ngIf=\"state === 'list' || hasTableData()\">\n        <tr *ngFor=\"let item of items; let i = index\" nof-row [item]=\"item\" [index]=\"i\" [withCheckbox]=\"!disableActions()\" [isTable]=\"state === 'table'\"></tr>\n    </tbody>\n</table>"

/***/ }),

/***/ 1348:
/***/ (function(module, exports) {

module.exports = "<nof-collection class=\"collection\" *ngFor=\"let collection of collections\" [collection]=\"collection\"></nof-collection>"

/***/ }),

/***/ 1349:
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"dialog\" class=\"dialog\">\n\t<div class=\"title\">\n\t\t{{title}}\n\t</div>\n\t<form (ngSubmit)=\"onSubmit()\" [formGroup]=\"form\" autocomplete=\"off\">\n\t\t<nof-parameters class=\"parameters\" [parameters]=\"parameters\" [parent]=\"dialog\" [form]=\"form\"></nof-parameters>\n        <div class=\"co-validation\">{{message}}</div>\n\t\t<div class=\"form-row\">\n\t\t\t<input class=\"ok\" tabindex=\"0\" type=\"submit\" value=\"OK\" title=\"{{tooltip}}\" geminiClick (leftClick)=\"onSubmit()\" (rightClick)=\"onSubmit(true)\" [disabled]=\"!form.valid\" />\n\t\t\t<input class=\"cancel\" tabindex=\"0\" type=\"button\" geminiClick (leftClick)=\"close()\" value=\"Cancel\"/>\n\t\t</div>\n\t</form>\n</div>"

/***/ }),

/***/ 1350:
/***/ (function(module, exports) {

module.exports = "<div #parent></div>\n"

/***/ }),

/***/ 1351:
/***/ (function(module, exports) {

module.exports = "<div [attr.id]=\"paneIdName\" [ngClass]=\"paneType\">\n    <div #parent></div>\n    <div *ngIf=\"showPlaceholder\"class=\"list\">\n        <div class=\"header\">\n            <div class=\"title\">\n                {{title}}\n            </div>\n            <nof-action-bar [actions]=\"actionHolders\"></nof-action-bar>\n        </div>\n    </div>\n</div>\n<router-outlet (activate)=\"onChild()\" (deactivate)=\"onChildless()\"></router-outlet>\n"

/***/ }),

/***/ 1352:
/***/ (function(module, exports) {

module.exports = "<div [attr.id]=\"paneIdName\" [ngClass]=\"paneType\">\n    <div #parent></div>\n</div>\n<router-outlet (activate)=\"onChild()\" (deactivate)=\"onChildless()\"></router-outlet>\n"

/***/ }),

/***/ 1353:
/***/ (function(module, exports) {

module.exports = "<div class=\"name\">\n    <label attr.for=\"{{parameterPaneId}}\">{{title}}</label>\n</div>\n<div [formGroup]=\"form\">\n    <ng-container *ngIf=\"parameterType === 'ref'\">\n\n        <div class=\"value input-control text\">\n            <!--\"EntryType.AutoComplete\"-->\n            <ng-container *ngIf=\"parameterEntryType === 5\">\n   \n                <md-input-container>\n                    <input #focus [id]=\"parameterPaneId\" class=\"value droppable\" type=\"text\" dnd-droppable [allowDrop]=\"accept(parameter)\" (onDropSuccess)=\"drop($event.dragData)\" [ngClass]=\"classes()\" placeholder=\"{{description}}\" mdInput [formControlName]=\"parameterId\" [formControl]=\"control\"  [mdAutocomplete]=\"auto\" [geminiClear]=\"parameter\" [form]=\"form\">\n                </md-input-container>\n\n                <md-autocomplete #auto=\"mdAutocomplete\">\n                    <md-option *ngFor=\"let option of choices\" [value]=\"option\">\n                        {{ option }}\n                    </md-option>\n                </md-autocomplete>\n            </ng-container>\n\n            <ng-container *ngIf=\"isChoices() && !isMultiple()\">\n                <select #focus [id]=\"parameterPaneId\"  [formControlName]=\"parameterId\">\n                    <option *ngFor=\"let choice of choices\" [label]=\"choiceName(choice)\" [ngValue]=\"choice\">{{choiceName(choice)}}</option>\n                </select>\n            </ng-container>\n            \n            <ng-container *ngIf=\"isChoices() && isMultiple()\">\n                <select #focus [id]=\"parameterPaneId\"  multiple [formControlName]=\"parameterId\">\n                    <option *ngFor=\"let choice of choices\" [label]=\"choiceName(choice)\" [ngValue]=\"choice\">{{choiceName(choice)}}</option>\n                </select>\n            </ng-container>\n\n            <!--\"EntryType.FreeForm\"-->\n            <ng-container *ngIf=\"parameterEntryType === 0\">\n                <input #focus [id]=\"parameterPaneId\" class=\"value droppable\" dnd-droppable [allowDrop]=\"accept(parameter)\" (onDropSuccess)=\"drop($event.dragData)\"\n                       [ngClass]=\"classes()\" placeholder=\"{{description}}\" type=\"text\" [formControlName]=\"parameterId\" [geminiClear]=\"parameter\" [form]=\"form\" />\n            </ng-container>\n\n        </div>\n    </ng-container>\n\n    <ng-container *ngIf=\"parameterType === 'scalar'\">\n        <div class=\"value input-control text\">\n\n            <!--\"EntryType.File\"-->\n            <ng-container *ngIf=\"parameterEntryType === 6\">\n                <input #focus [id]=\"parameterPaneId\" type=\"file\" placeholder=\"{{description}}\" (change)=\"fileUpload($event)\" />\n            </ng-container>\n\n            <!--\"EntryType.AutoComplete\"-->\n            <ng-container *ngIf=\"parameterEntryType === 5\">\n                <md-input-container>\n                    <input #focus [id]=\"parameterPaneId\" type=\"text\" [attr.placeholder]=\"description\" mdInput [formControlName]=\"parameterId\" [formControl]=\"control\" [mdAutocomplete]=\"auto\" [geminiClear]=\"parameter\" [form]=\"form\">\n                </md-input-container>\n\n                <md-autocomplete #auto=\"mdAutocomplete\">\n                    <md-option *ngFor=\"let option of choices\" [value]=\"option\">\n                        {{ option }}\n                    </md-option>\n                </md-autocomplete>\n            </ng-container>\n\n            <ng-container *ngIf=\"isChoices() && !isMultiple()\">\n                <select #focus [id]=\"parameterPaneId\" [formControlName]=\"parameterId\">\n                    <option *ngFor=\"let choice of choices\" [label]=\"choiceName(choice)\" [ngValue]=\"choice\">{{choiceName(choice)}}</option>\n                </select>\n            </ng-container>\n\n            <ng-container *ngIf=\"isChoices() && isMultiple()\">\n                <select #focus [id]=\"parameterPaneId\" multiple [formControlName]=\"parameterId\">\n                    <option *ngFor=\"let choice of choices\" [label]=\"choiceName(choice)\" [ngValue]=\"choice\">{{choiceName(choice)}}</option>\n                </select>\n            </ng-container>\n\n            <!--\"EntryType.FreeForm\"-->\n            <ng-container *ngIf=\"parameterEntryType === 0\">\n                <ng-container [ngSwitch]=\"parameterReturnType\">\n                    <ng-container *ngSwitchCase=\"'string'\" [ngSwitch]=\"format\">\n                        <ng-container *ngSwitchCase=\"'date'\">\n                            <input #focus [id]=\"parameterPaneId\" type=\"date\" placeholder=\"{{description}}\" [formControlName]=\"parameterId\"  />\n                        </ng-container>\n                        <ng-container *ngSwitchCase=\"'time'\">\n                            <input #focus [id]=\"parameterPaneId\" type=\"time\" placeholder=\"{{description}}\" [formControlName]=\"parameterId\" [geminiClear]=\"parameter\" [form]=\"form\" />\n                        </ng-container>\n                        <ng-container *ngSwitchDefault>\n                            <input #focus *ngIf=\"!isMultiline\" [id]=\"parameterPaneId\" placeholder=\"{{description}}\"\n                                   type=\"{{isPassword ? 'password' : 'text'}}\" [formControlName]=\"parameterId\" [geminiClear]=\"parameter\" [form]=\"form\" />\n                            <textarea #focus *ngIf=\"isMultiline\" rows=\"{{rows}}\" [id]=\"parameterPaneId\" [formControlName]=\"parameterId\"\n                                      placeholder=\"{{description}}\"></textarea>\n                        </ng-container>\n                    </ng-container>\n                    <ng-container *ngSwitchCase=\"'boolean'\">\n                        <input #focus [id]=\"parameterPaneId\" type=\"checkbox\" [geminiBoolean]=\"parameter\" />{{description}}\n                    </ng-container>\n                    <ng-container *ngSwitchDefault>\n                        <input #focus [id]=\"parameterPaneId\" type=\"text\" placeholder=\"{{description}}\" [formControlName]=\"parameterId\" [geminiClear]=\"parameter\" [form]=\"form\" />\n                    </ng-container>\n                </ng-container>\n            </ng-container>\n        </div>\n    </ng-container>\n</div>\n<div class=\"validation\">{{message}}</div>"

/***/ }),

/***/ 1354:
/***/ (function(module, exports) {

module.exports = "<div [formGroup]=\"form\">\n    <div class=\"name\">{{title}}:</div>\n    <div class=\"field\">\n    <ng-container *ngIf=\"propertyType === 'ref'\">\n\n        <div *ngIf=\"!isEditable\" class=\"value\" [ngClass]=\"classes()\">\n            {{formattedValue}}\n        </div>\n\n        <div *ngIf=\"isEditable\" class=\"input-control text\" [ngSwitch]=\"propertyEntryType\">\n            <!--\"EntryType.AutoComplete\"-->\n            <ng-container *ngSwitchCase=\"5\">\n               \n                <md-input-container>\n                    <input #focus [id]=\"propertyPaneId\" class=\"value droppable\" type=\"text\" dnd-droppable [allowDrop]=\"accept(property)\" (onDropSuccess)=\"drop($event.dragData)\" [ngClass]=\"classes()\" placeholder=\"{{propertyDescription}}\" mdInput [formControlName]=\"propertyId\" [formControl]=\"control\" [mdAutocomplete]=\"auto\" [geminiClear]=\"property\" [form]=\"form\">\n                </md-input-container>\n\n                <md-autocomplete #auto=\"mdAutocomplete\">\n                    <md-option *ngFor=\"let option of propertyChoices\" [value]=\"option\">\n                        {{ option }}\n                    </md-option>\n                </md-autocomplete>\n            </ng-container>\n\n            <!--\"EntryType.ConditionalChoices\"-->\n            <ng-container *ngSwitchCase=\"3\">\n                <select #focus [id]=\"propertyPaneId\"  [formControlName]=\"propertyId\">                 \n                    <option *ngFor=\"let choice of propertyChoices\" [ngValue]=\"choice\">{{choiceName(choice)}}</option>\n                </select>\n            </ng-container>\n\n            <!--\"EntryType.Choices\"-->\n            <ng-container *ngSwitchCase=\"1\">\n                <select #focus [id]=\"propertyPaneId\"  [formControlName]=\"propertyId\">\n                    <option *ngFor=\"let choice of propertyChoices\" [ngValue]=\"choice\">{{choiceName(choice)}}</option>\n                </select>\n            </ng-container>\n\n            <!--\"EntryType.FreeForm\"-->\n            <ng-container *ngSwitchCase=\"0\">\n                <input #focus [id]=\"propertyPaneId\" class=\"value droppable\" dnd-droppable [allowDrop]=\"accept(property)\" (onDropSuccess)=\"drop($event.dragData)\"\n                    [ngClass]=\"classes()\" placeholder=\"{{propertyDescription}}\" type=\"text\" [formControlName]=\"propertyId\"\n                    [geminiClear]=\"property\" [form]=\"form\" />\n            </ng-container>\n\n        </div>\n    </ng-container>\n\n    <ng-container *ngIf=\"propertyType === 'scalar'\">\n\n        <ng-container *ngIf=\"!isEditable\">\n            <ng-container [ngSwitch]=\"propertyReturnType\">\n                <ng-container *ngSwitchCase=\"'string'\">\n                    <nof-attachment-property *ngIf=\"isBlob\" [attachment]=\"attachment\"></nof-attachment-property>                    \n                    <ng-container *ngIf=\"!isBlob\">\n                        <div *ngIf=\"!isMultiline\" class=\"value\">\n                            {{formattedValue}}\n                        </div>\n                        <div *ngIf=\"isMultiline\" class=\"value multiline\" [ngStyle]=\"{height : multilineHeight}\">\n                            <pre>{{formattedValue}}</pre>\n                        </div>\n                    </ng-container>\n                </ng-container>\n\n                <ng-container *ngSwitchCase=\"'boolean'\">\n                    <input type=\"checkbox\"  [checked]=\"value\" disabled=\"disabled\" />\n                </ng-container>\n\n                <ng-container *ngSwitchDefault>\n                    <div class=\"value\">\n                        {{formattedValue}}\n                    </div>\n                </ng-container>\n\n            </ng-container>\n        </ng-container>\n\n        <div *ngIf=\"isEditable\" class=\"input-control text\">\n            <ng-container [ngSwitch]=\"propertyEntryType\">\n                <!--\"EntryType.AutoComplete\"-->\n                <ng-container *ngSwitchCase=\"5\">\n                \n                    <md-input-container>\n                        <input #focus [id]=\"propertyPaneId\"  type=\"text\" placeholder=\"{{propertyDescription}}\" mdInput [formControlName]=\"propertyId\" [formControl]=\"control\" [mdAutocomplete]=\"auto\" [geminiClear]=\"property\" [form]=\"form\">\n                    </md-input-container>\n\n                    <md-autocomplete #auto=\"mdAutocomplete\">\n                        <md-option *ngFor=\"let option of propertyChoices\" [value]=\"option\">\n                            {{ option }}\n                        </md-option>\n                    </md-autocomplete>\n\n                </ng-container>\n\n                <!--\"EntryType.ConditionalChoices\"-->\n                <ng-container *ngSwitchCase=\"3\">\n                    <select #focus [id]=\"propertyPaneId\"  [formControlName]=\"propertyId\">\n                        <option *ngFor=\"let choice of propertyChoices\" [ngValue]=\"choice\">{{choiceName(choice)}}</option>\n                    </select>\n                </ng-container>\n\n                <!--\"EntryType.Choices\"-->\n                <ng-container *ngSwitchCase=\"1\">\n                    <select #focus [id]=\"propertyPaneId\"  [formControlName]=\"propertyId\">\n                        <option *ngFor=\"let choice of propertyChoices\" [ngValue]=\"choice\">{{choiceName(choice)}}</option>\n                    </select>\n                </ng-container>\n\n                <!--\"EntryType.FreeForm\"-->\n                <ng-container *ngSwitchCase=\"0\">\n                    <ng-container [ngSwitch]=\"propertyReturnType\">\n                        <ng-container *ngSwitchCase=\"'string'\" [ngSwitch]=\"format\">\n                            <ng-container *ngSwitchCase=\"'date'\">\n                                <input #focus [id]=\"propertyPaneId\"  type=\"date\" placeholder=\"{{propertyDescription}}\" [formControlName]=\"propertyId\"\n                                     (change)=\"datePickerChanged($event)\"  />\n                            </ng-container>\n                            <ng-container *ngSwitchCase=\"'time'\">\n                                <input #focus [id]=\"propertyPaneId\"  type=\"time\" placeholder=\"{{propertyDescription}}\" [formControlName]=\"propertyId\"\n                                     [geminiClear]=\"property\" [form]=\"form\" />\n                            </ng-container>\n                            <ng-container *ngSwitchDefault>\n                                <input #focus *ngIf=\"!isMultiline\" [id]=\"propertyPaneId\"  placeholder=\"{{propertyDescription}}\" type=\"{{isPassword ? 'password' : 'text'}}\"\n                                     [formControlName]=\"propertyId\" [geminiClear]=\"property\" [form]=\"form\" />\n                                <textarea #focus *ngIf=\"isMultiline\" rows=\"{{rows}}\" [id]=\"propertyPaneId\" [formControlName]=\"propertyId\"\n                                    placeholder=\"{{propertyDescription}}\"></textarea>\n                            </ng-container>\n                        </ng-container>\n                        <ng-container *ngSwitchCase=\"'boolean'\">\n                            <input #focus [id]=\"propertyPaneId\"  type=\"checkbox\" [geminiBoolean]=\"property\" />{{propertyDescription}}\n                        </ng-container>\n                        <ng-container *ngSwitchDefault>\n                            <input #focus [id]=\"propertyPaneId\"  type=\"text\" placeholder=\"{{propertyDescription}}\" [formControlName]=\"propertyId\"\n                                [geminiClear]=\"property\" [form]=\"form\" />\n                        </ng-container>\n                    </ng-container>\n                </ng-container>\n            </ng-container>\n        </div>\n    </ng-container>\n    <div class=\"validation\">{{message}}</div>\n    </div>\n</div>"

/***/ }),

/***/ 1355:
/***/ (function(module, exports) {

module.exports = "<div class=\"error\">\n    <div class=\"title\">{{title}}</div>\n    <div class=\"message\">Message: {{message}}</div>\n    <div class=\"code\">Code: {{errorCode}}</div>\n    <div class=\"description\">Description: {{description}}.</div>\n    <div class=\"stacktrace\">\n        Stack Trace :\n        <div class=\"line\" *ngFor=\"let line of stackTrace\">{{line}}</div>\n    </div>\n</div>"

/***/ }),

/***/ 1356:
/***/ (function(module, exports) {

module.exports = "<div class=\"footer\">\n\t<div class=\"icon home\" title=\"Home (Alt-h)\" tabindex=\"0\" geminiClick (leftClick)=\"goHome()\" (rightClick)=\"goHome(true)\" accesskey=\"h\"></div>\n\t<div class=\"icon back\" title=\"Back (Alt-b)\" tabindex=\"0\" (click)=\"goBack()\" accesskey=\"b\"></div>\n\t<div class=\"icon forward\" title=\"Forward (Alt-f)\" tabindex=\"0\" (click)=\"goForward()\" accesskey=\"f\"></div>\n\t<div class=\"icon full\" title=\"Expand pane (Alt-e)\" tabindex=\"0\" geminiClick (leftClick)=\"singlePane()\" (rightClick)=\"singlePane(true)\"\n\t\taccesskey=\"e\"></div>\n\t<div class=\"icon swap\" [attr.disabled]=\"swapDisabled()\" title=\"Swap panes (Alt-s)\" tabindex=\"0\" (click)=\"swapPanes()\" accesskey=\"s\"></div>\n\t<div class=\"icon recent\" title=\"Recent objects (Alt-r)\" tabindex=\"0\" geminiClick (leftClick)=\"recent()\" (rightClick)=\"recent(true)\" accesskey=\"r\"></div>\n\t<div class=\"icon speech\" title=\"Cicero - Speech Interface (Alt-c)\" (click)=\"cicero()\" tabindex=\"0\" accesskey=\"c\"></div>\n\t<div class=\"icon properties\" title=\"Application Properties (Alt-p)\" (click)=\"applicationProperties()\" tabindex=\"0\" accesskey=\"p\"></div>\n\t<div class=\"icon logoff\" title=\"Log off (Alt-l)\" (click)=\"logOff()\" tabindex=\"0\" accesskey=\"l\"></div>\n\t<span class=\"loading\">{{loading}}</span>\n\n\t<div class=\"warnings\" *ngFor=\"let warning of warnings\">\n\t\t<div>{{warning}}</div>\n\t</div>\n\t<div class=\"messages\" *ngFor=\"let message of messages\">\n\t\t<div>{{message}}</div>\n\t</div>\n\t<div *ngIf=\"copyViewModel\" class=\"currentcopy\">\n        <span>Copying...</span>\n        <div class=\"reference\" [ngClass]=\"currentCopyColor\" dnd-draggable [dragEnabled]=\"true\" [dragData]=\"copyViewModel\">{{currentCopyTitle}}</div>\n    </div>\n\t<!--<div class=\"user\">{{userName}}</div>\n    <a style=\"display: none\"></a>-->\n</div>"

/***/ }),

/***/ 1357:
/***/ (function(module, exports) {

module.exports = "<th *ngIf=\"showAllCheckbox()\">\n    <input type=\"checkbox\" [id]=\"itemId()\" [ngModel]=\"allSelected()\" (click)=\"selectAll()\" title=\"All\" />\n</th>\n<ng-container *ngIf=\"header\">\n    <th *ngFor=\"let heading of header\" scope=\"col\">{{heading}}</th>\n</ng-container>\n"

/***/ }),

/***/ 1358:
/***/ (function(module, exports) {

module.exports = "<div [attr.id]=\"paneIdName\" [ngClass]=\"paneType\">\n\t<!--TODO: This is the Single/Split definition-->\n\t<div class=\"home\">\n\t\t<div class=\"header\">\n\t\t\t<div class=\"title\">Home</div>\n            <nof-menu-bar class=\"menus\" *ngIf=\"hasMenus\" [menus]=\"menuItems\"></nof-menu-bar> \n\t\t\t<div class=\"messages\"></div>\n\t\t</div>\n\t\t<nof-action-list *ngIf=\"selectedMenu\" [menuHolder]=\"selectedMenu\"></nof-action-list>\n\t\t<div class=\"main-column\">\n\t\t<nof-dialog *ngIf=\"selectedMenu\" [selectedDialogId]=\"selectedDialogId\" [parent]=\"selectedMenu\" ></nof-dialog>\n\t\t</div>\n\t</div>\n</div>\n<router-outlet (activate)=\"onChild()\" (deactivate)=\"onChildless()\"></router-outlet>\n"

/***/ }),

/***/ 1359:
/***/ (function(module, exports) {

module.exports = "<ng-container *ngIf=\"collection\">\n    <div class=\"list\">\n        <header class=\"header\">\n            <div class=\"title\">{{title}}</div>\n            <nof-action-bar [actions]=\"actionHolders\"></nof-action-bar>\n            <div class=\"messages\">{{message}}</div>\n        </header>\n        <div class=\"details\" *ngIf=\"size == 0\">{{description}}</div>\n        <ng-container *ngIf=\"size > 0\">\n            <nof-action-list *ngIf=\"showActions()\" [menuHolder]=\"collection\"></nof-action-list>\n            <nof-dialog [parent]=\"collection\" [selectedDialogId]=\"selectedDialogId\"></nof-dialog>\n            <div class=\"summary\">\n                <div class=\"details\">{{description}}</div>\n                <div *ngIf=\"state === 'list'\" class=\"icon table\" (click)=\"doTable()\"  title=\"View as Table\" alt=\"View as Table\"></div>\n                <div *ngIf=\"state === 'table'\" class=\"icon list\" (click)=\"doList()\"  title=\"View as List\" alt=\"View as List\"></div>\n             </div>\n             <table>\n                    <thead>\n                        <tr nof-header [collection]=\"collection\" [state]=\"currentState\"></tr>\n                    </thead>\n                    <tbody *ngIf=\"state === 'list' || hasTableData()\">\n                        <tr *ngFor=\"let item of items; let i = index\" nof-row [item]=\"item\" [index]=\"i\" [withCheckbox]=\"!disableActions()\" [isTable]=\"state === 'table'\"></tr>\n                    </tbody>\n                </table>\n        </ng-container>\n    </div>\n</ng-container>"

/***/ }),

/***/ 1360:
/***/ (function(module, exports) {

module.exports = "<div class = \"title\">Welcome to {{configService.config.applicationName}}</div>\n<p>Clicking the login button will generate a pop-up managed by the 'Auth0 service.</p>\n<p>This will allow you to login using your Google, or other recognised account.</p>\n<button class=\"btn btn-primary btn-margin\" (click)=\"auth.login()\">Log In</button>\n"

/***/ }),

/***/ 1361:
/***/ (function(module, exports) {

module.exports = "<nof-action *ngFor=\"let action of actions\" [action]=\"action\"></nof-action>\n"

/***/ }),

/***/ 1362:
/***/ (function(module, exports) {

module.exports = "<div id=\"pane1\" class=\"single\">\n    <div  *ngIf=\"dialog\" class=\"multilinedialog\">\n        <div class=\"header\">\n            <div class=\"type\">{{objectFriendlyName}}</div>\n            <div class=\"title\" gemini-drag tabindex=\"0\">\n                <div>{{objectTitle}}</div>\n                <div>{{dialogTitle}}</div>\n            </div>\n        </div>\n        <div class=\"columnHeader\" *ngFor=\"let column of header\">{{column}}</div>\n        <div class=\"lineDialog\" *ngFor=\"let row of rows; let i = index \">\n            <form *ngIf=\"!rowSubmitted(row)\" (ngSubmit)=\"invokeAndAdd(i)\" [formGroup]=\"form(i)\" autocomplete=\"off\">\n                <nof-parameters  class=\"parameters multilinedialog\" [parameters]=\"parameters(row)\" [parent]=\"row\" [form]=\"form(i)\"></nof-parameters>               \n                <input class=\"ok\" tabindex=\"0\" type=\"submit\" value=\"OK\" title=\"{{rowTooltip(row)}}\" [disabled]=\"rowDisabled(row)\" geminiClick (leftClick)=\"invokeAndAdd(i)\" />\n                <div class=\"co-validation\">{{rowMessage(row)}}</div>\n            </form>\n            <nof-parameters *ngIf=\"rowSubmitted(row)\" class=\"parameters\" [parameters]=\"parameters(row)\" [parent]=\"row\"></nof-parameters>\n            <div *ngIf=\"rowSubmitted(row)\" class=\"co-validation\">{{rowMessage(row)}}</div>\n        </div>\n        <input class=\"close\" tabindex=\"0\" type=\"submit\" value=\"Close\" title=\"\" geminiClick (leftClick)=\"close()\" />\n        <div class=\"count\">{{count}}</div>\n    </div>\n</div>\n"

/***/ }),

/***/ 1363:
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"expiredTransient\" class=\"title\">The requested view of unsaved object details has expired.</div>\n\n<!--place holder-->\n<ng-container *ngIf=\"!object\">\n    <div class=\"object view\" [ngClass]=\"color\"></div>\n</ng-container>\n<div *ngIf=\"object && viewMode === 'View'\" class=\"object view\" [ngClass]=\"color\">\n    <header class=\"header\">\n        <div class=\"title\" dnd-draggable [dragEnabled]=\"true\" [dragData]=\"object\" tabindex=\"0\" (keydown)=\"copy($event)\" (keypress)=\"copy($event)\">\n            <span class=\"type\">{{friendlyName}}</span> {{title()}}\n        </div>\n        <!--<div>{{object.isDirty() ? \"*\" : \"\"}} </div>-->\n        <nof-action-bar [actions]=\"actionHolders\"></nof-action-bar>\n        <div class=\"messages\">{{message()}}</div>\n    </header>\n    <nof-action-list *ngIf=\"showActions()\" [menuHolder]=\"object\"></nof-action-list>\n    <div class=\"main-column\">\n        <nof-dialog [parent]=\"object\" [selectedDialogId]=\"selectedDialogId\"></nof-dialog>\n        <nof-properties class=\"properties\" *ngIf=\"properties\" [properties]=\"properties\"></nof-properties>\n    </div>\n    <nof-collections class=\"collections\" *ngIf=\"collections\" [collections]=\"collections\"></nof-collections>\n</div>\n<div *ngIf=\"object && (viewMode === 'Edit' || viewMode === 'Transient' || viewMode === 'Form')\" class=\"object edit\" [ngClass]=\"color\">\n    <form (ngSubmit)=\"onSubmit()\" [formGroup]=\"form\" autocomplete=\"off\">\n        <header class=\"header\">\n            <div class=\"title\" dnd-draggable [dragEnabled]=\"true\" [dragData]=\"object\" tabindex=\"0\" (keydown)=\"copy($event)\" (keypress)=\"copy($event)\">\n                <span class=\"type\">{{friendlyName}}</span> {{title()}}\n            </div>\n            <div *ngIf=\"viewMode === 'Edit' || viewMode === 'Transient' || viewMode === 'Form'\" class=\"menus\">\n                <nof-action-bar [actions]=\"actionHolders\"></nof-action-bar>\n            </div>\n            <div class=\"messages\">{{message()}}</div>\n        </header>\n        <div class=\"main-column\">\n            <nof-properties *ngIf=\"properties\" [properties]=\"properties\" [parent]=\"object\" [form]=\"form\"></nof-properties>\n        </div>\n    </form>\n    <nof-collections *ngIf=\"collections\" [collections]=\"collections\"></nof-collections>\n</div>\n<div *ngIf=\"object && viewMode === 'NotPersistent'\" class=\"object view\" [ngClass]=\"color\">\n    <header class=\"header\">\n        <div class=\"title\" dnd-draggable [dragEnabled]=\"true\" [dragData]=\"object\" tabindex=\"0\" (keydown)=\"copy($event)\" (keypress)=\"copy($event)\">\n            <span class=\"type\">{{friendlyName}}</span> {{title()}}\n        </div>\n        <div class=\"messages\">{{message()}}</div>\n    </header>\n    <div class=\"main-column\">\n        <nof-properties class=\"properties\" *ngIf=\"properties\" [properties]=\"properties\"></nof-properties>\n    </div>\n    <nof-collections class=\"collections\" *ngIf=\"collections\" [collections]=\"collections\"></nof-collections>\n</div>\n"

/***/ }),

/***/ 1364:
/***/ (function(module, exports) {

module.exports = "<ng-container  *ngIf=\"form\"><nof-edit-parameter  [ngClass]=\"classes()\" *ngFor=\"let parm of parameters\" [parameter]=\"parm\" [parent]=\"parent\" [form]=\"form\"></nof-edit-parameter></ng-container>\n<ng-container *ngIf=\"!form\"><nof-view-parameter [ngClass]=\"classes()\" *ngFor=\"let parm of parameters\" [parameter]=\"parm\" [parent]=\"parent\"></nof-view-parameter></ng-container>"

/***/ }),

/***/ 1365:
/***/ (function(module, exports) {

module.exports = "<ng-container *ngIf=\"form\"><nof-edit-property class=\"property\" *ngFor=\"let prop of properties\" [property]=\"prop\" [parent]=\"parent\" [form]=\"form\"></nof-edit-property></ng-container>\n<ng-container *ngIf=\"!form\"><nof-view-property class=\"property\" *ngFor=\"let prop of properties\" [property]=\"prop\"></nof-view-property></ng-container>\n\n"

/***/ }),

/***/ 1366:
/***/ (function(module, exports) {

module.exports = "<div [attr.id]=\"paneIdName\" [ngClass]=\"paneType\">\n    <div class=\"recent\">\n        <div class=\"header\">\n            <div class=\"title\">{{title}}</div>\n            <nof-action-bar [actions]=\"actionHolders\"></nof-action-bar>\n        </div>\n        <ng-container *ngIf=\"recent\">\n            <div class=\"collection\">\n                <table>\n                    <tbody>\n                        <tr #row *ngFor=\"let item of items(); let i = index\" nof-row [item]=\"item\" [index]=\"i\" [withCheckbox]=\"false\" [isTable]=\"true\" tabindex=\"0\"></tr>\n                    </tbody>\n                </table>\n            </div>\n        </ng-container>\n    </div>\n</div>\n<router-outlet (activate)=\"onChild()\" (deactivate)=\"onChildless()\"></router-outlet>\n"

/***/ }),

/***/ 1367:
/***/ (function(module, exports) {

module.exports = "<td class=\"checkbox\" *ngIf=\"withCheckbox\">\n    <input type=\"checkbox\" [id]=\"id\" [(ngModel)]=\"item.selected\">\n</td>\n<ng-container *ngIf=\"!isTable\">\n    <td #focus class=\"reference\" [ngClass]=\"color\" geminiClick (leftClick)=\"doClick()\" (rightClick)=\"doClick(true)\"\n        dnd-draggable [dragEnabled]=\"true\" [dragData]=\"item\" (keydown)=\"copy($event, item)\" (keypress)=\"copy($event, item)\" tabindex=\"0\">\n        <label attr.for=\"{{id}}\">{{title}}</label>\n    </td>\n</ng-container>\n<ng-container *ngIf=\"isTable\">\n    <td #focus *ngIf=\"hasTableTitle()\" geminiClick (leftClick)=\"doClick()\" (rightClick)=\"doClick(true)\" [ngClass]=\"color\" class=\"cell\">\n        {{tableTitle()}}\n    </td>\n    <td *ngIf=\"friendlyName\" class=\"cell\" [ngClass]=\"color\" geminiClick (leftClick)=\"doClick()\" (rightClick)=\"doClick(true)\"\n        dnd-draggable [dragEnabled]=\"true\" [dragData]=\"item\" (keydown)=\"copy($event, item)\" (keypress)=\"copy($event, item)\">\n        {{friendlyName}}\n    </td>\n    <ng-container *ngIf=\"!friendlyName\">\n        <td #focus *ngFor=\"let property of tableProperties()\" geminiClick (leftClick)=\"doClick()\" (rightClick)=\"doClick(true)\"\n            [ngClass]=\"color\" class=\"cell\" dnd-draggable [dragEnabled]=\"true\" [dragData]=\"item\" (keydown)=\"copy($event, item)\" (keypress)=\"copy($event, item)\">\n            <ng-container [ngSwitch]=\"propertyType(property)\">\n                <ng-container *ngSwitchCase=\"'ref'\">\n                    {{propertyFormattedValue(property)}}\n                </ng-container>\n                <ng-container *ngSwitchCase=\"'scalar'\" [ngSwitch]=\"propertyReturnType(property)\">\n                    <ng-container *ngSwitchCase=\"'boolean'\">\n                        <input type=\"checkbox\" [checked]=\"propertyValue(property)\" disabled=\"disabled\" />\n                    </ng-container>\n                    <ng-container *ngSwitchCase=\"'number'\">\n                        <div class=\"number\">{{propertyFormattedValue(property)}}</div>\n                    </ng-container>\n                    <ng-container *ngSwitchDefault>\n                        <div>{{propertyFormattedValue(property)}}</div>\n                    </ng-container>\n                </ng-container>\n            </ng-container>\n        </td>\n    </ng-container>\n</ng-container>\n"

/***/ }),

/***/ 1368:
/***/ (function(module, exports) {

module.exports = "<div class=\"name\">\n    <label attr.for=\"{{parameterPaneId}}\">{{title}}</label>\n</div>\n\n<ng-container *ngIf=\"parameterType === 'ref'\">\n    <div class=\"value\">{{formattedValue}}</div>\n</ng-container>\n\n<ng-container *ngIf=\"parameterType === 'scalar'\" >\n    <ng-container [ngSwitch]=\"parameterReturnType\">\n        <ng-container *ngSwitchCase=\"string\">\n            <div *ngIf=\"!isMultiline\" class=\"value\">\n                {{formattedValue}}\n            </div>\n            <div *ngIf=\"isMultiline\" class=\"value multiline\" [ngStyle]=\"{height : multilineHeight}\">\n                <pre>{{formattedValue}}</pre>\n            </div>\n        </ng-container>\n\n        <ng-container *ngSwitchCase=\"'boolean'\">\n            <input type=\"checkbox\" [checked]=\"value\" disabled=\"disabled\" />\n        </ng-container>\n\n        <ng-container *ngSwitchDefault>\n            <div class=\"value\">{{formattedValue}}</div>\n        </ng-container>\n    </ng-container>\n</ng-container>\n\n\n\n\n"

/***/ }),

/***/ 1369:
/***/ (function(module, exports) {

module.exports = "<div class=\"name\">{{title}}:</div>\n\n<ng-container *ngIf=\"propertyType === 'ref'\">\n    <ng-container [ngSwitch]=\"propertyRefType\">\n        <div *ngSwitchCase=\"null\"></div>\n        <div *ngSwitchCase=\"'notNavigable'\">\n            <div class=\"value\">{{formattedValue}}</div>\n        </div>\n        <div *ngSwitchDefault class=\"reference\" [ngClass]=\"color\" geminiClick (leftClick)=\"doClick()\"\n             (rightClick)=\"doClick(true)\" dnd-draggable [dragEnabled]=\"true\" [dragData]=\"property\" tabindex=\"0\">\n            {{formattedValue}}\n        </div>\n    </ng-container>\n</ng-container>\n\n<ng-container *ngIf=\"propertyType === 'scalar'\">\n    <ng-container [ngSwitch]=\"propertyReturnType\">\n        <ng-container *ngSwitchCase=\"'string'\">\n\n            <nof-attachment-property *ngIf=\"isBlob\" [attachment]=\"attachment\"></nof-attachment-property>\n            <ng-container *ngIf=\"!isBlob\">\n                <div *ngIf=\"!isMultiline\" class=\"value\">\n                    {{formattedValue}}\n                </div>\n                <div *ngIf=\"isMultiline\" class=\"value multiline\" [ngStyle]=\"{height : multilineHeight}\">\n                    <pre>{{formattedValue}}</pre>\n                </div>\n            </ng-container>\n        </ng-container>\n\n        <ng-container *ngSwitchCase=\"'boolean'\">\n            <input type=\"checkbox\" [checked]=\"value\" disabled=\"disabled\" />\n        </ng-container>\n\n        <ng-container *ngSwitchDefault>\n            <div class=\"value\">{{formattedValue}}</div>\n        </ng-container>\n    </ng-container>\n</ng-container>"

/***/ }),

/***/ 147:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_angular2_jwt__ = __webpack_require__(370);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_angular2_jwt___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_angular2_jwt__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__rxjs_extensions__ = __webpack_require__(189);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__logger_service__ = __webpack_require__(87);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_auth0_lock__ = __webpack_require__(623);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_auth0_lock___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_7_auth0_lock__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AuthService", function() { return AuthService; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Auth0AuthService", function() { return Auth0AuthService; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "NullAuthService", function() { return NullAuthService; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};








var AuthService = (function () {
    function AuthService() {
    }
    return AuthService;
}());
var Auth0AuthService = (function (_super) {
    __extends(Auth0AuthService, _super);
    function Auth0AuthService(router, urlManager, logger, configService) {
        var _this = this;
        _super.call(this);
        this.router = router;
        this.urlManager = urlManager;
        this.logger = logger;
        this.configService = configService;
        var clientId = configService.config.authClientId;
        var domain = configService.config.authDomain;
        if (clientId && domain) {
            var options = {
                auth: {
                    params: { scope: 'openid email' },
                }
            };
            // Configure Auth0
            // this is client id which is public 
            this.lock = new __WEBPACK_IMPORTED_MODULE_7_auth0_lock___default.a(clientId, domain, options);
            // Add callback for lock `authenticated` event
            this.lock.on("authenticated", function (authResult) { return localStorage.setItem('id_token', authResult.idToken); });
            this
                .router
                .events
                .filter(function (event) { return event instanceof __WEBPACK_IMPORTED_MODULE_2__angular_router__["a" /* NavigationStart */]; })
                .filter(function (event) { return (/access_token|id_token|error/).test(event.url); })
                .subscribe(function () {
                _this.lock.resumeAuth(window.location.hash, function (error, authResult) {
                    if (error) {
                        logger.error(error);
                    }
                    else if (authResult && authResult.idToken) {
                        localStorage.setItem('id_token', authResult.idToken);
                        setTimeout(function () { return _this.urlManager.setHomeSinglePane(); });
                    }
                });
            });
        }
    }
    Auth0AuthService.prototype.login = function () {
        // Call the show method to display the widget.
        if (this.lock) {
            this.lock.show();
        }
    };
    Auth0AuthService.prototype.authenticated = function () {
        // Check if there's an unexpired JWT
        // This searches for an item in localStorage with key == 'id_token'
        return __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1_angular2_jwt__["tokenNotExpired"])();
    };
    Auth0AuthService.prototype.logout = function () {
        // Remove token from localStorage
        localStorage.removeItem('id_token');
    };
    Auth0AuthService.prototype.canActivate = function () {
        return this.authenticated();
    };
    Auth0AuthService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__angular_router__["b" /* Router */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__angular_router__["b" /* Router */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_5__logger_service__["a" /* LoggerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__logger_service__["a" /* LoggerService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_6__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__config_service__["a" /* ConfigService */]) === 'function' && _d) || Object])
    ], Auth0AuthService);
    return Auth0AuthService;
    var _a, _b, _c, _d;
}(AuthService));
var NullAuthService = (function (_super) {
    __extends(NullAuthService, _super);
    function NullAuthService() {
        _super.apply(this, arguments);
    }
    NullAuthService.prototype.login = function () { };
    NullAuthService.prototype.authenticated = function () {
        return true;
    };
    NullAuthService.prototype.logout = function () { };
    NullAuthService.prototype.canActivate = function () {
        return true;
    };
    NullAuthService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [])
    ], NullAuthService);
    return NullAuthService;
}(AuthService));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/auth.service.js.map

/***/ }),

/***/ 148:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return PaneComponent; });
var PaneComponent = (function () {
    function PaneComponent(activatedRoute, urlManager) {
        this.activatedRoute = activatedRoute;
        this.urlManager = urlManager;
    }
    PaneComponent.prototype.onChild = function () {
        this.paneType = "split";
    };
    PaneComponent.prototype.onChildless = function () {
        this.paneType = "single";
    };
    PaneComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(function (data) {
            _this.arData = data;
            _this.paneId = data.pane;
            _this.paneType = data.paneType;
            _this.paneIdName = _this.paneId === 1 ? "pane1" : "pane2";
            if (!_this.paneRouteDataSub) {
                _this.paneRouteDataSub =
                    _this.urlManager.getPaneRouteDataObservable(_this.paneId)
                        .subscribe(function (paneRouteData) {
                        if (!paneRouteData.isEqual(_this.lastPaneRouteData)) {
                            _this.lastPaneRouteData = paneRouteData;
                            _this.setup(paneRouteData);
                        }
                    });
            }
            ;
        });
    };
    PaneComponent.prototype.ngOnDestroy = function () {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    };
    return PaneComponent;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/pane.js.map

/***/ }),

/***/ 15:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__constants__ = __webpack_require__(188);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_moment__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_moment___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_moment__);
/* harmony export (immutable) */ __webpack_exports__["y"] = withNull;
/* harmony export (immutable) */ __webpack_exports__["L"] = withUndefined;
/* unused harmony export checkNotNull */
/* harmony export (immutable) */ __webpack_exports__["z"] = dirtyMarker;
/* harmony export (immutable) */ __webpack_exports__["O"] = getOtherPane;
/* harmony export (immutable) */ __webpack_exports__["n"] = toDateString;
/* harmony export (immutable) */ __webpack_exports__["x"] = toTimeString;
/* unused harmony export getUtcDate */
/* unused harmony export getTime */
/* harmony export (immutable) */ __webpack_exports__["m"] = isDateOrDateTime;
/* harmony export (immutable) */ __webpack_exports__["u"] = isTime;
/* harmony export (immutable) */ __webpack_exports__["t"] = toUtcDate;
/* harmony export (immutable) */ __webpack_exports__["v"] = toTime;
/* harmony export (immutable) */ __webpack_exports__["S"] = compress;
/* harmony export (immutable) */ __webpack_exports__["Q"] = decompress;
/* unused harmony export getClassName */
/* unused harmony export typeFromUrl */
/* unused harmony export idFromUrl */
/* harmony export (immutable) */ __webpack_exports__["R"] = propertyIdFromUrl;
/* harmony export (immutable) */ __webpack_exports__["p"] = friendlyTypeName;
/* harmony export (immutable) */ __webpack_exports__["f"] = friendlyNameForParam;
/* harmony export (immutable) */ __webpack_exports__["g"] = friendlyNameForProperty;
/* harmony export (immutable) */ __webpack_exports__["e"] = typePlusTitle;
/* harmony export (immutable) */ __webpack_exports__["r"] = validateMandatory;
/* harmony export (immutable) */ __webpack_exports__["s"] = validate;
/* harmony export (immutable) */ __webpack_exports__["H"] = isResourceRepresentation;
/* harmony export (immutable) */ __webpack_exports__["E"] = isErrorRepresentation;
/* harmony export (immutable) */ __webpack_exports__["G"] = isIDomainObjectRepresentation;
/* harmony export (immutable) */ __webpack_exports__["i"] = isIInvokableAction;
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return ErrorCategory; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "q", function() { return HttpStatusCode; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "c", function() { return ClientErrorCode; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ErrorWrapper; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "d", function() { return ObjectIdWrapper; });
/* unused harmony export HateosModel */
/* unused harmony export ArgumentMap */
/* unused harmony export NestedRepresentation */
/* unused harmony export RelParm */
/* unused harmony export Rel */
/* unused harmony export MediaType */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "j", function() { return Value; });
/* unused harmony export ErrorValue */
/* unused harmony export Result */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "w", function() { return ErrorMap; });
/* unused harmony export UpdateMap */
/* unused harmony export AddToRemoveFromMap */
/* unused harmony export ModifyMap */
/* unused harmony export ClearMap */
/* unused harmony export ResourceRepresentation */
/* unused harmony export Extensions */
/* unused harmony export InvokeMap */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "D", function() { return ActionResultRepresentation; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "k", function() { return Parameter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "B", function() { return ActionRepresentation; });
/* unused harmony export PromptMap */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "J", function() { return PromptRepresentation; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "M", function() { return CollectionRepresentation; });
/* unused harmony export PropertyRepresentation */
/* unused harmony export Member */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "l", function() { return PropertyMember; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "A", function() { return CollectionMember; });
/* unused harmony export ActionMember */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "C", function() { return InvokableActionMember; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "o", function() { return DomainObjectRepresentation; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "P", function() { return MenuRepresentation; });
/* unused harmony export ScalarValueRepresentation */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "N", function() { return ListRepresentation; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "F", function() { return ErrorRepresentation; });
/* unused harmony export PersistMap */
/* unused harmony export VersionRepresentation */
/* unused harmony export DomainServicesRepresentation */
/* unused harmony export MenusRepresentation */
/* unused harmony export UserRepresentation */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "K", function() { return DomainTypeActionInvokeRepresentation; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "I", function() { return HomePageRepresentation; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "T", function() { return Link; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "h", function() { return EntryType; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};




// log directly to avoid coupling back to angular  
function error(message) {
    console.error(message);
    throw new Error(message);
}
// coerce undefined to null
function withNull(v) {
    return v === undefined ? null : v;
}
function withUndefined(v) {
    return v === null ? undefined : v;
}
function validateExists(obj, name) {
    if (obj) {
        return obj;
    }
    error("validateExists - Expected " + name + " does not exist");
}
function getMember(members, id, owner) {
    var member = members[id];
    if (member) {
        return member;
    }
    error("getMember - no member " + id + " on " + owner);
}
function checkNotNull(v) {
    if (v != null) {
        return v;
    }
    error("checkNotNull - Unexpected null");
}
function dirtyMarker(context, configService, oid) {
    return (configService.config.showDirtyFlag && context.getIsDirty(oid)) ? "*" : "";
}
function getOtherPane(paneId) {
    return paneId === 1 ? 2 : 1;
}
function toDateString(dt) {
    var year = dt.getFullYear().toString();
    var month = (dt.getMonth() + 1).toString();
    var day = dt.getDate().toString();
    month = month.length === 1 ? "0" + month : month;
    day = day.length === 1 ? "0" + day : day;
    return year + "-" + month + "-" + day;
}
function toTimeString(dt) {
    var hours = dt.getHours().toString();
    var minutes = dt.getMinutes().toString();
    var seconds = dt.getSeconds().toString();
    hours = hours.length === 1 ? "0" + hours : hours;
    minutes = minutes.length === 1 ? "0" + minutes : minutes;
    seconds = seconds.length === 1 ? "0" + seconds : seconds;
    return hours + ":" + minutes + ":" + seconds;
}
function getUtcDate(rawDate) {
    if (!rawDate || rawDate.length === 0) {
        return null;
    }
    var year = parseInt(rawDate.substring(0, 4));
    var month = parseInt(rawDate.substring(5, 7)) - 1;
    var day = parseInt(rawDate.substring(8, 10));
    if (rawDate.length === 10) {
        return new Date(Date.UTC(year, month, day, 0, 0, 0));
    }
    if (rawDate.length >= 20) {
        var hours = parseInt(rawDate.substring(11, 13));
        var mins = parseInt(rawDate.substring(14, 16));
        var secs = parseInt(rawDate.substring(17, 19));
        return new Date(Date.UTC(year, month, day, hours, mins, secs));
    }
    return null;
}
function getTime(rawTime) {
    if (!rawTime || rawTime.length === 0) {
        return null;
    }
    var hours = parseInt(rawTime.substring(0, 2));
    var mins = parseInt(rawTime.substring(3, 5));
    var secs = parseInt(rawTime.substring(6, 8));
    return new Date(1970, 0, 1, hours, mins, secs);
}
function isDateOrDateTime(rep) {
    var returnType = rep.extensions().returnType();
    var format = rep.extensions().format();
    return (returnType === "string" && ((format === "date-time") || (format === "date")));
}
function isTime(rep) {
    var returnType = rep.extensions().returnType();
    var format = rep.extensions().format();
    return returnType === "string" && format === "time";
}
function toUtcDate(value) {
    var rawValue = value ? value.toString() : "";
    var dateValue = getUtcDate(rawValue);
    return dateValue ? dateValue : null;
}
function toTime(value) {
    var rawValue = value ? value.toString() : "";
    var dateValue = getTime(rawValue);
    return dateValue ? dateValue : null;
}
function compress(toCompress, shortCutMarker, urlShortCuts) {
    if (toCompress) {
        __WEBPACK_IMPORTED_MODULE_2_lodash__["forEach"](urlShortCuts, function (sc, i) { return toCompress = toCompress.replace(sc, "" + shortCutMarker + i); });
    }
    return toCompress;
}
function decompress(toDecompress, shortCutMarker, urlShortCuts) {
    if (toDecompress) {
        __WEBPACK_IMPORTED_MODULE_2_lodash__["forEach"](urlShortCuts, function (sc, i) { return toDecompress = toDecompress.replace("" + shortCutMarker + i, sc); });
    }
    return toDecompress;
}
function getClassName(obj) {
    var funcNameRegex = /function (.{1,})\(/;
    var results = (funcNameRegex).exec(obj.constructor.toString());
    return (results && results.length > 1) ? results[1] : "";
}
function typeFromUrl(url) {
    var typeRegex = /(objects|services)\/([\w|\.]+)/;
    var results = (typeRegex).exec(url);
    return (results && results.length > 2) ? results[2] : "";
}
function idFromUrl(href) {
    var urlRegex = /(objects|services)\/(.*?)\/([^\/]*)/;
    var results = (urlRegex).exec(href);
    return (results && results.length > 3) ? results[3] : "";
}
function propertyIdFromUrl(href) {
    var urlRegex = /(objects)\/(.*)\/(.*)\/(properties)\/(.*)/;
    var results = (urlRegex).exec(href);
    return (results && results.length > 5) ? results[5] : "";
}
function friendlyTypeName(fullName) {
    var shortName = __WEBPACK_IMPORTED_MODULE_2_lodash__["last"](fullName.split("."));
    var result = shortName.replace(/([A-Z])/g, " $1").trim();
    return result.charAt(0).toUpperCase() + result.slice(1);
}
function friendlyNameForParam(action, parmId) {
    var param = __WEBPACK_IMPORTED_MODULE_2_lodash__["find"](action.parameters(), function (p) { return p.id() === parmId; });
    return param ? param.extensions().friendlyName() : "";
}
function friendlyNameForProperty(obj, propId) {
    var prop = obj.propertyMember(propId);
    return prop.extensions().friendlyName();
}
function typePlusTitle(obj) {
    var type = obj.extensions().friendlyName();
    var title = obj.title();
    return type + ": " + title;
}
function isInteger(value) {
    return typeof value === "number" && isFinite(value) && Math.floor(value) === value;
}
function validateNumber(model, newValue, filter) {
    var format = model.extensions().format();
    switch (format) {
        case ("int"):
            if (!isInteger(newValue)) {
                return "Not an integer";
            }
    }
    var range = model.extensions().range();
    if (range) {
        var min = range.min;
        var max = range.max;
        if (min && newValue < min) {
            return __WEBPACK_IMPORTED_MODULE_1__user_messages__["a" /* outOfRange */](newValue, min, max, filter);
        }
        if (max && newValue > max) {
            return __WEBPACK_IMPORTED_MODULE_1__user_messages__["a" /* outOfRange */](newValue, min, max, filter);
        }
    }
    return "";
}
function validateStringFormat(model, newValue) {
    var maxLength = model.extensions().maxLength();
    var pattern = model.extensions().pattern();
    var len = newValue ? newValue.length : 0;
    if (maxLength && len > maxLength) {
        return __WEBPACK_IMPORTED_MODULE_1__user_messages__["b" /* tooLong */];
    }
    if (pattern) {
        var regex = new RegExp(pattern);
        return regex.test(newValue) ? "" : __WEBPACK_IMPORTED_MODULE_1__user_messages__["c" /* noPatternMatch */];
    }
    return "";
}
function validateDateTimeFormat(model, newValue) {
    return "";
}
function getDate(val) {
    var dt1 = __WEBPACK_IMPORTED_MODULE_3_moment__(val, "YYYY-MM-DD", true);
    return dt1.isValid() ? dt1.toDate() : null;
}
function validateDateFormat(model, newValue, filter) {
    var range = model.extensions().range();
    var newDate = (newValue instanceof Date) ? newValue : getDate(newValue);
    if (range && newDate) {
        var min = range.min ? getDate(range.min) : null;
        var max = range.max ? getDate(range.max) : null;
        if (min && newDate < min) {
            return __WEBPACK_IMPORTED_MODULE_1__user_messages__["a" /* outOfRange */](toDateString(newDate), getUtcDate(range.min), getUtcDate(range.max), filter);
        }
        if (max && newDate > max) {
            return __WEBPACK_IMPORTED_MODULE_1__user_messages__["a" /* outOfRange */](toDateString(newDate), getUtcDate(range.min), getUtcDate(range.max), filter);
        }
    }
    return "";
}
function validateTimeFormat(model, newValue) {
    return "";
}
function validateString(model, newValue, filter) {
    var format = model.extensions().format();
    switch (format) {
        case ("string"):
            return validateStringFormat(model, newValue);
        case ("date-time"):
            return validateDateTimeFormat(model, newValue);
        case ("date"):
            return validateDateFormat(model, newValue, filter);
        case ("time"):
            return validateTimeFormat(model, newValue);
        default:
            return "";
    }
}
function validateMandatory(model, viewValue) {
    // first check 
    var isMandatory = !model.extensions().optional();
    if (isMandatory && (viewValue === "" || viewValue == null)) {
        return __WEBPACK_IMPORTED_MODULE_1__user_messages__["d" /* mandatory */];
    }
    return "";
}
function validate(model, modelValue, viewValue, filter) {
    // first check 
    var mandatory = validateMandatory(model, viewValue);
    if (mandatory) {
        return mandatory;
    }
    // if optional but empty always valid 
    if (modelValue == null || modelValue === "") {
        return "";
    }
    // check type 
    var returnType = model.extensions().returnType();
    switch (returnType) {
        case ("number"):
            var valueAsNumber = parseFloat(viewValue);
            if (Number.isFinite(valueAsNumber)) {
                return validateNumber(model, valueAsNumber, filter);
            }
            return __WEBPACK_IMPORTED_MODULE_1__user_messages__["e" /* notANumber */];
        case ("string"):
            return validateString(model, viewValue, filter);
        case ("boolean"):
            return "";
        default:
            return "";
    }
}
// helper functions 
function isScalarType(typeName) {
    return typeName === "string" || typeName === "number" || typeName === "boolean" || typeName === "integer";
}
function isListType(typeName) {
    return typeName === "list";
}
function emptyResource() {
    return { links: [], extensions: {} };
}
function isILink(object) {
    return object && object instanceof Object && "href" in object;
}
function isIObjectOfType(object) {
    return object && object instanceof Object && "members" in object;
}
function isIValue(object) {
    return object && object instanceof Object && "value" in object;
}
function isResourceRepresentation(object) {
    return object && object instanceof Object && "links" in object && "extensions" in object;
}
function isErrorRepresentation(object) {
    return isResourceRepresentation(object) && "message" in object;
}
function isIDomainObjectRepresentation(object) {
    return isResourceRepresentation(object) && "domainType" in object && "instanceId" in object && "members" in object;
}
function isIInvokableAction(object) {
    return object && "parameters" in object && "extensions" in object;
}
function getId(prop) {
    if (prop instanceof PropertyRepresentation) {
        return prop.instanceId();
    }
    else {
        return prop.id();
    }
}
function wrapLinks(links) {
    return __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](links, function (l) { return new Link(l); });
}
function getLinkByRel(links, rel) {
    return __WEBPACK_IMPORTED_MODULE_2_lodash__["find"](links, function (i) { return i.rel().uniqueValue === rel.uniqueValue; });
}
function linkByRel(links, rel) {
    return getLinkByRel(links, new Rel(rel));
}
function linkByNamespacedRel(links, rel) {
    return getLinkByRel(links, new Rel("urn:org.restfulobjects:rels/" + rel));
}
var ErrorCategory;
(function (ErrorCategory) {
    ErrorCategory[ErrorCategory["HttpClientError"] = 0] = "HttpClientError";
    ErrorCategory[ErrorCategory["HttpServerError"] = 1] = "HttpServerError";
    ErrorCategory[ErrorCategory["ClientError"] = 2] = "ClientError";
})(ErrorCategory || (ErrorCategory = {}));
var HttpStatusCode;
(function (HttpStatusCode) {
    HttpStatusCode[HttpStatusCode["NoContent"] = 204] = "NoContent";
    HttpStatusCode[HttpStatusCode["BadRequest"] = 400] = "BadRequest";
    HttpStatusCode[HttpStatusCode["Unauthorized"] = 401] = "Unauthorized";
    HttpStatusCode[HttpStatusCode["Forbidden"] = 403] = "Forbidden";
    HttpStatusCode[HttpStatusCode["NotFound"] = 404] = "NotFound";
    HttpStatusCode[HttpStatusCode["MethodNotAllowed"] = 405] = "MethodNotAllowed";
    HttpStatusCode[HttpStatusCode["NotAcceptable"] = 406] = "NotAcceptable";
    HttpStatusCode[HttpStatusCode["PreconditionFailed"] = 412] = "PreconditionFailed";
    HttpStatusCode[HttpStatusCode["UnprocessableEntity"] = 422] = "UnprocessableEntity";
    HttpStatusCode[HttpStatusCode["PreconditionRequired"] = 428] = "PreconditionRequired";
    HttpStatusCode[HttpStatusCode["InternalServerError"] = 500] = "InternalServerError";
})(HttpStatusCode || (HttpStatusCode = {}));
var ClientErrorCode;
(function (ClientErrorCode) {
    ClientErrorCode[ClientErrorCode["ExpiredTransient"] = 0] = "ExpiredTransient";
    ClientErrorCode[ClientErrorCode["WrongType"] = 1] = "WrongType";
    ClientErrorCode[ClientErrorCode["NotImplemented"] = 2] = "NotImplemented";
    ClientErrorCode[ClientErrorCode["SoftwareError"] = 3] = "SoftwareError";
    ClientErrorCode[ClientErrorCode["ConnectionProblem"] = 0] = "ConnectionProblem";
})(ClientErrorCode || (ClientErrorCode = {}));
var ErrorWrapper = (function () {
    function ErrorWrapper(category, code, err, originalUrl) {
        this.category = category;
        this.originalUrl = originalUrl;
        this.handled = false;
        if (category === ErrorCategory.ClientError) {
            this.clientErrorCode = code;
            this.errorCode = ClientErrorCode[this.clientErrorCode];
            var description = __WEBPACK_IMPORTED_MODULE_1__user_messages__["f" /* errorUnknown */];
            switch (this.clientErrorCode) {
                case ClientErrorCode.ExpiredTransient:
                    description = __WEBPACK_IMPORTED_MODULE_1__user_messages__["g" /* errorExpiredTransient */];
                    break;
                case ClientErrorCode.WrongType:
                    description = __WEBPACK_IMPORTED_MODULE_1__user_messages__["h" /* errorWrongType */];
                    break;
                case ClientErrorCode.NotImplemented:
                    description = __WEBPACK_IMPORTED_MODULE_1__user_messages__["i" /* errorNotImplemented */];
                    break;
                case ClientErrorCode.SoftwareError:
                    description = __WEBPACK_IMPORTED_MODULE_1__user_messages__["j" /* errorSoftware */];
                    break;
                case ClientErrorCode.ConnectionProblem:
                    description = __WEBPACK_IMPORTED_MODULE_1__user_messages__["k" /* errorConnection */];
                    break;
            }
            this.description = description;
            this.title = __WEBPACK_IMPORTED_MODULE_1__user_messages__["l" /* errorClient */];
        }
        if (category === ErrorCategory.HttpClientError || category === ErrorCategory.HttpServerError) {
            this.httpErrorCode = code;
            this.errorCode = HttpStatusCode[this.httpErrorCode] + "(" + this.httpErrorCode + ")";
            this.description = category === ErrorCategory.HttpServerError
                ? "A software error has occurred on the server"
                : "An HTTP error code has been received from the server\n" +
                    "You can look up the meaning of this code in the Restful Objects specification.";
            this.title = "Error message received from server";
        }
        if (err instanceof ErrorMap) {
            var em = err;
            this.message = em.invalidReason() || em.warningMessage;
            this.error = em;
            this.stackTrace = [];
        }
        else if (err instanceof ErrorRepresentation) {
            var er = err;
            this.message = er.message();
            this.error = er;
            this.stackTrace = err.stackTrace();
        }
        else {
            this.message = err;
            this.error = null;
            this.stackTrace = [];
        }
    }
    return ErrorWrapper;
}());
// abstract classes 
function toOid(id, keySeparator) {
    return __WEBPACK_IMPORTED_MODULE_2_lodash__["reduce"](id, function (a, v) { return ("" + a + keySeparator + v); });
}
var ObjectIdWrapper = (function () {
    function ObjectIdWrapper(keySeparator) {
        this.keySeparator = keySeparator;
        if (keySeparator == null) {
            error("ObjectIdWrapper must have a keySeparator");
        }
    }
    ObjectIdWrapper.prototype.getKey = function () {
        return this.domainType + this.keySeparator + this.instanceId;
    };
    ObjectIdWrapper.safeSplit = function (id, keySeparator) {
        return id ? id.split(keySeparator) : [];
    };
    ObjectIdWrapper.fromObject = function (object, keySeparator) {
        var oid = new ObjectIdWrapper(keySeparator);
        oid.domainType = object.domainType() || "";
        oid.instanceId = object.instanceId() || "";
        oid.splitInstanceId = this.safeSplit(oid.instanceId, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    };
    ObjectIdWrapper.fromLink = function (link, keySeparator) {
        var href = link.href();
        return this.fromHref(href, keySeparator);
    };
    ObjectIdWrapper.fromHref = function (href, keySeparator) {
        var oid = new ObjectIdWrapper(keySeparator);
        oid.domainType = typeFromUrl(href);
        oid.instanceId = idFromUrl(href);
        oid.splitInstanceId = this.safeSplit(oid.instanceId, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    };
    ObjectIdWrapper.fromObjectId = function (objectId, keySeparator) {
        var oid = new ObjectIdWrapper(keySeparator);
        var _a = objectId.split(keySeparator), dt = _a[0], id = _a.slice(1);
        oid.domainType = dt;
        oid.splitInstanceId = id;
        oid.instanceId = toOid(id, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    };
    ObjectIdWrapper.fromRaw = function (dt, id, keySeparator) {
        var oid = new ObjectIdWrapper(keySeparator);
        oid.domainType = dt;
        oid.instanceId = id;
        oid.splitInstanceId = this.safeSplit(oid.instanceId, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    };
    ObjectIdWrapper.fromSplitRaw = function (dt, id, keySeparator) {
        var oid = new ObjectIdWrapper(keySeparator);
        oid.domainType = dt;
        oid.splitInstanceId = id;
        oid.instanceId = toOid(id, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    };
    ObjectIdWrapper.prototype.isSame = function (other) {
        return other && other.domainType === this.domainType && other.instanceId === this.instanceId;
    };
    return ObjectIdWrapper;
}());
var HateosModel = (function () {
    function HateosModel(model) {
        this.model = model;
        this.hateoasUrl = "";
        this.method = "GET";
    }
    HateosModel.prototype.populate = function (model) {
        this.model = model;
    };
    HateosModel.prototype.getBody = function () {
        if (this.method === "POST" || this.method === "PUT") {
            var m = __WEBPACK_IMPORTED_MODULE_2_lodash__["clone"](this.model);
            var up = __WEBPACK_IMPORTED_MODULE_2_lodash__["clone"](this.urlParms);
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["merge"](m, up);
        }
        return {};
    };
    HateosModel.prototype.getUrl = function () {
        var url = this.hateoasUrl;
        var attrAsJson = __WEBPACK_IMPORTED_MODULE_2_lodash__["clone"](this.model);
        if (this.method === "GET" || this.method === "DELETE") {
            if (__WEBPACK_IMPORTED_MODULE_2_lodash__["keys"](attrAsJson).length > 0) {
                // there are model parms so encode everything into json 
                var urlParmsAsJson = __WEBPACK_IMPORTED_MODULE_2_lodash__["clone"](this.urlParms);
                var asJson = __WEBPACK_IMPORTED_MODULE_2_lodash__["merge"](attrAsJson, urlParmsAsJson);
                if (__WEBPACK_IMPORTED_MODULE_2_lodash__["keys"](asJson).length > 0) {
                    var map = JSON.stringify(asJson);
                    var parmString = encodeURI(map);
                    return url + "?" + parmString;
                }
                return url;
            }
            if (__WEBPACK_IMPORTED_MODULE_2_lodash__["keys"](this.urlParms).length > 0) {
                // there are only url reserved parms so they can just be appended to url
                var urlParmString = __WEBPACK_IMPORTED_MODULE_2_lodash__["reduce"](this.urlParms, function (result, n, key) { return (result === "" ? "" : result + "&") + key + "=" + n; }, "");
                return url + "?" + urlParmString;
            }
        }
        return url;
    };
    HateosModel.prototype.setUrlParameter = function (name, value) {
        this.urlParms = this.urlParms || {};
        this.urlParms[name] = value;
    };
    return HateosModel;
}());
var ArgumentMap = (function (_super) {
    __extends(ArgumentMap, _super);
    function ArgumentMap(map, id) {
        _super.call(this, map);
        this.map = map;
        this.id = id;
    }
    ArgumentMap.prototype.populate = function (wrapped) {
        _super.prototype.populate.call(this, wrapped);
    };
    return ArgumentMap;
}(HateosModel));
var NestedRepresentation = (function () {
    function NestedRepresentation(model) {
        var _this = this;
        this.model = model;
        this.resource = function () { return _this.model; };
    }
    NestedRepresentation.prototype.links = function () {
        this.lazyLinks = this.lazyLinks || wrapLinks(this.resource().links);
        return this.lazyLinks;
    };
    NestedRepresentation.prototype.update = function (newResource) {
        this.model = newResource;
        this.lazyLinks = null;
    };
    NestedRepresentation.prototype.extensions = function () {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.model.extensions);
        return this.lazyExtensions;
    };
    return NestedRepresentation;
}());
// classes
var RelParm = (function () {
    function RelParm(asString) {
        this.asString = asString;
        this.decomposeParm();
    }
    RelParm.prototype.decomposeParm = function () {
        var regex = /(\w+)\W+(\w+)\W+/;
        var result = regex.exec(this.asString) || [];
        this.name = result[1], this.value = result[2];
    };
    return RelParm;
}());
var Rel = (function () {
    function Rel(asString) {
        this.asString = asString;
        this.ns = "";
        this.parms = [];
        this.decomposeRel();
    }
    Rel.prototype.decomposeRel = function () {
        var postFix;
        if (this.asString.substring(0, 3) === "urn") {
            // namespaced 
            this.ns = this.asString.substring(0, this.asString.indexOf("/") + 1);
            postFix = this.asString.substring(this.asString.indexOf("/") + 1);
        }
        else {
            postFix = this.asString;
        }
        var splitPostFix = postFix.split(";");
        this.uniqueValue = splitPostFix[0];
        if (splitPostFix.length > 1) {
            this.parms = __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](splitPostFix.slice(1), function (s) { return new RelParm(s); });
        }
    };
    return Rel;
}());
var MediaType = (function () {
    function MediaType(asString) {
        this.asString = asString;
        this.decomposeMediaType();
    }
    MediaType.prototype.decomposeMediaType = function () {
        var parms = this.asString.split(";");
        if (parms.length > 0) {
            this.applicationType = parms[0];
        }
        for (var i = 1; i < parms.length; i++) {
            if (parms[i].trim().substring(0, 7) === "profile") {
                this.profile = parms[i].trim();
                var profileValue = (this.profile.split("=")[1].replace(/\"/g, "")).trim();
                this.representationType = (profileValue.split("/")[1]).trim();
            }
            if (parms[i].trim().substring(0, 16) === __WEBPACK_IMPORTED_MODULE_0__constants__["a" /* roDomainType */]) {
                this.xRoDomainType = (parms[i]).trim();
                this.domainType = (this.xRoDomainType.split("=")[1].replace(/\"/g, "")).trim();
            }
        }
    };
    return MediaType;
}());
var Value = (function () {
    function Value(raw) {
        // can only be Link, number, boolean, string or null    
        if (raw instanceof Array) {
            this.wrapped = raw;
        }
        else if (raw instanceof Link) {
            this.wrapped = raw;
        }
        else if (isILink(raw)) {
            this.wrapped = new Link(raw);
        }
        else {
            this.wrapped = raw;
        }
    }
    Value.prototype.isBlob = function () {
        return this.wrapped instanceof Blob;
    };
    Value.prototype.isScalar = function () {
        return !this.isReference() && !this.isList();
    };
    Value.prototype.isReference = function () {
        return this.wrapped instanceof Link;
    };
    Value.prototype.isFileReference = function () {
        var href = this.href();
        return href ? href.indexOf("data") === 0 : false;
    };
    Value.prototype.isList = function () {
        return this.wrapped instanceof Array;
    };
    Value.prototype.isNull = function () {
        return this.wrapped == null;
    };
    Value.prototype.blob = function () {
        return this.isBlob() ? this.wrapped : null;
    };
    Value.prototype.link = function () {
        return this.isReference() ? this.wrapped : null;
    };
    Value.prototype.href = function () {
        var link = this.link();
        return link ? link.href() : null;
    };
    Value.prototype.scalar = function () {
        return this.isScalar() ? this.wrapped : null;
    };
    Value.prototype.list = function () {
        return this.isList() ? __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](this.wrapped, function (i) { return new Value(i); }) : null;
    };
    Value.prototype.toString = function () {
        if (this.isReference()) {
            return this.link().title() || ""; // know true
        }
        if (this.isList()) {
            var list = this.list(); // know true
            var ss = __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](list, function (v) { return v.toString(); });
            return ss.length === 0 ? "" : __WEBPACK_IMPORTED_MODULE_2_lodash__["reduce"](ss, function (m, s) { return m + "-" + s; }, "");
        }
        return (this.wrapped == null) ? "" : this.wrapped.toString();
    };
    // todo this modifies the object - fix so it doesn't  
    Value.prototype.compress = function (shortCutMarker, urlShortCuts) {
        if (this.isReference()) {
            this.link().compress(shortCutMarker, urlShortCuts); // know true
        }
        if (this.isList()) {
            var list = this.list(); // know true
            __WEBPACK_IMPORTED_MODULE_2_lodash__["forEach"](list, function (i) { return i.compress(shortCutMarker, urlShortCuts); });
        }
        ;
        if (this.scalar() && this.wrapped instanceof String) {
            this.wrapped = compress(this.wrapped, shortCutMarker, urlShortCuts);
        }
    };
    Value.prototype.decompress = function (shortCutMarker, urlShortCuts) {
        if (this.isReference()) {
            this.link().decompress(shortCutMarker, urlShortCuts); // know true
        }
        if (this.isList()) {
            var list = this.list(); // know true
            __WEBPACK_IMPORTED_MODULE_2_lodash__["forEach"](list, function (i) { return i.decompress(shortCutMarker, urlShortCuts); });
        }
        ;
        if (this.scalar() && this.wrapped instanceof String) {
            this.wrapped = decompress(this.wrapped, shortCutMarker, urlShortCuts);
        }
    };
    Value.fromJsonString = function (jsonString, shortCutMarker, urlShortCuts) {
        var value = new Value(JSON.parse(jsonString));
        value.decompress(shortCutMarker, urlShortCuts);
        return value;
    };
    Value.prototype.toValueString = function () {
        if (this.isReference()) {
            return this.link().href(); // know true
        }
        return (this.wrapped == null) ? "" : this.wrapped.toString();
    };
    Value.prototype.toJsonString = function (shortCutMarker, urlShortCuts) {
        var cloneThis = __WEBPACK_IMPORTED_MODULE_2_lodash__["cloneDeep"](this);
        cloneThis.compress(shortCutMarker, urlShortCuts);
        var value = cloneThis.wrapped;
        var raw = (value instanceof Link) ? value.wrapped : value;
        return JSON.stringify(raw);
    };
    Value.prototype.setValue = function (target) {
        if (this.isFileReference()) {
            target.value = this.link().wrapped; // know true
        }
        else if (this.isReference()) {
            target.value = { "href": this.link().href() }; // know true
        }
        else if (this.isList()) {
            var list = this.list(); // know true
            target.value = __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](list, function (v) { return v.isReference() ? { "href": v.link().href() } : v.scalar(); });
        }
        else if (this.isBlob()) {
            target.value = this.blob();
        }
        else {
            target.value = this.scalar();
        }
    };
    Value.prototype.set = function (target, name) {
        var t = target[name] = { value: null };
        this.setValue(t);
    };
    return Value;
}());
var ErrorValue = (function () {
    function ErrorValue(value, invalidReason) {
        this.value = value;
        this.invalidReason = invalidReason;
    }
    return ErrorValue;
}());
var Result = (function () {
    function Result(wrapped, resultType) {
        this.wrapped = wrapped;
        this.resultType = resultType;
    }
    Result.prototype.object = function () {
        if (!this.isNull() && this.resultType === "object") {
            return new DomainObjectRepresentation(this.wrapped);
        }
        return null;
    };
    Result.prototype.list = function () {
        if (!this.isNull() && this.resultType === "list") {
            return new ListRepresentation(this.wrapped);
        }
        return null;
    };
    Result.prototype.scalar = function () {
        if (!this.isNull() && this.resultType === "scalar") {
            return new ScalarValueRepresentation(this.wrapped);
        }
        return null;
    };
    Result.prototype.isNull = function () {
        return this.wrapped == null;
    };
    Result.prototype.isVoid = function () {
        return (this.resultType === "void");
    };
    return Result;
}());
var ErrorMap = (function () {
    function ErrorMap(map, statusCode, warningMessage) {
        var _this = this;
        this.map = map;
        this.statusCode = statusCode;
        this.warningMessage = warningMessage;
        this.wrapped = function () {
            var temp = _this.map;
            if (isIObjectOfType(temp)) {
                return temp.members;
            }
            else {
                return temp;
            }
        };
    }
    ErrorMap.prototype.valuesMap = function () {
        var values = __WEBPACK_IMPORTED_MODULE_2_lodash__["pickBy"](this.wrapped(), function (i) { return isIValue(i); });
        return __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](values, function (v) { return new ErrorValue(new Value(v.value), withNull(v.invalidReason)); });
    };
    ErrorMap.prototype.invalidReason = function () {
        var temp = this.map;
        if (isIObjectOfType(temp)) {
            return temp[__WEBPACK_IMPORTED_MODULE_0__constants__["b" /* roInvalidReason */]];
        }
        return this.wrapped()[__WEBPACK_IMPORTED_MODULE_0__constants__["b" /* roInvalidReason */]];
    };
    ErrorMap.prototype.containsError = function () {
        return !!this.invalidReason() || !!this.warningMessage || __WEBPACK_IMPORTED_MODULE_2_lodash__["some"](this.valuesMap(), function (ev) { return !!ev.invalidReason; });
    };
    return ErrorMap;
}());
var UpdateMap = (function (_super) {
    __extends(UpdateMap, _super);
    function UpdateMap(domainObject, map) {
        var _this = this;
        _super.call(this, map, checkNotNull(domainObject.instanceId()));
        this.domainObject = domainObject;
        var link = domainObject.updateLink();
        if (link) {
            link.copyToHateoasModel(this);
        }
        else {
            error("UpdateMap - attempting to create update map for object without update link");
        }
        __WEBPACK_IMPORTED_MODULE_2_lodash__["each"](this.properties(), function (value, key) {
            _this.setProperty(key, value);
        });
    }
    UpdateMap.prototype.properties = function () {
        return __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](this.map, function (v) { return new Value(v.value); });
    };
    UpdateMap.prototype.setProperty = function (name, value) {
        value.set(this.map, name);
    };
    UpdateMap.prototype.setValidateOnly = function () {
        this.map[__WEBPACK_IMPORTED_MODULE_0__constants__["c" /* roValidateOnly */]] = true;
    };
    return UpdateMap;
}(ArgumentMap));
var AddToRemoveFromMap = (function (_super) {
    __extends(AddToRemoveFromMap, _super);
    function AddToRemoveFromMap(collectionResource, map, add) {
        _super.call(this, map, collectionResource.collectionId());
        this.collectionResource = collectionResource;
        var link = add ? collectionResource.addToLink() : collectionResource.removeFromLink();
        if (link) {
            link.copyToHateoasModel(this);
        }
        else {
            var type = add ? "add" : "remove";
            error("AddToRemoveFromMap attempting to create " + type + " map for object without " + type + " link");
        }
    }
    return AddToRemoveFromMap;
}(ArgumentMap));
var ModifyMap = (function (_super) {
    __extends(ModifyMap, _super);
    function ModifyMap(propertyResource, map) {
        _super.call(this, map, getId(propertyResource));
        this.propertyResource = propertyResource;
        var link = propertyResource.modifyLink();
        if (link) {
            link.copyToHateoasModel(this);
        }
        else {
            error("ModifyMap attempting to create modify map for object without modify link");
        }
        propertyResource.value().set(this.map, this.id);
    }
    return ModifyMap;
}(ArgumentMap));
var ClearMap = (function (_super) {
    __extends(ClearMap, _super);
    function ClearMap(propertyResource) {
        _super.call(this, {}, getId(propertyResource));
        var link = propertyResource.clearLink();
        if (link) {
            link.copyToHateoasModel(this);
        }
        else {
            error("ClearMap attempting to create clear map for object without clear link");
        }
    }
    return ClearMap;
}(ArgumentMap));
// REPRESENTATIONS
var ResourceRepresentation = (function (_super) {
    __extends(ResourceRepresentation, _super);
    function ResourceRepresentation() {
        var _this = this;
        _super.apply(this, arguments);
        this.resource = function () { return _this.model; };
    }
    ResourceRepresentation.prototype.populate = function (wrapped) {
        _super.prototype.populate.call(this, wrapped);
    };
    ResourceRepresentation.prototype.links = function () {
        this.lazyLinks = this.lazyLinks || wrapLinks(this.resource().links);
        return this.lazyLinks;
    };
    ResourceRepresentation.prototype.extensions = function () {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.resource().extensions);
        return this.lazyExtensions;
    };
    return ResourceRepresentation;
}(HateosModel));
var Extensions = (function () {
    function Extensions(wrapped) {
        var _this = this;
        this.wrapped = wrapped;
        //Standard RO:
        this.friendlyName = function () { return _this.wrapped.friendlyName || ""; };
        this.description = function () { return _this.wrapped.description || ""; };
        this.returnType = function () { return _this.wrapped.returnType || null; };
        this.optional = function () { return _this.wrapped.optional || false; };
        this.hasParams = function () { return _this.wrapped.hasParams || false; };
        this.elementType = function () { return _this.wrapped.elementType || null; };
        this.domainType = function () { return _this.wrapped.domainType || null; };
        this.pluralName = function () { return _this.wrapped.pluralName || ""; };
        this.format = function () { return _this.wrapped.format; };
        this.memberOrder = function () { return _this.wrapped.memberOrder; };
        this.isService = function () { return _this.wrapped.isService || false; };
        this.minLength = function () { return _this.wrapped.minLength; };
        this.maxLength = function () { return _this.wrapped.maxLength; };
        this.pattern = function () { return _this.wrapped.pattern; };
        //Nof custom:
        this.choices = function () { return _this.wrapped["x-ro-nof-choices"]; };
        this.menuPath = function () { return _this.wrapped["x-ro-nof-menuPath"]; };
        this.mask = function () { return _this.wrapped["x-ro-nof-mask"]; };
        this.tableViewTitle = function () { return _this.wrapped["x-ro-nof-tableViewTitle"]; };
        this.tableViewColumns = function () { return _this.wrapped["x-ro-nof-tableViewColumns"]; };
        this.multipleLines = function () { return _this.wrapped["x-ro-nof-multipleLines"]; };
        this.warnings = function () { return _this.wrapped["x-ro-nof-warnings"]; };
        this.messages = function () { return _this.wrapped["x-ro-nof-messages"]; };
        this.interactionMode = function () { return _this.wrapped["x-ro-nof-interactionMode"]; };
        this.dataType = function () { return _this.wrapped["x-ro-nof-dataType"]; };
        this.range = function () { return _this.wrapped["x-ro-nof-range"]; };
        this.notNavigable = function () { return _this.wrapped["x-ro-nof-notNavigable"]; };
        this.renderEagerly = function () { return _this.wrapped["x-ro-nof-renderEagerly"]; };
        this.presentationHint = function () { return _this.wrapped["x-ro-nof-presentationHint"]; };
    }
    return Extensions;
}());
// matches a action invoke resource 19.0 representation 
var InvokeMap = (function (_super) {
    __extends(InvokeMap, _super);
    function InvokeMap(link) {
        _super.call(this, link.arguments(), "");
        this.link = link;
        link.copyToHateoasModel(this);
    }
    InvokeMap.prototype.setParameter = function (name, value) {
        value.set(this.map, name);
    };
    return InvokeMap;
}(ArgumentMap));
var ActionResultRepresentation = (function (_super) {
    __extends(ActionResultRepresentation, _super);
    function ActionResultRepresentation() {
        var _this = this;
        _super.call(this);
        this.wrapped = function () { return _this.resource(); };
    }
    // links 
    ActionResultRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self") || null;
    };
    // link representations 
    ActionResultRepresentation.prototype.getSelf = function () {
        var self = this.selfLink();
        return self ? self.getTargetAs() : null;
    };
    // properties 
    ActionResultRepresentation.prototype.resultType = function () {
        // todo later validate the result against the type so we can guarantee  
        return this.wrapped().resultType;
    };
    ActionResultRepresentation.prototype.result = function () {
        return new Result(withNull(this.wrapped().result), this.resultType());
    };
    ActionResultRepresentation.prototype.warningsOrMessages = function () {
        var has = function (arr) { return arr && arr.length > 0; };
        var wOrM = has(this.extensions().warnings()) ? this.extensions().warnings() : this.extensions().messages();
        if (has(wOrM)) {
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["reduce"](wOrM, function (s, t) { return s + " " + t; }, "");
        }
        return undefined;
    };
    ActionResultRepresentation.prototype.shouldExpectResult = function () {
        return this.result().isNull() && this.resultType() !== "void";
    };
    return ActionResultRepresentation;
}(ResourceRepresentation));
// matches 18.2.1
var Parameter = (function (_super) {
    __extends(Parameter, _super);
    // fix parent type
    function Parameter(wrapped, parent, paramId) {
        var _this = this;
        _super.call(this, wrapped);
        this.parent = parent;
        this.paramId = paramId;
        this.wrapped = function () { return _this.resource(); };
    }
    Parameter.prototype.id = function () {
        return this.paramId;
    };
    // properties 
    Parameter.prototype.choices = function () {
        var customExtensions = this.extensions();
        // use custom choices extension by preference 
        if (customExtensions.choices()) {
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](customExtensions.choices(), function (v) { return new Value(v); });
        }
        var choices = this.wrapped().choices;
        if (choices) {
            var values = __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](choices, function (item) { return new Value(item); });
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["fromPairs"](__WEBPACK_IMPORTED_MODULE_2_lodash__["map"](values, function (v) { return [v.toString(), v]; }));
        }
        return null;
    };
    Parameter.prototype.promptLink = function () {
        return linkByNamespacedRel(this.links(), "prompt") || null;
    };
    Parameter.prototype.getPromptMap = function () {
        var promptLink = this.promptLink();
        if (promptLink) {
            var pr = promptLink.getTargetAs();
            return new PromptMap(promptLink, pr.instanceId());
        }
        return null;
    };
    Parameter.prototype.default = function () {
        var dflt = this.wrapped().default == null ? (isScalarType(this.extensions().returnType()) ? "" : null) : this.wrapped().default;
        return new Value(withNull(dflt));
    };
    // helper
    Parameter.prototype.isScalar = function () {
        return isScalarType(this.extensions().returnType()) ||
            (isListType(this.extensions().returnType()) && isScalarType(this.extensions().elementType()));
    };
    Parameter.prototype.isList = function () {
        return isListType(this.extensions().returnType());
    };
    Parameter.prototype.hasPrompt = function () {
        return !!this.promptLink();
    };
    Parameter.prototype.isCollectionContributed = function () {
        var myparent = this.parent;
        var isOnList = (myparent instanceof ActionMember || myparent instanceof ActionRepresentation) &&
            (myparent.parent instanceof ListRepresentation || myparent.parent instanceof CollectionRepresentation || myparent.parent instanceof CollectionMember);
        var isList = this.isList();
        return isList && isOnList;
    };
    Parameter.prototype.hasChoices = function () { return __WEBPACK_IMPORTED_MODULE_2_lodash__["some"](this.choices() || {}); };
    Parameter.prototype.entryType = function () {
        var promptLink = this.promptLink();
        if (promptLink) {
            // ConditionalChoices, ConditionalMultipleChoices, AutoComplete 
            if (!!promptLink.arguments()[__WEBPACK_IMPORTED_MODULE_0__constants__["d" /* roSearchTerm */]]) {
                // autocomplete 
                return EntryType.AutoComplete;
            }
            if (isListType(this.extensions().returnType())) {
                return EntryType.MultipleConditionalChoices;
            }
            return EntryType.ConditionalChoices;
        }
        if (this.choices()) {
            if (isListType(this.extensions().returnType())) {
                return EntryType.MultipleChoices;
            }
            return EntryType.Choices;
        }
        if (this.extensions().format() === "blob") {
            return EntryType.File;
        }
        return EntryType.FreeForm;
    };
    return Parameter;
}(NestedRepresentation));
var ActionRepresentation = (function (_super) {
    __extends(ActionRepresentation, _super);
    function ActionRepresentation() {
        var _this = this;
        _super.apply(this, arguments);
        this.wrapped = function () { return _this.resource(); };
    }
    // links 
    ActionRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self"); // known to exist
    };
    ActionRepresentation.prototype.upLink = function () {
        return linkByRel(this.links(), "up"); // known to exist
    };
    ActionRepresentation.prototype.invokeLink = function () {
        return linkByNamespacedRel(this.links(), "invoke") || null; // may not exist if disabled 
    };
    // linked representations 
    ActionRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    ActionRepresentation.prototype.getUp = function () {
        return this.upLink().getTargetAs();
    };
    ActionRepresentation.prototype.getInvokeMap = function () {
        var link = this.invokeLink();
        return link ? new InvokeMap(link) : null;
    };
    // properties 
    ActionRepresentation.prototype.actionId = function () {
        return this.wrapped().id;
    };
    ActionRepresentation.prototype.initParameterMap = function () {
        var _this = this;
        if (!this.parameterMap) {
            var parameters = this.wrapped().parameters;
            this.parameterMap = __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](parameters, function (p, id) { return new Parameter(p, _this, id); });
        }
    };
    ActionRepresentation.prototype.parameters = function () {
        this.initParameterMap();
        return this.parameterMap;
    };
    ActionRepresentation.prototype.disabledReason = function () {
        return this.wrapped().disabledReason || "";
    };
    ActionRepresentation.prototype.isQueryOnly = function () {
        var invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() === "GET";
    };
    ActionRepresentation.prototype.isNotQueryOnly = function () {
        var invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() !== "GET";
    };
    ActionRepresentation.prototype.isPotent = function () {
        var invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() === "POST";
    };
    return ActionRepresentation;
}(ResourceRepresentation));
// new in 1.1 15.0 in spec 
var PromptMap = (function (_super) {
    __extends(PromptMap, _super);
    function PromptMap(link, promptId) {
        _super.call(this, link.arguments(), promptId);
        this.link = link;
        this.promptId = promptId;
        link.copyToHateoasModel(this);
    }
    PromptMap.prototype.promptMap = function () {
        return this.map;
    };
    PromptMap.prototype.setSearchTerm = function (term) {
        this.setArgument(__WEBPACK_IMPORTED_MODULE_0__constants__["d" /* roSearchTerm */], new Value(term));
    };
    PromptMap.prototype.setArgument = function (name, val) {
        val.set(this.map, name);
    };
    PromptMap.prototype.setArguments = function (args) {
        var _this = this;
        __WEBPACK_IMPORTED_MODULE_2_lodash__["each"](args, function (arg, key) { return _this.setArgument(key, arg); });
    };
    PromptMap.prototype.setMember = function (name, value) {
        value.set(this.promptMap()["x-ro-nof-members"], name);
    };
    PromptMap.prototype.setMembers = function (objectValues) {
        var _this = this;
        if (this.map["x-ro-nof-members"]) {
            __WEBPACK_IMPORTED_MODULE_2_lodash__["forEach"](objectValues(), function (v, k) { return _this.setMember(k, v); });
        }
    };
    return PromptMap;
}(ArgumentMap));
var PromptRepresentation = (function (_super) {
    __extends(PromptRepresentation, _super);
    function PromptRepresentation() {
        var _this = this;
        _super.call(this, emptyResource());
        this.wrapped = function () { return _this.resource(); };
    }
    // links 
    PromptRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self"); //known to exist
    };
    PromptRepresentation.prototype.upLink = function () {
        return linkByRel(this.links(), "up"); //known to exist
    };
    // linked representations 
    PromptRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    PromptRepresentation.prototype.getUp = function () {
        return this.upLink().getTargetAs();
    };
    // properties 
    PromptRepresentation.prototype.instanceId = function () {
        return this.wrapped().id;
    };
    PromptRepresentation.prototype.choices = function (addEmpty) {
        var ch = this.wrapped().choices;
        if (ch) {
            var values = __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](ch, function (item) { return new Value(item); });
            if (addEmpty) {
                var emptyValue = new Value("");
                values = __WEBPACK_IMPORTED_MODULE_2_lodash__["concat"]([emptyValue], values);
            }
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["fromPairs"](__WEBPACK_IMPORTED_MODULE_2_lodash__["map"](values, function (v) { return [v.toString(), v]; }));
        }
        return null;
    };
    return PromptRepresentation;
}(ResourceRepresentation));
// matches a collection representation 17.0 
var CollectionRepresentation = (function (_super) {
    __extends(CollectionRepresentation, _super);
    function CollectionRepresentation() {
        var _this = this;
        _super.apply(this, arguments);
        this.wrapped = function () { return _this.resource(); };
        this.hasTableData = function () {
            var valueLinks = _this.value();
            return valueLinks && __WEBPACK_IMPORTED_MODULE_2_lodash__["some"](valueLinks, function (i) { return i.members(); });
        };
    }
    // links 
    CollectionRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self"); // known to exist
    };
    CollectionRepresentation.prototype.upLink = function () {
        return linkByRel(this.links(), "up"); // known to exist
    };
    CollectionRepresentation.prototype.addToLink = function () {
        return linkByNamespacedRel(this.links(), "add-to") || null;
    };
    CollectionRepresentation.prototype.removeFromLink = function () {
        return linkByNamespacedRel(this.links(), "remove-from") || null;
    };
    // linked representations 
    CollectionRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    CollectionRepresentation.prototype.getUp = function () {
        return this.upLink().getTargetAs();
    };
    CollectionRepresentation.prototype.setFromMap = function (map) {
        //this.set(map.attributes);
        __WEBPACK_IMPORTED_MODULE_2_lodash__["assign"](this.resource(), map.map);
    };
    CollectionRepresentation.prototype.addToMap = function () {
        var link = this.addToLink();
        return link ? link.arguments() : null;
    };
    CollectionRepresentation.prototype.getAddToMap = function () {
        var map = this.addToMap();
        return map ? new AddToRemoveFromMap(this, map, true) : null;
    };
    CollectionRepresentation.prototype.removeFromMap = function () {
        var link = this.addToLink();
        return link ? link.arguments() : null;
    };
    CollectionRepresentation.prototype.getRemoveFromMap = function () {
        var map = this.removeFromMap();
        return map ? new AddToRemoveFromMap(this, map, false) : null;
    };
    // properties 
    CollectionRepresentation.prototype.collectionId = function () {
        return this.wrapped().id;
    };
    CollectionRepresentation.prototype.size = function () {
        return this.value().length;
    };
    CollectionRepresentation.prototype.value = function () {
        this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
        return this.lazyValue;
    };
    CollectionRepresentation.prototype.disabledReason = function () {
        return this.wrapped().disabledReason || "";
    };
    CollectionRepresentation.prototype.actionMembers = function () {
        var _this = this;
        this.actionMemberMap = this.actionMemberMap || __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](this.wrapped().members, function (m, id) { return Member.wrapMember(m, _this, id); });
        return this.actionMemberMap;
    };
    CollectionRepresentation.prototype.actionMember = function (id) {
        return getMember(this.actionMembers(), id, this.collectionId());
    };
    CollectionRepresentation.prototype.hasActionMember = function (id) {
        return !!this.actionMembers()[id];
    };
    return CollectionRepresentation;
}(ResourceRepresentation));
// matches a property representation 16.0 
var PropertyRepresentation = (function (_super) {
    __extends(PropertyRepresentation, _super);
    function PropertyRepresentation() {
        var _this = this;
        _super.apply(this, arguments);
        this.wrapped = function () { return _this.resource(); };
    }
    // links 
    PropertyRepresentation.prototype.modifyLink = function () {
        return linkByNamespacedRel(this.links(), "modify") || null;
    };
    PropertyRepresentation.prototype.clearLink = function () {
        return linkByNamespacedRel(this.links(), "clear") || null;
    };
    PropertyRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self"); // known to exist
    };
    PropertyRepresentation.prototype.upLink = function () {
        return linkByRel(this.links(), "up"); // known to exist
    };
    PropertyRepresentation.prototype.promptLink = function () {
        return linkByNamespacedRel(this.links(), "prompt") || null;
    };
    PropertyRepresentation.prototype.modifyMap = function () {
        var link = this.modifyLink();
        return link ? link.arguments() : null;
    };
    // linked representations 
    PropertyRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    PropertyRepresentation.prototype.getUp = function () {
        return this.upLink().getTargetAs();
    };
    PropertyRepresentation.prototype.setFromModifyMap = function (map) {
        //this.set(map.attributes);
        __WEBPACK_IMPORTED_MODULE_2_lodash__["assign"](this.resource(), map.map);
    };
    PropertyRepresentation.prototype.getModifyMap = function () {
        var map = this.modifyMap();
        return map ? new ModifyMap(this, map) : null;
    };
    PropertyRepresentation.prototype.getClearMap = function () {
        if (this.clearLink()) {
            return new ClearMap(this);
        }
        return null;
    };
    // properties 
    PropertyRepresentation.prototype.instanceId = function () {
        return this.wrapped().id;
    };
    PropertyRepresentation.prototype.value = function () {
        return new Value(withNull(this.wrapped().value));
    };
    PropertyRepresentation.prototype.choices = function () {
        // use custom choices extension by preference 
        var choices = this.extensions().choices();
        if (choices) {
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](choices, function (v) { return new Value(v); });
        }
        var ch = this.wrapped().choices;
        if (ch) {
            var values = __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](ch, function (item) { return new Value(item); });
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["fromPairs"](__WEBPACK_IMPORTED_MODULE_2_lodash__["map"](values, function (v) { return [v.toString(), v]; }));
        }
        return null;
    };
    PropertyRepresentation.prototype.disabledReason = function () {
        return this.wrapped().disabledReason || "";
    };
    // helper 
    PropertyRepresentation.prototype.isScalar = function () {
        return isScalarType(this.extensions().returnType());
    };
    PropertyRepresentation.prototype.hasPrompt = function () {
        return !!this.promptLink();
    };
    return PropertyRepresentation;
}(ResourceRepresentation));
// matches a domain object representation 14.0 
// base class for 14.4.1/2/3
var Member = (function (_super) {
    __extends(Member, _super);
    function Member(wrapped) {
        var _this = this;
        _super.call(this, wrapped);
        this.wrapped = function () { return _this.resource(); };
    }
    Member.prototype.update = function (newValue) {
        _super.prototype.update.call(this, newValue);
    };
    Member.prototype.memberType = function () {
        return this.wrapped().memberType;
    };
    Member.prototype.detailsLink = function () {
        return linkByNamespacedRel(this.links(), "details") || null;
    };
    Member.prototype.disabledReason = function () {
        return this.wrapped().disabledReason || "";
    };
    Member.prototype.isScalar = function () {
        return isScalarType(this.extensions().returnType());
    };
    Member.wrapMember = function (toWrap, parent, id) {
        if (toWrap.memberType === "property") {
            return new PropertyMember(toWrap, parent, id);
        }
        if (toWrap.memberType === "collection") {
            return new CollectionMember(toWrap, parent, id);
        }
        if (toWrap.memberType === "action" && !(parent instanceof Link)) {
            var member = new ActionMember(toWrap, parent, id);
            if (member.invokeLink()) {
                return new InvokableActionMember(toWrap, parent, id);
            }
            return member;
        }
        return null;
    };
    return Member;
}(NestedRepresentation));
// matches 14.4.1
var PropertyMember = (function (_super) {
    __extends(PropertyMember, _super);
    function PropertyMember(wrapped, parent, propId) {
        var _this = this;
        _super.call(this, wrapped);
        this.parent = parent;
        this.propId = propId;
        this.wrapped = function () { return _this.resource(); };
    }
    // inlined 
    PropertyMember.prototype.id = function () {
        return this.propId;
    };
    PropertyMember.prototype.modifyLink = function () {
        return linkByNamespacedRel(this.links(), "modify") || null;
    };
    PropertyMember.prototype.clearLink = function () {
        return linkByNamespacedRel(this.links(), "clear") || null;
    };
    PropertyMember.prototype.modifyMap = function () {
        var link = this.modifyLink();
        return link ? link.arguments() : null;
    };
    PropertyMember.prototype.setFromModifyMap = function (map) {
        var _this = this;
        __WEBPACK_IMPORTED_MODULE_2_lodash__["forOwn"](map.map, function (v, k) {
            _this.wrapped[k] = v;
        });
    };
    PropertyMember.prototype.getModifyMap = function (id) {
        var map = this.modifyMap();
        return map ? new ModifyMap(this, map) : null;
    };
    PropertyMember.prototype.getClearMap = function (id) {
        return this.clearLink() ? new ClearMap(this) : null;
    };
    PropertyMember.prototype.getPromptMap = function () {
        var link = this.promptLink();
        if (link) {
            var pr = link.getTargetAs();
            return new PromptMap(link, pr.instanceId());
        }
        return null;
    };
    PropertyMember.prototype.value = function () {
        return new Value(withNull(this.wrapped().value));
    };
    PropertyMember.prototype.isScalar = function () {
        return isScalarType(this.extensions().returnType());
    };
    PropertyMember.prototype.attachmentLink = function () {
        return linkByNamespacedRel(this.links(), "attachment") || null;
    };
    PropertyMember.prototype.promptLink = function () {
        return linkByNamespacedRel(this.links(), "prompt") || null;
    };
    PropertyMember.prototype.getDetails = function () {
        var link = this.detailsLink();
        return link ? link.getTargetAs() : null;
    };
    PropertyMember.prototype.hasChoices = function () {
        return this.wrapped().hasChoices;
    };
    PropertyMember.prototype.hasPrompt = function () {
        return !!this.promptLink();
    };
    PropertyMember.prototype.choices = function () {
        // use custom choices extension by preference 
        var choices = this.extensions().choices();
        if (choices) {
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](choices, function (v) { return new Value(v); });
        }
        var ch = this.wrapped().choices;
        if (ch) {
            var values = __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](ch, function (item) { return new Value(item); });
            return __WEBPACK_IMPORTED_MODULE_2_lodash__["fromPairs"](__WEBPACK_IMPORTED_MODULE_2_lodash__["map"](values, function (v) { return [v.toString(), v]; }));
        }
        return null;
    };
    PropertyMember.prototype.hasConditionalChoices = function () {
        return !!this.promptLink() && !this.hasPrompt();
    };
    //This is actually not relevant to a property. Slight smell here!
    PropertyMember.prototype.isCollectionContributed = function () {
        return false;
    };
    PropertyMember.prototype.entryType = function () {
        var link = this.promptLink();
        if (link) {
            // ConditionalChoices, ConditionalMultipleChoices, AutoComplete 
            if (!!link.arguments()[__WEBPACK_IMPORTED_MODULE_0__constants__["d" /* roSearchTerm */]]) {
                // autocomplete 
                return EntryType.AutoComplete;
            }
            return EntryType.ConditionalChoices;
        }
        if (this.choices()) {
            return EntryType.Choices;
        }
        return EntryType.FreeForm;
    };
    return PropertyMember;
}(Member));
// matches 14.4.2 
var CollectionMember = (function (_super) {
    __extends(CollectionMember, _super);
    function CollectionMember(wrapped, parent, id) {
        var _this = this;
        _super.call(this, wrapped);
        this.parent = parent;
        this.id = id;
        this.wrapped = function () { return _this.resource(); };
        this.hasTableData = function () {
            var valueLinks = _this.value();
            return valueLinks && __WEBPACK_IMPORTED_MODULE_2_lodash__["some"](valueLinks, function (i) { return i.members(); });
        };
        this.etagDigest = parent.etagDigest;
    }
    CollectionMember.prototype.collectionId = function () {
        return this.id;
    };
    CollectionMember.prototype.value = function () {
        this.lazyValue = this.lazyValue || (this.wrapped().value ? wrapLinks(this.wrapped().value) : null);
        return this.lazyValue;
    };
    CollectionMember.prototype.size = function () {
        return withNull(this.wrapped().size);
    };
    CollectionMember.prototype.getDetails = function () {
        var link = this.detailsLink();
        return link ? link.getTargetAs() : null;
    };
    CollectionMember.prototype.actionMembers = function () {
        var _this = this;
        var members = this.wrapped().members;
        if (members) {
            return this.actionMemberMap || __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](members, function (m, id) { return Member.wrapMember(m, _this, id); });
        }
        return {};
    };
    CollectionMember.prototype.hasActionMember = function (id) {
        return !!this.actionMembers()[id];
    };
    CollectionMember.prototype.actionMember = function (id, keySeparator) {
        return getMember(this.actionMembers(), id, this.collectionId());
    };
    return CollectionMember;
}(Member));
// matches 14.4.3 
var ActionMember = (function (_super) {
    __extends(ActionMember, _super);
    function ActionMember(wrapped, parent, id) {
        var _this = this;
        _super.call(this, wrapped);
        this.parent = parent;
        this.id = id;
        this.wrapped = function () { return _this.resource(); };
    }
    ActionMember.prototype.actionId = function () {
        return this.id;
    };
    ActionMember.prototype.getDetails = function () {
        var link = this.detailsLink();
        if (link) {
            var details = link.getTargetAs();
            details.parent = this.parent;
            return details;
        }
        return null;
    };
    // 1.1 inlined 
    ActionMember.prototype.invokeLink = function () {
        return linkByNamespacedRel(this.links(), "invoke") || null;
    };
    ActionMember.prototype.disabledReason = function () {
        return this.wrapped().disabledReason || "";
    };
    return ActionMember;
}(Member));
var InvokableActionMember = (function (_super) {
    __extends(InvokableActionMember, _super);
    function InvokableActionMember(wrapped, parent, id) {
        _super.call(this, wrapped, parent, id);
    }
    InvokableActionMember.prototype.getInvokeMap = function () {
        var invokeLink = this.invokeLink();
        return invokeLink ? new InvokeMap(invokeLink) : null;
    };
    InvokableActionMember.prototype.isQueryOnly = function () {
        var invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() === "GET";
    };
    InvokableActionMember.prototype.isNotQueryOnly = function () {
        var invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() !== "GET";
    };
    InvokableActionMember.prototype.isPotent = function () {
        var invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() === "POST";
    };
    InvokableActionMember.prototype.initParameterMap = function () {
        var _this = this;
        if (!this.parameterMap) {
            var parameters = this.wrapped().parameters;
            this.parameterMap = __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](parameters, function (p, id) { return new Parameter(p, _this, id); });
        }
    };
    InvokableActionMember.prototype.parameters = function () {
        this.initParameterMap();
        return this.parameterMap;
    };
    return InvokableActionMember;
}(ActionMember));
var DomainObjectRepresentation = (function (_super) {
    __extends(DomainObjectRepresentation, _super);
    function DomainObjectRepresentation(model) {
        var _this = this;
        _super.call(this, model);
        this.wrapped = function () { return _this.resource(); };
    }
    // todo later change this to not be dependent of keySeparator 
    DomainObjectRepresentation.prototype.id = function (keySeparator) {
        return "" + (this.domainType() || this.serviceId()) + (this.instanceId() ? "" + keySeparator + this.instanceId() : "");
    };
    DomainObjectRepresentation.prototype.title = function () {
        return this.wrapped().title;
    };
    DomainObjectRepresentation.prototype.domainType = function () {
        return withNull(this.wrapped().domainType);
    };
    DomainObjectRepresentation.prototype.serviceId = function () {
        return withNull(this.wrapped().serviceId);
    };
    DomainObjectRepresentation.prototype.instanceId = function () {
        return withNull(this.wrapped().instanceId);
    };
    DomainObjectRepresentation.prototype.resetMemberMaps = function () {
        var _this = this;
        var members = this.wrapped().members;
        this.memberMap = __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](members, function (m, id) { return Member.wrapMember(m, _this, id); });
        this.propertyMemberMap = __WEBPACK_IMPORTED_MODULE_2_lodash__["pickBy"](this.memberMap, function (m) { return m.memberType() === "property"; });
        this.collectionMemberMap = __WEBPACK_IMPORTED_MODULE_2_lodash__["pickBy"](this.memberMap, function (m) { return m.memberType() === "collection"; });
        this.actionMemberMap = __WEBPACK_IMPORTED_MODULE_2_lodash__["pickBy"](this.memberMap, function (m) { return m.memberType() === "action"; });
    };
    DomainObjectRepresentation.prototype.initMemberMaps = function () {
        if (!this.memberMap) {
            this.resetMemberMaps();
        }
    };
    DomainObjectRepresentation.prototype.members = function () {
        this.initMemberMaps();
        return this.memberMap;
    };
    DomainObjectRepresentation.prototype.propertyMembers = function () {
        this.initMemberMaps();
        return this.propertyMemberMap;
    };
    DomainObjectRepresentation.prototype.collectionMembers = function () {
        this.initMemberMaps();
        return this.collectionMemberMap;
    };
    DomainObjectRepresentation.prototype.actionMembers = function () {
        this.initMemberMaps();
        return this.actionMemberMap;
    };
    DomainObjectRepresentation.prototype.member = function (id) {
        return this.members()[id];
    };
    DomainObjectRepresentation.prototype.propertyMember = function (id) {
        return this.propertyMembers()[id];
    };
    DomainObjectRepresentation.prototype.collectionMember = function (id) {
        return this.collectionMembers()[id];
    };
    DomainObjectRepresentation.prototype.hasActionMember = function (id) {
        return !!this.actionMembers()[id];
    };
    // todo later change to not be dependent on keySeparator
    DomainObjectRepresentation.prototype.actionMember = function (id, keySeparator) {
        return getMember(this.actionMembers(), id, this.id(keySeparator));
    };
    DomainObjectRepresentation.prototype.updateLink = function () {
        return linkByNamespacedRel(this.links(), "update") || null;
    };
    DomainObjectRepresentation.prototype.isTransient = function () {
        return !!this.persistLink();
    };
    DomainObjectRepresentation.prototype.persistLink = function () {
        return linkByNamespacedRel(this.links(), "persist") || null;
    };
    DomainObjectRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self") || null;
    };
    DomainObjectRepresentation.prototype.updateMap = function () {
        var link = this.updateLink();
        return link ? link.arguments() : null;
    };
    DomainObjectRepresentation.prototype.persistMap = function () {
        var link = this.persistLink();
        return link ? link.arguments() : null;
    };
    // linked representations 
    DomainObjectRepresentation.prototype.getSelf = function () {
        var link = this.selfLink();
        return link ? link.getTargetAs() : null;
    };
    DomainObjectRepresentation.prototype.getPersistMap = function () {
        var map = validateExists(this.persistMap(), "PersistMap");
        return new PersistMap(this, map);
    };
    DomainObjectRepresentation.prototype.getUpdateMap = function () {
        var map = validateExists(this.updateMap(), "UpdateMap");
        return new UpdateMap(this, map);
    };
    DomainObjectRepresentation.prototype.setInlinePropertyDetails = function (flag) {
        this.setUrlParameter(__WEBPACK_IMPORTED_MODULE_0__constants__["e" /* roInlinePropertyDetails */], flag);
    };
    DomainObjectRepresentation.prototype.getOid = function (keySeparator) {
        if (!this.oid) {
            this.oid = ObjectIdWrapper.fromObject(this, keySeparator);
        }
        return this.oid;
    };
    DomainObjectRepresentation.prototype.updateSelfLinkWithTitle = function () {
        var link = this.selfLink();
        if (link) {
            link.setTitle(this.title());
        }
        return link;
    };
    return DomainObjectRepresentation;
}(ResourceRepresentation));
var MenuRepresentation = (function (_super) {
    __extends(MenuRepresentation, _super);
    function MenuRepresentation() {
        var _this = this;
        _super.call(this);
        this.wrapped = function () { return _this.resource(); };
    }
    MenuRepresentation.prototype.title = function () {
        return this.wrapped().title;
    };
    MenuRepresentation.prototype.menuId = function () {
        return this.wrapped().menuId;
    };
    MenuRepresentation.prototype.resetMemberMaps = function () {
        var _this = this;
        var members = this.wrapped().members;
        // todo know member won't be null because not link - not good code though 
        this.memberMap = __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](members, function (m, id) { return Member.wrapMember(m, _this, id); });
        this.actionMemberMap = __WEBPACK_IMPORTED_MODULE_2_lodash__["pickBy"](this.memberMap, function (m) { return m.memberType() === "action"; });
    };
    MenuRepresentation.prototype.initMemberMaps = function () {
        if (!this.memberMap) {
            this.resetMemberMaps();
        }
    };
    MenuRepresentation.prototype.members = function () {
        this.initMemberMaps();
        return this.memberMap;
    };
    MenuRepresentation.prototype.actionMembers = function () {
        this.initMemberMaps();
        return this.actionMemberMap;
    };
    MenuRepresentation.prototype.member = function (id) {
        return this.members()[id];
    };
    MenuRepresentation.prototype.hasActionMember = function (id) {
        return !!this.actionMembers()[id];
    };
    MenuRepresentation.prototype.actionMember = function (id, keySeparator) {
        return getMember(this.actionMembers(), id, this.menuId());
    };
    MenuRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self"); // mandatory
    };
    // linked representations 
    MenuRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    return MenuRepresentation;
}(ResourceRepresentation));
// matches scalar representation 12.0 
var ScalarValueRepresentation = (function (_super) {
    __extends(ScalarValueRepresentation, _super);
    function ScalarValueRepresentation(wrapped) {
        var _this = this;
        _super.call(this, wrapped);
        this.wrapped = function () { return _this.resource(); };
    }
    ScalarValueRepresentation.prototype.value = function () {
        return new Value(this.wrapped().value);
    };
    return ScalarValueRepresentation;
}(NestedRepresentation));
// matches List Representation 11.0
var ListRepresentation = (function (_super) {
    __extends(ListRepresentation, _super);
    function ListRepresentation(model) {
        var _this = this;
        _super.call(this, model);
        this.wrapped = function () { return _this.resource(); };
        this.hasTableData = function () {
            var valueLinks = _this.value();
            return valueLinks && __WEBPACK_IMPORTED_MODULE_2_lodash__["some"](valueLinks, function (i) { return i.members(); });
        };
    }
    // links
    ListRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self");
    };
    // linked representations 
    ListRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    ListRepresentation.prototype.value = function () {
        this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
        return this.lazyValue;
    };
    ListRepresentation.prototype.pagination = function () {
        return this.wrapped().pagination || null;
    };
    ListRepresentation.prototype.actionMembers = function () {
        var _this = this;
        this.actionMemberMap = this.actionMemberMap || __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](this.wrapped().members, function (m, id) { return Member.wrapMember(m, _this, id); });
        return this.actionMemberMap;
    };
    ListRepresentation.prototype.actionMember = function (id) {
        return getMember(this.actionMembers(), id, "list");
    };
    ListRepresentation.prototype.hasActionMember = function (id) {
        return !!this.actionMembers()[id];
    };
    return ListRepresentation;
}(ResourceRepresentation));
// matches the error representation 10.0 
var ErrorRepresentation = (function (_super) {
    __extends(ErrorRepresentation, _super);
    function ErrorRepresentation() {
        var _this = this;
        _super.call(this);
        this.wrapped = function () { return _this.resource(); };
    }
    ErrorRepresentation.create = function (message, stackTrace, causedBy) {
        var rawError = {
            links: [],
            extensions: {},
            message: message,
            stackTrace: stackTrace,
            causedBy: causedBy
        };
        var error = new ErrorRepresentation();
        error.populate(rawError);
        return error;
    };
    // scalar properties 
    ErrorRepresentation.prototype.message = function () {
        return this.wrapped().message;
    };
    ErrorRepresentation.prototype.stackTrace = function () {
        return this.wrapped().stackTrace || [];
    };
    ErrorRepresentation.prototype.causedBy = function () {
        var cb = this.wrapped().causedBy;
        return cb ? {
            message: function () { return cb.message; },
            stackTrace: function () { return cb.stackTrace || []; }
        } : undefined;
    };
    return ErrorRepresentation;
}(ResourceRepresentation));
// matches Objects of Type Resource 9.0 
var PersistMap = (function (_super) {
    __extends(PersistMap, _super);
    function PersistMap(domainObject, map) {
        _super.call(this, map);
        this.domainObject = domainObject;
        this.map = map;
        var link = domainObject.persistLink();
        if (link) {
            link.copyToHateoasModel(this);
        }
        else {
            error("PersistMap attempting to create persist map for object with no persist link");
        }
    }
    PersistMap.prototype.setMember = function (name, value) {
        value.set(this.map.members, name);
    };
    PersistMap.prototype.setValidateOnly = function () {
        this.map[__WEBPACK_IMPORTED_MODULE_0__constants__["c" /* roValidateOnly */]] = true;
    };
    return PersistMap;
}(HateosModel));
// matches the version representation 8.0 
var VersionRepresentation = (function (_super) {
    __extends(VersionRepresentation, _super);
    function VersionRepresentation() {
        var _this = this;
        _super.apply(this, arguments);
        this.wrapped = function () { return _this.resource(); };
    }
    // links 
    VersionRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self"); // mandatory
    };
    VersionRepresentation.prototype.upLink = function () {
        return linkByRel(this.links(), "up"); // mandatory
    };
    // linked representations 
    VersionRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    VersionRepresentation.prototype.getUp = function () {
        return this.upLink().getTargetAs();
    };
    // scalar properties 
    VersionRepresentation.prototype.specVersion = function () {
        return this.wrapped().specVersion;
    };
    VersionRepresentation.prototype.implVersion = function () {
        return this.wrapped().implVersion || null;
    };
    VersionRepresentation.prototype.optionalCapabilities = function () {
        return this.wrapped().optionalCapabilities;
    };
    return VersionRepresentation;
}(ResourceRepresentation));
// matches Domain Services Representation 7.0
var DomainServicesRepresentation = (function (_super) {
    __extends(DomainServicesRepresentation, _super);
    function DomainServicesRepresentation() {
        var _this = this;
        _super.apply(this, arguments);
        this.wrapped = function () { return _this.resource(); };
    }
    // links
    DomainServicesRepresentation.prototype.upLink = function () {
        return linkByRel(this.links(), "up"); // mandatory
    };
    // linked representations 
    DomainServicesRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    DomainServicesRepresentation.prototype.getUp = function () {
        return this.upLink().getTargetAs();
    };
    DomainServicesRepresentation.prototype.getService = function (serviceType) {
        var serviceLink = __WEBPACK_IMPORTED_MODULE_2_lodash__["find"](this.value(), function (link) { return link.rel().parms[0].value === serviceType; });
        return serviceLink ? serviceLink.getTargetAs() : null;
    };
    return DomainServicesRepresentation;
}(ListRepresentation));
// custom
var MenusRepresentation = (function (_super) {
    __extends(MenusRepresentation, _super);
    function MenusRepresentation() {
        var _this = this;
        _super.apply(this, arguments);
        this.wrapped = function () { return _this.resource(); };
    }
    // links
    MenusRepresentation.prototype.upLink = function () {
        return linkByRel(this.links(), "up"); // mandatory
    };
    // linked representations 
    MenusRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    MenusRepresentation.prototype.getUp = function () {
        return this.upLink().getTargetAs();
    };
    MenusRepresentation.prototype.getMenu = function (menuId) {
        var menuLink = __WEBPACK_IMPORTED_MODULE_2_lodash__["find"](this.value(), function (link) { return link.rel().parms[0].value === menuId; });
        if (menuLink) {
            return menuLink.getTargetAs();
        }
        error("MenusRepresentation:getMenu Failed to find menu " + menuId);
    };
    return MenusRepresentation;
}(ListRepresentation));
// matches the user representation 6.0
var UserRepresentation = (function (_super) {
    __extends(UserRepresentation, _super);
    function UserRepresentation() {
        var _this = this;
        _super.apply(this, arguments);
        this.wrapped = function () { return _this.resource(); };
    }
    // links 
    UserRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self"); // mandatory
    };
    UserRepresentation.prototype.upLink = function () {
        return linkByRel(this.links(), "up"); // mandatory
    };
    // linked representations 
    UserRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    UserRepresentation.prototype.getUp = function () {
        return this.upLink().getTargetAs();
    };
    // scalar properties 
    UserRepresentation.prototype.userName = function () {
        return this.wrapped().userName;
    };
    UserRepresentation.prototype.friendlyName = function () {
        return this.wrapped().friendlyName;
    };
    UserRepresentation.prototype.email = function () {
        return this.wrapped().email;
    };
    UserRepresentation.prototype.roles = function () {
        return this.wrapped().roles;
    };
    return UserRepresentation;
}(ResourceRepresentation));
var DomainTypeActionInvokeRepresentation = (function (_super) {
    __extends(DomainTypeActionInvokeRepresentation, _super);
    function DomainTypeActionInvokeRepresentation(againstType, toCheckType, appPath) {
        var _this = this;
        _super.call(this);
        this.wrapped = function () { return _this.resource(); };
        this.hateoasUrl = appPath + "/domain-types/" + toCheckType + "/type-actions/isSubtypeOf/invoke";
        this.urlParms = {};
        this.urlParms["supertype"] = againstType;
    }
    DomainTypeActionInvokeRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self");
    };
    // linked representations 
    DomainTypeActionInvokeRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    DomainTypeActionInvokeRepresentation.prototype.id = function () {
        return this.wrapped().id;
    };
    DomainTypeActionInvokeRepresentation.prototype.value = function () {
        return this.wrapped().value;
    };
    return DomainTypeActionInvokeRepresentation;
}(ResourceRepresentation));
// matches the home page representation  5.0 
var HomePageRepresentation = (function (_super) {
    __extends(HomePageRepresentation, _super);
    function HomePageRepresentation(rep, appPath) {
        var _this = this;
        _super.call(this, rep);
        this.wrapped = function () { return _this.resource(); };
        this.hateoasUrl = appPath;
    }
    // links 
    HomePageRepresentation.prototype.serviceLink = function () {
        return linkByNamespacedRel(this.links(), "services");
    };
    HomePageRepresentation.prototype.userLink = function () {
        return linkByNamespacedRel(this.links(), "user");
    };
    HomePageRepresentation.prototype.selfLink = function () {
        return linkByRel(this.links(), "self");
    };
    HomePageRepresentation.prototype.versionLink = function () {
        return linkByNamespacedRel(this.links(), "version");
    };
    // custom 
    HomePageRepresentation.prototype.menusLink = function () {
        return linkByNamespacedRel(this.links(), "menus");
    };
    // linked representations 
    HomePageRepresentation.prototype.getSelf = function () {
        return this.selfLink().getTargetAs();
    };
    HomePageRepresentation.prototype.getUser = function () {
        return this.userLink().getTargetAs();
    };
    HomePageRepresentation.prototype.getDomainServices = function () {
        // cannot use getTarget here as that will just return a ListRepresentation 
        var domainServices = new DomainServicesRepresentation();
        this.serviceLink().copyToHateoasModel(domainServices);
        return domainServices;
    };
    HomePageRepresentation.prototype.getVersion = function () {
        return this.versionLink().getTargetAs();
    };
    //  custom 
    HomePageRepresentation.prototype.getMenus = function () {
        // cannot use getTarget here as that will just return a ListRepresentation 
        var menus = new MenusRepresentation();
        this.menusLink().copyToHateoasModel(menus);
        return menus;
    };
    return HomePageRepresentation;
}(ResourceRepresentation));
// matches the Link representation 2.7
var Link = (function () {
    function Link(wrapped) {
        this.wrapped = wrapped;
        this.repTypeToModel = {
            "homepage": HomePageRepresentation,
            "user": UserRepresentation,
            "version": VersionRepresentation,
            "list": ListRepresentation,
            "object": DomainObjectRepresentation,
            "object-property": PropertyRepresentation,
            "object-collection": CollectionRepresentation,
            "object-action": ActionRepresentation,
            "action-result": ActionResultRepresentation,
            "error": ErrorRepresentation,
            "prompt": PromptRepresentation,
            // custom 
            "menu": MenuRepresentation
        };
    }
    Link.prototype.compress = function (shortCutMarker, urlShortCuts) {
        this.wrapped.href = compress(this.wrapped.href, shortCutMarker, urlShortCuts);
    };
    Link.prototype.decompress = function (shortCutMarker, urlShortCuts) {
        this.wrapped.href = decompress(this.wrapped.href, shortCutMarker, urlShortCuts);
    };
    Link.prototype.href = function () {
        return decodeURIComponent(this.wrapped.href);
    };
    Link.prototype.method = function () {
        return this.wrapped.method;
    };
    Link.prototype.rel = function () {
        return new Rel(this.wrapped.rel);
    };
    Link.prototype.type = function () {
        return new MediaType(this.wrapped.type);
    };
    Link.prototype.title = function () {
        return withNull(this.wrapped.title);
    };
    //Typically used to set a title on a link that doesn't naturally have one e.g. Self link
    Link.prototype.setTitle = function (title) {
        this.wrapped.title = title;
    };
    Link.prototype.arguments = function () {
        return withNull(this.wrapped.arguments);
    };
    Link.prototype.members = function () {
        var _this = this;
        var members = this.wrapped.members;
        return members ? __WEBPACK_IMPORTED_MODULE_2_lodash__["mapValues"](members, function (m, id) { return Member.wrapMember(m, _this, id); }) : null;
    };
    Link.prototype.extensions = function () {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.wrapped.extensions);
        return this.lazyExtensions;
    };
    Link.prototype.copyToHateoasModel = function (hateoasModel) {
        hateoasModel.hateoasUrl = this.href();
        hateoasModel.method = this.method();
    };
    Link.prototype.getHateoasTarget = function (targetType) {
        var MatchingType = this.repTypeToModel[targetType];
        var target = new MatchingType();
        return target;
    };
    // get the object that this link points to 
    Link.prototype.getTarget = function () {
        var target = this.getHateoasTarget(this.type().representationType);
        this.copyToHateoasModel(target);
        return target;
    };
    Link.prototype.getTargetAs = function () {
        var target = this.getHateoasTarget(this.type().representationType);
        this.copyToHateoasModel(target);
        return target;
    };
    Link.prototype.getOid = function (keySeparator) {
        if (!this.oid) {
            this.oid = ObjectIdWrapper.fromLink(this, keySeparator);
        }
        return this.oid;
    };
    return Link;
}());
var EntryType;
(function (EntryType) {
    EntryType[EntryType["FreeForm"] = 0] = "FreeForm";
    EntryType[EntryType["Choices"] = 1] = "Choices";
    EntryType[EntryType["MultipleChoices"] = 2] = "MultipleChoices";
    EntryType[EntryType["ConditionalChoices"] = 3] = "ConditionalChoices";
    EntryType[EntryType["MultipleConditionalChoices"] = 4] = "MultipleConditionalChoices";
    EntryType[EntryType["AutoComplete"] = 5] = "AutoComplete";
    EntryType[EntryType["File"] = 6] = "File";
})(EntryType || (EntryType = {}));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/models.js.map

/***/ }),

/***/ 1508:
/***/ (function(module, exports) {

module.exports = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAqklEQVR42u3asQ2DQBQFQV//Rds0YJBA9gr+bIpA7ya6gPXeeu20tr49O3r3X9+88r0FAAAAAAAADAY4c4AnBaAeUAegHlAHoB5QB+DKRegO/fQmeIcAAAAAAACAnfPVA+sA1APqANQD6gDUA+oAjL8IAQAAAAAAAIMB6oF1AOoBdQDqAXUA6gF1AMb/KAkAAAAAAAAMBjhzgCcFoB5QB6AeUAegHlA3HuADiqd/0NLmUrYAAAAASUVORK5CYII="

/***/ }),

/***/ 1513:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(855);


/***/ }),

/***/ 188:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* unused harmony export getSvrPath */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "k", function() { return geminiPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "j", function() { return ciceroPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "i", function() { return homePath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "m", function() { return objectPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "n", function() { return listPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "o", function() { return errorPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "p", function() { return recentPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "q", function() { return attachmentPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "l", function() { return applicationPropertiesPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "r", function() { return multiLineDialogPath; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return roDomainType; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return roInvalidReason; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "d", function() { return roSearchTerm; });
/* unused harmony export roPage */
/* unused harmony export roPageSize */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "e", function() { return roInlinePropertyDetails; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "c", function() { return roValidateOnly; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "h", function() { return roInlineCollectionItems; });
/* unused harmony export nofWarnings */
/* unused harmony export nofMessages */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "f", function() { return supportedDateFormats; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "g", function() { return clientVersion; });
// code constants - not for config
var path = null;
// path local server (ie ~) only needed if you want to hard code paths for some reason
var svrPath = "";
function getSvrPath() {
    if (!path) {
        var trimmedPath = svrPath.trim();
        if (trimmedPath.length === 0 || trimmedPath.charAt(svrPath.length - 1) === "/") {
            path = trimmedPath;
        }
        else {
            path = trimmedPath + "/";
        }
    }
    return path;
}
// routing constants 
var geminiPath = "gemini";
var ciceroPath = "cicero";
var homePath = "home";
var objectPath = "object";
var listPath = "list";
var errorPath = "error";
var recentPath = "recent";
var attachmentPath = "attachment";
var applicationPropertiesPath = "applicationProperties";
var multiLineDialogPath = "multiLineDialog";
//Restful Objects constants
var roDomainType = "x-ro-domain-type";
var roInvalidReason = "x-ro-invalidReason";
var roSearchTerm = "x-ro-searchTerm";
var roPage = "x-ro-page";
var roPageSize = "x-ro-pageSize";
var roInlinePropertyDetails = "x-ro-inline-property-details";
var roValidateOnly = "x-ro-validate-only";
var roInlineCollectionItems = "x-ro-inline-collection-items";
//NOF custom RO constants  
var nofWarnings = "x-ro-nof-warnings";
var nofMessages = "x-ro-nof-messages";
var supportedDateFormats = ["D/M/YYYY", "D/M/YY", "D MMM YYYY", "D MMMM YYYY", "D MMM YY", "D MMMM YY"];
// updated by build do not update manually or change name or regex may not match 
var clientVersion = '9.0.0-beta.4';
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/constants.js.map

/***/ }),

/***/ 189:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_rxjs_add_observable_of__ = __webpack_require__(833);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_rxjs_add_observable_of___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_rxjs_add_observable_of__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_add_observable_throw__ = __webpack_require__(1462);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_add_observable_throw___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_rxjs_add_observable_throw__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_catch__ = __webpack_require__(834);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_catch___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_catch__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_debounceTime__ = __webpack_require__(1464);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_debounceTime___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_debounceTime__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_add_operator_distinctUntilChanged__ = __webpack_require__(1465);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_add_operator_distinctUntilChanged___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_rxjs_add_operator_distinctUntilChanged__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_rxjs_add_operator_do__ = __webpack_require__(835);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_rxjs_add_operator_do___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_5_rxjs_add_operator_do__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_rxjs_add_operator_filter__ = __webpack_require__(836);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_rxjs_add_operator_filter___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_6_rxjs_add_operator_filter__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_rxjs_add_operator_map__ = __webpack_require__(203);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_rxjs_add_operator_map___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_7_rxjs_add_operator_map__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_rxjs_add_operator_switchMap__ = __webpack_require__(838);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_rxjs_add_operator_switchMap___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_8_rxjs_add_operator_switchMap__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9_rxjs_add_operator_toPromise__ = __webpack_require__(1469);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9_rxjs_add_operator_toPromise___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_9_rxjs_add_operator_toPromise__);










//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/rxjs-extensions.js.map

/***/ }),

/***/ 190:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return MessageViewModel; });
var MessageViewModel = (function () {
    function MessageViewModel() {
        var _this = this;
        this.previousMessage = "";
        this.message = "";
        this.clearMessage = function () {
            if (_this.message === _this.previousMessage) {
                _this.resetMessage();
            }
            else {
                _this.previousMessage = _this.message;
            }
        };
        this.resetMessage = function () { return _this.message = _this.previousMessage = ""; };
        this.setMessage = function (msg) { return _this.message = msg; };
        this.getMessage = function () { return _this.message; };
    }
    return MessageViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/message-view-model.js.map

/***/ }),

/***/ 21:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(103);
/* harmony export (immutable) */ __webpack_exports__["b"] = configFactory;
/* harmony export (immutable) */ __webpack_exports__["c"] = localeFactory;
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ConfigService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


function configFactory(config) {
    return function () { return config.load(); };
}
function localeFactory(config) {
    return config.config.defaultLocale;
}
var ConfigService = (function () {
    function ConfigService(http) {
        this.http = http;
        // defaults 
        this.appConfig = {
            authenticate: false,
            appPath: "",
            applicationName: "",
            logoffUrl: "",
            postLogoffUrl: "/gemini/home",
            defaultPageSize: 20,
            listCacheSize: 5,
            shortCutMarker: "___",
            urlShortCuts: [],
            keySeparator: "--",
            objectColor: "object-color",
            linkColor: "link-color",
            autoLoadDirty: true,
            showDirtyFlag: false,
            defaultLocale: "en-GB",
            httpCacheDepth: 50,
            transientCacheDepth: 4,
            recentCacheDepth: 20,
            doUrlValidation: false,
            leftClickHomeAlwaysGoesToSinglePane: true,
            logLevel: "error"
        };
    }
    Object.defineProperty(ConfigService.prototype, "config", {
        get: function () {
            return this.appConfig;
        },
        set: function (newConfig) {
            // merge defaults
            _.assign(this.appConfig, newConfig);
        },
        enumerable: true,
        configurable: true
    });
    ConfigService.prototype.getAppPath = function (appPath) {
        if (appPath.charAt(appPath.length - 1) === "/") {
            return appPath.length > 1 ? appPath.substring(0, appPath.length - 1) : "";
        }
        return appPath;
    };
    ConfigService.prototype.checkAppPath = function () {
        this.appConfig.appPath = this.getAppPath(this.appConfig.appPath);
    };
    ConfigService.prototype.load = function () {
        var _this = this;
        var options = {
            withCredentials: true
        };
        return this.http.get('config.json', options).
            map(function (res) { return res.json(); }).
            toPromise().
            then(function (serverConfig) {
            _this.config = serverConfig;
            _this.checkAppPath();
            return true;
        });
    };
    ConfigService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["Http"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_http__["Http"]) === 'function' && _a) || Object])
    ], ConfigService);
    return ConfigService;
    var _a;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/config.service.js.map

/***/ }),

/***/ 22:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_common__ = __webpack_require__(24);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__logger_service__ = __webpack_require__(87);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__rxjs_extensions__ = __webpack_require__(189);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_7_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__constants__ = __webpack_require__(188);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return UrlManagerService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};










var Transition;
(function (Transition) {
    Transition[Transition["Null"] = 0] = "Null";
    Transition[Transition["ToHome"] = 1] = "ToHome";
    Transition[Transition["ToMenu"] = 2] = "ToMenu";
    Transition[Transition["ToDialog"] = 3] = "ToDialog";
    Transition[Transition["FromDialog"] = 4] = "FromDialog";
    Transition[Transition["FromDialogKeepHistory"] = 5] = "FromDialogKeepHistory";
    Transition[Transition["ToObjectView"] = 6] = "ToObjectView";
    Transition[Transition["ToList"] = 7] = "ToList";
    Transition[Transition["LeaveEdit"] = 8] = "LeaveEdit";
    Transition[Transition["Page"] = 9] = "Page";
    Transition[Transition["ToTransient"] = 10] = "ToTransient";
    Transition[Transition["ToRecent"] = 11] = "ToRecent";
    Transition[Transition["ToAttachment"] = 12] = "ToAttachment";
    Transition[Transition["ToObjectWithMode"] = 13] = "ToObjectWithMode";
    Transition[Transition["ToMultiLineDialog"] = 14] = "ToMultiLineDialog";
})(Transition || (Transition = {}));
;
// keep in alphabetic order to help avoid name collisions 
// all key map
var akm = {
    action: "a",
    actions: "as",
    attachment: "at",
    collection: "c",
    dialog: "d",
    errorCat: "et",
    interactionMode: "i",
    menu: "m",
    object: "o",
    page: "pg",
    pageSize: "ps",
    parm: "pm",
    prop: "pp",
    reload: "r",
    selected: "s"
};
var UrlManagerService = (function () {
    function UrlManagerService(router, location, configService, loggerService) {
        var _this = this;
        this.router = router;
        this.location = location;
        this.configService = configService;
        this.loggerService = loggerService;
        this.capturedPanes = [];
        this.currentPaneId = 1;
        this.setHome = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.executeTransition({}, paneId, Transition.ToHome, function () { return true; });
        };
        this.setRecent = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.executeTransition({}, paneId, Transition.ToRecent, function () { return true; });
        };
        this.setMenu = function (menuId, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.menu + paneId;
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [menuId]);
            _this.executeTransition(newValues, paneId, Transition.ToMenu, function (search) { return _this.getId(key, search) !== menuId; });
        };
        this.setDialog = function (dialogId, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.dialog + paneId;
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [dialogId]);
            _this.executeTransition(newValues, paneId, Transition.ToDialog, function (search) { return _this.getId(key, search) !== dialogId; });
        };
        this.setMultiLineDialog = function (dialogId, paneId) {
            _this.pushUrlState();
            var key = "" + akm.dialog + 1; // always on 1
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [dialogId]);
            _this.executeTransition(newValues, paneId, Transition.ToMultiLineDialog, function (search) { return _this.getId(key, search) !== dialogId; });
        };
        this.setDialogOrMultiLineDialog = function (actionRep, paneId) {
            if (paneId === void 0) { paneId = 1; }
            if (actionRep.extensions().multipleLines()) {
                _this.setMultiLineDialog(actionRep.actionId(), paneId);
            }
            else {
                _this.setDialog(actionRep.actionId(), paneId);
            }
        };
        this.closeDialogKeepHistory = function (id, paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.closeOrCancelDialog(id, paneId, Transition.FromDialogKeepHistory);
        };
        this.closeDialogReplaceHistory = function (id, paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.closeOrCancelDialog(id, paneId, Transition.FromDialog);
        };
        this.setObject = function (resultObject, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var oid = resultObject.id(_this.keySeparator);
            var key = "" + akm.object + paneId;
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [oid]);
            _this.executeTransition(newValues, paneId, Transition.ToObjectView, function () { return true; });
        };
        this.setObjectWithMode = function (resultObject, newMode, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var oid = resultObject.id(_this.keySeparator);
            var okey = "" + akm.object + paneId;
            var mkey = "" + akm.interactionMode + paneId;
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([okey, mkey], [oid, __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */][newMode]]);
            _this.executeTransition(newValues, paneId, Transition.ToObjectWithMode, function () { return true; });
        };
        this.setList = function (actionMember, parms, fromPaneId, toPaneId) {
            if (fromPaneId === void 0) { fromPaneId = 1; }
            if (toPaneId === void 0) { toPaneId = 1; }
            var newValues = {};
            var parent = actionMember.parent;
            if (parent instanceof __WEBPACK_IMPORTED_MODULE_8__models__["o" /* DomainObjectRepresentation */]) {
                newValues[("" + akm.object + toPaneId)] = parent.id(_this.keySeparator);
            }
            if (parent instanceof __WEBPACK_IMPORTED_MODULE_8__models__["P" /* MenuRepresentation */]) {
                newValues[("" + akm.menu + toPaneId)] = parent.menuId();
            }
            newValues[("" + akm.action + toPaneId)] = actionMember.actionId();
            newValues[("" + akm.page + toPaneId)] = "1";
            newValues[("" + akm.pageSize + toPaneId)] = _this.configService.config.defaultPageSize.toString();
            newValues[("" + akm.selected + toPaneId + "_")] = "0";
            var newState = actionMember.extensions().renderEagerly() ? __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */][__WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table] : __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */][__WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].List];
            newValues[("" + akm.collection + toPaneId)] = newState;
            __WEBPACK_IMPORTED_MODULE_7_lodash__["forEach"](parms, function (p, id) { return _this.setId("" + akm.parm + toPaneId + "_" + id, p.toJsonString(_this.shortCutMarker, _this.urlShortCuts), newValues); });
            _this.executeTransition(newValues, toPaneId, Transition.ToList, function () { return true; });
            return newValues;
        };
        this.setProperty = function (href, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var oid = _this.getOidFromHref(href);
            var key = "" + akm.object + paneId;
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [oid]);
            _this.executeTransition(newValues, paneId, Transition.ToObjectView, function () { return true; });
        };
        this.setItem = function (link, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var href = link.href();
            var oid = _this.getOidFromHref(href);
            var key = "" + akm.object + paneId;
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [oid]);
            _this.executeTransition(newValues, paneId, Transition.ToObjectView, function () { return true; });
        };
        this.setAttachment = function (attachmentlink, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var href = attachmentlink.href();
            var okey = "" + akm.object + paneId;
            var akey = "" + akm.attachment + paneId;
            var oid = _this.getOidFromHref(href);
            var pid = _this.getPidFromHref(href);
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([okey, akey], [oid, pid]);
            _this.executeTransition(newValues, paneId, Transition.ToAttachment, function () { return true; });
        };
        this.toggleObjectMenu = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = akm.actions + paneId;
            var actionsId = _this.getSearch()[key] ? null : "open";
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [actionsId]);
            _this.executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        this.setParameterValue = function (actionId, p, pv, paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.checkAndSetValue(paneId, function (search) { return _this.getId("" + akm.action + paneId, search) === actionId; }, function (search) { return _this.setParameter(paneId, search, p, pv); });
        };
        this.setCollectionMemberState = function (collectionMemberId, state, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.collection + paneId + "_" + collectionMemberId;
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [__WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */][state]]);
            _this.executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        this.setListState = function (state, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.collection + paneId;
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [__WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */][state]]);
            _this.executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        this.setInteractionMode = function (newMode, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.interactionMode + paneId;
            var routeParams = _this.getSearch();
            var currentMode = _this.getInteractionMode(_this.getId(key, routeParams));
            var transition;
            if (currentMode === __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Edit && newMode !== __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Edit) {
                transition = Transition.LeaveEdit;
            }
            else if (newMode === __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Transient) {
                transition = Transition.ToTransient;
            }
            else {
                transition = Transition.Null;
            }
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [__WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */][newMode]]);
            _this.executeTransition(newValues, paneId, transition, function () { return true; });
        };
        this.setItemSelected = function (item, isSelected, collectionId, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.selected + paneId + "_" + collectionId;
            var currentSelected = _this.getSearch()[key];
            var selectedArray = _this.arrayFromMask(currentSelected);
            selectedArray[item] = isSelected;
            var currentSelectedAsString = (_this.createMask(selectedArray)).toString();
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [currentSelectedAsString]);
            _this.executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        this.setAllItemsSelected = function (isSelected, collectionId, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.selected + paneId + "_" + collectionId;
            var currentSelected = _this.getSearch()[key];
            var selectedArray = _this.arrayFromMask(currentSelected);
            __WEBPACK_IMPORTED_MODULE_7_lodash__["fill"](selectedArray, isSelected);
            var currentSelectedAsString = (_this.createMask(selectedArray)).toString();
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [currentSelectedAsString]);
            _this.executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        this.setListPaging = function (newPage, newPageSize, state, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var pageValues = {};
            pageValues[("" + akm.page + paneId)] = newPage.toString();
            pageValues[("" + akm.pageSize + paneId)] = newPageSize.toString();
            pageValues[("" + akm.collection + paneId)] = __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */][state];
            pageValues[("" + akm.selected + paneId + "_")] = "0"; // clear selection 
            _this.executeTransition(pageValues, paneId, Transition.Page, function () { return true; });
        };
        this.setError = function (errorCategory, ec) {
            var path = _this.getPath();
            var segments = path.split("/");
            var mode = segments[1];
            var newPath = "/" + mode + "/error";
            var search = {};
            // always on pane 1
            search[akm.errorCat + 1] = __WEBPACK_IMPORTED_MODULE_8__models__["b" /* ErrorCategory */][errorCategory];
            var result = { path: newPath, search: search, replace: false };
            if (errorCategory === __WEBPACK_IMPORTED_MODULE_8__models__["b" /* ErrorCategory */].HttpClientError && ec === __WEBPACK_IMPORTED_MODULE_8__models__["q" /* HttpStatusCode */].PreconditionFailed) {
                result.replace = true;
            }
            _this.setNewSearch(result);
        };
        this.getRouteData = function () {
            var routeData = new __WEBPACK_IMPORTED_MODULE_0__route_data__["d" /* RouteData */](_this.configService, _this.loggerService);
            _this.setPaneRouteData(routeData.pane1, 1);
            _this.setPaneRouteData(routeData.pane2, 2);
            return routeData;
        };
        this.getPaneRouteDataObservable = function (paneId) {
            return _this.router.routerState.root.queryParams.map(function (ps) {
                var routeData = new __WEBPACK_IMPORTED_MODULE_0__route_data__["d" /* RouteData */](_this.configService, _this.loggerService);
                var paneRouteData = routeData.pane(paneId);
                _this.setPaneRouteDataFromParms(paneRouteData, paneId, ps);
                paneRouteData.location = _this.getViewType(_this.getLocation(paneId));
                return paneRouteData;
            });
        };
        this.pushUrlState = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.capturedPanes[paneId] = _this.getUrlState(paneId);
        };
        this.getUrlState = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.currentPaneId = paneId;
            var path = _this.getPath();
            var segments = path.split("/");
            var paneType = segments[paneId + 1] || __WEBPACK_IMPORTED_MODULE_9__constants__["i" /* homePath */];
            var paneSearch = _this.capturePane(paneId);
            // clear any dialogs so we don't return  to a dialog
            paneSearch = __WEBPACK_IMPORTED_MODULE_7_lodash__["omit"](paneSearch, "" + akm.dialog + paneId);
            return { paneType: paneType, search: paneSearch };
        };
        this.getListCacheIndexFromSearch = function (search, paneId, newPage, newPageSize, format) {
            var s1 = _this.getId("" + akm.menu + paneId, search) || "";
            var s2 = _this.getId("" + akm.object + paneId, search) || "";
            var s3 = _this.getId("" + akm.action + paneId, search) || "";
            var parms = __WEBPACK_IMPORTED_MODULE_7_lodash__["pickBy"](search, function (v, k) { return !!k && k.indexOf(akm.parm + paneId) === 0; });
            var mappedParms = __WEBPACK_IMPORTED_MODULE_7_lodash__["mapValues"](parms, function (v) { return decodeURIComponent(__WEBPACK_IMPORTED_MODULE_8__models__["Q" /* decompress */](v, _this.shortCutMarker, _this.urlShortCuts)); });
            var s4 = __WEBPACK_IMPORTED_MODULE_7_lodash__["reduce"](mappedParms, function (r, n, k) { return r + (k + "=" + n + _this.keySeparator); }, "");
            var s5 = "" + newPage;
            var s6 = "" + newPageSize;
            var s7 = format ? "" + format : "";
            var ss = [s1, s2, s3, s4, s5, s6, s7];
            return __WEBPACK_IMPORTED_MODULE_7_lodash__["reduce"](ss, function (r, n) { return r + _this.keySeparator + n; }, "");
        };
        this.getListCacheIndex = function (paneId, newPage, newPageSize, format) {
            var search = _this.getSearch();
            return _this.getListCacheIndexFromSearch(search, paneId, newPage, newPageSize, format);
        };
        this.popUrlState = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.currentPaneId = paneId;
            var capturedPane = _this.capturedPanes[paneId];
            if (capturedPane) {
                _this.capturedPanes[paneId] = null;
                var search = _this.clearPane(_this.getSearch(), paneId);
                search = __WEBPACK_IMPORTED_MODULE_7_lodash__["merge"](search, capturedPane.search);
                var path = void 0;
                var replace = void 0;
                (_a = _this.setupPaneNumberAndTypes(paneId, capturedPane.paneType), path = _a.path, replace = _a.replace, _a);
                var result = { path: path, search: search, replace: replace };
                _this.setNewSearch(result);
            }
            else {
                // probably reloaded page so no state to pop. 
                // just go home 
                _this.setHome(paneId);
            }
            var _a;
        };
        this.clearUrlState = function (paneId) {
            _this.currentPaneId = paneId;
            _this.capturedPanes[paneId] = null;
        };
        this.swapPanes = function () {
            var path = _this.getPath();
            var segments = path.split("/");
            var mode = segments[1], oldPane1 = segments[2], _a = segments[3], oldPane2 = _a === void 0 ? __WEBPACK_IMPORTED_MODULE_9__constants__["i" /* homePath */] : _a;
            var newPath = "/" + mode + "/" + oldPane2 + "/" + oldPane1;
            var search = _this.swapSearchIds(_this.getSearch());
            _this.currentPaneId = __WEBPACK_IMPORTED_MODULE_8__models__["O" /* getOtherPane */](_this.currentPaneId);
            var tree = _this.router.createUrlTree([newPath], { queryParams: search });
            _this.router.navigateByUrl(tree);
        };
        this.cicero = function () { return _this.setMode(__WEBPACK_IMPORTED_MODULE_9__constants__["j" /* ciceroPath */]); };
        this.gemini = function () { return _this.setMode(__WEBPACK_IMPORTED_MODULE_9__constants__["k" /* geminiPath */]); };
        this.applicationProperties = function () {
            var newPath = "/" + __WEBPACK_IMPORTED_MODULE_9__constants__["k" /* geminiPath */] + "/" + __WEBPACK_IMPORTED_MODULE_9__constants__["l" /* applicationPropertiesPath */];
            _this.router.navigateByUrl(newPath);
        };
        this.currentpane = function () { return _this.currentPaneId; };
        this.setHomeSinglePane = function () {
            _this.currentPaneId = 1;
            var path = _this.getPath();
            var segments = path.split("/");
            var mode = segments[1];
            var newPath = "/" + mode + "/" + __WEBPACK_IMPORTED_MODULE_9__constants__["i" /* homePath */];
            var tree = _this.router.createUrlTree([newPath]);
            _this.router.navigateByUrl(tree);
        };
        this.singlePane = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.currentPaneId = 1;
            if (!_this.isSinglePane()) {
                var paneToKeepId = paneId;
                var paneToRemoveId = __WEBPACK_IMPORTED_MODULE_8__models__["O" /* getOtherPane */](paneToKeepId);
                var path = _this.getPath();
                var segments = path.split("/");
                var mode = segments[1];
                var paneToKeep = segments[paneToKeepId + 1];
                var newPath = "/" + mode + "/" + paneToKeep;
                var search = _this.getSearch();
                if (paneToKeepId === 1) {
                    // just remove second pane
                    search = _this.clearPane(search, paneToRemoveId);
                }
                if (paneToKeepId === 2) {
                    // swap pane 2 to pane 1 then remove 2
                    search = _this.swapSearchIds(search);
                    search = _this.clearPane(search, 2);
                }
                var tree = _this.router.createUrlTree([newPath], { queryParams: search });
                _this.router.navigateByUrl(tree);
            }
        };
        this.isHome = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.isLocation(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["i" /* homePath */]);
        };
        this.isObject = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.isLocation(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["m" /* objectPath */]);
        };
        this.isList = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.isLocation(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["n" /* listPath */]);
        };
        this.isError = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.isLocation(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["o" /* errorPath */]);
        };
        this.isRecent = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.isLocation(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["p" /* recentPath */]);
        };
        this.isAttachment = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.isLocation(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["q" /* attachmentPath */]);
        };
        this.isApplicationProperties = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.isLocation(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["l" /* applicationPropertiesPath */]);
        };
        this.isMultiLineDialog = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return _this.isLocation(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["r" /* multiLineDialogPath */]);
        };
        this.triggerPageReloadByFlippingReloadFlagInUrl = function () {
            var search = _this.getSearch();
            _this.toggleReloadFlag(search);
            var result = { path: _this.getPath(), search: search, replace: true };
            _this.setNewSearch(result);
        };
        this.shortCutMarker = configService.config.shortCutMarker;
        this.urlShortCuts = configService.config.urlShortCuts;
        this.keySeparator = this.configService.config.keySeparator;
    }
    UrlManagerService.prototype.createSubMask = function (arr) {
        var nMask = 0;
        var nFlag = 0;
        if (arr.length > 31) {
            var msg = "UrlManagerService:createSubMask Out of range " + arr.length;
            this.loggerService.error(msg);
            throw new TypeError(msg);
        }
        var nLen = arr.length;
        for (nFlag; nFlag < nLen; nMask |= arr[nFlag] << nFlag++)
            ;
        return nMask;
    };
    // convert from array of bools to mask string
    UrlManagerService.prototype.createArrays = function (arr, arrays) {
        arrays = arrays || [];
        if (arr.length > 31) {
            arrays.push(arr.slice(0, 31));
            return this.createArrays(arr.slice(31), arrays);
        }
        arrays.push(arr);
        return arrays;
    };
    UrlManagerService.prototype.createMask = function (arr) {
        // split into smaller arrays if necessary 
        var _this = this;
        var arrays = this.createArrays(arr);
        var masks = __WEBPACK_IMPORTED_MODULE_7_lodash__["map"](arrays, function (a) { return _this.createSubMask(a).toString(); });
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["reduce"](masks, function (res, val) { return res + "-" + val; });
    };
    // convert from mask string to array of bools
    UrlManagerService.prototype.arrayFromSubMask = function (sMask) {
        var nMask = parseInt(sMask);
        // nMask must be between 0 and 2147483647 - to keep simple we stick to 31 bits 
        if (nMask > 0x7fffffff || nMask < -0x80000000) {
            var msg = "UrlManagerService:arrayFromSubMask Out of range " + nMask;
            this.loggerService.error(msg);
            throw new TypeError(msg);
        }
        var aFromMask = [];
        var len = 31; // make array always 31 bit long as we may concat another on end
        for (var nShifted = nMask; len > 0; aFromMask.push(Boolean(nShifted & 1)), nShifted >>>= 1, --len)
            ;
        return aFromMask;
    };
    UrlManagerService.prototype.arrayFromMask = function (sMask) {
        var _this = this;
        sMask = sMask || "0";
        var sMasks = sMask.split("-");
        var maskArrays = __WEBPACK_IMPORTED_MODULE_7_lodash__["map"](sMasks, function (s) { return _this.arrayFromSubMask(s); });
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["reduce"](maskArrays, function (res, val) { return res.concat(val); }, []);
    };
    UrlManagerService.prototype.getSearch = function () {
        var url = this.router.url;
        return this.router.parseUrl(url).queryParams;
    };
    UrlManagerService.prototype.getPath = function () {
        var url = this.router.url;
        var end = url.indexOf(";");
        end = end === -1 ? url.indexOf("?") : end;
        var path = url.substring(0, end > 0 ? end : url.length);
        return path;
    };
    UrlManagerService.prototype.setNewSearch = function (result) {
        var tree = this.router.createUrlTree([result.path], { queryParams: result.search });
        this.router.navigateByUrl(tree);
        if (result.replace) {
            var u = this.router.serializeUrl(tree);
            this.location.replaceState(u);
        }
    };
    UrlManagerService.prototype.getIds = function (typeOfId, paneId) {
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["pickBy"](this.getSearch(), function (v, k) { return !!k && k.indexOf(typeOfId + paneId) === 0; });
    };
    UrlManagerService.prototype.mapIds = function (ids) {
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["mapKeys"](ids, function (v, k) { return k.substr(k.indexOf("_") + 1); });
    };
    UrlManagerService.prototype.getAndMapIds = function (typeOfId, paneId) {
        var ids = this.getIds(typeOfId, paneId);
        return this.mapIds(ids);
    };
    UrlManagerService.prototype.getMappedValues = function (mappedIds) {
        var _this = this;
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["mapValues"](mappedIds, function (v) { return __WEBPACK_IMPORTED_MODULE_8__models__["j" /* Value */].fromJsonString(v, _this.shortCutMarker, _this.urlShortCuts); });
    };
    UrlManagerService.prototype.getInteractionMode = function (rawInteractionMode) {
        return rawInteractionMode ? __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */][rawInteractionMode] : __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].View;
    };
    UrlManagerService.prototype.setPaneRouteDataFromParms = function (paneRouteData, paneId, routeParams) {
        var _this = this;
        paneRouteData.menuId = this.getId(akm.menu + paneId, routeParams);
        paneRouteData.actionId = this.getId(akm.action + paneId, routeParams);
        paneRouteData.dialogId = this.getId(akm.dialog + paneId, routeParams);
        var rawErrorCategory = this.getId(akm.errorCat + paneId, routeParams);
        paneRouteData.errorCategory = rawErrorCategory ? __WEBPACK_IMPORTED_MODULE_8__models__["b" /* ErrorCategory */][rawErrorCategory] : null;
        paneRouteData.objectId = this.getId(akm.object + paneId, routeParams);
        paneRouteData.actionsOpen = this.getId(akm.actions + paneId, routeParams);
        var rawCollectionState = this.getId(akm.collection + paneId, routeParams);
        paneRouteData.state = rawCollectionState ? __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */][rawCollectionState] : __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].List;
        var rawInteractionMode = this.getId(akm.interactionMode + paneId, routeParams);
        paneRouteData.interactionMode = this.getInteractionMode(rawInteractionMode);
        var collKeyMap = this.getAndMapIds(akm.collection, paneId);
        paneRouteData.collections = __WEBPACK_IMPORTED_MODULE_7_lodash__["mapValues"](collKeyMap, function (v) { return __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */][v]; });
        var collSelectedKeyMap = this.getAndMapIds(akm.selected, paneId);
        paneRouteData.selectedCollectionItems = __WEBPACK_IMPORTED_MODULE_7_lodash__["mapValues"](collSelectedKeyMap, function (v) { return _this.arrayFromMask(v); });
        var parmKeyMap = this.getAndMapIds(akm.parm, paneId);
        paneRouteData.actionParams = this.getMappedValues(parmKeyMap);
        paneRouteData.page = parseInt(this.getId(akm.page + paneId, routeParams));
        paneRouteData.pageSize = parseInt(this.getId(akm.pageSize + paneId, routeParams));
        paneRouteData.attachmentId = this.getId(akm.attachment + paneId, routeParams);
    };
    UrlManagerService.prototype.setPaneRouteData = function (paneRouteData, paneId) {
        var routeParams = this.getSearch();
        this.setPaneRouteDataFromParms(paneRouteData, paneId, routeParams);
        paneRouteData.validate(this.location.path());
    };
    UrlManagerService.prototype.isSinglePane = function () {
        return this.getPath().split("/").length <= 3;
    };
    UrlManagerService.prototype.searchKeysForPane = function (search, paneId, raw) {
        var ids = __WEBPACK_IMPORTED_MODULE_7_lodash__["map"](raw, function (s) { return s + paneId; });
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["filter"](__WEBPACK_IMPORTED_MODULE_7_lodash__["keys"](search), function (k) { return __WEBPACK_IMPORTED_MODULE_7_lodash__["some"](ids, function (id) { return k.indexOf(id) === 0; }); });
    };
    UrlManagerService.prototype.allSearchKeysForPane = function (search, paneId) {
        var raw = __WEBPACK_IMPORTED_MODULE_7_lodash__["values"](akm);
        return this.searchKeysForPane(search, paneId, raw);
    };
    UrlManagerService.prototype.clearPane = function (search, paneId) {
        var toClear = this.allSearchKeysForPane(search, paneId);
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["omit"](search, toClear);
    };
    UrlManagerService.prototype.clearSearchKeys = function (search, paneId, keys) {
        var toClear = this.searchKeysForPane(search, paneId, keys);
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["omit"](search, toClear);
    };
    UrlManagerService.prototype.paneIsAlwaysSingle = function (paneType) {
        return paneType === __WEBPACK_IMPORTED_MODULE_9__constants__["r" /* multiLineDialogPath */];
    };
    UrlManagerService.prototype.setupPaneNumberAndTypes = function (pane, newPaneType, newMode) {
        var path = this.getPath();
        var segments = path.split("/");
        var mode = segments[1], pane1Type = segments[2], pane2Type = segments[3];
        var changeMode = false;
        var mayReplace = true;
        var newPath = path;
        if (newMode) {
            var newModeString = newMode.toString().toLowerCase();
            changeMode = mode !== newModeString;
            mode = newModeString;
        }
        // changing item on pane 1
        // make sure pane is of correct type
        if (pane === 1 && pane1Type !== newPaneType) {
            var single = this.isSinglePane() || this.paneIsAlwaysSingle(newPaneType);
            newPath = "/" + mode + "/" + newPaneType + (single ? "" : "/" + pane2Type);
            changeMode = false;
            mayReplace = false;
        }
        // changing item on pane 2
        // either single pane so need to add new pane of appropriate type
        // or double pane with second pane of wrong type. 
        if (pane === 2 && (this.isSinglePane() || pane2Type !== newPaneType)) {
            newPath = "/" + mode + "/" + pane1Type + "/" + newPaneType;
            changeMode = false;
            mayReplace = false;
        }
        if (changeMode) {
            newPath = "/" + mode + "/" + pane1Type + "/" + pane2Type;
            mayReplace = false;
        }
        return { path: newPath, replace: mayReplace };
    };
    UrlManagerService.prototype.capturePane = function (paneId) {
        var search = this.getSearch();
        var toCapture = this.allSearchKeysForPane(search, paneId);
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["pick"](search, toCapture);
    };
    UrlManagerService.prototype.getOidFromHref = function (href) {
        var oid = __WEBPACK_IMPORTED_MODULE_8__models__["d" /* ObjectIdWrapper */].fromHref(href, this.keySeparator);
        return oid.getKey();
    };
    UrlManagerService.prototype.getPidFromHref = function (href) {
        return __WEBPACK_IMPORTED_MODULE_8__models__["R" /* propertyIdFromUrl */](href);
    };
    UrlManagerService.prototype.setValue = function (paneId, search, p, pv, valueType) {
        this.setId("" + valueType + paneId + "_" + p.id(), pv.toJsonString(this.shortCutMarker, this.urlShortCuts), search);
    };
    UrlManagerService.prototype.setParameter = function (paneId, search, p, pv) {
        this.setValue(paneId, search, p, pv, akm.parm);
    };
    UrlManagerService.prototype.getId = function (key, search) {
        return __WEBPACK_IMPORTED_MODULE_8__models__["Q" /* decompress */](search[key], this.shortCutMarker, this.urlShortCuts);
    };
    UrlManagerService.prototype.setId = function (key, id, search) {
        search[key] = __WEBPACK_IMPORTED_MODULE_8__models__["S" /* compress */](id, this.shortCutMarker, this.urlShortCuts);
    };
    UrlManagerService.prototype.clearId = function (key, search) {
        delete search[key];
    };
    UrlManagerService.prototype.validKeysForHome = function () {
        return [akm.menu, akm.dialog, akm.reload];
    };
    UrlManagerService.prototype.validKeysForObject = function () {
        return [akm.object, akm.interactionMode, akm.reload, akm.actions, akm.dialog, akm.collection, akm.prop, akm.selected];
    };
    UrlManagerService.prototype.validKeysForMultiLineDialog = function () {
        return [akm.object, akm.dialog, akm.menu];
    };
    UrlManagerService.prototype.validKeysForList = function () {
        return [akm.reload, akm.actions, akm.dialog, akm.menu, akm.action, akm.page, akm.pageSize, akm.selected, akm.collection, akm.parm, akm.object];
    };
    UrlManagerService.prototype.validKeysForAttachment = function () {
        return [akm.object, akm.attachment];
    };
    UrlManagerService.prototype.validKeys = function (path) {
        switch (path) {
            case __WEBPACK_IMPORTED_MODULE_9__constants__["i" /* homePath */]:
                return this.validKeysForHome();
            case __WEBPACK_IMPORTED_MODULE_9__constants__["m" /* objectPath */]:
                return this.validKeysForObject();
            case __WEBPACK_IMPORTED_MODULE_9__constants__["n" /* listPath */]:
                return this.validKeysForList();
            case __WEBPACK_IMPORTED_MODULE_9__constants__["r" /* multiLineDialogPath */]:
                return this.validKeysForMultiLineDialog();
            case __WEBPACK_IMPORTED_MODULE_9__constants__["q" /* attachmentPath */]:
                return this.validKeysForAttachment();
        }
        return [];
    };
    UrlManagerService.prototype.clearInvalidParmsFromSearch = function (paneId, search, path) {
        if (path) {
            var vks = this.validKeys(path);
            var ivks = __WEBPACK_IMPORTED_MODULE_7_lodash__["without"].apply(__WEBPACK_IMPORTED_MODULE_7_lodash__, [__WEBPACK_IMPORTED_MODULE_7_lodash__["values"](akm)].concat(vks));
            return this.clearSearchKeys(search, paneId, ivks);
        }
        return search;
    };
    UrlManagerService.prototype.handleTransition = function (paneId, search, transition) {
        var replace = true;
        var path = this.getPath();
        switch (transition) {
            case (Transition.ToHome):
                (_a = this.setupPaneNumberAndTypes(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["i" /* homePath */]), path = _a.path, replace = _a.replace, _a);
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToMenu):
                search = this.clearPane(search, paneId);
                break;
            case (Transition.FromDialog):
                replace = true;
                break;
            case (Transition.ToDialog):
            case (Transition.FromDialogKeepHistory):
                replace = false;
                break;
            case (Transition.ToObjectView):
                (_b = this.setupPaneNumberAndTypes(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["m" /* objectPath */]), path = _b.path, replace = _b.replace, _b);
                replace = false;
                search = this.clearPane(search, paneId);
                this.setId(akm.interactionMode + paneId, __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */][__WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].View], search);
                break;
            case (Transition.ToObjectWithMode):
                (_c = this.setupPaneNumberAndTypes(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["m" /* objectPath */]), path = _c.path, replace = _c.replace, _c);
                replace = false;
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToList):
                (_d = this.setupPaneNumberAndTypes(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["n" /* listPath */]), path = _d.path, replace = _d.replace, _d);
                this.clearId(akm.menu + paneId, search);
                this.clearId(akm.object + paneId, search);
                this.clearId(akm.dialog + paneId, search);
                break;
            case (Transition.LeaveEdit):
                search = this.clearSearchKeys(search, paneId, [akm.prop]);
                break;
            case (Transition.Page):
                replace = false;
                break;
            case (Transition.ToTransient):
                replace = false;
                break;
            case (Transition.ToRecent):
                (_e = this.setupPaneNumberAndTypes(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["p" /* recentPath */]), path = _e.path, replace = _e.replace, _e);
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToAttachment):
                (_f = this.setupPaneNumberAndTypes(paneId, __WEBPACK_IMPORTED_MODULE_9__constants__["q" /* attachmentPath */]), path = _f.path, replace = _f.replace, _f);
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToMultiLineDialog):
                (_g = this.setupPaneNumberAndTypes(1, __WEBPACK_IMPORTED_MODULE_9__constants__["r" /* multiLineDialogPath */]), path = _g.path, replace = _g.replace, _g); // always on 1
                if (paneId === 2) {
                    // came from 2 
                    search = this.swapSearchIds(search);
                }
                search = this.clearPane(search, 2); // always on pane 1
                break;
            default:
                // null transition 
                break;
        }
        var segments = path.split("/");
        var pane1Type = segments[2], pane2Type = segments[3];
        search = this.clearInvalidParmsFromSearch(1, search, pane1Type);
        search = this.clearInvalidParmsFromSearch(2, search, pane2Type);
        return { path: path, search: search, replace: replace };
        var _a, _b, _c, _d, _e, _f, _g;
    };
    UrlManagerService.prototype.executeTransition = function (newValues, paneId, transition, condition) {
        var _this = this;
        this.currentPaneId = paneId;
        var search = this.getSearch();
        if (condition(search)) {
            var result = this.handleTransition(paneId, search, transition);
            (search = result.search, result);
            __WEBPACK_IMPORTED_MODULE_7_lodash__["forEach"](newValues, function (v, k) {
                // k should always be non null
                if (v)
                    _this.setId(k, v, search);
                else
                    _this.clearId(k, search);
            });
            this.setNewSearch(result);
        }
    };
    UrlManagerService.prototype.closeOrCancelDialog = function (id, paneId, transition) {
        var key = "" + akm.dialog + paneId;
        var existingValue = this.getSearch()[key];
        if (existingValue === id) {
            var newValues = __WEBPACK_IMPORTED_MODULE_7_lodash__["zipObject"]([key], [null]);
            this.executeTransition(newValues, paneId, transition, function () { return true; });
        }
    };
    UrlManagerService.prototype.checkAndSetValue = function (paneId, check, set) {
        this.currentPaneId = paneId;
        var search = this.getSearch();
        // only add field if matching dialog or dialog (to catch case when swapping panes) 
        if (check(search)) {
            set(search);
            var result = { path: this.getPath(), search: search, replace: false };
            this.setNewSearch(result);
        }
    };
    UrlManagerService.prototype.getViewType = function (view) {
        switch (view) {
            case __WEBPACK_IMPORTED_MODULE_9__constants__["i" /* homePath */]: return __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].Home;
            case __WEBPACK_IMPORTED_MODULE_9__constants__["m" /* objectPath */]: return __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].Object;
            case __WEBPACK_IMPORTED_MODULE_9__constants__["n" /* listPath */]: return __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].List;
            case __WEBPACK_IMPORTED_MODULE_9__constants__["o" /* errorPath */]: return __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].Error;
            case __WEBPACK_IMPORTED_MODULE_9__constants__["p" /* recentPath */]: return __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].Recent;
            case __WEBPACK_IMPORTED_MODULE_9__constants__["q" /* attachmentPath */]: return __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].Attachment;
            case __WEBPACK_IMPORTED_MODULE_9__constants__["l" /* applicationPropertiesPath */]: return __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].ApplicationProperties;
            case __WEBPACK_IMPORTED_MODULE_9__constants__["r" /* multiLineDialogPath */]: return __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].MultiLineDialog;
        }
        this.loggerService.throw("UrlManagerService:getViewType " + view + " is not a valid ViewType");
    };
    UrlManagerService.prototype.swapSearchIds = function (search) {
        return __WEBPACK_IMPORTED_MODULE_7_lodash__["mapKeys"](search, function (v, k) { return k.replace(/(\D+)(\d{1})(\w*)/, function (match, p1, p2, p3) { return ("" + p1 + (p2 === "1" ? "2" : "1") + p3); }); });
    };
    UrlManagerService.prototype.setMode = function (newMode) {
        var newPath = "/" + newMode + "/" + this.getPath().split("/")[2];
        this.router.navigateByUrl(newPath);
    };
    UrlManagerService.prototype.getLocation = function (paneId) {
        var path = this.getPath();
        var segments = path.split("/");
        return segments[paneId + 1]; // e.g. segments 0=~/1=cicero/2=home/3=home
    };
    UrlManagerService.prototype.isLocation = function (paneId, location) {
        return this.getLocation(paneId) === location; // e.g. segments 0=~/1=cicero/2=home/3=home
    };
    UrlManagerService.prototype.toggleReloadFlag = function (search) {
        var currentFlag = search[akm.reload];
        var newFlag = currentFlag === "1" ? 0 : 1;
        search[akm.reload] = newFlag;
        return search;
    };
    UrlManagerService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__angular_router__["b" /* Router */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__angular_router__["b" /* Router */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__angular_common__["a" /* Location */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__angular_common__["a" /* Location */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_4__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__config_service__["a" /* ConfigService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_5__logger_service__["a" /* LoggerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__logger_service__["a" /* LoggerService */]) === 'function' && _d) || Object])
    ], UrlManagerService);
    return UrlManagerService;
    var _a, _b, _c, _d;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/url-manager.service.js.map

/***/ }),

/***/ 252:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__mask_service__ = __webpack_require__(256);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__cicero_commands__ = __webpack_require__(585);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__error_service__ = __webpack_require__(46);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CiceroRendererService; });
/* harmony export (immutable) */ __webpack_exports__["b"] = openCollectionIds;
/* harmony export (immutable) */ __webpack_exports__["d"] = renderFieldValue;
/* harmony export (immutable) */ __webpack_exports__["c"] = renderCollectionNameAndSize;
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};










var CiceroRendererService = (function () {
    function CiceroRendererService(context, configService, error, mask) {
        var _this = this;
        this.context = context;
        this.configService = configService;
        this.error = error;
        this.mask = mask;
        this.renderObject = function (cvm, routeData) {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            }
            else {
                var oid = __WEBPACK_IMPORTED_MODULE_0__models__["d" /* ObjectIdWrapper */].fromObjectId(routeData.objectId, _this.keySeparator);
                _this.context.getObject(1, oid, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                    .then(function (obj) {
                    var openCollIds = openCollectionIds(routeData);
                    if (_.some(openCollIds)) {
                        _this.renderOpenCollection(openCollIds[0], obj, cvm);
                    }
                    else if (obj.isTransient()) {
                        _this.renderTransientObject(routeData, obj, cvm);
                    }
                    else if (routeData.interactionMode === __WEBPACK_IMPORTED_MODULE_3__route_data__["a" /* InteractionMode */].Edit ||
                        routeData.interactionMode === __WEBPACK_IMPORTED_MODULE_3__route_data__["a" /* InteractionMode */].Form) {
                        _this.renderForm(routeData, obj, cvm);
                    }
                    else {
                        _this.renderObjectTitleAndDialogIfOpen(routeData, obj, cvm);
                    }
                }).catch(function (reject) {
                    //TODO: Is the first test necessary or would this be rendered OK by generic error handling?
                    if (reject.category === __WEBPACK_IMPORTED_MODULE_0__models__["b" /* ErrorCategory */].ClientError && reject.clientErrorCode === __WEBPACK_IMPORTED_MODULE_0__models__["c" /* ClientErrorCode */].ExpiredTransient) {
                        cvm.setOutputSource(__WEBPACK_IMPORTED_MODULE_1__user_messages__["g" /* errorExpiredTransient */]);
                    }
                    else {
                        _this.error.handleError(reject);
                    }
                });
            }
        };
        this.renderList = function (cvm, routeData) {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            }
            else {
                var listPromise = _this.context.getListFromMenu(routeData, routeData.page, routeData.pageSize);
                listPromise.
                    then(function (list) {
                    _this.context.getMenu(routeData.menuId).
                        then(function (menu) {
                        var count = list.value().length;
                        var numPages = list.pagination().numPages;
                        var description = _this.getListDescription(numPages, list, count);
                        var actionMember = menu.actionMember(routeData.actionId, _this.keySeparator);
                        var actionName = actionMember.extensions().friendlyName();
                        var output = "Result from " + actionName + ":\n" + description;
                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                    }).
                        catch(function (reject) { return _this.error.handleError(reject); });
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
        };
        this.renderError = function (cvm) {
            var err = _this.context.getError().error;
            cvm.clearInput();
            cvm.setOutputSource("Sorry, an application error has occurred. " + err.message());
        };
        this.keySeparator = configService.config.keySeparator;
    }
    //TODO: remove renderer.
    CiceroRendererService.prototype.renderHome = function (cvm, routeData) {
        if (cvm.message) {
            cvm.outputMessageThenClearIt();
        }
        else {
            if (routeData.menuId) {
                this.renderOpenMenu(routeData, cvm);
            }
            else {
                cvm.clearInput();
                cvm.setOutputSource(__WEBPACK_IMPORTED_MODULE_1__user_messages__["m" /* welcomeMessage */]);
            }
        }
    };
    ;
    CiceroRendererService.prototype.getListDescription = function (numPages, list, count) {
        if (numPages > 1) {
            var page = list.pagination().page;
            var totalCount = list.pagination().totalCount;
            return "Page " + page + " of " + numPages + " containing " + count + " of " + totalCount + " items";
        }
        else {
            return count + " items";
        }
    };
    //TODO functions become 'private'
    //Returns collection Ids for any collections on an object that are currently in List or Table mode
    CiceroRendererService.prototype.openCollectionIds = function (routeData) {
        return _.filter(_.keys(routeData.collections), function (k) { return routeData.collections[k] != __WEBPACK_IMPORTED_MODULE_3__route_data__["b" /* CollectionViewState */].Summary; });
    };
    CiceroRendererService.prototype.renderOpenCollection = function (collId, obj, cvm) {
        var coll = obj.collectionMember(collId);
        var output = renderCollectionNameAndSize(coll);
        output += "(" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["n" /* collection */] + " " + __WEBPACK_IMPORTED_MODULE_1__user_messages__["o" /* on */] + " " + __WEBPACK_IMPORTED_MODULE_0__models__["e" /* typePlusTitle */](obj) + ")";
        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
    };
    CiceroRendererService.prototype.renderTransientObject = function (routeData, obj, cvm) {
        var output = __WEBPACK_IMPORTED_MODULE_1__user_messages__["p" /* unsaved */] + " ";
        output += obj.extensions().friendlyName() + "\n";
        output += this.renderModifiedProperties(obj, routeData, this.mask);
        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
    };
    CiceroRendererService.prototype.renderForm = function (routeData, obj, cvm) {
        var _this = this;
        var output = __WEBPACK_IMPORTED_MODULE_1__user_messages__["q" /* editing */] + " ";
        output += __WEBPACK_IMPORTED_MODULE_0__models__["e" /* typePlusTitle */](obj) + "\n";
        if (routeData.dialogId) {
            this.context.getInvokableAction(obj.actionMember(routeData.dialogId, this.keySeparator)).
                then(function (invokableAction) {
                output += _this.renderActionDialog(invokableAction, routeData, _this.mask);
                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        }
        else {
            output += this.renderModifiedProperties(obj, routeData, this.mask);
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }
    };
    CiceroRendererService.prototype.renderObjectTitleAndDialogIfOpen = function (routeData, obj, cvm) {
        var _this = this;
        var output = __WEBPACK_IMPORTED_MODULE_0__models__["e" /* typePlusTitle */](obj) + "\n";
        if (routeData.dialogId) {
            this.context.getInvokableAction(obj.actionMember(routeData.dialogId, this.keySeparator)).
                then(function (invokableAction) {
                output += _this.renderActionDialog(invokableAction, routeData, _this.mask);
                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        }
        else {
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }
    };
    CiceroRendererService.prototype.renderOpenMenu = function (routeData, cvm) {
        var _this = this;
        var output = "";
        this.context.getMenu(routeData.menuId).
            then(function (menu) {
            output += __WEBPACK_IMPORTED_MODULE_1__user_messages__["r" /* menuTitle */](menu.title());
            return routeData.dialogId ? _this.context.getInvokableAction(menu.actionMember(routeData.dialogId, _this.keySeparator)) : Promise.resolve(null);
        }).
            then(function (invokableAction) {
            if (invokableAction) {
                output += "\n" + _this.renderActionDialog(invokableAction, routeData, _this.mask);
            }
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }).
            catch(function (reject) {
            _this.error.handleError(reject);
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        });
    };
    CiceroRendererService.prototype.renderActionDialog = function (invokable, routeData, mask) {
        var actionName = invokable.extensions().friendlyName();
        var output = "Action dialog: " + actionName + "\n";
        _.forEach(__webpack_require__.i(__WEBPACK_IMPORTED_MODULE_7__cicero_commands__["a" /* getParametersAndCurrentValue */])(invokable, this.context), function (value, paramId) {
            output += __WEBPACK_IMPORTED_MODULE_0__models__["f" /* friendlyNameForParam */](invokable, paramId) + ": ";
            var param = invokable.parameters()[paramId];
            output += renderFieldValue(param, value, mask);
            output += "\n";
        });
        return output;
    };
    CiceroRendererService.prototype.renderModifiedProperties = function (obj, routeData, mask) {
        var output = "";
        var props = this.context.getObjectCachedValues(obj.id(this.keySeparator));
        if (_.keys(props).length > 0) {
            output += __WEBPACK_IMPORTED_MODULE_1__user_messages__["s" /* modifiedProperties */] + ":\n";
            _.each(props, function (value, propId) {
                output += __WEBPACK_IMPORTED_MODULE_0__models__["g" /* friendlyNameForProperty */](obj, propId) + ": ";
                var pm = obj.propertyMember(propId);
                output += renderFieldValue(pm, value, mask);
                output += "\n";
            });
        }
        return output;
    };
    CiceroRendererService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_2__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_4__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__context_service__["a" /* ContextService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_5__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__config_service__["a" /* ConfigService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_8__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__error_service__["a" /* ErrorService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_6__mask_service__["a" /* MaskService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__mask_service__["a" /* MaskService */]) === 'function' && _d) || Object])
    ], CiceroRendererService);
    return CiceroRendererService;
    var _a, _b, _c, _d;
}());
//Returns collection Ids for any collections on an object that are currently in List or Table mode
function openCollectionIds(routeData) {
    return _.filter(_.keys(routeData.collections), function (k) { return routeData.collections[k] !== __WEBPACK_IMPORTED_MODULE_3__route_data__["b" /* CollectionViewState */].Summary; });
}
//Handles empty values, and also enum conversion
function renderFieldValue(field, value, mask) {
    if (!field.isScalar()) {
        return value.isNull() ? __WEBPACK_IMPORTED_MODULE_1__user_messages__["t" /* empty */] : value.toString();
    }
    //Rest is for scalar fields only:
    if (value.toString()) {
        if (field.entryType() === __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].Choices) {
            return renderSingleChoice(field, value);
        }
        else if (field.entryType() === __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].MultipleChoices && value.isList()) {
            return renderMultipleChoicesCommaSeparated(field, value);
        }
    }
    var properScalarValue;
    if (this.isDateOrDateTime(field)) {
        properScalarValue = this.toUtcDate(value);
    }
    else {
        properScalarValue = value.scalar();
    }
    if (properScalarValue === "" || properScalarValue == null) {
        return __WEBPACK_IMPORTED_MODULE_1__user_messages__["t" /* empty */];
    }
    else {
        var remoteMask = field.extensions().mask();
        var format = field.extensions().format();
        return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
    }
}
function renderSingleChoice(field, value) {
    //This is to handle an enum: render it as text, not a number:  
    var inverted = _.invert(field.choices());
    return inverted[value.toValueString()];
}
function renderMultipleChoicesCommaSeparated(field, value) {
    //This is to handle an enum: render it as text, not a number: 
    var inverted = _.invert(field.choices());
    var output = "";
    var values = value.list();
    _.forEach(values, function (v) {
        output += inverted[v.toValueString()] + ",";
    });
    return output;
}
function renderCollectionNameAndSize(coll) {
    var output = coll.extensions().friendlyName() + ": ";
    switch (coll.size()) {
        case 0:
            output += __WEBPACK_IMPORTED_MODULE_1__user_messages__["t" /* empty */];
            break;
        case 1:
            output += "1 " + __WEBPACK_IMPORTED_MODULE_1__user_messages__["u" /* item */];
            break;
        default:
            output += this.numberOfItems(coll.size());
    }
    return output + "\n";
}
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/cicero-renderer.service.js.map

/***/ }),

/***/ 253:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(0);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ClickHandlerService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ClickHandlerService = (function () {
    function ClickHandlerService() {
        this.pane = this.sameOtherClickHandler;
    }
    ClickHandlerService.prototype.leftRightClickHandler = function (currentPane, right) {
        if (right === void 0) { right = false; }
        return right ? 2 : 1;
    };
    ClickHandlerService.prototype.sameOtherClickHandler = function (currentPane, right) {
        if (right === void 0) { right = false; }
        return right ? __WEBPACK_IMPORTED_MODULE_0__models__["O" /* getOtherPane */](currentPane) : currentPane;
    };
    ClickHandlerService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [])
    ], ClickHandlerService);
    return ClickHandlerService;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/click-handler.service.js.map

/***/ }),

/***/ 254:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__type_result_cache__ = __webpack_require__(601);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__config_service__ = __webpack_require__(21);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ColorService; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var ColorService = (function (_super) {
    __extends(ColorService, _super);
    function ColorService(context, configService) {
        var _this = this;
        _super.call(this, context);
        this.configService = configService;
        this.toColorNumberFromHref = function (href) {
            var type = _this.typeFromUrl(href);
            return _this.toColorNumberFromType(type);
        };
        this.toColorNumberFromType = function (type) { return _this.getResult(type); };
        _super.prototype.setDefault.call(this, 0);
        this.configureFromConfig();
    }
    ColorService.prototype.typeFromUrl = function (url) {
        var oid = __WEBPACK_IMPORTED_MODULE_0__models__["d" /* ObjectIdWrapper */].fromHref(url, this.configService.config.keySeparator);
        return oid.domainType;
    };
    ColorService.prototype.addType = function (type, result) {
        _super.prototype.addType.call(this, type, result);
    };
    ColorService.prototype.addMatch = function (matcher, result) {
        _super.prototype.addMatch.call(this, matcher, result);
    };
    ColorService.prototype.addSubtype = function (type, result) {
        _super.prototype.addSubtype.call(this, type, result);
    };
    ColorService.prototype.setDefault = function (def) {
        _super.prototype.setDefault.call(this, def);
    };
    ColorService.prototype.getDefault = function () {
        return this.default;
    };
    ColorService.prototype.configureFromConfig = function () {
        var _this = this;
        var colorConfig = this.configService.config.colors;
        if (colorConfig) {
            var typeMap = colorConfig.typeMap;
            var subtypeMap = colorConfig.subtypeMap;
            var regexArray = colorConfig.regexArray;
            var dflt = colorConfig.default;
            if (typeMap) {
                __WEBPACK_IMPORTED_MODULE_1_lodash__["forEach"](typeMap, function (v, k) { return _this.addType(k, v); });
            }
            if (regexArray) {
                __WEBPACK_IMPORTED_MODULE_1_lodash__["forEach"](regexArray, function (item) { return _this.addMatch(new RegExp(item.regex), item.color); });
            }
            if (subtypeMap) {
                __WEBPACK_IMPORTED_MODULE_1_lodash__["forEach"](subtypeMap, function (v, k) { return _this.addSubtype(k, v); });
            }
            if (dflt != null) {
                this.setDefault(dflt);
            }
        }
    };
    ColorService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_2__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_5__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__config_service__["a" /* ConfigService */]) === 'function' && _b) || Object])
    ], ColorService);
    return ColorService;
    var _a, _b;
}(__WEBPACK_IMPORTED_MODULE_4__type_result_cache__["a" /* TypeResultCache */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/color.service.js.map

/***/ }),

/***/ 255:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__custom_component_config_service__ = __webpack_require__(587);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__object_object_component__ = __webpack_require__(598);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__list_list_component__ = __webpack_require__(596);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__type_result_cache__ = __webpack_require__(601);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__error_error_component__ = __webpack_require__(593);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CustomComponentService; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};









var CustomComponentCache = (function (_super) {
    __extends(CustomComponentCache, _super);
    function CustomComponentCache(context, def) {
        _super.call(this, context);
        this.setDefault(def);
    }
    return CustomComponentCache;
}(__WEBPACK_IMPORTED_MODULE_6__type_result_cache__["a" /* TypeResultCache */]));
var CustomComponentService = (function () {
    function CustomComponentService(context, config) {
        this.context = context;
        this.config = config;
        this.customComponentCaches = [];
        this.customComponentCaches = [];
        this.customComponentCaches[__WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].Object] = new CustomComponentCache(context, __WEBPACK_IMPORTED_MODULE_2__object_object_component__["a" /* ObjectComponent */]);
        this.customComponentCaches[__WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].List] = new CustomComponentCache(context, __WEBPACK_IMPORTED_MODULE_5__list_list_component__["a" /* ListComponent */]);
        this.customComponentCaches[__WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].Error] = new CustomComponentCache(context, __WEBPACK_IMPORTED_MODULE_8__error_error_component__["a" /* ErrorComponent */]);
        config.configureCustomObjects(this.customComponentCaches[__WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].Object]);
        config.configureCustomLists(this.customComponentCaches[__WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].List]);
        config.configureCustomErrors(this);
    }
    CustomComponentService.prototype.getErrorKey = function (rc, code) {
        var key = __WEBPACK_IMPORTED_MODULE_3__models__["b" /* ErrorCategory */][rc] + "-" + (rc === __WEBPACK_IMPORTED_MODULE_3__models__["b" /* ErrorCategory */].ClientError ? __WEBPACK_IMPORTED_MODULE_3__models__["c" /* ClientErrorCode */][code] : __WEBPACK_IMPORTED_MODULE_3__models__["q" /* HttpStatusCode */][code]);
        return key;
    };
    CustomComponentService.prototype.getCustomComponent = function (domainType, viewType) {
        return this.customComponentCaches[viewType].getResult(domainType);
    };
    CustomComponentService.prototype.getCustomErrorComponent = function (rc, code) {
        var key = this.getErrorKey(rc, code);
        return this.customComponentCaches[__WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].Error].getResult(key);
    };
    CustomComponentService.prototype.addError = function (rc, code, result) {
        var key = this.getErrorKey(rc, code);
        this.customComponentCaches[__WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].Error].addType(key, result);
    };
    CustomComponentService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_7__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__context_service__["a" /* ContextService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__custom_component_config_service__["a" /* CustomComponentConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__custom_component_config_service__["a" /* CustomComponentConfigService */]) === 'function' && _b) || Object])
    ], CustomComponentService);
    return CustomComponentService;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/custom-component.service.js.map

/***/ }),

/***/ 256:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__(24);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_moment__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_moment___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_moment__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return MaskService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var LocalStringFilter = (function () {
    function LocalStringFilter() {
    }
    LocalStringFilter.prototype.filter = function (val) {
        return val ? val.toString() : "";
    };
    return LocalStringFilter;
}());
function transform(tfm) {
    try {
        return tfm();
    }
    catch (e) {
        return "";
    }
}
var LocalCurrencyFilter = (function () {
    function LocalCurrencyFilter(locale, symbol, digits) {
        this.locale = locale;
        this.symbol = symbol;
        this.digits = digits;
    }
    LocalCurrencyFilter.prototype.filter = function (val) {
        var _this = this;
        if (!val) {
            return "";
        }
        var pipe = new __WEBPACK_IMPORTED_MODULE_1__angular_common__["i" /* CurrencyPipe */](this.locale);
        return transform(function () { return pipe.transform(val, _this.symbol, true, _this.digits); });
    };
    return LocalCurrencyFilter;
}());
var LocalDateFilter = (function () {
    function LocalDateFilter(locale, mask, tz) {
        this.locale = locale;
        this.mask = mask;
        this.tz = tz;
    }
    LocalDateFilter.prototype.filter = function (val) {
        if (!val) {
            return "";
        }
        // Angular date pipes no longer support timezones so we need to use moment here 
        var mmt = __WEBPACK_IMPORTED_MODULE_4_moment__["utc"](val);
        if (this.tz) {
            mmt = mmt.utcOffset(this.tz);
        }
        return mmt.format(this.mask);
    };
    return LocalDateFilter;
}());
var LocalNumberFilter = (function () {
    function LocalNumberFilter(locale, digits) {
        this.locale = locale;
        this.digits = digits;
    }
    LocalNumberFilter.prototype.filter = function (val) {
        var _this = this;
        if (val == null || val === "") {
            return "";
        }
        var pipe = new __WEBPACK_IMPORTED_MODULE_1__angular_common__["j" /* DecimalPipe */](this.locale);
        return transform(function () { return pipe.transform(val, _this.digits); });
    };
    return LocalNumberFilter;
}());
var MaskService = (function () {
    function MaskService(appConfig) {
        this.appConfig = appConfig;
        this.maskMap = {
            string: {},
            "date-time": {},
            date: {},
            time: {},
            "utc-millisec": {},
            "big-integer": {},
            "big-decimal": {},
            blob: {},
            clob: {},
            decimal: {},
            int: {}
        };
        this.defaultLocale = appConfig.config.defaultLocale;
        this.configureFromConfig();
    }
    MaskService.prototype.defaultLocalFilter = function (format) {
        switch (format) {
            case ("string"):
                return new LocalStringFilter();
            case ("date-time"):
                return new LocalDateFilter(this.defaultLocale, "D MMM YYYY HH:mm:ss");
            case ("date"):
                return new LocalDateFilter(this.defaultLocale, "D MMM YYYY", "+0000");
            case ("time"):
                return new LocalDateFilter(this.defaultLocale, "HH:mm", "+0000");
            case ("utc-millisec"):
                return new LocalNumberFilter(this.defaultLocale);
            case ("big-integer"):
                return new LocalNumberFilter(this.defaultLocale);
            case ("big-decimal"):
                return new LocalNumberFilter(this.defaultLocale);
            case ("blob"):
                return new LocalStringFilter();
            case ("clob"):
                return new LocalStringFilter();
            case ("decimal"):
                return new LocalNumberFilter(this.defaultLocale);
            case ("int"):
                return new LocalNumberFilter(this.defaultLocale);
            default:
                return new LocalStringFilter();
        }
    };
    ;
    MaskService.prototype.customFilter = function (format, remoteMask) {
        if (remoteMask && this.maskMap[format]) {
            return this.maskMap[format][remoteMask];
        }
        return undefined;
    };
    MaskService.prototype.toLocalFilter = function (remoteMask, format) {
        return this.customFilter(format, remoteMask) || this.defaultLocalFilter(format);
    };
    ;
    MaskService.prototype.setNumberMaskMapping = function (customMask, format, digits, locale) {
        this.maskMap[format][customMask] = new LocalNumberFilter(locale || this.defaultLocale, digits);
    };
    ;
    MaskService.prototype.setDateMaskMapping = function (customMask, format, mask, tz, locale) {
        this.maskMap[format][customMask] = new LocalDateFilter(locale || this.defaultLocale, mask, tz);
    };
    ;
    MaskService.prototype.setCurrencyMaskMapping = function (customMask, format, symbol, digits, locale) {
        this.maskMap[format][customMask] = new LocalCurrencyFilter(locale || this.defaultLocale, symbol, digits);
    };
    ;
    MaskService.prototype.configureFromConfig = function () {
        var _this = this;
        var maskConfig = this.appConfig.config.masks;
        if (maskConfig) {
            var currencyMasks = maskConfig.currencyMasks;
            var dateMasks = maskConfig.dateMasks;
            var numberMasks = maskConfig.numberMasks;
            if (currencyMasks) {
                __WEBPACK_IMPORTED_MODULE_0_lodash__["forEach"](currencyMasks, function (v, k) { return _this.setCurrencyMaskMapping(k, v.format, v.symbol, v.digits, v.locale); });
            }
            if (dateMasks) {
                __WEBPACK_IMPORTED_MODULE_0_lodash__["forEach"](dateMasks, function (v, k) { return _this.setDateMaskMapping(k, v.format, v.mask, v.tz, v.locale); });
            }
            if (numberMasks) {
                __WEBPACK_IMPORTED_MODULE_0_lodash__["forEach"](numberMasks, function (v, k) { return _this.setNumberMaskMapping(k, v.format, v.digits, v.locale); });
            }
        }
    };
    MaskService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_2__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_3__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__config_service__["a" /* ConfigService */]) === 'function' && _a) || Object])
    ], MaskService);
    return MaskService;
    var _a;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/mask.service.js.map

/***/ }),

/***/ 257:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__message_view_model__ = __webpack_require__(190);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return DialogViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};




var DialogViewModel = (function (_super) {
    __extends(DialogViewModel, _super);
    function DialogViewModel(color, context, viewModelFactory, urlManager, error, routeData, action, actionViewModel, isMultiLineDialogRow) {
        var _this = this;
        _super.call(this);
        this.color = color;
        this.context = context;
        this.viewModelFactory = viewModelFactory;
        this.urlManager = urlManager;
        this.error = error;
        this.routeData = routeData;
        this.isMultiLineDialogRow = isMultiLineDialogRow;
        this.actionMember = function () { return _this.actionViewModel.actionRep; };
        this.execute = function (right) {
            var pps = _this.parameters;
            return _this.actionViewModel.execute(pps, right);
        };
        this.submitted = false;
        this.closed = false; // make sure we never close more than once 
        this.refresh = function () {
            var fields = _this.context.getDialogCachedValues(_this.actionMember().actionId(), _this.onPaneId);
            __WEBPACK_IMPORTED_MODULE_3_lodash__["forEach"](_this.parameters, function (p) { return p.refresh(fields[p.id]); });
        };
        this.clientValid = function () { return __WEBPACK_IMPORTED_MODULE_3_lodash__["every"](_this.parameters, function (p) { return p.clientValid; }); };
        this.tooltip = function () { return __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__["g" /* tooltip */](_this, _this.parameters); };
        this.setParms = function () { return __WEBPACK_IMPORTED_MODULE_3_lodash__["forEach"](_this.parameters, function (p) { return _this.context.cacheFieldValue(_this.actionMember().actionId(), p.parameterRep.id(), p.getValue(), _this.onPaneId); }); };
        this.doInvoke = function (right) {
            return _this.execute(right)
                .then(function (actionResult) {
                if (actionResult.shouldExpectResult()) {
                    _this.setMessage(actionResult.warningsOrMessages() || __WEBPACK_IMPORTED_MODULE_1__user_messages__["_86" /* noResultMessage */]);
                }
                else if (actionResult.resultType() === "void") {
                    // dialog staying on same page so treat as cancel 
                    // for url replacing purposes
                    _this.doCloseReplaceHistory();
                }
                else if (!_this.isQueryOnly) {
                    // not query only - always close
                    _this.doClose();
                }
                else if (!right) {
                    // query only going to new page close dialog and keep history
                    _this.doClose();
                }
                // else query only going to other tab leave dialog open
                _this.doCompleteButLeaveOpen();
            })
                .catch(function (reject) {
                var display = function (em) { return __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__["h" /* handleErrorResponse */](em, _this, _this.parameters); };
                _this.error.handleErrorAndDisplayMessages(reject, display);
            });
        };
        // todo tidy and rework these getting confusing 
        this.doCloseKeepHistory = function () {
            _this.urlManager.closeDialogKeepHistory(_this.id, _this.onPaneId);
            _this.decrementPendingPotentAction();
        };
        this.doCloseReplaceHistory = function () {
            _this.urlManager.closeDialogReplaceHistory(_this.id, _this.onPaneId);
            _this.decrementPendingPotentAction();
        };
        this.doCompleteButLeaveOpen = function () {
        };
        this.clearMessages = function () {
            _this.resetMessage();
            __WEBPACK_IMPORTED_MODULE_3_lodash__["each"](_this.actionViewModel.parameters, function (parm) { return parm.clearMessage(); });
        };
        // todo not happy with the whole invokable action thing here casting is horrid.
        this.actionViewModel = actionViewModel ||
            this.viewModelFactory.actionViewModel(action, this, routeData);
        this.actionViewModel.makeInvokable(action);
        this.onPaneId = routeData.paneId;
        var fields = this.context.getDialogCachedValues(this.actionMember().actionId(), this.onPaneId);
        var parameters = __WEBPACK_IMPORTED_MODULE_3_lodash__["pickBy"](this.actionViewModel.invokableActionRep.parameters(), function (p) { return !p.isCollectionContributed(); });
        this.parameters = __WEBPACK_IMPORTED_MODULE_3_lodash__["map"](parameters, function (p) { return _this.viewModelFactory.parameterViewModel(p, fields[p.id()], _this.onPaneId); });
        this.title = this.actionMember().extensions().friendlyName();
        this.isQueryOnly = this.actionViewModel.invokableActionRep.isQueryOnly();
        this.resetMessage();
        this.id = this.actionViewModel.actionRep.actionId();
        this.incrementPendingPotentAction();
    }
    DialogViewModel.prototype.incrementPendingPotentAction = function () {
        if (!this.isMultiLineDialogRow) {
            __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__["i" /* incrementPendingPotentAction */](this.context, this.actionViewModel.invokableActionRep, this.onPaneId);
        }
    };
    DialogViewModel.prototype.decrementPendingPotentAction = function () {
        if (!this.isMultiLineDialogRow && !this.closed) {
            __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__["j" /* decrementPendingPotentAction */](this.context, this.actionViewModel.invokableActionRep, this.onPaneId);
        }
        this.closed = true;
    };
    DialogViewModel.prototype.doClose = function () {
        this.decrementPendingPotentAction();
    };
    return DialogViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__message_view_model__["a" /* MessageViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/dialog-view-model.js.map

/***/ }),

/***/ 258:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__message_view_model__ = __webpack_require__(190);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__choice_view_model__ = __webpack_require__(106);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_5_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__user_messages__ = __webpack_require__(30);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return DomainObjectViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};







var DomainObjectViewModel = (function (_super) {
    __extends(DomainObjectViewModel, _super);
    function DomainObjectViewModel(colorService, contextService, viewModelFactory, urlManager, error, configService, obj, routeData, forceReload) {
        var _this = this;
        _super.call(this);
        this.colorService = colorService;
        this.contextService = contextService;
        this.viewModelFactory = viewModelFactory;
        this.urlManager = urlManager;
        this.error = error;
        this.configService = configService;
        this.routeData = routeData;
        this.draggableTitle = function () { return _this.title; };
        this.editProperties = function () { return __WEBPACK_IMPORTED_MODULE_5_lodash__["filter"](_this.properties, function (p) { return p.isEditable && p.isDirty(); }); };
        this.isFormOrTransient = function () { return _this.domainObject.extensions().interactionMode() === "form" || _this.domainObject.extensions().interactionMode() === "transient"; };
        this.cancelHandler = function () { return _this.isFormOrTransient() ? function () { return _this.urlManager.popUrlState(_this.onPaneId); } : function () { return _this.urlManager.setInteractionMode(__WEBPACK_IMPORTED_MODULE_1__route_data__["a" /* InteractionMode */].View, _this.onPaneId); }; };
        this.saveHandler = function () {
            return _this.domainObject.isTransient() ? _this.contextService.saveObject : _this.contextService.updateObject;
        };
        this.validateHandler = function () { return _this.domainObject.isTransient() ? _this.contextService.validateSaveObject : _this.contextService.validateUpdateObject; };
        // leave this a lambda as it's passed as a function and we must keep the 'this'. 
        this.propertyMap = function () {
            var pps = __WEBPACK_IMPORTED_MODULE_5_lodash__["filter"](_this.properties, function (property) { return property.isEditable; });
            return __WEBPACK_IMPORTED_MODULE_5_lodash__["zipObject"](__WEBPACK_IMPORTED_MODULE_5_lodash__["map"](pps, function (p) { return p.id; }), __WEBPACK_IMPORTED_MODULE_5_lodash__["map"](pps, function (p) { return p.getValue(); }));
        };
        this.clientValid = function () { return __WEBPACK_IMPORTED_MODULE_5_lodash__["every"](_this.properties, function (p) { return p.clientValid; }); };
        this.tooltip = function () { return __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__["g" /* tooltip */](_this, _this.properties); };
        this.actionsTooltip = function () { return __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__["f" /* actionsTooltip */](_this, !!_this.routeData.actionsOpen); };
        this.toggleActionMenu = function () {
            _this.urlManager.toggleObjectMenu(_this.onPaneId);
        };
        this.setProperties = function () { return __WEBPACK_IMPORTED_MODULE_5_lodash__["forEach"](_this.editProperties(), function (p) { return _this.contextService.cachePropertyValue(_this.domainObject, p.propertyRep, p.getValue(), _this.onPaneId); }); };
        this.doEditCancel = function () {
            _this.contextService.clearObjectCachedValues(_this.onPaneId);
            _this.cancelHandler()();
        };
        this.clearCachedFiles = function () { return __WEBPACK_IMPORTED_MODULE_5_lodash__["forEach"](_this.properties, function (p) { return p.attachment ? p.attachment.clearCachedFile() : null; }); };
        this.doSave = function (viewObject) {
            _this.clearCachedFiles();
            var propMap = _this.propertyMap();
            _this.saveHandler()(_this.domainObject, propMap, _this.onPaneId, viewObject)
                .then(function (obj) { return _this.reset(obj, _this.urlManager.getRouteData().pane(_this.onPaneId), true); })
                .catch(function (reject) { return _this.handleWrappedError(reject); });
        };
        this.currentPaneData = function () { return _this.urlManager.getRouteData().pane(_this.onPaneId); };
        this.doSaveValidate = function () {
            var propMap = _this.propertyMap();
            return _this.validateHandler()(_this.domainObject, propMap)
                .then(function () {
                _this.resetMessage();
                return true;
            })
                .catch(function (reject) {
                _this.handleWrappedError(reject);
                return Promise.reject(false);
            });
        };
        this.doEdit = function () {
            _this.clearCachedFiles();
            _this.contextService.clearObjectCachedValues(_this.onPaneId);
            _this.contextService.getObjectForEdit(_this.onPaneId, _this.domainObject)
                .then(function (updatedObject) {
                _this.reset(updatedObject, _this.currentPaneData(), true);
                _this.urlManager.pushUrlState(_this.onPaneId);
                _this.urlManager.setInteractionMode(__WEBPACK_IMPORTED_MODULE_1__route_data__["a" /* InteractionMode */].Edit, _this.onPaneId);
            })
                .catch(function (reject) { return _this.handleWrappedError(reject); });
        };
        this.doReload = function () {
            _this.clearCachedFiles();
            _this.contextService.reloadObject(_this.onPaneId, _this.domainObject)
                .then(function (updatedObject) { return _this.reset(updatedObject, _this.currentPaneData(), true); })
                .catch(function (reject) { return _this.handleWrappedError(reject); });
        };
        this.hideEdit = function () { return _this.isFormOrTransient() || __WEBPACK_IMPORTED_MODULE_5_lodash__["every"](_this.properties, function (p) { return !p.isEditable; }); };
        this.disableActions = function () { return !_this.actions || _this.actions.length === 0; };
        this.canDropOn = function (targetType) { return _this.contextService.isSubTypeOf(_this.domainType, targetType); };
        this.showActions = function () { return !!_this.currentPaneData().actionsOpen; };
        this.keySeparator = configService.config.keySeparator;
        this.reset(obj, routeData, forceReload);
    }
    DomainObjectViewModel.prototype.handleWrappedError = function (reject) {
        var _this = this;
        var display = function (em) { return __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__["h" /* handleErrorResponse */](em, _this, _this.properties); };
        this.error.handleErrorAndDisplayMessages(reject, display);
    };
    ;
    DomainObjectViewModel.prototype.wrapAction = function (a) {
        var _this = this;
        var wrappedInvoke = a.execute;
        a.execute = function (pps, right) {
            _this.setProperties();
            var pairs = __WEBPACK_IMPORTED_MODULE_5_lodash__["map"](_this.editProperties(), function (p) { return [p.id, p.getValue()]; });
            var prps = __WEBPACK_IMPORTED_MODULE_5_lodash__["fromPairs"](pairs);
            var parmValueMap = __WEBPACK_IMPORTED_MODULE_5_lodash__["mapValues"](a.invokableActionRep.parameters(), function (p) { return ({ parm: p, value: prps[p.id()] }); });
            var allpps = __WEBPACK_IMPORTED_MODULE_5_lodash__["map"](parmValueMap, function (o) { return _this.viewModelFactory.parameterViewModel(o.parm, o.value, _this.onPaneId); });
            return wrappedInvoke(allpps, right)
                .catch(function (reject) {
                _this.handleWrappedError(reject);
                return Promise.reject(reject);
            });
        };
    };
    DomainObjectViewModel.prototype.reset = function (obj, routeData, resetting) {
        var _this = this;
        this.domainObject = obj;
        this.onPaneId = routeData.paneId;
        this.routeData = routeData;
        var iMode = this.domainObject.extensions().interactionMode();
        this.isInEdit = routeData.interactionMode !== __WEBPACK_IMPORTED_MODULE_1__route_data__["a" /* InteractionMode */].View || iMode === "form" || iMode === "transient";
        this.props = routeData.interactionMode !== __WEBPACK_IMPORTED_MODULE_1__route_data__["a" /* InteractionMode */].View ? this.contextService.getObjectCachedValues(this.domainObject.id(this.keySeparator), routeData.paneId) : {};
        var actions = __WEBPACK_IMPORTED_MODULE_5_lodash__["values"](this.domainObject.actionMembers());
        this.actions = __WEBPACK_IMPORTED_MODULE_5_lodash__["map"](actions, function (action) { return _this.viewModelFactory.actionViewModel(action, _this, _this.routeData); });
        this.menuItems = __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__["a" /* createMenuItems */](this.actions);
        this.properties = __WEBPACK_IMPORTED_MODULE_5_lodash__["map"](this.domainObject.propertyMembers(), function (property, id) { return _this.viewModelFactory.propertyViewModel(property, id, _this.props[id], _this.onPaneId, _this.propertyMap); });
        this.collections = __WEBPACK_IMPORTED_MODULE_5_lodash__["map"](this.domainObject.collectionMembers(), function (collection) { return _this.viewModelFactory.collectionViewModel(collection, _this.routeData, resetting); });
        this.unsaved = routeData.interactionMode === __WEBPACK_IMPORTED_MODULE_1__route_data__["a" /* InteractionMode */].Transient;
        this.title = this.unsaved ? "Unsaved " + this.domainObject.extensions().friendlyName() : this.domainObject.title();
        this.title = this.title + __WEBPACK_IMPORTED_MODULE_3__models__["z" /* dirtyMarker */](this.contextService, this.configService, obj.getOid(this.keySeparator));
        this.friendlyName = this.domainObject.extensions().friendlyName();
        this.presentationHint = this.domainObject.extensions().presentationHint();
        this.domainType = this.domainObject.domainType();
        this.instanceId = this.domainObject.instanceId();
        this.draggableType = this.domainObject.domainType();
        var selfAsValue = function () {
            var link = _this.domainObject.selfLink();
            if (link) {
                // not transient - can't drag transients so no need to set up IDraggable members on transients
                link.setTitle(_this.title);
                return new __WEBPACK_IMPORTED_MODULE_3__models__["j" /* Value */](link);
            }
            return null;
        };
        var sav = selfAsValue();
        this.value = sav ? sav.toString() : "";
        this.reference = sav ? sav.toValueString() : "";
        this.selectedChoice = sav ? new __WEBPACK_IMPORTED_MODULE_2__choice_view_model__["a" /* ChoiceViewModel */](sav, "") : null;
        this.colorService.toColorNumberFromType(this.domainObject.domainType())
            .then(function (c) { return _this.color = "" + _this.configService.config.objectColor + c; })
            .catch(function (reject) { return _this.error.handleError(reject); });
        this.resetMessage();
        if (routeData.interactionMode === __WEBPACK_IMPORTED_MODULE_1__route_data__["a" /* InteractionMode */].Form) {
            __WEBPACK_IMPORTED_MODULE_5_lodash__["forEach"](this.actions, function (a) { return _this.wrapAction(a); });
        }
    };
    DomainObjectViewModel.prototype.concurrency = function () {
        var _this = this;
        this.routeData = this.urlManager.getRouteData().pane(this.onPaneId);
        this.contextService.getObject(this.onPaneId, this.domainObject.getOid(this.keySeparator), this.routeData.interactionMode)
            .then(function (obj) {
            // cleared cached values so all values are from reloaded representation 
            _this.contextService.clearObjectCachedValues(_this.onPaneId);
            return _this.contextService.reloadObject(_this.onPaneId, obj);
        })
            .then(function (reloadedObj) {
            if (_this.routeData.dialogId) {
                _this.urlManager.closeDialogReplaceHistory(_this.routeData.dialogId, _this.onPaneId);
            }
            _this.reset(reloadedObj, _this.routeData, true);
            var em = new __WEBPACK_IMPORTED_MODULE_3__models__["w" /* ErrorMap */]({}, 0, __WEBPACK_IMPORTED_MODULE_6__user_messages__["_92" /* concurrencyMessage */]);
            __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__["h" /* handleErrorResponse */](em, _this, _this.properties);
        })
            .catch(function (reject) { return _this.error.handleError(reject); });
    };
    ;
    return DomainObjectViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__message_view_model__["a" /* MessageViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/domain-object-view-model.js.map

/***/ }),

/***/ 259:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__message_view_model__ = __webpack_require__(190);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__choice_view_model__ = __webpack_require__(106);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__helpers_view_models__ = __webpack_require__(41);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return FieldViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};






var FieldViewModel = (function (_super) {
    __extends(FieldViewModel, _super);
    function FieldViewModel(rep, colorService, error, context, configService, onPaneId, isScalar, id, isCollectionContributed, entryType) {
        var _this = this;
        _super.call(this);
        this.rep = rep;
        this.colorService = colorService;
        this.error = error;
        this.context = context;
        this.configService = configService;
        this.onPaneId = onPaneId;
        this.isScalar = isScalar;
        this.id = id;
        this.isCollectionContributed = isCollectionContributed;
        this.entryType = entryType;
        this.clientValid = true;
        this.reference = "";
        this.currentRawValue = null;
        this.choiceOptions = [];
        this.drop = function (newValue) { return __WEBPACK_IMPORTED_MODULE_5__helpers_view_models__["c" /* drop */](_this.context, _this.error, _this, newValue); };
        this.validate = function (modelValue, viewValue, mandatoryOnly) { return __WEBPACK_IMPORTED_MODULE_5__helpers_view_models__["d" /* validate */](_this.rep, _this, modelValue, viewValue, mandatoryOnly); };
        this.validator = function (c) {
            var viewValue = c.value;
            var isvalid = _this.isValid(viewValue);
            return isvalid ? null : { invalid: "invalid entry" };
        };
        this.setNewValue = function (newValue) {
            _this.selectedChoice = newValue.selectedChoice;
            _this.value = newValue.value;
            _this.reference = newValue.reference;
        };
        this.clear = function () {
            _this.selectedChoice = null;
            _this.value = null;
            _this.reference = "";
        };
        this.setValueFromControl = function (newValue) {
            if (newValue instanceof Array) {
                _this.selectedMultiChoices = newValue;
            }
            else if (newValue instanceof __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */]) {
                _this.selectedChoice = newValue;
            }
            else {
                _this.value = newValue;
            }
        };
        this.getValueForControl = function () { return _this.selectedMultiChoices || _this.selectedChoice || _this.value; };
        this.getValue = function () {
            if (_this.entryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].File) {
                return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](_this.file);
            }
            if (_this.entryType !== __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].FreeForm || _this.isCollectionContributed) {
                if (_this.entryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleChoices || _this.entryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleConditionalChoices || _this.isCollectionContributed) {
                    var selections = _this.selectedMultiChoices || [];
                    if (_this.type === "scalar") {
                        var selValues = __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](selections, function (cvm) { return cvm.getValue().scalar(); });
                        return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](selValues);
                    }
                    var selRefs = __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](selections, function (cvm) { return ({ href: cvm.getValue().href(), title: cvm.name }); }); // reference 
                    return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](selRefs);
                }
                var choiceValue = _this.selectedChoice ? _this.selectedChoice.getValue() : null;
                if (_this.type === "scalar") {
                    return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](choiceValue && choiceValue.scalar() != null ? choiceValue.scalar() : "");
                }
                // reference 
                return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](choiceValue && choiceValue.isReference() && _this.selectedChoice ? { href: choiceValue.href(), title: _this.selectedChoice.name } : null);
            }
            if (_this.type === "scalar") {
                if (_this.value == null) {
                    return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */]("");
                }
                if (_this.value instanceof Date) {
                    if (_this.format === "time") {
                        // time format
                        return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](__WEBPACK_IMPORTED_MODULE_2__models__["x" /* toTimeString */](_this.value));
                    }
                    if (_this.format === "date") {
                        // truncate time;
                        return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](__WEBPACK_IMPORTED_MODULE_2__models__["n" /* toDateString */](_this.value));
                    }
                    // date-time
                    return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](_this.value.toISOString());
                }
                return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](_this.value);
            }
            // reference
            return new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](_this.reference ? { href: _this.reference, title: _this.value.toString() } : null);
        };
        var ext = rep.extensions();
        this.optional = ext.optional();
        this.description = ext.description();
        this.presentationHint = ext.presentationHint();
        this.mask = ext.mask();
        this.title = ext.friendlyName();
        this.returnType = ext.returnType();
        this.format = __WEBPACK_IMPORTED_MODULE_2__models__["y" /* withNull */](ext.format());
        this.multipleLines = ext.multipleLines() || 1;
        this.password = ext.dataType() === "password";
        this.type = isScalar ? "scalar" : "ref";
        this.argId = "" + id.toLowerCase();
        this.paneArgId = "" + this.argId + onPaneId;
    }
    Object.defineProperty(FieldViewModel.prototype, "choices", {
        get: function () {
            return this.choiceOptions;
        },
        set: function (options) {
            this.choiceOptions = options;
            if (this.entryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleConditionalChoices) {
                var currentSelectedOptions_1 = this.selectedMultiChoices;
                this.selectedMultiChoices = __WEBPACK_IMPORTED_MODULE_4_lodash__["filter"](this.choiceOptions, function (c) { return __WEBPACK_IMPORTED_MODULE_4_lodash__["some"](currentSelectedOptions_1, function (choiceToSet) { return c.valuesEqual(choiceToSet); }); });
            }
            else if (this.entryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].ConditionalChoices) {
                var currentSelectedOption_1 = this.selectedChoice;
                this.selectedChoice = __WEBPACK_IMPORTED_MODULE_4_lodash__["find"](this.choiceOptions, function (c) { return c.valuesEqual(currentSelectedOption_1); });
            }
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(FieldViewModel.prototype, "selectedChoice", {
        get: function () {
            return this.currentChoice;
        },
        set: function (newChoice) {
            // type guard because angular pushes string value here until directive finds 
            // choice
            if (newChoice instanceof __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */] || newChoice == null) {
                this.currentChoice = newChoice;
                this.update();
            }
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(FieldViewModel.prototype, "value", {
        get: function () {
            return this.currentRawValue;
        },
        set: function (newValue) {
            this.currentRawValue = newValue;
            this.update();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(FieldViewModel.prototype, "selectedMultiChoices", {
        get: function () {
            return this.currentMultipleChoices;
        },
        set: function (choices) {
            this.currentMultipleChoices = choices;
            this.update();
        },
        enumerable: true,
        configurable: true
    });
    FieldViewModel.prototype.isValid = function (viewValue) {
        var _this = this;
        var val;
        if (viewValue instanceof __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */]) {
            val = viewValue.getValue().toValueString();
        }
        else if (viewValue instanceof Array) {
            if (viewValue.length) {
                return __WEBPACK_IMPORTED_MODULE_4_lodash__["every"](viewValue, function (v) { return _this.isValid(v); });
            }
            val = "";
        }
        else {
            val = viewValue;
        }
        if (this.entryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].AutoComplete && !(viewValue instanceof __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */])) {
            if (val) {
                this.setMessage(__WEBPACK_IMPORTED_MODULE_3__user_messages__["_88" /* pendingAutoComplete */]);
                this.clientValid = false;
                return false;
            }
            else if (!this.optional) {
                this.resetMessage();
                this.clientValid = false;
                return false;
            }
        }
        // only fully validate freeform scalar 
        var fullValidate = this.entryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].FreeForm && this.type === "scalar";
        return this.validate(viewValue, val, !fullValidate);
    };
    ;
    FieldViewModel.prototype.update = function () {
        this.setColor();
    };
    ;
    FieldViewModel.prototype.setupChoices = function (choices) {
        var _this = this;
        this.choices = __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](choices, function (v, n) { return new __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */](v, _this.id, n); });
    };
    FieldViewModel.prototype.setupAutocomplete = function (rep, parentValues, digest) {
        var _this = this;
        this.prompt = function (searchTerm) {
            var createcvm = __WEBPACK_IMPORTED_MODULE_4_lodash__["partial"](__WEBPACK_IMPORTED_MODULE_5__helpers_view_models__["e" /* createChoiceViewModels */], _this.id, searchTerm);
            return _this.context.autoComplete(rep, _this.id, parentValues, searchTerm, digest).then(createcvm);
        };
        var promptLink = rep.promptLink(); // always
        this.minLength = promptLink.extensions().minLength(); // always 
        this.description = this.description || __WEBPACK_IMPORTED_MODULE_3__user_messages__["_89" /* autoCompletePrompt */];
    };
    FieldViewModel.prototype.setupConditionalChoices = function (rep, digest) {
        var _this = this;
        this.conditionalChoices = function (args) {
            var createcvm = __WEBPACK_IMPORTED_MODULE_4_lodash__["partial"](__WEBPACK_IMPORTED_MODULE_5__helpers_view_models__["e" /* createChoiceViewModels */], _this.id, null);
            return _this.context.conditionalChoices(rep, _this.id, function () { return {}; }, args, digest).then(createcvm);
        };
        var promptLink = rep.promptLink();
        this.promptArguments = __WEBPACK_IMPORTED_MODULE_4_lodash__["fromPairs"](__WEBPACK_IMPORTED_MODULE_4_lodash__["map"](promptLink.arguments(), function (v, key) { return [key, new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](v.value)]; }));
    };
    FieldViewModel.prototype.getRequiredIndicator = function () {
        return this.optional || typeof this.value === "boolean" ? "" : "* ";
    };
    FieldViewModel.prototype.setColor = function () {
        var _this = this;
        if (this.entryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].AutoComplete && this.selectedChoice && this.type === "ref") {
            var href = this.selectedChoice.getValue().href();
            if (href) {
                this.colorService.toColorNumberFromHref(href)
                    .then(function (c) {
                    // only if we still have a choice may have been cleared by a later call
                    if (_this.selectedChoice) {
                        _this.color = "" + _this.configService.config.linkColor + c;
                    }
                })
                    .catch(function (reject) { return _this.error.handleError(reject); });
                return;
            }
        }
        else if (this.entryType !== __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].AutoComplete && this.value) {
            this.colorService.toColorNumberFromType(this.returnType)
                .then(function (c) {
                // only if we still have a value may have been cleared by a later call
                if (_this.value) {
                    _this.color = "" + _this.configService.config.linkColor + c;
                }
            })
                .catch(function (reject) { return _this.error.handleError(reject); });
            return;
        }
        this.color = "";
    };
    return FieldViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__message_view_model__["a" /* MessageViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/field-view-model.js.map

/***/ }),

/***/ 26:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__rep_loader_service__ = __webpack_require__(365);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__constants__ = __webpack_require__(188);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_6_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_rxjs_Subject__ = __webpack_require__(20);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_7_rxjs_Subject__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__logger_service__ = __webpack_require__(87);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ContextService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};










var DirtyState;
(function (DirtyState) {
    DirtyState[DirtyState["DirtyMustReload"] = 0] = "DirtyMustReload";
    DirtyState[DirtyState["DirtyMayReload"] = 1] = "DirtyMayReload";
    DirtyState[DirtyState["Clean"] = 2] = "Clean";
})(DirtyState || (DirtyState = {}));
var DirtyList = (function () {
    function DirtyList() {
        this.dirtyObjects = {};
    }
    DirtyList.prototype.setDirty = function (oid, alwaysReload) {
        if (alwaysReload === void 0) { alwaysReload = false; }
        this.setDirtyInternal(oid, alwaysReload ? DirtyState.DirtyMustReload : DirtyState.DirtyMayReload);
    };
    DirtyList.prototype.setDirtyInternal = function (oid, dirtyState) {
        var key = oid.getKey();
        this.dirtyObjects[key] = dirtyState;
    };
    DirtyList.prototype.getDirty = function (oid) {
        var key = oid.getKey();
        return this.dirtyObjects[key] || DirtyState.Clean;
    };
    DirtyList.prototype.clearDirty = function (oid) {
        var key = oid.getKey();
        this.dirtyObjects = __WEBPACK_IMPORTED_MODULE_6_lodash__["omit"](this.dirtyObjects, key);
    };
    DirtyList.prototype.clear = function () {
        this.dirtyObjects = {};
    };
    return DirtyList;
}());
function isSameObject(object, type, id) {
    if (object) {
        var sid = object.serviceId();
        return sid ? sid === type : (object.domainType() === type && object.instanceId() === id);
    }
    return false;
}
var TransientCache = (function () {
    function TransientCache(depth) {
        this.depth = depth;
        // todo - later - maybe ts 2.1 - investigate if we can use an enum for pane and not have empty array in index 0 ? 
        this.transientCache = [[], [], []]; // per pane 
    }
    TransientCache.prototype.add = function (paneId, obj) {
        var paneObjects = this.transientCache[paneId];
        if (paneObjects.length >= this.depth) {
            paneObjects = paneObjects.slice(-(this.depth - 1));
        }
        paneObjects.push(obj);
        this.transientCache[paneId] = paneObjects;
    };
    TransientCache.prototype.get = function (paneId, type, id) {
        var paneObjects = this.transientCache[paneId];
        return __WEBPACK_IMPORTED_MODULE_6_lodash__["find"](paneObjects, function (o) { return isSameObject(o, type, id); }) || null;
    };
    TransientCache.prototype.remove = function (paneId, type, id) {
        var paneObjects = this.transientCache[paneId];
        paneObjects = __WEBPACK_IMPORTED_MODULE_6_lodash__["remove"](paneObjects, function (o) { return isSameObject(o, type, id); });
        this.transientCache[paneId] = paneObjects;
    };
    TransientCache.prototype.clear = function () {
        this.transientCache = [[], [], []];
    };
    TransientCache.prototype.swap = function () {
        var _a = this.transientCache, t1 = _a[1], t2 = _a[2];
        this.transientCache[1] = t2;
        this.transientCache[2] = t1;
    };
    return TransientCache;
}());
var RecentCache = (function () {
    function RecentCache(keySeparator, depth) {
        this.keySeparator = keySeparator;
        this.depth = depth;
        this.recentCache = [];
    }
    RecentCache.prototype.add = function (obj) {
        var _this = this;
        // find any matching entries and remove them - should only be one
        __WEBPACK_IMPORTED_MODULE_6_lodash__["remove"](this.recentCache, function (i) { return i.id(_this.keySeparator) === obj.id(_this.keySeparator); });
        // push obj on top of array 
        this.recentCache = [obj].concat(this.recentCache);
        // drop oldest if we're full 
        if (this.recentCache.length > this.depth) {
            this.recentCache = this.recentCache.slice(0, this.depth);
        }
    };
    RecentCache.prototype.items = function () {
        return this.recentCache;
    };
    RecentCache.prototype.clear = function () {
        this.recentCache = [];
    };
    return RecentCache;
}());
var ValueCache = (function () {
    function ValueCache() {
        this.currentValues = [{}, {}, {}];
        this.currentId = ["", "", ""];
    }
    ValueCache.prototype.addValue = function (id, valueId, value, paneId) {
        if (this.currentId[paneId] !== id) {
            this.currentId[paneId] = id;
            this.currentValues[paneId] = {};
        }
        this.currentValues[paneId][valueId] = value;
    };
    ValueCache.prototype.getValue = function (id, valueId, paneId) {
        if (this.currentId[paneId] !== id) {
            this.currentId[paneId] = id;
            this.currentValues[paneId] = {};
        }
        return this.currentValues[paneId][valueId];
    };
    ValueCache.prototype.getValues = function (id, paneId) {
        if (id && this.currentId[paneId] !== id) {
            this.currentId[paneId] = id;
            this.currentValues[paneId] = {};
        }
        return this.currentValues[paneId];
    };
    ValueCache.prototype.clear = function (paneId) {
        this.currentId[paneId] = "";
        this.currentValues[paneId] = {};
    };
    ValueCache.prototype.swap = function () {
        var _a = this.currentId, i1 = _a[1], i2 = _a[2];
        this.currentId[1] = i2;
        this.currentId[2] = i1;
        var _b = this.currentValues, v1 = _b[1], v2 = _b[2];
        this.currentValues[1] = v2;
        this.currentValues[2] = v1;
    };
    return ValueCache;
}());
var ContextService = (function () {
    function ContextService(urlManager, repLoader, configService, loggerService) {
        var _this = this;
        this.urlManager = urlManager;
        this.repLoader = repLoader;
        this.configService = configService;
        this.loggerService = loggerService;
        // cached values
        this.currentObjects = []; // per pane 
        this.transientCache = new TransientCache(this.configService.config.transientCacheDepth);
        this.currentMenuList = {};
        this.currentServices = null;
        this.currentMenus = null;
        this.currentVersion = null;
        this.currentUser = null;
        this.recentcache = new RecentCache(this.keySeparator, this.configService.config.recentCacheDepth);
        this.dirtyList = new DirtyList();
        this.currentLists = {};
        this.parameterCache = new ValueCache();
        this.objectEditCache = new ValueCache();
        this.getFile = function (object, url, mt) {
            var isDirty = _this.getIsDirty(object.getOid(_this.keySeparator));
            return _this.repLoader.getFile(url, mt, isDirty);
        };
        this.setFile = function (object, url, mt, file) { return _this.repLoader.uploadFile(url, mt, file); };
        this.clearCachedFile = function (url) { return _this.repLoader.clearCache(url); };
        // exposed for test mocking
        this.getDomainObject = function (paneId, oid, interactionMode) {
            var type = oid.domainType;
            var id = oid.instanceId;
            var dirtyState = _this.dirtyList.getDirty(oid);
            var forceReload = (dirtyState === DirtyState.DirtyMustReload) || ((dirtyState === DirtyState.DirtyMayReload) && _this.configService.config.autoLoadDirty);
            if (!forceReload && isSameObject(_this.currentObjects[paneId], type, id)) {
                return Promise.resolve(_this.currentObjects[paneId]);
            }
            // deeper cache for transients
            if (interactionMode === __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Transient) {
                var transientObj = _this.transientCache.get(paneId, type, id);
                var p = transientObj
                    ? Promise.resolve(transientObj)
                    : Promise.reject(new __WEBPACK_IMPORTED_MODULE_5__models__["a" /* ErrorWrapper */](__WEBPACK_IMPORTED_MODULE_5__models__["b" /* ErrorCategory */].ClientError, __WEBPACK_IMPORTED_MODULE_5__models__["c" /* ClientErrorCode */].ExpiredTransient, ""));
                return p;
            }
            var object = new __WEBPACK_IMPORTED_MODULE_5__models__["o" /* DomainObjectRepresentation */]();
            object.hateoasUrl = _this.configService.config.appPath + "/objects/" + type + "/" + id;
            object.setInlinePropertyDetails(interactionMode === __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Edit);
            _this.incPendingPotentActionOrReload(paneId);
            return _this.repLoader.populate(object, forceReload)
                .then(function (obj) {
                _this.currentObjects[paneId] = obj;
                if (forceReload) {
                    _this.dirtyList.clearDirty(oid);
                }
                _this.cacheRecentlyViewed(obj);
                _this.decPendingPotentActionOrReload(paneId);
                return Promise.resolve(obj);
            });
        };
        this.getIsDirty = function (oid) { return !oid.isService && _this.dirtyList.getDirty(oid) !== DirtyState.Clean; };
        this.mustReload = function (oid) {
            var dirtyState = _this.dirtyList.getDirty(oid);
            return (dirtyState === DirtyState.DirtyMustReload) || ((dirtyState === DirtyState.DirtyMayReload) && _this.configService.config.autoLoadDirty);
        };
        this.getObjectForEdit = function (paneId, object) { return _this.editOrReloadObject(paneId, object, true); };
        this.reloadObject = function (paneId, object) { return _this.editOrReloadObject(paneId, object, false); };
        this.getService = function (paneId, serviceType) {
            if (isSameObject(_this.currentObjects[paneId], serviceType)) {
                return Promise.resolve(_this.currentObjects[paneId]);
            }
            return _this.getServices()
                .then(function (services) {
                var service = services.getService(serviceType);
                if (service) {
                    return _this.repLoader.populate(service);
                }
                return Promise.reject("unknown service " + serviceType);
            })
                .then(function (service) {
                _this.currentObjects[paneId] = service;
                return Promise.resolve(service);
            });
        };
        this.getActionDetails = function (actionMember) {
            var details = actionMember.getDetails();
            if (details) {
                return _this.repLoader.populate(details, true);
            }
            return Promise.reject("Couldn't find details on " + actionMember.actionId());
        };
        this.getCollectionDetails = function (collectionMember, state, ignoreCache) {
            var details = collectionMember.getDetails();
            if (details) {
                if (state === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table) {
                    details.setUrlParameter(__WEBPACK_IMPORTED_MODULE_4__constants__["h" /* roInlineCollectionItems */], true);
                }
                var parent = collectionMember.parent;
                var oid = parent.getOid(_this.keySeparator);
                var isDirty = _this.dirtyList.getDirty(oid) !== DirtyState.Clean;
                return _this.repLoader.populate(details, isDirty || ignoreCache);
            }
            return Promise.reject("Couldn't find details on " + collectionMember.collectionId());
        };
        this.getInvokableAction = function (action) {
            if (action.invokeLink()) {
                return Promise.resolve(action);
            }
            return _this.getActionDetails(action);
        };
        this.getMenu = function (menuId) {
            if (_this.currentMenuList[menuId]) {
                return Promise.resolve(_this.currentMenuList[menuId]);
            }
            return _this.getMenus()
                .then(function (menus) {
                var menu = menus.getMenu(menuId);
                if (menu) {
                    return _this.repLoader.populate(menu);
                }
                return Promise.reject("couldn't find menu " + menuId);
            })
                .then(function (menu) {
                _this.currentMenuList[menuId] = menu;
                return Promise.resolve(menu);
            });
        };
        this.clearMessages = function () { return _this.messagesSource.next([]); };
        this.clearWarnings = function () { return _this.warningsSource.next([]); };
        this.broadcastMessage = function (m) { return _this.messagesSource.next([m]); };
        this.broadcastWarning = function (w) { return _this.warningsSource.next([w]); };
        this.getHome = function () {
            // for moment don't bother caching only called on startup and for whatever resaon cache doesn't work. 
            // once version cached no longer called.  
            return _this.repLoader.populate(new __WEBPACK_IMPORTED_MODULE_5__models__["I" /* HomePageRepresentation */]({}, _this.configService.config.appPath));
        };
        this.getServices = function () {
            if (_this.currentServices) {
                return Promise.resolve(_this.currentServices);
            }
            return _this.getHome()
                .then(function (home) {
                var ds = home.getDomainServices();
                return _this.repLoader.populate(ds);
            })
                .then(function (services) {
                _this.currentServices = services;
                return Promise.resolve(services);
            });
        };
        this.getMenus = function () {
            if (_this.currentMenus) {
                return Promise.resolve(_this.currentMenus);
            }
            return _this.getHome()
                .then(function (home) {
                var ds = home.getMenus();
                return _this.repLoader.populate(ds);
            })
                .then(function (menus) {
                _this.currentMenus = menus;
                return Promise.resolve(_this.currentMenus);
            });
        };
        this.getVersion = function () {
            if (_this.currentVersion) {
                return Promise.resolve(_this.currentVersion);
            }
            return _this.getHome()
                .then(function (home) {
                var v = home.getVersion();
                return _this.repLoader.populate(v);
            })
                .then(function (version) {
                _this.currentVersion = version;
                return Promise.resolve(version);
            });
        };
        this.getUser = function () {
            if (_this.currentUser) {
                return Promise.resolve(_this.currentUser);
            }
            return _this.getHome()
                .then(function (home) {
                var u = home.getUser();
                return _this.repLoader.populate(u);
            })
                .then(function (user) {
                _this.currentUser = user;
                return Promise.resolve(user);
            });
        };
        this.getObject = function (paneId, oid, interactionMode) {
            return oid.isService ? _this.getService(paneId, oid.domainType) : _this.getDomainObject(paneId, oid, interactionMode);
        };
        this.getCachedList = function (paneId, page, pageSize) {
            var index = _this.urlManager.getListCacheIndex(paneId, page, pageSize);
            var entry = _this.currentLists[index];
            return entry ? entry.list : null;
        };
        this.clearCachedList = function (paneId, page, pageSize) {
            var index = _this.urlManager.getListCacheIndex(paneId, page, pageSize);
            delete _this.currentLists[index];
        };
        this.handleResult = function (paneId, result, page, pageSize) {
            if (result.resultType() === "list") {
                var resultList = result.result().list(); // not null
                var index = _this.urlManager.getListCacheIndex(paneId, page, pageSize);
                _this.cacheList(resultList, index);
                return Promise.resolve(resultList);
            }
            else {
                return Promise.reject(new __WEBPACK_IMPORTED_MODULE_5__models__["a" /* ErrorWrapper */](__WEBPACK_IMPORTED_MODULE_5__models__["b" /* ErrorCategory */].ClientError, __WEBPACK_IMPORTED_MODULE_5__models__["c" /* ClientErrorCode */].WrongType, "expect list"));
            }
        };
        this.getList = function (paneId, resultPromise, page, pageSize) {
            return resultPromise().then(function (result) { return _this.handleResult(paneId, result, page, pageSize); });
        };
        this.getActionExtensionsFromMenu = function (menuId, actionId) {
            return _this.getMenu(menuId).then(function (menu) { return Promise.resolve(menu.actionMember(actionId, _this.keySeparator).extensions()); });
        };
        this.getActionExtensionsFromObject = function (paneId, oid, actionId) {
            return _this.getObject(paneId, oid, __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].View).then(function (object) { return Promise.resolve(object.actionMember(actionId, _this.keySeparator).extensions()); });
        };
        this.getListFromMenu = function (routeData, page, pageSize) {
            var menuId = routeData.menuId;
            var actionId = routeData.actionId;
            var parms = routeData.actionParams;
            var state = routeData.state;
            var paneId = routeData.paneId;
            var newPage = page || routeData.page;
            var newPageSize = pageSize || routeData.pageSize;
            var urlParms = _this.getPagingParms(newPage, newPageSize);
            if (state === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table) {
                urlParms[__WEBPACK_IMPORTED_MODULE_4__constants__["h" /* roInlineCollectionItems */]] = true;
            }
            var promise = function () { return _this.getMenu(menuId).then(function (menu) { return _this.getInvokableAction(menu.actionMember(actionId, _this.keySeparator)); }).then(function (details) { return _this.repLoader.invoke(details, parms, urlParms); }); };
            return _this.getList(paneId, promise, newPage, newPageSize);
        };
        this.getListFromObject = function (routeData, page, pageSize) {
            var objectId = routeData.objectId;
            var actionId = routeData.actionId;
            var parms = routeData.actionParams;
            var state = routeData.state;
            var oid = __WEBPACK_IMPORTED_MODULE_5__models__["d" /* ObjectIdWrapper */].fromObjectId(objectId, _this.keySeparator);
            var paneId = routeData.paneId;
            var newPage = page || routeData.page;
            var newPageSize = pageSize || routeData.pageSize;
            var urlParms = _this.getPagingParms(newPage, newPageSize);
            if (state === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table) {
                urlParms[__WEBPACK_IMPORTED_MODULE_4__constants__["h" /* roInlineCollectionItems */]] = true;
            }
            var promise = function () { return _this.getObject(paneId, oid, __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].View)
                .then(function (object) { return _this.getInvokableAction(object.actionMember(actionId, _this.keySeparator)); })
                .then(function (details) { return _this.repLoader.invoke(details, parms, urlParms); }); };
            return _this.getList(paneId, promise, newPage, newPageSize);
        };
        this.setObject = function (paneId, co) { return _this.currentObjects[paneId] = co; };
        this.swapCurrentObjects = function () {
            _this.parameterCache.swap();
            _this.objectEditCache.swap();
            _this.transientCache.swap();
            var _a = _this.currentObjects, p1 = _a[1], p2 = _a[2];
            _this.currentObjects[1] = p2;
            _this.currentObjects[2] = p1;
        };
        this.currentError = null;
        this.getError = function () { return _this.currentError; };
        this.setError = function (e) { return _this.currentError = e; };
        this.previousUrl = null;
        this.getPreviousUrl = function () { return _this.previousUrl; };
        this.setPreviousUrl = function (url) { return _this.previousUrl = url; };
        this.doPrompt = function (field, id, searchTerm, setupPrompt, objectValues, digest) {
            var map = field.getPromptMap(); // not null
            map.setMembers(objectValues);
            setupPrompt(map);
            var addEmptyOption = field.entryType() !== __WEBPACK_IMPORTED_MODULE_5__models__["h" /* EntryType */].AutoComplete && field.extensions().optional();
            return _this.repLoader.retrieve(map, __WEBPACK_IMPORTED_MODULE_5__models__["J" /* PromptRepresentation */], digest).then(function (p) { return p.choices(addEmptyOption); });
        };
        this.autoComplete = function (field, id, objectValues, searchTerm, digest) {
            return _this.doPrompt(field, id, searchTerm, function (map) { return map.setSearchTerm(searchTerm); }, objectValues, digest);
        };
        this.conditionalChoices = function (field, id, objectValues, args, digest) {
            return _this.doPrompt(field, id, null, function (map) { return map.setArguments(args); }, objectValues, digest);
        };
        this.nextTransientId = 0;
        this.setResult = function (action, result, fromPaneId, toPaneId, page, pageSize) {
            if (!result.result().isNull()) {
                if (result.resultType() === "object") {
                    var resultObject = result.result().object();
                    if (resultObject.persistLink()) {
                        // transient object
                        var domainType = resultObject.extensions().domainType();
                        resultObject.wrapped().domainType = domainType;
                        resultObject.wrapped().instanceId = (_this.nextTransientId++).toString();
                        resultObject.hateoasUrl = "/" + domainType + "/" + _this.nextTransientId;
                        // copy the etag down into the object
                        resultObject.etagDigest = result.etagDigest;
                        _this.setObject(toPaneId, resultObject);
                        _this.transientCache.add(toPaneId, resultObject);
                        _this.urlManager.pushUrlState(toPaneId);
                        var interactionMode = resultObject.extensions().interactionMode() === "transient"
                            ? __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Transient
                            : __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].NotPersistent;
                        _this.urlManager.setObjectWithMode(resultObject, interactionMode, toPaneId);
                    }
                    else if (resultObject.selfLink()) {
                        var selfLink = resultObject.selfLink();
                        // persistent object
                        // set the object here and then update the url. That should reload the page but pick up this object 
                        // so we don't hit the server again. 
                        // copy the etag down into the object
                        resultObject.etagDigest = result.etagDigest;
                        _this.setObject(toPaneId, resultObject);
                        // update angular cache 
                        var url = selfLink.href() + "?" + __WEBPACK_IMPORTED_MODULE_4__constants__["e" /* roInlinePropertyDetails */] + "=false";
                        _this.repLoader.addToCache(url, resultObject.wrapped());
                        // if render in edit must be  a form 
                        if (resultObject.extensions().interactionMode() === "form") {
                            _this.urlManager.pushUrlState(toPaneId);
                            _this.urlManager.setObjectWithMode(resultObject, __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Form, toPaneId);
                        }
                        else {
                            _this.cacheRecentlyViewed(resultObject);
                            _this.urlManager.setObject(resultObject, toPaneId);
                        }
                    }
                    else {
                        _this.loggerService.throw("ContextService:setResult result object without self or persist link");
                    }
                }
                else if (result.resultType() === "list") {
                    var resultList = result.result().list();
                    var parms = _this.parameterCache.getValues(action.actionId(), fromPaneId);
                    var search = _this.urlManager.setList(action, parms, fromPaneId, toPaneId);
                    var index = _this.urlManager.getListCacheIndexFromSearch(search, toPaneId, page, pageSize);
                    _this.cacheList(resultList, index);
                }
            }
            else if (result.resultType() === "void") {
                _this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
            }
        };
        this.pendingPotentActionCount = [, 0, 0];
        this.warningsSource = new __WEBPACK_IMPORTED_MODULE_7_rxjs_Subject__["Subject"]();
        this.messagesSource = new __WEBPACK_IMPORTED_MODULE_7_rxjs_Subject__["Subject"]();
        this.warning$ = this.warningsSource.asObservable();
        this.messages$ = this.messagesSource.asObservable();
        this.copiedViewModelSource = new __WEBPACK_IMPORTED_MODULE_7_rxjs_Subject__["Subject"]();
        this.copiedViewModel$ = this.copiedViewModelSource.asObservable();
        this.concurrencyErrorSource = new __WEBPACK_IMPORTED_MODULE_7_rxjs_Subject__["Subject"]();
        this.concurrencyError$ = this.concurrencyErrorSource.asObservable();
        this.invokeAction = function (action, parms, fromPaneId, toPaneId, gotoResult) {
            if (fromPaneId === void 0) { fromPaneId = 1; }
            if (toPaneId === void 0) { toPaneId = 1; }
            if (gotoResult === void 0) { gotoResult = true; }
            var invokeOnMap = function (iAction) {
                var im = iAction.getInvokeMap();
                __WEBPACK_IMPORTED_MODULE_6_lodash__["each"](parms, function (parm, k) { return im.setParameter(k, parm); });
                var setDirty = _this.getSetDirtyFunction(iAction, parms);
                return _this.invokeActionInternal(im, iAction, fromPaneId, toPaneId, setDirty, gotoResult);
            };
            return invokeOnMap(action);
        };
        this.updateObject = function (object, props, paneId, viewSavedObject) {
            var update = object.getUpdateMap();
            __WEBPACK_IMPORTED_MODULE_6_lodash__["each"](props, function (v, k) { return update.setProperty(k, v); });
            return _this.repLoader.retrieve(update, __WEBPACK_IMPORTED_MODULE_5__models__["o" /* DomainObjectRepresentation */], object.etagDigest)
                .then(function (updatedObject) {
                // This is a kludge because updated object has no self link.
                var rawLinks = object.wrapped().links;
                updatedObject.wrapped().links = rawLinks;
                _this.setNewObject(updatedObject, paneId, viewSavedObject);
                return Promise.resolve(updatedObject);
            });
        };
        this.saveObject = function (object, props, paneId, viewSavedObject) {
            var persist = object.getPersistMap();
            __WEBPACK_IMPORTED_MODULE_6_lodash__["each"](props, function (v, k) { return persist.setMember(k, v); });
            return _this.repLoader.retrieve(persist, __WEBPACK_IMPORTED_MODULE_5__models__["o" /* DomainObjectRepresentation */], object.etagDigest)
                .then(function (updatedObject) {
                _this.transientCache.remove(paneId, object.domainType(), object.id(_this.keySeparator));
                _this.setNewObject(updatedObject, paneId, viewSavedObject);
                return Promise.resolve(updatedObject);
            });
        };
        this.validateUpdateObject = function (object, props) {
            var update = object.getUpdateMap();
            update.setValidateOnly();
            __WEBPACK_IMPORTED_MODULE_6_lodash__["each"](props, function (v, k) { return update.setProperty(k, v); });
            return _this.repLoader.validate(update, object.etagDigest);
        };
        this.validateSaveObject = function (object, props) {
            var persist = object.getPersistMap();
            persist.setValidateOnly();
            __WEBPACK_IMPORTED_MODULE_6_lodash__["each"](props, function (v, k) { return persist.setMember(k, v); });
            return _this.repLoader.validate(persist, object.etagDigest);
        };
        this.subTypeCache = {};
        this.isSubTypeOf = function (toCheckType, againstType) {
            if (_this.subTypeCache[toCheckType] && typeof _this.subTypeCache[toCheckType][againstType] !== "undefined") {
                return _this.subTypeCache[toCheckType][againstType];
            }
            var isSubTypeOf = new __WEBPACK_IMPORTED_MODULE_5__models__["K" /* DomainTypeActionInvokeRepresentation */](againstType, toCheckType, _this.configService.config.appPath);
            var promise = _this.repLoader.populate(isSubTypeOf, true)
                .then(function (updatedObject) {
                return updatedObject.value();
            })
                .catch(function (reject) {
                return false;
            });
            var entry = {};
            entry[againstType] = promise;
            _this.subTypeCache[toCheckType] = entry;
            return promise;
        };
        this.getRecentlyViewed = function () { return _this.recentcache.items(); };
        this.clearRecentlyViewed = function () { return _this.recentcache.clear(); };
        this.cacheFieldValue = function (dialogId, pid, pv, paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.parameterCache.addValue(dialogId, pid, pv, paneId);
        };
        this.getDialogCachedValues = function (dialogId, paneId) {
            if (dialogId === void 0) { dialogId = null; }
            if (paneId === void 0) { paneId = 1; }
            return _this.parameterCache.getValues(dialogId, paneId);
        };
        this.getObjectCachedValues = function (objectId, paneId) {
            if (objectId === void 0) { objectId = null; }
            if (paneId === void 0) { paneId = 1; }
            return _this.objectEditCache.getValues(objectId, paneId);
        };
        this.clearDialogCachedValues = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.parameterCache.clear(paneId);
        };
        this.clearObjectCachedValues = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.objectEditCache.clear(paneId);
        };
        this.cachePropertyValue = function (obj, p, pv, paneId) {
            if (paneId === void 0) { paneId = 1; }
            _this.dirtyList.setDirty(obj.getOid(_this.keySeparator));
            _this.objectEditCache.addValue(obj.id(_this.keySeparator), p.id(), pv, paneId);
        };
        this.keySeparator = this.configService.config.keySeparator;
    }
    ContextService.prototype.editOrReloadObject = function (paneId, object, inlineDetails) {
        var _this = this;
        var parms = {};
        parms[__WEBPACK_IMPORTED_MODULE_4__constants__["e" /* roInlinePropertyDetails */]] = inlineDetails;
        return this.repLoader.retrieveFromLink(object.selfLink(), parms)
            .then(function (obj) {
            _this.currentObjects[paneId] = obj;
            var oid = obj.getOid(_this.keySeparator);
            _this.dirtyList.clearDirty(oid);
            return Promise.resolve(obj);
        });
    };
    ContextService.prototype.cacheList = function (list, index) {
        var entry = this.currentLists[index];
        if (entry) {
            entry.list = list;
            entry.added = Date.now();
        }
        else {
            if (__WEBPACK_IMPORTED_MODULE_6_lodash__["keys"](this.currentLists).length >= this.configService.config.listCacheSize) {
                //delete oldest;
                var oldest_1 = __WEBPACK_IMPORTED_MODULE_6_lodash__["first"](__WEBPACK_IMPORTED_MODULE_6_lodash__["sortBy"](this.currentLists, "e.added")).added;
                var oldestIndex = __WEBPACK_IMPORTED_MODULE_6_lodash__["findKey"](this.currentLists, function (e) { return e.added === oldest_1; });
                if (oldestIndex) {
                    delete this.currentLists[oldestIndex];
                }
            }
            this.currentLists[index] = { list: list, added: Date.now() };
        }
    };
    ContextService.prototype.getPagingParms = function (page, pageSize) {
        return (page && pageSize) ? { "x-ro-page": page, "x-ro-pageSize": pageSize } : {};
    };
    ContextService.prototype.incPendingPotentActionOrReload = function (paneId) {
        this.pendingPotentActionCount[paneId]++;
    };
    ContextService.prototype.decPendingPotentActionOrReload = function (paneId) {
        var count = --this.pendingPotentActionCount[paneId];
        if (count < 0) {
            // should never happen
            this.pendingPotentActionCount[paneId] = 0;
            this.loggerService.warn("ContextService:decPendingPotentActionOrReload count less than 0");
        }
    };
    ContextService.prototype.isPendingPotentActionOrReload = function (paneId) {
        return this.pendingPotentActionCount[paneId] > 0;
    };
    ContextService.prototype.setMessages = function (result) {
        var warnings = result.extensions().warnings() || [];
        var messages = result.extensions().messages() || [];
        this.warningsSource.next(warnings);
        this.messagesSource.next(messages);
    };
    ContextService.prototype.setCopyViewModel = function (dvm) {
        this.copiedViewModel = dvm;
        this.copiedViewModelSource.next(__WEBPACK_IMPORTED_MODULE_5__models__["L" /* withUndefined */](dvm));
    };
    ContextService.prototype.getCopyViewModel = function () {
        return this.copiedViewModel;
    };
    ContextService.prototype.setConcurrencyError = function (oid) {
        this.concurrencyErrorSource.next(oid);
    };
    ContextService.prototype.invokeActionInternal = function (invokeMap, action, fromPaneId, toPaneId, setDirty, gotoResult) {
        var _this = this;
        if (gotoResult === void 0) { gotoResult = false; }
        invokeMap.setUrlParameter(__WEBPACK_IMPORTED_MODULE_4__constants__["e" /* roInlinePropertyDetails */], false);
        if (action.extensions().returnType() === "list" && action.extensions().renderEagerly()) {
            invokeMap.setUrlParameter(__WEBPACK_IMPORTED_MODULE_4__constants__["h" /* roInlineCollectionItems */], true);
        }
        return this.repLoader.retrieve(invokeMap, __WEBPACK_IMPORTED_MODULE_5__models__["D" /* ActionResultRepresentation */], action.parent.etagDigest)
            .then(function (result) {
            setDirty();
            _this.setMessages(result);
            if (gotoResult) {
                _this.setResult(action, result, fromPaneId, toPaneId, 1, _this.configService.config.defaultPageSize);
            }
            return result;
        });
    };
    ContextService.prototype.getSetDirtyFunction = function (action, parms) {
        var _this = this;
        var parent = action.parent;
        if (action.isNotQueryOnly()) {
            if (parent instanceof __WEBPACK_IMPORTED_MODULE_5__models__["o" /* DomainObjectRepresentation */]) {
                return function () { return _this.dirtyList.setDirty(parent.getOid(_this.keySeparator)); };
            }
            if (parent instanceof __WEBPACK_IMPORTED_MODULE_5__models__["M" /* CollectionRepresentation */]) {
                return function () {
                    var selfLink = parent.selfLink();
                    var oid = __WEBPACK_IMPORTED_MODULE_5__models__["d" /* ObjectIdWrapper */].fromLink(selfLink, _this.configService.config.keySeparator);
                    _this.dirtyList.setDirty(oid);
                };
            }
            if (parent instanceof __WEBPACK_IMPORTED_MODULE_5__models__["A" /* CollectionMember */]) {
                return function () { return _this.dirtyList.setDirty(parent.parent.getOid(_this.keySeparator)); };
            }
            if (parent instanceof __WEBPACK_IMPORTED_MODULE_5__models__["N" /* ListRepresentation */] && parms) {
                var ccaParm = __WEBPACK_IMPORTED_MODULE_6_lodash__["find"](action.parameters(), function (p) { return p.isCollectionContributed(); });
                var ccaId = ccaParm ? ccaParm.id() : null;
                var ccaValue = ccaId ? parms[ccaId] : null;
                // this should always be true 
                if (ccaValue && ccaValue.isList()) {
                    var links_1 = __WEBPACK_IMPORTED_MODULE_6_lodash__["chain"](ccaValue.list())
                        .filter(function (v) { return v.isReference(); })
                        .map(function (v) { return v.link(); })
                        .value();
                    return function () { return __WEBPACK_IMPORTED_MODULE_6_lodash__["forEach"](links_1, function (l) { return _this.dirtyList.setDirty(l.getOid(_this.keySeparator)); }); };
                }
            }
        }
        return function () { };
    };
    ContextService.prototype.setNewObject = function (updatedObject, paneId, viewSavedObject) {
        this.setObject(paneId, updatedObject);
        this.dirtyList.setDirty(updatedObject.getOid(this.configService.config.keySeparator), true);
        if (viewSavedObject) {
            this.urlManager.setObject(updatedObject, paneId);
        }
        else {
            this.urlManager.popUrlState(paneId);
        }
    };
    ContextService.prototype.cacheRecentlyViewed = function (obj) {
        this.recentcache.add(obj);
    };
    ContextService.prototype.logoff = function () {
        var _this = this;
        for (var pane = 1; pane <= 2; pane++) {
            delete this.currentObjects[pane];
        }
        this.currentServices = null;
        this.currentMenus = null;
        this.currentVersion = null;
        this.currentUser = null;
        this.transientCache.clear();
        this.recentcache.clear();
        this.dirtyList.clear();
        // k will always be defined 
        __WEBPACK_IMPORTED_MODULE_6_lodash__["forEach"](this.currentMenuList, function (v, k) { return delete _this.currentMenuList[k]; });
        __WEBPACK_IMPORTED_MODULE_6_lodash__["forEach"](this.currentLists, function (v, k) { return delete _this.currentLists[k]; });
    };
    ContextService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_3__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__rep_loader_service__["a" /* RepLoaderService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__rep_loader_service__["a" /* RepLoaderService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_8__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__config_service__["a" /* ConfigService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_9__logger_service__["a" /* LoggerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_9__logger_service__["a" /* LoggerService */]) === 'function' && _d) || Object])
    ], ContextService);
    return ContextService;
    var _a, _b, _c, _d;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/context.service.js.map

/***/ }),

/***/ 30:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_86", function() { return noResultMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_93", function() { return obscuredText; });
/* unused harmony export tooShort */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return tooLong; });
/* unused harmony export notAnInteger */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "e", function() { return notANumber; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "d", function() { return mandatory; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_27", function() { return optional; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_28", function() { return choices; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_88", function() { return pendingAutoComplete; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "c", function() { return noPatternMatch; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_80", function() { return closeActions; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_81", function() { return noActions; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_82", function() { return openActions; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_78", function() { return mandatoryFieldsPrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_79", function() { return invalidFieldsPrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_94", function() { return unknownFileTitle; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_83", function() { return unknownCollectionSize; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_84", function() { return emptyCollectionSize; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_91", function() { return noItemsFound; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_85", function() { return noItemsSelected; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_87", function() { return dropPrompt; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_89", function() { return autoCompletePrompt; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_92", function() { return concurrencyMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_103", function() { return loadingMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_95", function() { return submittedMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_99", function() { return clear; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_100", function() { return recentDisabledMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_101", function() { return recentMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_98", function() { return recentTitle; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_97", function() { return noUserMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return outOfRange; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_90", function() { return pageMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_102", function() { return logOffMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_96", function() { return submittedCount; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "m", function() { return welcomeMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_39", function() { return basicHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "I", function() { return actionCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "J", function() { return actionHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "U", function() { return backCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "V", function() { return backHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "W", function() { return cancelCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "X", function() { return cancelHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Y", function() { return clipboardCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_0", function() { return clipboardCopy; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_1", function() { return clipboardShow; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_2", function() { return clipboardGo; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_3", function() { return clipboardDiscard; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Z", function() { return clipboardHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_8", function() { return editCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_9", function() { return editHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_10", function() { return enterCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_11", function() { return enterHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_29", function() { return forwardCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_30", function() { return forwardHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_31", function() { return geminiCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_32", function() { return geminiHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_33", function() { return gotoCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_34", function() { return gotoHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_37", function() { return helpCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_38", function() { return helpHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_40", function() { return menuCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_41", function() { return menuHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_45", function() { return okCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_46", function() { return okHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_48", function() { return pageCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_50", function() { return pageFirst; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_51", function() { return pagePrevious; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_53", function() { return pageNext; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_55", function() { return pageLast; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_49", function() { return pageHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_58", function() { return reloadCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_59", function() { return reloadHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_60", function() { return rootCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_61", function() { return rootHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_62", function() { return saveCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_63", function() { return saveHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_64", function() { return selectionCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_65", function() { return selectionHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_66", function() { return showCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_67", function() { return showHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_73", function() { return whereCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_74", function() { return whereHelp; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_75", function() { return commandTooShort; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_76", function() { return noCommandMatch; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_77", function() { return commandsAvailable; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "w", function() { return noArguments; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "x", function() { return tooFewArguments; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "y", function() { return tooManyArguments; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "v", function() { return commandNotAvailable; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_72", function() { return startHigherEnd; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_71", function() { return highestItem; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "u", function() { return item; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "t", function() { return empty; });
/* unused harmony export numberOfItems */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "o", function() { return on; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "n", function() { return collection; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_70", function() { return modified; });
/* unused harmony export properties */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "s", function() { return modifiedProperties; });
/* unused harmony export page */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_69", function() { return noVisible; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_68", function() { return doesNotMatch; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_52", function() { return alreadyOnFirst; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_54", function() { return alreadyOnLast; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_56", function() { return pageArgumentWrong; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_57", function() { return pageNumberWrong; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "z", function() { return mayNotbeChainedMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_47", function() { return queryOnlyRider; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "A", function() { return noSuchCommand; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "B", function() { return missingArgument; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "C", function() { return wrongTypeArgument; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "D", function() { return isNotANumber; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "E", function() { return tooManyDashes; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "F", function() { return mustBeGreaterThanZero; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "G", function() { return pleaseCompleteOrCorrect; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "H", function() { return required; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "K", function() { return mustbeQuestionMark; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "L", function() { return noActionsAvailable; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "M", function() { return doesNotMatchActions; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "N", function() { return matchingActions; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "O", function() { return actionsMessage; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "P", function() { return actionPrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "R", function() { return disabledPrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Q", function() { return isDisabled; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "T", function() { return noDescription; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "S", function() { return descriptionPrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_4", function() { return clipboardError; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_5", function() { return clipboardContextError; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_6", function() { return clipboardContents; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_7", function() { return clipboardEmpty; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_12", function() { return doesNotMatchProperties; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_13", function() { return matchesMultiple; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_14", function() { return doesNotMatchDialog; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_15", function() { return multipleFieldMatches; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_16", function() { return isNotModifiable; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_17", function() { return invalidCase; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_18", function() { return invalidRefEntry; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_19", function() { return emptyClipboard; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_20", function() { return incompatibleClipboard; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_21", function() { return noMatch; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_22", function() { return multipleMatches; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_23", function() { return fieldName; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_24", function() { return descriptionFieldPrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_25", function() { return typePrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_26", function() { return unModifiablePrefix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_36", function() { return outOfItemRange; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_42", function() { return doesNotMatchMenu; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_43", function() { return matchingMenus; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "r", function() { return menuTitle; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_44", function() { return allMenus; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "_35", function() { return noRefFieldMatch; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "p", function() { return unsaved; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "q", function() { return editing; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "f", function() { return errorUnknown; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "g", function() { return errorExpiredTransient; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "h", function() { return errorWrongType; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "i", function() { return errorNotImplemented; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "j", function() { return errorSoftware; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "k", function() { return errorConnection; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "l", function() { return errorClient; });
// user message constants
var noResultMessage = "no result found";
var obscuredText = "*****";
var tooShort = "Too short";
var tooLong = "Too long";
var notAnInteger = "Not an integer";
var notANumber = "Not a number";
var mandatory = "Mandatory";
var optional = "Optional";
var choices = "Choices";
var pendingAutoComplete = "Pending auto-complete...";
var noPatternMatch = "Invalid entry";
var closeActions = "Close actions";
var noActions = "No actions available";
var openActions = "Open actions (Alt-a)";
var mandatoryFieldsPrefix = "Missing mandatory fields: ";
var invalidFieldsPrefix = "Invalid fields: ";
var unknownFileTitle = "UnknownFile";
var unknownCollectionSize = "";
var emptyCollectionSize = "Empty";
var noItemsFound = "No items found";
var noItemsSelected = "Must select items for collection contributed action";
var dropPrompt = "(drop here)";
var autoCompletePrompt = "(auto-complete or drop)";
var concurrencyMessage = "The object has been reloaded to pick up changes by another user. Please review, and re-enter any changes you still require.";
var loadingMessage = "Loading...";
var submittedMessage = "Submitted";
var clear = "Clear";
var recentDisabledMessage = "Nothing to clear";
var recentMessage = "Clear recent history";
var recentTitle = "Recently Viewed Objects";
var noUserMessage = "'No user set'";
var outOfRange = function (val, min, max, filter) {
    var minVal = filter ? filter.filter(min) : min;
    var maxVal = filter ? filter.filter(max) : max;
    return "Value is outside the range " + (minVal || "unlimited") + " to " + (maxVal || "unlimited");
};
var pageMessage = function (p, tp, c, tc) { return ("Page " + p + " of " + tp + "; viewing " + c + " of " + tc + " items"); };
var logOffMessage = function (u) { return ("Please confirm logoff of user: " + u); };
var submittedCount = function (c) { return (" with " + c + " lines submitted."); };
//Cicero commands and Help text
var welcomeMessage = "Welcome to Cicero. Type 'help' and the Enter key for more information.";
var basicHelp = "Cicero is a user interface purpose-designed to work with an audio screen-reader.\n" +
    "The display has only two fields: a read-only output field, and a single input field.\n" +
    "The input field always has the focus.\n" +
    "Commands are typed into the input field followed by the Enter key.\n" +
    "When the output field updates (either instantaneously or after the server has responded)\n" +
    "its contents are read out automatically, so \n" +
    "the user never has to navigate around the screen.\n" +
    "Commands, such as 'action', 'field' and 'save', may be typed in full\n" +
    "or abbreviated to the first two or more characters.\n" +
    "Commands are not case sensitive.\n" +
    "Some commands take one or more arguments.\n" +
    "There must be a space between the command word and the first argument,\n" +
    "and a comma between arguments.\n" +
    "Arguments may contain spaces if needed.\n" +
    "The commands available to the user vary according to the context.\n" +
    "The command 'help ?' (note that there is a space between help and '?')\n" +
    "will list the commands available to the user in the current context.\n" +
    "‘help’ followed by another command word (in full or abbreviated) will give more details about that command.\n" +
    "Some commands will change the context, for example using the Go command to navigate to an associated object, \n" +
    "in which case the new context will be read out.\n" +
    "Other commands - help being an example - do not change the context, but will read out information to the user.\n" +
    "If the user needs a reminder of the current context, the 'Where' command will read the context out again.\n" +
    "Hitting Enter on the empty input field has the same effect.\n" +
    "When the user enters a command and the output has been updated, the input field will  be cleared, \n" +
    "ready for the next command. The user may recall the previous command by hitting the up-arrow key.\n" +
    "The user might then edit or extend that previous command and hit Enter to run it again.\n" +
    "For advanced users: commands may be chained using a semi-colon between them,\n" +
    "however commands that do, or might, result in data updates cannot be chained.";
var actionCommand = "action";
var actionHelp = "Open the dialog for action from a menu, or from object actions.\n" +
    "A dialog is always opened for an action, even if it has no fields (parameters):\n" +
    "This is a safety mechanism, allowing the user to confirm that the action is the one intended.\n" +
    "Once the dialog fields have been completed, using the Enter command,\n" +
    "the action may then be invoked  with the OK command.\n" +
    "The action command takes two optional arguments.\n" +
    "The first is the name, or partial name, of the action.\n" +
    "If the partial name matches more than one action, a list of matches is returned but none opened.\n" +
    "If no argument is provided, a full list of available action names is returned.\n" +
    "The partial name may have more than one clause, separated by spaces.\n" +
    "these may match either parts of the action name or the sub-menu name if one exists.\n" +
    "If the action name matches a single action, then a question-mark may be added as a second\n" +
    "parameter, which will generate a more detailed description of the Action.";
var backCommand = "back";
var backHelp = "Move back to the previous context.";
var cancelCommand = "cancel";
var cancelHelp = "Leave the current activity (action dialog, or object edit), incomplete.";
var clipboardCommand = "clipboard";
var clipboardCopy = "copy";
var clipboardShow = "show";
var clipboardGo = "go";
var clipboardDiscard = "discard";
var clipboardHelp = "The clipboard command is used for temporarily\n" +
    "holding a reference to an object, so that it may be used later\n" +
    "to enter into a field.\n" +
    "Clipboard requires one argument, which may take one of four values:\n" +
    "copy, show, go, or discard\n" +
    "each of which may be abbreviated down to one character.\n" +
    "Copy copies a reference to the object being viewed into the clipboard,\n" +
    "overwriting any existing reference.\n" +
    "Show displays the content of the clipboard without using it.\n" +
    "Go takes you directly to the object held in the clipboard.\n" +
    "Discard removes any existing reference from the clipboard.\n" +
    "The reference held in the clipboard may be used within the Enter command.";
var editCommand = "edit";
var editHelp = "Put an object into Edit mode.";
var enterCommand = "enter";
var enterHelp = "Enter a value into a field,\n" +
    "meaning a parameter in an action dialog,\n" +
    "or  a property on an object being edited.\n" +
    "Enter requires 2 arguments.\n" +
    "The first argument is the partial field name, which must match a single field.\n" +
    "The second optional argument specifies the value, or selection, to be entered.\n" +
    "If a question mark is provided as the second argument, the field will not be\n" +
    "updated but further details will be provided about that input field.\n" +
    "If the word paste is used as the second argument, then, provided that the field is\n" +
    "a reference field, the object reference in the clipboard will be pasted into the field.\n";
var forwardCommand = "forward";
var forwardHelp = "Move forward to next context in the history\n" +
    "(if you have previously moved back).";
var geminiCommand = "gemini";
var geminiHelp = "Switch to the Gemini (graphical) user interface\n" +
    "preserving the current context.";
var gotoCommand = "goto";
var gotoHelp = "Go to the object referenced in a property,\n" +
    "or to a collection within an object,\n" +
    "or to an object within an open list or collection.\n" +
    "Goto takes one argument.  In the context of an object\n" +
    "that is the name or partial name of the property or collection.\n" +
    "In the context of an open list or collection, it is the\n" +
    "number of the item within the list or collection (starting at 1). ";
var helpCommand = "help";
var helpHelp = "If no argument is specified, help provides a basic explanation of how to use Cicero.\n" +
    "If help is followed by a question mark as an argument, this lists the commands available\n" +
    "in the current context. If help is followed by another command word as an argument\n" +
    "(or an abbreviation of it), a description of the specified Command is returned.";
var menuCommand = "menu";
var menuHelp = "Open a named main menu, from any context.\n" +
    "Menu takes one optional argument: the name, or partial name, of the menu.\n" +
    "If the partial name matches more than one menu, a list of matches is returned\n" +
    "but no menu is opened; if no argument is provided a list of all the menus\n" +
    "is returned.";
var okCommand = "ok";
var okHelp = "Invoke the action currently open as a dialog.\n" +
    "Fields in the dialog should be completed before this.";
var pageCommand = "page";
var pageFirst = "first";
var pagePrevious = "previous";
var pageNext = "next";
var pageLast = "last";
var pageHelp = "Supports paging of returned lists.\n" +
    "The page command takes a single argument, which may be one of these four words:\n" +
    "first, previous, next, or last, \n" +
    "which may be abbreviated down to the first character.\n" +
    "Alternative, the argument may be a specific page number.";
var reloadCommand = "reload";
var reloadHelp = "Not yet implemented. Reload the data from the server for an object or a list.\n" +
    "Note that for a list, which was generated by an action, reload runs the action again, \n" +
    "thus ensuring that the list is up to date. However, reloading a list does not reload the\n" +
    "individual objects in that list, which may still be cached. Invoking Reload on an\n" +
    "individual object, however, will ensure that its fields show the latest server data.";
var rootCommand = "root";
var rootHelp = "From within an opend collection context, the root command returns\n" +
    " to the root object that owns the collection. Does not take any arguments.\n";
var saveCommand = "save";
var saveHelp = "Save the updated fields on an object that is being edited,\n" +
    "and return from edit mode to a normal view of that object";
var selectionCommand = "selection";
var selectionHelp = "Not fully implemented. Select one or more items from a list,\n" +
    "prior to invoking an action on the selection.\n" +
    "Selection has one mandatory argument, which must be one of these words,\n" +
    "add, remove, all, clear, show.\n" +
    "The Add and Remove options must be followed by a second argument specifying\n" +
    "the item number, or range, to be added or removed.\n";
var showCommand = "show";
var showHelp = "In the context of an object, shows the name and content of\n" +
    "one or more of the properties.\n" +
    "May take 1 argument: the partial field name.\n" +
    "If this matches more than one property, a list of matches is returned.\n" +
    "If no argument is provided, the full list of properties is returned.\n" +
    "In the context of an opened object collection, or a list,\n" +
    "shows one or more items from that collection or list.\n" +
    "If no arguments are specified, show will list all of the the items in the collection,\n" +
    "or the first page of items if in a list view.\n" +
    "Alternatively, the command may be specified with an item number, or a range such as 3- 5.";
var whereCommand = "where";
var whereHelp = "Display a reminder of the current context.\n" +
    "The same can also be achieved by hitting the Return key on the empty input field.";
//Cicero feedback messages
var commandTooShort = "Command word must have at least 2 characters";
var noCommandMatch = function (a) { return ("No command begins with " + a); };
var commandsAvailable = "Commands available in current context:\n";
var noArguments = "No arguments provided";
var tooFewArguments = "Too few arguments provided";
var tooManyArguments = "Too many arguments provided";
var commandNotAvailable = function (c) { return ("The command: " + c + " is not available in the current context"); };
var startHigherEnd = "Starting item number cannot be greater than the ending item number";
var highestItem = function (n) { return ("The highest numbered item is " + n); };
var item = "item";
var empty = "empty";
var numberOfItems = function (n) { return (n + " items"); };
var on = "on";
var collection = "Collection";
var modified = "modified";
var properties = "properties";
var modifiedProperties = "Modified " + properties;
var page = "Page";
var noVisible = "No visible properties";
var doesNotMatch = function (name) { return (name + " does not match any properties"); };
var alreadyOnFirst = "List is already showing the first page";
var alreadyOnLast = "List is already showing the last page";
var pageArgumentWrong = "The argument must match: first, previous, next, last, or a single number";
var pageNumberWrong = function (max) { return ("Specified page number must be between 1 and " + max); };
var mayNotbeChainedMessage = function (c, r) { return (c + " command may not be chained" + r + ". Use Where command to see where execution stopped."); };
var queryOnlyRider = " unless the action is query-only";
var noSuchCommand = function (c) { return ("No such command: " + c); };
var missingArgument = function (i) { return ("Required argument number " + i + " is missing"); };
var wrongTypeArgument = function (i) { return ("Argument number " + i + " must be a number"); };
var isNotANumber = function (s) { return (s + " is not a number"); };
var tooManyDashes = "Cannot have more than one dash in argument";
var mustBeGreaterThanZero = "Item number or range values must be greater than zero";
var pleaseCompleteOrCorrect = "Please complete or correct these fields:\n";
var required = "required";
var mustbeQuestionMark = "Second argument may only be a question mark -  to get action details";
var noActionsAvailable = "No actions available";
var doesNotMatchActions = function (a) { return (a + " does not match any actions"); };
var matchingActions = "Matching actions:\n";
var actionsMessage = "Actions:\n";
var actionPrefix = "Action:";
var disabledPrefix = "disabled:";
var isDisabled = "is disabled.";
var noDescription = "No description provided";
var descriptionPrefix = "Description for action:";
var clipboardError = "Clipboard command may only be followed by copy, show, go, or discard";
var clipboardContextError = "Clipboard copy may only be used in the context of viewing an object";
var clipboardContents = function (contents) { return ("Clipboard contains: " + contents); };
var clipboardEmpty = "Clipboard is empty";
var doesNotMatchProperties = function (name) { return (name + " does not match any properties"); };
var matchesMultiple = "matches multiple fields:\n";
var doesNotMatchDialog = function (name) { return (name + " does not match any fields in the dialog"); };
var multipleFieldMatches = "Multiple fields match";
var isNotModifiable = "is not modifiable";
var invalidCase = "Invalid case";
var invalidRefEntry = "Invalid entry for a reference field. Use clipboard or clip";
var emptyClipboard = "Cannot use Clipboard as it is empty";
var incompatibleClipboard = "Contents of Clipboard are not compatible with the field";
var noMatch = function (s) { return ("None of the choices matches " + s); };
var multipleMatches = "Multiple matches:\n";
var fieldName = function (name) { return ("Field name: " + name); };
var descriptionFieldPrefix = "Description:";
var typePrefix = "Type:";
var unModifiablePrefix = function (reason) { return ("Unmodifiable: " + reason); };
var outOfItemRange = function (n) { return (n + " is out of range for displayed items"); };
var doesNotMatchMenu = function (name) { return (name + " does not match any menu"); };
var matchingMenus = "Matching menus:";
var menuTitle = function (title) { return (title + " menu"); };
var allMenus = "Menus:";
var noRefFieldMatch = function (s) { return (s + " does not match any reference fields or collections"); };
var unsaved = "Unsaved";
var editing = "Editing";
//Error messages
var errorUnknown = "Unknown software error";
var errorExpiredTransient = "The requested view of unsaved object details has expired";
var errorWrongType = "An unexpected type of result was returned";
var errorNotImplemented = "The requested software feature is not implemented";
var errorSoftware = "A software error occurred";
var errorConnection = "The client failed to connect to the server";
var errorClient = "Client Error";
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/user-messages.js.map

/***/ }),

/***/ 363:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__cicero_commands__ = __webpack_require__(585);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__mask_service__ = __webpack_require__(256);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__angular_common__ = __webpack_require__(24);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CiceroCommandFactoryService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};









var CiceroCommandFactoryService = (function () {
    function CiceroCommandFactoryService(urlManager, location, context, mask, error, configService) {
        var _this = this;
        this.urlManager = urlManager;
        this.location = location;
        this.context = context;
        this.mask = mask;
        this.error = error;
        this.configService = configService;
        this.commandsInitialised = false;
        this.commands = {
            "ac": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["b" /* Action */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "ba": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["c" /* Back */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "ca": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["d" /* Cancel */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "cl": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["e" /* Clipboard */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "ed": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["f" /* Edit */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "en": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["g" /* Enter */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "fo": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["h" /* Forward */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "ge": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["i" /* Gemini */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "go": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["j" /* Goto */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "he": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["k" /* Help */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "me": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["l" /* Menu */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "ok": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["m" /* OK */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "pa": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["n" /* Page */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "re": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["o" /* Reload */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "ro": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["p" /* Root */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "sa": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["q" /* Save */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "se": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["r" /* Selection */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "sh": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["s" /* Show */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
            "wh": new __WEBPACK_IMPORTED_MODULE_1__cicero_commands__["t" /* Where */](this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService),
        };
        this.processSingleCommand = function (input, cvm, chained) {
            try {
                input = input.trim();
                var firstWord = input.split(" ")[0].toLowerCase();
                var command = _this.getCommand(firstWord);
                var argString = null;
                var index = input.indexOf(" ");
                if (index >= 0) {
                    argString = input.substr(index + 1);
                }
                command.execute(argString, chained, cvm);
            }
            catch (e) {
                cvm.setOutputSource(e.message);
                cvm.input = "";
            }
        };
        //TODO: change the name & functionality to pre-parse or somesuch as could do more than auto
        //complete e.g. reject unrecognised action or one not available in context.
        this.autoComplete = function (input, cvm) {
            if (!input)
                return;
            var lastInChain = _.last(input.split(";")).toLowerCase();
            var charsTyped = lastInChain.length;
            lastInChain = lastInChain.trim();
            if (lastInChain.length === 0 || lastInChain.indexOf(" ") >= 0) {
                cvm.input += " ";
                return;
            }
            try {
                var command = _this.getCommand(lastInChain);
                var earlierChain = input.substr(0, input.length - charsTyped);
                cvm.input = earlierChain + command.fullCommand + " ";
            }
            catch (e) {
                cvm.setOutputSource(e.message);
            }
        };
        this.getCommand = function (commandWord) {
            if (commandWord.length < 2) {
                throw new Error(__WEBPACK_IMPORTED_MODULE_2__user_messages__["_75" /* commandTooShort */]);
            }
            var abbr = commandWord.substr(0, 2);
            var command = _this.commands[abbr];
            if (command == null) {
                throw new Error(__WEBPACK_IMPORTED_MODULE_2__user_messages__["_76" /* noCommandMatch */](abbr));
            }
            command.checkMatch(commandWord);
            return command;
        };
        this.allCommandsForCurrentContext = function () {
            var commandsInContext = _.filter(_this.commands, function (c) { return c.isAvailableInCurrentContext(); });
            return _.reduce(commandsInContext, function (r, c) { return r + c.fullCommand + "\n"; }, __WEBPACK_IMPORTED_MODULE_2__user_messages__["_77" /* commandsAvailable */]);
        };
    }
    CiceroCommandFactoryService.prototype.parseInput = function (input, cvm) {
        //TODO: sort out whether to process CVMs here, or in the execute method  -  is this ALWAYS called first?
        //Also, must not modify the *input* one here
        cvm.chainedCommands = null; //TODO: Maybe not needed if unexecuted commands are cleared down upon error?
        if (!input) {
            this.getCommand("wh").execute(null, false, cvm);
            return;
        }
        this.autoComplete(input, cvm);
        cvm.input = cvm.input.trim();
        cvm.previousInput = cvm.input;
        var commands = input.split(";");
        if (commands.length > 1) {
            var first = commands[0];
            commands.splice(0, 1);
            cvm.chainedCommands = commands;
            this.processSingleCommand(first, cvm, false);
        }
        else {
            this.processSingleCommand(input, cvm, false);
        }
    };
    ;
    CiceroCommandFactoryService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_8__angular_common__["a" /* Location */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__angular_common__["a" /* Location */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_5__mask_service__["a" /* MaskService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__mask_service__["a" /* MaskService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_6__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__error_service__["a" /* ErrorService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_7__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__config_service__["a" /* ConfigService */]) === 'function' && _f) || Object])
    ], CiceroCommandFactoryService);
    return CiceroCommandFactoryService;
    var _a, _b, _c, _d, _e, _f;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/cicero-command-factory.service.js.map

/***/ }),

/***/ 364:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__edit_parameter_edit_parameter_component__ = __webpack_require__(591);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__view_models_dialog_view_model__ = __webpack_require__(257);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ParametersComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var ParametersComponent = (function () {
    function ParametersComponent(ref) {
        var _this = this;
        this.ref = ref;
        // todo use proper classes syntax ! 
        this.classes = function () { return ("parameter" + (_this.parent.isMultiLineDialogRow ? " multilinedialog" : "")); };
    }
    ParametersComponent.prototype.focus = function () {
        var parms = this.parmComponents;
        if (parms && parms.length > 0) {
            // until first element returns true
            return _.some(parms.toArray(), function (i) { return i.focus(); });
        }
        return false;
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_3__view_models_dialog_view_model__["a" /* DialogViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__view_models_dialog_view_model__["a" /* DialogViewModel */]) === 'function' && _a) || Object)
    ], ParametersComponent.prototype, "parent", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_forms__["f" /* FormGroup */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_forms__["f" /* FormGroup */]) === 'function' && _b) || Object)
    ], ParametersComponent.prototype, "form", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Array)
    ], ParametersComponent.prototype, "parameters", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])(__WEBPACK_IMPORTED_MODULE_2__edit_parameter_edit_parameter_component__["a" /* EditParameterComponent */]), 
        __metadata('design:type', (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _c) || Object)
    ], ParametersComponent.prototype, "parmComponents", void 0);
    ParametersComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-parameters',
            template: __webpack_require__(1364),
            styles: [__webpack_require__(1299)]
        }), 
        __metadata('design:paramtypes', [(typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ChangeDetectorRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ChangeDetectorRef"]) === 'function' && _d) || Object])
    ], ParametersComponent);
    return ParametersComponent;
    var _a, _b, _c, _d;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/parameters.component.js.map

/***/ }),

/***/ 365:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(103);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__rxjs_extensions__ = __webpack_require__(189);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_rxjs_Subject__ = __webpack_require__(20);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_rxjs_Subject___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_5_rxjs_Subject__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__simple_lru_cache__ = __webpack_require__(1029);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_angular2_jwt__ = __webpack_require__(370);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_angular2_jwt___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_8_angular2_jwt__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RepLoaderService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};









var RepLoaderService = (function () {
    function RepLoaderService(
        //private readonly http: Http,
        http, configService) {
        var _this = this;
        this.http = http;
        this.configService = configService;
        this.loadingCount = 0;
        this.loadingCountSource = new __WEBPACK_IMPORTED_MODULE_5_rxjs_Subject__["Subject"]();
        this.loadingCount$ = this.loadingCountSource.asObservable();
        // use our own LRU cache 
        this.cache = new __WEBPACK_IMPORTED_MODULE_7__simple_lru_cache__["a" /* SimpleLruCache */](this.configService.config.httpCacheDepth);
        this.populate = function (model, ignoreCache) {
            var response = model;
            var config = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["RequestOptions"]({
                withCredentials: true,
                url: model.getUrl(),
                method: model.method,
                body: model.getBody()
            });
            return _this.httpPopulate(config, !!ignoreCache, response);
        };
        this.retrieve = function (map, rc, digest) {
            var response = new rc();
            var config = _this.setConfigFromMap(map, digest);
            return _this.httpPopulate(config, true, response);
        };
        this.validate = function (map, digest) {
            var config = _this.setConfigFromMap(map, digest);
            return _this.httpValidate(config);
        };
        this.retrieveFromLink = function (link, parms) {
            if (link) {
                var response = link.getTarget();
                var urlParms = "";
                if (parms) {
                    var urlParmString = __WEBPACK_IMPORTED_MODULE_4_lodash__["reduce"](parms, function (result, n, key) { return (result === "" ? "" : result + "&") + key + "=" + n; }, "");
                    urlParms = urlParmString !== "" ? "?" + urlParmString : "";
                }
                var config = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["RequestOptions"]({
                    method: link.method(),
                    url: link.href() + urlParms,
                    withCredentials: true
                });
                return _this.httpPopulate(config, true, response);
            }
            return Promise.reject("link must not be null");
        };
        this.invoke = function (action, parms, urlParms) {
            var invokeMap = action.getInvokeMap();
            if (invokeMap) {
                __WEBPACK_IMPORTED_MODULE_4_lodash__["each"](urlParms, function (v, k) { return invokeMap.setUrlParameter(k, v); });
                __WEBPACK_IMPORTED_MODULE_4_lodash__["each"](parms, function (v, k) { return invokeMap.setParameter(k, v); });
                return _this.retrieve(invokeMap, __WEBPACK_IMPORTED_MODULE_3__models__["D" /* ActionResultRepresentation */]);
            }
            return Promise.reject("attempting to invoke uninvokable action " + action.actionId());
        };
        this.clearCache = function (url) {
            _this.cache.remove(url);
        };
        this.addToCache = function (url, m) {
            _this.cache.add(url, m);
        };
        this.getFile = function (url, mt, ignoreCache) {
            if (ignoreCache) {
                // clear cache of existing values
                _this.cache.remove(url);
            }
            else {
                var blob = _this.cache.get(url);
                if (blob) {
                    return Promise.resolve(blob);
                }
            }
            var config = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["RequestOptions"]({
                method: "GET",
                url: url,
                responseType: __WEBPACK_IMPORTED_MODULE_1__angular_http__["ResponseContentType"].Blob,
                headers: new __WEBPACK_IMPORTED_MODULE_1__angular_http__["Headers"]({ "Accept": mt })
            });
            var request = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["Request"](config);
            return _this.http.request(request)
                .toPromise()
                .then(function (r) {
                var blob = r.blob();
                _this.cache.add(config.url, blob);
                return blob;
            })
                .catch(function (r) {
                r.url = r.url || config.url;
                return _this.handleError(r, config.url);
            });
        };
        this.uploadFile = function (url, mt, file) {
            var config = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["RequestOptions"]({
                method: "POST",
                url: url,
                body: file,
                headers: new __WEBPACK_IMPORTED_MODULE_1__angular_http__["Headers"]({ "Content-Type": mt })
            });
            var request = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["Request"](config);
            return _this.http.request(request)
                .toPromise()
                .then(function () {
                return Promise.resolve(true);
            })
                .catch(function () {
                return Promise.resolve(false);
            });
        };
    }
    RepLoaderService.prototype.addIfMatchHeader = function (config, digest) {
        if (digest && (config.method === __WEBPACK_IMPORTED_MODULE_1__angular_http__["RequestMethod"].Post || config.method === __WEBPACK_IMPORTED_MODULE_1__angular_http__["RequestMethod"].Put || config.method === __WEBPACK_IMPORTED_MODULE_1__angular_http__["RequestMethod"].Delete)) {
            config.headers = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["Headers"]({ "If-Match": digest });
        }
    };
    RepLoaderService.prototype.handleInvalidResponse = function (rc) {
        var rr = new __WEBPACK_IMPORTED_MODULE_3__models__["a" /* ErrorWrapper */](rc, __WEBPACK_IMPORTED_MODULE_3__models__["c" /* ClientErrorCode */].ConnectionProblem, "The response from the client was not parseable as a RestfulObject json Representation ");
        return Promise.reject(rr);
    };
    RepLoaderService.prototype.handleError = function (response, originalUrl) {
        var category;
        var error;
        if (response.status === __WEBPACK_IMPORTED_MODULE_3__models__["q" /* HttpStatusCode */].InternalServerError) {
            // this error should contain an error representatation 
            if (__WEBPACK_IMPORTED_MODULE_3__models__["E" /* isErrorRepresentation */](response.json())) {
                var errorRep = new __WEBPACK_IMPORTED_MODULE_3__models__["F" /* ErrorRepresentation */]();
                errorRep.populate(response.json());
                category = __WEBPACK_IMPORTED_MODULE_3__models__["b" /* ErrorCategory */].HttpServerError;
                error = errorRep;
            }
            else {
                return this.handleInvalidResponse(__WEBPACK_IMPORTED_MODULE_3__models__["b" /* ErrorCategory */].HttpServerError);
            }
        }
        else if (response.status <= 0) {
            // failed to connect
            category = __WEBPACK_IMPORTED_MODULE_3__models__["b" /* ErrorCategory */].ClientError;
            error = "Failed to connect to server: " + (response.url || "unknown");
        }
        else {
            category = __WEBPACK_IMPORTED_MODULE_3__models__["b" /* ErrorCategory */].HttpClientError;
            var message = response.headers.get("warning") || "Unknown client HTTP error";
            if (response.status === __WEBPACK_IMPORTED_MODULE_3__models__["q" /* HttpStatusCode */].BadRequest ||
                response.status === __WEBPACK_IMPORTED_MODULE_3__models__["q" /* HttpStatusCode */].UnprocessableEntity) {
                // these errors should contain a map          
                error = new __WEBPACK_IMPORTED_MODULE_3__models__["w" /* ErrorMap */](response.json(), response.status, message);
            }
            else if (response.status === __WEBPACK_IMPORTED_MODULE_3__models__["q" /* HttpStatusCode */].NotFound) {
                category = __WEBPACK_IMPORTED_MODULE_3__models__["b" /* ErrorCategory */].ClientError;
                error = "Failed to connect to server: " + (response.url || "unknown");
            }
            else {
                error = message;
            }
        }
        var rr = new __WEBPACK_IMPORTED_MODULE_3__models__["a" /* ErrorWrapper */](category, response.status, error, originalUrl);
        return Promise.reject(rr);
    };
    RepLoaderService.prototype.httpValidate = function (config) {
        var _this = this;
        this.loadingCountSource.next(++(this.loadingCount));
        return this.http.request(new __WEBPACK_IMPORTED_MODULE_1__angular_http__["Request"](config))
            .toPromise()
            .then(function () {
            _this.loadingCountSource.next(--(_this.loadingCount));
            return Promise.resolve(true);
        })
            .catch(function (r) {
            _this.loadingCountSource.next(--(_this.loadingCount));
            r.url = r.url || config.url;
            return _this.handleError(r, config.url);
        });
    };
    // special handler for case where we receive a redirected object back from server 
    // instead of an actionresult. Wrap the object in an actionresult and then handle normally
    RepLoaderService.prototype.handleRedirectedObject = function (response, data) {
        if (response instanceof __WEBPACK_IMPORTED_MODULE_3__models__["D" /* ActionResultRepresentation */] && __WEBPACK_IMPORTED_MODULE_3__models__["G" /* isIDomainObjectRepresentation */](data)) {
            var actionResult = {
                resultType: "object",
                result: data,
                links: [],
                extensions: {}
            };
            return actionResult;
        }
        return data;
    };
    RepLoaderService.prototype.isValidResponse = function (data) {
        return __WEBPACK_IMPORTED_MODULE_3__models__["H" /* isResourceRepresentation */](data);
    };
    RepLoaderService.prototype.logHeaders = function (headers) {
        var hNames = headers.keys();
        var hh = __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](hNames, function (hn) {
            var val = headers.get(hn);
            return hn + ":" + val;
        });
        var ss = __WEBPACK_IMPORTED_MODULE_4_lodash__["reduce"](hh, function (u, t) { return (u + "\n" + t); }, "");
        console.debug(ss);
    };
    RepLoaderService.prototype.httpPopulate = function (config, ignoreCache, response) {
        var _this = this;
        if (ignoreCache) {
            // clear cache of existing values
            this.cache.remove(config.url);
        }
        else {
            var cachedValue = this.cache.get(config.url);
            if (cachedValue) {
                response.populate(cachedValue);
                return Promise.resolve(response);
            }
        }
        this.loadingCountSource.next(++(this.loadingCount));
        return this.http.request(new __WEBPACK_IMPORTED_MODULE_1__angular_http__["Request"](config))
            .toPromise()
            .then(function (r) {
            _this.loadingCountSource.next(--(_this.loadingCount));
            _this.logHeaders(r.headers);
            var asJson = r.json();
            if (!_this.isValidResponse(asJson)) {
                return _this.handleInvalidResponse(__WEBPACK_IMPORTED_MODULE_3__models__["b" /* ErrorCategory */].ClientError);
            }
            var representation = _this.handleRedirectedObject(response, asJson);
            _this.cache.add(config.url, representation);
            response.populate(representation);
            response.etagDigest = r.headers.get("ETag");
            return Promise.resolve(response);
        })
            .catch(function (r) {
            _this.loadingCountSource.next(--(_this.loadingCount));
            r.url = r.url || config.url;
            return _this.handleError(r, config.url);
        });
    };
    RepLoaderService.prototype.setConfigFromMap = function (map, digest) {
        var config = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["RequestOptions"]({
            withCredentials: true,
            url: map.getUrl(),
            method: map.method,
            body: map.getBody()
        });
        this.addIfMatchHeader(config, digest);
        return config;
    };
    RepLoaderService.prototype.logoff = function () {
        this.cache.removeAll();
    };
    RepLoaderService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_8_angular2_jwt__["AuthHttp"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8_angular2_jwt__["AuthHttp"]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_6__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__config_service__["a" /* ConfigService */]) === 'function' && _b) || Object])
    ], RepLoaderService);
    return RepLoaderService;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/rep-loader.service.js.map

/***/ }),

/***/ 366:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__contributed_action_parent_view_model__ = __webpack_require__(603);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CollectionViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};




var CollectionViewModel = (function (_super) {
    __extends(CollectionViewModel, _super);
    function CollectionViewModel(viewModelFactory, colorService, error, context, urlManager, configService, loggerService, collectionRep, routeData, forceReload) {
        var _this = this;
        _super.call(this, context, viewModelFactory, urlManager, error, routeData.paneId);
        this.colorService = colorService;
        this.configService = configService;
        this.loggerService = loggerService;
        this.collectionRep = collectionRep;
        this.routeData = routeData;
        this.reset = function (routeData, resetting) {
            var state = routeData.collections[_this.collectionRep.collectionId()];
            // collections are always shown as summary on transient 
            if (routeData.interactionMode === __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Transient) {
                state = __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Summary;
            }
            function getDefaultTableState(exts) {
                if (exts.renderEagerly()) {
                    return exts.tableViewColumns() || exts.tableViewTitle() ? __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table : __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].List;
                }
                return __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Summary;
            }
            if (state == null) {
                state = getDefaultTableState(_this.collectionRep.extensions());
            }
            _this.editing = routeData.interactionMode === __WEBPACK_IMPORTED_MODULE_0__route_data__["a" /* InteractionMode */].Edit;
            if (resetting || state !== _this.currentState) {
                var size = _this.collectionRep.size();
                var itemLinks = _this.collectionRep.value();
                if (size > 0 || size == null) {
                    _this.mayHaveItems = true;
                }
                _this.details = __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__["b" /* getCollectionDetails */](size);
                var getDetails = itemLinks == null || state === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table;
                var actions = _this.collectionRep.actionMembers();
                _this.setActions(actions, routeData);
                if (state === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Summary) {
                    _this.items = [];
                }
                else if (getDetails) {
                    _this.context.getCollectionDetails(_this.collectionRep, state, resetting).
                        then(function (details) {
                        _this.items = _this.viewModelFactory.getItems(details.value(), state === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table, routeData, _this);
                        _this.details = __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__["b" /* getCollectionDetails */](_this.items.length);
                    }).
                        catch(function (reject) { return _this.error.handleError(reject); });
                }
                else {
                    _this.items = _this.viewModelFactory.getItems(itemLinks, _this.currentState === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table, routeData, _this);
                }
                _this.currentState = state;
            }
        };
        this.doSummary = function () { return _this.urlManager.setCollectionMemberState(_this.collectionRep.collectionId(), __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Summary, _this.onPaneId); };
        this.doList = function () { return _this.urlManager.setCollectionMemberState(_this.collectionRep.collectionId(), __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].List, _this.onPaneId); };
        this.doTable = function () { return _this.urlManager.setCollectionMemberState(_this.collectionRep.collectionId(), __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table, _this.onPaneId); };
        this.hasTableData = function () { return _this.items && __WEBPACK_IMPORTED_MODULE_3_lodash__["some"](_this.items, function (i) { return i.tableRowViewModel; }); };
        this.description = function () { return _this.details.toString(); };
        this.disableActions = function () { return _this.editing || !_this.actions || _this.actions.length === 0; };
        this.actionMember = function (id, keySeparator) {
            var actionViewModel = __WEBPACK_IMPORTED_MODULE_3_lodash__["find"](_this.actions, function (a) { return a.actionRep.actionId() === id; });
            if (actionViewModel) {
                return actionViewModel.actionRep;
            }
            _this.loggerService.throw("CollectionViewModel:actionMember no member " + id + " on " + _this.name);
        };
        this.hasMatchingLocallyContributedAction = function (id) {
            return id && _this.actions && _this.actions.length > 0 && _this.hasActionMember(id);
        };
        this.title = collectionRep.extensions().friendlyName();
        this.presentationHint = collectionRep.extensions().presentationHint();
        this.pluralName = collectionRep.extensions().pluralName();
        this.name = collectionRep.collectionId().toLowerCase();
        this.colorService.toColorNumberFromType(collectionRep.extensions().elementType()).
            then(function (c) { return _this.color = "" + _this.configService.config.linkColor + c; }).
            catch(function (reject) { return _this.error.handleError(reject); });
        this.reset(routeData, forceReload);
    }
    CollectionViewModel.prototype.hasActionMember = function (id) {
        return !!__WEBPACK_IMPORTED_MODULE_3_lodash__["find"](this.actions, function (a) { return a.actionRep.actionId() === id; });
    };
    return CollectionViewModel;
}(__WEBPACK_IMPORTED_MODULE_1__contributed_action_parent_view_model__["a" /* ContributedActionParentViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/collection-view-model.js.map

/***/ }),

/***/ 367:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__link_view_model__ = __webpack_require__(605);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ItemViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};

var ItemViewModel = (function (_super) {
    __extends(ItemViewModel, _super);
    function ItemViewModel(context, colorService, error, urlManager, configService, link, paneId, clickHandler, viewModelFactory, index, isSelected, id) {
        var _this = this;
        _super.call(this, context, colorService, error, urlManager, configService, link, paneId);
        this.clickHandler = clickHandler;
        this.viewModelFactory = viewModelFactory;
        this.index = index;
        this.isSelected = isSelected;
        this.id = id;
        this.selectionChange = function () {
            _this.urlManager.setItemSelected(_this.index, _this.selected, _this.id, _this.paneId);
        };
        this.doClick = function (right) {
            var currentPane = _this.clickHandler.pane(_this.paneId, right);
            _this.urlManager.setItem(_this.link, currentPane);
        };
        var members = link.members();
        if (members) {
            this.tableRowViewModel = this.viewModelFactory.tableRowViewModel(members, paneId, this.title);
        }
    }
    Object.defineProperty(ItemViewModel.prototype, "selected", {
        get: function () {
            return this.isSelected;
        },
        set: function (v) {
            this.isSelected = v;
            this.selectionChange();
        },
        enumerable: true,
        configurable: true
    });
    ItemViewModel.prototype.silentSelect = function (v) {
        this.isSelected = v;
    };
    return ItemViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__link_view_model__["a" /* LinkViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/item-view-model.js.map

/***/ }),

/***/ 368:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__field_view_model__ = __webpack_require__(259);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__choice_view_model__ = __webpack_require__(106);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ParameterViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};





var ParameterViewModel = (function (_super) {
    __extends(ParameterViewModel, _super);
    function ParameterViewModel(parameterRep, onPaneId, color, error, maskService, previousValue, viewModelFactory, context, configService) {
        var _this = this;
        _super.call(this, parameterRep, color, error, context, configService, onPaneId, parameterRep.isScalar(), parameterRep.id(), parameterRep.isCollectionContributed(), parameterRep.entryType());
        this.parameterRep = parameterRep;
        this.maskService = maskService;
        this.previousValue = previousValue;
        this.viewModelFactory = viewModelFactory;
        this.setAsRow = function (i) { return _this.paneArgId = "" + _this.argId + i; };
        this.dflt = parameterRep.default().toString();
        var fieldEntryType = this.entryType;
        if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].Choices || fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleChoices) {
            this.setupParameterChoices();
        }
        if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].AutoComplete) {
            this.setupParameterAutocomplete();
        }
        if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].FreeForm && this.type === "ref") {
            this.setupParameterFreeformReference();
        }
        if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].ConditionalChoices || fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleConditionalChoices) {
            this.setupParameterConditionalChoices();
        }
        if (fieldEntryType !== __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].FreeForm || this.isCollectionContributed) {
            this.setupParameterSelectedChoices();
        }
        else {
            this.setupParameterSelectedValue();
        }
        var remoteMask = parameterRep.extensions().mask();
        if (remoteMask && parameterRep.isScalar()) {
            var localFilter = this.maskService.toLocalFilter(remoteMask, parameterRep.extensions().format());
            this.localFilter = localFilter;
            // formatting also happens in in directive - at least for dates - value is now date in that case
            this.formattedValue = localFilter.filter(this.value.toString());
        }
        this.description = this.getRequiredIndicator() + this.description;
    }
    ParameterViewModel.prototype.setupParameterChoices = function () {
        this.setupChoices(this.parameterRep.choices());
    };
    ParameterViewModel.prototype.setupParameterAutocomplete = function () {
        var parmRep = this.parameterRep;
        this.setupAutocomplete(parmRep, function () { return {}; });
    };
    ParameterViewModel.prototype.setupParameterFreeformReference = function () {
        var parmRep = this.parameterRep;
        this.description = this.description || __WEBPACK_IMPORTED_MODULE_3__user_messages__["_87" /* dropPrompt */];
        var val = this.previousValue && !this.previousValue.isNull() ? this.previousValue : parmRep.default();
        if (!val.isNull() && val.isReference()) {
            var link = val.link();
            this.reference = link.href();
            this.selectedChoice = new __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */](val, this.id, link.title());
        }
    };
    ParameterViewModel.prototype.setupParameterConditionalChoices = function () {
        var parmRep = this.parameterRep;
        this.setupConditionalChoices(parmRep);
    };
    ParameterViewModel.prototype.setupParameterSelectedChoices = function () {
        var parmRep = this.parameterRep;
        var fieldEntryType = this.entryType;
        var parmViewModel = this;
        function setCurrentChoices(vals) {
            var list = vals.list();
            var choicesToSet = __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](list, function (val) { return new __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */](val, parmViewModel.id, val.link() ? val.link().title() : undefined); });
            if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleChoices) {
                parmViewModel.selectedMultiChoices = __WEBPACK_IMPORTED_MODULE_4_lodash__["filter"](parmViewModel.choices, function (c) { return __WEBPACK_IMPORTED_MODULE_4_lodash__["some"](choicesToSet, function (choiceToSet) { return c.valuesEqual(choiceToSet); }); });
            }
            else {
                parmViewModel.selectedMultiChoices = choicesToSet;
            }
        }
        function setCurrentChoice(val) {
            var choiceToSet = new __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */](val, parmViewModel.id, val.link() ? val.link().title() : undefined);
            if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].Choices) {
                var choices = parmViewModel.choices;
                parmViewModel.selectedChoice = __WEBPACK_IMPORTED_MODULE_4_lodash__["find"](choices, function (c) { return c.valuesEqual(choiceToSet); }) || null;
            }
            else {
                if (!parmViewModel.selectedChoice || parmViewModel.selectedChoice.getValue().toValueString() !== choiceToSet.getValue().toValueString()) {
                    parmViewModel.selectedChoice = choiceToSet;
                }
            }
        }
        parmViewModel.refresh = function (newValue) {
            if (newValue || parmViewModel.dflt) {
                var toSet = newValue || parmRep.default();
                if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleChoices || fieldEntryType === __WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleConditionalChoices ||
                    parmViewModel.isCollectionContributed) {
                    setCurrentChoices(toSet);
                }
                else {
                    setCurrentChoice(toSet);
                }
            }
        };
        parmViewModel.refresh(this.previousValue);
    };
    ParameterViewModel.prototype.toTriStateBoolean = function (valueToSet) {
        // looks stupid but note type checking
        if (valueToSet === true || valueToSet === "true") {
            return true;
        }
        if (valueToSet === false || valueToSet === "false") {
            return false;
        }
        return null;
    };
    ParameterViewModel.prototype.setupParameterSelectedValue = function () {
        var _this = this;
        var parmRep = this.parameterRep;
        var returnType = parmRep.extensions().returnType();
        this.refresh = function (newValue) {
            if (returnType === "boolean") {
                var valueToSet = (newValue ? newValue.toValueString() : null) || parmRep.default().scalar();
                var bValueToSet = _this.toTriStateBoolean(valueToSet);
                _this.value = bValueToSet;
            }
            else if (__WEBPACK_IMPORTED_MODULE_2__models__["m" /* isDateOrDateTime */](parmRep)) {
                var date = __WEBPACK_IMPORTED_MODULE_2__models__["t" /* toUtcDate */](newValue || new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](_this.dflt));
                _this.value = date ? __WEBPACK_IMPORTED_MODULE_2__models__["n" /* toDateString */](date) : "";
            }
            else if (__WEBPACK_IMPORTED_MODULE_2__models__["u" /* isTime */](parmRep)) {
                _this.value = __WEBPACK_IMPORTED_MODULE_2__models__["v" /* toTime */](newValue || new __WEBPACK_IMPORTED_MODULE_2__models__["j" /* Value */](_this.dflt));
            }
            else {
                _this.value = (newValue ? newValue.toString() : null) || _this.dflt || "";
            }
        };
        this.refresh(this.previousValue);
    };
    ParameterViewModel.prototype.update = function () {
        _super.prototype.update.call(this);
        switch (this.entryType) {
            case (__WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].FreeForm):
                if (this.type === "scalar") {
                    if (this.localFilter) {
                        this.formattedValue = this.value ? this.localFilter.filter(this.value) : "";
                    }
                    else {
                        this.formattedValue = this.value ? this.value.toString() : "";
                    }
                    break;
                }
            // fall through 
            case (__WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].AutoComplete):
            case (__WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].Choices):
            case (__WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].ConditionalChoices):
                this.formattedValue = this.selectedChoice ? this.selectedChoice.toString() : "";
                break;
            case (__WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleChoices):
            case (__WEBPACK_IMPORTED_MODULE_2__models__["h" /* EntryType */].MultipleConditionalChoices):
                var count = !this.selectedMultiChoices ? 0 : this.selectedMultiChoices.length;
                this.formattedValue = count + " selected";
                break;
            default:
                this.formattedValue = this.value ? this.value.toString() : "";
        }
    };
    return ParameterViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__field_view_model__["a" /* FieldViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/parameter-view-model.js.map

/***/ }),

/***/ 369:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__field_view_model__ = __webpack_require__(259);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__choice_view_model__ = __webpack_require__(106);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__helpers_view_models__ = __webpack_require__(41);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return PropertyViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};






var PropertyViewModel = (function (_super) {
    __extends(PropertyViewModel, _super);
    function PropertyViewModel(propertyRep, color, error, viewModelfactory, context, maskService, urlManager, clickHandler, configService, id, previousValue, onPaneId, parentValues) {
        var _this = this;
        _super.call(this, propertyRep, color, error, context, configService, onPaneId, propertyRep.isScalar(), id, propertyRep.isCollectionContributed(), propertyRep.entryType());
        this.propertyRep = propertyRep;
        this.viewModelfactory = viewModelfactory;
        this.maskService = maskService;
        this.urlManager = urlManager;
        this.clickHandler = clickHandler;
        this.previousValue = previousValue;
        this.draggableTitle = function () { return _this.formattedValue; };
        this.canDropOn = function (targetType) { return _this.context.isSubTypeOf(_this.returnType, targetType); };
        this.doClick = function (right) { return _this.urlManager.setProperty(_this.reference, _this.clickHandler.pane(_this.onPaneId, right)); };
        this.isDirty = function () { return !!_this.previousValue || _this.getValue().toValueString() !== _this.originalValue.toValueString(); };
        this.draggableType = propertyRep.extensions().returnType(); // todo fix extensions 
        this.isEditable = !propertyRep.disabledReason();
        this.attachment = this.viewModelfactory.attachmentViewModel(propertyRep, onPaneId);
        var fieldEntryType = this.entryType;
        if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_4__models__["h" /* EntryType */].AutoComplete) {
            this.setupPropertyAutocomplete(parentValues);
        }
        if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_4__models__["h" /* EntryType */].ConditionalChoices) {
            this.setupPropertyConditionalChoices();
        }
        if (propertyRep.isScalar()) {
            this.setupScalarPropertyValue();
        }
        else {
            // is reference
            this.setupReferencePropertyValue();
        }
        this.refresh(previousValue);
        if (!previousValue) {
            this.originalValue = this.getValue();
        }
        this.description = this.getRequiredIndicator() + this.description;
    }
    PropertyViewModel.prototype.getDigest = function (propertyRep) {
        var parent = propertyRep.parent;
        if (parent instanceof __WEBPACK_IMPORTED_MODULE_4__models__["o" /* DomainObjectRepresentation */]) {
            if (parent.isTransient()) {
                return parent.etagDigest;
            }
        }
        return null;
    };
    PropertyViewModel.prototype.setupPropertyAutocomplete = function (parentValues) {
        var propertyRep = this.propertyRep;
        this.setupAutocomplete(propertyRep, parentValues, this.getDigest(propertyRep));
    };
    PropertyViewModel.prototype.setupPropertyConditionalChoices = function () {
        var propertyRep = this.propertyRep;
        this.setupConditionalChoices(propertyRep, this.getDigest(propertyRep));
    };
    PropertyViewModel.prototype.callIfChanged = function (newValue, doRefresh) {
        var propertyRep = this.propertyRep;
        var value = newValue || propertyRep.value();
        if (this.currentValue == null || value.toValueString() !== this.currentValue.toValueString()) {
            doRefresh(value);
            this.currentValue = value;
        }
    };
    PropertyViewModel.prototype.setupChoice = function (newValue) {
        var propertyRep = this.propertyRep;
        if (this.entryType === __WEBPACK_IMPORTED_MODULE_4__models__["h" /* EntryType */].Choices) {
            var choices = propertyRep.choices();
            this.setupChoices(choices);
            if (this.optional) {
                var emptyChoice = new __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */](new __WEBPACK_IMPORTED_MODULE_4__models__["j" /* Value */](""), this.id);
                this.choices = __WEBPACK_IMPORTED_MODULE_2_lodash__["concat"]([emptyChoice], this.choices);
            }
            var currentChoice_1 = new __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */](newValue, this.id);
            this.selectedChoice = __WEBPACK_IMPORTED_MODULE_2_lodash__["find"](this.choices, function (c) { return c.valuesEqual(currentChoice_1); }) || null;
        }
        else if (!propertyRep.isScalar()) {
            this.selectedChoice = new __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */](newValue, this.id);
        }
    };
    PropertyViewModel.prototype.setupReference = function (value, rep) {
        if (value.isNull()) {
            this.reference = "";
            this.value = this.description;
            this.formattedValue = "";
            this.refType = "null";
        }
        else {
            this.reference = value.link().href();
            this.value = value.toString();
            this.formattedValue = value.toString();
            this.refType = rep.extensions().notNavigable() ? "notNavigable" : "navigable";
        }
        if (this.entryType === __WEBPACK_IMPORTED_MODULE_4__models__["h" /* EntryType */].FreeForm) {
            this.description = this.description || __WEBPACK_IMPORTED_MODULE_3__user_messages__["_87" /* dropPrompt */];
        }
    };
    PropertyViewModel.prototype.setupReferencePropertyValue = function () {
        var _this = this;
        var propertyRep = this.propertyRep;
        this.refresh = function (newValue) { return _this.callIfChanged(newValue, function (value) {
            _this.setupChoice(value);
            _this.setupReference(value, propertyRep);
        }); };
    };
    PropertyViewModel.prototype.setupScalarPropertyValue = function () {
        var _this = this;
        var propertyRep = this.propertyRep;
        var remoteMask = propertyRep.extensions().mask();
        var localFilter = this.maskService.toLocalFilter(remoteMask, propertyRep.extensions().format());
        this.localFilter = localFilter;
        // formatting also happens in in directive - at least for dates - value is now date in that case
        this.refresh = function (newValue) { return _this.callIfChanged(newValue, function (value) {
            _this.setupChoice(value);
            __WEBPACK_IMPORTED_MODULE_5__helpers_view_models__["k" /* setScalarValueInView */](_this, _this.propertyRep, value);
            if (propertyRep.entryType() === __WEBPACK_IMPORTED_MODULE_4__models__["h" /* EntryType */].Choices) {
                if (_this.selectedChoice) {
                    _this.value = _this.selectedChoice.name;
                    _this.formattedValue = _this.selectedChoice.name;
                }
            }
            else if (_this.password) {
                _this.formattedValue = __WEBPACK_IMPORTED_MODULE_3__user_messages__["_93" /* obscuredText */];
            }
            else {
                _this.formattedValue = localFilter.filter(_this.value);
            }
        }); };
    };
    return PropertyViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__field_view_model__["a" /* FieldViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/property-view-model.js.map

/***/ }),

/***/ 37:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "c", function() { return ViewType; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return CollectionViewState; });
/* unused harmony export ApplicationMode */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return InteractionMode; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "d", function() { return RouteData; });
/* unused harmony export PaneRouteData */

var ViewType;
(function (ViewType) {
    ViewType[ViewType["Home"] = 0] = "Home";
    ViewType[ViewType["Object"] = 1] = "Object";
    ViewType[ViewType["List"] = 2] = "List";
    ViewType[ViewType["Error"] = 3] = "Error";
    ViewType[ViewType["Recent"] = 4] = "Recent";
    ViewType[ViewType["Attachment"] = 5] = "Attachment";
    ViewType[ViewType["ApplicationProperties"] = 6] = "ApplicationProperties";
    ViewType[ViewType["MultiLineDialog"] = 7] = "MultiLineDialog";
})(ViewType || (ViewType = {}));
var CollectionViewState;
(function (CollectionViewState) {
    CollectionViewState[CollectionViewState["Summary"] = 0] = "Summary";
    CollectionViewState[CollectionViewState["List"] = 1] = "List";
    CollectionViewState[CollectionViewState["Table"] = 2] = "Table";
})(CollectionViewState || (CollectionViewState = {}));
var ApplicationMode;
(function (ApplicationMode) {
    ApplicationMode[ApplicationMode["Gemini"] = 0] = "Gemini";
    ApplicationMode[ApplicationMode["Cicero"] = 1] = "Cicero";
})(ApplicationMode || (ApplicationMode = {}));
var InteractionMode;
(function (InteractionMode) {
    InteractionMode[InteractionMode["View"] = 0] = "View";
    InteractionMode[InteractionMode["Edit"] = 1] = "Edit";
    InteractionMode[InteractionMode["Transient"] = 2] = "Transient";
    InteractionMode[InteractionMode["Form"] = 3] = "Form";
    InteractionMode[InteractionMode["NotPersistent"] = 4] = "NotPersistent";
})(InteractionMode || (InteractionMode = {}));
var RouteData = (function () {
    function RouteData(configService, loggerService) {
        var _this = this;
        this.configService = configService;
        this.loggerService = loggerService;
        this.pane = function (pane) {
            if (pane === 1) {
                return _this.pane1;
            }
            if (pane === 2) {
                return _this.pane2;
            }
            _this.loggerService.throw('RouteData:pane ${pane} is not a valid pane index on RouteData');
        };
        this.pane1 = new PaneRouteData(1, configService.config.doUrlValidation, loggerService);
        this.pane2 = new PaneRouteData(2, configService.config.doUrlValidation, loggerService);
    }
    return RouteData;
}());
var PaneRouteData = (function () {
    function PaneRouteData(paneId, doUrlValidation, loggerService) {
        this.paneId = paneId;
        this.doUrlValidation = doUrlValidation;
        this.loggerService = loggerService;
        this.isNull = {
            condition: function (val) { return !val; },
            name: "is null"
        };
        this.isNotNull = {
            condition: function (val) { return val; },
            name: "is not null"
        };
        this.isLength0 = {
            condition: function (val) { return val && val.length === 0; },
            name: "is length 0"
        };
        this.isEmptyMap = {
            condition: function (val) { return __WEBPACK_IMPORTED_MODULE_0_lodash__["keys"](val).length === 0; },
            name: "is an empty map"
        };
    }
    PaneRouteData.prototype.isValid = function (name) {
        if (!this.hasOwnProperty(name)) {
            this.loggerService.throw("PaneRouteData:isValid " + name + " is not a valid property on PaneRouteData");
        }
    };
    PaneRouteData.prototype.assertMustBe = function (context, name, contextCondition, valueCondition) {
        // make sure context and name are valid
        this.isValid(context);
        this.isValid(name);
        if (contextCondition.condition(this[context])) {
            if (!valueCondition.condition(this[name])) {
                this.loggerService.throw("PaneRouteData:assertMustBe Expect that " + name + " " + valueCondition.name + " when " + context + " " + contextCondition.name + " within url \"" + this.validatingUrl + "\"");
            }
        }
    };
    PaneRouteData.prototype.assertMustBeEmptyOutsideContext = function (context, name) {
        this.assertMustBe(context, name, this.isNull, this.isEmptyMap);
    };
    PaneRouteData.prototype.assertMustBeNullOutsideContext = function (context, name) {
        this.assertMustBe(context, name, this.isNull, this.isNull);
    };
    PaneRouteData.prototype.assertMustBeNullInContext = function (context, name) {
        this.assertMustBe(context, name, this.isNotNull, this.isNull);
    };
    PaneRouteData.prototype.assertMustBeZeroLengthInContext = function (context, name) {
        this.assertMustBe(context, name, this.isNotNull, this.isLength0);
    };
    PaneRouteData.prototype.validate = function (url) {
        this.validatingUrl = url;
        if (this.doUrlValidation) {
            // Can add more conditions here 
            this.assertMustBeNullInContext("objectId", "menuId");
            this.assertMustBeNullInContext("menuId", "objectId");
        }
    };
    PaneRouteData.prototype.isEqual = function (other) {
        return __WEBPACK_IMPORTED_MODULE_0_lodash__["isEqual"](this, other);
    };
    return PaneRouteData;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/route-data.js.map

/***/ }),

/***/ 41:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__menu_item_view_model__ = __webpack_require__(1034);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__choice_view_model__ = __webpack_require__(106);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_lodash__);
/* harmony export (immutable) */ __webpack_exports__["n"] = createForm;
/* harmony export (immutable) */ __webpack_exports__["l"] = copy;
/* harmony export (immutable) */ __webpack_exports__["g"] = tooltip;
/* unused harmony export createSubmenuItems */
/* harmony export (immutable) */ __webpack_exports__["a"] = createMenuItems;
/* harmony export (immutable) */ __webpack_exports__["f"] = actionsTooltip;
/* harmony export (immutable) */ __webpack_exports__["b"] = getCollectionDetails;
/* harmony export (immutable) */ __webpack_exports__["c"] = drop;
/* harmony export (immutable) */ __webpack_exports__["d"] = validate;
/* harmony export (immutable) */ __webpack_exports__["k"] = setScalarValueInView;
/* harmony export (immutable) */ __webpack_exports__["e"] = createChoiceViewModels;
/* harmony export (immutable) */ __webpack_exports__["h"] = handleErrorResponse;
/* harmony export (immutable) */ __webpack_exports__["i"] = incrementPendingPotentAction;
/* harmony export (immutable) */ __webpack_exports__["j"] = decrementPendingPotentAction;
/* harmony export (immutable) */ __webpack_exports__["m"] = focus;





function createForm(dialog, formBuilder) {
    var pps = dialog.parameters;
    var parms = __WEBPACK_IMPORTED_MODULE_4_lodash__["zipObject"](__WEBPACK_IMPORTED_MODULE_4_lodash__["map"](pps, function (p) { return p.id; }), __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](pps, function (p) { return p; }));
    var controls = __WEBPACK_IMPORTED_MODULE_4_lodash__["mapValues"](parms, function (p) { return [p.getValueForControl(), function (a) { return p.validator(a); }]; });
    var form = formBuilder.group(controls);
    form.valueChanges.subscribe(function (data) {
        // cache parm values
        __WEBPACK_IMPORTED_MODULE_4_lodash__["forEach"](data, function (v, k) { return parms[k].setValueFromControl(v); });
        dialog.setParms();
    });
    return { form: form, dialog: dialog, parms: parms };
}
function copy(event, item, context) {
    var cKeyCode = 67;
    if (event && (event.keyCode === cKeyCode && event.ctrlKey)) {
        context.setCopyViewModel(item);
        event.preventDefault();
    }
}
function tooltip(onWhat, fields) {
    if (onWhat.clientValid()) {
        return "";
    }
    var missingMandatoryFields = __WEBPACK_IMPORTED_MODULE_4_lodash__["filter"](fields, function (p) { return !p.clientValid && !p.getMessage(); });
    if (missingMandatoryFields.length > 0) {
        return __WEBPACK_IMPORTED_MODULE_4_lodash__["reduce"](missingMandatoryFields, function (s, t) { return s + t.title + "; "; }, __WEBPACK_IMPORTED_MODULE_3__user_messages__["_78" /* mandatoryFieldsPrefix */]);
    }
    var invalidFields = __WEBPACK_IMPORTED_MODULE_4_lodash__["filter"](fields, function (p) { return !p.clientValid; });
    if (invalidFields.length > 0) {
        return __WEBPACK_IMPORTED_MODULE_4_lodash__["reduce"](invalidFields, function (s, t) { return s + t.title + "; "; }, __WEBPACK_IMPORTED_MODULE_3__user_messages__["_79" /* invalidFieldsPrefix */]);
    }
    return "";
}
function getMenuNameForLevel(menupath, level) {
    var menu = "";
    if (menupath && menupath.length > 0) {
        var menus = menupath.split("_");
        if (menus.length > level) {
            menu = menus[level];
        }
    }
    return menu || "";
}
function removeDuplicateMenuNames(menus) {
    return __WEBPACK_IMPORTED_MODULE_4_lodash__["uniqWith"](menus, function (m1, m2) {
        if (m1.name && m2.name) {
            return m1.name === m2.name;
        }
        return false;
    });
}
function createSubmenuItems(avms, menuSlot, level) {
    // if not root menu aggregate all actions with same name
    var menuActions;
    var menuItems;
    if (menuSlot.name) {
        var actions = __WEBPACK_IMPORTED_MODULE_4_lodash__["filter"](avms, function (a) { return getMenuNameForLevel(a.menuPath, level) === menuSlot.name && !getMenuNameForLevel(a.menuPath, level + 1); });
        menuActions = actions;
        //then collate submenus 
        var submenuActions_1 = __WEBPACK_IMPORTED_MODULE_4_lodash__["filter"](avms, function (a) { return getMenuNameForLevel(a.menuPath, level) === menuSlot.name && getMenuNameForLevel(a.menuPath, level + 1); });
        var menuSubSlots = __WEBPACK_IMPORTED_MODULE_4_lodash__["chain"](submenuActions_1)
            .map(function (a) { return ({ name: getMenuNameForLevel(a.menuPath, level + 1), action: a }); })
            .value();
        menuSubSlots = removeDuplicateMenuNames(menuSubSlots);
        menuItems = __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](menuSubSlots, function (slot) { return createSubmenuItems(submenuActions_1, slot, level + 1); });
    }
    else {
        menuActions = [menuSlot.action];
        menuItems = null;
    }
    return new __WEBPACK_IMPORTED_MODULE_0__menu_item_view_model__["a" /* MenuItemViewModel */](menuSlot.name, menuActions, menuItems);
}
function createMenuItems(avms) {
    // first create a top level menu for each action 
    // note at top level we leave 'un-menued' actions
    // use an anonymous object locally so we can construct objects with readonly properties  
    var menuSlots = __WEBPACK_IMPORTED_MODULE_4_lodash__["chain"](avms)
        .map(function (a) { return ({ name: getMenuNameForLevel(a.menuPath, 0), action: a }); })
        .value();
    // remove non unique submenus 
    menuSlots = removeDuplicateMenuNames(menuSlots);
    // update submenus with all actions under same submenu
    return __WEBPACK_IMPORTED_MODULE_4_lodash__["map"](menuSlots, function (slot) { return createSubmenuItems(avms, slot, 0); });
}
function actionsTooltip(onWhat, actionsOpen) {
    if (actionsOpen) {
        return __WEBPACK_IMPORTED_MODULE_3__user_messages__["_80" /* closeActions */];
    }
    return onWhat.disableActions() ? __WEBPACK_IMPORTED_MODULE_3__user_messages__["_81" /* noActions */] : __WEBPACK_IMPORTED_MODULE_3__user_messages__["_82" /* openActions */];
}
function getCollectionDetails(count) {
    if (count == null) {
        return __WEBPACK_IMPORTED_MODULE_3__user_messages__["_83" /* unknownCollectionSize */];
    }
    if (count === 0) {
        return __WEBPACK_IMPORTED_MODULE_3__user_messages__["_84" /* emptyCollectionSize */];
    }
    var postfix = count === 1 ? "Item" : "Items";
    return count + " " + postfix;
}
function drop(context, error, vm, newValue) {
    return context.isSubTypeOf(newValue.draggableType, vm.returnType).
        then(function (canDrop) {
        if (canDrop) {
            vm.setNewValue(newValue);
            return true;
        }
        return false;
    }).
        catch(function (reject) { return error.handleError(reject); });
}
;
function validate(rep, vm, modelValue, viewValue, mandatoryOnly) {
    var message = mandatoryOnly ? __WEBPACK_IMPORTED_MODULE_2__models__["r" /* validateMandatory */](rep, viewValue) : __WEBPACK_IMPORTED_MODULE_2__models__["s" /* validate */](rep, modelValue, viewValue, vm.localFilter);
    if (message !== __WEBPACK_IMPORTED_MODULE_3__user_messages__["d" /* mandatory */]) {
        vm.setMessage(message);
    }
    else {
        vm.resetMessage();
    }
    vm.clientValid = !message;
    return vm.clientValid;
}
;
function setScalarValueInView(vm, propertyRep, value) {
    if (__WEBPACK_IMPORTED_MODULE_2__models__["m" /* isDateOrDateTime */](propertyRep)) {
        var date = __WEBPACK_IMPORTED_MODULE_2__models__["t" /* toUtcDate */](value);
        vm.value = date ? __WEBPACK_IMPORTED_MODULE_2__models__["n" /* toDateString */](date) : "";
    }
    else if (__WEBPACK_IMPORTED_MODULE_2__models__["u" /* isTime */](propertyRep)) {
        vm.value = __WEBPACK_IMPORTED_MODULE_2__models__["v" /* toTime */](value);
    }
    else {
        vm.value = value.scalar();
    }
}
function createChoiceViewModels(id, searchTerm, choices) {
    return Promise.resolve(__WEBPACK_IMPORTED_MODULE_4_lodash__["map"](choices, function (v, k) { return new __WEBPACK_IMPORTED_MODULE_1__choice_view_model__["a" /* ChoiceViewModel */](v, id, k, searchTerm); }));
}
function handleErrorResponse(err, messageViewModel, valueViewModels) {
    var requiredFieldsMissing = false; // only show warning message if we have nothing else 
    var fieldValidationErrors = false;
    var contributedParameterErrorMsg = "";
    __WEBPACK_IMPORTED_MODULE_4_lodash__["each"](err.valuesMap(), function (errorValue, k) {
        var valueViewModel = __WEBPACK_IMPORTED_MODULE_4_lodash__["find"](valueViewModels, function (vvm) { return vvm.id === k; });
        if (valueViewModel) {
            var reason = errorValue.invalidReason;
            if (reason) {
                if (reason === "Mandatory") {
                    var r = "REQUIRED";
                    requiredFieldsMissing = true;
                    valueViewModel.description = valueViewModel.description.indexOf(r) === 0 ? valueViewModel.description : r + " " + valueViewModel.description;
                }
                else {
                    valueViewModel.setMessage(reason);
                    fieldValidationErrors = true;
                }
            }
        }
        else {
            // no matching parm for message - this can happen in contributed actions 
            // make the message a dialog level warning.                               
            contributedParameterErrorMsg = errorValue.invalidReason || "";
        }
    });
    var msg = contributedParameterErrorMsg || err.invalidReason() || "";
    if (requiredFieldsMissing)
        msg = msg + " Please complete REQUIRED fields. ";
    if (fieldValidationErrors)
        msg = msg + " See field validation message(s). ";
    if (!msg)
        msg = err.warningMessage;
    messageViewModel.setMessage(msg);
}
function incrementPendingPotentAction(context, invokableaction, paneId) {
    if (invokableaction.isPotent()) {
        context.incPendingPotentActionOrReload(paneId);
    }
}
function decrementPendingPotentAction(context, invokableaction, paneId) {
    if (invokableaction.isPotent()) {
        context.decPendingPotentActionOrReload(paneId);
    }
}
function focus(renderer, element) {
    setTimeout(function () { return renderer.invokeElementMethod(element.nativeElement, "focus"); });
    return true;
}
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/helpers-view-models.js.map

/***/ }),

/***/ 46:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__config_service__ = __webpack_require__(21);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ErrorService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var ErrorService = (function () {
    function ErrorService(urlManager, context, configService) {
        var _this = this;
        this.urlManager = urlManager;
        this.context = context;
        this.configService = configService;
        this.preProcessors = [];
        this.setErrorPreprocessor(function (reject) {
            if (reject.category === __WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].HttpClientError && reject.httpErrorCode === __WEBPACK_IMPORTED_MODULE_2__models__["q" /* HttpStatusCode */].PreconditionFailed) {
                if (reject.originalUrl) {
                    var oid = __WEBPACK_IMPORTED_MODULE_2__models__["d" /* ObjectIdWrapper */].fromHref(reject.originalUrl, configService.config.keySeparator);
                    _this.context.setConcurrencyError(oid);
                    reject.handled = true;
                }
            }
        });
    }
    ErrorService.prototype.handleHttpServerError = function (reject) {
        this.urlManager.setError(__WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].HttpServerError);
    };
    ErrorService.prototype.handleHttpClientError = function (reject, displayMessages) {
        switch (reject.httpErrorCode) {
            case (__WEBPACK_IMPORTED_MODULE_2__models__["q" /* HttpStatusCode */].UnprocessableEntity):
                displayMessages(reject.error);
                break;
            default:
                this.urlManager.setError(__WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].HttpClientError, reject.httpErrorCode);
        }
    };
    ErrorService.prototype.handleClientError = function (reject) {
        this.urlManager.setError(__WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].ClientError, reject.clientErrorCode);
    };
    ErrorService.prototype.handleError = function (reject) {
        this.handleErrorAndDisplayMessages(reject, function () { });
    };
    ;
    ErrorService.prototype.handleErrorAndDisplayMessages = function (reject, displayMessages) {
        this.preProcessors.forEach(function (p) { return p(reject); });
        if (reject.handled) {
            return;
        }
        reject.handled = true;
        this.context.setError(reject);
        switch (reject.category) {
            case (__WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].HttpServerError):
                this.handleHttpServerError(reject);
                break;
            case (__WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].HttpClientError):
                this.handleHttpClientError(reject, displayMessages);
                break;
            case (__WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].ClientError):
                this.handleClientError(reject);
                break;
        }
    };
    ;
    ErrorService.prototype.setErrorPreprocessor = function (handler) {
        this.preProcessors.push(handler);
    };
    ;
    ErrorService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_3__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_4__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__config_service__["a" /* ConfigService */]) === 'function' && _c) || Object])
    ], ErrorService);
    return ErrorService;
    var _a, _b, _c;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/error.service.js.map

/***/ }),

/***/ 583:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__auth_service__ = __webpack_require__(147);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ApplicationPropertiesComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var ApplicationPropertiesComponent = (function () {
    function ApplicationPropertiesComponent(viewModelFactory, authService) {
        this.viewModelFactory = viewModelFactory;
        this.authService = authService;
    }
    Object.defineProperty(ApplicationPropertiesComponent.prototype, "applicationName", {
        get: function () {
            return this.applicationProperties.applicationName;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ApplicationPropertiesComponent.prototype, "userName", {
        get: function () {
            return this.applicationProperties.user ? this.applicationProperties.user.userName : __WEBPACK_IMPORTED_MODULE_2__user_messages__["_97" /* noUserMessage */];
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ApplicationPropertiesComponent.prototype, "serverUrl", {
        get: function () {
            return this.applicationProperties.serverUrl;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ApplicationPropertiesComponent.prototype, "implVersion", {
        get: function () {
            return this.applicationProperties.serverVersion ? this.applicationProperties.serverVersion.implVersion : "";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ApplicationPropertiesComponent.prototype, "clientVersion", {
        get: function () {
            return this.applicationProperties.clientVersion;
        },
        enumerable: true,
        configurable: true
    });
    ApplicationPropertiesComponent.prototype.ngOnInit = function () {
        this.applicationProperties = this.viewModelFactory.applicationPropertiesViewModel();
    };
    ApplicationPropertiesComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-application-properties',
            template: __webpack_require__(1343),
            styles: [__webpack_require__(1278)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__auth_service__ !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__auth_service__["AuthService"]) === 'function' && _b) || Object])
    ], ApplicationPropertiesComponent);
    return ApplicationPropertiesComponent;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/application-properties.component.js.map

/***/ }),

/***/ 584:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__pane_pane__ = __webpack_require__(148);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__config_service__ = __webpack_require__(21);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AttachmentComponent; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};









var AttachmentComponent = (function (_super) {
    __extends(AttachmentComponent, _super);
    function AttachmentComponent(activatedRoute, urlManager, viewModelFactory, context, error, configService) {
        _super.call(this, activatedRoute, urlManager);
        this.viewModelFactory = viewModelFactory;
        this.context = context;
        this.error = error;
        this.configService = configService;
    }
    AttachmentComponent.prototype.setup = function (routeData) {
        var _this = this;
        var oid = __WEBPACK_IMPORTED_MODULE_7__models__["d" /* ObjectIdWrapper */].fromObjectId(routeData.objectId, this.configService.config.keySeparator);
        this.context.getObject(routeData.paneId, oid, routeData.interactionMode)
            .then(function (object) {
            var attachmentId = routeData.attachmentId;
            var attachment = object.propertyMember(attachmentId);
            if (attachment) {
                var avm = _this.viewModelFactory.attachmentViewModel(attachment, routeData.paneId);
                if (avm) {
                    _this.title = avm.title;
                    avm.setImage(_this);
                }
            }
        })
            .catch(function (reject) { return _this.error.handleError(reject); });
    };
    AttachmentComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-attachment',
            template: __webpack_require__(1345),
            styles: [__webpack_require__(1280)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_3__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__angular_router__["d" /* ActivatedRoute */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_5__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__error_service__["a" /* ErrorService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_8__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__config_service__["a" /* ConfigService */]) === 'function' && _f) || Object])
    ], AttachmentComponent);
    return AttachmentComponent;
    var _a, _b, _c, _d, _e, _f;
}(__WEBPACK_IMPORTED_MODULE_6__pane_pane__["a" /* PaneComponent */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/attachment.component.js.map

/***/ }),

/***/ 585:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__cicero_renderer_service__ = __webpack_require__(252);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__constants__ = __webpack_require__(188);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_moment__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_moment___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_5_moment__);
/* harmony export (immutable) */ __webpack_exports__["a"] = getParametersAndCurrentValue;
/* unused harmony export Command */
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return Action; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "c", function() { return Back; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "d", function() { return Cancel; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "e", function() { return Clipboard; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "f", function() { return Edit; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "g", function() { return Enter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "h", function() { return Forward; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "i", function() { return Gemini; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "j", function() { return Goto; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "k", function() { return Help; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "l", function() { return Menu; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "m", function() { return OK; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "n", function() { return Page; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "o", function() { return Reload; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "p", function() { return Root; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "q", function() { return Save; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "r", function() { return Selection; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "s", function() { return Show; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "t", function() { return Where; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};






function getParametersAndCurrentValue(action, context) {
    if (__WEBPACK_IMPORTED_MODULE_0__models__["i" /* isIInvokableAction */](action)) {
        var parms = action.parameters();
        var values_1 = context.getDialogCachedValues(action.actionId());
        return _.mapValues(parms, function (p) {
            var value = values_1[p.id()];
            if (value === undefined) {
                return p.default();
            }
            return value;
        });
    }
    return {};
}
var Command = (function () {
    function Command(urlManager, location, commandFactory, context, mask, error, configService) {
        this.urlManager = urlManager;
        this.location = location;
        this.commandFactory = commandFactory;
        this.context = context;
        this.mask = mask;
        this.error = error;
        this.configService = configService;
        this.keySeparator = configService.config.keySeparator;
    }
    Command.prototype.execute = function (argString, chained, cvm) {
        this.cvm = cvm;
        //TODO Create outgoing Vm and copy across values as needed
        if (!this.isAvailableInCurrentContext()) {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["v" /* commandNotAvailable */](this.fullCommand));
            return;
        }
        //TODO: This could be moved into a pre-parse method as it does not depend on context
        if (argString == null) {
            if (this.minArguments > 0) {
                this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["w" /* noArguments */]);
                return;
            }
        }
        else {
            var args = argString.split(",");
            if (args.length < this.minArguments) {
                this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["x" /* tooFewArguments */]);
                return;
            }
            else if (args.length > this.maxArguments) {
                this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["y" /* tooManyArguments */]);
                return;
            }
        }
        this.doExecute(argString, chained);
    };
    //Helper methods follow
    Command.prototype.clearInputAndSetMessage = function (text) {
        this.cvm.clearInput();
        this.cvm.message = text;
        //TODO this.$route.reload();
    };
    Command.prototype.mayNotBeChained = function (rider) {
        if (rider === void 0) { rider = ""; }
        this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["z" /* mayNotbeChainedMessage */](this.fullCommand, rider));
    };
    //TODO: Change this  -  must build up output before setting the outputSource, which will result in refresh
    Command.prototype.appendAsNewLineToOutput = function (text) {
        this.cvm.setOutputSource("/n" + text);
    };
    Command.prototype.checkMatch = function (matchText) {
        if (this.fullCommand.indexOf(matchText) !== 0) {
            throw new Error(__WEBPACK_IMPORTED_MODULE_1__user_messages__["A" /* noSuchCommand */](matchText));
        }
    };
    //argNo starts from 0.
    //If argument does not parse correctly, message will be passed to UI and command aborted.
    Command.prototype.argumentAsString = function (argString, argNo, optional, toLower) {
        if (optional === void 0) { optional = false; }
        if (toLower === void 0) { toLower = true; }
        if (!argString)
            return undefined;
        if (!optional && argString.split(",").length < argNo + 1) {
            throw new Error(__WEBPACK_IMPORTED_MODULE_1__user_messages__["x" /* tooFewArguments */]);
        }
        var args = argString.split(",");
        if (args.length < argNo + 1) {
            if (optional) {
                return undefined;
            }
            else {
                throw new Error(__WEBPACK_IMPORTED_MODULE_1__user_messages__["B" /* missingArgument */](argNo + 1));
            }
        }
        return toLower ? args[argNo].trim().toLowerCase() : args[argNo].trim(); // which may be "" if argString ends in a ','
    };
    //argNo starts from 0.
    Command.prototype.argumentAsNumber = function (args, argNo, optional) {
        if (optional === void 0) { optional = false; }
        var arg = this.argumentAsString(args, argNo, optional);
        if (!arg && optional)
            return null;
        var number = parseInt(arg);
        if (isNaN(number)) {
            throw new Error(__WEBPACK_IMPORTED_MODULE_1__user_messages__["C" /* wrongTypeArgument */](argNo + 1));
        }
        return number;
    };
    Command.prototype.parseInt = function (input) {
        if (!input) {
            return null;
        }
        var number = parseInt(input);
        if (isNaN(number)) {
            throw new Error(__WEBPACK_IMPORTED_MODULE_1__user_messages__["D" /* isNotANumber */](input));
        }
        return number;
    };
    //Parses '17, 3-5, -9, 6-' into two numbers 
    Command.prototype.parseRange = function (arg) {
        if (!arg) {
            arg = "1-";
        }
        var clauses = arg.split("-");
        var range = { start: null, end: null };
        switch (clauses.length) {
            case 1:
                range.start = this.parseInt(clauses[0]);
                range.end = range.start;
                break;
            case 2:
                range.start = this.parseInt(clauses[0]);
                range.end = this.parseInt(clauses[1]);
                break;
            default:
                throw new Error(__WEBPACK_IMPORTED_MODULE_1__user_messages__["E" /* tooManyDashes */]);
        }
        if ((range.start != null && range.start < 1) || (range.end != null && range.end < 1)) {
            throw new Error(__WEBPACK_IMPORTED_MODULE_1__user_messages__["F" /* mustBeGreaterThanZero */]);
        }
        return range;
    };
    Command.prototype.getContextDescription = function () {
        //todo
        return null;
    };
    Command.prototype.routeData = function () {
        return this.urlManager.getRouteData().pane1;
    };
    //Helpers delegating to RouteData
    Command.prototype.isHome = function () {
        return this.cvm.viewType === __WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].Home;
    };
    Command.prototype.isObject = function () {
        return this.cvm.viewType === __WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].Object;
    };
    Command.prototype.getObject = function () {
        var _this = this;
        var oid = __WEBPACK_IMPORTED_MODULE_0__models__["d" /* ObjectIdWrapper */].fromObjectId(this.routeData().objectId, this.keySeparator);
        //TODO: Consider view model & transient modes?
        return this.context.getObject(1, oid, this.routeData().interactionMode).then(function (obj) {
            if (_this.routeData().interactionMode === __WEBPACK_IMPORTED_MODULE_4__route_data__["a" /* InteractionMode */].Edit) {
                return _this.context.getObjectForEdit(1, obj);
            }
            else {
                return obj; //To wrap a known object as a promise
            }
        });
    };
    Command.prototype.isList = function () {
        return this.cvm.viewType === __WEBPACK_IMPORTED_MODULE_4__route_data__["c" /* ViewType */].List;
    };
    Command.prototype.getList = function () {
        var routeData = this.routeData();
        //TODO: Currently covers only the list-from-menu; need to cover list from object action
        return this.context.getListFromMenu(routeData, routeData.page, routeData.pageSize);
    };
    Command.prototype.isMenu = function () {
        return !!this.routeData().menuId;
    };
    Command.prototype.getMenu = function () {
        return this.context.getMenu(this.routeData().menuId);
    };
    Command.prototype.isDialog = function () {
        return !!this.routeData().dialogId;
    };
    Command.prototype.getActionForCurrentDialog = function () {
        var _this = this;
        var dialogId = this.routeData().dialogId;
        if (this.isObject()) {
            return this.getObject().then(function (obj) { return _this.context.getInvokableAction(obj.actionMember(dialogId, _this.keySeparator)); });
        }
        else if (this.isMenu()) {
            return this.getMenu().then(function (menu) { return _this.context.getInvokableAction(menu.actionMember(dialogId, _this.keySeparator)); }); //i.e. return a promise
        }
        return Promise.reject(new __WEBPACK_IMPORTED_MODULE_0__models__["a" /* ErrorWrapper */](__WEBPACK_IMPORTED_MODULE_0__models__["b" /* ErrorCategory */].ClientError, __WEBPACK_IMPORTED_MODULE_0__models__["c" /* ClientErrorCode */].NotImplemented, "List actions not implemented yet"));
    };
    //Tests that at least one collection is open (should only be one). 
    //TODO: assumes that closing collection removes it from routeData NOT sets it to Summary
    Command.prototype.isCollection = function () {
        return this.isObject() && _.some(this.routeData().collections);
    };
    Command.prototype.closeAnyOpenCollections = function () {
        var _this = this;
        var open = __WEBPACK_IMPORTED_MODULE_2__cicero_renderer_service__["b" /* openCollectionIds */](this.routeData());
        _.forEach(open, function (id) {
            _this.urlManager.setCollectionMemberState(id, __WEBPACK_IMPORTED_MODULE_4__route_data__["b" /* CollectionViewState */].Summary);
        });
    };
    Command.prototype.isTable = function () {
        return false; //TODO
    };
    Command.prototype.isEdit = function () {
        return this.routeData().interactionMode === __WEBPACK_IMPORTED_MODULE_4__route_data__["a" /* InteractionMode */].Edit;
    };
    Command.prototype.isForm = function () {
        return this.routeData().interactionMode === __WEBPACK_IMPORTED_MODULE_4__route_data__["a" /* InteractionMode */].Form;
    };
    Command.prototype.isTransient = function () {
        return this.routeData().interactionMode === __WEBPACK_IMPORTED_MODULE_4__route_data__["a" /* InteractionMode */].Transient;
    };
    Command.prototype.matchingProperties = function (obj, match) {
        var props = _.map(obj.propertyMembers(), function (prop) { return prop; });
        if (match) {
            props = this.matchFriendlyNameAndOrMenuPath(props, match);
        }
        return props;
    };
    Command.prototype.matchingCollections = function (obj, match) {
        var allColls = _.map(obj.collectionMembers(), function (action) { return action; });
        if (match) {
            return this.matchFriendlyNameAndOrMenuPath(allColls, match);
        }
        else {
            return allColls;
        }
    };
    Command.prototype.matchingParameters = function (action, match) {
        var params = _.map(action.parameters(), function (p) { return p; });
        if (match) {
            params = this.matchFriendlyNameAndOrMenuPath(params, match);
        }
        return params;
    };
    Command.prototype.matchFriendlyNameAndOrMenuPath = function (reps, match) {
        var clauses = match.split(" ");
        //An exact match has preference over any partial match
        var exactMatches = _.filter(reps, function (rep) {
            var path = rep.extensions().menuPath();
            var name = rep.extensions().friendlyName().toLowerCase();
            return match === name ||
                (!!path && match === path.toLowerCase() + " " + name) ||
                _.every(clauses, function (clause) { return name === clause || (!!path && path.toLowerCase() === clause); });
        });
        if (exactMatches.length > 0)
            return exactMatches;
        return _.filter(reps, function (rep) {
            var path = rep.extensions().menuPath();
            var name = rep.extensions().friendlyName().toLowerCase();
            return _.every(clauses, function (clause) { return name.indexOf(clause) >= 0 || (!!path && path.toLowerCase().indexOf(clause) >= 0); });
        });
    };
    Command.prototype.findMatchingChoicesForRef = function (choices, titleMatch) {
        return _.filter(choices, function (v) { return v.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0; });
    };
    Command.prototype.findMatchingChoicesForScalar = function (choices, titleMatch) {
        var labels = _.keys(choices);
        var matchingLabels = _.filter(labels, function (l) { return l.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0; });
        var result = new Array();
        switch (matchingLabels.length) {
            case 0:
                break; //leave result empty
            case 1:
                //Push the VALUE for the key
                //For simple scalars they are the same, but not for Enums
                result.push(choices[matchingLabels[0]]);
                break;
            default:
                //Push the matching KEYs, wrapped as (pseudo) Values for display in message to user
                //For simple scalars the values would also be OK, but not for Enums
                _.forEach(matchingLabels, function (label) { return result.push(new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](label)); });
                break;
        }
        return result;
    };
    Command.prototype.handleErrorResponse = function (err, getFriendlyName) {
        var _this = this;
        if (err.invalidReason()) {
            this.clearInputAndSetMessage(err.invalidReason());
            return;
        }
        var msg = __WEBPACK_IMPORTED_MODULE_1__user_messages__["G" /* pleaseCompleteOrCorrect */];
        _.each(err.valuesMap(), function (errorValue, fieldId) {
            msg += _this.fieldValidationMessage(errorValue, function () { return getFriendlyName(fieldId); });
        });
        this.clearInputAndSetMessage(msg);
    };
    Command.prototype.fieldValidationMessage = function (errorValue, fieldFriendlyName) {
        var msg = "";
        var reason = errorValue.invalidReason;
        var value = errorValue.value;
        if (reason) {
            msg += fieldFriendlyName() + ": ";
            if (reason === __WEBPACK_IMPORTED_MODULE_1__user_messages__["d" /* mandatory */]) {
                msg += __WEBPACK_IMPORTED_MODULE_1__user_messages__["H" /* required */];
            }
            else {
                msg += value + " " + reason;
            }
            msg += "\n";
        }
        return msg;
    };
    Command.prototype.valueForUrl = function (val, field) {
        var _this = this;
        if (val.isNull())
            return val;
        var fieldEntryType = field.entryType();
        if (fieldEntryType !== __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].FreeForm || field.isCollectionContributed()) {
            if (fieldEntryType === __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].MultipleChoices || field.isCollectionContributed()) {
                var valuesFromRouteData_1 = new Array();
                if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["k" /* Parameter */]) {
                    var rd = getParametersAndCurrentValue(field.parent, this.context)[field.id()];
                    if (rd)
                        valuesFromRouteData_1 = rd.list(); //TODO: what if only one?
                }
                else if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["l" /* PropertyMember */]) {
                    var obj = field.parent;
                    var props = this.context.getObjectCachedValues(obj.id(this.keySeparator));
                    var rd = props[field.id()];
                    if (rd)
                        valuesFromRouteData_1 = rd.list(); //TODO: what if only one?
                }
                var vals = [];
                if (val.isReference() || val.isScalar()) {
                    vals = new Array(val);
                }
                else if (val.isList()) {
                    vals = val.list();
                }
                _.forEach(vals, function (v) {
                    _this.addOrRemoveValue(valuesFromRouteData_1, v);
                });
                if (vals[0].isScalar()) {
                    var scalars = _.map(valuesFromRouteData_1, function (v) { return v.scalar(); });
                    return new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](scalars);
                }
                else {
                    var links = _.map(valuesFromRouteData_1, function (v) { return ({ href: v.link().href(), title: v.link().title() }); });
                    return new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](links);
                }
            }
            if (val.isScalar()) {
                return val;
            }
            // reference 
            return this.leanLink(val);
        }
        if (val.isScalar()) {
            if (val.isNull()) {
                return new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */]("");
            }
            return val;
        }
        if (val.isReference()) {
            return this.leanLink(val);
        }
        return null;
    };
    Command.prototype.leanLink = function (val) {
        return new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */]({ href: val.link().href(), title: val.link().title() });
    };
    Command.prototype.addOrRemoveValue = function (valuesFromRouteData, val) {
        var index;
        var valToAdd;
        if (val.isScalar()) {
            valToAdd = val;
            index = _.findIndex(valuesFromRouteData, function (v) { return v.scalar() === val.scalar(); });
        }
        else {
            valToAdd = this.leanLink(val);
            index = _.findIndex(valuesFromRouteData, function (v) { return v.link().href() === valToAdd.link().href(); });
        }
        if (index > -1) {
            valuesFromRouteData.splice(index, 1);
        }
        else {
            valuesFromRouteData.push(valToAdd);
        }
    };
    Command.prototype.setFieldValueInContextAndUrl = function (field, urlVal) {
        this.context.cacheFieldValue(this.routeData().dialogId, field.id(), urlVal);
        this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
    };
    Command.prototype.setPropertyValueinContextAndUrl = function (obj, property, urlVal) {
        this.context.cachePropertyValue(obj, property, urlVal);
        this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
    };
    return Command;
}());
var Action = (function (_super) {
    __extends(Action, _super);
    function Action() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["I" /* actionCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["J" /* actionHelp */];
        this.minArguments = 0;
        this.maxArguments = 2;
    }
    Action.prototype.isAvailableInCurrentContext = function () {
        return (this.isMenu() || this.isObject() || this.isForm()) && !this.isDialog() && !this.isEdit(); //TODO add list
    };
    Action.prototype.doExecute = function (args, chained) {
        var _this = this;
        var match = this.argumentAsString(args, 0);
        var details = this.argumentAsString(args, 1, true);
        if (details && details !== "?") {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["K" /* mustbeQuestionMark */]);
            return;
        }
        if (this.isObject()) {
            this.getObject()
                .then(function (obj) { return _this.processActions(match, obj.actionMembers(), details); })
                .catch(function (reject) { return _this.error.handleError(reject); });
        }
        else if (this.isMenu()) {
            this.getMenu()
                .then(function (menu) { return _this.processActions(match, menu.actionMembers(), details); })
                .catch(function (reject) { return _this.error.handleError(reject); });
        }
        //TODO: handle list - CCAs
    };
    Action.prototype.processActions = function (match, actionsMap, details) {
        var actions = _.map(actionsMap, function (action) { return action; });
        if (actions.length === 0) {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["L" /* noActionsAvailable */]);
            return;
        }
        if (match) {
            actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
        }
        switch (actions.length) {
            case 0:
                this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["M" /* doesNotMatchActions */](match));
                break;
            case 1:
                var action = actions[0];
                if (details) {
                    this.renderActionDetails(action);
                }
                else if (action.disabledReason()) {
                    this.disabledAction(action);
                }
                else {
                    this.openActionDialog(action);
                }
                break;
            default:
                var output = match ? __WEBPACK_IMPORTED_MODULE_1__user_messages__["N" /* matchingActions */] : __WEBPACK_IMPORTED_MODULE_1__user_messages__["O" /* actionsMessage */];
                output += this.listActions(actions);
                this.clearInputAndSetMessage(output);
        }
    };
    Action.prototype.disabledAction = function (action) {
        var output = __WEBPACK_IMPORTED_MODULE_1__user_messages__["P" /* actionPrefix */] + " " + action.extensions().friendlyName() + " " + __WEBPACK_IMPORTED_MODULE_1__user_messages__["Q" /* isDisabled */] + " " + action.disabledReason();
        this.clearInputAndSetMessage(output);
    };
    Action.prototype.listActions = function (actions) {
        return _.reduce(actions, function (s, t) {
            var menupath = t.extensions().menuPath() ? t.extensions().menuPath() + " - " : "";
            var disabled = t.disabledReason() ? " (" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["R" /* disabledPrefix */] + " " + t.disabledReason() + ")" : "";
            return s + menupath + t.extensions().friendlyName() + disabled + "\n";
        }, "");
    };
    Action.prototype.openActionDialog = function (action) {
        var _this = this;
        this.context.clearDialogCachedValues();
        this.urlManager.setDialog(action.actionId());
        this.context.getInvokableAction(action).
            then(function (invokable) { return _.forEach(invokable.parameters(), function (p) { return _this.setFieldValueInContextAndUrl(p, p.default()); }); }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    Action.prototype.renderActionDetails = function (action) {
        var s = __WEBPACK_IMPORTED_MODULE_1__user_messages__["S" /* descriptionPrefix */] + " " + action.extensions().friendlyName() + "\n" + (action.extensions().description() || __WEBPACK_IMPORTED_MODULE_1__user_messages__["T" /* noDescription */]);
        this.clearInputAndSetMessage(s);
    };
    return Action;
}(Command));
var Back = (function (_super) {
    __extends(Back, _super);
    function Back() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["U" /* backCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["V" /* backHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Back.prototype.isAvailableInCurrentContext = function () {
        return true;
    };
    Back.prototype.doExecute = function (args, chained) {
        this.location.back();
    };
    ;
    return Back;
}(Command));
var Cancel = (function (_super) {
    __extends(Cancel, _super);
    function Cancel() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["W" /* cancelCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["X" /* cancelHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Cancel.prototype.isAvailableInCurrentContext = function () {
        return this.isDialog() || this.isEdit();
    };
    Cancel.prototype.doExecute = function (args, chained) {
        if (this.isEdit()) {
            this.urlManager.setInteractionMode(__WEBPACK_IMPORTED_MODULE_4__route_data__["a" /* InteractionMode */].View);
        }
        if (this.isDialog()) {
            this.urlManager.closeDialogReplaceHistory(""); //TODO provide ID
        }
    };
    ;
    return Cancel;
}(Command));
var Clipboard = (function (_super) {
    __extends(Clipboard, _super);
    function Clipboard() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["Y" /* clipboardCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["Z" /* clipboardHelp */];
        this.minArguments = 1;
        this.maxArguments = 1;
    }
    Clipboard.prototype.isAvailableInCurrentContext = function () {
        return true;
    };
    Clipboard.prototype.doExecute = function (args, chained) {
        var sub = this.argumentAsString(args, 0);
        if (__WEBPACK_IMPORTED_MODULE_1__user_messages__["_0" /* clipboardCopy */].indexOf(sub) === 0) {
            this.copy();
        }
        else if (__WEBPACK_IMPORTED_MODULE_1__user_messages__["_1" /* clipboardShow */].indexOf(sub) === 0) {
            this.show();
        }
        else if (__WEBPACK_IMPORTED_MODULE_1__user_messages__["_2" /* clipboardGo */].indexOf(sub) === 0) {
            this.go();
        }
        else if (__WEBPACK_IMPORTED_MODULE_1__user_messages__["_3" /* clipboardDiscard */].indexOf(sub) === 0) {
            this.discard();
        }
        else {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_4" /* clipboardError */]);
        }
    };
    ;
    Clipboard.prototype.copy = function () {
        var _this = this;
        if (!this.isObject()) {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_5" /* clipboardContextError */]);
            return;
        }
        this.getObject().
            then(function (obj) {
            _this.cvm.clipboard = obj;
            _this.show();
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    Clipboard.prototype.show = function () {
        if (this.cvm.clipboard) {
            var label = __WEBPACK_IMPORTED_MODULE_0__models__["e" /* typePlusTitle */](this.cvm.clipboard);
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_6" /* clipboardContents */](label));
        }
        else {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_7" /* clipboardEmpty */]);
        }
    };
    Clipboard.prototype.go = function () {
        var link = this.cvm.clipboard.selfLink();
        if (link) {
            this.urlManager.setItem(link);
        }
        else {
            this.show();
        }
    };
    Clipboard.prototype.discard = function () {
        this.cvm.clipboard = null;
        this.show();
    };
    return Clipboard;
}(Command));
var Edit = (function (_super) {
    __extends(Edit, _super);
    function Edit() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_8" /* editCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_9" /* editHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Edit.prototype.isAvailableInCurrentContext = function () {
        return this.isObject() && !this.isEdit();
    };
    Edit.prototype.doExecute = function (args, chained) {
        if (chained) {
            this.mayNotBeChained();
            return;
        }
        this.context.clearObjectCachedValues();
        this.urlManager.setInteractionMode(__WEBPACK_IMPORTED_MODULE_4__route_data__["a" /* InteractionMode */].Edit);
    };
    ;
    return Edit;
}(Command));
var Enter = (function (_super) {
    __extends(Enter, _super);
    function Enter() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_10" /* enterCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_11" /* enterHelp */];
        this.minArguments = 2;
        this.maxArguments = 2;
    }
    Enter.prototype.isAvailableInCurrentContext = function () {
        return this.isDialog() || this.isEdit() || this.isTransient() || this.isForm();
    };
    Enter.prototype.doExecute = function (args, chained) {
        var fieldName = this.argumentAsString(args, 0);
        var fieldEntry = this.argumentAsString(args, 1, false, false);
        if (this.isDialog()) {
            this.fieldEntryForDialog(fieldName, fieldEntry);
        }
        else {
            this.fieldEntryForEdit(fieldName, fieldEntry);
        }
    };
    ;
    Enter.prototype.fieldEntryForEdit = function (fieldName, fieldEntry) {
        var _this = this;
        this.getObject().
            then(function (obj) {
            var fields = _this.matchingProperties(obj, fieldName);
            var s;
            switch (fields.length) {
                case 0:
                    s = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_12" /* doesNotMatchProperties */](fieldName);
                    break;
                case 1:
                    var field = fields[0];
                    if (fieldEntry === "?") {
                        //TODO: does this work in edit mode i.e. show entered value
                        s = _this.renderFieldDetails(field, field.value());
                    }
                    else {
                        _this.findAndClearAnyDependentFields(field.id(), obj.propertyMembers());
                        _this.setField(field, fieldEntry);
                        return;
                    }
                    break;
                default:
                    s = fieldName + " " + __WEBPACK_IMPORTED_MODULE_1__user_messages__["_13" /* matchesMultiple */];
                    s += _.reduce(fields, function (s, prop) { return s + prop.extensions().friendlyName() + "\n"; }, "");
            }
            _this.clearInputAndSetMessage(s);
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    Enter.prototype.findAndClearAnyDependentFields = function (changingField, allFields) {
        var _this = this;
        _.forEach(allFields, function (field) {
            var promptLink = field.promptLink();
            if (promptLink) {
                var pArgs = promptLink.arguments();
                var argNames = _.keys(pArgs);
                if (argNames.indexOf(changingField.toLowerCase()) >= 0) {
                    _this.clearField(field);
                }
            }
        });
    };
    Enter.prototype.fieldEntryForDialog = function (fieldName, fieldEntry) {
        var _this = this;
        this.getActionForCurrentDialog().
            then(function (action) {
            //TODO: error -  need to get invokable action to get the params.
            var params = _.map(action.parameters(), function (param) { return param; });
            params = _this.matchFriendlyNameAndOrMenuPath(params, fieldName);
            switch (params.length) {
                case 0:
                    _this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_14" /* doesNotMatchDialog */](fieldName));
                    break;
                case 1:
                    if (fieldEntry === "?") {
                        var p = params[0];
                        var value = getParametersAndCurrentValue(p.parent, _this.context)[p.id()];
                        var s = _this.renderFieldDetails(p, value);
                        _this.clearInputAndSetMessage(s);
                    }
                    else {
                        _this.findAndClearAnyDependentFields(fieldName, action.parameters());
                        _this.setField(params[0], fieldEntry);
                    }
                    break;
                default:
                    _this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_15" /* multipleFieldMatches */] + " " + fieldName); //TODO: list them
                    break;
            }
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    Enter.prototype.clearField = function (field) {
        this.context.cacheFieldValue(this.routeData().dialogId, field.id(), new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](null));
        if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["k" /* Parameter */]) {
            this.context.cacheFieldValue(this.routeData().dialogId, field.id(), new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](null));
        }
        else if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["l" /* PropertyMember */]) {
            var parent = field.parent;
            this.context.cachePropertyValue(parent, field, new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](null));
        }
    };
    Enter.prototype.setField = function (field, fieldEntry) {
        if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["l" /* PropertyMember */] && field.disabledReason()) {
            this.clearInputAndSetMessage(field.extensions().friendlyName() + " " + __WEBPACK_IMPORTED_MODULE_1__user_messages__["_16" /* isNotModifiable */]);
            return;
        }
        var entryType = field.entryType();
        switch (entryType) {
            case __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].FreeForm:
                this.handleFreeForm(field, fieldEntry);
                return;
            case __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].AutoComplete:
                this.handleAutoComplete(field, fieldEntry);
                return;
            case __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].Choices:
                this.handleChoices(field, fieldEntry);
                return;
            case __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].MultipleChoices:
                this.handleChoices(field, fieldEntry);
                return;
            case __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].ConditionalChoices:
                this.handleConditionalChoices(field, fieldEntry);
                return;
            case __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].MultipleConditionalChoices:
                this.handleConditionalChoices(field, fieldEntry);
                return;
            default:
                throw new Error(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_17" /* invalidCase */]);
        }
    };
    Enter.prototype.handleFreeForm = function (field, fieldEntry) {
        if (field.isScalar()) {
            var value = new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](fieldEntry);
            //TODO: handle a non-parsable date
            if (__WEBPACK_IMPORTED_MODULE_0__models__["m" /* isDateOrDateTime */](field)) {
                var m = __WEBPACK_IMPORTED_MODULE_5_moment__(fieldEntry, __WEBPACK_IMPORTED_MODULE_3__constants__["f" /* supportedDateFormats */], "en-GB", true); //TODO get actual locale
                if (m.isValid()) {
                    var dt = m.toDate();
                    value = new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](__WEBPACK_IMPORTED_MODULE_0__models__["n" /* toDateString */](dt));
                }
            }
            this.setFieldValue(field, value);
        }
        else {
            this.handleReferenceField(field, fieldEntry);
        }
    };
    Enter.prototype.setFieldValue = function (field, value) {
        var urlVal = this.valueForUrl(value, field);
        if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["k" /* Parameter */]) {
            this.setFieldValueInContextAndUrl(field, urlVal);
        }
        else if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["l" /* PropertyMember */]) {
            var parent = field.parent;
            if (parent instanceof __WEBPACK_IMPORTED_MODULE_0__models__["o" /* DomainObjectRepresentation */]) {
                this.setPropertyValueinContextAndUrl(parent, field, urlVal);
            }
        }
    };
    Enter.prototype.handleReferenceField = function (field, fieldEntry) {
        if (this.isPaste(fieldEntry)) {
            this.handleClipboard(field);
        }
        else {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_18" /* invalidRefEntry */]);
        }
    };
    Enter.prototype.isPaste = function (fieldEntry) {
        return "paste".indexOf(fieldEntry) === 0;
    };
    Enter.prototype.handleClipboard = function (field) {
        var _this = this;
        var ref = this.cvm.clipboard;
        if (!ref) {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_19" /* emptyClipboard */]);
            return;
        }
        var paramType = field.extensions().returnType();
        var refType = ref.domainType();
        this.context.isSubTypeOf(refType, paramType).
            then(function (isSubType) {
            if (isSubType) {
                var obj = _this.cvm.clipboard;
                var selfLink = obj.selfLink();
                //Need to add a title to the SelfLink as not there by default
                selfLink.setTitle(obj.title());
                var value = new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](selfLink);
                _this.setFieldValue(field, value);
            }
            else {
                _this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_20" /* incompatibleClipboard */]);
            }
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    Enter.prototype.handleAutoComplete = function (field, fieldEntry) {
        var _this = this;
        //TODO: Need to check that the minimum number of characters has been entered or fail validation
        if (!field.isScalar() && this.isPaste(fieldEntry)) {
            this.handleClipboard(field);
        }
        else {
            this.context.autoComplete(field, field.id(), null, fieldEntry).
                then(function (choices) {
                var matches = _this.findMatchingChoicesForRef(choices, fieldEntry);
                _this.switchOnMatches(field, fieldEntry, matches);
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        }
    };
    Enter.prototype.handleChoices = function (field, fieldEntry) {
        var matches;
        if (field.isScalar()) {
            matches = this.findMatchingChoicesForScalar(field.choices(), fieldEntry);
        }
        else {
            matches = this.findMatchingChoicesForRef(field.choices(), fieldEntry);
        }
        this.switchOnMatches(field, fieldEntry, matches);
    };
    Enter.prototype.switchOnMatches = function (field, fieldEntry, matches) {
        switch (matches.length) {
            case 0:
                this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_21" /* noMatch */](fieldEntry));
                break;
            case 1:
                this.setFieldValue(field, matches[0]);
                break;
            default:
                var msg_1 = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_22" /* multipleMatches */];
                _.forEach(matches, function (m) { return msg_1 += m.toString() + "\n"; });
                this.clearInputAndSetMessage(msg_1);
                break;
        }
    };
    Enter.prototype.getPropertiesAndCurrentValue = function (obj) {
        var props = obj.propertyMembers();
        var values = _.mapValues(props, function (p) { return p.value(); });
        var modifiedProps = this.context.getObjectCachedValues(obj.id(this.keySeparator));
        _.forEach(values, function (v, k) {
            var newValue = modifiedProps[k];
            if (newValue) {
                values[k] = newValue;
            }
        });
        return _.mapKeys(values, function (v, k) { return k.toLowerCase(); });
    };
    Enter.prototype.handleConditionalChoices = function (field, fieldEntry) {
        var _this = this;
        var enteredFields;
        if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["k" /* Parameter */]) {
            enteredFields = getParametersAndCurrentValue(field.parent, this.context);
        }
        if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["l" /* PropertyMember */]) {
            enteredFields = this.getPropertiesAndCurrentValue(field.parent);
        }
        var args = _.fromPairs(_.map(field.promptLink().arguments(), function (v, key) { return [key, new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](v.value)]; }));
        _.forEach(_.keys(args), function (key) { return args[key] = enteredFields[key]; });
        this.context.conditionalChoices(field, field.id(), null, args).
            then(function (choices) {
            var matches = _this.findMatchingChoicesForRef(choices, fieldEntry);
            _this.switchOnMatches(field, fieldEntry, matches);
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    Enter.prototype.renderFieldDetails = function (field, value) {
        var s = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_23" /* fieldName */](field.extensions().friendlyName());
        var desc = field.extensions().description();
        s += desc ? "\n" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["_24" /* descriptionFieldPrefix */] + " " + desc : "";
        s += "\n" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["_25" /* typePrefix */] + " " + __WEBPACK_IMPORTED_MODULE_0__models__["p" /* friendlyTypeName */](field.extensions().returnType());
        if (field instanceof __WEBPACK_IMPORTED_MODULE_0__models__["l" /* PropertyMember */] && field.disabledReason()) {
            s += "\n" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["_26" /* unModifiablePrefix */](field.disabledReason());
        }
        else {
            s += field.extensions().optional() ? "\n" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["_27" /* optional */] : "\n" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["d" /* mandatory */];
            if (field.choices()) {
                var label = "\n" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["_28" /* choices */] + ": ";
                s += _.reduce(field.choices(), function (s, cho) { return s + cho + " "; }, label);
            }
        }
        return s;
    };
    return Enter;
}(Command));
var Forward = (function (_super) {
    __extends(Forward, _super);
    function Forward() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_29" /* forwardCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_30" /* forwardHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Forward.prototype.isAvailableInCurrentContext = function () {
        return true;
    };
    Forward.prototype.doExecute = function (args, chained) {
        this.cvm.clearInput(); //To catch case where can't go any further forward and hence url does not change.
        this.location.forward();
    };
    ;
    return Forward;
}(Command));
var Gemini = (function (_super) {
    __extends(Gemini, _super);
    function Gemini() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_31" /* geminiCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_32" /* geminiHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Gemini.prototype.isAvailableInCurrentContext = function () {
        return true;
    };
    Gemini.prototype.doExecute = function (args, chained) {
        this.urlManager.gemini();
    };
    ;
    return Gemini;
}(Command));
var Goto = (function (_super) {
    __extends(Goto, _super);
    function Goto() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_33" /* gotoCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_34" /* gotoHelp */];
        this.minArguments = 1;
        this.maxArguments = 1;
    }
    Goto.prototype.isAvailableInCurrentContext = function () {
        return this.isObject() || this.isList();
    };
    Goto.prototype.doExecute = function (args, chained) {
        var _this = this;
        var arg0 = this.argumentAsString(args, 0);
        if (this.isList()) {
            var itemNo_1;
            try {
                itemNo_1 = this.parseInt(arg0);
            }
            catch (e) {
                this.clearInputAndSetMessage(e.message);
                return;
            }
            this.getList().then(function (list) {
                _this.attemptGotoLinkNumber(itemNo_1, list.value());
                return;
            });
            return;
        }
        if (this.isObject) {
            this.getObject().
                then(function (obj) {
                if (_this.isCollection()) {
                    var itemNo_2 = _this.argumentAsNumber(args, 0);
                    var openCollIds = __WEBPACK_IMPORTED_MODULE_2__cicero_renderer_service__["b" /* openCollectionIds */](_this.routeData());
                    var coll = obj.collectionMember(openCollIds[0]);
                    //Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                    _this.context.getCollectionDetails(coll, __WEBPACK_IMPORTED_MODULE_4__route_data__["b" /* CollectionViewState */].List, false).
                        then(function (details) { return _this.attemptGotoLinkNumber(itemNo_2, details.value()); }).
                        catch(function (reject) { return _this.error.handleError(reject); });
                }
                else {
                    var matchingProps = _this.matchingProperties(obj, arg0);
                    var matchingRefProps = _.filter(matchingProps, function (p) { return !p.isScalar(); });
                    var matchingColls = _this.matchingCollections(obj, arg0);
                    var s = "";
                    switch (matchingRefProps.length + matchingColls.length) {
                        case 0:
                            s = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_35" /* noRefFieldMatch */](arg0);
                            break;
                        case 1:
                            //TODO: Check for any empty reference
                            if (matchingRefProps.length > 0) {
                                var link = matchingRefProps[0].value().link();
                                _this.urlManager.setItem(link);
                            }
                            else {
                                _this.openCollection(matchingColls[0]);
                            }
                            break;
                        default:
                            var props = _.reduce(matchingRefProps, function (s, prop) {
                                return s + prop.extensions().friendlyName() + "\n";
                            }, "");
                            var colls = _.reduce(matchingColls, function (s, coll) {
                                return s + coll.extensions().friendlyName() + "\n";
                            }, "");
                            s = "Multiple matches for " + arg0 + ":\n" + props + colls;
                    }
                    _this.clearInputAndSetMessage(s);
                }
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        }
    };
    ;
    Goto.prototype.attemptGotoLinkNumber = function (itemNo, links) {
        if (itemNo < 1 || itemNo > links.length) {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_36" /* outOfItemRange */](itemNo));
        }
        else {
            var link = links[itemNo - 1]; // On UI, first item is '1'
            this.urlManager.setItem(link);
        }
    };
    Goto.prototype.openCollection = function (collection) {
        this.closeAnyOpenCollections();
        this.cvm.clearInput();
        this.urlManager.setCollectionMemberState(collection.collectionId(), __WEBPACK_IMPORTED_MODULE_4__route_data__["b" /* CollectionViewState */].List);
    };
    return Goto;
}(Command));
var Help = (function (_super) {
    __extends(Help, _super);
    function Help() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_37" /* helpCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_38" /* helpHelp */];
        this.minArguments = 0;
        this.maxArguments = 1;
    }
    Help.prototype.isAvailableInCurrentContext = function () {
        return true;
    };
    Help.prototype.doExecute = function (args, chained) {
        var arg = this.argumentAsString(args, 0);
        if (!arg) {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_39" /* basicHelp */]);
        }
        else if (arg === "?") {
            var commands = this.commandFactory.allCommandsForCurrentContext();
            this.clearInputAndSetMessage(commands);
        }
        else {
            try {
                var c = this.commandFactory.getCommand(arg);
                this.clearInputAndSetMessage(c.fullCommand + " command:\n" + c.helpText);
            }
            catch (e) {
                this.clearInputAndSetMessage(e.message);
            }
        }
    };
    ;
    return Help;
}(Command));
var Menu = (function (_super) {
    __extends(Menu, _super);
    function Menu() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_40" /* menuCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_41" /* menuHelp */];
        this.minArguments = 0;
        this.maxArguments = 1;
    }
    Menu.prototype.isAvailableInCurrentContext = function () {
        return true;
    };
    Menu.prototype.doExecute = function (args, chained) {
        var _this = this;
        var name = this.argumentAsString(args, 0);
        this.context.getMenus()
            .then(function (menus) {
            var links = menus.value();
            if (name) {
                //TODO: do multi-clause match
                var exactMatches = _.filter(links, function (t) { return t.title().toLowerCase() === name; });
                var partialMatches = _.filter(links, function (t) { return t.title().toLowerCase().indexOf(name) > -1; });
                links = exactMatches.length === 1 ? exactMatches : partialMatches;
            }
            switch (links.length) {
                case 0:
                    _this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_42" /* doesNotMatchMenu */](name));
                    break;
                case 1:
                    var menuId = links[0].rel().parms[0].value;
                    _this.urlManager.setHome();
                    _this.urlManager.clearUrlState(1);
                    _this.urlManager.setMenu(menuId);
                    break;
                default:
                    var label = name ? __WEBPACK_IMPORTED_MODULE_1__user_messages__["_43" /* matchingMenus */] + "\n" : __WEBPACK_IMPORTED_MODULE_1__user_messages__["_44" /* allMenus */] + "\n";
                    var s = _.reduce(links, function (s, t) { return s + t.title() + "\n"; }, label);
                    _this.clearInputAndSetMessage(s);
            }
        });
    };
    return Menu;
}(Command));
var OK = (function (_super) {
    __extends(OK, _super);
    function OK() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_45" /* okCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_46" /* okHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    OK.prototype.isAvailableInCurrentContext = function () {
        return this.isDialog();
    };
    OK.prototype.doExecute = function (args, chained) {
        var _this = this;
        this.getActionForCurrentDialog().
            then(function (action) {
            if (chained && action.invokeLink().method() !== "GET") {
                _this.mayNotBeChained(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_47" /* queryOnlyRider */]);
                return;
            }
            var fieldMap;
            if (_this.isForm()) {
                var obj = action.parent;
                fieldMap = _this.context.getObjectCachedValues(obj.id(_this.keySeparator)); //Props passed in as pseudo-params to action
            }
            else {
                fieldMap = getParametersAndCurrentValue(action, _this.context);
            }
            _this.context.invokeAction(action, fieldMap).
                then(function (result) {
                // todo handle case where result is empty - this is no longer handled 
                // by reject below
                var warnings = result.extensions().warnings();
                if (warnings) {
                    _.forEach(warnings, function (w) { return _this.cvm.alert += "\nWarning: " + w; });
                }
                var messages = result.extensions().messages();
                if (messages) {
                    _.forEach(messages, function (m) { return _this.cvm.alert += "\n" + m; });
                }
                _this.urlManager.closeDialogReplaceHistory(""); //TODO provide Id
            }).
                catch(function (reject) {
                var display = function (em) {
                    var paramFriendlyName = function (paramId) { return __WEBPACK_IMPORTED_MODULE_0__models__["f" /* friendlyNameForParam */](action, paramId); };
                    _this.handleErrorResponse(em, paramFriendlyName);
                };
                _this.error.handleErrorAndDisplayMessages(reject, display);
            });
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    return OK;
}(Command));
var Page = (function (_super) {
    __extends(Page, _super);
    function Page() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_48" /* pageCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_49" /* pageHelp */];
        this.minArguments = 1;
        this.maxArguments = 1;
    }
    Page.prototype.isAvailableInCurrentContext = function () {
        return this.isList();
    };
    Page.prototype.doExecute = function (args, chained) {
        var _this = this;
        var arg = this.argumentAsString(args, 0);
        this.getList().
            then(function (listRep) {
            var numPages = listRep.pagination().numPages;
            var page = _this.routeData().page;
            var pageSize = _this.routeData().pageSize;
            if (__WEBPACK_IMPORTED_MODULE_1__user_messages__["_50" /* pageFirst */].indexOf(arg) === 0) {
                _this.setPage(1);
                return;
            }
            else if (__WEBPACK_IMPORTED_MODULE_1__user_messages__["_51" /* pagePrevious */].indexOf(arg) === 0) {
                if (page === 1) {
                    _this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_52" /* alreadyOnFirst */]);
                }
                else {
                    _this.setPage(page - 1);
                }
            }
            else if (__WEBPACK_IMPORTED_MODULE_1__user_messages__["_53" /* pageNext */].indexOf(arg) === 0) {
                if (page === numPages) {
                    _this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_54" /* alreadyOnLast */]);
                }
                else {
                    _this.setPage(page + 1);
                }
            }
            else if (__WEBPACK_IMPORTED_MODULE_1__user_messages__["_55" /* pageLast */].indexOf(arg) === 0) {
                _this.setPage(numPages);
            }
            else {
                var number = parseInt(arg);
                if (isNaN(number)) {
                    _this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_56" /* pageArgumentWrong */]);
                    return;
                }
                if (number < 1 || number > numPages) {
                    _this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_57" /* pageNumberWrong */](numPages));
                    return;
                }
                _this.setPage(number);
            }
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    Page.prototype.setPage = function (page) {
        var pageSize = this.routeData().pageSize;
        this.urlManager.setListPaging(page, pageSize, __WEBPACK_IMPORTED_MODULE_4__route_data__["b" /* CollectionViewState */].List);
    };
    return Page;
}(Command));
var Reload = (function (_super) {
    __extends(Reload, _super);
    function Reload() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_58" /* reloadCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_59" /* reloadHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Reload.prototype.isAvailableInCurrentContext = function () {
        return this.isObject() || this.isList();
    };
    Reload.prototype.doExecute = function (args, chained) {
        this.clearInputAndSetMessage("Reload command is not yet implemented");
    };
    ;
    return Reload;
}(Command));
var Root = (function (_super) {
    __extends(Root, _super);
    function Root() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_60" /* rootCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_61" /* rootHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Root.prototype.isAvailableInCurrentContext = function () {
        return this.isCollection();
    };
    Root.prototype.doExecute = function (args, chained) {
        this.closeAnyOpenCollections();
    };
    ;
    return Root;
}(Command));
var Save = (function (_super) {
    __extends(Save, _super);
    function Save() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_62" /* saveCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_63" /* saveHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Save.prototype.isAvailableInCurrentContext = function () {
        return this.isEdit() || this.isTransient();
    };
    Save.prototype.doExecute = function (args, chained) {
        var _this = this;
        if (chained) {
            this.mayNotBeChained();
            return;
        }
        this.getObject().
            then(function (obj) {
            var props = obj.propertyMembers();
            var newValsFromUrl = _this.context.getObjectCachedValues(obj.id(_this.keySeparator));
            var propIds = new Array();
            var values = new Array();
            _.forEach(props, function (propMember, propId) {
                if (!propMember.disabledReason()) {
                    propIds.push(propId);
                    var newVal = newValsFromUrl[propId];
                    if (newVal) {
                        values.push(newVal);
                    }
                    else if (propMember.value().isNull() &&
                        propMember.isScalar()) {
                        values.push(new __WEBPACK_IMPORTED_MODULE_0__models__["j" /* Value */](""));
                    }
                    else {
                        values.push(propMember.value());
                    }
                }
            });
            var propMap = _.zipObject(propIds, values);
            var mode = obj.extensions().interactionMode();
            var toSave = mode === "form" || mode === "transient";
            var saveOrUpdate = toSave ? _this.context.saveObject : _this.context.updateObject;
            saveOrUpdate(obj, propMap, 1, true).
                catch(function (reject) {
                var display = function (em) { return _this.handleError(em, obj); };
                _this.error.handleErrorAndDisplayMessages(reject, display);
            });
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    ;
    Save.prototype.handleError = function (err, obj) {
        if (err.containsError()) {
            var propFriendlyName = function (propId) { return __WEBPACK_IMPORTED_MODULE_0__models__["g" /* friendlyNameForProperty */](obj, propId); };
            this.handleErrorResponse(err, propFriendlyName);
        }
        else {
            this.urlManager.setInteractionMode(__WEBPACK_IMPORTED_MODULE_4__route_data__["a" /* InteractionMode */].View);
        }
    };
    return Save;
}(Command));
var Selection = (function (_super) {
    __extends(Selection, _super);
    function Selection() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_64" /* selectionCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_65" /* selectionHelp */];
        this.minArguments = 1;
        this.maxArguments = 1;
    }
    Selection.prototype.isAvailableInCurrentContext = function () {
        return this.isList();
    };
    Selection.prototype.doExecute = function (args, chained) {
        var _this = this;
        //TODO: Add in sub-commands: Add, Remove, All, Clear & Show
        var arg = this.argumentAsString(args, 0);
        var _a = this.parseRange(arg), start = _a.start, end = _a.end; //'destructuring'
        this.getList().
            then(function (list) { return _this.selectItems(list, start, end); }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    ;
    Selection.prototype.selectItems = function (list, startNo, endNo) {
        var itemNo;
        for (itemNo = startNo; itemNo <= endNo; itemNo++) {
            this.urlManager.setItemSelected(itemNo - 1, true, "");
        }
    };
    return Selection;
}(Command));
var Show = (function (_super) {
    __extends(Show, _super);
    function Show() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_66" /* showCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_67" /* showHelp */];
        this.minArguments = 0;
        this.maxArguments = 1;
    }
    Show.prototype.isAvailableInCurrentContext = function () {
        return this.isObject() || this.isCollection() || this.isList();
    };
    Show.prototype.doExecute = function (args, chained) {
        var _this = this;
        if (this.isCollection()) {
            var arg = this.argumentAsString(args, 0, true);
            var _a = this.parseRange(arg), start_1 = _a.start, end_1 = _a.end;
            this.getObject().
                then(function (obj) {
                var openCollIds = __WEBPACK_IMPORTED_MODULE_2__cicero_renderer_service__["b" /* openCollectionIds */](_this.routeData());
                var coll = obj.collectionMember(openCollIds[0]);
                _this.renderCollectionItems(coll, start_1, end_1);
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
            return;
        }
        else if (this.isList()) {
            var arg = this.argumentAsString(args, 0, true);
            var _b = this.parseRange(arg), start_2 = _b.start, end_2 = _b.end;
            this.getList().
                then(function (list) { return _this.renderItems(list, start_2, end_2); }).
                catch(function (reject) { return _this.error.handleError(reject); });
        }
        else if (this.isObject()) {
            var fieldName_1 = this.argumentAsString(args, 0);
            this.getObject().
                then(function (obj) {
                var props = _this.matchingProperties(obj, fieldName_1);
                var colls = _this.matchingCollections(obj, fieldName_1);
                //TODO -  include these
                var s;
                switch (props.length + colls.length) {
                    case 0:
                        s = fieldName_1 ? __WEBPACK_IMPORTED_MODULE_1__user_messages__["_68" /* doesNotMatch */](fieldName_1) : __WEBPACK_IMPORTED_MODULE_1__user_messages__["_69" /* noVisible */];
                        break;
                    case 1:
                        s = props.length > 0 ? _this.renderPropNameAndValue(props[0]) : __WEBPACK_IMPORTED_MODULE_2__cicero_renderer_service__["c" /* renderCollectionNameAndSize */](colls[0]);
                        break;
                    default:
                        s = _.reduce(props, function (s, prop) { return s + _this.renderPropNameAndValue(prop); }, "");
                        s += _.reduce(colls, function (s, coll) { return s + __WEBPACK_IMPORTED_MODULE_2__cicero_renderer_service__["c" /* renderCollectionNameAndSize */](coll); }, "");
                }
                _this.clearInputAndSetMessage(s);
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        }
    };
    ;
    Show.prototype.renderPropNameAndValue = function (pm) {
        var name = pm.extensions().friendlyName();
        var value;
        var parent = pm.parent;
        var props = this.context.getObjectCachedValues(parent.id(this.keySeparator));
        var modifiedValue = props[pm.id()];
        if (this.isEdit() && !pm.disabledReason() && modifiedValue) {
            value = __WEBPACK_IMPORTED_MODULE_2__cicero_renderer_service__["d" /* renderFieldValue */](pm, modifiedValue, this.mask) + (" (" + __WEBPACK_IMPORTED_MODULE_1__user_messages__["_70" /* modified */] + ")");
        }
        else {
            value = __WEBPACK_IMPORTED_MODULE_2__cicero_renderer_service__["d" /* renderFieldValue */](pm, pm.value(), this.mask);
        }
        return name + ": " + value + "\n";
    };
    Show.prototype.renderCollectionItems = function (coll, startNo, endNo) {
        var _this = this;
        if (coll.value()) {
            this.renderItems(coll, startNo, endNo);
        }
        else {
            this.context.getCollectionDetails(coll, __WEBPACK_IMPORTED_MODULE_4__route_data__["b" /* CollectionViewState */].List, false).
                then(function (details) { return _this.renderItems(details, startNo, endNo); }).
                catch(function (reject) { return _this.error.handleError(reject); });
        }
    };
    Show.prototype.renderItems = function (source, startNo, endNo) {
        //TODO: problem here is that unless collections are in-lined value will be null.
        var max = source.value().length;
        if (!startNo) {
            startNo = 1;
        }
        if (!endNo) {
            endNo = max;
        }
        if (startNo > max || endNo > max) {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_71" /* highestItem */](source.value().length));
            return;
        }
        if (startNo > endNo) {
            this.clearInputAndSetMessage(__WEBPACK_IMPORTED_MODULE_1__user_messages__["_72" /* startHigherEnd */]);
            return;
        }
        var output = "";
        var i;
        var links = source.value();
        for (i = startNo; i <= endNo; i++) {
            output += __WEBPACK_IMPORTED_MODULE_1__user_messages__["u" /* item */] + " " + i + ": " + links[i - 1].title() + "\n";
        }
        this.clearInputAndSetMessage(output);
    };
    return Show;
}(Command));
var Where = (function (_super) {
    __extends(Where, _super);
    function Where() {
        _super.apply(this, arguments);
        this.fullCommand = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_73" /* whereCommand */];
        this.helpText = __WEBPACK_IMPORTED_MODULE_1__user_messages__["_74" /* whereHelp */];
        this.minArguments = 0;
        this.maxArguments = 0;
    }
    Where.prototype.isAvailableInCurrentContext = function () {
        return true;
    };
    Where.prototype.doExecute = function (args, chained) {
        //this.$route.reload(); TODO
    };
    ;
    return Where;
}(Command));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/cicero-commands.js.map

/***/ }),

/***/ 586:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__cicero_command_factory_service__ = __webpack_require__(363);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__cicero_renderer_service__ = __webpack_require__(252);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__view_models_cicero_view_model__ = __webpack_require__(1032);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__url_manager_service__ = __webpack_require__(22);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CiceroComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var CiceroComponent = (function () {
    function CiceroComponent(commandFactory, renderer, urlManager) {
        var _this = this;
        this.commandFactory = commandFactory;
        this.renderer = renderer;
        this.urlManager = urlManager;
        this.alert = ""; //Alert is appended before the output
        this.selectPreviousInput = function () {
            _this.inputText = _this.previousInput;
        };
        this.clearInput = function () {
            _this.inputText = null;
        };
        this.cvm = new __WEBPACK_IMPORTED_MODULE_4__view_models_cicero_view_model__["a" /* CiceroViewModel */]();
    }
    CiceroComponent.prototype.ngOnInit = function () {
        //Set up subscriptions to observables on CiceroViewModel
        //TODO:  Message, and other props?
        var _this = this;
        if (!this.cvmSub) {
            this.cvmSub = this.cvm.output$.subscribe(function (op) {
                _this.outputText = op;
            });
        }
        if (!this.paneRouteDataSub) {
            this.paneRouteDataSub =
                this.urlManager.getPaneRouteDataObservable(1)
                    .subscribe(function (paneRouteData) {
                    if (!paneRouteData.isEqual(_this.lastPaneRouteData)) {
                        _this.lastPaneRouteData = paneRouteData;
                        switch (paneRouteData.location) {
                            case __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].Home: {
                                _this.renderer.renderHome(_this.cvm, paneRouteData);
                                break;
                            }
                            case __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].Object: {
                                _this.renderer.renderObject(_this.cvm, paneRouteData);
                                break;
                            }
                            case __WEBPACK_IMPORTED_MODULE_0__route_data__["c" /* ViewType */].List: {
                                _this.renderer.renderList(_this.cvm, paneRouteData);
                                break;
                            }
                            default: {
                                _this.renderer.renderError(_this.cvm);
                                break;
                            }
                        }
                    }
                });
        }
        ;
    };
    CiceroComponent.prototype.ngOnDestroy = function () {
        if (this.cvmSub) {
            this.cvmSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    };
    CiceroComponent.prototype.parseInput = function (input) {
        this.cvm.input = input;
        this.commandFactory.parseInput(input, this.cvm);
        //TODO: check this  -  why not writing straight to output?
        this.cvm.setOutputSource(this.cvm.message);
    };
    ;
    CiceroComponent.prototype.ifSpaceThenAutoComplete = function (input) {
        //TODO: recognise tab also?
        if (input.substring(input.length - 1) == " ") {
            input = input.substr(0, input.length - 2);
            this.commandFactory.autoComplete(input, this.cvm);
        }
    };
    ;
    CiceroComponent.prototype.executeNextChainedCommandIfAny = function () {
        if (this.chainedCommands && this.chainedCommands.length > 0) {
            var next = this.popNextCommand();
            this.commandFactory.processSingleCommand(next, this.cvm, true);
        }
    };
    ;
    CiceroComponent.prototype.popNextCommand = function () {
        if (this.chainedCommands) {
            var next = this.chainedCommands[0];
            this.chainedCommands.splice(0, 1);
            return next;
        }
        return null;
    };
    CiceroComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["Component"])({
            selector: 'nof-cicero',
            template: __webpack_require__(1346),
            styles: [__webpack_require__(1281)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__cicero_command_factory_service__["a" /* CiceroCommandFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__cicero_command_factory_service__["a" /* CiceroCommandFactoryService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_3__cicero_renderer_service__["a" /* CiceroRendererService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__cicero_renderer_service__["a" /* CiceroRendererService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_5__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _c) || Object])
    ], CiceroComponent);
    return CiceroComponent;
    var _a, _b, _c;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/cicero.component.js.map

/***/ }),

/***/ 587:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return CustomComponentConfigService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

// default implementation which does nothing
var CustomComponentConfigService = (function () {
    function CustomComponentConfigService() {
    }
    // Remember custom components need to be added to "entryComponents" in app.module.ts !
    CustomComponentConfigService.prototype.configureCustomObjects = function (custom) { };
    CustomComponentConfigService.prototype.configureCustomLists = function (custom) { };
    CustomComponentConfigService.prototype.configureCustomErrors = function (custom) { };
    CustomComponentConfigService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [])
    ], CustomComponentConfigService);
    return CustomComponentConfigService;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/custom-component-config.service.js.map

/***/ }),

/***/ 588:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__custom_component_service__ = __webpack_require__(255);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__logger_service__ = __webpack_require__(87);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__url_manager_service__ = __webpack_require__(22);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return DynamicErrorComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var DynamicErrorComponent = (function () {
    function DynamicErrorComponent(context, componentFactoryResolver, customComponentService, loggerService, urlManagerService) {
        this.context = context;
        this.componentFactoryResolver = componentFactoryResolver;
        this.customComponentService = customComponentService;
        this.loggerService = loggerService;
        this.urlManagerService = urlManagerService;
    }
    DynamicErrorComponent.prototype.ngOnInit = function () {
        var _this = this;
        var errorWrapper = this.context.getError();
        if (errorWrapper) {
            this.customComponentService.getCustomErrorComponent(errorWrapper.category, errorWrapper.httpErrorCode | errorWrapper.clientErrorCode).then(function (c) {
                var childComponent = _this.componentFactoryResolver.resolveComponentFactory(c);
                _this.parent.createComponent(childComponent);
            });
        }
        else {
            this.loggerService.warn("No error found returning to home page");
            this.urlManagerService.setHomeSinglePane();
        }
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChild"])('parent', { read: __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"] }), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"]) === 'function' && _a) || Object)
    ], DynamicErrorComponent.prototype, "parent", void 0);
    DynamicErrorComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-dynamic-error',
            template: __webpack_require__(1350),
            styles: [__webpack_require__(1285)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__context_service__["a" /* ContextService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ComponentFactoryResolver"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ComponentFactoryResolver"]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_1__custom_component_service__["a" /* CustomComponentService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__custom_component_service__["a" /* CustomComponentService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_3__logger_service__["a" /* LoggerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__logger_service__["a" /* LoggerService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _f) || Object])
    ], DynamicErrorComponent);
    return DynamicErrorComponent;
    var _a, _b, _c, _d, _e, _f;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/dynamic-error.component.js.map

/***/ }),

/***/ 589:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__custom_component_service__ = __webpack_require__(255);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__pane_pane__ = __webpack_require__(148);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__config_service__ = __webpack_require__(21);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return DynamicListComponent; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};










var DynamicListComponent = (function (_super) {
    __extends(DynamicListComponent, _super);
    function DynamicListComponent(activatedRoute, urlManager, context, error, componentFactoryResolver, customComponentService, configService) {
        var _this = this;
        _super.call(this, activatedRoute, urlManager);
        this.context = context;
        this.error = error;
        this.componentFactoryResolver = componentFactoryResolver;
        this.customComponentService = customComponentService;
        this.configService = configService;
        this.title = "";
        this.showPlaceholder = true;
        this.reloadPlaceholderButton = {
            value: "Reload",
            doClick: function () { return _this.reload(); },
            show: function () { return true; },
            disabled: function () { return null; },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
    }
    DynamicListComponent.prototype.getActionExtensions = function (routeData) {
        return routeData.objectId
            ? this.context.getActionExtensionsFromObject(routeData.paneId, __WEBPACK_IMPORTED_MODULE_8__models__["d" /* ObjectIdWrapper */].fromObjectId(routeData.objectId, this.configService.config.keySeparator), routeData.actionId)
            : this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
    };
    DynamicListComponent.prototype.reload = function () {
        var _this = this;
        var recreate = function () {
            return _this.cachedRouteData.objectId
                ? _this.context.getListFromObject(_this.cachedRouteData)
                : _this.context.getListFromMenu(_this.cachedRouteData);
        };
        recreate()
            .then(function () { return _this.setup(_this.cachedRouteData); })
            .catch(function (reject) {
            _this.error.handleError(reject);
        });
    };
    Object.defineProperty(DynamicListComponent.prototype, "actionHolders", {
        get: function () {
            return [this.reloadPlaceholderButton];
        },
        enumerable: true,
        configurable: true
    });
    DynamicListComponent.prototype.setup = function (routeData) {
        var _this = this;
        this.cachedRouteData = routeData;
        var cachedList = this.context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);
        if (cachedList) {
            this.showPlaceholder = false;
            var et = cachedList.extensions().elementType();
            if (et && et !== this.lastOid) {
                this.lastOid = et;
                this.parent.clear();
                this.customComponentService.getCustomComponent(et, __WEBPACK_IMPORTED_MODULE_3__route_data__["c" /* ViewType */].List).then(function (c) {
                    var childComponent = _this.componentFactoryResolver.resolveComponentFactory(c);
                    _this.parent.createComponent(childComponent);
                });
            }
        }
        else {
            this.showPlaceholder = true;
            this.parent.clear();
            this.lastOid = null; // so we recreate child after reload
            this.getActionExtensions(routeData)
                .then(function (ext) {
                return _this.title = ext.friendlyName();
            })
                .catch(function (reject) { return _this.error.handleError(reject); });
        }
    };
    DynamicListComponent.prototype.ngOnDestroy = function () {
        _super.prototype.ngOnDestroy.call(this);
        this.parent.clear();
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChild"])('parent', { read: __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"] }), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"]) === 'function' && _a) || Object)
    ], DynamicListComponent.prototype, "parent", void 0);
    DynamicListComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-dynamic-list',
            template: __webpack_require__(1351),
            styles: [__webpack_require__(1286)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__angular_router__["d" /* ActivatedRoute */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_6__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__context_service__["a" /* ContextService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_7__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__error_service__["a" /* ErrorService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ComponentFactoryResolver"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ComponentFactoryResolver"]) === 'function' && _f) || Object, (typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_1__custom_component_service__["a" /* CustomComponentService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__custom_component_service__["a" /* CustomComponentService */]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */]) === 'function' && _h) || Object])
    ], DynamicListComponent);
    return DynamicListComponent;
    var _a, _b, _c, _d, _e, _f, _g, _h;
}(__WEBPACK_IMPORTED_MODULE_5__pane_pane__["a" /* PaneComponent */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/dynamic-list.component.js.map

/***/ }),

/***/ 590:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__custom_component_service__ = __webpack_require__(255);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__pane_pane__ = __webpack_require__(148);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__config_service__ = __webpack_require__(21);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return DynamicObjectComponent; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};








var DynamicObjectComponent = (function (_super) {
    __extends(DynamicObjectComponent, _super);
    function DynamicObjectComponent(activatedRoute, urlManager, componentFactoryResolver, customComponentService, configService) {
        _super.call(this, activatedRoute, urlManager);
        this.componentFactoryResolver = componentFactoryResolver;
        this.customComponentService = customComponentService;
        this.configService = configService;
    }
    DynamicObjectComponent.prototype.setup = function (routeData) {
        var _this = this;
        if (!routeData.objectId) {
            return;
        }
        var oid = __WEBPACK_IMPORTED_MODULE_6__models__["d" /* ObjectIdWrapper */].fromObjectId(routeData.objectId, this.configService.config.keySeparator);
        if (oid.domainType !== this.lastOid) {
            this.lastOid = oid.domainType;
            this.parent.clear();
            this.customComponentService.getCustomComponent(this.lastOid, __WEBPACK_IMPORTED_MODULE_3__route_data__["c" /* ViewType */].Object).then(function (c) {
                var childComponent = _this.componentFactoryResolver.resolveComponentFactory(c);
                _this.parent.createComponent(childComponent);
            });
        }
    };
    DynamicObjectComponent.prototype.ngOnDestroy = function () {
        _super.prototype.ngOnDestroy.call(this);
        this.parent.clear();
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChild"])('parent', { read: __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"] }), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewContainerRef"]) === 'function' && _a) || Object)
    ], DynamicObjectComponent.prototype, "parent", void 0);
    DynamicObjectComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-dynamic-object',
            template: __webpack_require__(1352),
            styles: [__webpack_require__(1287)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__angular_router__["d" /* ActivatedRoute */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ComponentFactoryResolver"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ComponentFactoryResolver"]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_1__custom_component_service__["a" /* CustomComponentService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__custom_component_service__["a" /* CustomComponentService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_7__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__config_service__["a" /* ConfigService */]) === 'function' && _f) || Object])
    ], DynamicObjectComponent);
    return DynamicObjectComponent;
    var _a, _b, _c, _d, _e, _f;
}(__WEBPACK_IMPORTED_MODULE_5__pane_pane__["a" /* PaneComponent */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/dynamic-object.component.js.map

/***/ }),

/***/ 591:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__field_field_component__ = __webpack_require__(594);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__view_models_parameter_view_model__ = __webpack_require__(368);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__view_models_dialog_view_model__ = __webpack_require__(257);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__logger_service__ = __webpack_require__(87);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return EditParameterComponent; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};











var EditParameterComponent = (function (_super) {
    __extends(EditParameterComponent, _super);
    function EditParameterComponent(viewModelFactory, urlManager, myElement, context, configService, loggerService, renderer) {
        _super.call(this, myElement, context, configService, loggerService, renderer);
        this.viewModelFactory = viewModelFactory;
        this.urlManager = urlManager;
        this.choiceName = function (choice) { return choice.name; };
    }
    Object.defineProperty(EditParameterComponent.prototype, "parameter", {
        get: function () {
            return this.parm;
        },
        set: function (value) {
            this.parm = value;
            this.droppable = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "parameterPaneId", {
        get: function () {
            return this.parameter.paneArgId;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "title", {
        get: function () {
            return this.parameter.title;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "parameterType", {
        get: function () {
            return this.parameter.type;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "parameterEntryType", {
        get: function () {
            return this.parameter.entryType;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "parameterReturnType", {
        get: function () {
            return this.parameter.returnType;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "format", {
        get: function () {
            return this.parameter.format;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "description", {
        get: function () {
            return this.parameter.description;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "parameterId", {
        get: function () {
            return this.parameter.id;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "choices", {
        get: function () {
            return this.parameter.choices;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "isMultiline", {
        get: function () {
            return !(this.parameter.multipleLines === 1);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "isPassword", {
        get: function () {
            return this.parameter.password;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "multilineHeight", {
        get: function () {
            return this.parameter.multipleLines * 20 + "px";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditParameterComponent.prototype, "rows", {
        get: function () {
            return this.parameter.multipleLines;
        },
        enumerable: true,
        configurable: true
    });
    EditParameterComponent.prototype.classes = function () {
        return (_a = {},
            _a[this.parm.color] = true,
            _a["candrop"] = this.canDrop,
            _a["mat-input-element"] = null,
            _a
        );
        var _a;
    };
    EditParameterComponent.prototype.ngOnInit = function () {
        _super.prototype.init.call(this, this.parent, this.parameter, this.form.controls[this.parm.id]);
    };
    Object.defineProperty(EditParameterComponent.prototype, "form", {
        get: function () {
            return this.formGroup;
        },
        set: function (fm) {
            this.formGroup = fm;
        },
        enumerable: true,
        configurable: true
    });
    EditParameterComponent.prototype.isChoices = function () {
        return this.parm.entryType === __WEBPACK_IMPORTED_MODULE_3__models__["h" /* EntryType */].Choices ||
            this.parm.entryType === __WEBPACK_IMPORTED_MODULE_3__models__["h" /* EntryType */].ConditionalChoices ||
            this.parm.entryType === __WEBPACK_IMPORTED_MODULE_3__models__["h" /* EntryType */].MultipleChoices ||
            this.parm.entryType === __WEBPACK_IMPORTED_MODULE_3__models__["h" /* EntryType */].MultipleConditionalChoices;
    };
    EditParameterComponent.prototype.isMultiple = function () {
        return this.parm.entryType === __WEBPACK_IMPORTED_MODULE_3__models__["h" /* EntryType */].MultipleChoices ||
            this.parm.entryType === __WEBPACK_IMPORTED_MODULE_3__models__["h" /* EntryType */].MultipleConditionalChoices;
    };
    EditParameterComponent.prototype.onKeydown = function (event) {
        this.handleKeyEvents(event, this.isMultiline);
    };
    EditParameterComponent.prototype.onKeypress = function (event) {
        this.handleKeyEvents(event, this.isMultiline);
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_8__view_models_dialog_view_model__["a" /* DialogViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__view_models_dialog_view_model__["a" /* DialogViewModel */]) === 'function' && _a) || Object)
    ], EditParameterComponent.prototype, "parent", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_7__view_models_parameter_view_model__["a" /* ParameterViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__view_models_parameter_view_model__["a" /* ParameterViewModel */]) === 'function' && _b) || Object), 
        __metadata('design:paramtypes', [(typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_7__view_models_parameter_view_model__["a" /* ParameterViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__view_models_parameter_view_model__["a" /* ParameterViewModel */]) === 'function' && _c) || Object])
    ], EditParameterComponent.prototype, "parameter", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_5__angular_forms__["f" /* FormGroup */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__angular_forms__["f" /* FormGroup */]) === 'function' && _d) || Object), 
        __metadata('design:paramtypes', [(typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_5__angular_forms__["f" /* FormGroup */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__angular_forms__["f" /* FormGroup */]) === 'function' && _e) || Object])
    ], EditParameterComponent.prototype, "form", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('keydown', ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], EditParameterComponent.prototype, "onKeydown", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('keypress', ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], EditParameterComponent.prototype, "onKeypress", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])("focus"), 
        __metadata('design:type', (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _f) || Object)
    ], EditParameterComponent.prototype, "focusList", void 0);
    EditParameterComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-edit-parameter',
            template: __webpack_require__(1353),
            styles: [__webpack_require__(1288)]
        }), 
        __metadata('design:paramtypes', [(typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _h) || Object, (typeof (_j = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]) === 'function' && _j) || Object, (typeof (_k = typeof __WEBPACK_IMPORTED_MODULE_6__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__context_service__["a" /* ContextService */]) === 'function' && _k) || Object, (typeof (_l = typeof __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */]) === 'function' && _l) || Object, (typeof (_m = typeof __WEBPACK_IMPORTED_MODULE_10__logger_service__["a" /* LoggerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_10__logger_service__["a" /* LoggerService */]) === 'function' && _m) || Object, (typeof (_o = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"]) === 'function' && _o) || Object])
    ], EditParameterComponent);
    return EditParameterComponent;
    var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o;
}(__WEBPACK_IMPORTED_MODULE_4__field_field_component__["a" /* FieldComponent */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/edit-parameter.component.js.map

/***/ }),

/***/ 592:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__field_field_component__ = __webpack_require__(594);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__view_models_property_view_model__ = __webpack_require__(369);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__view_models_domain_object_view_model__ = __webpack_require__(258);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__logger_service__ = __webpack_require__(87);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return EditPropertyComponent; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};










var EditPropertyComponent = (function (_super) {
    __extends(EditPropertyComponent, _super);
    function EditPropertyComponent(myElement, router, error, context, configService, loggerService, renderer) {
        _super.call(this, myElement, context, configService, loggerService, renderer);
        this.router = router;
        this.error = error;
    }
    Object.defineProperty(EditPropertyComponent.prototype, "property", {
        get: function () {
            return this.prop;
        },
        set: function (value) {
            this.prop = value;
            this.droppable = value;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "propertyPaneId", {
        get: function () {
            return this.property.paneArgId;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "propertyId", {
        get: function () {
            return this.property.id;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "propertyChoices", {
        get: function () {
            return this.property.choices;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "title", {
        get: function () {
            return this.property.title;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "propertyType", {
        get: function () {
            return this.property.type;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "propertyReturnType", {
        get: function () {
            return this.property.returnType;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "propertyEntryType", {
        get: function () {
            return this.property.entryType;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "isEditable", {
        get: function () {
            return this.property.isEditable;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "formattedValue", {
        get: function () {
            return this.property.formattedValue;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "value", {
        get: function () {
            return this.property.formattedValue;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "format", {
        get: function () {
            return this.property.format;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "isBlob", {
        get: function () {
            return this.property.format === "blob";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "isMultiline", {
        get: function () {
            return !(this.property.multipleLines === 1);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "isPassword", {
        get: function () {
            return this.property.password;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "multilineHeight", {
        get: function () {
            return this.property.multipleLines * 20 + "px";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "rows", {
        get: function () {
            return this.property.multipleLines;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "propertyDescription", {
        get: function () {
            return this.property.description;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "message", {
        get: function () {
            return this.property.getMessage();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(EditPropertyComponent.prototype, "attachment", {
        get: function () {
            return this.property.attachment;
        },
        enumerable: true,
        configurable: true
    });
    EditPropertyComponent.prototype.choiceName = function (choice) {
        return choice.name;
    };
    EditPropertyComponent.prototype.classes = function () {
        return (_a = {},
            _a[this.prop.color] = true,
            _a["candrop"] = this.canDrop,
            _a["mat-input-element"] = null,
            _a
        );
        var _a;
    };
    Object.defineProperty(EditPropertyComponent.prototype, "form", {
        get: function () {
            return this.formGroup;
        },
        set: function (fm) {
            this.formGroup = fm;
        },
        enumerable: true,
        configurable: true
    });
    EditPropertyComponent.prototype.ngOnInit = function () {
        _super.prototype.init.call(this, this.parent, this.property, this.form.controls[this.prop.id]);
    };
    EditPropertyComponent.prototype.datePickerChanged = function (evt) {
        var val = evt.target.value;
        this.formGroup.value[this.property.id] = val;
    };
    EditPropertyComponent.prototype.onKeydown = function (event) {
        this.handleKeyEvents(event, this.isMultiline);
    };
    EditPropertyComponent.prototype.onKeypress = function (event) {
        this.handleKeyEvents(event, this.isMultiline);
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_7__view_models_domain_object_view_model__["a" /* DomainObjectViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__view_models_domain_object_view_model__["a" /* DomainObjectViewModel */]) === 'function' && _a) || Object)
    ], EditPropertyComponent.prototype, "parent", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_6__view_models_property_view_model__["a" /* PropertyViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__view_models_property_view_model__["a" /* PropertyViewModel */]) === 'function' && _b) || Object), 
        __metadata('design:paramtypes', [(typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_6__view_models_property_view_model__["a" /* PropertyViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__view_models_property_view_model__["a" /* PropertyViewModel */]) === 'function' && _c) || Object])
    ], EditPropertyComponent.prototype, "property", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_2__angular_forms__["f" /* FormGroup */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__angular_forms__["f" /* FormGroup */]) === 'function' && _d) || Object), 
        __metadata('design:paramtypes', [(typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_2__angular_forms__["f" /* FormGroup */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__angular_forms__["f" /* FormGroup */]) === 'function' && _e) || Object])
    ], EditPropertyComponent.prototype, "form", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('keydown', ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], EditPropertyComponent.prototype, "onKeydown", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["HostListener"])('keypress', ['$event']), 
        __metadata('design:type', Function), 
        __metadata('design:paramtypes', [Object]), 
        __metadata('design:returntype', void 0)
    ], EditPropertyComponent.prototype, "onKeypress", null);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])("focus"), 
        __metadata('design:type', (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _f) || Object)
    ], EditPropertyComponent.prototype, "focusList", void 0);
    EditPropertyComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-edit-property',
            template: __webpack_require__(1354),
            styles: [__webpack_require__(1289)]
        }), 
        __metadata('design:paramtypes', [(typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_3__angular_router__["b" /* Router */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__angular_router__["b" /* Router */]) === 'function' && _h) || Object, (typeof (_j = typeof __WEBPACK_IMPORTED_MODULE_4__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__error_service__["a" /* ErrorService */]) === 'function' && _j) || Object, (typeof (_k = typeof __WEBPACK_IMPORTED_MODULE_5__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__context_service__["a" /* ContextService */]) === 'function' && _k) || Object, (typeof (_l = typeof __WEBPACK_IMPORTED_MODULE_8__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__config_service__["a" /* ConfigService */]) === 'function' && _l) || Object, (typeof (_m = typeof __WEBPACK_IMPORTED_MODULE_9__logger_service__["a" /* LoggerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_9__logger_service__["a" /* LoggerService */]) === 'function' && _m) || Object, (typeof (_o = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["Renderer"]) === 'function' && _o) || Object])
    ], EditPropertyComponent);
    return EditPropertyComponent;
    var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o;
}(__WEBPACK_IMPORTED_MODULE_1__field_field_component__["a" /* FieldComponent */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/edit-property.component.js.map

/***/ }),

/***/ 593:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_model_factory_service__ = __webpack_require__(64);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ErrorComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var ErrorComponent = (function () {
    function ErrorComponent(context, viewModelFactory) {
        this.context = context;
        this.viewModelFactory = viewModelFactory;
    }
    ErrorComponent.prototype.ngOnInit = function () {
        // expect dynamic-error to  have checked if the context has an error 
        // todo do we cover all the possible errors from Spa 1 ?
        var errorWrapper = this.context.getError();
        var error = this.viewModelFactory.errorViewModel(errorWrapper);
        this.title = error.title;
        this.message = error.message;
        this.errorCode = error.errorCode;
        this.description = error.description;
        this.stackTrace = error.stackTrace;
    };
    ErrorComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-error',
            template: __webpack_require__(1355),
            styles: [__webpack_require__(1290)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _b) || Object])
    ], ErrorComponent);
    return ErrorComponent;
    var _a, _b;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/error.component.js.map

/***/ }),

/***/ 594:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__view_models_choice_view_model__ = __webpack_require__(106);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__view_models_helpers_view_models__ = __webpack_require__(41);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return FieldComponent; });




var FieldComponent = (function () {
    function FieldComponent(myElement, context, configService, loggerService, renderer) {
        this.context = context;
        this.configService = configService;
        this.loggerService = loggerService;
        this.renderer = renderer;
        this.currentOptions = [];
        this.canDrop = false;
        this.elementRef = myElement;
    }
    FieldComponent.prototype.init = function (vmParent, vm, control) {
        this.vmParent = vmParent;
        this.model = vm;
        this.control = control;
        this.paneId = this.model.onPaneId;
        this.isConditionalChoices = (this.model.entryType === __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].ConditionalChoices ||
            this.model.entryType === __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].MultipleConditionalChoices);
        this.isAutoComplete = this.model.entryType === __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].AutoComplete;
        if (this.isConditionalChoices) {
            this.pArgs = __WEBPACK_IMPORTED_MODULE_1_lodash__["omit"](this.model.promptArguments, "x-ro-nof-members");
            this.populateDropdown();
        }
    };
    FieldComponent.prototype.accept = function (droppableVm) {
        var _this = this;
        return function (draggableVm) {
            if (draggableVm) {
                draggableVm.canDropOn(droppableVm.returnType).then(function (canDrop) { return _this.canDrop = canDrop; }).catch(function () { return _this.canDrop = false; });
                return true;
            }
            return false;
        };
    };
    ;
    FieldComponent.prototype.drop = function (draggableVm) {
        var _this = this;
        if (this.canDrop) {
            this.droppable.drop(draggableVm)
                .then(function (success) {
                _this.control.setValue(_this.model.selectedChoice);
            });
        }
    };
    FieldComponent.prototype.isDomainObjectViewModel = function (object) {
        return object && "properties" in object;
    };
    FieldComponent.prototype.mapValues = function (args, parmsOrProps) {
        return __WEBPACK_IMPORTED_MODULE_1_lodash__["mapValues"](this.pArgs, function (v, n) {
            var pop = __WEBPACK_IMPORTED_MODULE_1_lodash__["find"](parmsOrProps, function (p) { return p.argId === n; });
            return pop.getValue();
        });
    };
    FieldComponent.prototype.populateArguments = function () {
        var dialog = this.vmParent;
        var object = this.vmParent;
        if (!dialog && !object) {
            this.loggerService.throw("FieldComponent:populateArguments Expect dialog or object");
        }
        var parmsOrProps;
        if (this.isDomainObjectViewModel(object)) {
            parmsOrProps = object.properties;
        }
        else {
            parmsOrProps = dialog.parameters;
        }
        return this.mapValues(this.pArgs, parmsOrProps);
    };
    FieldComponent.prototype.populateDropdown = function () {
        var _this = this;
        var nArgs = this.populateArguments();
        var prompts = this.model.conditionalChoices(nArgs); //  scope.select({ args: nArgs });
        prompts.
            then(function (cvms) {
            // if unchanged return 
            if (cvms.length === _this.currentOptions.length && __WEBPACK_IMPORTED_MODULE_1_lodash__["every"](cvms, function (c, i) { return c.equals(_this.currentOptions[i]); })) {
                return;
            }
            _this.model.choices = cvms;
            _this.currentOptions = cvms;
            if (_this.isConditionalChoices) {
                // need to reset control to find the selected options 
                if (_this.model.entryType === __WEBPACK_IMPORTED_MODULE_0__models__["h" /* EntryType */].MultipleConditionalChoices) {
                    _this.control.reset(_this.model.selectedMultiChoices);
                }
                else {
                    _this.control.reset(_this.model.selectedChoice);
                }
            }
        }).
            catch(function () {
            // error clear everything 
            _this.model.selectedChoice = null;
            _this.currentOptions = [];
        });
    };
    FieldComponent.prototype.wrapReferences = function (val) {
        if (val && this.model.type === "ref") {
            return { href: val };
        }
        return val;
    };
    FieldComponent.prototype.onChange = function () {
        if (this.isConditionalChoices) {
            this.populateDropdown();
        }
        else if (this.isAutoComplete) {
            this.populateAutoComplete();
        }
    };
    Object.defineProperty(FieldComponent.prototype, "message", {
        get: function () {
            return this.model.getMessage();
        },
        enumerable: true,
        configurable: true
    });
    FieldComponent.prototype.onValueChanged = function () {
        if (this.model) {
            this.onChange();
        }
    };
    Object.defineProperty(FieldComponent.prototype, "formGroup", {
        get: function () {
            return this.formGrp;
        },
        set: function (fm) {
            var _this = this;
            this.formGrp = fm;
            this.formGrp.valueChanges.subscribe(function (data) { return _this.onValueChanged(); });
            this.onValueChanged(); // (re)set validation messages now
        },
        enumerable: true,
        configurable: true
    });
    FieldComponent.prototype.populateAutoComplete = function () {
        var _this = this;
        var input = this.control.value;
        if (input instanceof __WEBPACK_IMPORTED_MODULE_2__view_models_choice_view_model__["a" /* ChoiceViewModel */]) {
            return;
        }
        if (input && input.length > 0 && input.length >= this.model.minLength) {
            this.model.prompt(input)
                .then(function (cvms) {
                if (cvms.length === _this.currentOptions.length && __WEBPACK_IMPORTED_MODULE_1_lodash__["every"](cvms, function (c, i) { return c.equals(_this.currentOptions[i]); })) {
                    return;
                }
                _this.model.choices = cvms;
                _this.currentOptions = cvms;
                _this.model.selectedChoice = null;
            })
                .catch(function () {
                _this.model.choices = [];
                _this.currentOptions = [];
                _this.model.selectedChoice = null;
            });
        }
        else {
            this.model.choices = [];
            this.currentOptions = [];
            this.model.selectedChoice = null;
        }
    };
    FieldComponent.prototype.select = function (item) {
        this.model.choices = [];
        this.model.selectedChoice = item;
        this.control.reset(item);
    };
    FieldComponent.prototype.fileUpload = function (evt) {
        var _this = this;
        var file = evt.target.files[0];
        var fileReader = new FileReader();
        fileReader.onloadend = function () {
            var link = new __WEBPACK_IMPORTED_MODULE_0__models__["T" /* Link */]({
                href: fileReader.result,
                type: file.type,
                title: file.name
            });
            _this.control.reset(link);
            _this.model.file = link;
        };
        fileReader.readAsDataURL(file);
    };
    FieldComponent.prototype.paste = function (event) {
        var _this = this;
        var vKeyCode = 86;
        var deleteKeyCode = 46;
        if (event && (event.keyCode === vKeyCode && event.ctrlKey)) {
            var cvm = this.context.getCopyViewModel();
            if (cvm) {
                this.droppable.drop(cvm)
                    .then(function (success) {
                    _this.control.setValue(_this.model.selectedChoice);
                });
                event.preventDefault();
            }
        }
        if (event && event.keyCode === deleteKeyCode) {
            this.context.setCopyViewModel(null);
        }
    };
    FieldComponent.prototype.filterEnter = function (event) {
        var enterKeyCode = 13;
        if (event && event.keyCode === enterKeyCode) {
            event.preventDefault();
        }
    };
    FieldComponent.prototype.handleKeyEvents = function (event, isMultiline) {
        this.paste(event);
        // catch and filter enters or they will submit form - ok for multiline
        if (!isMultiline) {
            this.filterEnter(event);
        }
    };
    FieldComponent.prototype.focus = function () {
        return !!(this.focusList && this.focusList.first) && __WEBPACK_IMPORTED_MODULE_3__view_models_helpers_view_models__["m" /* focus */](this.renderer, this.focusList.first);
    };
    return FieldComponent;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/field.component.js.map

/***/ }),

/***/ 595:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__pane_pane__ = __webpack_require__(148);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return HomeComponent; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};







var HomeComponent = (function (_super) {
    __extends(HomeComponent, _super);
    function HomeComponent(urlManager, activatedRoute, viewModelFactory, contextService, errorService, myElement) {
        _super.call(this, activatedRoute, urlManager);
        this.viewModelFactory = viewModelFactory;
        this.contextService = contextService;
        this.errorService = errorService;
        this.myElement = myElement;
    }
    Object.defineProperty(HomeComponent.prototype, "hasMenus", {
        // template API 
        get: function () {
            return !!this.menus;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(HomeComponent.prototype, "menuItems", {
        get: function () {
            return this.menus.items;
        },
        enumerable: true,
        configurable: true
    });
    HomeComponent.prototype.getMenus = function (paneRouteData) {
        var _this = this;
        this.contextService.getMenus()
            .then(function (menus) {
            _this.menus = _this.viewModelFactory.menusViewModel(menus, paneRouteData);
        })
            .catch(function (reject) {
            _this.errorService.handleError(reject);
        });
    };
    HomeComponent.prototype.getMenu = function (paneRouteData) {
        var _this = this;
        var menuId = paneRouteData.menuId;
        if (menuId) {
            this.contextService.getMenu(menuId)
                .then(function (menu) {
                _this.selectedMenu = _this.viewModelFactory.menuViewModel(menu, paneRouteData);
                _this.selectedDialogId = paneRouteData.dialogId;
            })
                .catch(function (reject) {
                _this.errorService.handleError(reject);
            });
        }
        else {
            this.selectedMenu = null;
            this.selectedDialogId = null;
        }
    };
    HomeComponent.prototype.setup = function (paneRouteData) {
        this.getMenus(paneRouteData);
        this.getMenu(paneRouteData);
    };
    HomeComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-home',
            template: __webpack_require__(1358),
            styles: [__webpack_require__(1293)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_router__["d" /* ActivatedRoute */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_5__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__context_service__["a" /* ContextService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_4__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__error_service__["a" /* ErrorService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["ElementRef"]) === 'function' && _f) || Object])
    ], HomeComponent);
    return HomeComponent;
    var _a, _b, _c, _d, _e, _f;
}(__WEBPACK_IMPORTED_MODULE_6__pane_pane__["a" /* PaneComponent */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/home.component.js.map

/***/ }),

/***/ 596:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__color_service__ = __webpack_require__(254);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__logger_service__ = __webpack_require__(87);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ListComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};











var ListComponent = (function () {
    function ListComponent(activatedRoute, urlManager, context, color, viewModelFactory, error, configService, loggerService) {
        var _this = this;
        this.activatedRoute = activatedRoute;
        this.urlManager = urlManager;
        this.context = context;
        this.color = color;
        this.viewModelFactory = viewModelFactory;
        this.error = error;
        this.configService = configService;
        this.loggerService = loggerService;
        this.toggleActionMenu = function () { return _this.collection.toggleActionMenu(); };
        this.reloadList = function () { return _this.collection.reload(); };
        this.pageFirst = function () { return _this.collection.pageFirst(); };
        this.pagePrevious = function () { return _this.collection.pagePrevious(); };
        this.pageNext = function () { return _this.collection.pageNext(); };
        this.pageLast = function () { return _this.collection.pageLast(); };
        this.disableActions = function () { return _this.collection.disableActions() ? true : null; };
        this.hideAllCheckbox = function () { return _this.collection.disableActions() || _this.collection.items.length === 0; };
        this.pageFirstDisabled = function () { return _this.collection.pageFirstDisabled() ? true : null; };
        this.pagePreviousDisabled = function () { return _this.collection.pagePreviousDisabled() ? true : null; };
        this.pageNextDisabled = function () { return _this.collection.pageNextDisabled() ? true : null; };
        this.pageLastDisabled = function () { return _this.collection.pageLastDisabled() ? true : null; };
        this.showActions = function () { return _this.collection.showActions(); };
        this.doTable = function () { return _this.collection.doTable(); };
        this.doList = function () { return _this.collection.doList(); };
        this.doSummary = function () { return _this.collection.doSummary(); };
        this.hasTableData = function () { return _this.collection.hasTableData(); };
        this.actionButton = {
            value: "Actions",
            doClick: function () { return _this.toggleActionMenu(); },
            show: function () { return true; },
            disabled: function () { return _this.disableActions(); },
            tempDisabled: function () { return null; },
            title: function () { return _this.actionsTooltip; },
            accesskey: "a"
        };
        this.reloadButton = {
            value: "Reload",
            doClick: function () { return _this.reloadList(); },
            show: function () { return true; },
            disabled: function () { return null; },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
        this.firstButton = {
            value: "First",
            doClick: function () { return _this.pageFirst(); },
            show: function () { return true; },
            disabled: function () { return _this.pageFirstDisabled(); },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
        this.previousButton = {
            value: "Previous",
            doClick: function () { return _this.pagePrevious(); },
            show: function () { return true; },
            disabled: function () { return _this.pagePreviousDisabled(); },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
        this.nextButton = {
            value: "Next",
            doClick: function () { return _this.pageNext(); },
            show: function () { return true; },
            disabled: function () { return _this.pageNextDisabled(); },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
        this.lastButton = {
            value: "Last",
            doClick: function () { return _this.pageLast(); },
            show: function () { return true; },
            disabled: function () { return _this.pageLastDisabled(); },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
        this.title = "";
        this.currentState = __WEBPACK_IMPORTED_MODULE_6__route_data__["b" /* CollectionViewState */].List;
    }
    Object.defineProperty(ListComponent.prototype, "actionsTooltip", {
        get: function () {
            return this.collection.actionsTooltip();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ListComponent.prototype, "message", {
        get: function () {
            return this.collection.getMessage();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ListComponent.prototype, "description", {
        get: function () {
            return this.collection.description();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ListComponent.prototype, "size", {
        get: function () {
            return this.collection.size;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ListComponent.prototype, "items", {
        get: function () {
            return this.collection.items;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ListComponent.prototype, "header", {
        get: function () {
            return this.collection.header;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ListComponent.prototype, "actionHolders", {
        get: function () {
            return [this.actionButton, this.reloadButton, this.firstButton, this.previousButton, this.nextButton, this.lastButton];
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ListComponent.prototype, "state", {
        get: function () {
            return __WEBPACK_IMPORTED_MODULE_6__route_data__["b" /* CollectionViewState */][this.currentState].toString().toLowerCase();
        },
        enumerable: true,
        configurable: true
    });
    ListComponent.prototype.getActionExtensions = function (routeData) {
        return routeData.objectId
            ? this.context.getActionExtensionsFromObject(routeData.paneId, __WEBPACK_IMPORTED_MODULE_8__models__["d" /* ObjectIdWrapper */].fromObjectId(routeData.objectId, this.configService.config.keySeparator), routeData.actionId)
            : this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
    };
    ListComponent.prototype.setup = function (routeData) {
        var _this = this;
        this.cachedRouteData = routeData;
        var cachedList = this.context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);
        this.getActionExtensions(routeData)
            .then(function (ext) {
            return _this.title = ext.friendlyName();
        })
            .catch(function (reject) { return _this.error.handleError(reject); });
        var listKey = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);
        if (this.collection && this.collection.id === listKey) {
            // same collection/page
            this.currentState = routeData.state;
            this.collection.refresh(routeData);
        }
        else if (this.collection && cachedList) {
            // same collection different page
            this.currentState = routeData.state;
            this.collection.reset(cachedList, routeData);
        }
        else if (cachedList) {
            // new collection 
            this.collection = this.viewModelFactory.listViewModel(cachedList, routeData);
            this.currentState = routeData.state;
            this.collection.refresh(routeData);
        }
        else {
            // should never get here 
            this.loggerService.throw("ListComponent:setup Missing cachedList");
        }
        if (this.collection) {
            // if any previous messages clear them
            this.collection.resetMessage();
        }
        this.selectedDialogId = routeData.dialogId;
    };
    // now this is a child investigate reworking so object is passed in from parent 
    ListComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(function (data) {
            var paneId = data.pane;
            if (!_this.paneRouteDataSub) {
                _this.paneRouteDataSub =
                    _this.urlManager.getPaneRouteDataObservable(paneId)
                        .subscribe(function (paneRouteData) {
                        if (!paneRouteData.isEqual(_this.lastPaneRouteData)) {
                            _this.lastPaneRouteData = paneRouteData;
                            _this.setup(paneRouteData);
                        }
                    });
            }
            ;
        });
    };
    ListComponent.prototype.ngOnDestroy = function () {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    };
    ListComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-list',
            template: __webpack_require__(1359),
            styles: [__webpack_require__(1294)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_3__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__angular_router__["d" /* ActivatedRoute */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_2__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__context_service__["a" /* ContextService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_4__color_service__["a" /* ColorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__color_service__["a" /* ColorService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_7__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_5__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__error_service__["a" /* ErrorService */]) === 'function' && _f) || Object, (typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_10__logger_service__["a" /* LoggerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_10__logger_service__["a" /* LoggerService */]) === 'function' && _h) || Object])
    ], ListComponent);
    return ListComponent;
    var _a, _b, _c, _d, _e, _f, _g, _h;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/list.component.js.map

/***/ }),

/***/ 597:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__pane_pane__ = __webpack_require__(148);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__parameters_parameters_component__ = __webpack_require__(364);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__view_models_helpers_view_models__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_13_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return MultiLineDialogComponent; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};














var MultiLineDialogComponent = (function (_super) {
    __extends(MultiLineDialogComponent, _super);
    function MultiLineDialogComponent(activatedRoute, urlManager, viewModelFactory, context, error, formBuilder, configService) {
        var _this = this;
        _super.call(this, activatedRoute, urlManager);
        this.viewModelFactory = viewModelFactory;
        this.context = context;
        this.error = error;
        this.formBuilder = formBuilder;
        this.configService = configService;
        this.form = function (i) {
            var rowData = _this.rowData[i];
            return rowData.form;
        };
        this.parameters = function (row) { return row.parameters; };
        this.rowSubmitted = function (row) { return row.submitted; };
        this.rowTooltip = function (row) { return row.tooltip(); };
        this.rowMessage = function (row) {
            return row.submitted ? __WEBPACK_IMPORTED_MODULE_11__user_messages__["_95" /* submittedMessage */] : row.getMessage();
        };
        this.rowDisabled = function (row) {
            return !row.clientValid() || row.submitted;
        };
        this.close = function () {
            _this.urlManager.popUrlState();
        };
    }
    Object.defineProperty(MultiLineDialogComponent.prototype, "objectFriendlyName", {
        get: function () {
            return this.dialog.objectFriendlyName;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(MultiLineDialogComponent.prototype, "objectTitle", {
        get: function () {
            return this.dialog.objectTitle;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(MultiLineDialogComponent.prototype, "dialogTitle", {
        get: function () {
            return this.dialog.title;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(MultiLineDialogComponent.prototype, "header", {
        get: function () {
            return this.dialog.header();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(MultiLineDialogComponent.prototype, "rows", {
        get: function () {
            return this.dialog.dialogs;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(MultiLineDialogComponent.prototype, "count", {
        get: function () {
            return __WEBPACK_IMPORTED_MODULE_11__user_messages__["_96" /* submittedCount */](this.dialog.submittedCount());
        },
        enumerable: true,
        configurable: true
    });
    MultiLineDialogComponent.prototype.invokeAndAdd = function (index) {
        var _this = this;
        var parms = this.rowData[index].parms;
        __WEBPACK_IMPORTED_MODULE_13_lodash__["forEach"](parms, function (p) {
            var newValue = _this.rowData[index].form.value[p.id];
            p.setValueFromControl(newValue);
        });
        var addedIndex = this.dialog.invokeAndAdd(index);
        if (addedIndex) {
            this.rowData.push(this.createForm(this.dialog.dialogs[addedIndex]));
        }
    };
    MultiLineDialogComponent.prototype.createForm = function (dialog) {
        return __WEBPACK_IMPORTED_MODULE_10__view_models_helpers_view_models__["n" /* createForm */](dialog, this.formBuilder);
    };
    MultiLineDialogComponent.prototype.setMultiLineDialog = function (holder, newDialogId, routeData, actionViewModel) {
        var _this = this;
        var action = holder.actionMember(newDialogId, this.configService.config.keySeparator);
        this.context.getInvokableAction(action).
            then(function (details) {
            if (actionViewModel) {
                actionViewModel.makeInvokable(details);
            }
            _this.dialog = _this.viewModelFactory.multiLineDialogViewModel(routeData, details, holder);
            _this.rowData = __WEBPACK_IMPORTED_MODULE_13_lodash__["map"](_this.dialog.dialogs, function (d) { return _this.createForm(d); });
        }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    MultiLineDialogComponent.prototype.setup = function (routeData) {
        var _this = this;
        if (routeData.menuId) {
            this.context.getMenu(routeData.menuId)
                .then(function (menu) {
                _this.setMultiLineDialog(menu, routeData.dialogId, routeData);
            })
                .catch(function (reject) {
                _this.error.handleError(reject);
            });
        }
        else if (routeData.objectId) {
            var oid = __WEBPACK_IMPORTED_MODULE_12__models__["d" /* ObjectIdWrapper */].fromObjectId(routeData.objectId, this.configService.config.keySeparator);
            this.context.getObject(routeData.paneId, oid, routeData.interactionMode).
                then(function (object) {
                var ovm = _this.viewModelFactory.domainObjectViewModel(object, routeData, false);
                var newDialogId = routeData.dialogId;
                var lcaCollection = __WEBPACK_IMPORTED_MODULE_13_lodash__["find"](ovm.collections, function (c) { return c.hasMatchingLocallyContributedAction(newDialogId); });
                if (lcaCollection) {
                    var actionViewModel = __WEBPACK_IMPORTED_MODULE_13_lodash__["find"](lcaCollection.actions, function (a) { return a.actionRep.actionId() === newDialogId; });
                    _this.setMultiLineDialog(lcaCollection, newDialogId, routeData, actionViewModel);
                }
                else {
                    _this.setMultiLineDialog(object, newDialogId, routeData);
                }
            }).
                catch(function (reject) {
                _this.error.handleError(reject);
            });
        }
    };
    MultiLineDialogComponent.prototype.focus = function (parms) {
        if (parms && parms.length > 0) {
            __WEBPACK_IMPORTED_MODULE_13_lodash__["some"](parms.toArray(), function (p) { return p.focus(); });
        }
    };
    MultiLineDialogComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        this.parmComponents.changes.subscribe(function (ql) { return _this.focus(ql); });
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])(__WEBPACK_IMPORTED_MODULE_2__parameters_parameters_component__["a" /* ParametersComponent */]), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _a) || Object)
    ], MultiLineDialogComponent.prototype, "parmComponents", void 0);
    MultiLineDialogComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-multi-line-dialog',
            template: __webpack_require__(1362),
            styles: [__webpack_require__(1297)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_4__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__angular_router__["d" /* ActivatedRoute */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_5__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_3__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_6__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__context_service__["a" /* ContextService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_7__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_7__error_service__["a" /* ErrorService */]) === 'function' && _f) || Object, (typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_8__angular_forms__["e" /* FormBuilder */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__angular_forms__["e" /* FormBuilder */]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_9__config_service__["a" /* ConfigService */]) === 'function' && _h) || Object])
    ], MultiLineDialogComponent);
    return MultiLineDialogComponent;
    var _a, _b, _c, _d, _e, _f, _g, _h;
}(__WEBPACK_IMPORTED_MODULE_1__pane_pane__["a" /* PaneComponent */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/multi-line-dialog.component.js.map

/***/ }),

/***/ 598:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_9_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__action_action_component__ = __webpack_require__(122);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__color_service__ = __webpack_require__(254);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13__properties_properties_component__ = __webpack_require__(599);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_14__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_15__view_models_helpers_view_models__ = __webpack_require__(41);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ObjectComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
















var ObjectComponent = (function () {
    function ObjectComponent(activatedRoute, urlManager, context, viewModelFactory, colorService, error, formBuilder, configService) {
        var _this = this;
        this.activatedRoute = activatedRoute;
        this.urlManager = urlManager;
        this.context = context;
        this.viewModelFactory = viewModelFactory;
        this.colorService = colorService;
        this.error = error;
        this.formBuilder = formBuilder;
        this.configService = configService;
        // template API 
        this.expiredTransient = false;
        // todo investigate if logic in this would be better here rather than view model
        this.disableActions = function () {
            var obj = _this.object;
            return obj && obj.disableActions() ? true : null;
        };
        this.actionsTooltip = function () {
            var obj = _this.object;
            return obj ? obj.actionsTooltip() : "";
        };
        this.unsaved = function () {
            var obj = _this.object;
            return !!obj && obj.unsaved;
        };
        this.toggleActionMenu = function () {
            _this.do(function (o) { return o.toggleActionMenu(); });
        };
        this.doEdit = function () {
            _this.do(function (o) { return o.doEdit(); });
        };
        this.doEditCancel = function () {
            _this.do(function (o) { return o.doEditCancel(); });
        };
        this.showEdit = function () {
            var obj = _this.object;
            return !!obj && !obj.hideEdit();
        };
        this.doReload = function () {
            _this.do(function (o) { return o.doReload(); });
        };
        this.message = function () {
            var obj = _this.object;
            return obj ? obj.getMessage() : "";
        };
        this.showActions = function () {
            var obj = _this.object;
            return !!obj && obj.showActions();
        };
        this.menuItems = function () {
            var obj = _this.object;
            return obj ? obj.menuItems : [];
        };
        this.actionButton = {
            value: "Actions",
            doClick: function () { return _this.toggleActionMenu(); },
            show: function () { return true; },
            disabled: function () { return _this.disableActions(); },
            tempDisabled: function () { return null; },
            title: function () { return _this.actionsTooltip(); },
            accesskey: "a"
        };
        this.editButton = {
            value: "Edit",
            doClick: function () { return _this.doEdit(); },
            show: function () { return _this.showEdit(); },
            disabled: function () { return null; },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
        this.reloadButton = {
            value: "Reload",
            doClick: function () { return _this.doReload(); },
            show: function () { return true; },
            disabled: function () { return null; },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
        this.saveButton = {
            value: "Save",
            doClick: function () { return _this.onSubmit(true); },
            show: function () { return true; },
            disabled: function () { return _this.form && !_this.form.valid ? true : null; },
            tempDisabled: function () { return null; },
            title: function () { return _this.tooltip; },
            accesskey: null
        };
        this.saveAndCloseButton = {
            value: "Save & Close",
            doClick: function () { return _this.onSubmit(false); },
            show: function () { return _this.unsaved(); },
            disabled: function () { return _this.form && !_this.form.valid ? true : null; },
            tempDisabled: function () { return null; },
            title: function () { return _this.tooltip; },
            accesskey: null
        };
        this.cancelButton = {
            value: "Cancel",
            doClick: function () { return _this.doEditCancel(); },
            show: function () { return true; },
            disabled: function () { return null; },
            tempDisabled: function () { return null; },
            title: function () { return ""; },
            accesskey: null
        };
        this.pendingColor = "" + configService.config.objectColor + this.colorService.getDefault();
    }
    Object.defineProperty(ObjectComponent.prototype, "viewMode", {
        get: function () {
            return this.mode == null ? "" : __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */][this.mode];
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ObjectComponent.prototype, "friendlyName", {
        // must be properties as object may change - eg be reloaded 
        get: function () {
            var obj = this.object;
            return obj ? obj.friendlyName : "";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ObjectComponent.prototype, "color", {
        get: function () {
            var obj = this.object;
            return obj ? obj.color : this.pendingColor;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ObjectComponent.prototype, "properties", {
        get: function () {
            var obj = this.object;
            return obj ? obj.properties : "";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ObjectComponent.prototype, "collections", {
        get: function () {
            var obj = this.object;
            return obj ? obj.collections : "";
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ObjectComponent.prototype, "tooltip", {
        get: function () {
            var obj = this.object;
            return obj ? obj.tooltip() : "";
        },
        enumerable: true,
        configurable: true
    });
    ObjectComponent.prototype.onSubmit = function (viewObject) {
        var obj = this.object;
        if (obj) {
            obj.doSave(viewObject);
        }
    };
    ObjectComponent.prototype.copy = function (event) {
        var obj = this.object;
        if (obj) {
            __WEBPACK_IMPORTED_MODULE_15__view_models_helpers_view_models__["l" /* copy */](event, obj, this.context);
        }
    };
    ObjectComponent.prototype.title = function () {
        var obj = this.object;
        var prefix = this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].Edit || this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].Transient ? __WEBPACK_IMPORTED_MODULE_14__user_messages__["q" /* editing */] + " - " : "";
        return obj ? "" + prefix + obj.title : "";
    };
    ObjectComponent.prototype.do = function (f) {
        var obj = this.object;
        if (obj) {
            f(obj);
        }
    };
    Object.defineProperty(ObjectComponent.prototype, "actionHolders", {
        get: function () {
            if (this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].View) {
                return [this.actionButton, this.editButton, this.reloadButton];
            }
            if (this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].Edit || this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].Transient) {
                return [this.saveButton, this.saveAndCloseButton, this.cancelButton];
            }
            if (this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].Form) {
                // cache because otherwise we will recreate this array of actionHolders everytime page changes !
                if (!this.actionButtons) {
                    var menuItems = this.menuItems();
                    var actions = __WEBPACK_IMPORTED_MODULE_9_lodash__["flatten"](__WEBPACK_IMPORTED_MODULE_9_lodash__["map"](menuItems, function (mi) { return mi.actions; }));
                    this.actionButtons = __WEBPACK_IMPORTED_MODULE_9_lodash__["map"](actions, function (a) { return __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_10__action_action_component__["b" /* wrapAction */])(a); });
                }
                return this.actionButtons;
            }
            return [];
        },
        enumerable: true,
        configurable: true
    });
    // todo each component should be looking out for it's own changes - make this generic - eg 
    // component can register for changes it's wants to see rather  than this horrible filtering 
    // I'm doing everywhere 
    ObjectComponent.prototype.setup = function (routeData) {
        // subscription means may get with no oid 
        var _this = this;
        if (!routeData.objectId) {
            this.mode = null;
            return;
        }
        this.expiredTransient = false;
        var oid = __WEBPACK_IMPORTED_MODULE_2__models__["d" /* ObjectIdWrapper */].fromObjectId(routeData.objectId, this.configService.config.keySeparator);
        // todo this is a recurring pattern in angular 2 code - generalise 
        // across components 
        if (this.object && !this.object.domainObject.getOid(this.configService.config.keySeparator).isSame(oid)) {
            // object has changed - clear existing 
            this.object = null;
            this.form = null;
            this.actionButtons = null;
        }
        var isChanging = !this.object;
        var modeChanging = this.mode !== routeData.interactionMode;
        this.mode = routeData.interactionMode;
        var wasDirty = this.context.getIsDirty(oid);
        this.selectedDialogId = routeData.dialogId;
        if (isChanging || modeChanging || wasDirty) {
            // set pendingColor at once to smooth transition
            this.colorService.toColorNumberFromType(oid.domainType).then(function (c) { return _this.pendingColor = "" + _this.configService.config.objectColor + c; });
            this.context.getObject(routeData.paneId, oid, routeData.interactionMode)
                .then(function (object) {
                // only change the object property if the object has changed 
                if (isChanging || wasDirty) {
                    _this.object = _this.viewModelFactory.domainObjectViewModel(object, routeData, wasDirty);
                }
                if (modeChanging || isChanging) {
                    if (_this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].Edit ||
                        _this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].Form ||
                        _this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].Transient) {
                        _this.createForm(_this.object); // will never be null
                    }
                }
            })
                .catch(function (reject) {
                if (reject.category === __WEBPACK_IMPORTED_MODULE_2__models__["b" /* ErrorCategory */].ClientError && reject.clientErrorCode === __WEBPACK_IMPORTED_MODULE_2__models__["c" /* ClientErrorCode */].ExpiredTransient) {
                    _this.context.setError(reject);
                    _this.expiredTransient = true;
                }
                else {
                    _this.error.handleError(reject);
                }
            });
        }
    };
    ObjectComponent.prototype.createForm = function (vm) {
        var _this = this;
        var pps = vm.properties;
        var props = __WEBPACK_IMPORTED_MODULE_9_lodash__["zipObject"](__WEBPACK_IMPORTED_MODULE_9_lodash__["map"](pps, function (p) { return p.id; }), __WEBPACK_IMPORTED_MODULE_9_lodash__["map"](pps, function (p) { return p; }));
        var editableProps = __WEBPACK_IMPORTED_MODULE_9_lodash__["filter"](props, function (p) { return p.isEditable; });
        var editablePropsMap = __WEBPACK_IMPORTED_MODULE_9_lodash__["zipObject"](__WEBPACK_IMPORTED_MODULE_9_lodash__["map"](editableProps, function (p) { return p.id; }), __WEBPACK_IMPORTED_MODULE_9_lodash__["map"](editableProps, function (p) { return p; }));
        var controls = __WEBPACK_IMPORTED_MODULE_9_lodash__["mapValues"](editablePropsMap, function (p) { return [p.getValueForControl(), function (a) { return p.validator(a); }]; });
        this.form = this.formBuilder.group(controls);
        this.form.valueChanges.subscribe(function (data) {
            // cache parm values
            var obj = _this.object;
            if (obj) {
                __WEBPACK_IMPORTED_MODULE_9_lodash__["forEach"](data, function (v, k) { return editablePropsMap[k].setValueFromControl(v); });
                obj.setProperties();
            }
        });
    };
    // todo now this is a child investigate reworking so object is passed in from parent 
    ObjectComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(function (data) {
            var paneId = data.pane;
            if (!_this.paneRouteDataSub) {
                _this.paneRouteDataSub =
                    _this.urlManager.getPaneRouteDataObservable(paneId)
                        .subscribe(function (paneRouteData) {
                        if (!paneRouteData.isEqual(_this.lastPaneRouteData)) {
                            _this.lastPaneRouteData = paneRouteData;
                            _this.setup(paneRouteData);
                        }
                    });
            }
            ;
        });
        this.concurrencyErrorSub = this.context.concurrencyError$.subscribe(function (oid) {
            if (_this.object && _this.object.domainObject.getOid(_this.configService.config.keySeparator).isSame(oid)) {
                _this.object.concurrency();
            }
        });
    };
    ObjectComponent.prototype.ngOnDestroy = function () {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
        if (this.concurrencyErrorSub) {
            this.concurrencyErrorSub.unsubscribe();
        }
    };
    ObjectComponent.prototype.focus = function (parms) {
        if (this.mode == null || this.mode === __WEBPACK_IMPORTED_MODULE_7__route_data__["a" /* InteractionMode */].View) {
            return;
        }
        if (parms && parms.length > 0) {
            __WEBPACK_IMPORTED_MODULE_9_lodash__["some"](parms.toArray(), function (p) { return p.focus(); });
        }
    };
    ObjectComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        this.propComponents.changes.subscribe(function (ql) { return _this.focus(ql); });
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])(__WEBPACK_IMPORTED_MODULE_13__properties_properties_component__["a" /* PropertiesComponent */]), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _a) || Object)
    ], ObjectComponent.prototype, "propComponents", void 0);
    ObjectComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-object',
            template: __webpack_require__(1363),
            styles: [__webpack_require__(1298)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_router__["d" /* ActivatedRoute */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_3__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_4__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__context_service__["a" /* ContextService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_5__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_11__color_service__["a" /* ColorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_11__color_service__["a" /* ColorService */]) === 'function' && _f) || Object, (typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_6__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__error_service__["a" /* ErrorService */]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_8__angular_forms__["e" /* FormBuilder */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_8__angular_forms__["e" /* FormBuilder */]) === 'function' && _h) || Object, (typeof (_j = typeof __WEBPACK_IMPORTED_MODULE_12__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_12__config_service__["a" /* ConfigService */]) === 'function' && _j) || Object])
    ], ObjectComponent);
    return ObjectComponent;
    var _a, _b, _c, _d, _e, _f, _g, _h, _j;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/object.component.js.map

/***/ }),

/***/ 599:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_forms__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__edit_property_edit_property_component__ = __webpack_require__(592);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__view_models_domain_object_view_model__ = __webpack_require__(258);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return PropertiesComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var PropertiesComponent = (function () {
    function PropertiesComponent() {
    }
    PropertiesComponent.prototype.focus = function () {
        var prop = this.propComponents;
        if (prop && prop.length > 0) {
            // until first element returns true
            return _.some(prop.toArray(), function (i) { return i.focus(); });
        }
        return false;
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_3__view_models_domain_object_view_model__["a" /* DomainObjectViewModel */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__view_models_domain_object_view_model__["a" /* DomainObjectViewModel */]) === 'function' && _a) || Object)
    ], PropertiesComponent.prototype, "parent", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__angular_forms__["f" /* FormGroup */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_forms__["f" /* FormGroup */]) === 'function' && _b) || Object)
    ], PropertiesComponent.prototype, "form", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Input"])(), 
        __metadata('design:type', Array)
    ], PropertiesComponent.prototype, "properties", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])(__WEBPACK_IMPORTED_MODULE_2__edit_property_edit_property_component__["a" /* EditPropertyComponent */]), 
        __metadata('design:type', (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _c) || Object)
    ], PropertiesComponent.prototype, "propComponents", void 0);
    PropertiesComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-properties',
            template: __webpack_require__(1365),
            styles: [__webpack_require__(1300)]
        }), 
        __metadata('design:paramtypes', [])
    ], PropertiesComponent);
    return PropertiesComponent;
    var _a, _b, _c;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/properties.component.js.map

/***/ }),

/***/ 600:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__ = __webpack_require__(64);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_router__ = __webpack_require__(45);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__pane_pane__ = __webpack_require__(148);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__user_messages__ = __webpack_require__(30);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RecentComponent; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var RecentComponent = (function (_super) {
    __extends(RecentComponent, _super);
    function RecentComponent(activatedRoute, urlManager, viewModelFactory) {
        var _this = this;
        _super.call(this, activatedRoute, urlManager);
        this.viewModelFactory = viewModelFactory;
        // template API 
        this.title = __WEBPACK_IMPORTED_MODULE_5__user_messages__["_98" /* recentTitle */];
        this.items = function () { return _this.recent.items; };
        this.clearButton = {
            value: __WEBPACK_IMPORTED_MODULE_5__user_messages__["_99" /* clear */],
            doClick: function () { return _this.clear(); },
            show: function () { return true; },
            disabled: function () { return _this.clearDisabled(); },
            tempDisabled: function () { return null; },
            title: function () { return _this.clearDisabled() ? __WEBPACK_IMPORTED_MODULE_5__user_messages__["_100" /* recentDisabledMessage */] : __WEBPACK_IMPORTED_MODULE_5__user_messages__["_101" /* recentMessage */]; },
            accesskey: "c"
        };
    }
    Object.defineProperty(RecentComponent.prototype, "actionHolders", {
        get: function () {
            return [this.clearButton];
        },
        enumerable: true,
        configurable: true
    });
    RecentComponent.prototype.clear = function () {
        this.recent.clear();
    };
    RecentComponent.prototype.clearDisabled = function () {
        return this.recent.items.length === 0 ? true : null;
    };
    RecentComponent.prototype.setup = function (routeData) {
        this.recent = this.viewModelFactory.recentItemsViewModel(this.paneId);
    };
    RecentComponent.prototype.focusOnFirstRow = function (rows) {
        if (rows && rows.first) {
            // until first element returns true
            rows.first.focus();
        }
    };
    RecentComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        this.focusOnFirstRow(this.actionChildren);
        this.actionChildren.changes.subscribe(function (ql) { return _this.focusOnFirstRow(ql); });
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["ViewChildren"])("row"), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["QueryList"]) === 'function' && _a) || Object)
    ], RecentComponent.prototype, "actionChildren", void 0);
    RecentComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Component"])({
            selector: 'nof-recent',
            template: __webpack_require__(1366),
            styles: [__webpack_require__(1301)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__angular_router__["d" /* ActivatedRoute */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__angular_router__["d" /* ActivatedRoute */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_3__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__["a" /* ViewModelFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__view_model_factory_service__["a" /* ViewModelFactoryService */]) === 'function' && _d) || Object])
    ], RecentComponent);
    return RecentComponent;
    var _a, _b, _c, _d;
}(__WEBPACK_IMPORTED_MODULE_4__pane_pane__["a" /* PaneComponent */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/recent.component.js.map

/***/ }),

/***/ 601:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return TypeResultCache; });
var TypeResultCache = (function () {
    function TypeResultCache(context) {
        this.context = context;
        this.resultCache = {};
        this.regexCache = [];
        this.subtypeCache = [];
    }
    TypeResultCache.prototype.addType = function (type, result) {
        this.resultCache[type] = result;
    };
    TypeResultCache.prototype.addMatch = function (matcher, result) {
        this.regexCache.push({ regex: matcher, result: result });
    };
    TypeResultCache.prototype.addSubtype = function (type, result) {
        this.subtypeCache.push({ type: type, result: result });
    };
    TypeResultCache.prototype.setDefault = function (def) {
        this.default = def;
    };
    TypeResultCache.prototype.cacheAndReturn = function (type, result) {
        this.resultCache[type] = result;
        return Promise.resolve(result);
    };
    TypeResultCache.prototype.isSubtypeOf = function (subtype, index, count) {
        var _this = this;
        if (index >= count) {
            return Promise.reject("");
        }
        var entry = this.subtypeCache[index];
        return this.context.isSubTypeOf(subtype, entry.type).then(function (b) { return b ? Promise.resolve(entry.result) : _this.isSubtypeOf(subtype, index + 1, count); });
    };
    TypeResultCache.prototype.isSubtype = function (subtype) {
        var _this = this;
        var subtypeChecks = this.subtypeCache.length;
        if (subtypeChecks > 0) {
            return this.isSubtypeOf(subtype, 0, subtypeChecks)
                .then(function (c) {
                return _this.cacheAndReturn(subtype, c);
            })
                .catch(function () {
                return _this.cacheAndReturn(subtype, _this.default);
            });
        }
        return this.cacheAndReturn(subtype, this.default);
    };
    TypeResultCache.prototype.getResult = function (type) {
        // 1 cache 
        // 2 match regex 
        // 3 match subtype 
        // this is potentially expensive - need to filter out non ref types ASAP
        if (!type || type === "string" || type === "number" || type === "boolean") {
            return Promise.resolve(this.default);
        }
        var cachedEntry = this.resultCache[type];
        if (cachedEntry) {
            return Promise.resolve(cachedEntry);
        }
        for (var _i = 0, _a = this.regexCache; _i < _a.length; _i++) {
            var entry = _a[_i];
            if (entry.regex.test(type)) {
                return this.cacheAndReturn(type, entry.result);
            }
        }
        return this.isSubtype(type);
    };
    return TypeResultCache;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/type-result-cache.js.map

/***/ }),

/***/ 602:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__user_messages__ = __webpack_require__(30);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AttachmentViewModel; });

var AttachmentViewModel = (function () {
    function AttachmentViewModel(link, parent, context, error, onPaneId) {
        var _this = this;
        this.link = link;
        this.parent = parent;
        this.context = context;
        this.error = error;
        this.onPaneId = onPaneId;
        this.downloadFile = function () { return _this.context.getFile(_this.parent, _this.href, _this.mimeType); };
        this.clearCachedFile = function () { return _this.context.clearCachedFile(_this.href); };
        this.displayInline = function () {
            return _this.mimeType === "image/jpeg" ||
                _this.mimeType === "image/gif" ||
                _this.mimeType === "application/octet-stream";
        };
        this.href = link.href();
        this.mimeType = link.type().asString;
        this.title = link.title() || __WEBPACK_IMPORTED_MODULE_0__user_messages__["_94" /* unknownFileTitle */];
    }
    AttachmentViewModel.prototype.setImage = function (setImageOn) {
        var _this = this;
        this.downloadFile().then(function (blob) {
            var reader = new FileReader();
            reader.onloadend = function () {
                if (reader.result) {
                    setImageOn.image = reader.result;
                }
            };
            reader.readAsDataURL(blob);
        }).catch(function (reject) { return _this.error.handleError(reject); });
    };
    return AttachmentViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/attachment-view-model.js.map

/***/ }),

/***/ 603:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__message_view_model__ = __webpack_require__(190);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__ = __webpack_require__(41);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ContributedActionParentViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};





var ContributedActionParentViewModel = (function (_super) {
    __extends(ContributedActionParentViewModel, _super);
    function ContributedActionParentViewModel(context, viewModelFactory, urlManager, error, onPaneId) {
        var _this = this;
        _super.call(this);
        this.context = context;
        this.viewModelFactory = viewModelFactory;
        this.urlManager = urlManager;
        this.error = error;
        this.onPaneId = onPaneId;
        this.allSelected = function () { return __WEBPACK_IMPORTED_MODULE_3_lodash__["every"](_this.items, function (item) { return item.selected; }); };
        this.selectAll = function () {
            var newState = !_this.allSelected();
            _this.setItems(newState);
        };
    }
    ContributedActionParentViewModel.prototype.isLocallyContributed = function (action) {
        return __WEBPACK_IMPORTED_MODULE_3_lodash__["some"](action.parameters(), function (p) { return p.isCollectionContributed(); });
    };
    ContributedActionParentViewModel.prototype.setActions = function (actions, routeData) {
        var _this = this;
        this.actions = __WEBPACK_IMPORTED_MODULE_3_lodash__["map"](actions, function (action) { return _this.viewModelFactory.actionViewModel(action, _this, routeData); });
        this.menuItems = __WEBPACK_IMPORTED_MODULE_4__helpers_view_models__["a" /* createMenuItems */](this.actions);
        __WEBPACK_IMPORTED_MODULE_3_lodash__["forEach"](this.actions, function (a) { return _this.decorate(a); });
    };
    ContributedActionParentViewModel.prototype.collectionContributedActionDecorator = function (actionViewModel) {
        var _this = this;
        var wrappedInvoke = actionViewModel.execute;
        actionViewModel.execute = function (pps, right) {
            var selected = __WEBPACK_IMPORTED_MODULE_3_lodash__["filter"](_this.items, function (i) { return i.selected; });
            var rejectAsNeedSelection = function (action) {
                if (_this.isLocallyContributed(action)) {
                    if (selected.length === 0) {
                        var em = new __WEBPACK_IMPORTED_MODULE_1__models__["w" /* ErrorMap */]({}, 0, __WEBPACK_IMPORTED_MODULE_2__user_messages__["_85" /* noItemsSelected */]);
                        var rp = new __WEBPACK_IMPORTED_MODULE_1__models__["a" /* ErrorWrapper */](__WEBPACK_IMPORTED_MODULE_1__models__["b" /* ErrorCategory */].HttpClientError, __WEBPACK_IMPORTED_MODULE_1__models__["q" /* HttpStatusCode */].UnprocessableEntity, em);
                        return rp;
                    }
                }
                return null;
            };
            var getParms = function (action) {
                var parms = __WEBPACK_IMPORTED_MODULE_3_lodash__["values"](action.parameters());
                var contribParm = __WEBPACK_IMPORTED_MODULE_3_lodash__["find"](parms, function (p) { return p.isCollectionContributed(); });
                if (contribParm) {
                    var parmValue = new __WEBPACK_IMPORTED_MODULE_1__models__["j" /* Value */](__WEBPACK_IMPORTED_MODULE_3_lodash__["map"](selected, function (i) { return i.link; }));
                    var collectionParmVm = _this.viewModelFactory.parameterViewModel(contribParm, parmValue, _this.onPaneId);
                    var allpps = __WEBPACK_IMPORTED_MODULE_3_lodash__["clone"](pps);
                    allpps.push(collectionParmVm);
                    return allpps;
                }
                return pps;
            };
            var detailsPromise = actionViewModel.invokableActionRep
                ? Promise.resolve(actionViewModel.invokableActionRep)
                : _this.context.getActionDetails(actionViewModel.actionRep);
            return detailsPromise.
                then(function (details) {
                var rp = rejectAsNeedSelection(details);
                return rp ? Promise.reject(rp) : wrappedInvoke(getParms(details), right);
            }).
                then(function (result) {
                // clear selected items on void actions 
                _this.clearSelected(result);
                return result;
            });
        };
    };
    ContributedActionParentViewModel.prototype.collectionContributedInvokeDecorator = function (actionViewModel) {
        var _this = this;
        var showDialog = function () {
            return _this.context.getInvokableAction(actionViewModel.actionRep).
                then(function (invokableAction) {
                actionViewModel.makeInvokable(invokableAction);
                var keyCount = __WEBPACK_IMPORTED_MODULE_3_lodash__["keys"](invokableAction.parameters()).length;
                return keyCount > 1 || keyCount === 1 && !__WEBPACK_IMPORTED_MODULE_3_lodash__["toArray"](invokableAction.parameters())[0].isCollectionContributed();
            });
        };
        // make sure not invokable  while waiting for promise to assign correct function 
        actionViewModel.doInvoke = function () { };
        var invokeWithoutDialog = function (right) {
            return actionViewModel.invokeWithoutDialogWithParameters(Promise.resolve([]), right).then(function (actionResult) {
                _this.setMessage(actionResult.shouldExpectResult() ? actionResult.warningsOrMessages() || __WEBPACK_IMPORTED_MODULE_2__user_messages__["_86" /* noResultMessage */] : "");
                // clear selected items on void actions 
                _this.clearSelected(actionResult);
            });
        };
        showDialog().
            then(function (show) { return actionViewModel.doInvoke = show ? actionViewModel.invokeWithDialog : invokeWithoutDialog; }).
            catch(function (reject) { return _this.error.handleError(reject); });
    };
    ContributedActionParentViewModel.prototype.decorate = function (actionViewModel) {
        this.collectionContributedActionDecorator(actionViewModel);
        this.collectionContributedInvokeDecorator(actionViewModel);
    };
    ContributedActionParentViewModel.prototype.setItems = function (newValue) {
        __WEBPACK_IMPORTED_MODULE_3_lodash__["each"](this.items, function (item) { return item.silentSelect(newValue); });
        var id = __WEBPACK_IMPORTED_MODULE_3_lodash__["first"](this.items).id;
        this.urlManager.setAllItemsSelected(newValue, id, this.onPaneId);
    };
    ContributedActionParentViewModel.prototype.clearSelected = function (result) {
        if (result.resultType() === "void") {
            this.setItems(false);
        }
    };
    return ContributedActionParentViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__message_view_model__["a" /* MessageViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/contributed-action-parent-view-model.js.map

/***/ }),

/***/ 604:
/***/ (function(module, exports) {

//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/imenu-holder-view-model.js.map

/***/ }),

/***/ 605:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__choice_view_model__ = __webpack_require__(106);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__models__ = __webpack_require__(15);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return LinkViewModel; });


var LinkViewModel = (function () {
    function LinkViewModel(context, colorService, error, urlManager, configService, link, paneId) {
        var _this = this;
        this.context = context;
        this.colorService = colorService;
        this.error = error;
        this.urlManager = urlManager;
        this.configService = configService;
        this.link = link;
        this.paneId = paneId;
        this.draggableTitle = function () { return _this.title; };
        this.canDropOn = function (targetType) { return _this.context.isSubTypeOf(_this.domainType, targetType); };
        // because may be clicking on menu already open so want to reset focus
        this.doClick = function (right) { return _this.urlManager.setMenu(_this.link.rel().parms[0].value, _this.paneId); };
        this.title = link.title() + __WEBPACK_IMPORTED_MODULE_1__models__["z" /* dirtyMarker */](this.context, this.configService, link.getOid(this.configService.config.keySeparator));
        this.domainType = link.type().domainType;
        // for dropping 
        var value = new __WEBPACK_IMPORTED_MODULE_1__models__["j" /* Value */](link);
        this.value = value.toString();
        this.reference = value.toValueString();
        this.selectedChoice = new __WEBPACK_IMPORTED_MODULE_0__choice_view_model__["a" /* ChoiceViewModel */](value, "");
        this.draggableType = this.domainType;
        this.colorService.toColorNumberFromHref(link.href()).
            then(function (c) { return _this.color = "" + _this.configService.config.linkColor + c; }).
            catch(function (reject) { return _this.error.handleError(reject); });
    }
    return LinkViewModel;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/link-view-model.js.map

/***/ }),

/***/ 606:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__route_data__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__user_messages__ = __webpack_require__(30);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__contributed_action_parent_view_model__ = __webpack_require__(603);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ListViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};





var ListViewModel = (function (_super) {
    __extends(ListViewModel, _super);
    function ListViewModel(colorService, context, viewModelFactory, urlManager, error, loggerService, list, routeData) {
        var _this = this;
        _super.call(this, context, viewModelFactory, urlManager, error, routeData.paneId);
        this.colorService = colorService;
        this.loggerService = loggerService;
        this.routeData = routeData;
        this.name = "item";
        this.recreate = function (page, pageSize) {
            return _this.routeData.objectId
                ? _this.context.getListFromObject(_this.routeData, page, pageSize)
                : _this.context.getListFromMenu(_this.routeData, page, pageSize);
        };
        this.currentPaneData = function () { return _this.urlManager.getRouteData().pane(_this.onPaneId); };
        this.pageOrRecreate = function (newPage, newPageSize, newState) {
            _this.recreate(newPage, newPageSize)
                .then(function (list) {
                _this.urlManager.setListPaging(newPage, newPageSize, newState || _this.routeData.state, _this.onPaneId);
                _this.routeData = _this.currentPaneData();
                _this.reset(list, _this.routeData);
            })
                .catch(function (reject) {
                var display = function (em) { return _this.setMessage(em.invalidReason() || em.warningMessage); };
                _this.error.handleErrorAndDisplayMessages(reject, display);
            });
        };
        this.setPage = function (newPage, newState) {
            _this.pageOrRecreate(newPage, _this.pageSize, newState);
        };
        this.earlierDisabled = function () { return _this.page === 1 || _this.numPages === 1; };
        this.laterDisabled = function () { return _this.page === _this.numPages || _this.numPages === 1; };
        this.pageFirstDisabled = this.earlierDisabled;
        this.pageLastDisabled = this.laterDisabled;
        this.pageNextDisabled = this.laterDisabled;
        this.pagePreviousDisabled = this.earlierDisabled;
        this.updateItems = function (value) {
            _this.items = _this.viewModelFactory.getItems(value, _this.state === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table, _this.routeData, _this);
            var totalCount = _this.listRep.pagination().totalCount;
            var count = _this.items.length;
            _this.size = count;
            if (count > 0) {
                _this.description = function () { return __WEBPACK_IMPORTED_MODULE_3__user_messages__["_90" /* pageMessage */](_this.page, _this.numPages, count, totalCount); };
            }
            else {
                _this.description = function () { return __WEBPACK_IMPORTED_MODULE_3__user_messages__["_91" /* noItemsFound */]; };
            }
        };
        this.hasTableData = function () { return _this.listRep.hasTableData(); };
        this.refresh = function (routeData) {
            _this.routeData = routeData;
            if (_this.state !== routeData.state) {
                if (routeData.state === __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table && !_this.hasTableData()) {
                    _this.recreate(_this.page, _this.pageSize)
                        .then(function (list) {
                        _this.state = list.hasTableData() ? __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table : __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].List;
                        _this.listRep = list;
                        _this.updateItems(list.value());
                    })
                        .catch(function (reject) {
                        _this.error.handleError(reject);
                    });
                }
                else {
                    _this.state = routeData.state;
                    _this.updateItems(_this.listRep.value());
                }
            }
        };
        this.reset = function (list, routeData) {
            _this.listRep = list;
            _this.routeData = routeData;
            _this.id = _this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);
            _this.page = _this.listRep.pagination().page;
            _this.pageSize = _this.listRep.pagination().pageSize;
            _this.numPages = _this.listRep.pagination().numPages;
            _this.state = _this.listRep.hasTableData() ? __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table : __WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].List;
            _this.updateItems(list.value());
        };
        this.toggleActionMenu = function () {
            if (_this.disableActions())
                return;
            _this.urlManager.toggleObjectMenu(_this.onPaneId);
        };
        this.pageNext = function () {
            if (_this.pageNextDisabled())
                return;
            _this.setPage(_this.page < _this.numPages ? _this.page + 1 : _this.page, _this.state);
        };
        this.pagePrevious = function () {
            if (_this.pagePreviousDisabled())
                return;
            _this.setPage(_this.page > 1 ? _this.page - 1 : _this.page, _this.state);
        };
        this.pageFirst = function () {
            if (_this.pageFirstDisabled())
                return;
            _this.setPage(1, _this.state);
        };
        this.pageLast = function () {
            if (_this.pageLastDisabled())
                return;
            _this.setPage(_this.numPages, _this.state);
        };
        this.doSummary = function () {
            _this.urlManager.setListState(__WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Summary, _this.onPaneId);
        };
        this.doList = function () {
            _this.urlManager.setListState(__WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].List, _this.onPaneId);
        };
        this.doTable = function () {
            _this.urlManager.setListState(__WEBPACK_IMPORTED_MODULE_0__route_data__["b" /* CollectionViewState */].Table, _this.onPaneId);
        };
        this.reload = function () {
            _this.context.clearCachedList(_this.onPaneId, _this.routeData.page, _this.routeData.pageSize);
            _this.setPage(_this.page, _this.state);
        };
        this.disableActions = function () { return !_this.actions || _this.actions.length === 0 || !_this.items || _this.items.length === 0; };
        this.actionsTooltip = function () { return __WEBPACK_IMPORTED_MODULE_2__helpers_view_models__["f" /* actionsTooltip */](_this, !!_this.routeData.actionsOpen); };
        this.actionMember = function (id) {
            var actionViewModel = __WEBPACK_IMPORTED_MODULE_1_lodash__["find"](_this.actions, function (a) { return a.actionRep.actionId() === id; });
            if (actionViewModel) {
                return actionViewModel.actionRep;
            }
            _this.loggerService.throw("no actionviewmodel " + id + " on " + _this.id);
        };
        this.showActions = function () {
            return !!_this.currentPaneData().actionsOpen;
        };
        this.reset(list, routeData);
        var actions = this.listRep.actionMembers();
        this.setActions(actions, routeData);
    }
    return ListViewModel;
}(__WEBPACK_IMPORTED_MODULE_4__contributed_action_parent_view_model__["a" /* ContributedActionParentViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/list-view-model.js.map

/***/ }),

/***/ 607:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__message_view_model__ = __webpack_require__(190);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__helpers_view_models__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_lodash__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return MenuViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};



var MenuViewModel = (function (_super) {
    __extends(MenuViewModel, _super);
    function MenuViewModel(viewModelFactory, menuRep, routeData) {
        var _this = this;
        _super.call(this);
        this.viewModelFactory = viewModelFactory;
        this.menuRep = menuRep;
        this.routeData = routeData;
        this.id = menuRep.menuId();
        var actions = menuRep.actionMembers();
        this.title = menuRep.title();
        this.actions = __WEBPACK_IMPORTED_MODULE_2_lodash__["map"](actions, function (action) { return viewModelFactory.actionViewModel(action, _this, routeData); });
        this.menuItems = __WEBPACK_IMPORTED_MODULE_1__helpers_view_models__["a" /* createMenuItems */](this.actions);
    }
    return MenuViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__message_view_model__["a" /* MessageViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/menu-view-model.js.map

/***/ }),

/***/ 608:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__item_view_model__ = __webpack_require__(367);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return RecentItemViewModel; });
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};

var RecentItemViewModel = (function (_super) {
    __extends(RecentItemViewModel, _super);
    function RecentItemViewModel(context, colorService, error, urlManager, configService, link, paneId, clickHandler, viewModelFactory, index, isSelected, friendlyName) {
        _super.call(this, context, colorService, error, urlManager, configService, link, paneId, clickHandler, viewModelFactory, index, isSelected, "");
        this.friendlyName = friendlyName;
    }
    return RecentItemViewModel;
}(__WEBPACK_IMPORTED_MODULE_0__item_view_model__["a" /* ItemViewModel */]));
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/recent-item-view-model.js.map

/***/ }),

/***/ 64:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__models__ = __webpack_require__(15);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__context_service__ = __webpack_require__(26);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__url_manager_service__ = __webpack_require__(22);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__color_service__ = __webpack_require__(254);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__click_handler_service__ = __webpack_require__(253);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__error_service__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__mask_service__ = __webpack_require__(256);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__view_models_attachment_view_model__ = __webpack_require__(602);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__view_models_error_view_model__ = __webpack_require__(1033);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__view_models_link_view_model__ = __webpack_require__(605);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__view_models_item_view_model__ = __webpack_require__(367);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__view_models_recent_item_view_model__ = __webpack_require__(608);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13__view_models_table_row_column_view_model__ = __webpack_require__(1038);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_14__view_models_table_row_view_model__ = __webpack_require__(1039);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_15__view_models_recent_items_view_model__ = __webpack_require__(1037);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_16__view_models_parameter_view_model__ = __webpack_require__(368);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_17__view_models_action_view_model__ = __webpack_require__(1030);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_18__view_models_property_view_model__ = __webpack_require__(369);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_19__view_models_collection_view_model__ = __webpack_require__(366);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_20__view_models_menu_view_model__ = __webpack_require__(607);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_21__view_models_menus_view_model__ = __webpack_require__(1035);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_22_lodash__ = __webpack_require__(16);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_22_lodash___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_22_lodash__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_23__view_models_list_view_model__ = __webpack_require__(606);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_24__view_models_dialog_view_model__ = __webpack_require__(257);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_25__view_models_domain_object_view_model__ = __webpack_require__(258);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_26__view_models_multi_line_dialog_view_model__ = __webpack_require__(1036);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_27__config_service__ = __webpack_require__(21);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_28__logger_service__ = __webpack_require__(87);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_29__view_models_application_properties_view_model__ = __webpack_require__(1031);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_30__cicero_command_factory_service__ = __webpack_require__(363);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_31__cicero_renderer_service__ = __webpack_require__(252);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ViewModelFactoryService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
































var ViewModelFactoryService = (function () {
    function ViewModelFactoryService(context, urlManager, color, error, clickHandler, mask, configService, loggerService, commandFactory, ciceroRenderer) {
        var _this = this;
        this.context = context;
        this.urlManager = urlManager;
        this.color = color;
        this.error = error;
        this.clickHandler = clickHandler;
        this.mask = mask;
        this.configService = configService;
        this.loggerService = loggerService;
        this.commandFactory = commandFactory;
        this.ciceroRenderer = ciceroRenderer;
        this.errorViewModel = function (error) {
            return new __WEBPACK_IMPORTED_MODULE_9__view_models_error_view_model__["a" /* ErrorViewModel */](error);
        };
        this.attachmentViewModel = function (propertyRep, paneId) {
            var link = propertyRep.attachmentLink();
            if (link) {
                var parent = propertyRep.parent;
                return new __WEBPACK_IMPORTED_MODULE_8__view_models_attachment_view_model__["a" /* AttachmentViewModel */](link, parent, _this.context, _this.error, paneId);
            }
            return null;
        };
        this.linkViewModel = function (linkRep, paneId) {
            return new __WEBPACK_IMPORTED_MODULE_10__view_models_link_view_model__["a" /* LinkViewModel */](_this.context, _this.color, _this.error, _this.urlManager, _this.configService, linkRep, paneId);
        };
        this.itemViewModel = function (linkRep, paneId, selected, index, id) {
            return new __WEBPACK_IMPORTED_MODULE_11__view_models_item_view_model__["a" /* ItemViewModel */](_this.context, _this.color, _this.error, _this.urlManager, _this.configService, linkRep, paneId, _this.clickHandler, _this, index, selected, id);
        };
        this.recentItemViewModel = function (obj, linkRep, paneId, selected, index) {
            return new __WEBPACK_IMPORTED_MODULE_12__view_models_recent_item_view_model__["a" /* RecentItemViewModel */](_this.context, _this.color, _this.error, _this.urlManager, _this.configService, linkRep, paneId, _this.clickHandler, _this, index, selected, obj.extensions().friendlyName());
        };
        this.actionViewModel = function (actionRep, vm, routeData) {
            return new __WEBPACK_IMPORTED_MODULE_17__view_models_action_view_model__["a" /* ActionViewModel */](_this, _this.context, _this.urlManager, _this.error, _this.clickHandler, actionRep, vm, routeData);
        };
        this.propertyTableViewModel = function (id, propertyRep) {
            return propertyRep ? new __WEBPACK_IMPORTED_MODULE_13__view_models_table_row_column_view_model__["a" /* TableRowColumnViewModel */](id, propertyRep, _this.mask) : new __WEBPACK_IMPORTED_MODULE_13__view_models_table_row_column_view_model__["a" /* TableRowColumnViewModel */](id);
        };
        this.propertyViewModel = function (propertyRep, id, previousValue, paneId, parentValues) {
            return new __WEBPACK_IMPORTED_MODULE_18__view_models_property_view_model__["a" /* PropertyViewModel */](propertyRep, _this.color, _this.error, _this, _this.context, _this.mask, _this.urlManager, _this.clickHandler, _this.configService, id, previousValue, paneId, parentValues);
        };
        this.dialogViewModel = function (routeData, action, actionViewModel, isRow) {
            return new __WEBPACK_IMPORTED_MODULE_24__view_models_dialog_view_model__["a" /* DialogViewModel */](_this.color, _this.context, _this, _this.urlManager, _this.error, routeData, action, actionViewModel, isRow);
        };
        this.multiLineDialogViewModel = function (routeData, action, holder) {
            return new __WEBPACK_IMPORTED_MODULE_26__view_models_multi_line_dialog_view_model__["a" /* MultiLineDialogViewModel */](_this.color, _this.context, _this, _this.urlManager, _this.error, routeData, action, holder);
        };
        this.domainObjectViewModel = function (obj, routeData, forceReload) {
            var ovm = new __WEBPACK_IMPORTED_MODULE_25__view_models_domain_object_view_model__["a" /* DomainObjectViewModel */](_this.color, _this.context, _this, _this.urlManager, _this.error, _this.configService, obj, routeData, forceReload);
            if (forceReload) {
                ovm.clearCachedFiles();
            }
            return ovm;
        };
        this.listViewModel = function (list, routeData) {
            return new __WEBPACK_IMPORTED_MODULE_23__view_models_list_view_model__["a" /* ListViewModel */](_this.color, _this.context, _this, _this.urlManager, _this.error, _this.loggerService, list, routeData);
        };
        this.parameterViewModel = function (parmRep, previousValue, paneId) {
            return new __WEBPACK_IMPORTED_MODULE_16__view_models_parameter_view_model__["a" /* ParameterViewModel */](parmRep, paneId, _this.color, _this.error, _this.mask, previousValue, _this, _this.context, _this.configService);
        };
        this.collectionViewModel = function (collectionRep, routeData, forceReload) {
            return new __WEBPACK_IMPORTED_MODULE_19__view_models_collection_view_model__["a" /* CollectionViewModel */](_this, _this.color, _this.error, _this.context, _this.urlManager, _this.configService, _this.loggerService, collectionRep, routeData, forceReload);
        };
        this.menuViewModel = function (menuRep, routeData) {
            return new __WEBPACK_IMPORTED_MODULE_20__view_models_menu_view_model__["a" /* MenuViewModel */](_this, menuRep, routeData);
        };
        this.menusViewModel = function (menusRep, routeData) {
            return new __WEBPACK_IMPORTED_MODULE_21__view_models_menus_view_model__["a" /* MenusViewModel */](_this, menusRep, routeData.paneId);
        };
        this.recentItemsViewModel = function (paneId) {
            return new __WEBPACK_IMPORTED_MODULE_15__view_models_recent_items_view_model__["a" /* RecentItemsViewModel */](_this, _this.context, _this.urlManager, paneId);
        };
        this.tableRowViewModel = function (properties, paneId, title) {
            return new __WEBPACK_IMPORTED_MODULE_14__view_models_table_row_view_model__["a" /* TableRowViewModel */](_this, properties, paneId, title);
        };
        this.applicationPropertiesViewModel = function () { return new __WEBPACK_IMPORTED_MODULE_29__view_models_application_properties_view_model__["a" /* ApplicationPropertiesViewModel */](_this.context, _this.error, _this.configService); };
        this.getItems = function (links, tableView, routeData, listViewModel) {
            var collection = listViewModel instanceof __WEBPACK_IMPORTED_MODULE_19__view_models_collection_view_model__["a" /* CollectionViewModel */] ? listViewModel : null;
            var id = collection ? collection.name : "";
            var selectedItems = routeData.selectedCollectionItems[id];
            var items = __WEBPACK_IMPORTED_MODULE_22_lodash__["map"](links, function (link, i) { return _this.itemViewModel(link, routeData.paneId, selectedItems && selectedItems[i], i, id); });
            if (tableView) {
                var getActionExtensions = routeData.objectId ?
                    function () { return _this.context.getActionExtensionsFromObject(routeData.paneId, __WEBPACK_IMPORTED_MODULE_0__models__["d" /* ObjectIdWrapper */].fromObjectId(routeData.objectId, _this.configService.config.keySeparator), routeData.actionId); } :
                    function () { return _this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId); };
                var getExtensions = listViewModel instanceof __WEBPACK_IMPORTED_MODULE_19__view_models_collection_view_model__["a" /* CollectionViewModel */] ? function () { return Promise.resolve(listViewModel.collectionRep.extensions()); } : getActionExtensions;
                // clear existing header 
                listViewModel.header = null;
                if (items.length > 0) {
                    getExtensions().
                        then(function (ext) {
                        __WEBPACK_IMPORTED_MODULE_22_lodash__["forEach"](items, function (itemViewModel) {
                            itemViewModel.tableRowViewModel.conformColumns(ext.tableViewTitle(), ext.tableViewColumns());
                        });
                        if (!listViewModel.header) {
                            var firstItem_1 = items[0].tableRowViewModel;
                            var propertiesHeader = __WEBPACK_IMPORTED_MODULE_22_lodash__["map"](firstItem_1.properties, function (p, i) {
                                var match = __WEBPACK_IMPORTED_MODULE_22_lodash__["find"](items, function (item) { return item.tableRowViewModel.properties[i].title; });
                                return match ? match.tableRowViewModel.properties[i].title : firstItem_1.properties[i].id;
                            });
                            listViewModel.header = firstItem_1.showTitle ? [""].concat(propertiesHeader) : propertiesHeader;
                        }
                    }).
                        catch(function (reject) { return _this.error.handleError(reject); });
                }
            }
            return items;
        };
    }
    ViewModelFactoryService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_7__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__context_service__["a" /* ContextService */]) === 'function' && _a) || Object, (typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_2__url_manager_service__["a" /* UrlManagerService */]) === 'function' && _b) || Object, (typeof (_c = typeof __WEBPACK_IMPORTED_MODULE_3__color_service__["a" /* ColorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_3__color_service__["a" /* ColorService */]) === 'function' && _c) || Object, (typeof (_d = typeof __WEBPACK_IMPORTED_MODULE_5__error_service__["a" /* ErrorService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_5__error_service__["a" /* ErrorService */]) === 'function' && _d) || Object, (typeof (_e = typeof __WEBPACK_IMPORTED_MODULE_4__click_handler_service__["a" /* ClickHandlerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_4__click_handler_service__["a" /* ClickHandlerService */]) === 'function' && _e) || Object, (typeof (_f = typeof __WEBPACK_IMPORTED_MODULE_6__mask_service__["a" /* MaskService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_6__mask_service__["a" /* MaskService */]) === 'function' && _f) || Object, (typeof (_g = typeof __WEBPACK_IMPORTED_MODULE_27__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_27__config_service__["a" /* ConfigService */]) === 'function' && _g) || Object, (typeof (_h = typeof __WEBPACK_IMPORTED_MODULE_28__logger_service__["a" /* LoggerService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_28__logger_service__["a" /* LoggerService */]) === 'function' && _h) || Object, (typeof (_j = typeof __WEBPACK_IMPORTED_MODULE_30__cicero_command_factory_service__["a" /* CiceroCommandFactoryService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_30__cicero_command_factory_service__["a" /* CiceroCommandFactoryService */]) === 'function' && _j) || Object, (typeof (_k = typeof __WEBPACK_IMPORTED_MODULE_31__cicero_renderer_service__["a" /* CiceroRendererService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_31__cicero_renderer_service__["a" /* CiceroRendererService */]) === 'function' && _k) || Object])
    ], ViewModelFactoryService);
    return ViewModelFactoryService;
    var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/view-model-factory.service.js.map

/***/ }),

/***/ 850:
/***/ (function(module, exports) {

module.exports = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAkklEQVR42u3WMRHAMAADsYQ/6LQsNPiN4E+T7/t3hncDCCCAAHREAAFAAB2gF4AO0AtAB+gFoAP0Apg/QgEEEEAAOiKAACCADtALQAfoBaAD9ALQAXoBzB+hAAIIIAAdEUAAEEAH6AWgA/QC0AF6AegAvQDmj1AAAQQQgI4IIAAIoAP0AtABegHoAL0AdIDePMAHKZh/wZQyzlgAAAAASUVORK5CYII="

/***/ }),

/***/ 851:
/***/ (function(module, exports) {

module.exports = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAjElEQVR42u3WwQ3AIBADwdB/0Qkd5IV8wrMFgDWvW+/uKW4BAAAAQHoEAACDAdbu5IDT//++DwAAAAAAAACoDQAAAAAApEcAmAxQfwgBAAAAAAAAxQC3BwAAAAAA0iMATAaoP4QAAAAAAACAYoDbAwAAAAAA6REAkgDpAekApAekA5AekA5AekC6eoAPSqGfcPv6taUAAAAASUVORK5CYII="

/***/ }),

/***/ 854:
/***/ (function(module, exports) {

function webpackEmptyContext(req) {
	throw new Error("Cannot find module '" + req + "'.");
}
webpackEmptyContext.keys = function() { return []; };
webpackEmptyContext.resolve = webpackEmptyContext;
module.exports = webpackEmptyContext;
webpackEmptyContext.id = 854;


/***/ }),

/***/ 855:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser_dynamic__ = __webpack_require__(981);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__environments_environment__ = __webpack_require__(1042);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__app_app_module__ = __webpack_require__(1015);




if (__WEBPACK_IMPORTED_MODULE_2__environments_environment__["a" /* environment */].production) {
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["enableProdMode"])();
}
__webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_platform_browser_dynamic__["a" /* platformBrowserDynamic */])().bootstrapModule(__WEBPACK_IMPORTED_MODULE_3__app_app_module__["a" /* AppModule */]);
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/main.js.map

/***/ }),

/***/ 87:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__config_service__ = __webpack_require__(21);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return LoggerService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var LoggerService = (function () {
    function LoggerService(configService) {
        this.configService = configService;
        this.noop = function (message) {
            var optionalParams = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                optionalParams[_i - 1] = arguments[_i];
            }
        };
        switch (configService.config.logLevel) {
            case ("debug"):
                this.logError = this.logWarn = this.logInfo = this.logDebug = true;
                break;
            case ("info"):
                this.logError = this.logWarn = this.logInfo = true;
                this.logDebug = false;
                break;
            case ("warn"):
                this.logError = this.logWarn = true;
                this.logInfo = this.logDebug = false;
                break;
            case ("none"):
                this.logError = this.logWarn = this.logInfo = this.logDebug = false;
                break;
            case ("error"):
            default:
                this.logError = true;
                this.logWarn = this.logInfo = this.logDebug = false;
                break;
        }
    }
    Object.defineProperty(LoggerService.prototype, "error", {
        get: function () {
            if (this.logError)
                return console.error.bind(console);
            return this.noop;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(LoggerService.prototype, "warn", {
        get: function () {
            if (this.logWarn)
                return console.warn.bind(console);
            return this.noop;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(LoggerService.prototype, "info", {
        get: function () {
            if (this.logInfo)
                return console.info.bind(console);
            return this.noop;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(LoggerService.prototype, "debug", {
        get: function () {
            if (this.logDebug)
                return console.debug.bind(console);
            return this.noop;
        },
        enumerable: true,
        configurable: true
    });
    LoggerService.prototype.throw = function (message) {
        var optionalParams = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            optionalParams[_i - 1] = arguments[_i];
        }
        this.error(message, optionalParams);
        throw new Error(message);
    };
    LoggerService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["Injectable"])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__config_service__["a" /* ConfigService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__config_service__["a" /* ConfigService */]) === 'function' && _a) || Object])
    ], LoggerService);
    return LoggerService;
    var _a;
}());
//# sourceMappingURL=C:/projects/nakedobjectsframework-7b999/Spa2/nakedobjectsspa/src/logger.service.js.map

/***/ })

},[1513]);
//# sourceMappingURL=main.bundle.js.map