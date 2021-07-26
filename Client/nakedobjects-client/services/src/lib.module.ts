import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, ErrorHandler, LOCALE_ID, ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { ObfuscateService } from './obfuscate.service';
import { AuthInterceptor } from './auth.interceptor';
import { AuthService } from './auth.service';
import { ClickHandlerService } from './click-handler.service';
import { ColorService } from './color.service';
import { configFactory, ConfigService, localeFactory } from './config.service';
import { ContextService } from './context.service';
import { GeminiErrorHandler } from './error.handler';
import { ErrorService } from './error.service';
import { LoggerService } from './logger.service';
import { MaskService } from './mask.service';
import { RepLoaderService } from './rep-loader.service';
import { UrlManagerService } from './url-manager.service';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        RouterModule,
    ]
})
export class LibModule {
    public static forRoot(): ModuleWithProviders<LibModule> {

        return {
            ngModule: LibModule,
            providers: [
                UrlManagerService,
                ClickHandlerService,
                ContextService,
                RepLoaderService,
                ColorService,
                ErrorService,
                MaskService,
                LoggerService,
                ConfigService,
                AuthService,
                ObfuscateService,
                { provide: ErrorHandler, useClass: GeminiErrorHandler },
                { provide: APP_INITIALIZER, useFactory: configFactory, deps: [ConfigService], multi: true },
                { provide: LOCALE_ID, useFactory: localeFactory, deps: [ConfigService] },
                { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
            ]
        };
    }
}
