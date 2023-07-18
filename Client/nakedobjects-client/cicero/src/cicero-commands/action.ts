import * as Ro from '@nakedobjects/restful-objects';
import { Location } from '@angular/common';
import { Dictionary, result } from 'lodash';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import reduce from 'lodash-es/reduce';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';
import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';

export class Action extends Command {

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

    override shortCommand = 'ac';
    override fullCommand = Usermessages.actionCommand;

    override helpText = Usermessages.actionHelp;
    protected override minArguments = 0;
    protected override maxArguments = 2;

    isAvailableInCurrentContext(): boolean {
        return (this.isMenu() || this.isObject() || this.isForm()) && !this.isDialog() && !this.isEdit(); // TODO add list
    }

    doExecute(args: string | null, chained: boolean, result: CommandResult): Promise<CommandResult> {
        const match = this.argumentAsString(args, 0);
        const details = this.argumentAsString(args, 1, true);
        if (details && details !== '?') {
            return this.returnResult('', Usermessages.mustbeQuestionMark);
        }
        if (this.isObject()) {
            return this.getObject().then(obj => this.processActions(match, obj.actionMembers(), details));

        } else if (this.isMenu()) {
            return this.getMenu().then(menu => this.processActions(match, menu.actionMembers(), details));
        }
        // TODO: handle list - CCAs
        return Promise.reject('TODO: handle list - CCAs');
    }

    private processActions(match: string | undefined, actionsMap: Dictionary<Ro.ActionMember>, details: string | undefined): Promise<CommandResult> {
        let actions = map(actionsMap, action => action);
        if (actions.length === 0) {
            return this.returnResult('', Usermessages.noActionsAvailable);
        }
        if (match) {
            actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
        }
        switch (actions.length) {
            case 0:
                return this.returnResult('', Usermessages.doesNotMatchActions(match));
            case 1: {
                const action = actions[0];
                if (details) {
                    return this.returnResult('', this.renderActionDetails(action));
                } else if (action.disabledReason()) {
                    return this.returnResult('', this.disabledAction(action));
                } else {
                    return this.openActionDialog(action);
                }
            }
            default: {
                let output = match ? Usermessages.matchingActions : Usermessages.actionsMessage;
                output += this.listActions(actions);
                return this.returnResult('', output);
            }
        }
    }

    private disabledAction(action: Ro.ActionMember) {
        return `${Usermessages.actionPrefix} ${action.extensions().friendlyName()} ${Usermessages.isDisabled} ${action.disabledReason()}`;
    }

    private listActions(actions: Ro.ActionMember[]): string {
        return reduce(actions,
            (s, t) => {
                const menupath = t.extensions().menuPath() ? `${t.extensions().menuPath()} - ` : '';
                const disabled = t.disabledReason() ? ` (${Usermessages.disabledPrefix} ${t.disabledReason()})` : '';
                return s + menupath + t.extensions().friendlyName() + disabled + '\n';
            },
            '');
    }

    private openActionDialog(action: Ro.ActionMember): Promise<CommandResult> {

        return this.context.getInvokableAction(action).
            then(invokable => {

                this.context.clearDialogCachedValues();
                this.urlManager.setDialog(action.actionId());
                forEach(invokable.parameters(), p => this.setFieldValueInContext(p, p.default()));

                return this.returnResult('', '');
            });
    }

    private renderActionDetails(action: Ro.ActionMember) {
        return `${Usermessages.descriptionPrefix} ${action.extensions().friendlyName()}\n${action.extensions().description() || Usermessages.noDescription}`;
    }
}
