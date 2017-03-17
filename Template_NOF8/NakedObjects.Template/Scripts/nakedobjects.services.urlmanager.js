/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var Decompress = NakedObjects.Models.decompress;
    var Compress = NakedObjects.Models.compress;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var HttpStatusCode = NakedObjects.Models.HttpStatusCode;
    var DomainObjectRepresentation = NakedObjects.Models.DomainObjectRepresentation;
    var Value = NakedObjects.Models.Value;
    var MenuRepresentation = NakedObjects.Models.MenuRepresentation;
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    var propertyIdFromUrl = NakedObjects.Models.propertyIdFromUrl;
    var getOtherPane = NakedObjects.Models.getOtherPane;
    NakedObjects.app.service("urlManager", function ($routeParams, $location, $window) {
        var helper = this;
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
        var capturedPanes = [];
        var currentPaneId = 1;
        function createSubMask(arr) {
            var nMask = 0;
            var nFlag = 0;
            if (arr.length > 31) {
                throw new TypeError("createSubMask - out of range");
            }
            var nLen = arr.length;
            for (nFlag; nFlag < nLen; nMask |= arr[nFlag] << nFlag++)
                ;
            return nMask;
        }
        // convert from array of bools to mask string
        function createArrays(arr, arrays) {
            arrays = arrays || [];
            if (arr.length > 31) {
                arrays.push(arr.slice(0, 31));
                return createArrays(arr.slice(31), arrays);
            }
            arrays.push(arr);
            return arrays;
        }
        function createMask(arr) {
            // split into smaller arrays if necessary 
            var arrays = createArrays(arr);
            var masks = _.map(arrays, function (a) { return createSubMask(a).toString(); });
            return _.reduce(masks, function (res, val) { return res + "-" + val; });
        }
        // convert from mask string to array of bools
        function arrayFromSubMask(sMask) {
            var nMask = parseInt(sMask);
            // nMask must be between 0 and 2147483647 - to keep simple we stick to 31 bits 
            if (nMask > 0x7fffffff || nMask < -0x80000000) {
                throw new TypeError("arrayFromMask - out of range");
            }
            var aFromMask = [];
            var len = 31; // make array always 31 bit long as we may concat another on end
            for (var nShifted = nMask; len > 0; aFromMask.push(Boolean(nShifted & 1)), nShifted >>>= 1, --len)
                ;
            return aFromMask;
        }
        function arrayFromMask(sMask) {
            sMask = sMask || "0";
            var sMasks = sMask.split("-");
            var maskArrays = _.map(sMasks, function (s) { return arrayFromSubMask(s); });
            return _.reduce(maskArrays, function (res, val) { return res.concat(val); }, []);
        }
        function getSearch() {
            return $location.search();
        }
        function setNewSearch(search) {
            $location.search(search);
        }
        function getIds(typeOfId, paneId) {
            return _.pickBy($routeParams, function (v, k) { return k.indexOf(typeOfId + paneId) === 0; });
        }
        function mapIds(ids) {
            return _.mapKeys(ids, function (v, k) { return k.substr(k.indexOf("_") + 1); });
        }
        function getAndMapIds(typeOfId, paneId) {
            var ids = getIds(typeOfId, paneId);
            return mapIds(ids);
        }
        function getMappedValues(mappedIds) {
            return _.mapValues(mappedIds, function (v) { return Value.fromJsonString(v); });
        }
        function getInteractionMode(rawInteractionMode) {
            return rawInteractionMode ? NakedObjects.InteractionMode[rawInteractionMode] : NakedObjects.InteractionMode.View;
        }
        function setPaneRouteData(paneRouteData, paneId) {
            paneRouteData.menuId = getId(akm.menu + paneId, $routeParams);
            paneRouteData.actionId = getId(akm.action + paneId, $routeParams);
            paneRouteData.dialogId = getId(akm.dialog + paneId, $routeParams);
            var rawErrorCategory = getId(akm.errorCat + paneId, $routeParams);
            paneRouteData.errorCategory = rawErrorCategory ? ErrorCategory[rawErrorCategory] : null;
            paneRouteData.objectId = getId(akm.object + paneId, $routeParams);
            paneRouteData.actionsOpen = getId(akm.actions + paneId, $routeParams);
            var rawCollectionState = getId(akm.collection + paneId, $routeParams);
            paneRouteData.state = rawCollectionState ? NakedObjects.CollectionViewState[rawCollectionState] : NakedObjects.CollectionViewState.List;
            var rawInteractionMode = getId(akm.interactionMode + paneId, $routeParams);
            paneRouteData.interactionMode = getInteractionMode(rawInteractionMode);
            var collKeyMap = getAndMapIds(akm.collection, paneId);
            paneRouteData.collections = _.mapValues(collKeyMap, function (v) { return NakedObjects.CollectionViewState[v]; });
            var collSelectedKeyMap = getAndMapIds(akm.selected, paneId);
            paneRouteData.selectedCollectionItems = _.mapValues(collSelectedKeyMap, function (v) { return arrayFromMask(v); });
            var parmKeyMap = getAndMapIds(akm.parm, paneId);
            paneRouteData.actionParams = getMappedValues(parmKeyMap);
            paneRouteData.page = parseInt(getId(akm.page + paneId, $routeParams));
            paneRouteData.pageSize = parseInt(getId(akm.pageSize + paneId, $routeParams));
            paneRouteData.attachmentId = getId(akm.attachment + paneId, $routeParams);
            paneRouteData.validate($location.url());
        }
        function singlePane() {
            return $location.path().split("/").length <= 3;
        }
        function searchKeysForPane(search, paneId, raw) {
            var ids = _.map(raw, function (s) { return s + paneId; });
            return _.filter(_.keys(search), function (k) { return _.some(ids, function (id) { return k.indexOf(id) === 0; }); });
        }
        function allSearchKeysForPane(search, paneId) {
            var raw = _.values(akm);
            return searchKeysForPane(search, paneId, raw);
        }
        function clearPane(search, paneId) {
            var toClear = allSearchKeysForPane(search, paneId);
            // always add reload flag 
            toClear.push(akm.reload);
            return _.omit(search, toClear);
        }
        function clearSearchKeys(search, paneId, keys) {
            var toClear = searchKeysForPane(search, paneId, keys);
            return _.omit(search, toClear);
        }
        function paneIsAlwaysSingle(paneType) {
            return paneType === NakedObjects.multiLineDialogPath;
        }
        function setupPaneNumberAndTypes(pane, newPaneType, newMode) {
            var path = $location.path();
            var segments = path.split("/");
            var mode = segments[1], pane1Type = segments[2], pane2Type = segments[3];
            var changeMode = false;
            var mayReplace = true;
            if (newMode) {
                var newModeString = newMode.toString().toLowerCase();
                changeMode = mode !== newModeString;
                mode = newModeString;
            }
            // changing item on pane 1
            // make sure pane is of correct type
            if (pane === 1 && pane1Type !== newPaneType) {
                var single = singlePane() || paneIsAlwaysSingle(newPaneType);
                var newPath = "/" + mode + "/" + newPaneType + (single ? "" : "/" + pane2Type);
                changeMode = false;
                mayReplace = false;
                $location.path(newPath);
            }
            // changing item on pane 2
            // either single pane so need to add new pane of appropriate type
            // or double pane with second pane of wrong type. 
            if (pane === 2 && (singlePane() || pane2Type !== newPaneType)) {
                var newPath = "/" + mode + "/" + pane1Type + "/" + newPaneType;
                changeMode = false;
                mayReplace = false;
                $location.path(newPath);
            }
            if (changeMode) {
                var newPath = "/" + mode + "/" + pane1Type + "/" + pane2Type;
                $location.path(newPath);
                mayReplace = false;
            }
            return mayReplace;
        }
        function capturePane(paneId) {
            var search = getSearch();
            var toCapture = allSearchKeysForPane(search, paneId);
            return _.pick(search, toCapture);
        }
        function getOidFromHref(href) {
            var oid = ObjectIdWrapper.fromHref(href);
            return oid.getKey();
        }
        function getPidFromHref(href) {
            return propertyIdFromUrl(href);
        }
        function setValue(paneId, search, p, pv, valueType) {
            setId("" + valueType + paneId + "_" + p.id(), pv.toJsonString(), search);
        }
        function setParameter(paneId, search, p, pv) {
            setValue(paneId, search, p, pv, akm.parm);
        }
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
            Transition[Transition["ToMultiLineDialog"] = 13] = "ToMultiLineDialog";
        })(Transition || (Transition = {}));
        function getId(key, search) {
            return Decompress(search[key]);
        }
        function setId(key, id, search) {
            search[key] = Compress(id);
        }
        function clearId(key, search) {
            delete search[key];
        }
        function validKeysForHome() {
            return [akm.menu, akm.dialog, akm.reload];
        }
        function validKeysForObject() {
            return [akm.object, akm.interactionMode, akm.reload, akm.actions, akm.dialog, akm.collection, akm.prop, akm.selected];
        }
        function validKeysForMultiLineDialog() {
            return [akm.object, akm.dialog, akm.menu];
        }
        function validKeysForList() {
            return [akm.reload, akm.actions, akm.dialog, akm.menu, akm.action, akm.page, akm.pageSize, akm.selected, akm.collection, akm.parm, akm.object];
        }
        function validKeysForAttachment() {
            return [akm.object, akm.attachment];
        }
        function validKeys(path) {
            switch (path) {
                case NakedObjects.homePath:
                    return validKeysForHome();
                case NakedObjects.objectPath:
                    return validKeysForObject();
                case NakedObjects.listPath:
                    return validKeysForList();
                case NakedObjects.multiLineDialogPath:
                    return validKeysForMultiLineDialog();
                case NakedObjects.attachmentPath:
                    return validKeysForAttachment();
            }
            return [];
        }
        function clearInvalidParmsFromSearch(paneId, search, path) {
            if (path) {
                var vks = validKeys(path);
                var ivks = _.without.apply(_, [_.values(akm)].concat(vks));
                return clearSearchKeys(search, paneId, ivks);
            }
            return search;
        }
        function handleTransition(paneId, search, transition) {
            var replace = true;
            switch (transition) {
                case (Transition.ToHome):
                    replace = setupPaneNumberAndTypes(paneId, NakedObjects.homePath);
                    search = clearPane(search, paneId);
                    break;
                case (Transition.ToMenu):
                    search = clearPane(search, paneId);
                    break;
                case (Transition.FromDialog):
                    replace = true;
                    break;
                case (Transition.ToDialog):
                case (Transition.FromDialogKeepHistory):
                    replace = false;
                    break;
                case (Transition.ToObjectView):
                    replace = false;
                    setupPaneNumberAndTypes(paneId, NakedObjects.objectPath);
                    search = clearPane(search, paneId);
                    setId(akm.interactionMode + paneId, NakedObjects.InteractionMode[NakedObjects.InteractionMode.View], search);
                    search = toggleReloadFlag(search);
                    break;
                case (Transition.ToList):
                    replace = setupPaneNumberAndTypes(paneId, NakedObjects.listPath);
                    clearId(akm.menu + paneId, search);
                    clearId(akm.object + paneId, search);
                    clearId(akm.dialog + paneId, search);
                    break;
                case (Transition.LeaveEdit):
                    search = clearSearchKeys(search, paneId, [akm.prop]);
                    break;
                case (Transition.Page):
                    replace = false;
                    break;
                case (Transition.ToTransient):
                    replace = false;
                    break;
                case (Transition.ToRecent):
                    replace = setupPaneNumberAndTypes(paneId, NakedObjects.recentPath);
                    search = clearPane(search, paneId);
                    break;
                case (Transition.ToAttachment):
                    replace = setupPaneNumberAndTypes(paneId, NakedObjects.attachmentPath);
                    search = clearPane(search, paneId);
                    break;
                case (Transition.ToMultiLineDialog):
                    replace = setupPaneNumberAndTypes(1, NakedObjects.multiLineDialogPath); // always on 1
                    if (paneId === 2) {
                        // came from 2 
                        search = swapSearchIds(search);
                    }
                    search = clearPane(search, 2); // always on pane 1
                    break;
                default:
                    // null transition 
                    break;
            }
            if (replace) {
                $location.replace();
            }
            var path = $location.path();
            var segments = path.split("/");
            var pane1Type = segments[2], pane2Type = segments[3];
            search = clearInvalidParmsFromSearch(1, search, pane1Type);
            search = clearInvalidParmsFromSearch(2, search, pane2Type);
            return search;
        }
        function executeTransition(newValues, paneId, transition, condition) {
            currentPaneId = paneId;
            var search = getSearch();
            if (condition(search)) {
                search = handleTransition(paneId, search, transition);
                _.forEach(newValues, function (v, k) {
                    if (v)
                        setId(k, v, search);
                    else
                        clearId(k, search);
                });
                setNewSearch(search);
            }
        }
        helper.setHome = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            executeTransition({}, paneId, Transition.ToHome, function () { return true; });
        };
        helper.setRecent = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            executeTransition({}, paneId, Transition.ToRecent, function () { return true; });
        };
        helper.setMenu = function (menuId, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.menu + paneId;
            var newValues = _.zipObject([key], [menuId]);
            executeTransition(newValues, paneId, Transition.ToMenu, function (search) { return getId(key, search) !== menuId; });
        };
        helper.setDialog = function (dialogId, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.dialog + paneId;
            var newValues = _.zipObject([key], [dialogId]);
            executeTransition(newValues, paneId, Transition.ToDialog, function (search) { return getId(key, search) !== dialogId; });
        };
        helper.setMultiLineDialog = function (dialogId, paneId) {
            helper.pushUrlState();
            var key = "" + akm.dialog + 1; // always on 1
            var newValues = _.zipObject([key], [dialogId]);
            executeTransition(newValues, paneId, Transition.ToMultiLineDialog, function (search) { return getId(key, search) !== dialogId; });
        };
        helper.setDialogOrMultiLineDialog = function (actionRep, paneId) {
            if (paneId === void 0) { paneId = 1; }
            if (actionRep.extensions().multipleLines()) {
                helper.setMultiLineDialog(actionRep.actionId(), paneId);
            }
            else {
                helper.setDialog(actionRep.actionId(), paneId);
            }
        };
        function closeOrCancelDialog(paneId, transition) {
            var key = "" + akm.dialog + paneId;
            var newValues = _.zipObject([key], [null]);
            executeTransition(newValues, paneId, transition, function () { return true; });
        }
        helper.closeDialogKeepHistory = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            closeOrCancelDialog(paneId, Transition.FromDialogKeepHistory);
        };
        helper.closeDialogReplaceHistory = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            closeOrCancelDialog(paneId, Transition.FromDialog);
        };
        helper.setObject = function (resultObject, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var oid = resultObject.id();
            var key = "" + akm.object + paneId;
            var newValues = _.zipObject([key], [oid]);
            executeTransition(newValues, paneId, Transition.ToObjectView, function () { return true; });
        };
        helper.setList = function (actionMember, parms, fromPaneId, toPaneId) {
            if (fromPaneId === void 0) { fromPaneId = 1; }
            if (toPaneId === void 0) { toPaneId = 1; }
            var newValues = {};
            var parent = actionMember.parent;
            if (parent instanceof DomainObjectRepresentation) {
                newValues[("" + akm.object + toPaneId)] = parent.id();
            }
            if (parent instanceof MenuRepresentation) {
                newValues[("" + akm.menu + toPaneId)] = parent.menuId();
            }
            newValues[("" + akm.action + toPaneId)] = actionMember.actionId();
            newValues[("" + akm.page + toPaneId)] = "1";
            newValues[("" + akm.pageSize + toPaneId)] = NakedObjects.defaultPageSize.toString();
            newValues[("" + akm.selected + toPaneId + "_")] = "0";
            var newState = actionMember.extensions().renderEagerly() ?
                NakedObjects.CollectionViewState[NakedObjects.CollectionViewState.Table] :
                NakedObjects.CollectionViewState[NakedObjects.CollectionViewState.List];
            newValues[("" + akm.collection + toPaneId)] = newState;
            // This will also swap the panes of the field values if we are 
            // right clicking into the other pane.
            _.forEach(parms, function (p, id) { return setId("" + akm.parm + toPaneId + "_" + id, p.toJsonString(), newValues); });
            executeTransition(newValues, toPaneId, Transition.ToList, function () { return true; });
        };
        helper.setProperty = function (propertyMember, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var href = propertyMember.value().link().href();
            var oid = getOidFromHref(href);
            var key = "" + akm.object + paneId;
            var newValues = _.zipObject([key], [oid]);
            executeTransition(newValues, paneId, Transition.ToObjectView, function () { return true; });
        };
        helper.setItem = function (link, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var href = link.href();
            var oid = getOidFromHref(href);
            var key = "" + akm.object + paneId;
            var newValues = _.zipObject([key], [oid]);
            executeTransition(newValues, paneId, Transition.ToObjectView, function () { return true; });
        };
        helper.setAttachment = function (attachmentlink, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var href = attachmentlink.href();
            var okey = "" + akm.object + paneId;
            var akey = "" + akm.attachment + paneId;
            var oid = getOidFromHref(href);
            var pid = getPidFromHref(href);
            var newValues = _.zipObject([okey, akey], [oid, pid]);
            executeTransition(newValues, paneId, Transition.ToAttachment, function () { return true; });
        };
        helper.toggleObjectMenu = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = akm.actions + paneId;
            var actionsId = getSearch()[key] ? null : "open";
            var newValues = _.zipObject([key], [actionsId]);
            executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        function checkAndSetValue(paneId, check, set) {
            currentPaneId = paneId;
            var search = getSearch();
            // only add field if matching dialog or dialog (to catch case when swapping panes) 
            if (check(search)) {
                set(search);
                setNewSearch(search);
                $location.replace();
            }
        }
        helper.setParameterValue = function (actionId, p, pv, paneId) {
            if (paneId === void 0) { paneId = 1; }
            return checkAndSetValue(paneId, function (search) { return getId("" + akm.action + paneId, search) === actionId; }, function (search) { return setParameter(paneId, search, p, pv); });
        };
        helper.setCollectionMemberState = function (collectionMemberId, state, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.collection + paneId + "_" + collectionMemberId;
            var newValues = _.zipObject([key], [NakedObjects.CollectionViewState[state]]);
            executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        helper.setListState = function (state, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.collection + paneId;
            var newValues = _.zipObject([key], [NakedObjects.CollectionViewState[state]]);
            executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        helper.setInteractionMode = function (newMode, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.interactionMode + paneId;
            var currentMode = getInteractionMode(getId(key, $routeParams));
            var transition;
            if (currentMode === NakedObjects.InteractionMode.Edit && newMode !== NakedObjects.InteractionMode.Edit) {
                transition = Transition.LeaveEdit;
            }
            else if (newMode === NakedObjects.InteractionMode.Transient) {
                transition = Transition.ToTransient;
            }
            else {
                transition = Transition.Null;
            }
            var newValues = _.zipObject([key], [NakedObjects.InteractionMode[newMode]]);
            executeTransition(newValues, paneId, transition, function () { return true; });
        };
        helper.setItemSelected = function (item, isSelected, collectionId, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var key = "" + akm.selected + paneId + "_" + collectionId;
            var currentSelected = getSearch()[key];
            var selectedArray = arrayFromMask(currentSelected);
            selectedArray[item] = isSelected;
            var currentSelectedAsString = (createMask(selectedArray)).toString();
            var newValues = _.zipObject([key], [currentSelectedAsString]);
            executeTransition(newValues, paneId, Transition.Null, function () { return true; });
        };
        helper.setListPaging = function (newPage, newPageSize, state, paneId) {
            if (paneId === void 0) { paneId = 1; }
            var pageValues = {};
            pageValues[("" + akm.page + paneId)] = newPage.toString();
            pageValues[("" + akm.pageSize + paneId)] = newPageSize.toString();
            pageValues[("" + akm.collection + paneId)] = NakedObjects.CollectionViewState[state];
            pageValues[("" + akm.selected + paneId + "_")] = "0"; // clear selection 
            executeTransition(pageValues, paneId, Transition.Page, function () { return true; });
        };
        helper.setError = function (errorCategory, ec) {
            var path = $location.path();
            var segments = path.split("/");
            var mode = segments[1];
            var newPath = "/" + mode + "/error";
            var search = {};
            // always on pane 1
            search[akm.errorCat + 1] = ErrorCategory[errorCategory];
            $location.path(newPath);
            setNewSearch(search);
            if (errorCategory === ErrorCategory.HttpClientError && ec === HttpStatusCode.PreconditionFailed) {
                // on concurrency fail replace url so we can't just go back
                $location.replace();
            }
        };
        helper.getRouteData = function () {
            var routeData = new NakedObjects.RouteData();
            setPaneRouteData(routeData.pane1, 1);
            setPaneRouteData(routeData.pane2, 2);
            return routeData;
        };
        helper.pushUrlState = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            capturedPanes[paneId] = helper.getUrlState(paneId);
        };
        helper.getUrlState = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            currentPaneId = paneId;
            var path = $location.path();
            var segments = path.split("/");
            var paneType = segments[paneId + 1] || NakedObjects.homePath;
            var paneSearch = capturePane(paneId);
            // clear any dialogs so we don't return  to a dialog
            paneSearch = _.omit(paneSearch, "" + akm.dialog + paneId);
            return { paneType: paneType, search: paneSearch };
        };
        helper.getListCacheIndex = function (paneId, newPage, newPageSize, format) {
            var search = getSearch();
            var s1 = getId("" + akm.menu + paneId, search) || "";
            var s2 = getId("" + akm.object + paneId, search) || "";
            var s3 = getId("" + akm.action + paneId, search) || "";
            var parms = _.pickBy(search, function (v, k) { return k.indexOf(akm.parm + paneId) === 0; });
            var mappedParms = _.mapValues(parms, function (v) { return decodeURIComponent(Decompress(v)); });
            var s4 = _.reduce(mappedParms, function (r, n, k) { return r + (k + "=" + n + NakedObjects.keySeparator); }, "");
            var s5 = "" + newPage;
            var s6 = "" + newPageSize;
            var s7 = format ? "" + format : "";
            var ss = [s1, s2, s3, s4, s5, s6, s7];
            return _.reduce(ss, function (r, n) { return r + NakedObjects.keySeparator + n; }, "");
        };
        helper.popUrlState = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            currentPaneId = paneId;
            var capturedPane = capturedPanes[paneId];
            var mayReplace = true;
            if (capturedPane) {
                capturedPanes[paneId] = null;
                var search = clearPane(getSearch(), paneId);
                search = _.merge(search, capturedPane.search);
                mayReplace = setupPaneNumberAndTypes(paneId, capturedPane.paneType);
                setNewSearch(search);
            }
            else {
                // probably reloaded page so no state to pop. 
                // just go home 
                helper.setHome(paneId);
            }
            if (mayReplace) {
                $location.replace();
            }
        };
        helper.clearUrlState = function (paneId) {
            currentPaneId = paneId;
            capturedPanes[paneId] = null;
        };
        function swapSearchIds(search) {
            return _.mapKeys(search, function (v, k) { return k.replace(/(\D+)(\d{1})(\w*)/, function (match, p1, p2, p3) { return ("" + p1 + (p2 === "1" ? "2" : "1") + p3); }); });
        }
        helper.swapPanes = function () {
            var path = $location.path();
            var segments = path.split("/");
            var mode = segments[1], oldPane1 = segments[2], _a = segments[3], oldPane2 = _a === void 0 ? NakedObjects.homePath : _a;
            var newPath = "/" + mode + "/" + oldPane2 + "/" + oldPane1;
            var search = swapSearchIds(getSearch());
            currentPaneId = getOtherPane(currentPaneId);
            $location.path(newPath).search(search);
        };
        helper.cicero = function () {
            var newPath = "/" + NakedObjects.ciceroPath + "/" + $location.path().split("/")[2];
            $location.path(newPath);
        };
        helper.applicationProperties = function () {
            $location.path("/" + NakedObjects.geminiPath + "/" + NakedObjects.applicationPropertiesPath);
        };
        helper.currentpane = function () { return currentPaneId; };
        helper.singlePane = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            currentPaneId = 1;
            if (!singlePane()) {
                var paneToKeepId = paneId;
                var paneToRemoveId = getOtherPane(paneToKeepId);
                var path = $location.path();
                var segments = path.split("/");
                var mode = segments[1];
                var paneToKeep = segments[paneToKeepId + 1];
                var newPath = "/" + mode + "/" + paneToKeep;
                var search = getSearch();
                if (paneToKeepId === 1) {
                    // just remove second pane
                    search = clearPane(search, paneToRemoveId);
                }
                if (paneToKeepId === 2) {
                    // swap pane 2 to pane 1 then remove 2
                    search = swapSearchIds(search);
                    search = clearPane(search, 2);
                }
                $location.path(newPath).search(search);
            }
        };
        helper.reload = function () {
            $window.location.reload(true);
        };
        function isLocation(paneId, location) {
            var path = $location.path();
            var segments = path.split("/");
            return segments[paneId + 1] === location; // e.g. segments 0=~/1=cicero/2=home/3=home
        }
        ;
        helper.isHome = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return isLocation(paneId, NakedObjects.homePath);
        };
        helper.isObject = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return isLocation(paneId, NakedObjects.objectPath);
        };
        helper.isList = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return isLocation(paneId, NakedObjects.listPath);
        };
        helper.isError = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return isLocation(paneId, NakedObjects.errorPath);
        };
        helper.isRecent = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return isLocation(paneId, NakedObjects.recentPath);
        };
        helper.isAttachment = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return isLocation(paneId, NakedObjects.attachmentPath);
        };
        helper.isApplicationProperties = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return isLocation(paneId, NakedObjects.applicationPropertiesPath);
        };
        helper.isMultiLineDialog = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            return isLocation(paneId, NakedObjects.multiLineDialogPath);
        };
        function toggleReloadFlag(search) {
            var currentFlag = search[akm.reload];
            var newFlag = currentFlag ? 0 : 1;
            search[akm.reload] = newFlag;
            return search;
        }
        helper.triggerPageReloadByFlippingReloadFlagInUrl = function () {
            var search = getSearch();
            setNewSearch(toggleReloadFlag(search));
            $location.replace();
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.urlmanager.js.map