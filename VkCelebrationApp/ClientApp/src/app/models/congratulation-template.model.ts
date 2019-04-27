export class CongratulationTemplate {
    id: number;
    text: string;
    createdById: number;

    constructor(text: string) {
        this.text = text;
    }
}