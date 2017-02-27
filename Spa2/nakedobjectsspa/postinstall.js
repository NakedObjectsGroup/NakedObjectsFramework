var gentlyCopy = require('gently-copy');
var fs = require('fs');

var appfileList = [   
    './lib/app/app-routing.module.ts',
    './lib/app/app.component.css',
    './lib/app/app.component.html',
    './lib/app/app.component.ts',
    './lib/app/app.module.ts'];

var rootfileList = [
    './lib/config.json',
    './lib/index.html',
    './lib/styles.css',
    './lib/fonts',
    './lib/assets'];

var rootdest = '../../src';
var appdest = '../../src/app/';

gentlyCopy(appfileList, appdest, { overwrite: true });
gentlyCopy(rootfileList, rootdest, { overwrite: true });

var ngCliFile = '../../.angular-cli.json';

if (fs.existsSync(ngCliFile)) {
    var f = fs.readFileSync(ngCliFile, 'utf8');
    var ac = JSON.parse(f);
    var assets = ac.apps[0].assets;
    var found = false;

    for (var i = 0; i < assets.length; i++) {
        found = assets[i] === "config.json" || found;
    }

    if (!found) {
        assets.push("config.json");
        fs.writeFile(ngCliFile, JSON.stringify(ac, null, 2));
    }
}
