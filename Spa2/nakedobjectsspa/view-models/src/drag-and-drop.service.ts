import { Injectable } from '@angular/core';
import * as Models from '@nakedobjects/restful-objects';
import { Subject } from 'rxjs';
import { IDraggableViewModel } from './idraggable-view-model';

@Injectable()
export class DragAndDropService {

    constructor() {  }

    private copiedViewModelSource = new Subject<IDraggableViewModel>();

    copiedViewModel$ = this.copiedViewModelSource.asObservable();

    private copiedViewModel: IDraggableViewModel | null;

    setCopyViewModel(dvm: IDraggableViewModel | null) {
        this.copiedViewModel = dvm;
        this.copiedViewModelSource.next(Models.withUndefined(dvm));
    }

    getCopyViewModel() {
        return this.copiedViewModel;
    }
}
