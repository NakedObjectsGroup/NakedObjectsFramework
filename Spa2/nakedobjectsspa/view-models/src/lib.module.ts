import { HttpClientModule } from '@angular/common/http';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { DragAndDropService } from './drag-and-drop.service';
import { ViewModelFactoryService } from './view-model-factory.service';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        RouterModule,
    ],
    exports: [
    ],
})
export class LibModule {
    public static forRoot(): ModuleWithProviders {

        return {
          ngModule: LibModule,
          providers: [
            ViewModelFactoryService,
            DragAndDropService
        ]
        };
      }
 }
