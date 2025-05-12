import { Component, OnInit } from '@angular/core';

@Component({
    // eslint-disable-next-line @angular-eslint/component-selector
    selector: 'object-not-found-error',
    templateUrl: 'object-not-found-error.component.html',
    styleUrls: ['object-not-found-error.component.css'],
    standalone: false
})
export class ObjectNotFoundErrorComponent implements OnInit {
    
    // template API

    title = '';
    message = '';

    ngOnInit(): void {
        this.title = 'Object does not exist';
        this.message = 'The requested object might have been deleted by you or another user. If not, please contact your system administrator.';
    }
}
