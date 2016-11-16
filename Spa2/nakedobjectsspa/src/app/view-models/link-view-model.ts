import { IDraggableViewModel } from './idraggable-view-model';
import { ChoiceViewModel } from './choice-view-model';
import * as Ro from "../ro-interfaces";
import * as Models from "../models";
import * as Config from "../config";
import { ContextService } from '../context.service';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { UrlManagerService } from '../url-manager.service';

export class LinkViewModel implements IDraggableViewModel {

    constructor(protected context: ContextService,
                protected colorService: ColorService,
                protected error: ErrorService,
                protected urlManager: UrlManagerService,
                public link: Models.Link,
                protected paneId: number) {

        this.title = link.title() + Models.dirtyMarker(this.context, link.getOid());
        this.domainType = link.type().domainType;

        // for dropping 
        const value = new Models.Value(link);

        this.value = value.toString();
        this.reference = value.toValueString();
        this.selectedChoice = ChoiceViewModel.create(value, "");
        this.draggableType = this.domainType;

        this.colorService.toColorNumberFromHref(link.href()).
            then(c => this.color = `${Config.linkColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    }

    title: string;
    domainType: string;

    // IDraggableViewModel 
    color: string;
    value: Ro.scalarValueType;
    reference: string;
    selectedChoice: ChoiceViewModel;
    draggableType: string;

    draggableTitle = () => this.title;

    canDropOn = (targetType: string) => this.context.isSubTypeOf(this.domainType, targetType);

    // because may be clicking on menu already open so want to reset focus
    doClick = (right?: boolean) => this.urlManager.setMenu(this.link.rel().parms[0].value, this.paneId);
}