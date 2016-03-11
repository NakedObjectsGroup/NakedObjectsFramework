// Karma configuration
// Generated on Tue Jun 03 2014 11:24:37 GMT+0100 (GMT Daylight Time)

module.exports = function(config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: "",


    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ["jasmine"],

      
    // list of files / patterns to load in the browser
    files: [
      "Scripts/jquery-2.1.4.js",
      "Scripts/lodash.js",
      "Scripts/angular.js",
      "Scripts/angular-route.js",
      "Scripts/angular-touch.js",
      "Scripts/angular-mocks.js",
      "Scripts/nakedobjects.config.js",
      "Scripts/nakedobjects.userMessages.config.js",
      "Scripts/nakedobjects.constants.js",
      "Scripts/nakedobjects.models.helpers.js",
      "Scripts/nakedobjects.models.js",
      "Scripts/nakedobjects.viewmodels.js",
      "Scripts/nakedobjects.routedata.js",
      "Scripts/nakedobjects.app.js",
      "Scripts/nakedobjects.services.color.js",
      "Scripts/nakedobjects.services.color.config.js",
      "Scripts/nakedobjects.services.mask.js",
      "Scripts/nakedobjects.services.representationloader.js",
      "Scripts/nakedobjects.controllers.js",
      "Scripts/nakedobjects.directives.js",
      "Scripts/nakedobjects.services.viewmodelfactory.js",
      "Scripts/nakedobjects.services.context.js",
      "Scripts/nakedobjects.services.handlers.js",
      "Scripts/nakedobjects.services.urlmanager.js",
      "Scripts/nakedobjects.services.focusmanager.js",
      "Scripts/nakedobjects.services.navigation.browser.js",
      "Scripts/nakedobjects.services.clickhandler.js",
      "Scripts/nakedobjects.cicerocommands.js",
      "Scripts/nakedobjects.cicerocommandFactory.js",
      "Tests/specs/nakedobjects.test.helpers.js",
      "Tests/specs/nakedobjects.test.js",
      "Tests/specs/nakedobjects.test.masks.js"
    ],


    // list of files to exclude
    exclude: [
      
    ],


    //preprocess matching files before serving them to the browser
    //available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
        'Scripts/nakedobjects.*.js': ["coverage"]
    },


    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ["progress", "junit", "coverage"],
    
    junitReporter : {
        outputFile: "test-results/karma-test-results.xml"
    },
    
    coverageReporter : {
        dir: "coverage",
        reporters: [  
          { type: "html" },
          { type: "text", file: "text.txt" },
          { type: "text-summary",  file: "text-summary.txt" }
        ]
    },

    // web server port
    port: 9876,

      // cli runner port
    runnerPort : 9100,

    // enable / disable colors in the output (reporters and logs)
    colors: true,

    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,

    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,

    captureTimeout : 60000,

    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ["IE"],

    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: true
  });
};
