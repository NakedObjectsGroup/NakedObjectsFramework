import * as Ro from '@nakedobjects/restful-objects';
import { ColorService, ConfigService, ContextService, ErrorService, ErrorWrapper, Pane, UrlManagerService } from '@nakedobjects/services';
import { ChoiceViewModel } from './choice-view-model';
import * as Helpers from './helpers-view-models';
import { IDraggableViewModel } from './idraggable-view-model';

export class LinkViewModel implements IDraggableViewModel {

    constructor(
        protected readonly context: ContextService,
        protected readonly colorService: ColorService,
        protected readonly error: ErrorService,
        protected readonly urlManager: UrlManagerService,
        private readonly configService: ConfigService,
        public readonly link: Ro.Link,
        public readonly paneId: Pane
    ) {

        this.title = link.title() + Helpers.dirtyMarker(this.context, this.configService, link.getOid(this.configService.config.keySeparator));
        this.domainType = link.type().domainType;

        // for dropping
        const value = new Ro.Value(link);

        this.value = value.toString();
        this.reference = value.toValueString();
        this.selectedChoice = new ChoiceViewModel(value, '');
        this.draggableType = this.domainType;

        this.colorService.toColorNumberFromHref(link.href()).
            then(c => this.color = `${this.configService.config.linkColor}${c}`).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));
    }

    readonly title: string;
    private readonly domainType: string;

    // IDraggableViewModel
    color: string;
    readonly value: Ro.ScalarValueType;
    readonly reference: string;
    readonly selectedChoice: ChoiceViewModel;
    readonly draggableType: string;

    readonly draggableTitle = () => this.title;

    readonly canDropOn = (targetType: string) => this.context.isSubTypeOf(this.domainType, targetType);

    // because may be clicking on menu already open so want to reset focus
    readonly doClick = (right?: boolean) => this.urlManager.setMenu(this.link.rel().parms[0].value!, this.paneId);
}
