import { Component, OnInit } from '@angular/core';
import { NgSelectConfig } from '@ng-select/ng-select';
import { PaginationConfig } from 'ngx-bootstrap';
import { SwUpdate } from '@angular/service-worker';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(private readonly selectConfig: NgSelectConfig,
    private readonly paginationConfig: PaginationConfig,
    private readonly swUpdate: SwUpdate) {
    this.selectConfig.loadingText = 'Загрузка...';
    this.selectConfig.typeToSearchText = 'Введите текст для поиска';

    this.paginationConfig.main = {
      itemsPerPage: 20,
      firstText: 'Перв.',
      lastText: 'Послед.',
      maxSize: void 0,
      boundaryLinks: false,
      directionLinks: true,
      previousText: 'Пред.',
      nextText: 'След.',
      pageBtnClass: '',
      rotate: true
    };
  }

  ngOnInit(): void {
    if (this.swUpdate.isEnabled) {
      this.swUpdate.available.subscribe(() => {
        if (confirm('Обнаружена новая версия. Загрузить?')) {
          window.location.reload();
        }
      });
    }
  }
}
