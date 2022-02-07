import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { Color, BaseChartDirective, Label } from 'ng2-charts';
import * as pluginAnnotations from 'chartjs-plugin-annotation';

@Component({
  selector: 'app-card-chart-line',
  templateUrl: './card-chart-line.component.html',
  styleUrls: ['./card-chart-line.component.css']
})
export class CardChartLineComponent implements OnInit {

  @Input() info: any;

  lineChartData: ChartDataSets[] = [];
  lineChartLabels: Label[] = [];
  lineChartOptions: (ChartOptions & { annotation: any }) = {
    responsive: true,
    legend: {
      position: 'bottom'
    },
    scales: {
      // We use this empty structure as a placeholder for dynamic theming.
      xAxes: [{}],
      yAxes: [
        {
          id: 'y-axis-0',
          position: 'left',
        },
      ]
    },
    annotation: {
      annotations: [
        {
          type: 'line',
          mode: 'vertical',
          scaleID: 'x-axis-0',
          value: 'March',
          borderColor: 'orange',
          borderWidth: 2,
          label: {
            enabled: true,
            fontColor: 'orange',
            content: 'LineAnno'
          }
        },
      ],
    },
  };
  lineChartColors: Color[] = [
    { // blue
      backgroundColor: '#64b5f6',
      borderColor: '#2196f3',
      pointBackgroundColor: '#1565c0',
      pointBorderColor: '#1565c0',
      pointHoverBackgroundColor: '#1565c0',
      pointHoverBorderColor: '#1565c0'
    },
    { // green
      backgroundColor: '#a5d6a7',
      borderColor: '#66bb6a',
      pointBackgroundColor: '#43a047',
      pointBorderColor: '#43a047',
      pointHoverBackgroundColor: '#43a047',
      pointHoverBorderColor: '#43a047'
    },
  ];
  lineChartLegend = true;
  lineChartType = 'line';
  lineChartPlugins = [pluginAnnotations];

  @ViewChild(BaseChartDirective, { static: true }) chart: BaseChartDirective;

  constructor() { }

  ngOnInit() {}

  ngOnChanges() {
    this.lineChartData = [
      { data: this.info['Adjudicaciones']['Expedientes'], label: 'Adjudicaciones' },
      { data: this.info['Obras']['Expedientes'], label: 'Obra Pública' },
    ];

    this.lineChartLabels = this.info['Adjudicaciones']['Anios'];
  }

}
