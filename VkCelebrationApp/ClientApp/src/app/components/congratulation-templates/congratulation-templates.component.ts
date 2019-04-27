import { Component, OnInit } from '@angular/core';
import { CongratulationTemplatesService } from '../../services/congratulation-templates.service';
import { CongratulationTemplate } from '../../models/congratulation-template.model';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-congratulation-templates',
    templateUrl: './congratulation-templates.component.html',
    styleUrls: ['./congratulation-templates.component.css']
})

export class CongratulationTemplatesComponent implements OnInit {

    congratulationTemplates: CongratulationTemplate[];
    congratulationTemplate: CongratulationTemplate;
    tableMode: boolean = true;
    submitted: boolean = false;

    constructor(private readonly congratulationTemplatesService: CongratulationTemplatesService,
        private toastr: ToastrService) {

    }

    ngOnInit() {
        this.load();
    }

    load() {
        this.congratulationTemplatesService.getCongratulationTemplates().subscribe((data: CongratulationTemplate[]) => {
            this.congratulationTemplates = data;
        }, () => {
            this.showErrorToast('Ошибка загрузки заготовок поздравлений');
        });
    }

    save() {
        this.submitted = true;
        if (this.congratulationTemplate.id == null) {
            this.congratulationTemplatesService.createCongratulationTemplate(this.congratulationTemplate)
                .subscribe((data: CongratulationTemplate) => {
                    this.load();
                    this.toastr.success('Успешно создано', 'Успех');
                }, () => {
                    this.showErrorToast('Ошибка добавления');
                });
        } else {
            this.congratulationTemplatesService.updateCongratulationTemplate(this.congratulationTemplate)
                .pipe(finalize(() => this.load()))
                .subscribe(() => {
                    this.toastr.success('Успешно обновлено', 'Успех');
                }, () => {
                    this.showErrorToast('Ошибка обновления');
                });
        }
        this.cancel();
    }

    editCongratulationTemplate(t: CongratulationTemplate) {
        this.congratulationTemplate = t;
    }

    cancel() {
        this.congratulationTemplate = new CongratulationTemplate('');
        this.tableMode = true;
        this.submitted = false;
    }

    delete(t: CongratulationTemplate) {
        this.congratulationTemplatesService.deleteCongratulationTemplate(t.id)
            .subscribe(() => {
                this.load();
                this.toastr.success('Успешно удалено', 'Успех');
            }, () => {
                this.showErrorToast('Ошибка удаления');
            });
    }

    add() {
        this.cancel();
        this.tableMode = false;
    }

    showErrorToast(message: string) {
        const errToast = this.toastr.error(message, 'Ошибка');
        if (errToast) {
            errToast.onTap.subscribe(() => {
                this.load();
            });
        }
    }
}
