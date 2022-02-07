import { Component, OnInit, Input } from '@angular/core';
import { Label, MultiDataSet } from 'ng2-charts';
import { ChartType, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-chart-doughnut-dashboard',
  templateUrl: './chart-doughnut-dashboard.component.html',
  styleUrls: ['./chart-doughnut-dashboard.component.css']
})
export class ChartDoughnutDashboardComponent implements OnInit {

  @Input() title: string;
  @Input() labels: Label[];
  @Input() data: MultiDataSet;
  @Input() colors: any;

  doughnutChartLabels: Label[] = [''];
  doughnutChartData: MultiDataSet = [
    [0]
  ];
  doughnutChartType: ChartType = 'doughnut';
  doughnutChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
    },
  };
  doughnutChartColors = [
    {
      backgroundColor: [
        '#ff6f00'
      ],
    },
  ];

  constructor() { }

  ngOnInit(): void {
  }

  ngOnChanges(): void {
    this.doughnutChartLabels = this.labels;
    this.doughnutChartData = this.data;
    this.doughnutChartColors = this.colors;
  }

}
