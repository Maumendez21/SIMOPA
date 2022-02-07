import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-card-top-dm',
  templateUrl: './card-top-dm.component.html',
  styleUrls: ['./card-top-dm.component.css']
})
export class CardTopDmComponent implements OnInit {

  @Input() presupuestoComprometido: any;
  @Input() presupuestoAutorizado: any;
  @Input() needleValue: number = 0;

  canvasWidth = 150
  centralLabel = ''
  options = {
    hasNeedle: true,
    needleColor: 'black',
    arcColors: ['rgb(255,84,84)', 'rgb(239,214,19)', 'rgb(61,204,91)'],
    arcDelimiters: [40,60],
    rangeLabel: ['0', '100'],
    needleStartValue: 50,
  }

  constructor() { }

  ngOnInit(): void {
  }

}
