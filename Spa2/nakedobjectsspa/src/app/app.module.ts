import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DndModule } from '@beyerleinf/ngx-dnd';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { LibModule as ServicesModule } from '@nakedobjects/services';
import { LibModule as ViewModelModule } from '@nakedobjects/view-models';
import { LibModule as CiceroModule } from '@nakedobjects/cicero';
import { LibModule as GeminiModule } from '@nakedobjects/gemini';

@NgModule({
    declarations: [
        AppComponent,
    ],
    imports: [
        BrowserModule,
        DndModule.forRoot(),
        FormsModule,
        RoutingModule,
        ReactiveFormsModule,
        HttpClientModule,
        ServicesModule.forRoot(),
        ViewModelModule.forRoot(),
        CiceroModule.forRoot(),
        GeminiModule.forRoot(),
    ],
    providers: [
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
