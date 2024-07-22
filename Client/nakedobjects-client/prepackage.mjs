/* eslint-disable */

import { replaceInFile } from 'replace-in-file';
import { find } from 'find-in-files';

// get current version

const s = await find("version", ".", "package.json");

try {
    var versionLine = s["package.json"].line[0];
    var versionSplit = versionLine.split('"');
    var version = versionSplit[3];

    // to update client version in code
    var options2 = {
        files: ["./gemini/src/version.ts"],
        from: [/clientVersion.*/g],
        to: "clientVersion = '" + version + "';"
    };

    await replaceInFile(options2);
} catch (e) {
    console.error('Error occurred updating version:', e);
}