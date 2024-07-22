import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { LibModule as CiceroModule } from '@nakedobjects/cicero';
import { LibModule as GeminiModule } from '@nakedobjects/gemini';
import { LibModule as ServicesModule } from '@nakedobjects/services';
//import { ObfuscateService } from '@nakedobjects/services';
import { LibModule as ViewModelModule } from '@nakedobjects/view-models';
import { RoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
// import { Base64ObfuscateService } from './base64obfuscate.service';

@NgModule({ declarations: [
        AppComponent,
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        FormsModule,
        RoutingModule,
        ReactiveFormsModule,
        ServicesModule.forRoot(),
        ViewModelModule.forRoot(),
        CiceroModule.forRoot(),
        GeminiModule.forRoot()], providers: [
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule { }
