module NakedObjects.Gemini.Test.Helpers {
    import PaneRouteData = Angular.Gemini.PaneRouteData;
    import INakedObjectsScope = Angular.INakedObjectsScope;
    import IHomePageRepresentation = NakedObjects.RoInterfaces.IHomePageRepresentation;
    import IListRepresentation = NakedObjects.RoInterfaces.IListRepresentation;
    import MenusViewModel = NakedObjects.Angular.Gemini.MenusViewModel;
    import IDomainObjectRepresentation = NakedObjects.RoInterfaces.IDomainObjectRepresentation;
    import IMenuRepresentation = NakedObjects.RoInterfaces.Custom.IMenuRepresentation;

    const homeRepresentation: IHomePageRepresentation = {
        links: [
            {
                rel: "self",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/homepage\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/"
            },
            {
                rel: "urn:org.restfulobjects:rels/user",
                method: "GET",
                type: "application/json; profile =\"urn:org.restfulobjects:repr-types/user\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/user"
            },
            {
                rel: "urn:org.restfulobjects:rels/services",
                method: "GET",
                type: "application/ json; profile =\"urn:org.restfulobjects:repr-types/list\"; charset=utf-8; x-ro-element-type=\"System.Object\"",
                href: "http://nakedobjectsrodemo.azurewebsites.net/services"
            },
            {
                rel: "urn: org.restfulobjects:rels/menus",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/list\"; charset=utf-8; x-ro-element-type=\"System.Object\"",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus"
            },
            {
                rel: "urn:org.restfulobjects:rels/version",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/version\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/version"
            },
            {
                rel: "urn:org.restfulobjects:rels/domain-types",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/type-list\"; charset= utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types"
            }
        ],
        extensions: {}
    }

    const menusRepresentation: IListRepresentation = {
        links: [
            {
                rel: "self",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/list\"; charset=utf-8; x-ro-element-type=\"System.Object\"",
                href: "http://nakedobjectsrodemo.azurewebsites.net/services"
            },
            {
                rel: "up",
                method: "GET",
                type: "application/ json; profile =\"urn:org.restfulobjects:repr-types/homepage\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/"
            }
        ],
        extensions: {},
        value: [
            {
                title: "Customers",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"CustomerRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/CustomerRepository"
            },
            {
                title: "Orders",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"OrderRepository\"",
                method: "GET",
                type: "application/json; profile =\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/OrderRepository"
            },
            {
                title: "Products",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"ProductRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/ProductRepository"
            },
            {
                title: "Employees",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"EmployeeRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/EmployeeRepository"
            },
            {
                title: "Sales",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"SalesRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/SalesRepository"
            },
            {
                title: "Special Offers",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"SpecialOfferRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/SpecialOfferRepository"
            },
            {
                title: "Contacts",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"PersonRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/PersonRepository"
            },
            {
                title: "Vendors",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"VendorRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/VendorRepository"
            },
            {
                title: "Purchase Orders",
                rel: "urn: org.restfulobjects:rels/menu;menuId=\"PurchaseOrderRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/PurchaseOrderRepository"
            },
            {
                title: "Work Orders",
                rel: "urn:org.restfulobjects:rels/menu;menuId=\"WorkOrderRepository\"",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/WorkOrderRepository"
            }]
    }

    const vendorRepositoryMenuRepresentation : IMenuRepresentation  =  {
        title: "Vendors",
        menuId: "VendorRepository",
        links: [
            {
                rel: "self",
                method: "GET",
                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                href: "http://nakedobjectsrodemo.azurewebsites.net/menus/VendorRepository"
            }
        ],
        extensions: {},
        members: {
            AllVendorsWithWebAddresses: {
                parameters: {},
                memberType: "action",
                id: "AllVendorsWithWebAddresses",
                links: [
                    {
                        rel: "urn:org.restfulobjects:rels/details; action=\"AllVendorsWithWebAddresses\"",
                        method: "GET",
                        type: "application/json; profile =\"urn:org.restfulobjects:repr-types/object-action\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/services/AdventureWorksModel.VendorRepository/actions/AllVendorsWithWebAddresses"
                    },
                    {
                        arguments: {},
                        rel: "urn:org.restfulobjects:rels/invoke;action=\"AllVendorsWithWebAddresses\"",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-result\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/services/AdventureWorksModel.VendorRepository/actions/AllVendorsWithWebAddresses/invoke"
                    },
                    {
                        rel: "urn: org.restfulobjects:rels/return-type",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/domain-type\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/list"
                    },
                    {
                        rel: "urn:org.restfulobjects:rels/element-type",
                        method: "GET",
                        type: "application/ json; profile =\"urn:org.restfulobjects:repr-types/domain-type\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.Vendor"
                    },
                    {
                        rel: "describedby",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-description\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.VendorRepository/actions/AllVendorsWithWebAddresses"
                    }
                ],
                extensions: {
                    friendlyName: "All Vendors With Web Addresses",
                    description: "",
                    hasParams: false,
                    memberOrder: 0,
                    returnType: "list",
                    elementType: "AdventureWorksModel.Vendor",
                    pluralName: "Vendors"
                }
            },
            FindVendorByAccountNumber: {
                parameters: {
                    accountNumber: {
                        links: [
                            {
                                rel: "describedby",
                                method: "GET",
                                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-param-description\"; charset=utf-8",
                                href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.VendorRepository/actions/FindVendorByAccountNumber/params/accountNumber"
                            }
                        ],
                        extensions: {
                            friendlyName: "Account Number",
                            description: "",
                            optional: false,
                            returnType: "string",
                            format: "string",
                            maxLength: 0,
                            pattern: ""
                        }
                    }
                },
                memberType: "action",
                id: "FindVendorByAccountNumber",
                links: [
                    {
                        rel: "urn:org.restfulobjects:rels/details;action=\"FindVendorByAccountNumber\"",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/object-action\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/services/AdventureWorksModel.VendorRepository/actions/FindVendorByAccountNumber"
                    },
                    {
                        arguments: { accountNumber: { value: null } },
                        rel: "urn:org.restfulobjects:rels/invoke;action =\"FindVendorByAccountNumber\"",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-result\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/services/AdventureWorksModel.VendorRepository/actions/FindVendorByAccountNumber/invoke"
                    },
                    {
                        rel: "urn:org.restfulobjects:rels/return-type",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/domain-type\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.Vendor"
                    },
                    {
                        rel: "describedby",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-description\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.VendorRepository/actions/FindVendorByAccountNumber"
                    },
                    {
                        id: "accountNumber",
                        rel: "urn:org.restfulobjects:rels/action-param",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-param-description\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.VendorRepository/actions/FindVendorByAccountNumber/params/accountNumber"
                    }
                ],
                extensions: {
                    friendlyName: "Find Vendor By Account Number",
                    description: "",
                    hasParams: true,
                    memberOrder: 0,
                    returnType: "AdventureWorksModel.Vendor"
                }
            },
            FindVendorByName: {
                parameters: {
                    name: {
                        links: [
                            {
                                rel: "describedby",
                                method: "GET",
                                type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-param-description\"; charset=utf-8",
                                href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.VendorRepository/actions/FindVendorByName/params/name"
                            }
                        ],
                        extensions: {
                            friendlyName: "Name",
                            description: "",
                            optional: false,
                            returnType: "string",
                            format: "string",
                            maxLength: 0,
                            pattern: ""
                        }
                    }
                },
                memberType: "action",
                id: "FindVendorByName",
                links: [
                    {
                        rel: "urn:org.restfulobjects:rels/details;action=\"FindVendorByName\"",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/object-action\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/services/AdventureWorksModel.VendorRepository/actions/FindVendorByName"
                    },
                    {
                        arguments: { name: { value: null } },
                        rel: "urn:org.restfulobjects:rels/invoke;action=\"FindVendorByName\"",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-result \"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/services/AdventureWorksModel.VendorRepository/actions/FindVendorByName/invoke"
                    },
                    {
                        rel: "urn:org.restfulobjects:rels/return-type",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/domain-type\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/list"
                    },
                    {
                        rel: "urn:org.restfulobjects:rels/element-type",
                        method: "GET",
                        type: "application/ json; profile=\"urn:org.restfulobjects:repr-types/domain-type\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.Vendor"
                    },
                    {
                        rel: "describedby",
                        method: "GET",
                        type: "application/ json; profile=\"urn:org.restfulobjects:repr-types/action-description\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.VendorRepository/actions/FindVendorByName"
                    },
                    {
                        id: "name",
                        rel: "urn:org.restfulobjects:rels/action-param",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-param-description\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.VendorRepository/actions/FindVendorByName/params/name"
                    }
                ],
                extensions: {
                    friendlyName: "Find Vendor By Name",
                    description: "",
                    hasParams: true,
                    memberOrder: 0,
                    returnType: "list",
                    elementType: "AdventureWorksModel.Vendor",
                    pluralName: "Vendors"
                }
            },
            RandomVendor: {
                parameters: {},
                memberType: "action",
                id: "RandomVendor",
                links: [
                    {
                        rel: "urn:org.restfulobjects:rels/details;action=\"RandomVendor\"",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/object-action\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/services/AdventureWorksModel.VendorRepository/actions/RandomVendor"
                    },
                    {
                        arguments: {},
                        rel: "urn:org.restfulobjects:rels/invoke;action=\"RandomVendor\"",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-result\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/services/AdventureWorksModel.VendorRepository/actions/RandomVendor/invoke"
                    },
                    {
                        rel: "urn:org.restfulobjects:rels/return-type",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/domain-type\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.Vendor"
                    },
                    {
                        rel: "describedby",
                        method: "GET",
                        type: "application/json; profile=\"urn:org.restfulobjects:repr-types/action-description\"; charset=utf-8",
                        href: "http://nakedobjectsrodemo.azurewebsites.net/domain-types/AdventureWorksModel.VendorRepository/actions/RandomVendor"
                    }
                ],
                extensions: {
                    friendlyName: "Random Vendor",
                    description: "",
                    hasParams: false,
                    memberOrder: 0,
                    returnType: "AdventureWorksModel.Vendor"
                }
            }
        }
    }

    let homeRequestHandler: ng.mock.IRequestHandler;
    let menusRequestHandler: ng.mock.IRequestHandler;
    let vendorRepositoryMenuRequestHandler: ng.mock.IRequestHandler;

    export function setupBackend($httpBackend: ng.IHttpBackendService) {
        // backend definition common for all tests
        const root = "http://nakedobjectsrodemo.azurewebsites.net";
        homeRequestHandler = $httpBackend.when("GET", root);
        homeRequestHandler.respond(homeRepresentation);
        menusRequestHandler = $httpBackend.when("GET", root + "/menus");
        menusRequestHandler.respond(menusRepresentation);
        vendorRepositoryMenuRequestHandler = $httpBackend.when("GET", root + "/menus/VendorRepository");
        vendorRepositoryMenuRequestHandler.respond(vendorRepositoryMenuRepresentation);
    }

}