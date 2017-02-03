import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import './rxjs-extensions';
import { IRepresentation } from './ro-interfaces';
import { ConfigService } from './config.service';
import * as Models from './models';

@Injectable()
export class RepresentationsService {
    constructor(private readonly http: Http, private readonly configService: ConfigService) {}

    getHomeRepresentation(): Observable<Models.HomePageRepresentation> {
        return this.http
            .get(this.configService.config.appPath)
            .map((r: Response) =>
                new Models.HomePageRepresentation(r.json() as IRepresentation, this.configService.config.appPath));
    }

    getRepresentation<T extends Models.HateosModel>(url: string): Observable<T> {
        return this.http
            .get(url)
            .map((r: Response) => r.json() as T);
    }

    getRepresentationFromLink<T extends Models.HateosModel>(link: Models.Link): Observable<T> {
        return this.http
            .get(link.href())
            .map((r: Response) => {
                const tgt = link.getTarget();
                tgt.populate(r.json() as IRepresentation);
                return tgt as T;
            });
    }


}