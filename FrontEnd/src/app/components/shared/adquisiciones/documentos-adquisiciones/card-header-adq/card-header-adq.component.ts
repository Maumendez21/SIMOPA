import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PbrService } from 'src/app/services/pbr.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-card-header-adq',
  templateUrl: './card-header-adq.component.html',
  styleUrls: ['./card-header-adq.component.css']
})
export class CardHeaderAdqComponent implements OnInit {

  rol: string = localStorage.getItem("role");

  @Input() idExpediente: number;
  @Input() estatusColor: string;
  @Input() estatus: string;
  @Input() info: any[] = [];
  @Input() cargarAnexos: any;

  constructor(private pbrService: PbrService) {}

  ngOnInit(): void {
  }

  actualizarEstatus(dot) {

    this.estatus = dot;

    const data = {
      'estatusExpediente': dot
    }

    this.verificarEstatusColor();

    this.pbrService.setEstatusExpediente("adquisiciones", this.idExpediente, data)
      .subscribe(data => {
        if (data['success']) {
          Swal.fire(
            'Exito!',
            'Estatus actualizado exitosamente!',
            'success'
          );
        } else {
          Swal.fire(
            'Error',
            'Al parecer hubo un error al tratar de agregar la actualizar el estatus',
            'error'
          );
        }
      });
  }

  descargarObservacionesRecomendaciones(id: number) {
    return `https://apipbrdevelolop.azurewebsites.net/api/reporte/A/cedula/observaciones/recomendaciones/${id}`;
  }

  verificarEstatusColor() {
    console.log(this.estatus);
    switch (this.estatus) {
      case "ALTA EN SISTEMA":
        this.estatusColor = "blue";
        break;
      case "CARGA EN PROCESO":
        this.estatusColor = "gray";
        break;
      case "CARGADO":
        this.estatusColor = "blue-cargado";
        break;
      case "EN REVISION":
        this.estatusColor = "yellow";
        break;
      case "EN SOLVENTACION":
        this.estatusColor = "purple";
        break;
      case "REVISADO CON OBSERVACIONES":
        this.estatusColor = "danger";
        break;
      default:
        this.estatusColor = "success";
    }
  }

  asignarmeExpediente() {
    Swal.fire({
      title: '¿Seguro que quieres reasignarte el Expediente?',
      text: "Esta acción no se puede revertir",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Si, estoy seguro',
      cancelButtonText: 'Cancelar',
    }).then((result) => {
      if (result.value) {
        this.pbrService.setAsignarAuditorBitacora("Adquisiciones", this.idExpediente.toString())
          .subscribe(response => {
            if (response["success"]) {

              Swal.fire(
                'Exito!',
                'Se reasigno exitosamente el Expediente.',
                'success'
              );

              this.info["ExpedienteOwner"] = !this.info["ExpedienteOwner"];
              this.info["NombreAuditor"] = localStorage.getItem("name");

            } else {

              Swal.fire(
                'Error!',
                'Al parecer hubo un error al tratar de reasignar el Expediente',
                'error'
              );
            }
          });
      }
    });
  }

}
