import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-table-info-global-dm',
  templateUrl: './table-info-global-dm.component.html',
  styleUrls: ['./table-info-global-dm.component.css']
})
export class TableInfoGlobalDmComponent implements OnInit {

  @Input() title: string;
  @Input() info: any;

  constructor() { }

  ngOnInit(): void {
  }

}
