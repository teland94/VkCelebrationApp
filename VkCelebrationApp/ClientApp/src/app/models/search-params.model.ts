export enum Sex {
  All = 0,
  Female = 1,
  Male = 2
}

export enum RelationType {
  Unknown = 0,
  NotMarried = 1,
  HasFriend = 2,
  Engaged = 3,
  Married = 4,
  ItsComplex = 5,
  InActiveSearch = 6,
  Amorous = 7,
  CivilMarriage = 8
}

export enum LastSeenMode {
  Online = 0,
  Last24Hours = 1
}

export class SearchParams {
  ageFrom: number;
  ageTo: number;
  lastSeenMode: LastSeenMode;
  sex: Sex;
  relationTypes: Array<RelationType>;
  cityId?: number;
  universityId?: number;
  canWritePrivateMessage: boolean;
  isOpened: boolean;

  constructor(cityId?: number, universityId?: number) {
    this.ageFrom = 18;
    this.ageTo = 28;
    this.lastSeenMode = LastSeenMode.Online;
    this.sex = Sex.Female;
    this.cityId = cityId;
    this.universityId = universityId;
    this.canWritePrivateMessage = true;
    this.isOpened = true;
    this.relationTypes = [
      RelationType.NotMarried,
      RelationType.InActiveSearch
    ];
  }
}
