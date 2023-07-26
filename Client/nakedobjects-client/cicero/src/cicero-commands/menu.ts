import * as Ro from '@nakedobjects/restful-objects';
import filter from 'lodash-es/filter';
import reduce from 'lodash-es/reduce';
import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Menu extends Command {

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

    override shortCommand = 'me';
    override fullCommand = Usermessages.menuCommand;
    override helpText = Usermessages.menuHelp;
    protected override minArguments = 0;
    protected override maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string | null, _chained: boolean): Promise<CommandResult> {
        const name = this.argumentAsString(args, 0);
        return this.context.getMenus()
            .then((menus: Ro.MenusRepresentation) => {
                let links = menus.value();
                if (name) {
                    // TODO: do multi-clause match
                    const exactMatches = filter(links, t => (t.title() || '').toLowerCase() === name);
                    const partialMatches = filter(links, t => (t.title() || '').toLowerCase().indexOf(name) > -1);
                    links = exactMatches.length === 1 ? exactMatches : partialMatches;
                }
                switch (links.length) {
                    case 0:
                        return this.returnResult('', Usermessages.doesNotMatchMenu(name));
                    case 1: {
                        const menuId = links[0].rel()?.parms[0].value;
                        this.urlManager.setHome();
                        this.urlManager.clearUrlState(1);
                        return this.returnResult('', '', () => { if (menuId) { this.urlManager.setMenu(menuId);}});
                    }
                    default: {
                        const label = name ? `${Usermessages.matchingMenus}\n` : `${Usermessages.allMenus}\n`;
                        const ss = reduce(links, (s, t) => s + t.title() + '\n', label);
                        return this.returnResult('', ss);
                    }
                }
            });
    }
}
