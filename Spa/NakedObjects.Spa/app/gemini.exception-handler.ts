import { ExceptionHandler, Injector, Injectable } from '@angular/core';
import { Context } from './context.service';
import { UrlManager } from './urlmanager.service';
import * as Models from './models';

@Injectable()
export class GeminiExceptionHandler extends ExceptionHandler {

   
    constructor() {
      
        super(null, null);

    }
   


    call(error : any, stackTrace : any = null, reason : any = null) {
        const rp = new Models.ErrorWrapper(Models.ErrorCategory.ClientError, Models.ClientErrorCode.SoftwareError, error.message);
        rp.stackTrace = stackTrace ? stackTrace.split("\n") : [];

        //const context = this.injector.get(Context);


        //this.context.setError(rp);
        //this.urlManager.setError(Models.ErrorCategory.ClientError, Models.ClientErrorCode.SoftwareError);
    }

   
}