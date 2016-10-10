import { Component, OnInit, Input, OnDestroy, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RepresentationsService } from '../representations.service'
import { getAppPath } from "../config";
import { Observable } from 'rxjs/Observable';
import { ISubscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router, Data } from '@angular/router';
import { UrlManagerService } from "../url-manager.service";
import { ContextService } from "../context.service";
import { ErrorService } from '../error.service';
import { FocusManagerService } from "../focus-manager.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ColorService } from "../color.service";
import { RouteData, PaneRouteData } from "../route-data";
import * as Models from "../models";
import * as ViewModels from "../view-models";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {

  constructor() { }

  ngOnInit() {


  }

  ngOnDestroy() {



  }

}
