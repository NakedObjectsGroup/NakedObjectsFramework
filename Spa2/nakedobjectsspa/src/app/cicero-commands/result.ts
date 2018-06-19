export class Result {
    input: string | null;
    output: string | null;

    static create(input: string | null, output: string | null): Result {
        return { input: input, output: output };
    }
}
