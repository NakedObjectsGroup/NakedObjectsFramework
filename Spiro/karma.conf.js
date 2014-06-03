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

        'Spiro.Modern/Scripts/underscore.js',
  'Spiro.Modern/Scripts/angular.js',
  'Spiro.Modern/Scripts/angular-route.js',
  'Spiro.Modern/Scripts/angular-touch.js',
  'Spiro.Modern/Scripts/angular-mocks.js',
  'Spiro.Modern/Scripts/spiro.config.js',
  'Spiro.Modern/Scripts/spiro.models.helpers.js',
  'Spiro.Modern/Scripts/spiro.models.shims.js',
  'Spiro.Modern/Scripts/spiro.models.js',
  'Spiro.Modern/Scripts/spiro.angular.config.js',
  'Spiro.Modern/Scripts/spiro.angular.viewmodels.js',
  'Spiro.Modern/Scripts/spiro.angular.app.js',
  'Spiro.Modern/Scripts/spiro.angular.controllers.js',
  'Spiro.Modern/Scripts/spiro.angular.directives.js',
  'Spiro.Modern/Scripts/spiro.angular.services.representationloader.js',
  'Spiro.Modern/Scripts/spiro.angular.services.representationhandlers.js',
  'Spiro.Modern/Scripts/spiro.angular.services.viewmodelfactory.js',
  'Spiro.Modern/Scripts/spiro.angular.services.urlhelper.js',
  'Spiro.Modern/Scripts/spiro.angular.services.context.js',
  'Spiro.Modern/Scripts/spiro.angular.services.handlers.js',
  'Spiro.Modern/Scripts/spiro.angular.services.color.js',
  'Spiro.Modern/Scripts/spiro.angular.services.mask.js',
  'Spiro.Modern/Scripts/spiro.angular.services.navigation.simple.js',


  'Spiro.Modern/Tests/specs/controllers.js',
  'Spiro.Modern/Tests/specs/services.js'
      
    ],


    // list of files to exclude
    exclude: [
      
    ],


    // preprocess matching files before serving them to the browser
    // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
        'Spiro.Modern/Scripts/spiro.angular*.*.js': 'coverage'
    },


    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress', 'junit', 'coverage'],

    
    junitReporter : {
        outputFile: '../test-results/karma-test-results.xml'
    },
    
    coverageReporter : {
        type: 'cobertura',
        dir: '../coverage/'
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


    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['Firefox'],

    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: true
  });
};
