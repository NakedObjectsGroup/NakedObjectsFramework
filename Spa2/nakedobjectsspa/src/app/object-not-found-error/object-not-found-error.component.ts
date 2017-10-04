import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-object-not-found-error',
  templateUrl: './object-not-found-error.component.html',
  styleUrls: ['./object-not-found-error.component.css']
})
export class ObjectNotFoundErrorComponent implements OnInit {
  constructor() { }

  // template API 

  title: string;
  message: string;
 
  ngOnInit(): void {

    this.title = "Destroyed Error";
    this.message = "The object you wish to view does not exist in the database. It may have been deleted by you or another user. If not, please contact your system administrator.";
  }
}
