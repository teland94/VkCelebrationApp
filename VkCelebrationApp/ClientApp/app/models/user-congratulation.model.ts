﻿import { VkUser } from "./vk-user.model";

export class UserCongratulation {
    id: number;
    text: string;
    congratulationDate: Date;
    vkUserId: number;
    vkUser: VkUser;
    userId: number;

    constructor(text: string, vkUserId: number) {
        this.text = text;
        this.vkUserId = vkUserId;
    }
}