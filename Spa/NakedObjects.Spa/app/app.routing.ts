import { provideRouter, RouterConfig } from '@angular/router';
import { HomeComponent } from "./home.component";
import { ObjectComponent} from "./object.component";
import { ListComponent} from "./list.component";
import { Routes, RouterModule } from '@angular/router';
import { ErrorComponent } from './error.component';
const routes: RouterConfig = [
    {
        path: '',
        redirectTo: '/gemini/home',
        pathMatch: 'full'
    },
    {
        path: 'gemini/home',
        component: HomeComponent,
        data: { pane: 1, class : "single" },
        children: [
            { path: "" },
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: ObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: ListComponent, data: { pane: 2, class: "split" } }       
        ]
    },
    {
        path: 'gemini/object',
        component: ObjectComponent,
        data: { pane: 1, class: "single" },
        children: [
            { path: "" },
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: ObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: ListComponent, data: { pane: 2, class: "split" } }
        ]
    },
    {
        path: 'gemini/list',
        component: ListComponent,
        data: { pane: 1, class: "single" },
        children: [
            { path: "" },
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: ObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: ListComponent, data: { pane: 2, class: "split" } }
        ]
    },
    {
        path: 'gemini/error',
        component: ErrorComponent,
        data: { pane: 1, class: "single" }
    }
];

export const routing = RouterModule.forRoot(routes);