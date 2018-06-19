import { IMessageViewModel } from './imessage-view-model';

export abstract class MessageViewModel implements IMessageViewModel {
    private previousMessage = "";
    private message = "";

    readonly clearMessage = () => {
        if (this.message === this.previousMessage) {
            this.resetMessage();
        } else {
            this.previousMessage = this.message;
        }
    }
    readonly resetMessage = () => this.message = this.previousMessage = "";
    readonly setMessage = (msg: string) => this.message = msg;
    readonly getMessage = () => this.message;
}
