export class Result {
    input: string | null = null;
    output: string | null = null;

    static create(input: string | null, output: string | null): Result {
        return { input: input, output: output };
    }
}
