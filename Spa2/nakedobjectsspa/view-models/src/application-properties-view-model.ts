import * as Ro from '@nakedobjects/restful-objects';
import { ContextService } from '@nakedobjects/services';
import { ErrorService } from '@nakedobjects/services';
import { ConfigService } from '@nakedobjects/services';
import * as Models from '@nakedobjects/restful-objects';
import * as Constants from './constants';
import { ErrorWrapper } from '@nakedobjects/services';

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
            then((u: Models.UserRepresentation) => this.user = u.wrapped()).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));

        this.context.getVersion().
            then((v: Models.VersionRepresentation) => this.serverVersion = v.wrapped()).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));

        this.serverUrl = this.configService.config.appPath;

        this.clientVersion = Constants.clientVersion;

        this.applicationName = this.configService.config.applicationName;
    }
}
