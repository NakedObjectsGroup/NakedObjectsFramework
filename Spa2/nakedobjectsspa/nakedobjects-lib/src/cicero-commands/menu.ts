import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Models from '../models';
import * as Usermessages from '../user-messages';
import filter from 'lodash-es/filter';
import reduce from 'lodash-es/reduce';

export class Menu extends Command {

    shortCommand = "me";
    fullCommand = Usermessages.menuCommand;
    helpText = Usermessages.menuHelp;
    protected minArguments = 0;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        const name = this.argumentAsString(args, 0);
        return this.context.getMenus()
            .then((menus: Models.MenusRepresentation) => {
                let links = menus.value();
                if (name) {
                    // TODO: do multi-clause match
                    const exactMatches = filter(links, t => (t.title() || "").toLowerCase() === name);
                    const partialMatches = filter(links, t => (t.title() || "").toLowerCase().indexOf(name) > -1);
                    links = exactMatches.length === 1 ? exactMatches : partialMatches;
                }
                switch (links.length) {
                case 0:
                    return this.returnResult("", Usermessages.doesNotMatchMenu(name));
                case 1:
                    const menuId = links[0].rel().parms[0].value!;
                    this.urlManager.setHome();
                    this.urlManager.clearUrlState(1);
                    return this.returnResult("", "", () => this.urlManager.setMenu(menuId));

                default:
                    const label = name ? `${Usermessages.matchingMenus}\n` : `${Usermessages.allMenus}\n`;
                    const ss = reduce(links, (s, t) => s + t.title() + "\n", label);
                    return this.returnResult("", ss);
                }
            });
    }
}
