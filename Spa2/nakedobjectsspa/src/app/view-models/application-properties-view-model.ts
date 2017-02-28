import * as Ro from '../ro-interfaces';
import * as Contextservice from '../context.service';
import * as Errorservice from '../error.service';
import * as Configservice from '../config.service';
import * as Models from '../models';
import * as Constants from '../constants';

export class ApplicationPropertiesViewModel {

    constructor(
        private readonly context: Contextservice.ContextService,
        private readonly error: Errorservice.ErrorService,
        private readonly configService: Configservice.ConfigService
    ) {
        this.setUp();
    }

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


    serverVersion: Ro.IVersionRepresentation;
    user: Ro.IUserRepresentation;
    serverUrl: string;
    clientVersion: string;
    applicationName : string;
}