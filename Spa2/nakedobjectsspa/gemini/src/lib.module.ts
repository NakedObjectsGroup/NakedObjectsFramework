import { HttpClientModule } from '@angular/common/http';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { DndModule } from '@beyerleinf/ngx-dnd';
import { ActionBarComponent } from './action-bar/action-bar.component';
import { ActionListComponent } from './action-list/action-list.component';
import { ActionComponent } from './action/action.component';
import { ApplicationPropertiesComponent } from './application-properties/application-properties.component';
import { AttachmentPropertyComponent } from './attachment-property/attachment-property.component';
import { AttachmentComponent } from './attachment/attachment.component';
import { AutoCompleteComponent } from './auto-complete/auto-complete.component';
import { CallbackComponent } from './callback/callback.component';
import { ClearDirective } from './clear.directive';
import { ClickDirective } from './click.directive';
import { CollectionComponent } from './collection/collection.component';
import { CollectionsComponent } from './collections/collections.component';
import { CustomComponentConfigService } from './custom-component-config.service';
import { CustomComponentService } from './custom-component.service';
import { DatePickerFacadeComponent } from './date-picker-facade/date-picker-facade.component';
import { DatePickerComponent } from './date-picker/date-picker.component';
import { DialogComponent } from './dialog/dialog.component';
import { DynamicErrorComponent } from './dynamic-error/dynamic-error.component';
import { DynamicListComponent } from './dynamic-list/dynamic-list.component';
import { DynamicObjectComponent } from './dynamic-object/dynamic-object.component';
import { EditParameterComponent } from './edit-parameter/edit-parameter.component';
import { EditPropertyComponent } from './edit-property/edit-property.component';
import { ErrorComponent } from './error/error.component';
import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { HomeComponent } from './home/home.component';
import { ListComponent } from './list/list.component';
import { LoginComponent } from './login/login.component';
import { LogoffComponent } from './logoff/logoff.component';
import { MenuBarComponent } from './menu-bar/menu-bar.component';
import { MultiLineDialogComponent } from './multi-line-dialog/multi-line-dialog.component';
import { ObjectNotFoundErrorComponent } from './object-not-found-error/object-not-found-error.component';
import { ObjectComponent } from './object/object.component';
import { ParametersComponent } from './parameters/parameters.component';
import { PropertiesComponent } from './properties/properties.component';
import { RecentComponent } from './recent/recent.component';
import { RowComponent } from './row/row.component';
import { TimePickerFacadeComponent } from './time-picker-facade/time-picker-facade.component';
import { TimePickerComponent } from './time-picker/time-picker.component';
import { ViewParameterComponent } from './view-parameter/view-parameter.component';
import { ViewPropertyComponent } from './view-property/view-property.component';

@NgModule({
    declarations: [
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
        CallbackComponent,
    ],
    entryComponents: [
        ObjectComponent,
        ListComponent,
        ErrorComponent,
        ObjectNotFoundErrorComponent,
    ],
    imports: [
        BrowserModule,
        DndModule.forRoot(),
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        RouterModule,
    ],
    exports: [
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
        CallbackComponent,
    ],
})
export class LibModule {
    public static forRoot(): ModuleWithProviders {

        return {
          ngModule: LibModule,
          providers: [
            CustomComponentService,
            // to configure custom components create implementation of ICustomComponentConfigService and bind in here
            { provide: CustomComponentConfigService, useClass: CustomComponentConfigService },
        ]
        };
      }
 }
