import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiConditionalChoices]' })
export class GeminiConditionalChoicesDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

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

//function isDomainObjectViewModel(object: any): object is DomainObjectViewModel {
//    return object && "properties" in object;
//}

//function mapValues(args: _.Dictionary<Value>, parmsOrProps: { argId: string, getValue: () => Value }[]) {
//    return _.mapValues(pArgs, (v, n) => {
//        const pop = _.find(parmsOrProps, p => p.argId === n);
//        return pop.getValue();
//    });
//}


//function populateArguments() {

//    const dialog = parent.dialog;
//    const object = parent.object;

//    if (!dialog && !object) {
//        throw { message: "Expect dialog or object in geminiConditionalchoices", stack: "" };
//    }

//    let parmsOrProps: { argId: string, getValue: () => Value }[] = [];

//    if (dialog) {
//        parmsOrProps = dialog.parameters;
//    }

//    if (isDomainObjectViewModel(object)) {
//        parmsOrProps = object.properties;
//    }

//    return mapValues(pArgs, parmsOrProps);
//}

//function populateDropdown() {
//    const nArgs = populateArguments();
//    const prompts = scope.select({ args: nArgs });
//    prompts.then((cvms: ChoiceViewModel[]) => {
//        // if unchanged return 
//        if (cvms.length === currentOptions.length && _.every(cvms, (c, i) => c.equals(currentOptions[i]))) {
//            return;
//        }

//        element.find("option").remove();

//        if (viewModel.optional) {
//            const emptyOpt = $("<option></option>");
//            element.append(emptyOpt);
//        }

//        _.forEach(cvms, cvm => {

//            const opt = $("<option></option>");
//            opt.val(cvm.getValue().toValueString());
//            opt.text(cvm.name);

//            element.append(opt);
//        });

//        currentOptions = cvms;

//        if (viewModel.entryType === EntryType.MultipleConditionalChoices) {
//            const vals = _.map(viewModel.selectedMultiChoices, c => c.getValue().toValueString());
//            $(element).val(vals);
//        } else if (viewModel.selectedChoice) {
//            $(element).val(viewModel.selectedChoice.getValue().toValueString());
//        }
//        else {
//            $(element).val("");
//        }

//        setTimeout(() => {
//            $(element).change();
//        }, 1);


//    }).catch(() => {
//        // error clear everything 
//        element.find("option").remove();
//        viewModel.selectedChoice = null;
//        currentOptions = [];
//    });
//}

//function wrapReferences(val: string): string | RoInterfaces.ILink {
//    if (val && viewModel.type === "ref") {
//        return { href: val };
//    }
//    return val;
//}

//function optionChanged() {

//    if (viewModel.entryType === EntryType.MultipleConditionalChoices) {
//        const options = $(element).find("option:selected");
//        const kvps = [] as any[];

//        options.each((n, e) => kvps.push({ key: $(e).text(), value: $(e).val() }));
//        const cvms = _.map(kvps, o => ChoiceViewModel.create(new Value(wrapReferences(o.value)), viewModel.id, o.key));
//        viewModel.selectedMultiChoices = cvms;

//    } else {
//        const option = $(element).find("option:selected");
//        const val = option.val();
//        const key = option.text();
//        const cvm = ChoiceViewModel.create(new Value(wrapReferences(val)), viewModel.id, key);
//        viewModel.selectedChoice = cvm;
//        scope.$apply(() => {
//            ngModel.$parsers.push(() => cvm);
//            ngModel.$setViewValue(cvm.name);
//        });
//    }
//}


//function setListeners() {
//    _.forEach(pArgs, (v, n) => $(`#${n}${paneId}`).on("change", () => populateDropdown()));
//    $(element).on("change", optionChanged);
//}

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