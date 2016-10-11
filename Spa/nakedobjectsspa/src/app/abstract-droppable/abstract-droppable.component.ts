import * as ViewModels from "../view-models";

export class AbstractDroppableComponent {

    canDrop = false;

    droppable : ViewModels.IFieldViewModel;

    accept = (draggableVm: ViewModels.IDraggableViewModel) => {

        if (draggableVm) {
            draggableVm.canDropOn(this.droppable.returnType).
                then((canDrop: boolean) => this.canDrop = canDrop).
                catch(() => this.canDrop = false);
            return true;
        }
        return false;
    };

    drop(draggableVm: ViewModels.IDraggableViewModel) {
        if (this.canDrop) {
            this.droppable.drop(draggableVm);
        }
    }
}