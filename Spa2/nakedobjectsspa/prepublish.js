var cpx = require('cpx');
var replace = require('replace-in-file');

var copyTemplates = "./src/**/*.{html,css,png,eot,svg,ttf,woff,txt}";
var copyBoilerPlate = "./src/app/app*.ts";
var copyConfig = "./src/config.json";
var copyToLib = "lib";
var copyToApp = "lib/app";

cpx.copySync(copyTemplates, copyToLib);
cpx.copySync(copyConfig, copyToLib);
cpx.copySync(copyBoilerPlate, copyToApp);

options = {
    files: ["./lib/app/app-routing.module.ts", "./lib/app/app.module.ts"],
    from: [/\.\/.*\/.*\.component/g, /\.\/.*\.(service|directive|handler)/g, /\.\/route-data/g],
    to : "nakedobjects.spa"
};

try {
    let changedFiles = replace.sync(options);
    console.log('Modified files:', changedFiles.join(', '));
}
catch (error) {
    console.error('Error occurred:', error);
}
