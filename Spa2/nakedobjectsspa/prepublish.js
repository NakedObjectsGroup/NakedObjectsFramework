var cpx = require('cpx');

var copyTemplates = "./src/**/*.{html,css,png}";
var copyBoilerPlate = "./src/app/app*.ts";
var copyConfig = "./src/config.json";
var copyToLib = "lib";
var copyToApp = "lib/app";

cpx.copy(copyTemplates, copyToLib);
cpx.copy(copyConfig, copyToLib);
cpx.copy(copyBoilerPlate, copyToApp);