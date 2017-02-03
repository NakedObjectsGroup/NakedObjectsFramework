import { Component, Input, OnInit, OnDestroy, AfterViewInit, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { RepresentationsService } from "../representations.service";
import { ActivatedRoute } from '@angular/router';
import * as Models from "../models";
import { UrlManagerService } from "../url-manager.service";
import { ClickHandlerService } from "../click-handler.service";
import { ContextService } from "../context.service";
import { RepLoaderService } from "../rep-loader.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ErrorService } from "../error.service";
import { MaskService } from "../mask.service";
import { PaneRouteData, RouteData, InteractionMode } from "../route-data";
import { FormBuilder, FormGroup, FormControl, AbstractControl } from '@angular/forms';
import * as _ from "lodash";
import { PropertyViewModel } from '../view-models/property-view-model';
import { CollectionViewModel } from '../view-models/collection-view-model';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { PaneComponent } from '../pane/pane';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import { IButton } from '../button/button.component';
import { ColorService } from '../color.service';
import { ConfigService } from '../config.service';
import { copy } from '../view-models/idraggable-view-model';

@Component({
    selector: 'nof-object',
    templateUrl: './object.component.html',
    styleUrls: ['./object.component.css']
})
export class ObjectComponent extends PaneComponent implements OnInit, OnDestroy, AfterViewInit {
    // todo  this and ListComponent should not  extend PaneComponent they are no longer panes !

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        private readonly context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly colorService: ColorService,
        private readonly error: ErrorService,
        private readonly formBuilder: FormBuilder,
        private readonly configService: ConfigService
    ) {
        super(activatedRoute, urlManager);
    }

    // template API 
    expiredTransient = false;
    object: DomainObjectViewModel | null;

    // todo add mode as string property for template to make easier to read and make this private 
    mode: InteractionMode | null;
    form: FormGroup | null;

    // must be properties as object may change - eg be reloaded 
    get friendlyName() {
        const obj = this.object;
        return obj ? obj.friendlyName : "";
    }

    get color() {
        const obj = this.object;
        return obj ? obj.color : this.backgroundColor;
    }

    get properties() {
        const obj = this.object;
        return obj ? obj.properties : "";
    }

    get collections() {
        const obj = this.object;
        return obj ? obj.collections : "";
    }

    get tooltip(): string {
        const obj = this.object;
        return obj ? obj.tooltip() : "";
    }

    onSubmit(viewObject: boolean) {
        const obj = this.object;
        if (obj) {
            obj.doSave(viewObject);
        }
    }


    copy(event: KeyboardEvent) {
        const obj = this.object;
        if (obj) {
            copy(event, obj, this.context);
        }
    }

    title() {
        // todo add string consts to user messages !
        const obj = this.object;
        const prefix = this.mode === InteractionMode.Edit || this.mode === InteractionMode.Transient ? "Editing - " : "";
        return obj ? `${prefix}${obj.title}` : "";
    }

    // todo investigate if logic in this would be better here rather than view model

    disableActions = () => {
        const obj = this.object;
        return obj && obj.disableActions() ? true : null;
    }

    actionsTooltip = () => {
        const obj = this.object;
        return obj ? obj.actionsTooltip() : "";
    }

    unsaved = () => {
        const obj = this.object;
        return !!obj && obj.unsaved;
    }

    private do(f: (o: DomainObjectViewModel) => void) {
        const obj = this.object;
        if (obj) {
            f(obj);
        }
    }

    toggleActionMenu = () => {
        this.do((o) => o.toggleActionMenu());
    }

    doEdit = () => {
        this.do((o) => o.doEdit());
    }

    doEditCancel = () => {
        this.do((o) => o.doEditCancel());
    }

    showEdit = () => {
        const obj = this.object;
        return !!obj && !obj.hideEdit();
    }

    doReload = () => {
        this.do((o) => o.doReload());
    }

    message = () => {
        const obj = this.object;
        return obj ? obj.getMessage() : "";
    }

    showActions = () => {
        const obj = this.object;
        return !!obj && obj.showActions();
    }

    menuItems = () => {
        const obj = this.object;
        return obj ? obj.menuItems : [];
    }

    private actionButton: IButton = {
        value: "Actions",
        doClick: () => this.toggleActionMenu(),
        show: () => true,
        disabled: () => this.disableActions(),
        title: () => this.actionsTooltip(),
        accesskey: "a"
    }

    private editButton: IButton = {
        value: "Edit",
        doClick: () => this.doEdit(),
        show: () => this.showEdit(),
        disabled: () => null,
        title: () => "",
        accesskey: null
    }

    private reloadButton: IButton = {
        value: "Reload",
        doClick: () => this.doReload(),
        show: () => true,
        disabled: () => null,
        title: () => "",
        accesskey: null
    }

    private saveButton: IButton = {
        value: "Save",
        doClick: () => this.onSubmit(true),
        show: () => true,
        disabled: () => this.form ? !this.form.valid : true,
        title: () => this.tooltip,
        accesskey: null
    }

    private saveAndCloseButton: IButton = {
        value: "Save & Close",
        doClick: () => this.onSubmit(false),
        show: () => this.unsaved(),
        disabled: () => this.form ? !this.form.valid : true,
        title: () => this.tooltip,
        accesskey: null
    }

    private cancelButton: IButton = {
        value: "Cancel",
        doClick: () => this.doEditCancel(),
        show: () => true,
        disabled: () => null,
        title: () => "",
        accesskey: null
    }

    get buttons() {
        if (this.mode === InteractionMode.View) {
            return [this.actionButton, this.editButton, this.reloadButton];
        }

        if (this.mode === InteractionMode.Edit) {
            return [this.saveButton, this.saveAndCloseButton, this.cancelButton];
        }

        if (this.mode === InteractionMode.Transient || this.mode === InteractionMode.Form) {

            const menuItems = this.menuItems() !;
            const actions = _.flatten(_.map(menuItems, (mi: MenuItemViewModel) => mi.actions!));

            return _.map(actions, a => ({
                value: a.title,
                doClick: () => a.doInvoke(),
                doRightClick: () => a.doInvoke(true),
                show: () => true,
                disabled: () => a.disabled() ? true : null,
                title: () => a.description,
                accesskey: null
            })) as IButton[];
        }

        return [] as IButton[];
    }

    // todo that we access viewmodel directly in template from this I think is smell that we should have a 
    // child component here 
    actions = (item: MenuItemViewModel) => item.actions;

    // todo each component should be looking out for it's own changes - make this generic - eg 
    // component can register for changes it's wants to see rather  than this horrible filtering 
    // I'm doing everywhere 

    protected setup(routeData: PaneRouteData) {
        // subscription means may get with no oid 

        if (!routeData.objectId) {
            this.mode = null;
            return;
        }

        this.expiredTransient = false;

        const oid = Models.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator);

        // todo this is a recurring pattern in angular 2 code - generalise 
        // across components 
        if (this.object && !this.object.domainObject.getOid(this.configService.config.keySeparator).isSame(oid)) {
            // object has changed - clear existing 
            this.object = null;
            this.form = null;
        }

        const isChanging = !this.object;

        const modeChanging = this.mode !== routeData.interactionMode;

        this.mode = routeData.interactionMode;

        const wasDirty = this.context.getIsDirty(oid);

        this.selectedDialogId = routeData.dialogId;

        if (isChanging || modeChanging || wasDirty) {

            // set background color at once to smooth transition
            this.colorService.toColorNumberFromType(oid.domainType).then(c => this.backgroundColor = `${this.configService.config.objectColor}${c}`);

            this.context.getObject(routeData.paneId, oid, routeData.interactionMode)
                .then((object: Models.DomainObjectRepresentation) => {

                    // only change the object property if the object has changed 
                    if (isChanging || wasDirty) {
                        this.object = this.viewModelFactory.domainObjectViewModel(object, routeData, wasDirty);
                    }

                    if (modeChanging || isChanging) {
                        if (this.mode === InteractionMode.Edit ||
                            this.mode === InteractionMode.Form ||
                            this.mode === InteractionMode.Transient) {
                            this.createForm(this.object as DomainObjectViewModel); // will never be null
                        }
                    }
                })
                .catch((reject: Models.ErrorWrapper) => {
                    if (reject.category === Models.ErrorCategory.ClientError && reject.clientErrorCode === Models.ClientErrorCode.ExpiredTransient) {
                        this.context.setError(reject);
                        this.expiredTransient = true;
                    } else {
                        this.error.handleError(reject);
                    }
                });
        }
    }

    private createForm(vm: DomainObjectViewModel) {
        const pps = vm.properties;
        const props = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p)) as _.Dictionary<PropertyViewModel>;
        const editableProps = _.filter(props, p => p.isEditable);
        const editablePropsMap = _.zipObject(_.map(editableProps, p => p.id), _.map(editableProps, p => p));

        const controls = _.mapValues(editablePropsMap, p => [p.getValueForControl(), a => p.validator(a)]) as _.Dictionary<any>;
        this.form = this.formBuilder.group(controls);

        this.form!.valueChanges.subscribe((data: any) => {
            // cache parm values
            const obj = this.object;
            if (obj) {
                _.forEach(data, (v, k) => editablePropsMap[k!].setValueFromControl(v));
                obj.setProperties();
            }
        });
    }

    // todo give #ttl a better name 
    @ViewChildren("ttl")
    titleDiv: QueryList<ElementRef>;

    focusOnTitle(e: QueryList<ElementRef>) {
        if (this.mode === InteractionMode.View) {
            if (e && e.first) {
                setTimeout(() => e.first.nativeElement.focus(), 0);
            }
        }
    }

    ngAfterViewInit(): void {
        this.focusOnTitle(this.titleDiv);
        this.titleDiv.changes.subscribe((ql: QueryList<ElementRef>) => this.focusOnTitle(ql));
    }

    selectedDialogId: string;
}
