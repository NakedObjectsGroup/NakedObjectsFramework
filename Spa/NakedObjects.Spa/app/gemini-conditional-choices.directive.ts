import { Directive, ElementRef, HostListener, Output, EventEmitter, Input, OnInit } from '@angular/core';
import * as ViewModels  from './nakedobjects.viewmodels';
import * as Models from './models';
import * as _ from "lodash";
import * as Ro from './nakedobjects.rointerfaces';
import { Observable } from 'rxjs/Observable';
import "./rxjs-extensions";
import { Subject } from 'rxjs/Subject';

@Directive({ selector: '[geminiConditionalChoices]' })
export class GeminiConditionalChoicesDirective implements OnInit {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    model: ViewModels.ParameterViewModel;
    currentOptions: ViewModels.ChoiceViewModel[] = [];
    pArgs: _.Dictionary<Models.Value>;

    paneId : number;

    @Input('geminiConditionalChoices')
    set viewModel(vm: ViewModels.ParameterViewModel) {
        this.model = vm;
        this.pArgs = _.omit(this.model.promptArguments, "x-ro-nof-members") as _.Dictionary<Models.Value>;
        this.paneId = this.model.onPaneId;
    }

    @Input('parent')
    parent : ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel;

    @Input('parameterChanged')
    parameterChanged: Observable<boolean>;


//    // up the priority of this directive to that viewmodel is set before ng-options - 
//    // then angular doesn't add an empty entry on dropdown
//    priority: 10,
//    // Enforce the angularJS default of restricting the directive to
//    // attributes only
//    restrict: "A",
//    // Always use along with an ng-model
//    require: "?ngModel",
//    // This method needs to be defined and passed in from the
//    // passed in to the directive from the view controller
//    scope: {
//        select: "&" // Bind the select function we refer to the right scope
//    },
//    link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
//        if(!ngModel) return;

//    const parent = scope.$parent as IPropertyOrParameterScope;
//    const viewModel = parent.parameter || parent.property;
//    const pArgs = _.omit(viewModel.promptArguments, "x-ro-nof-members") as _.Dictionary<Value>;
//    const paneId = viewModel.onPaneId;
//                let currentOptions: ChoiceViewModel[] = [];

 isDomainObjectViewModel(object: any): object is ViewModels.DomainObjectViewModel {
    return object && "properties" in object;
}

 mapValues(args: _.Dictionary<Models.Value>, parmsOrProps: { argId: string, getValue: () => Models.Value }[]) {
    return _.mapValues(this.pArgs, (v, n) => {
        const pop = _.find(parmsOrProps, p => p.argId === n);
        return pop.getValue();
    });
}

 populateArguments() {

     const dialog = this.parent as ViewModels.DialogViewModel;
     const object = this.parent as ViewModels.DomainObjectViewModel;

    if (!dialog && !object) {
        throw { message: "Expect dialog or object in geminiConditionalchoices", stack: "" };
    }

    let parmsOrProps: { argId: string, getValue: () => Models.Value }[] = [];

     if (this.isDomainObjectViewModel(object)) {
         parmsOrProps = object.properties;
     } else {
        parmsOrProps = dialog.parameters;
     }

     return this.mapValues(this.pArgs, parmsOrProps);
}

 populateDropdown() {
    const nArgs = this.populateArguments();
    const prompts = this.model.conditionalChoices(nArgs);//  scope.select({ args: nArgs });
    prompts.then((cvms: ViewModels.ChoiceViewModel[]) => {
        // if unchanged return 
        if (cvms.length === this.currentOptions.length && _.every(cvms, (c, i) => c.equals(this.currentOptions[i]))) {
            return;
        }
        this.model.choices = cvms;

        //element.find("option").remove();

        //if (viewModel.optional) {
        //    const emptyOpt = $("<option></option>");
        //    element.append(emptyOpt);
        //}

        //_.forEach(cvms, cvm => {

        //    const opt = $("<option></option>");
        //    opt.val(cvm.getValue().toValueString());
        //    opt.text(cvm.name);

        //    element.append(opt);
        //});

        this.currentOptions = cvms;

        if (this.model.entryType === Models.EntryType.MultipleConditionalChoices) {
            const vals = _.map(this.model.selectedMultiChoices, c => c.getValue().toValueString());
            //$(element).val(vals);
        } else if (this.model.selectedChoice) {
            //$(element).val(this.model.selectedChoice.getValue().toValueString());
        }
        else {
            //$(element).val("");
        }

        //setTimeout(() => {
        //    $(element).change();
        //}, 1);


    }).catch(() => {
        // error clear everything 
        //element.find("option").remove();
        this.model.selectedChoice = null;
        this.currentOptions = [];
    });
}

wrapReferences(val: string): string | Ro.ILink {
    if (val && this.model.type === "ref") {
        return { href: val };
    }
    return val;
 }

 optionChanged() {

    // if (this.model.entryType === Models.EntryType.MultipleConditionalChoices) {
    //    //const options = $(element).find("option:selected");
    //    const kvps = [] as any[];

    //    options.each((n, e) => kvps.push({ key: $(e).text(), value: $(e).val() }));
    //    const cvms = _.map(kvps, o => ViewModels.ChoiceViewModel.create(new Models.Value(this.wrapReferences(o.value)), this.model.id, o.key));
    //    this.model.selectedMultiChoices = cvms;

    //} else {
    //    //const option = $(element).find("option:selected");
    //    const val = option.val();
    //    const key = option.text();
    //    const cvm = ViewModels.ChoiceViewModel.create(new Models.Value(this.wrapReferences(val)), this.model.id, key);
    //    this.model.selectedChoice = cvm;
    //    scope.$apply(() => {
    //        ngModel.$parsers.push(() => cvm);
    //        ngModel.$setViewValue(cvm.name);
    //    });
    //}
}

 ngOnInit(): void {
     //this.setListeners();
     this.populateDropdown();

     this.parameterChanged.subscribe((change) => {

         if (change) {
             this.populateDropdown();
         }

     });

 }


setListeners() {
    _.forEach(this.pArgs, (v, n) => document.getElementById(`${n}${this.paneId}`).addEventListener("change", () =>

       this.populateDropdown()));
    //$(element).on("change", optionChanged);
}

@HostListener('change')
onChange() {
   this.optionChanged();
}


//ngModel.$render = () => { }; // do on the next event loop,

//setTimeout(() => {
//    setListeners();
//    // initial populate

//    // do this initially so that there is a valid model 
//    // otherwise angular will insert another empty value giving two 
//    element.find("option").remove();
//    if (viewModel.optional) {
//        const emptyOpt = $("<option></option>");
//        element.append(emptyOpt);
//        $(element).val("");
//    }

//    populateDropdown();
//}, 1);
//            }
//        };


}