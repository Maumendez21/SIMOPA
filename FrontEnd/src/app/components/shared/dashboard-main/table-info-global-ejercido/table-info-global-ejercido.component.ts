import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-table-info-global-ejercido',
  templateUrl: './table-info-global-ejercido.component.html',
  styleUrls: ['./table-info-global-ejercido.component.css']
})
export class TableInfoGlobalEjercidoComponent implements OnInit {
  
  @Input() info: any;

  constructor() { }

  ngOnInit(): void {
  }

}
