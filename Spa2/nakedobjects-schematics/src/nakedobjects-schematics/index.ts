import { Rule, SchematicContext, Tree, url, FileEntry } from '@angular-devkit/schematics';

function getUpdateFiles(localFiles: Tree) {
    return [
        { path: "src/app/app.module.ts", file: localFiles.get("code/module")! },
        { path: "src/app/app.component.ts", file: localFiles.get("code/component")! },
        { path: "src/app/app.component.html", file: localFiles.get("code/component_template")! },
        { path: "src/styles.css", file: localFiles.get("assets/styles.css")! },
        { path: "src/index.html", file: localFiles.get("assets/index.html")! },
    ];
}

function getCreateFiles(localFiles: Tree) {
    return [
        { path: "src/app/app-routing.module.ts", file: localFiles.get("code/routing")! },
        { path: "src/config.json", file: localFiles.get("assets/config")! },
        { path: "src/assets/alt.calendar.png", file: localFiles.get("assets/alt.calendar.png")! },
        { path: "src/assets/alt.list.png", file: localFiles.get("assets/alt.list.png")! },
        { path: "src/assets/alt.summary.png", file: localFiles.get("assets/alt.summary.png")! },
        { path: "src/assets/alt.table.png", file: localFiles.get("assets/alt.table.png")! },
        { path: "src/assets/calendar.png", file: localFiles.get("assets/calendar.png")! },
        { path: "src/assets/list.png", file: localFiles.get("assets/list.png")! },
        { path: "src/assets/summary.png", file: localFiles.get("assets/summary.png")! },
        { path: "src/assets/table.png", file: localFiles.get("assets/table.png")! },
        { path: "src/fonts/iconFont.eot", file: localFiles.get("fonts/iconFont.eot")! },
        { path: "src/fonts/iconFont.svg", file: localFiles.get("fonts/iconFont.svg")! },
        { path: "src/fonts/iconFont.ttf", file: localFiles.get("fonts/iconFont.ttf")! },
        { path: "src/fonts/iconFont.woff", file: localFiles.get("fonts/iconFont.woff")! },
        { path: "src/fonts/license.txt", file: localFiles.get("fonts/license.txt")! },
        { path: "src/alt.styles.css", file: localFiles.get("assets/styles.alt.css")! },
    ];
}

function updateFile(remoteFiles: Tree, { path, file }: { path: string, file: FileEntry }) {
    if (!file) {
        console.log(`missing: ${path}`)
    }

    remoteFiles.overwrite(path, file.content);
}

function createFile(remoteFiles: Tree, { path, file }: { path: string, file: FileEntry }) {
    if (!file) {
        console.log(`missing: ${path}`)
    }

    remoteFiles.create(path, file.content);
}

// You don't have to export the function as default. You can also have more than one rule factory
// per file.
export function newProject(/* options: any */): Rule {
  return (tree: Tree, _context: SchematicContext) => {

    const localFiles = url("./files")(_context) as Tree;
    const updateFiles = getUpdateFiles(localFiles);
    const createFiles = getCreateFiles(localFiles);

    if (updateFiles.length > 0) {
        updateFiles.forEach(f => updateFile(tree, f));
    }

    if (createFiles.length > 0) {
        createFiles.forEach(f => createFile(tree, f));
    }

    const config = tree.read(".angular-cli.json");
    if (config) {
        const asJson = JSON.parse(config.toString());
        asJson.apps[0].assets.push("config.json");
        tree.overwrite(".angular-cli.json", JSON.stringify(asJson, null, 2));
    }

    // angular 6
    const configA6 = tree.read("angular.json");
    if (configA6) {
        const asJson = JSON.parse(configA6.toString());

        Object.entries(asJson.projects).forEach(([,value] : any) => {
            const assets = value.architect &&
                value.architect.build &&
                value.architect.build.options &&
                value.architect.build.options.assets;

            if (assets) {
                assets.push("src/config.json");
            }
        });

        tree.overwrite("angular.json", JSON.stringify(asJson, null, 2));
    }

    return tree;
  };
}
