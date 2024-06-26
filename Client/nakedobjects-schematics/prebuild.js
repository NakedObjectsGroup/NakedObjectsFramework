var cpx = require('cpx2');
var mv = require('mv');

var copyCode = "../nakedobjects-client/src/app/*.{ts,css,html}";
var copyPng = "../nakedobjects-client/src/assets/*.png";
var copyCss = "../nakedobjects-client/src/*.css";
var copyIndex = "../nakedobjects-client/src/index.html";
var copyConfig = "../nakedobjects-client/src/empty_config.json";
var copyFonts = "../nakedobjects-client/src/fonts/*.{eot,svg,ttf,woff,txt}";
var tempCodeDir = "./temp/code";
var tempAssetsDir = "./temp/assets";
var tempFontsDir = "./temp/fonts";

var codeDir = "./src/nakedobjects-schematics/files/code";
var assetsDir = "./src/nakedobjects-schematics/files/assets";
var fontsDir = "./src/nakedobjects-schematics/files/fonts";

cpx.copySync(copyCode, tempCodeDir);
cpx.copySync(copyConfig, tempCodeDir);
cpx.copySync(copyPng, tempAssetsDir);
cpx.copySync(copyCss, tempAssetsDir);
cpx.copySync(copyIndex, tempAssetsDir);
cpx.copySync(copyFonts, tempFontsDir);

mv(`${tempCodeDir}/app-routing.module.ts`, `${codeDir}/routing`, { mkdirp: true }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/app.component.css`, `${codeDir}/component_css`, { mkdirp: true }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/app.component.html`, `${codeDir}/component_template`, { mkdirp: true }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/app.component.ts`, `${codeDir}/component`, { mkdirp: true }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/app.module.ts`, `${codeDir}/module`, { mkdirp: true }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/empty_config.json`, `${codeDir}/config`, { mkdirp: true }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempAssetsDir}`, `${assetsDir}`, { mkdirp: true }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempFontsDir}`, `${fontsDir}`, { mkdirp: true }, function (err) { if (err) console.error('Error occurred:', err); });
