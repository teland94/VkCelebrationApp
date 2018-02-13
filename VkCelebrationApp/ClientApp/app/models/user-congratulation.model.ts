﻿export class UserCongratulation {
    id: number;
    text: string;
    congratulationDate: Date;
    vkUserId: number;
    userId: number;

    constructor(text: string, vkUserId: number) {
        this.text = text;
        this.vkUserId = vkUserId;
    }
}