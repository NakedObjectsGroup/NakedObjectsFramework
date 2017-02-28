var mv = require('mv');
var replace = require('replace-in-file');
var find = require('find-in-files');

const fErr = function (err) { if (err) console.error('Error occurred:', err); };
const opts = { clobber: false, mkdirp: false }

function copyAndReplace(name) {
    var fromName = `./lib/app/${name}/${name}.component.alt.css`;
    var tempName = `./lib/app/${name}/${name}.component.temp.css`;
    var toName = `./lib/app/${name}/${name}.component.css`;


    const mv3 = () => mv(tempName, fromName, opts, fErr);
    const mv2 = () => mv(fromName, toName, opts, mv3);
    const mv1 = () => mv(toName, tempName, opts, mv2);

    mv1();
}

function copyAndReplaceAll(names) {
    for (name of names) {
        copyAndReplace(name);
    }
}

let names = ["action", "actions", "application-properties", "attachment", "attachment-property", "button", "buttons",
    "cicero", "collection", "collections", "dialog", "dynamic-error", "dynamic-list", "dynamic-object", "edit-parameter",
    "edit-property", "error", "footer", "home", "list", "menu", "menus", "multi-line-dialog", "object", "parameters",
    "properties", "recent"]

copyAndReplaceAll(names);

const ms3 = () => mv("../../src/styles.temp.css", "../../src/styles.alt.css", opts, fErr);
const ms2 = () => mv("../../src/styles.alt.css", "../../src/styles.css", opts, ms3);
const ms1 = () => mv("../../src/styles.css", "../../src/styles.temp.css", opts, ms2);

ms1();
