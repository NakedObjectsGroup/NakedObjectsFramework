export class Result {
    input: string;
    output: string;

    static create = (input: string | null, output: string | null): Result => ({ input: input, output: output });
}