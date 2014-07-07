/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.models.ts" />
/// <reference path="../../Scripts/spiro.angular.services.color.ts" />
/// <reference path="../../Scripts/spiro.modern.viewmodels.ts" />
/// <reference path="helpers.ts" />
describe('viewModelFactory Service', function () {
    beforeEach(module('app'));

    describe('create errorViewModel', function () {
        var resultVm;
        var rawError = { message: "a message", stackTrace: ["line1", "line2"] };
        var emptyError = {};

        describe('from populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.errorViewModel(new Spiro.ErrorRepresentation(rawError));
            }));

            it('creates a error view model', function () {
                expect(resultVm.message).toBe("a message");
                expect(resultVm.stackTrace.length).toBe(2);
                expect(resultVm.stackTrace.pop()).toBe("line2");
                expect(resultVm.stackTrace.pop()).toBe("line1");
            });
        });

        describe('from empty rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.errorViewModel(new Spiro.ErrorRepresentation(emptyError));
            }));

            it('creates a error view model', function () {
                expect(resultVm.message).toBe("An Error occurred");
                expect(resultVm.stackTrace.length).toBe(1);
                expect(resultVm.stackTrace.pop()).toBe("Empty");
            });
        });
    });

    describe('create linkViewModel', function () {
        var resultVm;
        var rawLink = { title: "a title", href: "http://objects/AdventureWorksModel.Product/1" };
        var emptyLink = {};

        describe('from populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.linkViewModel(new Spiro.Link(rawLink));
            }));

            it('creates a link view model', function () {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.href).toBe("#/objects/AdventureWorksModel.Product/1");
                expect(resultVm.color).toBe("bg-color-orangeDark");
            });
        });

        describe('from empty rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.linkViewModel(new Spiro.Link(emptyLink));
            }));

            it('creates a link view model', function () {
                expect(resultVm.title).toBeUndefined();
                expect(resultVm.href).toBe("");
                expect(resultVm.color).toBe("bg-color-darkBlue");
            });
        });
    });

    describe('create itemViewModel', function () {
        var resultVm;
        var rawLink = { title: "a title", href: "http://objects/AdventureWorksModel.Product/1" };
        var emptyLink = {};

        describe('from populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.itemViewModel(new Spiro.Link(rawLink), "http://objects/AdventureWorksModel.SalesOrderHeader/1");
            }));

            it('creates an item view model', function () {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.href).toBe("#/objects/AdventureWorksModel.SalesOrderHeader/1?collectionItem=AdventureWorksModel.Product/1");
                expect(resultVm.color).toBe("bg-color-orangeDark");
            });
        });

        describe('from empty rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.itemViewModel(new Spiro.Link(emptyLink), "");
            }));

            it('creates an item view model', function () {
                expect(resultVm.title).toBeUndefined();
                expect(resultVm.href).toBe("");
                expect(resultVm.color).toBe("bg-color-darkBlue");
            });
        });
    });

    describe('create actionViewModel', function () {
        var resultVm;
        var rawdetailsLink = { rel: "urn:org.restfulobjects:rels/details", href: "http://objects/AdventureWorksModel.Product/1/actions/anaction" };
        var rawAction = { extensions: { friendlyName: "a title" }, links: [rawdetailsLink] };

        describe('from populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.actionViewModel(new Spiro.ActionMember(rawAction, {}));
            }));

            it('creates an action view model', function () {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.href).toBe("#/objects/AdventureWorksModel.Product/1?action=anaction");
            });
        });
    });

    describe('create dialogViewModel', function () {
        var resultVm;
        var rawInvokeLink = { rel: "urn:org.restfulobjects:rels/invoke", href: "http://objects/AdventureWorksModel.Product/1/actions/anaction" };
        var rawUpLink = { rel: "urn:org.restfulobjects:rels/up", href: "http://objects/AdventureWorksModel.Product/1" };

        var rawAction = { extensions: { friendlyName: "a title" }, links: [rawInvokeLink, rawUpLink] };

        describe('from simple rep', function () {
            beforeEach(inject(function (viewModelFactory, $routeParams) {
                $routeParams.action = "";
                resultVm = viewModelFactory.dialogViewModel(new Spiro.ActionRepresentation(rawAction), function () {
                });
            }));

            it('creates a dialog view model', function () {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.isQuery).toBe(false);
                expect(resultVm.message).toBe("");
                expect(resultVm.close).toBe("#/objects/AdventureWorksModel.Product/1");
                expect(resultVm.parameters.length).toBe(0);
                expect(resultVm.doShow).toBeTruthy();
                expect(resultVm.doInvoke).toBeTruthy();
            });
        });
    });

    describe('create collectionViewModel', function () {
        var resultVm;
        var rawDetailsLink = { rel: "urn:org.restfulobjects:rels/details", href: "http://objects/AdventureWorksModel.Product/1/collections/acollection" };
        var rawSelfLink = { rel: "urn:org.restfulobjects:rels/self", href: "http://objects/AdventureWorksModel.Product/1/collections/acollection" };

        var rawCollection = { size: 0, extensions: { friendlyName: "a title", pluralName: "somethings", elementType: "AdventureWorksModel.Product" }, links: [rawDetailsLink] };

        describe('from collection member rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.collectionViewModel(new Spiro.CollectionMember(rawCollection, {}));
            }));

            it('creates a dialog view model', function () {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.size).toBe(0);
                expect(resultVm.href).toBe("#/objects/AdventureWorksModel.Product/1?collection=acollection");
                expect(resultVm.color).toBe("bg-color-orangeDark");
                expect(resultVm.items.length).toBe(0);
                expect(resultVm.pluralName).toBe("somethings");
            });
        });

        describe('from collection details rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                rawCollection.value = [];
                rawCollection.links.push(rawSelfLink);

                resultVm = viewModelFactory.collectionViewModel(new Spiro.CollectionRepresentation(rawCollection));
            }));

            it('creates a dialog view model', function () {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.size).toBe(0);
                expect(resultVm.href).toBe("#/objects/AdventureWorksModel.Product/1?collection=acollection");
                expect(resultVm.color).toBe("bg-color-orangeDark");
                expect(resultVm.items.length).toBe(0);
                expect(resultVm.pluralName).toBe("somethings");
            });
        });

        describe('from list rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                var rawList = { value: [], links: [rawSelfLink] };

                resultVm = viewModelFactory.collectionViewModel(new Spiro.ListRepresentation(rawList));
            }));

            it('creates a dialog view model', function () {
                expect(resultVm.size).toBe(0);
                expect(resultVm.items.length).toBe(0);
                expect(resultVm.pluralName).toBe("Objects");
            });
        });
    });

    describe("create services view model", function () {
        var resultVm;
        var rawServices = { value: [] };

        describe('from populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.servicesViewModel(new Spiro.DomainServicesRepresentation(rawServices));
            }));

            it('creates a services view model', function () {
                expect(resultVm.title).toBe("Services");
                expect(resultVm.color).toBe("bg-color-darkBlue");
                expect(resultVm.items.length).toBe(0);
            });
        });
    });

    describe("create service view model", function () {
        var resultVm;
        var rawSelfLink = { rel: "urn:org.restfulobjects:rels/self", href: "http://services/AdventureWorksModel.ProductRepository" };

        var rawService = { serviceId: "a service", value: [], links: [rawSelfLink], title: "a title" };

        describe('from populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.serviceViewModel(new Spiro.DomainObjectRepresentation(rawService));
            }));

            it('creates a service view model', function () {
                expect(resultVm.serviceId).toBe("a service");
                expect(resultVm.title).toBe("a title");
                expect(resultVm.actions.length).toBe(0);
                expect(resultVm.color).toBe("bg-color-greenLight");
                expect(resultVm.href).toBe("#/services/AdventureWorksModel.ProductRepository");
            });
        });
    });

    describe("create object view model", function () {
        var resultVm;
        var rawSelfLink = { rel: "urn:org.restfulobjects:rels/self", href: "http://objects/AdventureWorksModel.Product/1" };

        var rawObject = { domainType: "an object", links: [rawSelfLink], title: "a title", extensions: { friendlyName: "a name" } };

        describe('from populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.domainObjectViewModel(new Spiro.DomainObjectRepresentation(rawObject));
            }));

            it('creates a object view model', function () {
                expect(resultVm.domainType).toBe("an object");
                expect(resultVm.title).toBe("a title");
                expect(resultVm.actions.length).toBe(0);
                expect(resultVm.properties.length).toBe(0);
                expect(resultVm.collections.length).toBe(0);
                expect(resultVm.color).toBe("bg-color-red");
                expect(resultVm.href).toBe("#/objects/AdventureWorksModel.Product/1");
                expect(resultVm.cancelEdit).toBe("#/objects/AdventureWorksModel.Product/1");
            });
        });

        describe('from transient populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                var rawPersistLink = { rel: "urn:org.restfulobjects:rels/persist", href: "http://objects/AdventureWorksModel.Product" };
                rawObject.links.pop();
                rawObject.links.push(rawPersistLink);
                var doRep = new Spiro.DomainObjectRepresentation(rawObject);
                doRep.hateoasUrl = "http://objects/AdventureWorksModel.Product";

                resultVm = viewModelFactory.domainObjectViewModel(doRep);
            }));

            it('creates a object view model', function () {
                expect(resultVm.domainType).toBe("an object");
                expect(resultVm.title).toBe("Unsaved a name");
                expect(resultVm.actions.length).toBe(0);
                expect(resultVm.properties.length).toBe(0);
                expect(resultVm.collections.length).toBe(0);
                expect(resultVm.color).toBe("bg-color-red");
                expect(resultVm.href).toBe("#/objects/AdventureWorksModel.Product");
                expect(resultVm.cancelEdit).toBe("");
            });
        });
    });

    describe("create parameter view model", function () {
        var resultVm;

        var rawParameter = { extensions: { friendlyName: "a parm" }, links: [] };
        var rawAction = {};

        describe('from populated rep', function () {
            beforeEach(inject(function (viewModelFactory) {
                resultVm = viewModelFactory.parameterViewModel(new Spiro.Parameter(rawParameter, new Spiro.ActionRepresentation(rawAction)), "", "pv");
            }));

            it('creates a parameter view model', function () {
                expect(resultVm.type).toBe("ref");
                expect(resultVm.title).toBe("a parm");
                expect(resultVm.dflt).toBe("");
                expect(resultVm.message).toBe("");
                expect(resultVm.mask).toBeUndefined();
                expect(resultVm.id).toBe("");
                expect(resultVm.argId).toBe("");
                expect(resultVm.returnType).toBeUndefined();
                expect(resultVm.format).toBeUndefined();
                expect(resultVm.reference).toBe("");

                expect(resultVm.choices.length).toBe(0);
                expect(resultVm.hasChoices).toBe(false);
                expect(resultVm.hasPrompt).toBe(false);
                expect(resultVm.hasConditionalChoices).toBe(false);
                expect(resultVm.isMultipleChoices).toBe(false);
                expect(resultVm.value).toBe("pv");
            });
        });

        describe('from populated rep with scalar choices', function () {
            beforeEach(inject(function (viewModelFactory) {
                rawParameter.choices = [1, 2, 3];
                rawParameter.default = 1;

                resultVm = viewModelFactory.parameterViewModel(new Spiro.Parameter(rawParameter, new Spiro.ActionRepresentation(rawAction)), "", "");
            }));

            it('creates a parameter view model with choices', function () {
                expect(resultVm.choices.length).toBe(3);
                expect(resultVm.hasChoices).toBe(true);
                expect(resultVm.hasPrompt).toBe(false);
                expect(resultVm.hasConditionalChoices).toBe(false);
                expect(resultVm.isMultipleChoices).toBe(false);
                expect(resultVm.choice.value).toBe("1");
                expect(resultVm.value).toBeUndefined();
            });
        });

        describe('from populated rep with prompt autocomplete', function () {
            beforeEach(inject(function (viewModelFactory) {
                var rawPromptLink = {
                    rel: "urn:org.restfulobjects:rels/prompt",
                    href: "http://services/AdventureWorksModel.ProductRepository/prompt",
                    arguments: { "x-ro-searchTerm": { value: null } },
                    extensions: { minLength: 0 },
                    type: 'application/json; profile = "urn:org.restfulobjects:repr-types/prompt"'
                };

                rawParameter.choices = null;
                rawParameter.default = 1;
                rawParameter.links.push(rawPromptLink);

                resultVm = viewModelFactory.parameterViewModel(new Spiro.Parameter(rawParameter, new Spiro.ActionRepresentation(rawAction)), "", "");
            }));

            it('creates a parameter view model with prompt', function () {
                expect(resultVm.choices.length).toBe(0);
                expect(resultVm.hasChoices).toBe(false);
                expect(resultVm.hasPrompt).toBe(true);
                expect(resultVm.hasConditionalChoices).toBe(false);
                expect(resultVm.isMultipleChoices).toBe(false);
                expect(resultVm.choice.value).toBe("1");
                expect(resultVm.value).toBeUndefined();
            });
        });

        describe('from populated rep with prompt conditional choices', function () {
            beforeEach(inject(function (viewModelFactory) {
                var rawPromptLink = {
                    rel: "urn:org.restfulobjects:rels/prompt",
                    href: "http://services/AdventureWorksModel.ProductRepository/prompt",
                    arguments: { "parm": { value: null } },
                    extensions: { minLength: 0 },
                    type: 'application/json; profile = "urn:org.restfulobjects:repr-types/prompt"'
                };

                rawParameter.choices = null;
                rawParameter.default = 1;
                rawParameter.links.pop();
                rawParameter.links.push(rawPromptLink);

                resultVm = viewModelFactory.parameterViewModel(new Spiro.Parameter(rawParameter, new Spiro.ActionRepresentation(rawAction)), "", "");
            }));

            it('creates a parameter view model with prompt', function () {
                expect(resultVm.choices.length).toBe(0);
                expect(resultVm.hasChoices).toBe(false);
                expect(resultVm.hasPrompt).toBe(false);
                expect(resultVm.hasConditionalChoices).toBe(true);
                expect(resultVm.isMultipleChoices).toBe(false);
                expect(resultVm.choice.value).toBe("1");
                expect(resultVm.value).toBeUndefined();
            });
        });
    });
});
//# sourceMappingURL=viewModelFactoryService.js.map
