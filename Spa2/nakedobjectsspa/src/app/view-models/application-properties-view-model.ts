import * as Ro from '../ro-interfaces';
import { ContextService } from '../context.service';
import { ErrorService } from '../error.service';
import { ConfigService } from '../config.service';
import * as Models from '../models';
import * as Constants from '../constants';

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
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.context.getVersion().
            then((v: Models.VersionRepresentation) => this.serverVersion = v.wrapped()).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.serverUrl = this.configService.config.appPath;

        this.clientVersion = Constants.clientVersion;

        this.applicationName = this.configService.config.applicationName;
    }
}
