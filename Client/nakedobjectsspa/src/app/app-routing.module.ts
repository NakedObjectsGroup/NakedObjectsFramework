﻿import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CiceroComponent } from '@nakedobjects/cicero';
import {
    ApplicationPropertiesComponent,
    AttachmentComponent,
    CallbackComponent,
    DynamicErrorComponent,
    DynamicListComponent,
    DynamicObjectComponent,
    HomeComponent,
    LogoffComponent,
    MultiLineDialogComponent,
    RecentComponent
} from '@nakedobjects/gemini';
import { AuthService, ViewType } from '@nakedobjects/services';

const routes: Routes = [
    {
        path: '',
        redirectTo: '/gemini/home',
        canActivate: [AuthService],
        pathMatch: 'full'
    },
    {
        path: 'gemini/home',
        component: HomeComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'gemini/object',
        component: DynamicObjectComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single', dynamicType: ViewType.Object },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'gemini/list',
        component: DynamicListComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'gemini/attachment',
        component: AttachmentComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'gemini/recent',
        component: RecentComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'gemini/error',
        component: DynamicErrorComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'gemini/applicationProperties',
        component: ApplicationPropertiesComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'gemini/multiLineDialog',
        component: MultiLineDialogComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'gemini/logoff',
        component: LogoffComponent,
        canActivate: [AuthService],
        canDeactivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'gemini/callback',
        component: CallbackComponent
    },
    {
        path: 'cicero/home',
        component: CiceroComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'cicero/object',
        component: CiceroComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'cicero/list',
        component: CiceroComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'cicero/error',
        component: CiceroComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: '**',
        redirectTo: '/gemini/home',
        canActivate: [AuthService],
        pathMatch: 'full'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    providers: []
})
export class RoutingModule { }
