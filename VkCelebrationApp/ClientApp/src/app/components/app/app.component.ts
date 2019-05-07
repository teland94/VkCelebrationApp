import { Component, OnInit } from '@angular/core';
import { NgSelectConfig } from '@ng-select/ng-select';
import { PaginationConfig } from 'ngx-bootstrap';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private selectConfig: NgSelectConfig,
    private paginationConfig: PaginationConfig) {
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
}
