import { Component } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PbrService } from 'src/app/services/pbr.service';
import Swal from 'sweetalert2';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-editar-expediente-obras',
  templateUrl: './editar-expediente-obras.component.html',
  styleUrls: ['./editar-expediente-obras.component.css']
})
export class EditarExpedienteObrasComponent {

  idExpediente: string;
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

  constructor(private app: AppComponent, private shared: SharedService, private route: Router, private toastr: ToastrService, private pbrService: PbrService, private router: ActivatedRoute, private pipeDate: DatePipe) {
    if(this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.app.setTitle("Actualizar Expediente - Obras");
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

  prepareBreadcrumb() {
    this.shared.activePage.emit("Actualizar Expediente Obras");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" }
    ]);
  }

  verifyParamsRoute() {
    this.router.params.subscribe(params => {
      this.idExpediente = params['id'];
      this.getInfo();
    });
  }

  getInfo() {

    this.pbrService.getDocumentsObras(this.idExpediente)
        .subscribe((data:any) => {

          //console.log(this.pipeDate.transform(data.fechaProcedimiento, 'yyyy-MM-ddT00:00:00'));

          this.procedimiento = data.procedimiento;
          this.programa = data.programa;
          this.ejecutor = data.ejecutor;
          this.localidad = data.localidad;
          this.nombreObra = data.nombreObra;
          this.proyecto = data.proyecto;
          this.numeroObra = data.numeroObra;
          this.responsable = data.responsable;
          this.modalidad = data.modalidad;
          this.tipoAdjudicacion = data.tipoAdjudicacion;
          this.tipoContrato = data.tipoContrato;
          this.tipoProveedor = data.tipoProveedor;
          this.proveedor = data.proveedor;
          this.numeroProcedimiento = data.numeroProcedimiento;
          this.fechaProcedimiento = data.fechaProcedimiento;
          this.numeroContrato = data.numeroContrato;
          this.fechaContrato = data.fechaContrato;
          this.montoAsignado = data.montoAsignado;
          this.montoContrato = data.montoContrato.split(",").join("");
          this.montoEjercicio = data.montoEjercido;

          this.app.loading = false;
        }, error => {
          this.app.auth = false;
          this.shared.clearSharedSession();
        });
  }

  actualizarExpediente() {
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
            'NumeroObra': this.numeroObra,
            'Latitud': "",
            'Longitud': "",
            'Ejercicio': this.ejercicio
          }

          this.pbrService.setUpdateExpedienteObra(this.idExpediente, body)
            .subscribe(data => {

              if (data['success']) {

                Swal.fire(
                  'Exito!',
                  'Expediente actualizado exitosamente!',
                  'success'
                );

                this.route.navigate(['/obras/expediente', this.idExpediente]);

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
