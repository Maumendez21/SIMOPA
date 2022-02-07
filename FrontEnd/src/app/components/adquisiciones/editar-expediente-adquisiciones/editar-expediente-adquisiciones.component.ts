import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PbrService } from 'src/app/services/pbr.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-editar-expediente-adquisiciones',
  templateUrl: './editar-expediente-adquisiciones.component.html',
  styleUrls: ['./editar-expediente-adquisiciones.component.css']
})
export class EditarExpedienteAdquisicionesComponent {

  idExpediente: number;
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

  constructor(private app: AppComponent, private pbrService: PbrService, private shared: SharedService, private route: Router, private toastr: ToastrService, private router: ActivatedRoute) {
    if(this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.app.setTitle("Actualizar Expediente - Adquisiciones");
      this.prepareBreadcrumb();
      this.verifyParamsRoute();
      this.prepareEjercicio();
      this.app.loading = false;
    } else {
      this.route.navigate(['/login']);
    }
  }

  prepareEjercicio() {
    this.shared.ejercicioStream$.subscribe(data => {
      this.ejercicio = data;
    });
  }

  verifyParamsRoute() {
    this.router.params.subscribe(params => {
      this.idExpediente = params['id'];
      this.getInfo();
    });
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Actualizar Expediente Adquisiciones");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  getInfo() {

    this.pbrService.getDocumentsAdjudicacion(this.idExpediente)
        .subscribe((data:any) => {
          console.log(data);
          this.tipoAdjudicacion = data.tipoAdjudicacion;
          this.numeroAdjudicacion = data.numeroAdjudicacion;
          this.numeroContrato = data.numeroContrato;
          this.entidad = data.entidad;
          this.objetoContrato = data.objetoContrato;
          this.proveedorAdjudicado = data.proveedorAdjudicado;
          this.montoAdjudicacion = data.montoAdjudicado;
          this.origenRecurso = data.origenRecurso;
          this.observacionesGenerales = data.ObservacionesGenerales;

        }, error => {
          this.app.auth = false;
          this.shared.clearSharedSession();
        });
  }

  actualizarExpediente() {
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
            'NumeroContrato': this.numeroContrato,
            'Entidad': this.entidad,
            'Objeto': this.objetoContrato,
            'Proveedor': this.proveedorAdjudicado,
            'MontoAdjudicacion': this.montoAdjudicacion,
            'OrigenRecurso': this.origenRecurso,
            'ObservacionesGenerales': this.observacionesGenerales,
            'Ejercicio': this.ejercicio
          }

          this.pbrService.setUpdateExpedienteAdquisiciones(this.idExpediente, body)
            .subscribe(data => {
              if (data['success']) {

                Swal.fire(
                  'Exito!',
                  'Expediente actualizado exitosamente!',
                  'success'
                );

                this.route.navigate(['/adquisiciones/expediente', this.idExpediente]);

              } else {
                Swal.fire(
                  'Error al Actualizar!',
                  'Parece ser que hubo un error al actualizar el expediente, intentalo nuevamente',
                  'error'
                );
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
