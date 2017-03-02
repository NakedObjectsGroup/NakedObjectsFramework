import { Component, OnInit, Input, QueryList, AfterViewInit, ViewChildren } from '@angular/core';
import { LinkViewModel } from '../view-models/link-view-model';
import * as Actioncomponent from '../action/action.component';
import * as Urlmanagerservice from '../url-manager.service';

@Component({
    selector: 'nof-menu-bar',
    template: require('./menu-bar.component.html'),
    styles: [require('./menu-bar.component.css')]
})
export class MenuBarComponent implements AfterViewInit {

    constructor(private readonly urlManager : Urlmanagerservice.UrlManagerService) {  }

    @Input()
    set menus(m: LinkViewModel[]) {

        this.actions = _.map(m,
            a => ({
                value: a.title,
                doClick: () => {
                    const menuId = a.link.rel().parms[0].value;
                    this.urlManager.setMenu(menuId!, a.paneId);
                },
                doRightClick: () => {},
                show: () => true,
                disabled: () =>  null,
                tempDisabled: () => false,
                title: () => a.title,
                accesskey: null
            })) as Actioncomponent.IActionHolder[];

    }

    actions: Actioncomponent.IActionHolder[];

    focusOnFirstMenu(menusList: QueryList<Actioncomponent.ActionComponent>) {
        if (menusList && menusList.first) {
            menusList.first.focus();
        }
    }

    @ViewChildren(Actioncomponent.ActionComponent)
    actionComponents: QueryList<Actioncomponent.ActionComponent>;

    ngAfterViewInit(): void {
        this.focusOnFirstMenu(this.actionComponents);
        this.actionComponents.changes.subscribe((ql: QueryList<Actioncomponent.ActionComponent>) => this.focusOnFirstMenu(ql));
    }
}
