var gentlyCopy = require('gently-copy');

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
    './lib/assets'];

var rootdest = '../../';
var appdest = '../../app/';

gentlyCopy(appfileList, appdest, { overwrite: true });
gentlyCopy(rootfileList, rootdest, { overwrite: true });