import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DndModule } from '@beyerleinf/ngx-dnd';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { LibModule } from '@nakedobjects/lib';

// export function authServiceFactory(configService: ConfigService, auth0AuthService: Auth0AuthService, nullAuthService: NullAuthService): any {
//     if (configService.config.authenticate) {
//         return auth0AuthService;
//     }
//     else {
//         return nullAuthService;
//     }
// }

@NgModule({
    declarations: [
        AppComponent,
    ],
    // entryComponents: [
    //     ObjectComponent,
    //     ListComponent,
    //     ErrorComponent,
    //     ObjectNotFoundErrorComponent
    // ],
    imports: [
        BrowserModule,
        DndModule.forRoot(),
        FormsModule,
        RoutingModule,
        ReactiveFormsModule,
        HttpClientModule,
        LibModule.forRoot()
    ],
    providers: [
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
