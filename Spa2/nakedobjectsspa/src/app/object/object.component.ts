import { Component, OnInit, OnDestroy, ViewChildren, QueryList, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as Models from '../models';
import { UrlManagerService } from '../url-manager.service';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ErrorService } from '../error.service';
import { PaneRouteData, InteractionMode, ICustomActivatedRouteData } from '../route-data';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PropertyViewModel } from '../view-models/property-view-model';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import { IActionHolder, wrapAction } from '../action/action.component';
import { ColorService } from '../color.service';
import { ConfigService } from '../config.service';
import { PropertiesComponent } from '../properties/properties.component';
import * as Msg from '../user-messages';
import * as Helpers from '../view-models/helpers-view-models';
import { CollectionViewModel } from '../view-models/collection-view-model';
import { Dictionary } from 'lodash';
import filter from 'lodash-es/filter';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import flatten from 'lodash-es/flatten';
import zipObject from 'lodash-es/zipObject';
import mapValues from 'lodash-es/mapValues';
import some from 'lodash-es/some';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe } from '../helpers-components';
import { debounceTime } from 'rxjs/operators';

@Component({
    selector: 'nof-object',
    templateUrl: 'object.component.html',
    styleUrls: ['object.component.css']
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

    private actionButton: IActionHolder = {
        value: "Actions",
        doClick: () => this.toggleActionMenu(),
        show: () => true,
        disabled: () => this.disableActions(),
        tempDisabled: () => null,
        title: () => this.actionsTooltip(),
        accesskey: "a"
    };

    private editButton: IActionHolder = {
        value: "Edit",
        doClick: () => this.doEdit(),
        show: () => this.showEdit(),
        disabled: () => null,
        tempDisabled: () => null,
        title: () => "",
        accesskey: null
    };

    private reloadButton: IActionHolder = {
        value: "Reload",
        doClick: () => this.doReload(),
        show: () => true,
        disabled: () => null,
        tempDisabled: () => null,
        title: () => "",
        accesskey: null
    };

    private saveButton: IActionHolder = {
        value: "Save",
        doClick: () => this.onSubmit(true),
        show: () => true,
        disabled: () => this.form && !this.form.valid ? true : null,
        tempDisabled: () => null,
        title: () => this.tooltip,
        accesskey: null
    };

    private saveAndCloseButton: IActionHolder = {
        value: "Save & Close",
        doClick: () => this.onSubmit(false),
        show: () => this.unsaved(),
        disabled: () => this.form && !this.form.valid ? true : null,
        tempDisabled: () => null,
        title: () => this.tooltip,
        accesskey: null
    };

    private cancelButton: IActionHolder = {
        value: "Cancel",
        doClick: () => this.doEditCancel(),
        show: () => true,
        disabled: () => null,
        tempDisabled: () => null,
        title: () => "",
        accesskey: null
    };

    private actionButtons: IActionHolder[] | null;
    private viewButtons =  [this.actionButton, this.editButton, this.reloadButton];
    private saveButtons = [this.saveButton, this.saveAndCloseButton, this.cancelButton];

    private lastPaneRouteData: PaneRouteData;

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;
    private concurrencyErrorSub: ISubscription;
    private formSub: ISubscription;
    private focusSub: ISubscription;

    selectedDialogId: string;

    @ViewChildren(PropertiesComponent)
    propComponents: QueryList<PropertiesComponent>;

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
    private pendingColor: string;

    get color() {
        const obj = this.object;
        return obj ? obj.color : this.pendingColor;
    }

    get properties() {
        const obj = this.object;
        return obj ? obj.properties : "";
    }

    get collections(): CollectionViewModel[] {
        const obj = this.object;
        return obj ? obj.collections : [];
    }

    get tooltip(): string {
        const obj = this.object;
        return obj ? obj.tooltip() : "";
    }

    onSubmit(viewObject: boolean) {
        const obj = this.object;
        if (obj) {
            // if save OK we will want to null object and form as returned object may differ
            // and redrawing in current form can fail. If save not OK don't null as
            // will redraw and display errors.
            const onSuccess = () => this.clearCurrentObject();
            obj.doSave(viewObject, onSuccess);
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

    disableActions = () => {
        const obj = this.object;
        return obj && obj.noActions() ? true : null;
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

    get actionHolders() {
        if (this.mode === InteractionMode.View) {
            return this.viewButtons;
        }

        if (this.mode === InteractionMode.Edit || this.mode === InteractionMode.Transient) {
            return this.saveButtons;
        }

        if (this.mode === InteractionMode.Form) {

            // cache because otherwise we will recreate this array of actionHolders everytime page changes !
            if (!this.actionButtons) {

                const menuItems = this.menuItems() !;
                const actions = flatten(map(menuItems, (mi: MenuItemViewModel) => mi.actions!));
                this.actionButtons = map(actions, a => wrapAction(a));
            }

            return this.actionButtons;
        }

        return [] as IActionHolder[];
    }

    private clearCurrentObject() {
        this.object = null;
        this.form = null;
        this.actionButtons = null;
    }

    protected setup(routeData: PaneRouteData) {
        // subscription means may get with no oid

        if (!routeData.objectId) {
            this.mode = null;
            return;
        }

        this.expiredTransient = false;

        const oid = Models.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator);

        if (this.object && !this.object.domainObject.getOid().isSame(oid)) {
            // object has changed - clear existing
            this.clearCurrentObject();
        }

        const isChanging = !this.object;

        const modeChanging = this.mode !== routeData.interactionMode;

        this.mode = routeData.interactionMode;

        const wasDirty = this.isDirty(routeData, oid);

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

                    if (modeChanging || isChanging || wasDirty) {
                        if (this.mode === InteractionMode.Edit ||
                            this.mode === InteractionMode.Form ||
                            this.mode === InteractionMode.Transient) {
                            this.createForm(this.object!); // will never be null
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
        safeUnsubscribe(this.formSub);

        const pps = vm.properties;
        const props = zipObject(map(pps, p => p.id), map(pps, p => p)) as Dictionary<PropertyViewModel>;
        const editableProps = filter(props, p => p.isEditable);
        const editablePropsMap = zipObject(map(editableProps, p => p.id), map(editableProps, p => p)) as Dictionary<PropertyViewModel>;

        const controls = mapValues(editablePropsMap, p => [p.getValueForControl(), (a: any) => p.validator(a)]) as Dictionary<any>;
        this.form = this.formBuilder.group(controls);
        this.formSub = this.form!.valueChanges.subscribe((data: any) => {
            // cache parm values
            const obj = this.object;
            if (obj) {
                forEach(data, (v, k) => editablePropsMap[k!].setValueFromControl(v));
                obj.setProperties();
            }
        });
    }

    isDirty(paneRouteData: PaneRouteData, oid?: Models.ObjectIdWrapper) {
        oid = oid || Models.ObjectIdWrapper.fromObjectId(paneRouteData.objectId, this.configService.config.keySeparator);
        return this.context.getIsDirty(oid);
    }

    ngOnInit(): void {
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe((data: ICustomActivatedRouteData) => {

            const paneId = data.pane;

            if (!this.paneRouteDataSub) {
                const paneRouteData = this.urlManager.getPaneRouteDataObservable(paneId);
                this.paneRouteDataSub =
                    paneRouteData.pipe(debounceTime(10))
                    .subscribe((prd: PaneRouteData) => {
                            if (!prd.isEqual(this.lastPaneRouteData) || this.isDirty(prd)) {
                                this.lastPaneRouteData = prd;
                                this.setup(prd);
                            }
                        });
            }
        });

        this.concurrencyErrorSub = this.context.concurrencyError$.subscribe(oid => {
            if (this.object && this.object.domainObject.getOid().isSame(oid)) {
                this.object.concurrency();
            }
        });
    }

    focus(parms: QueryList<PropertiesComponent>) {
        if (this.mode == null || this.mode === InteractionMode.View) {
            return;
        }
        if (parms && parms.length > 0) {
            some(parms.toArray(), p => p.focus());
        }
    }

    ngAfterViewInit(): void {
        this.focusSub = this.propComponents.changes.subscribe(ql => this.focus(ql));
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.activatedRouteDataSub);
        safeUnsubscribe(this.paneRouteDataSub);
        safeUnsubscribe(this.concurrencyErrorSub);
        safeUnsubscribe(this.formSub);
        safeUnsubscribe(this.focusSub);
    }
}
