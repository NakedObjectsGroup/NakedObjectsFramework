import * as Ro from '../ro-interfaces';

export class ApplicationPropertiesViewModel {
    serverVersion: Ro.IVersionRepresentation;
    user: Ro.IUserRepresentation;
    serverUrl: string;
    clientVersion: string;
}