import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { LibModule as CiceroModule } from '@nakedobjects/cicero';
import { LibModule as GeminiModule } from '@nakedobjects/gemini';
import { LibModule as ServicesModule, ObfuscateService } from '@nakedobjects/services';
import { LibModule as ViewModelModule } from '@nakedobjects/view-models';
import { RoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
//import { Base64ObfuscateService } from './base64obfuscate.service';

@NgModule({
    declarations: [
        AppComponent,
    ],
    imports: [
        BrowserModule,
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
        //{provide : ObfuscateService, useClass: Base64ObfuscateService}
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
