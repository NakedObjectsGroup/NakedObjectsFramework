var mv = require('mv');
var replace = require('replace-in-file');
var find = require('find-in-files');

function copyAndReplace(name) {
    var fromName = `./src/app/${name}/${name}.component.alt.css`;
    var tempName = `./src/app/${name}/${name}.component.temp.css`;
    var toName = `./src/app/${name}/${name}.component.css`;

    const fErr = function (err) { if (err) console.error('Error occurred:', err); };
    const opts = { clobber: false, mkdirp: false }

    const p = new Promise(() => {
        mv(toName, tempName, opts, fErr);
        return true;
    }).then(() => {
        mv(fromName, toName, opts, fErr);
        return true;
    }).then(() => {
        mv(tempName, fromName, opts, fErr);
        return true;
    });
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

mv("./src/styles.css", "./src/styles.temp.css", { clobber: true, mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv("./src/styles.alt.css", "./src/styles.css", { clobber: true, mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv("./src/styles.temp.css", "./src/styles.alt.css", { clobber: true, mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });

// to update package name 
var optionsToAlt = {
    files: ["./package.json"],
    from: [/nakedobjects.spa/g],
    to: "nakedobjects.alt.spa"
};

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