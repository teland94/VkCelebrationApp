export class VkCollection<T> {
    totalCount: number;
    items: T[];

    constructor() {
      this.totalCount = 0;
      this.items = [];
    }
}
