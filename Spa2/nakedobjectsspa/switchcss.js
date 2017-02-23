var mv = require('mv');

function copyAndReplace(name) {
    var fromName = `./src/app/${name}/${name}.component.alt.css`;
    var toName = `./src/app/${name}/${name}.component.css`;
    mv(fromName, toName, { clobber: true, mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
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
mv("./src/styles.alt.css", "./src/styles.css", { clobber: true, mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
