import { Component, OnInit, Input } from '@angular/core';

export interface IButton {
    doClick: () => void;
    show: () => boolean;
    disable: () => boolean;
    value: string;
}

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.css']
})
export class ButtonComponent implements OnInit {

  constructor() { }

  @Input()
  button : IButton;

  doClick() {
      this.button.doClick();
  }

  show() {
      this.button.show();
  }

  disable() {
      this.button.disable(); 
  }

  get value() {
      return this.button.value;
  }

  ngOnInit() {
  }

}
