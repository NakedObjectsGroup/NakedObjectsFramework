import * as Ro from '@nakedobjects/restful-objects';
import { ErrorWrapper } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult, getParametersAndCurrentValue } from './command-result';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class OK extends Command {

    constructor(urlManager: UrlManagerService,
        location: Location,
        commandFactory: CiceroCommandFactoryService,
        context: ContextService,
        mask: MaskService,
        error: ErrorService,
        configService: ConfigService,
        ciceroContext: CiceroContextService,
        ciceroRenderer: CiceroRendererService,
    )  {
        super(urlManager, location, commandFactory, context, mask, error, configService, ciceroContext, ciceroRenderer);
    }

    override shortCommand = 'ok';
    override fullCommand = Usermessages.okCommand;
    override helpText = Usermessages.okHelp;
    protected override minArguments = 0;
    protected override maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        return this.getActionForCurrentDialog().then((action: Ro.ActionRepresentation | Ro.InvokableActionMember) => {

            if (chained && action.isNotQueryOnly()) {
                return this.returnResult('', this.mayNotBeChained(Usermessages.queryOnlyRider), () => { }, true);
            }

            let fieldMap: Dictionary<Ro.Value>;
            if (this.isForm()) {
                const obj = action.parent as Ro.DomainObjectRepresentation;
                fieldMap = this.context.getObjectCachedValues(obj.id()); // Props passed in as pseudo-params to action
            } else {
                fieldMap = getParametersAndCurrentValue(action, this.context);
            }

            return this.context.invokeAction(action, fieldMap).then((result: Ro.ActionResultRepresentation) => {

                return this.returnResult('', null, () => this.urlManager.closeDialogReplaceHistory(this.routeData().dialogId!));

            }).catch((reject: ErrorWrapper) => {
                if (reject.error instanceof Ro.ErrorMap) {
                    const paramFriendlyName = (paramId: string) => Ro.friendlyNameForParam(action, paramId);
                    return this.handleErrorResponse(reject.error as Ro.ErrorMap, paramFriendlyName);
                }
                return Promise.reject(reject);
            });
        });
    }
}
