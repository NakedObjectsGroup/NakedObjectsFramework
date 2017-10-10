import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'object-not-found-error',
  template: require('./object-not-found-error.component.html'),
  styles: [require('./object-not-found-error.component.css')]
})
export class ObjectNotFoundErrorComponent implements OnInit {
  constructor() { }

  // template API 

  title: string;
  message: string;
 
  ngOnInit(): void {

    this.title = "Object does not exist";
    this.message = "The requested object might have been deleted by you or another user. If not, please contact your system administrator.";
  }
}
