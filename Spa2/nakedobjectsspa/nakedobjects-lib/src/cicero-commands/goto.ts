import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Models from '../models';
import * as Usermessages from '../user-messages';
import * as Routedata from '../route-data';
import filter from 'lodash-es/filter';
import reduce from 'lodash-es/reduce';

export class Goto extends Command {

    shortCommand = "go";
    fullCommand = Usermessages.gotoCommand;
    helpText = Usermessages.gotoHelp;
    protected minArguments = 1;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isList();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        const arg0 = this.argumentAsString(args, 0);

        if (arg0 === undefined) {
            return this.returnResult("", Usermessages.outOfItemRange(undefined));
        }

        if (this.isList()) {
            let itemNo: number;
            try {
                itemNo = this.parseInt(arg0);
            } catch (e) {
                return this.returnResult("", e.message);
            }
            return this.getList().then((list: Models.ListRepresentation) => this.attemptGotoLinkNumber(itemNo, list.value()));
        }
        if (this.isObject) {

            return this.getObject().then((obj: Models.DomainObjectRepresentation) => {
                if (this.isCollection()) {
                    const itemNo = this.argumentAsNumber(args, 0)!;
                    const openCollIds = this.ciceroRenderer.openCollectionIds(this.routeData());
                    const coll = obj.collectionMember(openCollIds[0]);
                    // Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                    return this.context.getCollectionDetails(coll, Routedata.CollectionViewState.List, false).then(details => this.attemptGotoLinkNumber(itemNo, details.value()));

                } else {
                    const matchingProps = this.matchingProperties(obj, arg0);
                    const matchingRefProps = filter(matchingProps, p => !p.isScalar());
                    const matchingColls = this.matchingCollections(obj, arg0);

                    switch (matchingRefProps.length + matchingColls.length) {
                    case 0:

                        return this.returnResult("", Usermessages.noRefFieldMatch(arg0));
                    case 1:
                        // TODO: Check for any empty reference
                        if (matchingRefProps.length > 0) {
                            const link = matchingRefProps[0].value().link();
                            if (link) {
                                this.urlManager.setItem(link);
                            }

                            return this.returnResult("", "");

                        } else { // Must be collection

                            return this.returnResult("", "", () => this.openCollection(matchingColls[0]));
                        }

                    default:
                        const props = reduce(matchingRefProps, (str, prop) => str + prop.extensions().friendlyName() + "\n", "");
                        const colls = reduce(matchingColls, (str, coll) => str + coll.extensions().friendlyName() + "\n", "");
                        const s = `Multiple matches for ${arg0}:\n${props}${colls}`;
                        return this.returnResult("", s);
                    }

                }
            });
        }
        // should never happen
        return this.returnResult("", Usermessages.commandNotAvailable(this.fullCommand));
    }

    private attemptGotoLinkNumber(itemNo: number, links: Models.Link[]): Promise<CommandResult> {
        if (itemNo < 1 || itemNo > links.length) {
            return this.returnResult("", Usermessages.outOfItemRange(itemNo));
        } else {
            const link = links[itemNo - 1]; // On UI, first item is '1'
            return this.returnResult("", "", () => this.urlManager.setItem(link));
        }
    }

    private openCollection(collection: Models.CollectionMember): void {
        this.closeAnyOpenCollections();
        this.urlManager.setCollectionMemberState(collection.collectionId(), Routedata.CollectionViewState.List);
    }
}
