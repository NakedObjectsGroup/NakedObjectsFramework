import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from "./home/home.component";
import { ObjectComponent} from "./object/object.component";
import { ListComponent} from "./list/list.component";
import { ErrorComponent } from './error/error.component';
import { RecentComponent } from './recent/recent.component';
import { ApplicationPropertiesComponent } from './application-properties/application-properties.component';

const routes: Routes = [
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
            { path: "list", component: ListComponent, data: { pane: 2, class: "split" } },
            { path: "recent", component: RecentComponent, data: { pane: 2, class: "split" } }       
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
            { path: "list", component: ListComponent, data: { pane: 2, class: "split" } },
            { path: "recent", component: RecentComponent, data: { pane: 2, class: "split" } }       
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
            { path: "list", component: ListComponent, data: { pane: 2, class: "split" } },
            { path: "recent", component: RecentComponent, data: { pane: 2, class: "split" } }       

        ]
    },
    {
        path: 'gemini/recent',
        component: RecentComponent,
        data: { pane: 1, class: "single" },
        children: [
            { path: "" },
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: ObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: ListComponent, data: { pane: 2, class: "split" } },
            { path: "recent", component: RecentComponent, data: { pane: 2, class: "split" } }       
        ]
    },
    {
        path: 'gemini/error',
        component: ErrorComponent,
        data: { pane: 1, class: "single" }
    },
    {
        path: 'gemini/applicationProperties',
        component: ApplicationPropertiesComponent,
        data: { pane: 1, class: "single" }
    }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: []
})
export class RoutingModule { }


