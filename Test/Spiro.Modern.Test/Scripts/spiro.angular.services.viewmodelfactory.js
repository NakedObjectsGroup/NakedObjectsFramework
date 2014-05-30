/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.representationhandlers.ts" />
/// <reference path="spiro.angular.services.urlhelper.ts" />
/// <reference path="spiro.angular.services.context.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        Spiro.Angular.app.service('viewModelFactory', function ($q, $location, $filter, urlHelper, repLoader, color, context, repHandlers, mask) {
            var viewModelFactory = this;

            viewModelFactory.errorViewModel = function (errorRep) {
                var errorViewModel = new Spiro.Angular.ErrorViewModel();
                errorViewModel.message = errorRep.message() || "An Error occurred";
                var stackTrace = errorRep.stacktrace();

                errorViewModel.stackTrace = !stackTrace || stackTrace.length === 0 ? ["Empty"] : stackTrace;
                return errorViewModel;
            };

            viewModelFactory.linkViewModel = function (linkRep) {
                var linkViewModel = new Spiro.Angular.LinkViewModel();
                linkViewModel.title = linkRep.title();
                linkViewModel.href = urlHelper.toAppUrl(linkRep.href());
                linkViewModel.color = color.toColorFromHref(linkRep.href());
                return linkViewModel;
            };

            viewModelFactory.itemViewModel = function (linkRep, parentHref, index) {
                var itemViewModel = new Spiro.Angular.ItemViewModel();
                itemViewModel.title = linkRep.title();
                itemViewModel.href = urlHelper.toItemUrl(parentHref, linkRep.href());
                itemViewModel.color = color.toColorFromHref(linkRep.href());

                return itemViewModel;
            };

            viewModelFactory.parameterViewModel = function (parmRep, id, previousValue) {
                var parmViewModel = new Spiro.Angular.ParameterViewModel();

                parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";

                parmViewModel.title = parmRep.extensions().friendlyName;
                parmViewModel.dflt = parmRep.default().toValueString();
                parmViewModel.message = "";
                parmViewModel.mask = parmRep.extensions["x-ro-nof-mask"];

                if (parmRep.extensions().returnType === "boolean") {
                    parmViewModel.value = previousValue ? previousValue.toLowerCase() === 'true' : parmRep.default().scalar();
                } else {
                    parmViewModel.value = previousValue || parmViewModel.dflt;
                }

                parmViewModel.id = id;
                parmViewModel.argId = id.toLowerCase();
                parmViewModel.returnType = parmRep.extensions().returnType;
                parmViewModel.format = parmRep.extensions().format;

                parmViewModel.reference = "";

                parmViewModel.choices = _.map(parmRep.choices(), function (v, n) {
                    return Spiro.Angular.ChoiceViewModel.create(v, id, n);
                });

                parmViewModel.hasChoices = parmViewModel.choices.length > 0;
                parmViewModel.isMultipleChoices = parmViewModel.hasChoices && parmRep.extensions().returnType == "list";

                if (parmViewModel.hasChoices && parmViewModel.value) {
                    if (parmViewModel.type == "scalar") {
                        // TODO fix so this works same way for both scalar and ref !
                        parmViewModel.choice = _.find(parmViewModel.choices, function (c) {
                            return c.value === parmViewModel.value;
                        });
                    } else {
                        parmViewModel.choice = _.find(parmViewModel.choices, function (c) {
                            return c.name === parmViewModel.value;
                        });
                    }
                }

                parmViewModel.hasPrompt = !!parmRep.promptLink() && parmRep.promptLink().arguments()["x-ro-searchTerm"];
                parmViewModel.hasConditionalChoices = !!parmRep.promptLink() && !parmViewModel.hasPrompt;

                var promptRep;
                if (parmViewModel.hasPrompt) {
                    promptRep = parmRep.getPrompts();
                    parmViewModel.prompt = _.partial(repHandlers.prompt, promptRep, id);

                    // TODO and at least investigate if we can do all choices/prompt caching the same way !!
                    if (previousValue) {
                        parmViewModel.choice = context.getSelectedChoice(id, previousValue);
                    } else if (parmViewModel.dflt) {
                        var dflt = parmRep.default();
                        parmViewModel.choice = Spiro.Angular.ChoiceViewModel.create(dflt, parmViewModel.id, dflt.link().title());
                    } else {
                        // clear any previous
                        context.clearSelectedChoice();
                    }
                }

                if (parmViewModel.hasConditionalChoices) {
                    promptRep = parmRep.getPrompts();
                    parmViewModel.conditionalChoices = _.partial(repHandlers.conditionalChoices, promptRep, id);

                    parmViewModel.arguments = _.object(_.map(parmRep.promptLink().arguments(), function (v, key) {
                        return [key, new Spiro.Value(v.value)];
                    }));

                    // TODO and at least investigate if we can do all choices/prompt caching the same way !!
                    if (previousValue) {
                        parmViewModel.choice = context.getSelectedChoice(id, previousValue);
                    } else {
                        // clear any previous
                        context.clearSelectedChoice();
                    }
                }

                var remoteMask = parmRep.extensions()["x-ro-nof-mask"];

                if (remoteMask && parmRep.isScalar()) {
                    var localFilter = mask.toLocalFilter(remoteMask);
                    if (localFilter) {
                        parmViewModel.value = $filter(localFilter.name)(parmViewModel.value, localFilter.mask);
                    }
                }

                return parmViewModel;
            };

            viewModelFactory.actionViewModel = function (actionRep) {
                var actionViewModel = new Spiro.Angular.ActionViewModel();
                actionViewModel.title = actionRep.extensions().friendlyName;
                actionViewModel.href = urlHelper.toActionUrl(actionRep.detailsLink().href());
                return actionViewModel;
            };

            viewModelFactory.dialogViewModel = function (actionRep, invoke) {
                var dialogViewModel = new Spiro.Angular.DialogViewModel();
                var parameters = actionRep.parameters();
                var parms = urlHelper.actionParms();

                dialogViewModel.title = actionRep.extensions().friendlyName;
                dialogViewModel.isQuery = actionRep.invokeLink().method() === "GET";

                dialogViewModel.message = "";

                dialogViewModel.close = urlHelper.toAppUrl(actionRep.upLink().href(), ["action"]);

                var i = 0;
                dialogViewModel.parameters = _.map(parameters, function (parm, id) {
                    return viewModelFactory.parameterViewModel(parm, id, parms[i++]);
                });

                dialogViewModel.doShow = function () {
                    dialogViewModel.show = true;
                    invoke(dialogViewModel);
                };
                dialogViewModel.doInvoke = function () {
                    dialogViewModel.show = false;
                    invoke(dialogViewModel);
                };

                return dialogViewModel;
            };

            viewModelFactory.propertyViewModel = function (propertyRep, id, propertyDetails) {
                var propertyViewModel = new Spiro.Angular.PropertyViewModel();
                propertyViewModel.title = propertyRep.extensions().friendlyName;
                propertyViewModel.value = propertyRep.isScalar() ? propertyRep.value().scalar() : propertyRep.value().toString();
                propertyViewModel.type = propertyRep.isScalar() ? "scalar" : "ref";
                propertyViewModel.returnType = propertyRep.extensions().returnType;
                propertyViewModel.format = propertyRep.extensions().format;
                propertyViewModel.href = propertyRep.isScalar() || propertyRep.detailsLink() == null ? "" : urlHelper.toPropertyUrl(propertyRep.detailsLink().href());
                propertyViewModel.target = propertyRep.isScalar() || propertyRep.value().isNull() ? "" : urlHelper.toAppUrl(propertyRep.value().link().href());
                propertyViewModel.reference = propertyRep.isScalar() || propertyRep.value().isNull() ? "" : propertyRep.value().link().href();

                if (propertyRep.attachmentLink() != null) {
                    propertyViewModel.attachment = Spiro.Angular.AttachmentViewModel.create(propertyRep.attachmentLink().href(), propertyRep.attachmentLink().type().asString, propertyRep.attachmentLink().title());
                }

                // only set color if has value
                propertyViewModel.color = propertyViewModel.value ? color.toColorFromType(propertyRep.extensions().returnType) : "";

                propertyViewModel.id = id;
                propertyViewModel.argId = id.toLowerCase();
                propertyViewModel.isEditable = !propertyRep.disabledReason();
                propertyViewModel.choices = [];
                propertyViewModel.hasPrompt = propertyRep.hasPrompt();

                if (propertyDetails && propertyRep.hasChoices()) {
                    propertyViewModel.choices = _.map(propertyDetails.choices(), function (v, n) {
                        return Spiro.Angular.ChoiceViewModel.create(v, id, n);
                    });
                }

                propertyViewModel.hasChoices = propertyViewModel.choices.length > 0;

                if (propertyViewModel.hasChoices) {
                    propertyViewModel.choice = _.find(propertyViewModel.choices, function (c) {
                        return c.name === (propertyViewModel.value ? propertyViewModel.value.toString() : "");
                    });
                    if (propertyViewModel.choice) {
                        propertyViewModel.value = propertyViewModel.choice.name;
                    }
                } else if (propertyViewModel.type === "ref") {
                    propertyViewModel.choice = Spiro.Angular.ChoiceViewModel.create(propertyRep.value(), id);
                } else {
                    propertyViewModel.choice = null;
                }

                propertyViewModel.hasPrompt = !!propertyDetails && !!propertyDetails.promptLink() && propertyDetails.promptLink().arguments()["x-ro-searchTerm"];
                propertyViewModel.hasConditionalChoices = !!propertyDetails && !!propertyDetails.promptLink() && !propertyViewModel.hasPrompt;

                if (propertyViewModel.hasPrompt && propertyDetails) {
                    var list = propertyDetails.getPrompts();
                    propertyViewModel.prompt = _.partial(repHandlers.prompt, list, id);
                } else {
                    propertyViewModel.prompt = function (st) {
                        return $q.when([]);
                    };
                }

                if (propertyViewModel.hasConditionalChoices) {
                    var promptRep = propertyDetails.getPrompts();
                    propertyViewModel.conditionalChoices = _.partial(repHandlers.conditionalChoices, promptRep, id);

                    propertyViewModel.arguments = _.object(_.map(propertyDetails.promptLink().arguments(), function (v, key) {
                        return [key, new Spiro.Value(v.value)];
                    }));

                    propertyViewModel.choice = Spiro.Angular.ChoiceViewModel.create(propertyRep.value(), id);
                }

                var remoteMask = propertyRep.extensions()["x-ro-nof-mask"];

                if (remoteMask && propertyRep.isScalar()) {
                    var localFilter = mask.toLocalFilter(remoteMask);
                    if (localFilter) {
                        propertyViewModel.value = $filter(localFilter.name)(propertyViewModel.value, localFilter.mask);
                    }
                }

                // if a reference and no way to set (ie not choices or autocomplete) set editable to false
                if (propertyViewModel.type == "ref" && !propertyViewModel.hasPrompt && !propertyViewModel.hasChoices && !propertyViewModel.hasConditionalChoices) {
                    propertyViewModel.isEditable = false;
                }

                return propertyViewModel;
            };

            function create(collectionRep) {
                var collectionViewModel = new Spiro.Angular.CollectionViewModel();

                collectionViewModel.title = collectionRep.extensions().friendlyName;
                collectionViewModel.size = collectionRep.size();
                collectionViewModel.pluralName = collectionRep.extensions().pluralName;

                collectionViewModel.href = collectionRep.detailsLink() ? urlHelper.toCollectionUrl(collectionRep.detailsLink().href()) : "";
                collectionViewModel.color = color.toColorFromType(collectionRep.extensions().elementType);

                collectionViewModel.items = [];

                return collectionViewModel;
            }

            function getItems(cvm, links, href, populateItems) {
                var i = 0;
                if (populateItems) {
                    return _.map(links, function (link) {
                        var ivm = viewModelFactory.itemViewModel(link, href, i++);
                        var tempTgt = link.getTarget();
                        repLoader.populate(tempTgt).then(function (obj) {
                            ivm.target = viewModelFactory.domainObjectViewModel(obj);

                            if (!cvm.header) {
                                cvm.header = _.map(ivm.target.properties, function (property) {
                                    return property.title;
                                });
                            }
                        });
                        return ivm;
                    });
                } else {
                    return _.map(links, function (link) {
                        return viewModelFactory.itemViewModel(link, href, i++);
                    });
                }
            }

            function createFromDetails(collectionRep, populateItems, $scope) {
                var collectionViewModel = new Spiro.Angular.CollectionViewModel();
                var links = collectionRep.value().models;

                collectionViewModel.title = collectionRep.extensions().friendlyName;
                collectionViewModel.size = links.length;
                collectionViewModel.pluralName = collectionRep.extensions().pluralName;

                collectionViewModel.href = urlHelper.toCollectionUrl(collectionRep.selfLink().href());
                collectionViewModel.color = color.toColorFromType(collectionRep.extensions().elementType);

                collectionViewModel.items = getItems(collectionViewModel, links, collectionViewModel.href, populateItems);

                return collectionViewModel;
            }

            function createFromList(listRep, populateItems) {
                var collectionViewModel = new Spiro.Angular.CollectionViewModel();
                var links = listRep.value().models;

                collectionViewModel.size = links.length;
                collectionViewModel.pluralName = "Objects";

                collectionViewModel.items = getItems(collectionViewModel, links, $location.path(), populateItems);

                return collectionViewModel;
            }

            viewModelFactory.collectionViewModel = function (collection, populateItems) {
                if (collection instanceof Spiro.CollectionMember) {
                    return create(collection);
                }
                if (collection instanceof Spiro.CollectionRepresentation) {
                    return createFromDetails(collection, populateItems);
                }
                if (collection instanceof Spiro.ListRepresentation) {
                    return createFromList(collection, populateItems);
                }
                return null;
            };

            viewModelFactory.servicesViewModel = function (servicesRep) {
                var servicesViewModel = new Spiro.Angular.ServicesViewModel();

                // filter out contributed action services
                var links = _.filter(servicesRep.value().models, function (m) {
                    var sid = m.rel().parms[0].split('=')[1];
                    return sid.indexOf("ContributedActions") == -1;
                });

                servicesViewModel.title = "Services";
                servicesViewModel.color = "bg-color-darkBlue";
                servicesViewModel.items = _.map(links, function (link) {
                    return viewModelFactory.linkViewModel(link);
                });
                return servicesViewModel;
            };

            viewModelFactory.serviceViewModel = function (serviceRep) {
                var serviceViewModel = new Spiro.Angular.ServiceViewModel();
                var actions = serviceRep.actionMembers();
                serviceViewModel.serviceId = serviceRep.serviceId();
                serviceViewModel.title = serviceRep.title();
                serviceViewModel.actions = _.map(actions, function (action) {
                    return viewModelFactory.actionViewModel(action);
                });
                serviceViewModel.color = color.toColorFromType(serviceRep.serviceId());
                serviceViewModel.href = urlHelper.toAppUrl(serviceRep.getUrl());

                return serviceViewModel;
            };

            viewModelFactory.domainObjectViewModel = function (objectRep, details, save) {
                var objectViewModel = new Spiro.Angular.DomainObjectViewModel();
                var isTransient = !!objectRep.persistLink();

                objectViewModel.href = urlHelper.toAppUrl(objectRep.getUrl());

                objectViewModel.cancelEdit = isTransient ? "" : urlHelper.toAppUrl(objectRep.getUrl());

                objectViewModel.color = color.toColorFromType(objectRep.domainType());

                objectViewModel.doSave = save ? function () {
                    return save(objectViewModel);
                } : function () {
                };

                var properties = objectRep.propertyMembers();
                var collections = objectRep.collectionMembers();
                var actions = objectRep.actionMembers();

                objectViewModel.domainType = objectRep.domainType();
                objectViewModel.title = isTransient ? "Unsaved " + objectRep.extensions().friendlyName : objectRep.title();

                objectViewModel.message = "";

                objectViewModel.properties = _.map(properties, function (property, id) {
                    return viewModelFactory.propertyViewModel(property, id, _.find(details, function (d) {
                        return d.instanceId() === id;
                    }));
                });
                objectViewModel.collections = _.map(collections, function (collection) {
                    return viewModelFactory.collectionViewModel(collection);
                });
                objectViewModel.actions = _.map(actions, function (action) {
                    return viewModelFactory.actionViewModel(action);
                });

                return objectViewModel;
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.viewmodelfactory.js.map
