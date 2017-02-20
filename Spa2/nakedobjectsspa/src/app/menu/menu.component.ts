import { Component, OnInit, Input, ElementRef } from '@angular/core';
import { LinkViewModel } from '../view-models/link-view-model';
import { UrlManagerService } from '../url-manager.service';

@Component({
    selector: 'nof-menu',
    template: require('./menu.component.html'),
    styles: [require('./menu.component.css')]
})
export class MenuComponent implements OnInit {

    constructor(
        private readonly urlManager: UrlManagerService,
        private readonly elementRef: ElementRef
    ) { }

    @Input()
    menu: LinkViewModel;

    ngOnInit() {
    }

    doClick() {
        const menuId = this.menu.link.rel().parms[0].value;
        this.urlManager.setMenu(menuId!, this.menu.paneId);
    }

    get title() {
        return this.menu.title;
    }

    focus() {
        this.elementRef.nativeElement.children[0].focus();
    }
}
