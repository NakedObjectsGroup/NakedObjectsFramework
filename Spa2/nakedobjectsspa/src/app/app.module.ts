import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FooterComponent } from './footer/footer.component';
import { HomeComponent } from './home/home.component';
import { ObjectComponent } from './object/object.component';
import { ListComponent } from './list/list.component';
import { ErrorComponent } from './error/error.component';
import { PropertiesComponent } from './properties/properties.component';
import { CollectionsComponent } from './collections/collections.component';
import { DialogComponent } from './dialog/dialog.component';
import { ParametersComponent } from './parameters/parameters.component';
import { EditPropertyComponent } from './edit-property/edit-property.component';
import { ViewPropertyComponent } from './view-property/view-property.component';
import { EditParameterComponent } from './edit-parameter/edit-parameter.component';
import { RecentComponent } from './recent/recent.component';
import { ApplicationPropertiesComponent } from './application-properties/application-properties.component';
import { ClickDirective  } from './click.directive';
import { ClearDirective  } from './clear.directive';
import { ErrorService } from './error.service';
import { ContextService } from './context.service';
import { UrlManagerService } from './url-manager.service';
import { ClickHandlerService } from './click-handler.service';
import { RepLoaderService } from './rep-loader.service';
import { ViewModelFactoryService } from './view-model-factory.service';
import { ColorService } from './color.service';
import { MaskService } from './mask.service';
import { CollectionComponent } from './collection/collection.component';
import { DndModule } from 'ng2-dnd';
import { ReactiveFormsModule } from '@angular/forms';
import { AttachmentComponent } from './attachment/attachment.component';
import { MultiLineDialogComponent } from './multi-line-dialog/multi-line-dialog.component';
import { ViewParameterComponent } from './view-parameter/view-parameter.component';
import { GeminiErrorHandler } from './error.handler';
import { MenuBarComponent } from './menu-bar/menu-bar.component';
import { ActionComponent } from './action/action.component';
import { DynamicObjectComponent } from './dynamic-object/dynamic-object.component';
import { CustomComponentService } from './custom-component.service';
import { CustomComponentConfigService } from './custom-component-config.service';
import { DynamicListComponent } from './dynamic-list/dynamic-list.component';
import { ConfigService, configFactory, localeFactory } from './config.service';
import { LoggerService } from './logger.service';
import { AttachmentPropertyComponent } from './attachment-property/attachment-property.component';
import { DynamicErrorComponent } from './dynamic-error/dynamic-error.component';
import { CiceroComponent } from './cicero/cicero.component';
import { CiceroCommandFactoryService } from './cicero-command-factory.service';
import { CiceroRendererService } from './cicero-renderer.service';
import { ActionBarComponent } from './action-bar/action-bar.component';
import { ActionListComponent } from './action-list/action-list.component';
import { RowComponent } from './row/row.component';
import { HeaderComponent } from './header/header.component';
import { AuthService } from './auth.service';
import { LoginComponent } from './login/login.component';
import { LogoffComponent } from './logoff/logoff.component';
import { CiceroContextService } from './cicero-context.service';
import { DatePickerFacadeComponent } from './date-picker-facade/date-picker-facade.component';
import { AutoCompleteComponent } from './auto-complete/auto-complete.component';
import { DatePickerComponent } from './date-picker/date-picker.component';
import { TimePickerComponent } from './time-picker/time-picker.component';
import { TimePickerFacadeComponent } from './time-picker-facade/time-picker-facade.component';
import { ObjectNotFoundErrorComponent } from './object-not-found-error/object-not-found-error.component';
import { HttpClientModule } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';

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
        FooterComponent,
        HomeComponent,
        ObjectComponent,
        ListComponent,
        ErrorComponent,
        ActionListComponent,
        ActionBarComponent,
        PropertiesComponent,
        CollectionsComponent,
        DialogComponent,
        ParametersComponent,
        EditPropertyComponent,
        ViewPropertyComponent,
        EditParameterComponent,
        RecentComponent,
        ApplicationPropertiesComponent,
        ClickDirective,
        ClearDirective,
        CollectionComponent,
        AttachmentComponent,
        MultiLineDialogComponent,
        ViewParameterComponent,
        MenuBarComponent,
        ActionComponent,
        DynamicObjectComponent,
        DynamicListComponent,
        AttachmentPropertyComponent,
        DynamicErrorComponent,
        CiceroComponent,
        RowComponent,
        HeaderComponent,
        LoginComponent,
        LogoffComponent,
        DatePickerFacadeComponent,
        AutoCompleteComponent,
        DatePickerComponent,
        TimePickerComponent,
        TimePickerFacadeComponent,
        ObjectNotFoundErrorComponent,
    ],
    entryComponents: [
        ObjectComponent,
        ListComponent,
        ErrorComponent,
        ObjectNotFoundErrorComponent
    ],
    imports: [
        BrowserModule,
        DndModule.forRoot(),
        FormsModule,
        RoutingModule,
        ReactiveFormsModule,
        HttpClientModule
    ],
    providers: [
        UrlManagerService,
        ClickHandlerService,
        ContextService,
        RepLoaderService,
        ViewModelFactoryService,
        ColorService,
        ErrorService,
        MaskService,
        CustomComponentService,
        // to configure custom components create implementation of ICustomComponentConfigService and bind in here
        { provide: CustomComponentConfigService, useClass: CustomComponentConfigService },
        LoggerService,
        ConfigService,
        CiceroCommandFactoryService,
        CiceroRendererService,
        CiceroContextService,
        AuthService,
        { provide: ErrorHandler, useClass: GeminiErrorHandler },
        { provide: APP_INITIALIZER, useFactory: configFactory, deps: [ConfigService], multi: true },
        { provide: LOCALE_ID, useFactory: localeFactory, deps: [ConfigService] },
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
