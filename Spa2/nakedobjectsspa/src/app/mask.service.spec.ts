import { TestBed, inject } from '@angular/core/testing';
import { MaskService } from './mask.service';
import { ConfigService } from './config.service';
import * as Ro from './ro-interfaces';
import * as momentNs from 'moment';
import * as Constants from './constants';
import { HttpClientModule, HttpClient, HttpRequest, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';

const moment = momentNs;

describe('MaskService', () => {

    beforeEach(() => TestBed.configureTestingModule({
        imports: [HttpClientTestingModule],
        providers: [ConfigService, MaskService ]
      }));

      beforeEach(() => {
        // 0. set up the test environment
        TestBed.configureTestingModule({
          imports: [
            // no more boilerplate code w/ custom providers needed :-)
            HttpClientModule,
            HttpClientTestingModule
          ],
          providers: [
            ConfigService,
            MaskService
          ]
        });
      });

    // beforeEach(() => {
    //     TestBed.configureTestingModule({
    //         providers: [
    //             MockBackend,
    //             BaseRequestOptions,
    //             {
    //                 provide: HttpClient,
    //                 deps: [MockBackend, BaseRequestOptions],
    //                 useFactory: (backend: XHRBackend, defaultOptions: BaseRequestOptions) => new Http(backend, defaultOptions)
    //             },
    //             ConfigService,
    //             MaskService]
    //     });
    // });

    function testMask(maskService: MaskService, input: any, mask: string, format: Ro.FormatType, expectedResult: string) {
        const result = maskService.toLocalFilter(mask, format).filter(input);
        expect(result).toBe(expectedResult);
    }

    function testDefaultMask(maskService: MaskService, input: any, format: Ro.FormatType, expectedResult: string) {
        testMask(maskService, input, "", format, expectedResult);
    }

    describe("default string", () => {
        it("masks empty", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "", "string", "")));
        it("masks null", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, null, "string", "")));
        it("masks a string", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "a string", "string", "a string")));
    });

    describe("default int", () => {
        it("masks empty", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "", "int", "")));
        it("masks null", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, null, "int", "")));
        it("masks 0", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, 0, "int", "0")));
        it("masks 101", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, 101, "int", "101")));
        it("masks 1002", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, 1002, "int", "1,002")));
        it("masks 10003", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, 10003, "int", "10,003")));
        // TODO fix
        // it("masks max int", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, Number.MAX_VALUE, "int",
        // tslint:disable-next-line:max-line-length
        //    "179,769,313,486,232,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000")));
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
        it("masks null", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, null, "date-time", "")));

        // these tests are locale specific UTC -> GMT/BST
        it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate1, "date-time", ts1)));
        it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate2, "date-time", ts2)));
        it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate3, "date-time", ts3)));
    });

    describe("default time", () => {
        it("masks empty", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, "", "time", "")));
        it("masks null", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, null, "time", "")));

        // these tests are UTC => UTC
        it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate1, "time", "16:27")));
        it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate2, "time", "01:13")));
        it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testDefaultMask(maskService, arbitaryDate3, "time", "23:45")));
    });

    describe("custom date-time", () => {

        beforeEach(inject([MaskService], (maskService: MaskService) => {
            maskService.setDateMaskMapping("customdt", "date-time", "M DD YYYY hh-mm-ss", "+1000");
        }));

        it("masks empty", inject([MaskService], (maskService: MaskService) => testMask(maskService, "", "customdt", "date-time", "")));
        it("masks null", inject([MaskService], (maskService: MaskService) => testMask(maskService, null, "customdt", "date-time", "")));

        // these tests are locale specific UTC -> +1000
        it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate1, "customdt", "date-time", "6 05 1985 02-27-10")));
        it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate2, "customdt", "date-time", "2 20 2003 11-13-55")));
        it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate3, "customdt", "date-time", "8 06 2016 09-45-08")));
    });

    describe("custom date", () => {

        beforeEach(inject([MaskService], (maskService: MaskService) => {
            maskService.setDateMaskMapping("customd", "date", "M DD YYYY", "+1100");
        }));

        it("masks empty", inject([MaskService], (maskService: MaskService) => testMask(maskService, "", "customd", "date", "")));
        it("masks null", inject([MaskService], (maskService: MaskService) => testMask(maskService, null, "customd", "date", "")));

        // these tests are locale specific UTC -> +1000
        it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate1, "customd", "date", "6 05 1985")));
        it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate2, "customd", "date", "2 20 2003")));
        it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate3, "customd", "date", "8 06 2016")));
    });

    describe("custom time", () => {

        beforeEach(inject([MaskService], (maskService: MaskService) => {
            maskService.setDateMaskMapping("customt", "time", "hh-mm-ss", "+0030");
        }));

        it("masks empty", inject([MaskService], (maskService: MaskService) => testMask(maskService, "", "customt", "time", "")));
        it("masks null", inject([MaskService], (maskService: MaskService) => testMask(maskService, null, "customt", "time", "")));

        // these tests are UTC => UTC
        it("masks arbitaryDate1", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate1, "customt", "time", "04-57-10")));
        it("masks arbitaryDate2", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate2, "customt", "time", "01-43-55")));
        it("masks arbitaryDate3", inject([MaskService], (maskService: MaskService) => testMask(maskService, arbitaryDate3, "customt", "time", "12-15-08")));
    });

    describe("Moment", () => {

        function testFormat(toTest: string, valid: boolean, expected: Date) {

            const m = moment(toTest, Constants.supportedDateFormats, "en-GB", true);

            expect(m.isValid()).toBe(valid);

            if (valid) {
                expect(m.toDate().toISOString()).toBe(expected.toISOString());
            }
        }

        describe("test formats", () => {
            it("formats", () => testFormat("1/1/2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("7/8/2016", true, new Date(2016, 7, 7)));
            it("formats", () => testFormat("8/7/2016", true, new Date(2016, 6, 8)));
            it("formats", () => testFormat("1/1/16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("7/8/16", true, new Date(2016, 7, 7)));
            it("formats", () => testFormat("8/7/16", true, new Date(2016, 6, 8)));
            it("formats", () => testFormat("01/01/16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("07/08/16", true, new Date(2016, 7, 7)));
            it("formats", () => testFormat("08/07/16", true, new Date(2016, 6, 8)));

            it("formats", () => testFormat("1 Jan 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 January 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 jan 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 january 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 JAN 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 JANUARY 2016", true, new Date(2016, 0, 1)));

            it("formats", () => testFormat("01 Jan 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 January 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 jan 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 january 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 JAN 2016", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 JANUARY 2016", true, new Date(2016, 0, 1)));

            it("formats", () => testFormat("1 Jan 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 January 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 jan 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 january 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 JAN 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("1 JANUARY 16", true, new Date(2016, 0, 1)));

            it("formats", () => testFormat("01 Jan 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 January 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 jan 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 january 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 JAN 16", true, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 JANUARY 16", true, new Date(2016, 0, 1)));

            // not valid
            it("formats", () => testFormat("01 Janua 16", false, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 janu 16", false, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 januar 16", false, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 JANUA 16", false, new Date(2016, 0, 1)));
            it("formats", () => testFormat("01 JA 16", false, new Date(2016, 0, 1)));
        });
    });
});
