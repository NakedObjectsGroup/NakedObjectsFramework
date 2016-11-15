import * as Rointerfaces from '../ro-interfaces';

export class ApplicationPropertiesViewModel {
    serverVersion: Rointerfaces.IVersionRepresentation;
    user: Rointerfaces.IUserRepresentation;
    serverUrl: string;
    clientVersion: string;
}