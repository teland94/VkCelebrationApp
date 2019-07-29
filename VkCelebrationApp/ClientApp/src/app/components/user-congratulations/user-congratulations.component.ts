import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserCongratulationsService } from '../../services/user-congratulations.service';
import { UserCongratulation } from '../../models/user-congratulation.model';
import { ToastrService } from 'ngx-toastr';
import { BsLocaleService } from 'ngx-bootstrap';
import { UtilitiesService } from 'src/app/services/utilities.service';
import { finalize } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-congratulations',
  templateUrl: './user-congratulations.component.html',
  styleUrls: ['./user-congratulations.component.css']
})
export class UserCongratulationsComponent implements OnInit, OnDestroy {

  private exportSubscription: Subscription;

  baseUrl = 'http://vk.com/id';

  userCongratulations: UserCongratulation[];

  minDate = new Date(2018, 1, 1);
  maxDate = new Date();

  currentDate = new Date();

  loading = false;

  constructor(private readonly userCongratulationsService: UserCongratulationsService,
    private readonly utilitiesService: UtilitiesService,
    private readonly toastr: ToastrService,
    private readonly localeService: BsLocaleService) {
  }

  ngOnInit() {
    this.localeService.use('ru');
    this.load();
  }

  load() {
    const date = this.getDate(this.currentDate);
    this.userCongratulationsService.getUserCongratulations(date)
      .subscribe(data => {
        this.userCongratulations = data;

        window.scrollTo(0, 0);
      }, () => {
        this.showErrorToast('Ошибка загрузки истории поздравлений');
      });
  }

  getImageData(url: string) {
    return this.utilitiesService.getImageData(url);
  }

  dateChanged(date: Date) {
    if (this.currentDate !== date) {
      this.currentDate = date;
      this.load();
    }
  }

  export() {
    this.loading = true;
    const date = this.getDate(this.currentDate);
    this.exportSubscription = this.userCongratulationsService.getUserCongratulationsExcelData(date)
      .pipe(finalize(() => this.loading = false))
      .subscribe(data => {
        this.downloadData(data, `Congrats ${date.toLocaleDateString()}.xlsx`);
      }, error => {
        if (error.status === 400) {
          this.showErrorToast(error.error);
        }
        console.log(error);
      });
  }

  exportAll() {
    this.loading = true;
    this.exportSubscription = this.userCongratulationsService.getUserCongratulationsExcelData()
      .pipe(finalize(() => this.loading = false))
      .subscribe(data => {
        this.downloadData(data, `Congrats.xlsx`);
      }, error => {
        if (error.status === 400) {
          this.showErrorToast(error.error);
        }
        console.log(error);
      });
  }

  ngOnDestroy() {
    if (this.exportSubscription) {
      this.exportSubscription.unsubscribe();
    }
  }

  private downloadData(data: any, fileName: string) {
    const url = window.URL.createObjectURL(data);

    const downloadLink = document.createElement('a');
    downloadLink.href = url;
    downloadLink.download = fileName;
    downloadLink.click();

    window.URL.revokeObjectURL(url);
  }

  private getDate(date: Date) {
    return new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), this.currentDate.getDate(), 0, 0, 0);
  }

  private showErrorToast(message: string) {
    const errToast = this.toastr.error(message, 'Ошибка');
    if (errToast) {
      errToast.onTap.subscribe(() => {
        this.load();
      });
    }
  }
}
