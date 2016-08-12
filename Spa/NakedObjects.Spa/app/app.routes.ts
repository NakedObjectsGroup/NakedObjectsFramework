import { provideRouter, RouterConfig } from '@angular/router';
import { SingleHomeComponent } from './single-home.component';
import { SplitHomeComponent } from "./split-home.component";
import * as Singleobjectcomponent from "./single-object.component";
import * as Singlelistcomponent from "./single-list.component";
const routes: RouterConfig = [
    {
        path: '',
        redirectTo: '/gemini/home',
        pathMatch: 'full'
    },
    {
        path: 'gemini/home',
        component: SingleHomeComponent
    },
    {
        path: 'gemini/home/home',
        component: SplitHomeComponent
    },
    {
        path: 'gemini/home/object',
        component: SplitHomeComponent
    },
    {
        path: 'gemini/object',
        component: Singleobjectcomponent.SingleObjectComponent
    },
    {
        path: 'gemini/list',
        component: Singlelistcomponent.SingleListComponent
    }
];

export const appRouterProviders = [
    provideRouter(routes)
];