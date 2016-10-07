/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ClickHandlerService } from './click-handler.service';

describe('Service: ClickHandler', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ClickHandlerService]
    });
  });

  it('should ...', inject([ClickHandlerService], (service: ClickHandlerService) => {
    expect(service).toBeTruthy();
  }));
});
