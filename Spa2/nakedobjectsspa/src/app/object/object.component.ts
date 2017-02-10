import { Component, Input, OnInit, OnDestroy, AfterViewInit, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { RepresentationsService } from '../representations.service';
import { ActivatedRoute } from '@angular/router';
import * as Models from '../models';
import { UrlManagerService } from '../url-manager.service';
import { ClickHandlerService } from '../click-handler.service';
import { ContextService } from '../context.service';
import { RepLoaderService } from '../rep-loader.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ErrorService } from '../error.service';
import { MaskService } from '../mask.service';
import { PaneRouteData, RouteData, InteractionMode, ICustomActivatedRouteData } from '../route-data';
import { FormBuilder, FormGroup, FormControl, AbstractControl } from '@angular/forms';
import * as _ from 'lodash';
import { PropertyViewModel } from '../view-models/property-view-model';
import { CollectionViewModel } from '../view-models/collection-view-model';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { PaneComponent } from '../pane/pane';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import { IButton } from '../button/button.component';
import { ColorService } from '../color.service';
import { ConfigService } from '../config.service';
import { ISubscription } from 'rxjs/Subscription';
import * as Msg from '../user-messages';
import * as Helpers from '../view-models/helpers-view-models';


@Component({
    selector: 'nof-object',
    templateUrl: './object.component.html',
    styleUrls: ['./object.component.css']
})
export class ObjectComponent implements OnInit, OnDestroy, AfterViewInit {
    
    constructor(
        private readonly activatedRoute: ActivatedRoute,
        private readonly urlManager: UrlManagerService,
        private readonly context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly colorService: ColorService,
        private readonly error: ErrorService,
        private readonly formBuilder: FormBuilder,
        private readonly configService: ConfigService
    ) {
        this.pendingColor = `${configService.config.objectColor}${this.colorService.getDefault()}`;
    }

    // template API 
    expiredTransient = false;
    object: DomainObjectViewModel | null;

    
    private mode: InteractionMode | null;
    form: FormGroup | null;

    get viewMode() {
        return this.mode == null ? "" : InteractionMode[this.mode];
    }

    // must be properties as object may change - eg be reloaded 
    get friendlyName() {
        const obj = this.object;
        return obj ? obj.friendlyName : "";
    }

    // used to smooth transition before object set 
    private pendingColor = "object-color0";

    get color() {
        const obj = this.object;      
        return obj ? obj.color : this.pendingColor;
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
            Helpers.copy(event, obj, this.context);
        }
    }

    title() {
        const obj = this.object;
        const prefix = this.mode === InteractionMode.Edit || this.mode === InteractionMode.Transient ? `${Msg.editing} - ` : "";
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
        disabled: () => this.form && !this.form.valid ? true : null,
        title: () => this.tooltip,
        accesskey: null
    }

    private saveAndCloseButton: IButton = {
        value: "Save & Close",
        doClick: () => this.onSubmit(false),
        show: () => this.unsaved(),
        disabled: () => this.form && !this.form.valid ? true : null,
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

    private actionButtons : IButton[]; 

    get buttons() {
        if (this.mode === InteractionMode.View) {
            return [this.actionButton, this.editButton, this.reloadButton];
        }

        if (this.mode === InteractionMode.Edit || this.mode === InteractionMode.Transient) {
            return [this.saveButton, this.saveAndCloseButton, this.cancelButton];
        }

        if (this.mode === InteractionMode.Form) {

            // cache becuase otherwise we will recreate this array of buttons everytime page changes ! 
            if (!this.actionButtons) {

                const menuItems = this.menuItems()!;
                const actions = _.flatten(_.map(menuItems, (mi: MenuItemViewModel) => mi.actions!));

                this.actionButtons = _.map(actions,
                    a => ({
                        value: a.title,
                        doClick: () =>
                            a.doInvoke(),
                        doRightClick: () =>
                            a.doInvoke(true),
                        show: () => true,
                        disabled: () => a.disabled() ? true : null,
                        title: () => a.description,
                        accesskey: null
                    })) as IButton[];
            }

            return this.actionButtons;
        }

        return [] as IButton[];
    }

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
            this.actionButtons = null; 
        }

        const isChanging = !this.object;

        const modeChanging = this.mode !== routeData.interactionMode;

        this.mode = routeData.interactionMode;

        const wasDirty = this.context.getIsDirty(oid);

        this.selectedDialogId = routeData.dialogId;

        if (isChanging || modeChanging || wasDirty) {

            // set pendingColor at once to smooth transition
            this.colorService.toColorNumberFromType(oid.domainType).then(c => this.pendingColor = `${this.configService.config.objectColor}${c}`);

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

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;
    private lastPaneRouteData: PaneRouteData;
    private concurrencyErrorSub : ISubscription;

    // todo now this is a child investigate reworking so object is passed in from parent 
    ngOnInit(): void {
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe((data: ICustomActivatedRouteData) => {

            const paneId = data.pane;

            if (!this.paneRouteDataSub) {
                this.paneRouteDataSub =
                    this.urlManager.getPaneRouteDataObservable(paneId)
                        .subscribe((paneRouteData: PaneRouteData) => {
                            if (!paneRouteData.isEqual(this.lastPaneRouteData)) {
                                this.lastPaneRouteData = paneRouteData;
                                this.setup(paneRouteData);
                            }
                        });
            };
        });

        this.concurrencyErrorSub = this.context.concurrencyError$.subscribe(oid => {
            if (this.object && this.object.domainObject.getOid(this.configService.config.keySeparator).isSame(oid)) {
                this.object.concurrency();
            }
        });
    }

    ngOnDestroy(): void {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
        if (this.concurrencyErrorSub) {
            this.concurrencyErrorSub.unsubscribe();
        }
    }


    ngAfterViewInit(): void {
        this.focusOnTitle(this.titleDiv);
        this.titleDiv.changes.subscribe((ql: QueryList<ElementRef>) => this.focusOnTitle(ql));
    }

    selectedDialogId: string;
}
