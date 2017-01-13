import { Injectable } from '@angular/core';
import * as moment from 'moment';

// todo this whole service is a hack becuase I can't get moment to work through 
// webpack remove and try again later 
@Injectable()
export class MomentWrapperService {
    moment(date: string, format?: any, language?: string, strict?: boolean) {
        return moment.utc(date, format, language, strict);
    }
}
