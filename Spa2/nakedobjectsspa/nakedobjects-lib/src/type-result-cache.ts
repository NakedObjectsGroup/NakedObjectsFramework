import { ContextService } from './context.service';
import { Dictionary } from 'lodash';

export abstract class TypeResultCache<T> {

    protected constructor(protected readonly context: ContextService) { }

    private readonly resultCache: Dictionary<T> = {};
    private readonly regexCache: { regex: RegExp, result: T }[] = [];
    private readonly subtypeCache: { type: string, result: T }[] = [];

    protected default: T;

    addType(type: string, result: T) {
        this.resultCache[type] = result;
    }

    addMatch(matcher: RegExp, result: T) {
        this.regexCache.push({ regex: matcher, result: result });
    }

    addSubtype(type: string, result: T) {
        this.subtypeCache.push({ type: type, result: result });
    }

    setDefault(def: T) {
        this.default = def;
    }

    private cacheAndReturn(type: string, result: T) {
        this.resultCache[type] = result;
        return Promise.resolve(result);
    }

    private isSubtypeOf(subtype: string, index: number, count: number): Promise<T> {

        if (index >= count) {
            return Promise.reject("") as any;
        }

        const entry = this.subtypeCache[index];
        return this.context.isSubTypeOf(subtype, entry.type).then(b => b ? Promise.resolve(entry.result) : this.isSubtypeOf(subtype, index + 1, count));
    }

    private isSubtype(subtype: string): Promise<any> {

        const subtypeChecks = this.subtypeCache.length;

        if (subtypeChecks > 0) {
            return this.isSubtypeOf(subtype, 0, subtypeChecks)
                .then((c: T) => {
                    return this.cacheAndReturn(subtype, c);
                })
                .catch(() => {
                    return this.cacheAndReturn(subtype, this.default);
                });
        }

        return this.cacheAndReturn(subtype, this.default);
    }

    getResult(type: string | null) {
        // 1 cache
        // 2 match regex
        // 3 match subtype

        // this is potentially expensive - need to filter out non ref types ASAP

        if (!type || type === "string" || type === "number" || type === "boolean") {
            return Promise.resolve(this.default);
        }

        const cachedEntry = this.resultCache[type];

        if (cachedEntry) {
            return Promise.resolve(cachedEntry);
        }

        for (const entry of this.regexCache) {
            if (entry.regex.test(type)) {
                return this.cacheAndReturn(type, entry.result);
            }
        }

        return this.isSubtype(type);
    }
}
