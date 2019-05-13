import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';
import { Sex, SearchParams, RelationType } from '../../models/search-params.model';
import { VkDatabaseService } from 'src/app/services/vk-database.service';
import { distinctUntilChanged, debounceTime, switchMap } from 'rxjs/operators';
import { VkCollection } from 'src/app/models/vk-collection.model';
import { VkCity } from 'src/app/models/vk-city.model';
import { VkUniversity } from 'src/app/models/vk-university.model';

@Component({
  selector: 'app-search-settings',
  templateUrl: './search-settings.component.html',
  styleUrls: ['./search-settings.component.css']
})
export class SearchSettingsComponent implements OnInit {

  private minAge = 14;
  private maxAge = 80;
  private defaultCountryId = 2; // Ukraine

  private relationTypes = [
    { type: RelationType.NotMarried, label: 'Не женат', sex: Sex.Male },
    { type: RelationType.NotMarried, label: 'Не замужем', sex: Sex.Female },
    { type: RelationType.HasFriend, label: 'Встречается', sex: Sex.All },
    { type: RelationType.Engaged, label: 'Помолвлен', sex: Sex.Male },
    { type: RelationType.Engaged, label: 'Помолвлена', sex: Sex.Female },
    { type: RelationType.Married, label: 'Женат', sex: Sex.Male },
    { type: RelationType.Married, label: 'Замужем', sex: Sex.Female },
    { type: RelationType.ItsComplex, label: 'Все сложно', sex: Sex.All },
    { type: RelationType.InActiveSearch, label: 'В активном поиске', sex: Sex.All },
    { type: RelationType.Engaged, label: 'Помолвлен', sex: Sex.Male },
    { type: RelationType.Engaged, label: 'Помолвлена', sex: Sex.Female },
    { type: RelationType.CivilMarriage, label: 'В гражданском браке', sex: Sex.All }
  ];

  searchSettingsForm: FormGroup;
  sex = Sex;
  fromAges: Array<number>;
  toAges: Array<number>;
  filteredRelationTypes: any;

  cities: VkCollection<VkCity>;
  universities: VkCollection<VkUniversity>;
  citiesTypeahead = new EventEmitter<string>();
  universititesTypeahead = new EventEmitter<string>();

  @Input()
  set settings(settings: SearchParams) {
    if (settings) {
      this.loadAges(settings);
      this.searchSettingsForm.patchValue(settings, { emitEvent: false });
      this.setRelationTypes();
    }
  }

  @Output() settingsChange = new EventEmitter<SearchParams>();
  @Output() settingsSaveClick = new EventEmitter<SearchParams>();
  @Output() settingsResetClick = new EventEmitter();

  constructor(private readonly fb: FormBuilder,
    private readonly vkDatabaseService: VkDatabaseService) {
    this.searchSettingsForm = this.fb.group({
      ageFrom: new FormControl(),
      ageTo: new FormControl(),
      online: new FormControl(),
      sex: new FormControl(),
      relationTypes: new FormControl(),
      cityId: new FormControl(),
      universityId: new FormControl({ value: '', disabled: true }),
      canWritePrivateMessage: new FormControl(),
      isOpened: new FormControl(),
    });
  }

  ngOnInit() {
    this.loadCities();
    this.loadTypeaheads();
    this.onChanges();
  }

  public loadCities() {
    this.vkDatabaseService.getCities(this.defaultCountryId)
      .subscribe((data: VkCollection<VkCity>) => {
      this.cities = data;
      this.loadUniversities();
    });
  }

  public loadUniversities() {
    const selectedCity = this.searchSettingsForm.get('cityId').value;
    if (selectedCity) {
      this.vkDatabaseService.getUniversities(this.defaultCountryId, selectedCity)
        .subscribe((data: VkCollection<VkUniversity>) => {
        this.universities = data;
        this.searchSettingsForm.get('universityId').enable();
      });
    } else {
      this.universities = new VkCollection<VkUniversity>();
      this.searchSettingsForm.get('universityId').setValue(null);
      this.searchSettingsForm.get('universityId').disable();
    }
  }

  onSubmit() {
    this.settingsSaveClick.emit(this.searchSettingsForm.value);
  }

  onReset() {
    this.settingsResetClick.emit();
  }

  private loadAges(settings?: SearchParams) {
    this.fromAges = new Array();
    this.toAges = new Array();
    const ageFrom = settings ? settings.ageFrom : this.minAge;
    const ageTo = settings ? settings.ageTo : this.maxAge;
    for (let i = this.minAge; i <= ageTo; i++) {
      this.fromAges.push(i);
    }
    for (let i = ageFrom; i <= this.maxAge; i++) {
      this.toAges.push(i);
    }
  }

  private setRelationTypes() {
    let filterSex = this.searchSettingsForm.get('sex').value;
    if (filterSex === Sex.All) { filterSex = Sex.Male; }
    this.filteredRelationTypes = this.relationTypes
      .filter(rt => rt.sex === Sex.All || rt.sex === filterSex);
  }

  private loadTypeaheads() {
    this.citiesTypeahead
    .pipe(
      distinctUntilChanged(),
      debounceTime(200),
      switchMap((text: string) => this.vkDatabaseService.getCities(this.defaultCountryId, text ? text : ''))
    )
    .subscribe((items: VkCollection<VkCity>) => {
      this.cities = items;
    });

    this.universititesTypeahead
      .pipe(
        distinctUntilChanged(),
        debounceTime(200),
        switchMap((text: string) => this.vkDatabaseService.getUniversities(this.defaultCountryId,
          this.searchSettingsForm.get('cityId').value, text ? text : ''))
      )
      .subscribe((data: VkCollection<VkUniversity>) => {
        this.universities = data;
      });
  }

  private onChanges() {
    this.searchSettingsForm.valueChanges.subscribe(val => {
      if (val.canWritePrivateMessage == null) { return; }
      this.settingsChange.emit(val);
    });
  }
}
