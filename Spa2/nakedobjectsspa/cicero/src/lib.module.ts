import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ModuleWithProviders } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { CiceroComponent } from './cicero/cicero.component';
import { CiceroCommandFactoryService } from './cicero-command-factory.service';
import { CiceroRendererService } from './cicero-renderer.service';
import { CiceroContextService } from './cicero-context.service';
import { HttpClientModule } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

@NgModule({
    declarations: [
        CiceroComponent,
    ],
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        RouterModule,
    ],
    exports: [
        CiceroComponent,
    ],
})
export class LibModule {
    public static forRoot(): ModuleWithProviders {

        return {
            ngModule: LibModule,
            providers: [
                CiceroCommandFactoryService,
                CiceroRendererService,
                CiceroContextService,
            ]
        };
    }
}
