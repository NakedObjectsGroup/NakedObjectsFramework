export class ToolBarViewModel {
    loading: string;
    template: string;
    footerTemplate: string;
    goHome: (right?: boolean) => void;
    goBack: () => void;
    goForward: () => void;
    swapPanes: () => void;
    logOff: () => void;
    singlePane: (right?: boolean) => void;
    recent: (right?: boolean) => void;
    cicero: () => void;
    userName: string;
    applicationProperties: () => void;

    warnings: string[];
    messages: string[];
}