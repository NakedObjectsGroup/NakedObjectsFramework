import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ModuleWithProviders  } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ViewModelFactoryService } from './view-model-factory.service';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { DragAndDropService } from './drag-and-drop.service';

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
