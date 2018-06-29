var mv = require('mv');

mv("../nakedobjectsspa/src/app/app.component.ts", "./src/files/code/component", { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv("../nakedobjectsspa/src/app/app.component.css", "./src/files/code/component_css", { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv("../nakedobjectsspa/src/app/app.component.html", "./src/files/code/component_template", { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv("../nakedobjectsspa/src/app/app.module.ts", "./src/files/code/module", { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });
mv("../nakedobjectsspa/src/app/app-routing.module.ts", "./src/files/code/routing", { mkdirp: false }, function (err) { if (err) console.error('Error occurred:', err); });

