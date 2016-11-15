import { IMessageViewModel } from './imessage-view-model';

export abstract class MessageViewModel implements IMessageViewModel {
    private previousMessage = "";
    private message = "";

    clearMessage = () => {
        if (this.message === this.previousMessage) {
            this.resetMessage();
        } else {
            this.previousMessage = this.message;
        }
    };
    resetMessage = () => this.message = this.previousMessage = "";
    setMessage = (msg: string) => this.message = msg;
    getMessage = () => this.message;
}