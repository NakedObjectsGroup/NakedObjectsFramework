import { Component, Input, OnInit, OnDestroy, AfterViewInit, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { RepresentationsService } from "../representations.service";
import { ActivatedRoute } from '@angular/router';
import * as Models from "../models";
import { UrlManagerService } from "../url-manager.service";
import { ClickHandlerService } from "../click-handler.service";
import { ContextService } from "../context.service";
import { RepLoaderService } from "../rep-loader.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ColorService } from "../color.service";
import { ErrorService } from "../error.service";
import { MaskService } from "../mask.service";
import { PaneRouteData, RouteData, InteractionMode } from "../route-data";
import { ISubscription } from 'rxjs/Subscription';
import { Subject } from 'rxjs/Subject';
import { FormBuilder, FormGroup, FormControl, AbstractControl } from '@angular/forms';
import * as _ from "lodash";
import { PropertyViewModel } from '../view-models/property-view-model';
import { CollectionViewModel } from '../view-models/collection-view-model';
import { MenuItemViewModel } from '../view-models/menu-item-view-model';
import { PaneComponent } from '../pane/pane';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';

@Component({
    selector: 'object',
    templateUrl: './object.component.html',
    styleUrls: ['./object.component.css']
})
export class ObjectComponent extends PaneComponent implements OnInit, OnDestroy, AfterViewInit {

    constructor(activatedRoute: ActivatedRoute,
                urlManager: UrlManagerService,
                private context: ContextService,
                private colorService: ColorService,
                private viewModelFactory: ViewModelFactoryService,
                private error: ErrorService,
                private formBuilder: FormBuilder) {

        super(activatedRoute, urlManager);
    }

    // template API 
    expiredTransient = false;
    object: DomainObjectViewModel;  
    
    // todo add mode as string property for template to make easier to read and make this private 
    mode: InteractionMode;
    form: FormGroup;

    // must be properties as object may change - eg be reloaded 
    get friendlyName() {
        return this.object.friendlyName;
    }

    get color() {
        return this.object.color;
    }

    get properties() {
        return this.object.properties;
    }

    get collections() {
        return this.object.collections;
    }

    get tooltip(): string {
        return this.object.tooltip();
    }

    onSubmit(viewObject: boolean) {
        this.object.doSave(viewObject);
    }

    // todo DRY this - and rename - copy not cut
    cut(event: any) {
        const cKeyCode = 67;
        if (event && (event.keyCode === cKeyCode && event.ctrlKey)) {
            this.context.setCutViewModel(this.object);
            event.preventDefault();
        }
    }

    title() {
        // todo add string consts to user messages !
        const prefix = this.mode === InteractionMode.Edit || this.mode === InteractionMode.Transient ? "Editing - " : "";
        return `${prefix}${this.object.title}`;
    }

    // todo investigate if logic in this would be better here rather than view model
    toggleActionMenu = () => this.object.toggleActionMenu();

    disableActions = () => this.object.disableActions() ? true : null;

    actionsTooltip = () => this.object.actionsTooltip();
    unsaved = () => this.object.unsaved;
   
    doEdit = () => this.object.doEdit();
    doEditCancel = () => this.object.doEditCancel();
    showEdit = () => !this.object.hideEdit();
    doReload = () => this.object.doReload(); 
    message = () => this.object.getMessage();
    showActions = () => this.object.showActions();

    menuItems = () => this.object.menuItems;

    // todo that we access viewmodel directly in template from this I think is smell that we should have a 
    // child component here 
    actions = (item : MenuItemViewModel) => item.actions;

    protected setup(routeData: PaneRouteData) {
        // subscription means may get with no oid 

        if (!routeData.objectId) {
            this.mode = null;
            return;
        }

        this.expiredTransient = false;

        const oid = Models.ObjectIdWrapper.fromObjectId(routeData.objectId);

        // todo this is a recurring pattern in angular 2 code - generalise 
        // across components 
        if (this.object && !this.object.domainObject.getOid().isSame(oid)) {
            // object has changed - clear existing 
            this.object = null;
        }

        this.mode = routeData.interactionMode;

        this.context.clearObjectUpdater(routeData.paneId);

        const wasDirty = this.context.getIsDirty(oid);

        this.context.getObject(routeData.paneId, oid, routeData.interactionMode)
            .then((object: Models.DomainObjectRepresentation) => {

                const ovm = new DomainObjectViewModel(this.colorService, this.context, this.viewModelFactory, this.urlManager, this.error);
                ovm.reset(object, routeData);
                if (wasDirty) {
                    ovm.clearCachedFiles();
                }

                if (this.mode === InteractionMode.Edit ||
                    this.mode === InteractionMode.Form ||
                    this.mode === InteractionMode.Transient) {
                    this.createForm(ovm);
                }

                this.object = ovm;
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
    
    private createForm(vm: DomainObjectViewModel) {
        const pps = vm.properties;
        const props = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p)) as _.Dictionary<PropertyViewModel>;
        const editableProps = _.filter(props, p => p.isEditable);
        const editablePropsMap = _.zipObject(_.map(editableProps, p => p.id), _.map(editableProps, p => p));

        const controls = _.mapValues(editablePropsMap, p => [p.getValueForControl(), a => p.validator(a)]) as _.Dictionary<any>;
        this.form = this.formBuilder.group(controls);

        this.form.valueChanges.subscribe((data: any) => {
            // cache parm values
            _.forEach(data,
                (v, k) => {
                    const prop = editablePropsMap[k];
                    prop.setValueFromControl(v);
                });
            this.object.setProperties();
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
}