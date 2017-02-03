import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListComponent } from './list/list.component';
import { ErrorComponent } from './error/error.component';
import { RecentComponent } from './recent/recent.component';
import { ApplicationPropertiesComponent } from './application-properties/application-properties.component';
import { AttachmentComponent } from './attachment/attachment.component';
import { MultiLineDialogComponent } from './multi-line-dialog/multi-line-dialog.component';
import { DynamicObjectComponent } from './dynamic-object/dynamic-object.component';
import { ICustomActivatedRouteData, ViewType} from './route-data';
import { DynamicListComponent} from './dynamic-list/dynamic-list.component';
const routes: Routes = [
    {
        path: '',
        redirectTo: '/gemini/home',
        pathMatch: 'full'
    },
    {
        path: 'gemini/home',
        component: HomeComponent,
        data: { pane: 1, class: "single" },
        children: [
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: DynamicObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: DynamicListComponent, data: { pane: 2, class: "split" } },
            { path: "attachment", component: AttachmentComponent, data: { pane: 2, class: "split" } },
            { path: "recent", component: RecentComponent, data: { pane: 2, class: "split" } }
        ]
    },
    {
        path: 'gemini/object',
        component: DynamicObjectComponent,
        data: { pane: 1, class: "single", dynamicType : ViewType.Object },
        children: [
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: DynamicObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: DynamicListComponent, data: { pane: 2, class: "split" } },
            { path: "attachment", component: AttachmentComponent, data: { pane: 2, class: "split" } },
            { path: "recent", component: RecentComponent, data: { pane: 2, class: "split" } }
        ]
    },
    {
        path: 'gemini/list',
        component: DynamicListComponent,
        data: { pane: 1, class: "single" },
        children: [
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: DynamicObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: DynamicListComponent, data: { pane: 2, class: "split" } },
            { path: "attachment", component: AttachmentComponent, data: { pane: 2, class: "split" } },
            { path: "recent", component: RecentComponent, data: { pane: 2, class: "split" } }
        ]
    },
    {
        path: 'gemini/attachment',
        component: AttachmentComponent,
        data: { pane: 1, class: "single" },
        children: [
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: DynamicObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: DynamicListComponent, data: { pane: 2, class: "split" } },
            { path: "attachment", component: AttachmentComponent, data: { pane: 2, class: "split" } },
            { path: "recent", component: RecentComponent, data: { pane: 2, class: "split" } }
        ]
    },
    {
        path: 'gemini/recent',
        component: RecentComponent,
        data: { pane: 1, class: "single" },
        children: [
            { path: "home", component: HomeComponent, data: { pane: 2, class: "split" } },
            { path: "object", component: DynamicObjectComponent, data: { pane: 2, class: "split" } },
            { path: "list", component: DynamicListComponent, data: { pane: 2, class: "split" } },
            { path: "attachment", component: AttachmentComponent, data: { pane: 2, class: "split" } },
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
    },
    {
        path: 'gemini/multiLineDialog',
        component: MultiLineDialogComponent,
        data: { pane: 1, class: "single" }
    },
    {
        path: '**',
        redirectTo: '/gemini/home',
        pathMatch: 'full'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    providers: []
})
export class RoutingModule { }


