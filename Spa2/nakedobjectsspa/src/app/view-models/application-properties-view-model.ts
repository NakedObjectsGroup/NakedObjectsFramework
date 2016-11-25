import * as Ro from '../ro-interfaces';

// todo do we need this ?
export class ApplicationPropertiesViewModel {
    serverVersion: Ro.IVersionRepresentation;
    user: Ro.IUserRepresentation;
    serverUrl: string;
    clientVersion: string;
}