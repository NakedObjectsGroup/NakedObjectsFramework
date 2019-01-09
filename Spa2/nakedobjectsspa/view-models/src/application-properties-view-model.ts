import * as Ro from '@nakedobjects/restful-objects';
import { ConfigService, ContextService, ErrorService, ErrorWrapper } from '@nakedobjects/services';
import * as Constants from './constants';

export class ApplicationPropertiesViewModel {

    constructor(
        private readonly context: ContextService,
        private readonly error: ErrorService,
        private readonly configService: ConfigService
    ) {
        this.setUp();
    }

    serverVersion: Ro.IVersionRepresentation;
    user: Ro.IUserRepresentation;
    serverUrl: string;
    clientVersion: string;
    applicationName: string;

    private setUp() {
        this.context.getUser().
            then((u: Ro.UserRepresentation) => this.user = u.wrapped()).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));

        this.context.getVersion().
            then((v: Ro.VersionRepresentation) => this.serverVersion = v.wrapped()).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));

        this.serverUrl = this.configService.config.appPath;

        this.clientVersion = Constants.clientVersion;

        this.applicationName = this.configService.config.applicationName;
    }
}
