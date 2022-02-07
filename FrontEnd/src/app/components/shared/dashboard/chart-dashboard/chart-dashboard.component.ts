import { Component, OnInit, Input } from '@angular/core';
import { Label, MultiDataSet } from 'ng2-charts';
import { ChartType, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-chart-dashboard',
  templateUrl: './chart-dashboard.component.html',
  styleUrls: ['./chart-dashboard.component.css']
})
export class ChartDashboardComponent implements OnInit {

  @Input() label: string;
  @Input() doughnutChartDataInput: number[];

  pieChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
    plugins: {
      datalabels: {
        formatter: (value, ctx) => {
          const label = ctx.chart.data.labels[ctx.dataIndex];
          return label;
        },
      },
    }
  };
  pieChartLabels: Label[] = [
    'Exp. cargado en sistema',
    'Exp. en revisión',
    'Exp. revisado sin observaciones',
    'Exp. revisado con observaciones',
    'Exp. revisado corregido'
  ];
  pieChartData: number[] = [0, 0, 0, 0, 0];
  pieChartType: ChartType = 'pie';
  pieChartLegend = true;
  pieChartColors = [
    {
      backgroundColor: [
        '#ff6f00',
        '#ffeb3b',
        '#1b5e20',
        '#dc3546',
        '#27a844'
      ],
    },
  ];

  constructor() {}

  ngOnInit(): void {
  }

  ngOnChanges(): void {
    this.pieChartData = [...this.doughnutChartDataInput];
  }

}
