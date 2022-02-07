import { Component, OnInit, Input } from '@angular/core';
import { ChartDataSets, ChartOptions, ChartType, ChartPluginsOptions } from 'chart.js';
import { Label } from 'ng2-charts';
import * as pluginAnnotations  from 'chartjs-plugin-annotation';

@Component({
  selector: 'app-card-presupuesto-grafica-dm',
  templateUrl: './card-presupuesto-grafica-dm.component.html',
  styleUrls: ['./card-presupuesto-grafica-dm.component.css']
})
export class CardPresupuestoGraficaDmComponent implements OnInit {

  @Input() barChartData: ChartDataSets[];
  @Input() barChartOptions: (ChartOptions & { anotation: any });
  
  barChartLabels: Label[];
  barChartLegend = true;
  barChartType: ChartType = 'bar';
  lineChartPlugins: ChartPluginsOptions = [pluginAnnotations];

  constructor() { }

  ngOnInit(): void {
  }

}
