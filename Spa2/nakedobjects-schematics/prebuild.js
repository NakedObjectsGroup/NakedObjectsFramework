var cpx = require('cpx');
var mv = require('mv');

var copyCode = "../nakedobjectsspa/src/app/*.{ts,css,html}";
var copyAssets = "../nakedobjectsspa/src/app/*.{ts,css,html}";
var tempCodeDir = "./temp/code";
var codeDir = "./src/nakedobjects-schematics/files/code";

cpx.copySync(copyCode, tempCodeDir);

mv(`${tempCodeDir}/app-routing.module.ts`, `${codeDir}/routing`, { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/app.component.css`, `${codeDir}/component_css`, { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/app.component.html`, `${codeDir}/component_template`, { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/app.component.ts`, `${codeDir}/component`, { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv(`${tempCodeDir}/app.module.ts`, `${codeDir}/module`, { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });

