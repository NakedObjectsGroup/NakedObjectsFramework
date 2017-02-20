var cpx = require('cpx');
var replace = require('replace-in-file');
var mv = require('mv');

var copyTemplates = "./src/**/*.{html,css,png,eot,svg,ttf,woff,txt}";
var copyBoilerPlate = "./src/app/app*.ts";
var copyConfig = "./src/empty_config.json";
var copyToLib = "lib";
var copyToApp = "lib/app";

cpx.copySync(copyTemplates, copyToLib);
cpx.copySync(copyConfig, copyToLib);
cpx.copySync(copyBoilerPlate, copyToApp);

mv("./lib/empty_config.json", "./lib/config.json", { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });

var options = {
    files: ["./lib/app/app-routing.module.ts", "./lib/app/app.module.ts"],
    from: [/\.\/.*\/.*\.component/g, /\.\/.*\.(service|directive|handler)/g, /\.\/route-data/g],
    to : "nakedobjects.spa"
};

try {
    var changedFiles = replace.sync(options);
    //console.log('Modified files:', changedFiles.join(', '));
}
catch (error) {
    console.error('Error occurred:', error);
}
