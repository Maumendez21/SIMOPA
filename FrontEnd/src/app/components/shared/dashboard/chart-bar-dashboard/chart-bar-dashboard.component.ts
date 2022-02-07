import { Component, OnInit, Input } from '@angular/core';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';
import * as pluginDataLabels from 'chartjs-plugin-datalabels';

@Component({
  selector: 'app-chart-bar-dashboard',
  templateUrl: './chart-bar-dashboard.component.html',
  styleUrls: ['./chart-bar-dashboard.component.css']
})
export class ChartBarDashboardComponent implements OnInit {

  @Input() currency: boolean = false;
  @Input() title: string;
  @Input() labels: Label[];
  @Input() data: ChartDataSets[];

  barChartOptions: ChartOptions = {
    responsive: true,
    legend: { position: 'bottom' },
    scales: { xAxes: [{}], yAxes: [{}] },
    plugins: {
      datalabels: {
        anchor: 'end',
        align: 'end',
      }
    },
  };
  barChartLabels: Label[] = [''];
  barChartType: ChartType = 'bar';
  barChartLegend = true;

  barChartData: ChartDataSets[] = [
    { data: [0], label: '' }
  ];

  chartColors = [{ backgroundColor: ['#d32f2f', '#d32f2f', '#d32f2f', '#d32f2f', '#d32f2f', '#fbc02d', '#fbc02d', '#fbc02d', '#fbc02d', '#2e7d32'] }]

  constructor() { }

  ngOnInit(): void {
  }

  ngOnChanges(): void {
    this.barChartLabels = this.labels;
    this.barChartData = this.data;

    if (this.currency) {
      this.barChartOptions.tooltips = {
        callbacks: {
          label: function(t, d) {
             var xLabel = d.datasets[t.datasetIndex].label;
             var yLabel = t.yLabel >= 1000 ? '$' + t.yLabel.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") : '$' + t.yLabel;
             return xLabel + ': ' + yLabel;
          }
        }
      };
      this.barChartOptions.scales = {
        yAxes: [{
          ticks: {
              callback: function(value, index, values) {
                if (value >= 1000) {
                    return '$' + value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                } else {
                    return '$' + value;
                }
              }
          }
        }]
      };
    }
  }

}
