import { Component } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PbrService } from 'src/app/services/pbr.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-nuevo-expediente-obras',
  templateUrl: './nuevo-expediente-obras.component.html',
  styleUrls: ['./nuevo-expediente-obras.component.css']
})
export class NuevoExpedienteObrasComponent {

  procedimiento: string;
  programa: string;
  ejecutor: string;
  localidad: string;
  nombreObra: string;
  proyecto: string;
  numeroObra: string;
  responsable: string;
  modalidad: string;
  tipoAdjudicacion: string;
  tipoContrato: string;
  tipoProveedor: string;
  proveedor: string;
  numeroProcedimiento: string;
  fechaProcedimiento: string;
  numeroContrato: string;
  fechaContrato: string;
  montoAsignado: number;
  montoContrato: number;
  montoEjercicio: number;
  ejercicio: number;

  constructor(private app: AppComponent, private shared: SharedService, private route: Router, private toastr: ToastrService, private pbrService: PbrService) {
    if(this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(true);
      this.app.setTitle("Nuevo Expediente - Obras");
      this.prepareBreadcrumb();
      this.prepareEjercicio();
      this.app.loading = false;
    } else {
      this.route.navigate(['/login']);
    }
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Nuevo Expediente Obras");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  prepareEjercicio() {
    this.shared.ejercicioStream$.subscribe(data => {
      this.ejercicio = data;
    });
  }

  registrarExpediente() {
    if (this.verifyEmpty(this.nombreObra) &&
        this.verifyEmpty(this.procedimiento) &&
        this.verifyEmpty(this.ejecutor) &&
        this.verifyEmpty(this.localidad) &&
        this.verifyEmpty(this.numeroObra) &&
        this.verifyEmpty(this.modalidad) &&
        this.verifyEmpty(this.tipoAdjudicacion) &&
        this.verifyEmpty(this.tipoContrato) &&
        this.verifyEmpty(this.numeroProcedimiento) &&
        this.verifyEmpty(this.numeroContrato) &&
        this.montoContrato > 0) {

          const body = {
            'TipoAdjudicacion': this.tipoAdjudicacion,
            'NumeroProcedimiento': this.numeroProcedimiento,
            'NumeroContrato': this.numeroContrato,
            'Procedimiento': this.procedimiento,
            'NombreObra': this.nombreObra,
            'Proveedor': this.proveedor,
            'Programa': this.programa,
            'Ejecutor': this.ejecutor,
            'Localidad': this.localidad,
            'Proyecto': this.proyecto,
            'Responsable': this.responsable,
            'Modalidad': this.modalidad,
            'tipoContrato': this.tipoContrato,
            'tipoProveedor': this.tipoProveedor,
            'FechaProcedimiento': this.fechaProcedimiento,
            'FechaContrato': this.fechaContrato,
            'MontoAsignado': this.montoAsignado,
            'MontoContrato': this.montoContrato,
            'MontoEjercido': this.montoEjercicio,
            'numeroObra': this.numeroObra,
            'Latitud': "",
            'Longitud': "",
            'Ejercicio': this.ejercicio
          }

          this.pbrService.setNuevoExpedienteObra(body)
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

                this.route.navigate(['/obras/detalle']);
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
