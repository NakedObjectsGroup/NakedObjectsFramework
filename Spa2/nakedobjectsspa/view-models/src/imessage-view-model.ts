export interface IMessageViewModel {
    clearMessage: () => void;
    resetMessage: () => void;
    setMessage: (msg: string) => void;
    getMessage: () => string;
}
