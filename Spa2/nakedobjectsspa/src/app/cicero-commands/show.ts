import * as Cicerocommands from './command-result';
import * as Models from '../models';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import * as Cicerorendererservice from '../cicero-renderer.service';
import * as Routedata from '../route-data';
import map from 'lodash/map';
import some from 'lodash/some';
import filter from 'lodash/filter';
import every from 'lodash/every';
import each from 'lodash/each';
import forEach from 'lodash/forEach';
import reduce from 'lodash/reduce';
import { Location } from '@angular/common';

export class Show extends Command.Command {

    fullCommand = Usermessages.showCommand;
    helpText = Usermessages.showHelp;
    protected minArguments = 0;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isCollection() || this.isList();
    }

   

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        if (this.isCollection()) {
            const arg = this.argumentAsString(args, 0, true);
            try {
                const { start, end } = this.parseRange(arg);
                return this.getObject().then(obj => {
                    const openCollIds = Cicerorendererservice.openCollectionIds(this.routeData());
                    const coll = obj.collectionMember(openCollIds[0]);
                    return this.renderCollectionItems(coll, start, end);
                });
            }
            catch (e1) {
                return this.returnResult("", e1.message);
            }
        } else if (this.isList()) {
            const arg = this.argumentAsString(args, 0, true);
            try {
                const { start, end } = this.parseRange(arg);
                return this.getList().then(list => this.renderItems(list, start, end));
            }
            catch (e2) {
                return this.returnResult("", e2.message);
            }
        } else if (this.isObject()) {
            const fieldName = this.argumentAsString(args, 0);
            return this.getObject().then((obj: Models.DomainObjectRepresentation) => {
                const props = this.matchingProperties(obj, fieldName);
                const colls = this.matchingCollections(obj, fieldName);
                //TODO -  include these
                let s: string;
                switch (props.length + colls.length) {
                case 0:
                    s = fieldName ? Usermessages.doesNotMatch(fieldName) : Usermessages.noVisible;
                    break;
                case 1:
                    s = props.length > 0 ? this.renderPropNameAndValue(props[0]) : Cicerorendererservice.renderCollectionNameAndSize(colls[0]);
                    break;
                default:
                    s = reduce(props, (s, prop) => s + this.renderPropNameAndValue(prop), "");
                    s += reduce(colls, (s, coll) => s + Cicerorendererservice.renderCollectionNameAndSize(coll), "");
                }
                return this.returnResult("", s);
            });
        }
    };

    private renderPropNameAndValue(pm: Models.PropertyMember): string {
        const name = pm.extensions().friendlyName();
        let value: string;
        const parent = pm.parent as Models.DomainObjectRepresentation;
        const props = this.context.getObjectCachedValues(parent.id());
        const modifiedValue = props[pm.id()];
        if (this.isEdit() && !pm.disabledReason() && modifiedValue) {
            value = Cicerorendererservice.renderFieldValue(pm, modifiedValue, this.mask) + ` (${Usermessages.modified})`;
        } else {
            value = Cicerorendererservice.renderFieldValue(pm, pm.value(), this.mask);
        }
        return `${name}: ${value}\n`;
    }

    private renderCollectionItems(coll: Models.CollectionMember, startNo: number, endNo: number) {
        if (coll.value()) {
            return this.renderItems(coll, startNo, endNo);
        } else {
            return this.context.getCollectionDetails(coll, Routedata.CollectionViewState.List, false).
                then(details => this.renderItems(details, startNo, endNo));
        }
    }

    private renderItems(source: Models.IHasLinksAsValue, startNo: number, endNo: number) {
        //TODO: problem here is that unless collections are in-lined value will be null.
        const max = source.value().length;
        if (!startNo) {
            startNo = 1;
        }
        if (!endNo) {
            endNo = max;
        }
        if (startNo > max || endNo > max) {
           
            return this.returnResult("", Usermessages.highestItem(source.value().length));
        }
        if (startNo > endNo) {
            
            return this.returnResult("", Usermessages.startHigherEnd);
        }
        let output = "";
        let i: number;
        const links = source.value();
        for (i = startNo; i <= endNo; i++) {
            output += `${Usermessages.item} ${i}: ${links[i - 1].title()}\n`;
        }
       
        return this.returnResult("", output);
    }
}