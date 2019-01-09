import { Injectable } from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
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
        this.copiedViewModelSource.next(Ro.withUndefined(dvm));
    }

    getCopyViewModel() {
        return this.copiedViewModel;
    }
}
