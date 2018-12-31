import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ModuleWithProviders  } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
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
import { ViewModelFactoryService } from './view-model-factory.service';
import { CollectionComponent } from './collection/collection.component';
import { DndModule } from '@beyerleinf/ngx-dnd';
import { ReactiveFormsModule } from '@angular/forms';
import { AttachmentComponent } from './attachment/attachment.component';
import { MultiLineDialogComponent } from './multi-line-dialog/multi-line-dialog.component';
import { ViewParameterComponent } from './view-parameter/view-parameter.component';
import { MenuBarComponent } from './menu-bar/menu-bar.component';
import { ActionComponent } from './action/action.component';
import { DynamicObjectComponent } from './dynamic-object/dynamic-object.component';
import { CustomComponentService } from './custom-component.service';
import { CustomComponentConfigService } from './custom-component-config.service';
import { DynamicListComponent } from './dynamic-list/dynamic-list.component';
import { AttachmentPropertyComponent } from './attachment-property/attachment-property.component';
import { DynamicErrorComponent } from './dynamic-error/dynamic-error.component';
import { CiceroComponent } from './cicero/cicero.component';
import { CiceroCommandFactoryService } from './cicero-command-factory.service';
import { CiceroRendererService } from './cicero-renderer.service';
import { ActionBarComponent } from './action-bar/action-bar.component';
import { ActionListComponent } from './action-list/action-list.component';
import { RowComponent } from './row/row.component';
import { HeaderComponent } from './header/header.component';
import { LoginComponent } from './login/login.component';
import { LogoffComponent } from './logoff/logoff.component';
import { CallbackComponent } from './callback/callback.component';
import { CiceroContextService } from './cicero-context.service';
import { DatePickerFacadeComponent } from './date-picker-facade/date-picker-facade.component';
import { AutoCompleteComponent } from './auto-complete/auto-complete.component';
import { DatePickerComponent } from './date-picker/date-picker.component';
import { TimePickerComponent } from './time-picker/time-picker.component';
import { TimePickerFacadeComponent } from './time-picker-facade/time-picker-facade.component';
import { ObjectNotFoundErrorComponent } from './object-not-found-error/object-not-found-error.component';
import { HttpClientModule } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { DragAndDropService } from './view-models/drag-and-drop.service';

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
        CallbackComponent,
    ],
})
export class LibModule {
    public static forRoot(): ModuleWithProviders {

        return {
          ngModule: LibModule,
          providers: [
            ViewModelFactoryService,
            CustomComponentService,
            // to configure custom components create implementation of ICustomComponentConfigService and bind in here
            { provide: CustomComponentConfigService, useClass: CustomComponentConfigService },
            CiceroCommandFactoryService,
            CiceroRendererService,
            CiceroContextService,
            DragAndDropService,
        ]
        };
      }
 }
