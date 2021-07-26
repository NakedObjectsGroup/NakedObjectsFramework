import { Injectable } from '@angular/core';

@Injectable()
export class Base64ObfuscateService {
    public obfuscate(s: string) {
        return s ? btoa(s) : s;
    }

    public deobfuscate(s: string) {
        return s ? atob(s) : s;
    }
}
