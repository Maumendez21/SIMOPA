import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PbrService } from 'src/app/services/pbr.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-nuevo-expediente-adquisiciones',
  templateUrl: './nuevo-expediente-adquisiciones.component.html',
  styleUrls: ['./nuevo-expediente-adquisiciones.component.css']
})
export class NuevoExpedienteAdquisicionesComponent {

  tipoAdjudicacion: string;
  numeroAdjudicacion: string;
  numeroContrato: string;
  entidad: string;
  objetoContrato: string;
  proveedorAdjudicado: string;
  montoAdjudicacion: number;
  origenRecurso: string;
  observacionesGenerales: string;
  ejercicio: number;

  constructor(private app: AppComponent, private pbrService: PbrService, private shared: SharedService, private route: Router, private toastr: ToastrService) {
    if(this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(true);
      this.app.setTitle("Nuevo Expediente - Adquisiciones");
      this.prepareBreadcrumb();
      this.prepareEjercicio();
      this.app.loading = false;
    } else {
      this.route.navigate(['/login']);
    }
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Nuevo Expediente Adquisiciones");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  prepareEjercicio() {
    this.shared.ejercicioStream$.subscribe(data => {
      console.log(data);
      this.ejercicio = data;
    });
  }

  registrarExpediente() {
    if (this.verifyEmpty(this.tipoAdjudicacion) &&
        this.verifyEmpty(this.numeroAdjudicacion) &&
        this.verifyEmpty(this.numeroContrato) &&
        this.verifyEmpty(this.entidad) &&
        this.verifyEmpty(this.objetoContrato) &&
        this.verifyEmpty(this.proveedorAdjudicado) &&
        this.verifyEmpty(this.origenRecurso) &&
        this.montoAdjudicacion > 0) {

          const body = {
            'TipoAdjudicacion': this.tipoAdjudicacion,
            'NumeroAdjudicacion': this.numeroAdjudicacion,
            'NumeroContrato ': this.numeroContrato,
            'Entidad': this.entidad,
            'Objeto': this.objetoContrato,
            'Proveedor': this.proveedorAdjudicado,
            'MontoAdjudicacion': this.montoAdjudicacion,
            'OrigenRecurso': this.origenRecurso,
            'ObservacionesGenerales': this.observacionesGenerales,
            'Ejercicio': this.ejercicio
          }
          console.log(body);
          
          this.pbrService.setNuevoExpedienteAdquisiciones(body)
            .subscribe(data => {
              if (data['messages'][0] === "Registro duplicado") {

                Swal.fire(
                  'Registro duplicado!',
                  'El expediente se encuentra duplicado',
                  'error'
                );

              } else {
                Swal.fire(
                  'Exito!',
                  'Expediente registrado exitosamente!',
                  'success'
                );

                this.route.navigate(['/adquisiciones/detalle']);
              }
            });

        } else {
          this.toastr.info("Es necesario llenar todos los campos con asterisco", "Campos Vacios");
        }
  }

  verifyEmpty(input: string) {
    return input != null && input != "";
  }

}
