import * as Ro from '@nakedobjects/restful-objects';
import { CollectionViewState } from '@nakedobjects/services';
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
import { messageFrom } from '../helpers-components';

export class Goto extends Command {

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

    override shortCommand = 'go';
    override fullCommand = Usermessages.gotoCommand;
    override helpText = Usermessages.gotoHelp;
    protected override minArguments = 1;
    protected override maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isList();
    }

    doExecute(args: string | null, _chained: boolean): Promise<CommandResult> {
        const arg0 = this.argumentAsString(args, 0);

        if (arg0 === undefined) {
            return this.returnResult('', Usermessages.outOfItemRange(undefined));
        }

        if (this.isList()) {
            let itemNo: number;
            try {
                itemNo = this.parseInt(arg0);
            } catch (e) {
                return this.returnResult('', messageFrom(e));
            }
            return this.getList().then((list: Ro.ListRepresentation) => this.attemptGotoLinkNumber(itemNo, list.value()));
        }
        if ((this.isObject())) {

            return this.getObject().then((obj: Ro.DomainObjectRepresentation) => {
                if (this.isCollection()) {
                    const itemNo = this.argumentAsNumber(args, 0)!;
                    const openCollIds = this.ciceroRenderer.openCollectionIds(this.routeData());
                    const coll = obj.collectionMember(openCollIds[0]);
                    // Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                    return this.context.getCollectionDetails(coll, CollectionViewState.List, false).then(details => this.attemptGotoLinkNumber(itemNo, details.value()!));

                } else {
                    const matchingProps = this.matchingProperties(obj, arg0);
                    const matchingRefProps = filter(matchingProps, p => !p.isScalar());
                    const matchingColls = this.matchingCollections(obj, arg0);

                    switch (matchingRefProps.length + matchingColls.length) {
                        case 0:

                            return this.returnResult('', Usermessages.noRefFieldMatch(arg0));
                        case 1:
                            // TODO: Check for any empty reference
                            if (matchingRefProps.length > 0) {
                                const link = matchingRefProps[0].value().link();
                                if (link) {
                                    this.urlManager.setItem(link);
                                }

                                return this.returnResult('', '');

                            } else { // Must be collection

                                return this.returnResult('', '', () => this.openCollection(matchingColls[0]));
                            }

                        default: {
                            const props = reduce(matchingRefProps, (str, prop) => str + prop.extensions().friendlyName() + '\n', '');
                            const colls = reduce(matchingColls, (str, coll) => str + coll.extensions().friendlyName() + '\n', '');
                            const s = `Multiple matches for ${arg0}:\n${props}${colls}`;
                            return this.returnResult('', s);
                        }
                    }

                }
            });
        }
        // should never happen
        return this.returnResult('', Usermessages.commandNotAvailable(this.fullCommand));
    }

    private attemptGotoLinkNumber(itemNo: number, links: Ro.Link[]): Promise<CommandResult> {
        if (itemNo < 1 || itemNo > links.length) {
            return this.returnResult('', Usermessages.outOfItemRange(itemNo));
        } else {
            const link = links[itemNo - 1]; // On UI, first item is '1'
            return this.returnResult('', '', () => this.urlManager.setItem(link));
        }
    }

    private openCollection(collection: Ro.CollectionMember): void {
        this.closeAnyOpenCollections();
        this.urlManager.setCollectionMemberState(collection.collectionId(), CollectionViewState.List);
    }
}
