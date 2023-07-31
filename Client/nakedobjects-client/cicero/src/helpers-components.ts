import { ElementRef } from '@angular/core';
import { SubscriptionLike as ISubscription } from 'rxjs';

export function safeUnsubscribe(sub?: ISubscription) {
    if (sub) {
        sub.unsubscribe();
    }
}

function isFocusable(nativeElement: unknown): nativeElement is { focus: () => void } {
    return !!(nativeElement && nativeElement instanceof Object && 'focus' in nativeElement);
}

function safeFocus(nativeElement?: unknown) {
    if (isFocusable(nativeElement)) {
        nativeElement.focus();
    }
}

export function focus(element?: ElementRef) {
    setTimeout(() => safeFocus(element?.nativeElement));
    return true;
}

export function hasMessage(obj: unknown): obj is { message: string } {
    return typeof obj === 'object' && obj !== null && 'message' in obj && typeof obj.message == 'string';
}

export function messageFrom(e: unknown) {
    return hasMessage(e) ? e.message : 'unknown error';
}