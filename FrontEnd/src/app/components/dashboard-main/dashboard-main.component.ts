import { Component } from '@angular/core';
import { ChartOptions, ChartDataSets } from 'chart.js';
import { SharedService } from 'src/app/services/shared.service';
import { AppComponent } from 'src/app/app.component';
import { Router } from '@angular/router';
import { PbrService } from 'src/app/services/pbr.service';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-dashboard-main',
  templateUrl: './dashboard-main.component.html',
  styleUrls: ['./dashboard-main.component.css']
})
export class DashboardMainComponent {

  data: any;

  titleAdquisicion: string = "Adquisiciones";
  titleObra: string = "Obra Pública";

  barChartOptionsAdquisicion: (ChartOptions & { annotation: any });
  barChartOptionsObra: (ChartOptions & { annotation: any })
  barChartDataAdquisiciones: ChartDataSets[];
  barChartDataObra: ChartDataSets[];

  infoGlobalAdquisiciones: any;
  infoGlobalObras: any;
  infoGlobalObrasEjercido: any;
  infoPieAdquisiciones: any;
  infoPieObras: any;
  infoLine: any;
  infoLineAdquisiciones: any;
  infoLineObras: any;

  needleValue = 0;

  constructor(private app: AppComponent, private router: Router, private shared: SharedService, private pbrService: PbrService, private pipeCurrency: CurrencyPipe) {
    if (this.app.auth) {

      this.app.setTitle("Dashboard");
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(true);
      this.prepareBreadcrumb();
      this.getData();
    } else {
      this.router.navigate(['/login']);
    }
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Dashboard Expedientes");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  getData() {

    this.pbrService.getChartsDashboardMain()
      .subscribe(response => {
        console.log(response);

        this.infoGlobalAdquisiciones = response['InformationGlobalAdquisiciones'];
        this.infoGlobalObras = response['InformationGlobalObras'];
        this.infoGlobalObrasEjercido = response['InformationGlobalObrasEjercido'];
        this.infoPieAdquisiciones = response['InformationCakeAdjudicaciones'][0];
        this.infoPieObras = response['InformationCakeObras'][0];

        this.infoLine = {
          'Adjudicaciones': response['InformationAdjudicaciones'][0],
          'Obras': response['InformationObras'][0]
        };

        this.infoLineAdquisiciones = response['InformationAdjudicaciones'][0];
        this.infoLineObras = response['InformationObras'][0];
      });

    this.shared.ejercicioStream$.subscribe(data => {
      this.pbrService.getDashboardData(data)
        .subscribe(response => {

          console.log(response);

          this.data = response;

          this.needleValue = response.PorcentajeContratadoPresupuestoAutorizado;

          this.barChartOptionsAdquisicion = {
            responsive: true,
            legend: { position: 'bottom' },
            scales: { 
              xAxes: [{
                id: 'x-axis-0'
              }], 
              yAxes: [
                {
                  ticks: {
                    beginAtZero: true,
                    max: (response.PresupuestoAutorizadoAdquisiciones + ( response.PresupuestoAutorizadoAdquisiciones / 4 ) ),
                    display: false,          
                  },
                  scaleLabel: {
                    display: true,
                    labelString: 'MONEDA NACIONAL MXN',
                  }
                }
              ] 
            },
            annotation: {
              annotations: [
                {
                  type: 'line',
                  mode: 'horizontal',
                  scaleID: 'y-axis-0',
                  value: response.PresupuestoAutorizadoAdquisiciones,
                  borderColor: 'red',
                  borderWidth: 2,
                  label: {
                    enabled: true,
                    fontColor: 'red',
                    content: "$"+this.pipeCurrency.transform(response.PresupuestoAutorizadoAdquisiciones, 'MXN'),
                    position: 'right'
                  }
                },
              ],
            },
          };

          this.barChartOptionsObra = {
            responsive: true,
            legend: { position: 'bottom' },
            scales: { 
              xAxes: [{
                id: 'x-axis-0'
              }], 
              yAxes: [
                {
                  ticks: {
                    beginAtZero: true,
                    max: (response.PresupuestoAutorizadoObraPublica + (response.PresupuestoAutorizadoObraPublica / 4) ),
                    display: false,          
                  },
                  scaleLabel: {
                    display: true,
                    labelString: 'MONEDA NACIONAL MXN',
                  }
                }
              ] 
            },
            annotation: {
              annotations: [
                {
                  type: 'line',
                  mode: 'horizontal',
                  scaleID: 'y-axis-0',
                  value: response.PresupuestoAutorizadoObraPublica,
                  borderColor: 'red',
                  borderWidth: 2,
                  label: {
                    enabled: true,
                    fontColor: 'red',
                    content: "$"+this.pipeCurrency.transform(response.PresupuestoAutorizadoObraPublica, 'MXN'),
                    position: 'right'
                  }
                },
              ],
            },
          };
          
          this.barChartDataAdquisiciones = [
            { data: [response.MontoAdquisicionesContratadas], label: 'Contratado' },
            { data: [response.MontoAdquisicionesPagadas], label: 'Pagado' }
          ];

          this.barChartDataObra = [
            { data: [response.MontoObrasContratadas], label: 'Contratado' },
            { data: [response.MontoObrasPagadas], label: 'Pagado' },
          ];
        }, error => {
          this.app.auth = false;
          this.shared.clearSharedSession();
          this.router.navigate(["/login"]);
        });

        this.app.loading = false;
    });
  }

}