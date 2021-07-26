import * as Ro from '@nakedobjects/restful-objects';
import { ConfigService, ContextService, ErrorService, ErrorWrapper } from '@nakedobjects/services';
import * as Msg from './user-messages';

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
    applicationName: string;

    get userName() {
        return this.user ? this.user.userName : Msg.noUserMessage;
    }

    private setUp() {
        this.context.getUser().
            then((u: Ro.UserRepresentation) => this.user = u.wrapped()).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));

        this.context.getVersion().
            then((v: Ro.VersionRepresentation) => this.serverVersion = v.wrapped()).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));

        this.serverUrl = this.configService.config.appPath;

        this.applicationName = this.configService.config.applicationName;
    }
}
