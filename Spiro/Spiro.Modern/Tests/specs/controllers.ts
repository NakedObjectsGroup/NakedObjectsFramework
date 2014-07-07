/// <reference path="../../Scripts/typings/jasmine/jasmine-1.3.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.modern.app.ts" />
/// <reference path="../../Scripts/spiro.modern.services.handlers.ts" />

describe('Controllers', () => {
	var $scope, ctrl;

	beforeEach(module('app'));

	describe('BackgroundController', () => {
		var handleBackground;

		beforeEach(inject(($rootScope, $controller, handlers) => {
			$scope = $rootScope.$new();
			handleBackground = spyOn(handlers, 'handleBackground');
			ctrl = $controller('BackgroundController', { $scope: $scope, handlers: handlers });
		}));

		it('should call the handler', () => {
			expect(handleBackground).toHaveBeenCalledWith($scope);
		});  
	});

	describe('ServicesController', () => {

		var handleServices;

		beforeEach(inject(($rootScope, $controller, handlers) => {
			$scope = $rootScope.$new();
			handleServices = spyOn(handlers, 'handleServices');
			ctrl = $controller('ServicesController', { $scope: $scope, handlers: handlers });
		}));

		it('should call the handler', () => {
			expect(handleServices).toHaveBeenCalledWith($scope);
		});

	});

	describe('ServiceController', () => {

		var handleService;

		beforeEach(inject(($rootScope, $controller, handlers) => {
			$scope = $rootScope.$new();
			handleService = spyOn(handlers, 'handleService');
			ctrl = $controller('ServiceController', { $scope: $scope, handlers: handlers });
		}));

		it('should call the handler', () => {
			expect(handleService).toHaveBeenCalledWith($scope);
		});

	});

	describe('ObjectController', () => {

		var handleObject;
		var handleEditObject;

		beforeEach(inject(($rootScope, $controller, handlers) => {
			$scope = $rootScope.$new();
			handleObject = spyOn(handlers, 'handleObject');
			handleEditObject = spyOn(handlers, 'handleEditObject');

		}));

		describe('if edit mode', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.editMode = "test";
				ctrl = $controller('ObjectController', { $scope: $scope, handlers: handlers });
			}));

			it('should not call the view handler', () => {
				expect(handleObject).wasNotCalled();
			});

			it('should call the edit handler', () => {
				expect(handleEditObject).toHaveBeenCalledWith($scope);
			});
		});

		describe('if not edit mode', () => {
            
			beforeEach(inject(($controller, handlers) => {
				ctrl = $controller('ObjectController', { $scope: $scope, handlers: handlers });
			}));

			it('should not call the edit handler', () => {
				expect(handleEditObject).wasNotCalled();
			});

			it('should call the view handler', () => {
				expect(handleObject).toHaveBeenCalledWith($scope);
			});

		});




	});


	describe('DialogController', () => {

		var handleActionDialog;

		beforeEach(inject(($rootScope, handlers) => {
			$scope = $rootScope.$new();
			handleActionDialog = spyOn(handlers, 'handleActionDialog');
		}));


		describe('if action parm set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.action = "test";
				ctrl = $controller('DialogController', { $scope: $scope, handlers: handlers });
			}));

			it('should call the handler', () => {
				expect(handleActionDialog).toHaveBeenCalledWith($scope);
			});
		});

		describe('if action parm not set', () => {
            
			beforeEach(inject(($controller, handlers) => {
				ctrl = $controller('DialogController', { $scope: $scope, handlers: handlers });
			}));

			it('should not call the handler', () => {
				expect(handleActionDialog).wasNotCalled();
			});

		});
	});

	describe('NestedObjectController', () => {

		var handleActionResult;
		var handleProperty;
		var handleCollectionItem;
		var handleResult;

		beforeEach(inject(($rootScope, handlers) => {
			$scope = $rootScope.$new();
			handleActionResult = spyOn(handlers, 'handleActionResult');
			handleProperty = spyOn(handlers, 'handleProperty');
			handleCollectionItem = spyOn(handlers, 'handleCollectionItem');
			handleResult = spyOn(handlers, 'handleResult');
		}));


		describe('if action parm set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.action = "test";
				ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
			}));

			it('should call the action handler only', () => {
				expect(handleActionResult).toHaveBeenCalledWith($scope);
				expect(handleProperty).wasNotCalled();
				expect(handleCollectionItem).wasNotCalled();
				expect(handleResult).wasNotCalled();
			});
		});

		describe('if property parm set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.property = "test";
				ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
			}));

			it('should call the property handler only', () => {
				expect(handleActionResult).wasNotCalled();
				expect(handleProperty).toHaveBeenCalledWith($scope);
				expect(handleCollectionItem).wasNotCalled();
				expect(handleResult).wasNotCalled();
			});

		});

		describe('if collection Item parm set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.collectionItem = "test";
				ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
			}));

			it('should call the collection item handler only', () => {
				expect(handleActionResult).wasNotCalled();
				expect(handleProperty).wasNotCalled();
				expect(handleCollectionItem).toHaveBeenCalledWith($scope);
				expect(handleResult).wasNotCalled();
			});

		});

		describe('if result object parm set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.resultObject = "test";
				ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
			}));

			it('should call the result object handler only', () => {
				expect(handleActionResult).wasNotCalled();
				expect(handleProperty).wasNotCalled();
				expect(handleCollectionItem).wasNotCalled();
				expect(handleResult).toHaveBeenCalledWith($scope);
			});
		});

		describe('if all parms set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.action = "test";
				$routeParams.property = "test";
				$routeParams.collectionItem = "test";
				$routeParams.resultObject = "test";
				ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
			}));

			it('should call the action and property handler only', () => {
				expect(handleActionResult).toHaveBeenCalledWith($scope);
				expect(handleProperty).toHaveBeenCalledWith($scope);
				expect(handleCollectionItem).wasNotCalled();
				expect(handleResult).wasNotCalled();
			});
		});

		describe('if no parms set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
			}));

			it('should call no handlers', () => {
				expect(handleActionResult).wasNotCalled();
				expect(handleProperty).wasNotCalled();
				expect(handleCollectionItem).wasNotCalled();
				expect(handleResult).wasNotCalled();
			});
		});

	});

	describe('CollectionController', () => {

		var handleCollectionResult;
		var handleCollection;

		beforeEach(inject(($rootScope, handlers) => {
			$scope = $rootScope.$new();
			handleCollectionResult = spyOn(handlers, 'handleCollectionResult');
			handleCollection = spyOn(handlers, 'handleCollection');
		}));


		describe('if result collection parm set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.resultCollection = "test";
				ctrl = $controller('CollectionController', { $scope: $scope, handlers: handlers });
			}));

			it('should call the result collection handler', () => {
				expect(handleCollectionResult).toHaveBeenCalledWith($scope);
				expect(handleCollection).wasNotCalled();
			});
		});

		describe('if collection parm set', () => {

			beforeEach(inject(($routeParams, $controller, handlers) => {
				$routeParams.collection = "test";
				ctrl = $controller('CollectionController', { $scope: $scope, handlers: handlers });
			}));

			it('should  call the collection handler', () => {
				expect(handleCollectionResult).wasNotCalled();
				expect(handleCollection).toHaveBeenCalledWith($scope);
			});

		});

		describe('if no parms set', () => {

			beforeEach(inject(($controller, handlers) => {
				ctrl = $controller('CollectionController', { $scope: $scope, handlers: handlers });
			}));

			it('should not call the handler', () => {
				expect(handleCollectionResult).wasNotCalled();
				expect(handleCollection).wasNotCalled();
			});

		});

	});

    describe('TransientObjectController', () => {

		var handleTransientObject;

		beforeEach(inject(($rootScope, $controller, handlers) => {
			$scope = $rootScope.$new();
			handleTransientObject = spyOn(handlers, 'handleTransientObject');
            ctrl = $controller('TransientObjectController', { $scope: $scope, handlers: handlers });
		}));

		it('should call the handler', () => {
			expect(handleTransientObject).toHaveBeenCalledWith($scope);
		});

    });

    describe('ErrorController', () => {

        var handleError;

        beforeEach(inject(($rootScope, $controller, handlers) => {
            $scope = $rootScope.$new();
            handleError = spyOn(handlers, 'handleError');
            ctrl = $controller('ErrorController', { $scope: $scope, handlers: handlers });
        }));

        it('should call the handler', () => {
            expect(handleError).toHaveBeenCalledWith($scope);
        });

    });


	describe('AppBarController', () => {

		var handleAppBar;

		beforeEach(inject(($rootScope, $controller, handlers) => {
			$scope = $rootScope.$new();
			handleAppBar = spyOn(handlers, 'handleAppBar');
			ctrl = $controller('AppBarController', { $scope: $scope, handlers: handlers });
		}));

		it('should call the handler', () => {
			expect(handleAppBar).toHaveBeenCalledWith($scope);
		});

	});

});