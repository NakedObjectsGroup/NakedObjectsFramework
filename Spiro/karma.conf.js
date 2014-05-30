// Karma configuration
// Generated on Thu Jul 25 2013 16:24:24 GMT+0100 (GMT Daylight Time)


// base path, that will be used to resolve files and exclude
basePath = '';


// list of files / patterns to load in the browser
files = [
  JASMINE,
  JASMINE_ADAPTER,
  
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

];


// list of files to exclude
exclude = [
  
];

preprocessors = {
  'Spiro.Modern/Scripts/spiro.angular*.*.js': 'coverage'
};


// test results reporter to use
// possible values: 'dots', 'progress', 'junit'
reporters = ['progress', 'junit', 'coverage' ];


coverageReporter = {
    type: 'cobertura',
    dir: 'coverage/'
};

junitReporter = {
    outputFile: 'test-results/karma-test-results.xml'
};

// web server port
port = 9876;


// cli runner port
runnerPort = 9100;


// enable / disable colors in the output (reporters and logs)
colors = true;


// level of logging
// possible values: LOG_DISABLE || LOG_ERROR || LOG_WARN || LOG_INFO || LOG_DEBUG
logLevel = LOG_INFO;


// enable / disable watching file and executing tests whenever any file changes
autoWatch = true;


// Start these browsers, currently available:
// - Chrome
// - ChromeCanary
// - Firefox
// - Opera
// - Safari (only Mac)
// - PhantomJS
// - IE (only Windows)
browsers = ['Firefox'];


// If browser does not capture in given timeout [ms], kill it
captureTimeout = 60000;


// Continuous Integration mode
// if true, it capture browsers, run tests and exit
singleRun = true;
