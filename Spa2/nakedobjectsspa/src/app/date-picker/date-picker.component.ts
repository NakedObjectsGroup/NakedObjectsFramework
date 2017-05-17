import { Component, OnInit } from '@angular/core';


@Component({
  selector: 'nof-date-picker',
  templateUrl: require('./date-picker.component.html'),
  styleUrls: [require('./date-picker.component.css')]
})
export class DatePickerComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }


  datepickerConfig = { format: "D MMM YYYY" }
}
