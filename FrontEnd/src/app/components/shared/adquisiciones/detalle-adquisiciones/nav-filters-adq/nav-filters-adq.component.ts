import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-nav-filters-adq',
  templateUrl: './nav-filters-adq.component.html',
  styleUrls: ['./nav-filters-adq.component.css']
})
export class NavFiltersAdqComponent implements OnInit {

  @Input() adjudicaciones: any[] = [];
  @Input() busqueda: string;
  @Input() estatus: number = 2;
  @Input() estatusExpediente: string;
  @Input() ejercicio: number;

  @Input() changeStatus: any;
  @Input() changeEstatusExpediente: any;
  @Input() changeOrder: any;
  @Input() buscar: any;

  constructor() { }

  ngOnInit(): void {
  }

}
