import { VkUser } from './vk-user.model';

export class VkCollection<T> {
    totalCount: number;
    items: T[];
}
