import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { IDraggableViewModel } from './idraggable-view-model';

@Injectable()
export class DragAndDropService {

    private copiedViewModelSource = new Subject<IDraggableViewModel | null>();

    copiedViewModel$ = this.copiedViewModelSource.asObservable();

    private copiedViewModel: IDraggableViewModel | null = null;

    private dropZoneIds: string[] = [];
    // Use BehaviorSubject so that subscriptions pick up current values
    private dropZoneIdsSource = new BehaviorSubject<string[]>(this.dropZoneIds);

    dropZoneIds$ = this.dropZoneIdsSource.asObservable();

    setCopyViewModel(dvm: IDraggableViewModel | null) {
        this.copiedViewModel = dvm;
        this.copiedViewModelSource.next(dvm);
    }

    getCopyViewModel() {
        return this.copiedViewModel;
    }

    setDropZoneId (id: string) {
        if (!this.dropZoneIds.includes(id)) {
            this.dropZoneIds.push(id);
            this.dropZoneIdsSource.next(this.dropZoneIds);
        }
    }

    clearDropZoneId(id: string) {
        if (this.dropZoneIds.includes(id)) {
            this.dropZoneIds = this.dropZoneIds.filter(v => v !== id);
            this.dropZoneIdsSource.next(this.dropZoneIds);
        }
    }
}
