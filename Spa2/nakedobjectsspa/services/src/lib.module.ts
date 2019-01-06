import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER, LOCALE_ID, ModuleWithProviders  } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ErrorService } from './error.service';
import { ContextService } from './context.service';
import { UrlManagerService } from './url-manager.service';
import { ClickHandlerService } from './click-handler.service';
import { RepLoaderService } from './rep-loader.service';
import { ColorService } from './color.service';
import { MaskService } from './mask.service';
import { ReactiveFormsModule } from '@angular/forms';
import { GeminiErrorHandler } from './error.handler';
import { ConfigService, configFactory, localeFactory } from './config.service';
import { LoggerService } from './logger.service';
import { AuthService } from './auth.service';
import { HttpClientModule } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';

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
    public static forRoot(): ModuleWithProviders {

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
            { provide: ErrorHandler, useClass: GeminiErrorHandler },
            { provide: APP_INITIALIZER, useFactory: configFactory, deps: [ConfigService], multi: true },
            { provide: LOCALE_ID, useFactory: localeFactory, deps: [ConfigService] },
            { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
        ]
        };
      }
 }
