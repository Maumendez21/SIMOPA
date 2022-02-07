import { Component } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { Router } from '@angular/router';
import { SharedService } from 'src/app/services/shared.service';
import { MultiDataSet, Label } from 'ng2-charts';
import { PbrService } from 'src/app/services/pbr.service';
import { ChartDataSets } from 'chart.js';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {

  dataCards: any;

  cards: any = [
    {
      style: {
        backgroundColor: '#00796b',
        borderColor: '#00796b',
        color: 'white'
      },
      text: 'Expedientes cargados'
    },
    {
      style: {
        backgroundColor: '#0097a7',
        borderColor: '#0097a7',
        color: 'white'
      },
      text: 'En revisión'
    },
    {
      style: {
        backgroundColor: '#0097a7',
        borderColor: '#0097a7',
        color: 'white'
      },
      text: 'Revisado con observaciones'
    },
    {
      style: {
        backgroundColor: '#0097a7',
        borderColor: '#0097a7',
        color: 'white'
      },
      text: 'En solventación'
    },
    {
      style: {
        backgroundColor: '#0097a7',
        borderColor: '#0097a7',
        color: 'white'
      },
      text: 'Revisado sin observaciones / corregido'
    },
    {
      style: {
        backgroundColor: '#00796b',
        borderColor: '#00796b',
        color: 'white'
      },
      text: 'Promedio de Integración documental'
    },
  ];

  doughnutChartData: number[];

  labelsBar: Label[] = [''];
  dataBar: ChartDataSets[] = [
    { data: [0], label: 'Montos' }
  ];

  labelsBarDoc: Label[] = [''];
  dataBarDoc: ChartDataSets[] = [
    { data: [0], label: 'Expedientes' }
  ];

  doughnutLabels: Label[] = [''];
  doughnutData: MultiDataSet = [[0]];
  doughnutColors = [
    { 
      backgroundColor: [
        '#2196f3', '#4caf50', '#ffeb3b', '#f44336', '#9c27b0'
      ],
    },
  ];

  ejercicio: number;
  auditor: string;
  data: any;
  expedientes: any;

  constructor(private app: AppComponent, private router: Router, private shared: SharedService, private pbrService: PbrService) {
    if (this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(true);
      this.shared.broadcastShowSelectEjercicio(true);
      this.app.setTitle("Dashboard - Adquisiciones");
      this.prepareBreadcrumb();
      this.prepareAuditorEjercicio();
      this.loadData();
      this.loadDataCharts();
    } else {
      this.router.navigate(['/login']);
    }
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Dashboard Adquisiciones");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  loadData() {

    this.shared.updateStream$.subscribe(data => {
      this.pbrService.getDashboardExpedientes("adquisiciones", this.ejercicio, this.auditor)
        .subscribe(response => {

          this.dataCards = [
            response.TotalExpedientes,
            response.EnRevision,
            response.RevisadoConObservaciones,
            response.Solventacion,
            response.RevisadaSinObservaciones,
            response.Porcentaje+'%'
          ];

          this.data = response;
          this.expedientes = response['ListaExpedientesEnRevision'];
          this.doughnutChartData = [
            response.Cargados, 
            response.EnRevision, 
            response.RevisadaSinObservaciones, 
            response.RevisadoConObservaciones, 
            response.RevisadoCorregido
          ];

          this.app.loading = false;
        });
    });
  }

  loadDataCharts() {
    this.shared.updateStream$.subscribe(data => {
      this.pbrService.getChartsDashboard("adquisiciones", this.ejercicio, this.auditor)
        .subscribe(response => {

          this.doughnutData = [response.GraficaCake.data];
          this.doughnutLabels = response.GraficaCake.label;

          this.labelsBar = response.GraficaBarras.label;
          this.dataBar = [
            { data: response.GraficaBarras.data, label: 'Montos' }
          ];

          this.labelsBarDoc = response.GraficaIntegreacion.label;
          this.dataBarDoc = [
            { data: response.GraficaIntegreacion.data, label: 'Expedientes' }
          ];
        });
    });
  }

  prepareAuditorEjercicio() {

    this.shared.ejercicioStream$.subscribe(data => {
      this.ejercicio = data;
      this.shared.broadcastUpdateStream(false);
    });

    this.shared.auditorStream$.subscribe(data => {
      this.auditor = data;
      this.shared.broadcastUpdateStream(false);
    });
  }
}
