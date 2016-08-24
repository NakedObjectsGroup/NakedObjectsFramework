import { NgModule }       from '@angular/core';
import { BrowserModule }  from '@angular/platform-browser';
import { FormsModule }    from '@angular/forms';
import { HttpModule }     from '@angular/http';

import { routing }        from './app.routing';

import { AppComponent }  from './app.component';
import { HomeComponent } from './home.component';
import { ObjectComponent } from './object.component';
import { ListComponent } from './list.component';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        routing,
        HttpModule
    ],
    declarations: [
        AppComponent,
        HomeComponent,
        ObjectComponent,
        ListComponent,
    ],
    providers: [
        
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}