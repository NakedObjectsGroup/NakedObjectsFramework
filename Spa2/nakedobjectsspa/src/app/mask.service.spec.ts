import { BaseRequestOptions, Http, XHRBackend } from '@angular/http';
import { TestBed, inject } from '@angular/core/testing';
import { MaskService } from './mask.service';
import { ConfigService } from './config.service';
import { MockBackend } from '@angular/http/testing';
import * as Ro from './ro-interfaces';

describe('MaskService', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                MockBackend,
                BaseRequestOptions,
                {
                    provide: Http,
                    deps: [MockBackend, BaseRequestOptions],
                    useFactory: (backend: XHRBackend, defaultOptions: BaseRequestOptions) => new Http(backend, defaultOptions)
                },
                ConfigService,
                MaskService]
        });
    });

    function testMask(maskService: MaskService, input: any, mask: string, format: Ro.formatType, expectedResult: string) {
        const result = maskService.toLocalFilter(mask, format).filter(input);
        expect(result).toBe(expectedResult);
    }

    function testDefaultMask(maskService: MaskService, input: any, format: Ro.formatType, expectedResult: string) {
        testMask(maskService, input, "", format, expectedResult);
    }

    describe("default string", () => {
        it("masks empty", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "", "string", "")));
        it("masks null", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, null, "string", "")));
        it("masks a string", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "a string", "string", "a string")));
    });

    // todo investigate and fix failures
     describe("default int", () => {
         //it("masks empty", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "", "int", "0")));
         //it("masks null", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, null, "int", null)));
         it("masks 0", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, 0, "int", "0")));
         it("masks 101", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, 101, "int", "101")));
         it("masks 1002", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, 1002, "int", "1,002")));
         it("masks 10003", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, 10003, "int", "10,003")));
         //it("masks max int", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, Number.MAX_VALUE, "int", "1.798e+308")));
     });


     const arbitaryDate1 = new Date(Date.UTC(1985, 5, 4, 16, 27, 10));
     const arbitaryDate2 = new Date(Date.UTC(2003, 1, 20, 1, 13, 55));
     const arbitaryDate3 = new Date(Date.UTC(2016, 7, 5, 23, 45, 8));

     describe("default date-time", () => {

         let ts1: string;
         let ts2: string;
         let ts3: string;

         beforeEach(inject([MaskService], (maskService: MaskService) => {
             maskService.setDateMaskMapping("test", "date-time", "D MMM YYYY HH:mm:ss");

             const f = maskService.toLocalFilter("test", "date-time");

             ts1 = f.filter(arbitaryDate1); // "4 Jun 1985 05:27:10"
             ts2 = f.filter(arbitaryDate2); // "20 Feb 2003 01:13:55"
             ts3 = f.filter(arbitaryDate3); // "6 Aug 2016 12:45:08"
         }));

         it("masks empty", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "", "date-time", "")));
         //it("masks null", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, null, "date-time", null)));

         // these tests are locale specific UTC -> GMT/BST
         it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate1, "date-time", ts1)));
         it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate2, "date-time", ts2)));
         it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate3, "date-time", ts3)));
     });


     describe("default time", () => {
         it("masks empty", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "", "time", "")));
         //it("masks null", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, null, "time", null)));

         // these tests are UTC => UTC
         //it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate1, "time", "16:27")));
         it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate2, "time", "01:13")));
         //it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate3, "time", "23:45")));
     });

     describe("custom date-time", () => {

         beforeEach(inject([MaskService], (maskService: MaskService) => {
             maskService.setDateMaskMapping("customdt", "date-time", "M DD YYYY hh-mm-ss", "+1000");
         }));


         it("masks empty", inject([MaskService], (maskService: MaskService) => testMask(maskService, "", "customdt", "date-time", "")));
         //it("masks null", inject([MaskService], (maskService: MaskService) => testMask(maskService, null, "customdt", "date-time", null)));

         // these tests are locale specific UTC -> +1000
         //it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate1, "customdt", "date-time", "6 05 1985 02-27-10")));
         //it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate2, "customdt", "date-time", "2 20 2003 11-13-55")));
         //it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate3, "customdt", "date-time", "8 06 2016 09-45-08")));
     });

     describe("custom date", () => {

         beforeEach(inject([MaskService], (maskService: MaskService) => {
             maskService.setDateMaskMapping("customd", "date", "M DD YYYY", "+1100");
         }));

         it("masks empty", inject([MaskService], (maskService: MaskService) => testMask(maskService, "", "customd", "date", "")));
         //it("masks null", inject([MaskService], (maskService: MaskService) => testMask(maskService, null, "customd", "date", null)));

         // these tests are locale specific UTC -> +1000
         //it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate1, "customd", "date", "6 05 1985")));
         it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate2, "customd", "date", "2 20 2003")));
         it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate3, "customd", "date", "8 06 2016")));
     });

     describe("custom time", () => {

         beforeEach(inject([MaskService], (maskService: MaskService) => {
             maskService.setDateMaskMapping("customt", "time", "hh-mm-ss", "+0030");
         }));

         it("masks empty", inject([MaskService], (maskService: MaskService) => testMask(maskService, "", "customt", "time", "")));
         //it("masks null", inject([MaskService], (maskService: MaskService) => testMask(maskService, null, "customt", "time", null)));

         // these tests are UTC => UTC
         //it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate1, "customt", "time", "04-57-10")));
         //it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate2, "customt", "time", "01-43-55")));
         //it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate3, "customt", "time", "12-15-08")));
     });

    // it('should ...', inject([MaskService], (maskService: MaskService) => {
    //     expect(maskService).toBeTruthy();
    // }));
});
