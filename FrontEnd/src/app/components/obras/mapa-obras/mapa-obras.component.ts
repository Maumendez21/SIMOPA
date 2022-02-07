import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { PbrService } from 'src/app/services/pbr.service';

@Component({
  selector: 'app-mapa-obras',
  templateUrl: './mapa-obras.component.html',
  styleUrls: ['./mapa-obras.component.css']
})
export class MapaObrasComponent implements OnInit {

  centerCords = {
    lat: 18.967548,
    lng: -97.901011
  };

  iconBase:string = "assets/global/images/pins/";

  icons = {
    'ALTA EN SISTEMA': { 'icon': this.iconBase + 'marker_azul.png' },
    'CARGA EN PROCESO': { 'icon': this.iconBase + 'marker_gris.png' },
    'CARGADO': { 'icon': this.iconBase + 'marker_azul_cargado.png' },
    'EN REVISION': { 'icon': this.iconBase + 'marker_amarillo.png' },
    'EN SOLVENTACION': { 'icon': this.iconBase + 'marker_morado.png' },
    'REVISADO CON OBSERVACIONES': { 'icon': this.iconBase + 'marker_rojo.png' },
    'REVISADO SIN OBSERVACIONES CORREGIDO': { 'icon': this.iconBase + 'marker_verde.png' }
  };

  markersOnMap: any;

  infoWindowOpened = null
  previous_info_window = null

  constructor(private app: AppComponent, private shared: SharedService, private pbr: PbrService) {
    this.app.setTitle("Map View - Obras Publicas");
    this.shared.broadcastShowSelectAuditores(false);
    this.shared.broadcastShowSelectEjercicio(true);
    this.prepareBreadcrumb();
    this.getCoordenadas();
  }

  ngOnInit(): void {
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Mapa de Obras");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  close_window() {
    if (this.previous_info_window != null ) {
      this.previous_info_window.close();
    }
  }    

  closeOtherInfo(infoWindow) {
    if (this.previous_info_window == null) {
      this.previous_info_window = infoWindow;
    } else{
      this.infoWindowOpened = infoWindow
      this.previous_info_window.close()
    }
      this.previous_info_window = infoWindow
  }

  getCoordenadas() {

    this.shared.ejercicioStream$.subscribe(data => {
      this.pbr.getObras(data, "")
        .subscribe(data => {
          console.log(data);
          this.markersOnMap = data;
        });
    });
  }
}
