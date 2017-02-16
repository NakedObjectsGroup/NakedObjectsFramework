import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'nof-cicero',
  templateUrl: './cicero.component.html',
  styleUrls: ['./cicero.component.css']
})
export class CiceroComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

private inputCommand : string;

  set inp(newValue: string) {
    this.inputCommand = newValue;
    this.outputText = this.inputCommand + "done";
  }
  get inp() :string {
    return this.inputCommand;
  }

private outputText: string;
  set output(newValue: string) {
    this.outputText = newValue;
  }
  get output() : string {
    return this.outputText;
  }

}
