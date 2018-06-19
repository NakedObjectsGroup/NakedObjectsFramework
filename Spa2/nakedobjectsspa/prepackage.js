/* eslint-disable */

var replace = require('replace-in-file');
var find = require('find-in-files');

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
