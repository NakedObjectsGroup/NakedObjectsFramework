import * as Ro from './models';
import * as Msg from './user-messages';
import * as Models from './models';
import { Injectable } from '@angular/core';
import { PaneRouteData, CollectionViewState } from './route-data';
import { ContextService } from './context.service';
import { ConfigService } from './config.service';
import { InteractionMode } from './route-data';
import { MaskService } from './mask.service';
import { getParametersAndCurrentValue } from './cicero-commands/command-result';
import { ErrorService } from './error.service';
import filter from 'lodash-es/filter';
import keys from 'lodash-es/keys';
import some from 'lodash-es/some';
import reduce from 'lodash-es/reduce';
import invert from 'lodash-es/invert';
import { Result } from './cicero-commands/result';

@Injectable()
export class CiceroRendererService {

    constructor(
        private readonly context: ContextService,
        private readonly configService: ConfigService,
        private readonly error: ErrorService,
        private readonly mask: MaskService
    ) {
    }

    protected get keySeparator() {
        return this.configService.config.keySeparator;
    }

    private returnResult = (input: string, output: string): Promise<Result> => Promise.resolve(Result.create(input, output));

    // TODO: remove renderer.
    renderHome(routeData: PaneRouteData): Promise<Result> {
        if (routeData.menuId) {
            return this.renderOpenMenu(routeData);
        } else {
            return this.returnResult("", Msg.welcomeMessage);
        }
    }

    renderObject(routeData: PaneRouteData): Promise<Result> {

        const oid = Ro.ObjectIdWrapper.fromObjectId(routeData.objectId, this.keySeparator);

        return this.context.getObject(1, oid, routeData.interactionMode) // TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
            .then((obj: Ro.DomainObjectRepresentation) => {
                const openCollIds = this.openCollectionIds(routeData);
                if (some(openCollIds)) {
                    return this.renderOpenCollection(openCollIds[0], obj);
                } else if (obj.isTransient()) {
                    return this.renderTransientObject(routeData, obj);
                } else if (routeData.interactionMode === InteractionMode.Edit ||
                    routeData.interactionMode === InteractionMode.Form) {
                    return this.renderForm(routeData, obj);
                } else {
                    return this.renderObjectTitleAndDialogIfOpen(routeData, obj);
                }
            });
    }

    renderList(routeData: PaneRouteData): Promise<Result> {
        const listPromise = this.context.getListFromMenu(routeData, routeData.page, routeData.pageSize);
        return listPromise.
            then((list: Ro.ListRepresentation) =>
                this.context.getMenu(routeData.menuId).
                    then(menu => {
                        const count = list.value().length;
                        const description = this.getListDescription(list, count);
                        const actionMember = menu.actionMember(routeData.actionId);
                        const actionName = actionMember.extensions().friendlyName();
                        const output = `Result from ${actionName}:\n${description}`;

                        return this.returnResult("", output);
                    })
            );
    }

    renderError(message: string) {
        const err = this.context.getError();
        const errRep = err ? err.error : null;
        const msg = (errRep instanceof Ro.ErrorRepresentation) ? errRep.message() : "Unknown";
        return this.returnResult("", `Sorry, an application error has occurred. ${msg}`);
    }

    private getListDescription(list: Ro.ListRepresentation, count: number) {
        const pagination = list.pagination();
        if (pagination) {
            const numPages = pagination.numPages;
            if (numPages > 1) {
                const page = pagination.page;
                const totalCount = pagination.totalCount;
                return `Page ${page} of ${numPages} containing ${count} of ${totalCount} items`;
            }
        }
        return `${count} items`;
    }

    // TODO functions become 'private'
    // Returns collection Ids for any collections on an object that are currently in List or Table mode

    private renderOpenCollection(collId: string, obj: Ro.DomainObjectRepresentation): Promise<Result> {
        const coll = obj.collectionMember(collId);
        const output = `${this.renderCollectionNameAndSize(coll)}(${Msg.collection} ${Msg.on} ${Ro.typePlusTitle(obj)})`;
        return this.returnResult("", output);
    }

    private renderTransientObject(routeData: PaneRouteData, obj: Ro.DomainObjectRepresentation) {
        const output = `${Msg.unsaved} ${obj.extensions().friendlyName()}\n${this.renderModifiedProperties(obj, routeData, this.mask)}`;
        return this.returnResult("", output);
    }

    private renderForm(routeData: PaneRouteData, obj: Ro.DomainObjectRepresentation) {
        const prefix = `${Msg.editing} ${Ro.typePlusTitle(obj)}\n`;
        if (routeData.dialogId) {
            return this.context.getInvokableAction(obj.actionMember(routeData.dialogId)).
                then(invokableAction => {
                    const output = `${prefix}${this.renderActionDialog(invokableAction, routeData, this.mask)}`;
                    return this.returnResult("", output);
                });
        } else {
            const output = `${prefix}${this.renderModifiedProperties(obj, routeData, this.mask)}`;
            return this.returnResult("", output);
        }
    }

    private renderObjectTitleAndDialogIfOpen(routeData: PaneRouteData, obj: Ro.DomainObjectRepresentation) {
        const prefix = `${Ro.typePlusTitle(obj)}\n`;
        if (routeData.dialogId) {
            return this.context.getInvokableAction(obj.actionMember(routeData.dialogId)).
                then(invokableAction => {
                    const output = `${prefix}${this.renderActionDialog(invokableAction, routeData, this.mask)}`;
                    return this.returnResult("", output);
                });
        } else {
            return this.returnResult("", prefix);
        }
    }

    private renderOpenMenu(routeData: PaneRouteData): Promise<Result> {

        return this.context.getMenu(routeData.menuId).then(menu => {
            const prefix = Msg.menuTitle(menu.title());
            if (routeData.dialogId) {
                return this.context.getInvokableAction(menu.actionMember(routeData.dialogId)).then(invokableAction => {
                    const output = `${prefix}\n${this.renderActionDialog(invokableAction, routeData, this.mask)}`;
                    return this.returnResult("", output);
                });
            } else {
                return this.returnResult("", prefix);
            }
        });
    }

    private renderActionDialog(invokable: Models.ActionRepresentation | Models.InvokableActionMember,
        routeData: PaneRouteData,
        mask: MaskService): string {

        const actionName = invokable.extensions().friendlyName();
        const prefix = `Action dialog: ${actionName}\n`;
        const parms = getParametersAndCurrentValue(invokable, this.context);
        return reduce(parms, (s, value, paramId) => {
            const param = invokable.parameters()[paramId];
            return `${s}${Ro.friendlyNameForParam(invokable, paramId)}: ${this.renderFieldValue(param, value, mask)}\n`;
        }, prefix);
    }

    private renderModifiedProperties(obj: Ro.DomainObjectRepresentation, routeData: PaneRouteData, mask: MaskService): string {
        const props = this.context.getObjectCachedValues(obj.id());
        if (keys(props).length > 0) {
            const prefix = `${Msg.modifiedProperties}:\n`;

            return reduce(props, (s, value, propId) => {
                const pm = obj.propertyMember(propId);
                return `${s}${Ro.friendlyNameForProperty(obj, propId)}: ${this.renderFieldValue(pm, value, mask)}\n`;
            }, prefix);
        }
        return "";
    }

    private renderSingleChoice(field: Ro.IField, value: Ro.Value) {
        // This is to handle an enum: render it as text, not a number:
        const inverted = invert(field.choices()!);
        return (<any>inverted)[value.toValueString()];
    }

    private renderMultipleChoicesCommaSeparated(field: Ro.IField, value: Ro.Value) {
        // This is to handle an enum: render it as text, not a number:
        const inverted = invert(field.choices()!);
        const values = value.list()!;
        return reduce(values, (s, v) => `${s}${(<any>inverted)[v.toValueString()]},`, "");
    }

    // helpers

    renderCollectionNameAndSize(coll: Ro.CollectionMember): string {
        const prefix = `${coll.extensions().friendlyName()}`;
        const size = coll.size() || 0;
        switch (size) {
            case 0:
                return `${prefix}: ${Msg.empty}\n`;
            case 1:
                return `${prefix}: 1 ${Msg.item}\n`;
            default:
                return `${prefix}: ${Msg.numberOfItems(size)}\n`;
        }
    }

    openCollectionIds(routeData: PaneRouteData): string[] {
        return filter(keys(routeData.collections), k => routeData.collections[k] !== CollectionViewState.Summary);
    }

    // Handles empty values, and also enum conversion
    renderFieldValue(field: Ro.IField, value: Ro.Value, mask: MaskService): string {
        if (!field.isScalar()) { // i.e. a reference
            return value.isNull() ? Msg.empty : value.toString();
        }
        // Rest is for scalar fields only:
        if (value.toString()) { // i.e. not empty
            if (field.entryType() === Ro.EntryType.Choices) {
                return this.renderSingleChoice(field, value);
            } else if (field.entryType() === Ro.EntryType.MultipleChoices && value.isList()) {
                return this.renderMultipleChoicesCommaSeparated(field, value);
            }
        }
        let properScalarValue: number | string | boolean | Date | null;
        if (Ro.isDateOrDateTime(field)) {
            properScalarValue = Ro.toUtcDate(value);
        } else {
            properScalarValue = value.scalar();
        }
        if (properScalarValue === "" || properScalarValue == null) {
            return Msg.empty;
        } else {
            const remoteMask = field.extensions().mask();
            const format = field.extensions().format()!;
            return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
        }
    }
}
