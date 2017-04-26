import * as Ro from './models';
import * as Msg from './user-messages';
import * as Models from './models';
import { Injectable } from '@angular/core';
import { CiceroViewModel } from './view-models/cicero-view-model';
import { PaneRouteData, CollectionViewState } from './route-data';
import { ContextService } from './context.service';
import { ConfigService, IAppConfig } from './config.service';
import { InteractionMode } from './route-data';
import { MaskService } from './mask.service';
import { getParametersAndCurrentValue } from './cicero-commands';
import { ErrorService } from './error.service';
import { Dictionary } from 'lodash';
import each from 'lodash/each';
import filter from 'lodash/filter';
import forEach from 'lodash/forEach';
import keys from 'lodash/keys';
import some from 'lodash/some';
import invert from 'lodash/invert';

@Injectable()
export class CiceroRendererService {

    constructor(protected context: ContextService,
        protected configService: ConfigService,
        protected error: ErrorService,
        protected mask: MaskService) {
        this.keySeparator = configService.config.keySeparator;
    }
    protected keySeparator: string;

    //TODO: remove renderer.
    renderHome(cvm: CiceroViewModel, routeData: PaneRouteData): void {
        if (cvm.message) {
            cvm.outputMessageThenClearIt();
        } else {
            if (routeData.menuId) {
                this.renderOpenMenu(routeData, cvm);
            } else {
                cvm.clearInput();
                cvm.setOutputSource(Msg.welcomeMessage);
            }
        }
    };
    renderObject = (cvm: CiceroViewModel, routeData: PaneRouteData) => {
        if (cvm.message) {
            cvm.outputMessageThenClearIt();
        } else {
            const oid = Ro.ObjectIdWrapper.fromObjectId(routeData.objectId, this.keySeparator);
            this.context.getObject(1, oid, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                .then((obj: Ro.DomainObjectRepresentation) => {
                    const openCollIds = openCollectionIds(routeData);
                    if (some(openCollIds)) {
                        this.renderOpenCollection(openCollIds[0], obj, cvm);
                    } else if (obj.isTransient()) {
                        this.renderTransientObject(routeData, obj, cvm);
                    } else if (routeData.interactionMode === InteractionMode.Edit ||
                        routeData.interactionMode === InteractionMode.Form) {
                        this.renderForm(routeData, obj, cvm);
                    } else {
                        this.renderObjectTitleAndDialogIfOpen(routeData, obj, cvm);
                    }
                }).catch((reject: Ro.ErrorWrapper) => {
                    //TODO: Is the first test necessary or would this be rendered OK by generic error handling?
                    if (reject.category === Ro.ErrorCategory.ClientError && reject.clientErrorCode === Ro.ClientErrorCode.ExpiredTransient) {
                        cvm.setOutputSource(Msg.errorExpiredTransient);
                    } else {
                        this.error.handleError(reject);
                    }
                });
        }
    };

    renderList = (cvm: CiceroViewModel, routeData: PaneRouteData) => {
        if (cvm.message) {
            cvm.outputMessageThenClearIt();
        } else {
            const listPromise = this.context.getListFromMenu(routeData, routeData.page, routeData.pageSize);
            listPromise.
                then((list: Ro.ListRepresentation) => {
                    this.context.getMenu(routeData.menuId).
                        then(menu => {
                            const count = list.value().length;
                            const numPages = list.pagination().numPages;
                            const description = this.getListDescription(numPages, list, count);
                            const actionMember = menu.actionMember(routeData.actionId);
                            const actionName = actionMember.extensions().friendlyName();
                            const output = `Result from ${actionName}:\n${description}`;
                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                        }).
                        catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
                }).
                catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        }
    };

    renderError = (cvm: CiceroViewModel) => {
        const err = this.context.getError().error as Ro.ErrorRepresentation;
        cvm.clearInput();
        cvm.setOutputSource(`Sorry, an application error has occurred. ${err.message()}`);
    };

    private getListDescription(numPages: number, list: Ro.ListRepresentation, count: number) {
        if (numPages > 1) {
            const page = list.pagination().page;
            const totalCount = list.pagination().totalCount;
            return `Page ${page} of ${numPages} containing ${count} of ${totalCount} items`;
        } else {
            return `${count} items`;
        }
    }


    //TODO functions become 'private'
    //Returns collection Ids for any collections on an object that are currently in List or Table mode
    private openCollectionIds(routeData: PaneRouteData): string[] {
        return filter(keys(routeData.collections), k => routeData.collections[k] != CollectionViewState.Summary);
    }

    private renderOpenCollection(collId: string, obj: Ro.DomainObjectRepresentation, cvm: CiceroViewModel) {
        const coll = obj.collectionMember(collId);
        let output = renderCollectionNameAndSize(coll);
        output += `(${Msg.collection} ${Msg.on} ${Ro.typePlusTitle(obj)})`;
        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
    }

    private renderTransientObject(routeData: PaneRouteData, obj: Ro.DomainObjectRepresentation, cvm: CiceroViewModel) {
        var output = `${Msg.unsaved} `;
        output += obj.extensions().friendlyName() + "\n";
        output += this.renderModifiedProperties(obj, routeData, this.mask);
        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
    }

    private renderForm(routeData: PaneRouteData, obj: Ro.DomainObjectRepresentation, cvm: CiceroViewModel) {
        let output = `${Msg.editing} `;
        output += Ro.typePlusTitle(obj) + "\n";
        if (routeData.dialogId) {
            this.context.getInvokableAction(obj.actionMember(routeData.dialogId)).
                then(invokableAction => {
                    output += this.renderActionDialog(invokableAction, routeData, this.mask);
                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                }).
                catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        } else {
            output += this.renderModifiedProperties(obj, routeData, this.mask);
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }
    }

    private renderObjectTitleAndDialogIfOpen(routeData: PaneRouteData, obj: Ro.DomainObjectRepresentation, cvm: CiceroViewModel) {
        let output = Ro.typePlusTitle(obj) + "\n";
        if (routeData.dialogId) {
            this.context.getInvokableAction(obj.actionMember(routeData.dialogId)).
                then(invokableAction => {
                    output += this.renderActionDialog(invokableAction, routeData, this.mask);
                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                }).
                catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        } else {
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }
    }

    private renderOpenMenu(routeData: PaneRouteData, cvm: CiceroViewModel) {
        var output = "";
        this.context.getMenu(routeData.menuId).
            then(menu => {
                output += Msg.menuTitle(menu.title());
                return routeData.dialogId ? this.context.getInvokableAction(menu.actionMember(routeData.dialogId)) : Promise.resolve(null);
            }).
            then(invokableAction => {
                if (invokableAction) {
                    output += `\n${this.renderActionDialog(invokableAction, routeData, this.mask)}`;
                }
                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
            }).
            catch((reject: Ro.ErrorWrapper) => {
                this.error.handleError(reject);
                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
            });
    }

    private renderActionDialog(invokable: Models.ActionRepresentation | Models.InvokableActionMember,
        routeData: PaneRouteData,
        mask: MaskService): string {
        const actionName = invokable.extensions().friendlyName();
        let output = `Action dialog: ${actionName}\n`;
        forEach(getParametersAndCurrentValue(invokable, this.context), (value, paramId) => {
            output += Ro.friendlyNameForParam(invokable, paramId) + ": ";
            const param = invokable.parameters()[paramId];
            output += renderFieldValue(param, value, mask);
            output += "\n";
        });
        return output;
    }

    private renderModifiedProperties(obj: Ro.DomainObjectRepresentation, routeData: PaneRouteData, mask: MaskService): string {
        let output = "";
        const props = this.context.getObjectCachedValues(obj.id());
        if (keys(props).length > 0) {
            output += Msg.modifiedProperties + ":\n";
            each(props, (value, propId) => {
                output += Ro.friendlyNameForProperty(obj, propId) + ": ";
                const pm = obj.propertyMember(propId);
                output += renderFieldValue(pm, value, mask);
                output += "\n";
            });
        }
        return output;
    }
}

//Returns collection Ids for any collections on an object that are currently in List or Table mode
export function openCollectionIds(routeData: PaneRouteData): string[] {
    return filter(keys(routeData.collections), k => routeData.collections[k] !== CollectionViewState.Summary);
}

//Handles empty values, and also enum conversion
export function renderFieldValue(field: Ro.IField, value: Ro.Value, mask: MaskService): string {
    if (!field.isScalar()) { //i.e. a reference
        return value.isNull() ? Msg.empty : value.toString();
    }
    //Rest is for scalar fields only:
    if (value.toString()) { //i.e. not empty        
        if (field.entryType() === Ro.EntryType.Choices) {
            return renderSingleChoice(field, value);
        } else if (field.entryType() === Ro.EntryType.MultipleChoices && value.isList()) {
            return renderMultipleChoicesCommaSeparated(field, value);
        }
    }
    let properScalarValue: number | string | boolean | Date;
    if (this.isDateOrDateTime(field)) {
        properScalarValue = this.toUtcDate(value);
    } else {
        properScalarValue = value.scalar();
    }
    if (properScalarValue === "" || properScalarValue == null) {
        return Msg.empty;
    } else {
        const remoteMask = field.extensions().mask();
        const format = field.extensions().format();
        return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
    }
}

function renderSingleChoice(field: Ro.IField, value: Ro.Value) {
    //This is to handle an enum: render it as text, not a number:  
    const inverted = invert(field.choices());
    return (<any>inverted)[value.toValueString()];
}

function renderMultipleChoicesCommaSeparated(field: Ro.IField, value: Ro.Value) {
    //This is to handle an enum: render it as text, not a number: 
    const inverted = invert(field.choices());
    let output = "";
    const values = value.list();
    forEach(values, v => {
        output += (<any>inverted)[v.toValueString()] + ",";
    });
    return output;
}

export function renderCollectionNameAndSize(coll: Ro.CollectionMember): string {
    let output: string = coll.extensions().friendlyName() + ": ";
    switch (coll.size()) {
        case 0:
            output += Msg.empty;
            break;
        case 1:
            output += `1 ${Msg.item}`;
            break;
        default:
            output += this.numberOfItems(coll.size());
    }
    return output + "\n";
}