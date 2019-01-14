import { HttpClientModule } from '@angular/common/http';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { CiceroCommandFactoryService } from './cicero-command-factory.service';
import { CiceroContextService } from './cicero-context.service';
import { CiceroRendererService } from './cicero-renderer.service';
import { CiceroComponent } from './cicero/cicero.component';

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
