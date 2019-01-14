import { ElementRef } from '@angular/core';
import { SubscriptionLike as ISubscription } from 'rxjs';

export function safeUnsubscribe(sub: ISubscription) {
    if (sub) {
        sub.unsubscribe();
    }
}

function safeFocus(nativeElement: any) {
    if (nativeElement && nativeElement.focus) {
        nativeElement.focus();
    }
}

export function focus(element: ElementRef) {
    setTimeout(() => safeFocus(element.nativeElement));
    return true;
}
