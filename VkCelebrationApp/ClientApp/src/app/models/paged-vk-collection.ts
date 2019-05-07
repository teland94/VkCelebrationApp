import { VkCollection } from "./vk-collection.model";

export class PagedVkCollection<T> {
  vkCollection: VkCollection<T>;
  totalCount: number;
}
