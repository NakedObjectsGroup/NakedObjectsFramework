import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import {ConfigService} from '../config.service';

@Component({
  selector: 'nof-login',
  template: require('./login.component.html'),
  styles: [require('./login.component.css')]
})
export class LoginComponent {

  constructor(
    public readonly auth: AuthService, 
    public readonly configService : ConfigService
  ) { }

}
