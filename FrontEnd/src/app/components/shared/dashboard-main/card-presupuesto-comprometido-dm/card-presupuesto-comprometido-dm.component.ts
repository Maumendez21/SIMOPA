import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-card-presupuesto-comprometido-dm',
  templateUrl: './card-presupuesto-comprometido-dm.component.html',
  styleUrls: ['./card-presupuesto-comprometido-dm.component.css']
})
export class CardPresupuestoComprometidoDmComponent implements OnInit {

  @Input() title: string = "N/D";
  @Input() presupuestoAutorizado: number = 0;
  @Input() porcentajePresupuestoAutorizado: number = 0;
  @Input() porcentajeAvanceIntengracionDoc: number = 0;

  constructor() { }

  ngOnInit(): void {
  }

}
