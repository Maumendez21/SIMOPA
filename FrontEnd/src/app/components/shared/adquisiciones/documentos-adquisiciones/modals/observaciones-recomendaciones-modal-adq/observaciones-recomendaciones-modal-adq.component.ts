import { Component, OnInit, Input } from '@angular/core';
import { PbrService } from 'src/app/services/pbr.service';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-observaciones-recomendaciones-modal-adq',
  templateUrl: './observaciones-recomendaciones-modal-adq.component.html',
  styleUrls: ['./observaciones-recomendaciones-modal-adq.component.css']
})
export class ObservacionesRecomendacionesModalAdqComponent implements OnInit {

  @Input() idExpediente: number;
  @Input() claveExpediente: number;
  @Input() documentos: any;
  @Input() indexDocument: number;
  @Input() observaciones: any;

  @Input() getObservaciones: any;
  
  idObservacion: number;
  nuevoComentario: string;
  nuevaRecomendacion: string;
  edicionComentarioRecomendacion: boolean = false;
  estatusComentarioRecomendacion: number = 1;

  disabledBtnObservacionesRecomendaciones: boolean = false;

  constructor(private pbrService: PbrService, private app: AppComponent, private shared: SharedService, private router: Router) { }

  ngOnInit(): void {
  }

  setDataUpdateComentarios(data: any) {
    this.changeEstatusUpdateComentarios(true);
    this.idObservacion = data.Id;
    this.nuevoComentario = data.Observacion;
    this.nuevaRecomendacion = data.Recomendacion;
    this.estatusComentarioRecomendacion = data.Estatus;
  }

  changeEstatusUpdateComentarios(estatus: boolean) {
    this.edicionComentarioRecomendacion = estatus;
  }

  actualizarObservacion() {

    this.disabledBtnObservacionesRecomendaciones = true;

    const data = {
      'id': this.idObservacion,
      'idExpediente': this.idExpediente,
      'TipoExpediente': 'Adquisiciones',
      'Clave': this.claveExpediente,
      'Observacion': this.nuevoComentario,
      'Recomendacion': this.nuevaRecomendacion,
      'Estatus': this.estatusComentarioRecomendacion
    };

    this.pbrService.updateObservacionesExpediente("adquisiciones", this.idExpediente, this.claveExpediente, data)
      .subscribe(response => {

        this.documentos[this.indexDocument].comentario = this.calcularObservaciones();

        this.getObservaciones(this.claveExpediente.toString(), this.indexDocument);
        this.changeEstatusUpdateComentarios(false);
        this.nuevoComentario = "";
        this.nuevaRecomendacion = "";
        this.disabledBtnObservacionesRecomendaciones = false;
      }, error => {
        this.app.auth = false;
        this.shared.clearSharedSession();
        this.router.navigate(["/login"]);
      });
  }

  registrarObservacion() {

    if (this.nuevoComentario != "" && 
        this.nuevaRecomendacion != "" && 
        this.nuevoComentario != null && 
        this.nuevaRecomendacion != null) {

      this.disabledBtnObservacionesRecomendaciones = true;

      const data = {
        'id': '',
        'idExpediente': this.idExpediente,
        'TipoExpediente': 'Adquisiciones',
        'Clave': this.claveExpediente,
        'Observacion': this.nuevoComentario,
        'Recomendacion': this.nuevaRecomendacion,
        'Estatus': 2
      };

      this.pbrService.setObservacionesExpediente("adquisiciones", this.idExpediente, this.claveExpediente, data)
        .subscribe(response => {
          this.getObservaciones(this.claveExpediente.toString(), this.indexDocument);
          this.nuevoComentario = "";
          this.nuevaRecomendacion = "";
          this.documentos[this.indexDocument].comentario = this.newObservaciones();
          this.disabledBtnObservacionesRecomendaciones = false;
        }, error => {
          this.app.auth = false;
          this.shared.clearSharedSession();
          this.router.navigate(["/login"]);
        });
    }
  }

  newObservaciones() : string {
    let atentidas: number = 0;
    let noAtendidas: number = 0;
    let total: number = this.observaciones.length + 1;

    this.observaciones.map(function(x) {
      (x.Estatus == 1) ? atentidas = atentidas+1 : noAtendidas = noAtendidas+1;
    });

    return atentidas+"/"+total;
  }

  calcularObservaciones() : string {

    let conteo: number = 0;

    this.observaciones.map(function(x) {
      conteo = (x.Estatus == 1) ? conteo+1 : conteo;
    });

    conteo = (this.estatusComentarioRecomendacion == 1) ? conteo + 1 : conteo - 1;

    return conteo+"/"+this.observaciones.length;
  }

}
