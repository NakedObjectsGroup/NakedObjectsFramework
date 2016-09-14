import { NgModule, ExceptionHandler }       from '@angular/core';
import { BrowserModule }  from '@angular/platform-browser';
import { FormsModule }    from '@angular/forms';
import { HttpModule }     from '@angular/http';
import { routing }        from './app.routing';
import { AppComponent }  from './app.component';
import { HomeComponent } from './home.component';
import { ObjectComponent } from './object.component';
import { ListComponent } from './list.component';
import { GeminiExceptionHandler } from './gemini.exception-handler';
import { Context } from './context.service';
import { UrlManager } from './urlmanager.service';
import { RepLoader } from './reploader.service';
import { ErrorComponent } from './error.component';

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
        ErrorComponent
    ],
    providers: [
        //{ provide: ExceptionHandler, useClass: GeminiExceptionHandler }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}