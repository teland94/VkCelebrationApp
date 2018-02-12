import { Directive, ElementRef, AfterViewChecked } from '@angular/core';

@Directive({
    selector: '[appAutofocus]'
})
export class AutofocusDirective implements AfterViewChecked {

    constructor(private el: ElementRef) {
    }

    ngAfterViewChecked() {
        this.el.nativeElement.focus();
    }

}