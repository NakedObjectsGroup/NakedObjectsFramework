var mv = require('mv');
var replace = require('replace-in-file');
var find = require('find-in-files');
var pathExists = require('path-exists');

const fErr = function (err) { if (err) console.error('Error occurred:', err); };
const opts = { clobber: false, mkdirp: false }

function copyAndReplace(name, path) {
    var fromName = `./${path}/app/${name}/${name}.component.alt.css`;
    var tempName = `./${path}/app/${name}/${name}.component.temp.css`;
    var toName = `./${path}/app/${name}/${name}.component.css`;

    const mv3 = () => mv(tempName, fromName, opts, fErr);
    const mv2 = () => mv(fromName, toName, opts, mv3);
    const mv1 = () => mv(toName, tempName, opts, mv2);

    mv1();
}

function copyAndReplaceAll(names) {
    for (name of names) {
        const lname = name;
        pathExists("./node_modules/nakedobjects.spa").then(exists => {
            const path = exists ? "node_modules/nakedobjects.spa/lib" : "src";
            copyAndReplace(lname, path);
        });
    }
}

let names = ["action", "action-bar", "action-list", "application-properties", "attachment",
             "attachment-property", "auto-complete", "auto-complete-facade", "cicero",
             "collection", "collections", "date-picker", "date-picker-facade","dialog",
             "dynamic-error", "dynamic-list", "dynamic-object", "edit-parameter",
             "edit-property", "error", "footer", "header", "home", "list", "login", "logoff",
             "menu-bar", "multi-line-dialog", "object", "object-not-found-error", "parameters", "properties", "recent",
             "row", "time-picker", "time-picker-facade", "view-parameter", "view-property"]


copyAndReplaceAll(names);

const ms3 = () => mv("./src/styles.temp.css", "./src/styles.alt.css", opts, fErr);
const ms2 = () => mv("./src/styles.alt.css", "./src/styles.css", opts, ms3);
const ms1 = () => mv("./src/styles.css", "./src/styles.temp.css", opts, ms2);

ms1();

find.findSync("name", ".", "package.json").then(s => {

    try {
        var nameLine = s["package.json"].line[0];
        var nameSplit = nameLine.split('"');
        var name = nameSplit[3];

        var newName = name.indexOf("alt") >= 0 ? "nakedobjects.spa" : "nakedobjects.alt.spa";

        var regex = new RegExp(name, "g");

        var options = {
            files: ["package.json"],
            from: regex,
            to: newName
        };

        replace.sync(options);

    } catch (e) {
        console.error('Error occurred updating name:', e);
    }
});