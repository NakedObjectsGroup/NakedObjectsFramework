export class Result {
    input: string | null;
    output: string | null;

    static create = (input: string | null, output: string | null): Result => ({ input: input, output: output });
}