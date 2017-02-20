import { Component, OnInit, Input, QueryList, AfterViewInit, ViewChildren } from '@angular/core';
import { LinkViewModel } from '../view-models/link-view-model';
import { MenuComponent } from '../menu/menu.component';

@Component({
    selector: 'nof-menus',
    template: require('./menus.component.html'),
    styles: [require('./menus.component.css')]
})
export class MenusComponent implements AfterViewInit {

    @Input()
    menus: LinkViewModel[];

    focusOnFirstMenu(menusList: QueryList<MenuComponent>) {
        if (menusList && menusList.first) {
            menusList.first.focus();
        }
    }

    @ViewChildren(MenuComponent)
    menuComponents: QueryList<MenuComponent>;

    ngAfterViewInit(): void {
        this.focusOnFirstMenu(this.menuComponents);
        this.menuComponents.changes.subscribe((ql: QueryList<MenuComponent>) => this.focusOnFirstMenu(ql));
    }
}
