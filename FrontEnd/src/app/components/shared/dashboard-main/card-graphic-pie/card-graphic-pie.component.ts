import { Component, OnInit, Input } from '@angular/core';
import { ChartType, ChartOptions } from 'chart.js';
import { Label } from 'ng2-charts';

@Component({
  selector: 'app-card-graphic-pie',
  templateUrl: './card-graphic-pie.component.html',
  styleUrls: ['./card-graphic-pie.component.css']
})
export class CardGraphicPieComponent implements OnInit {

  @Input() title: string;
  @Input() info: any;

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
  
  pieChartLabels: Label[] = [];
  pieChartData: number[] = [];
  pieChartType: ChartType = 'pie';
  pieChartLegend = true;
  pieChartColors = [
    {
      backgroundColor: ['#64b5f6', '#66bb6a', '#ffee58'],
    },
  ];

  constructor() { }

  ngOnInit() {}

  ngOnChanges() {
    this.pieChartLabels = this.info['Anios'];
    this.pieChartData = this.info['Expedientes'];
  }

}
