import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiFocusOn]' })
export class GeminiFocusOnDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }


    //$timeout(() => {

    //    let focusElements: JQuery;

    //    switch (target) {
    //        case FocusTarget.Menu:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.home div.menu, div.single div.home div.menu`);
    //            break;
    //        case FocusTarget.SubAction:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.actions div.action, div.single div.actions div.action`);
    //            break;
    //        case FocusTarget.Action:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.action, div.single div.action`);
    //            break;
    //        case FocusTarget.ObjectTitle:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.object div.title, div.single div.object div.title`);
    //            break;
    //        case FocusTarget.Dialog:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.parameters div.parameter :input[type!='hidden'], div.single div.parameters div.parameter :input[type!='hidden']`);
    //            break;
    //        case FocusTarget.ListItem:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.collection td.reference, div.single div.collection td.reference`);
    //            break;
    //        case FocusTarget.Property:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.properties div.property :input[type!='hidden'], div.single div.properties div.property :input[type!='hidden']`);
    //            break;
    //        case FocusTarget.TableItem:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.collection tbody tr, div.single div.collection tbody tr`);
    //            break;
    //        case FocusTarget.Input:
    //            focusElements = $(elem).find("input");
    //            break;
    //        case FocusTarget.CheckBox:
    //            focusElements = $(elem).find(`#pane${paneId}.split div.collection td.checkbox input, div.single div.collection td.checkbox input`);
    //            break;

    //    }

    //    if (focusElements) {
    //        if (focusElements.length >= index) {
    //            $(focusElements[index]).focus();
    //        } else {
    //            // haven't found anything to focus - try again - but not forever
    //            if (count < 10) {
    //                focusManager.focusOn(target, index, paneId, ++count);
    //            }
    //        }
    //    }
    //}, 0, false);
    //    });

}