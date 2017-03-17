var cpx = require('cpx');
var replace = require('replace-in-file');
var find = require('find-in-files');
var mv = require('mv');

var copyTemplates = "./src/**/*.{html,css,png,eot,svg,ttf,woff,txt}";
var copyBoilerPlate = "./src/app/app*.ts";
var copyConfig = "./src/empty_config.json";
var copyTsConfig = "./src/tsconfig.app.json";
var copyToLib = "lib";
var copyToApp = "lib/app";

cpx.copySync(copyTemplates, copyToLib);
cpx.copySync(copyConfig, copyToLib);
cpx.copySync(copyTsConfig, copyToLib);
cpx.copySync(copyBoilerPlate, copyToApp);

mv("./lib/empty_config.json", "./lib/config.json", { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });

// get current version 

var version = find.findSync("version", ".", "package.json").then(s => {

    try {
        var versionLine = s["package.json"].line[0];
        var versionSplit = versionLine.split('"');
        var version = versionSplit[3];

        // to update client version in code 
        var options2 = {
            files: ["./src/app/constants.ts"],
            from: [/clientVersion.*/g],
            to: "clientVersion = '" + version + "';"
        };

        replace.sync(options2);
    } catch (e) {
        console.error('Error occurred updating version:', e);
    }
});

// to update imports to use npm module 
find.findSync("name", ".", "package.json").then(s => {

    try {
        var nameLine = s["package.json"].line[0];
        var nameSplit = nameLine.split('"');
        var name = nameSplit[3];

        var options1 = {
            files: ["./lib/app/app-routing.module.ts", "./lib/app/app.module.ts"],
            from: [/\.\/.*\/.*\.component/g, /\.\/.*\.(service|directive|handler)/g, /\.\/route-data/g],
            to: name
        };

        replace.sync(options1);

    } catch (e) {
        console.error('Error occurred updating name:', e);
    }
});
