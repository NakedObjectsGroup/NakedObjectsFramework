import * as Ro from './ro-interfaces';
import { Dictionary } from 'lodash';

export type CachableTypes = Ro.IRepresentation | Blob;

class LruNode {

    constructor(public readonly key: string, public value: CachableTypes) { }

    next: LruNode | null = null;
    previous: LruNode | null = null;
}

export class SimpleLruCache {

    constructor(private readonly depth: number) { }

    private cache: Dictionary<LruNode> = {};
    private count = 0;
    private head: LruNode | null = null;
    private tail: LruNode | null = null;

    private unlinkNode(node: LruNode) {
        const nodePrevious = node.previous;
        const nodeNext = node.next;

        if (nodePrevious) {
            nodePrevious.next = nodeNext;
        } else {
            // was head
            this.head = nodeNext;
        }

        if (nodeNext) {
            nodeNext.previous = nodePrevious;
        } else {
            // was tail
            this.tail = nodePrevious;
        }
        this.count--;
    }

    private moveNodeToHead(node: LruNode) {
        const existingHead = this.head;
        node.previous = null;
        node.next = existingHead;

        if (existingHead) {
            existingHead.previous = node;
        } else {
            // no existing head so this is also tail
            this.tail = node;
        }
        this.head = node;
        this.count++;
    }

    add(key: string, value: CachableTypes) {

        if (this.cache[key]) {
            this.updateExistingEntry(key, value);
        } else {
            this.addNewEntry(key, value);
        }
    }

    remove(key: string) {
        const node = this.cache[key];

        if (node) {
            this.unlinkNode(node);
            delete this.cache[key];
        }
    }

    removeAll() {
        this.head = this.tail = null;
        this.cache = {};
        this.count = 0;
    }

    private getNode(key: string): LruNode | null {
        const node = this.cache[key];

        if (node) {
            this.unlinkNode(node);
            this.moveNodeToHead(node);
        }
        return node;
    }

    get(key: string): CachableTypes | null {
        const node = this.getNode(key);
        return node ? node.value : null;
    }

    private updateExistingEntry(key: string, value: CachableTypes): void {
        const node = this.getNode(key) !;
        node.value = value;
    }

    private addNewEntry(key: string, value: CachableTypes): void {
        const newNode = new LruNode(key, value);
        this.cache[key] = newNode;
        this.moveNodeToHead(newNode);
        this.trimCache();
    }

    private trimCache() {
        while (this.count > this.depth) {
            const tail = this.tail;
            if (tail) {
                this.unlinkNode(tail);
                delete this.cache[tail.key];
            }
        }
    }
}
