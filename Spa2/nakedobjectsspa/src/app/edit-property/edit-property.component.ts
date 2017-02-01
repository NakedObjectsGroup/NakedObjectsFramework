import { Component, Input, ElementRef, OnInit, HostListener, ViewChildren, QueryList } from '@angular/core';
import * as Models from "../models";
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorService } from "../error.service";
import { ContextService } from "../context.service";
import { AttachmentViewModel } from '../view-models/attachment-view-model';
import { FieldViewModel } from '../view-models/field-view-model';
import { PropertyViewModel } from '../view-models/property-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { ConfigService } from '../config.service';

@Component({
    host: {
        '(document:click)': 'handleClick($event)'
    },
    selector: 'nof-edit-property',
    templateUrl: './edit-property.component.html',
    styleUrls: ['./edit-property.component.css']
})
export class EditPropertyComponent extends FieldComponent implements OnInit {

    constructor(
        myElement: ElementRef,
        private readonly router: Router,
        private readonly error: ErrorService,
        context: ContextService,
        configService: ConfigService
    ) {
        super(myElement, context, configService);
    }

    prop: PropertyViewModel;

    @Input()
    parent: DomainObjectViewModel;

    @Input()
    set property(value: PropertyViewModel) {
        this.prop = value;
        this.droppable = value as FieldViewModel;
    }

    get property() {
        return this.prop;
    }

    get propertyPaneId() {
        return this.property.paneArgId;
    }

    get propertyId() {
        return this.property.id;
    }

    get propertyChoices() {
        return this.property.choices;
    }

    get title() {
        return this.property.title;
    }

    get propertyType() {
        return this.property.type;
    }

    get propertyReturnType() {
        return this.property.returnType;
    }

    get propertyEntryType() {
        return this.property.entryType;
    }

    get isEditable() {
        return this.property.isEditable;
    }

    get formattedValue() {
        return this.property.formattedValue;
    }

    get value() {
        return this.property.formattedValue;
    }

    get format() {
        return this.property.format;
    }

    get isBlob() {
        return this.property.format === "blob";
    }

    get isMultiline() {
        return !(this.property.multipleLines === 1);
    }

    get isPassword() {
        return this.property.password;
    }

    get multilineHeight() {
        return `${this.property.multipleLines * 20}px`;
    }

    get rows() {
        return this.property.multipleLines;
    }

    get propertyDescription() {
        return this.property.description;
    }

    get message() {
        return this.property.getMessage();
    }

    choiceName(choice: ChoiceViewModel) {
        return choice.name;
    }


    classes(): string {
        return `${this.prop.color}${this.canDrop ? " candrop" : ""}`;
    }

    @Input()
    set form(fm: FormGroup) {
        this.formGroup = fm;
    }

    get form() {
        return this.formGroup;
    }

    // todo delegated click here smell that we need another component 
    doAttachmentClick = (right?: boolean) => this.property.attachment!.doClick(right);

    ngOnInit(): void {
        super.init(this.parent, this.property, this.form.controls[this.prop.id]);

        // todo this is cloned across view/edit property types - DRY it!!!!! 
        if (this.property && this.property.attachment) {
            const attachment = this.property.attachment;

            this.attachmentTitle = attachment.title;

            if (attachment.displayInline()) {
                attachment.downloadFile()
                    .then(blob => {
                        const reader = new FileReader();
                        reader.onloadend = () => {
                            if (reader.result) {
                                this.image = reader.result;
                            }
                        };
                        reader.readAsDataURL(blob);
                    })
                    .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            } else {
                attachment.doClick = this.clickHandler(attachment);
            }

        } else {
            this.attachmentTitle = "Attachment not yet supported on transient";
        }
    }

    datePickerChanged(evt: any) {
        const val = evt.currentTarget.value;
        this.formGroup.value[this.property.id] = val;
    }

    clickHandler(attachment: AttachmentViewModel) {

        return () => {

            if (!attachment.displayInline()) {
                attachment.downloadFile()
                    .then(blob => {
                        if (window.navigator.msSaveBlob) {
                            // internet explorer 
                            window.navigator.msSaveBlob(blob, attachment.title);
                        } else {
                            const burl = URL.createObjectURL(blob);
                            this.router.navigateByUrl(burl);
                        }
                    })
                    .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            }

            return false;
        };
    };

    attachmentTitle: string;
    image: string;

    @HostListener('keydown', ['$event'])
    onEnter(event: KeyboardEvent) {
        this.paste(event);
    }

    @HostListener('keypress', ['$event'])
    onEnter1(event: KeyboardEvent) {
        this.paste(event);
    }

    @ViewChildren("focus")
    focusList: QueryList<ElementRef>;
}