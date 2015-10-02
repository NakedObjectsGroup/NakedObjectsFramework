module NakedObjects.Cicero {

    export class Command {

        constructor(private input: string) {
            this.parse();
        }

        parse(): void { //TODO: Make abstract?
        };

        newPath(): string {
            return null;
        }
    }

    export class Home extends Command {

        newPath(): string {
            return "/cicero/home";
        }
    }

    export class Gemini extends Command {

        newPath(): string {
            return "/gemini/home";
        }
    }

    export class Unrecognised extends Command  {

    }
}