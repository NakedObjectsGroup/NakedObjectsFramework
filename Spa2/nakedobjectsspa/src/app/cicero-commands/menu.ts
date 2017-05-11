import * as Cicerocommands from './command-result';
import * as Models from '../models';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import map from 'lodash/map';
import some from 'lodash/some';
import filter from 'lodash/filter';
import every from 'lodash/every';
import each from 'lodash/each';
import forEach from 'lodash/forEach';
import findIndex from 'lodash/findIndex';
import reduce from 'lodash/reduce';
import { Location } from '@angular/common';

export class Menu extends Command.Command {

    fullCommand = Usermessages.menuCommand;
    helpText = Usermessages.menuHelp;
    protected minArguments = 0;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

   

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        const name = this.argumentAsString(args, 0);
        return this.context.getMenus()
            .then((menus: Models.MenusRepresentation) => {
                var links = menus.value();
                if (name) {
                    //TODO: do multi-clause match
                    const exactMatches = filter(links, (t) => { return t.title().toLowerCase() === name; });
                    const partialMatches = filter(links, (t) => { return t.title().toLowerCase().indexOf(name) > -1; });
                    links = exactMatches.length === 1 ? exactMatches : partialMatches;
                }
                switch (links.length) {
                case 0:
                    return this.returnResult("", Usermessages.doesNotMatchMenu(name));
                case 1:
                    const menuId = links[0].rel().parms[0].value;
                    this.urlManager.setHome();
                    this.urlManager.clearUrlState(1);
                    return this.returnResult("", "", () => this.urlManager.setMenu(menuId));

                default:
                    const label = name ? `${Usermessages.matchingMenus}\n` : `${Usermessages.allMenus}\n`;
                    const ss = reduce(links, (s, t) => { return s + t.title() + "\n"; }, label);
                    return this.returnResult("", ss);
                }
            });
    };
}