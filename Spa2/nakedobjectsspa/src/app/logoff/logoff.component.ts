import { Pane } from '../route-data';
import { ContextService } from '../context.service';
import { Component, OnInit } from '@angular/core';
import { ConfigService } from '../config.service';
import { AuthService } from '../auth.service';
import { Http, RequestOptionsArgs } from '@angular/http';
import { UrlManagerService } from '../url-manager.service';
import { Location } from '@angular/common';

@Component({
  selector: 'nof-logoff',
  templateUrl: require('./logoff.component.html'),
  styleUrls: [require('./logoff.component.css')]
})
export class LogoffComponent implements OnInit {

  constructor(
    private readonly context: ContextService,
    private readonly authService: AuthService,
    private readonly configService: ConfigService,
    private readonly http: Http,
    private readonly urlManager: UrlManagerService,
    private readonly location: Location,
  ) { }

  userId: string;

  isActive = true;;

  userIsLoggedIn() {
    return this.authService.userIsLoggedIn()
  } 

  cancel() {
    this.isActive = false;
    this.location.back();
  }

  logoff() {
    this.isActive = false;
    const serverLogoffUrl = this.configService.config.logoffUrl;

    if (serverLogoffUrl) {

      const args: RequestOptionsArgs = {
        withCredentials: true
      };

      this.http.post(this.configService.config.logoffUrl, args);
    }

    this.authService.logout();

    // logoff client without waiting for server
    // todo do we need to do this to clear data ? 
    //window.location.href = this.configService.config.postLogoffUrl;

  }

  ngOnInit() {
    this.context.getUser().then(u => this.userId = u.userName() || "Unknown");
  }
}
