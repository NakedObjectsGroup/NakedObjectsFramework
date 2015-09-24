// Karma configuration
// Generated on Tue Jun 03 2014 11:24:37 GMT+0100 (GMT Daylight Time)

module.exports = function(config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: '',


    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ['jasmine'],

      
    // list of files / patterns to load in the browser
    files: [
      'Scripts/jquery-2.1.4.js',
      'Scripts/lodash.js',
      'Scripts/angular.js',
      'Scripts/angular-route.js',
      'Scripts/angular-touch.js',
      'Scripts/angular-mocks.js',
      'Scripts/nakedobjects.config.js',
      'Scripts/nakedobjects.models.helpers.js',
      'Scripts/nakedobjects.models.shims.js',
      'Scripts/nakedobjects.models.js',
      'Scripts/nakedobjects.gemini.viewmodels.js',
      'Scripts/nakedobjects.gemini.routedata.js',
      'Scripts/nakedobjects.gemini.app.js',
      'Scripts/nakedobjects.angular.services.color.js',
      'Scripts/nakedobjects.angular.services.mask.js',
      'Scripts/nakedobjects.angular.services.representationloader.js',
      'Scripts/nakedobjects.gemini.controllers.js',
      'Scripts/nakedobjects.gemini.directives.js',
      'Scripts/nakedobjects.gemini.services.viewmodelfactory.js',
      'Scripts/nakedobjects.gemini.services.context.js',
      'Scripts/nakedobjects.gemini.services.handlers.js',
      'Scripts/nakedobjects.gemini.services.urlmanager.js',
      'Scripts/nakedobjects.gemini.services.navigation.browser.js',
      'Tests/specs/nakedobjects.gemini.controllers.test.js',
      'Tests/specs/nakedobjects.gemini.services.context.test.js',
      'Tests/specs/nakedobjects.gemini.services.handlers.test.js',
      'Tests/specs/nakedobjects.gemini.services.urlmanager.test.js',
      'Tests/specs/nakedobjects.gemini.services.viewmodelfactory.test.js'
    ],


    // list of files to exclude
    exclude: [
      
    ],


    // preprocess matching files before serving them to the browser
    // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
        'source/Scripts/nakedobjects*.js': ['coverage']
    },


    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress', 'junit', 'coverage'],

    
    junitReporter : {
        outputFile: 'test-results/karma-test-results.xml'
    },
    
    coverageReporter : {
        dir: 'coverage',
        reporters: [  
          { type: 'html' },
          { type: 'text', file: 'text.txt' },
          { type: 'text-summary',  file: 'text-summary.txt' },
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
    browsers: ['IE'],

    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: true
  });
};
