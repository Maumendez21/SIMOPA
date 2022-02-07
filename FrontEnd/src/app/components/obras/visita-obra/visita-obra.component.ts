import { Component } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { AppComponent } from 'src/app/app.component';
import { PbrService } from 'src/app/services/pbr.service';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedService } from 'src/app/services/shared.service';
import { Subject } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-visita-obra',
  templateUrl: './visita-obra.component.html',
  styleUrls: ['./visita-obra.component.css']
})
export class VisitaObraComponent {

  idVisita: string;

  dtOptions: DataTables.Settings = {};
  dtTrigger = new Subject();

  numeroExpediente: string;
  info: any;
  lista: any;
  principalesPartidas: any;

  fechaVisita: string;
  porcentajeFisico: number;
  porcentajeFinanciero: number;
  situacionActual: string;
  problematica: string;

  fechaVisitaEdicion: string;
  porcentajeFisicoEdicion: number;
  porcentajeFinancieroEdicion: number;
  situacionActualEdicion: string;
  problematicaEdicion: string;

  tituloEvidencia: string;
  descripcionEvidencia: string;
  fileEvidencia: any;

  principalPartida: string;

  url: string = "https://www.colorquimica.com.co/wp-content/uploads/2017/09/test.pdf";


  constructor(private app: AppComponent, private toastr: ToastrService, private pbrService: PbrService, private http: HttpClient, private router: Router, private route: ActivatedRoute, private shared: SharedService, private datepipe: DatePipe) {
    if (this.app.auth) {
      this.app.loading = true;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.app.setTitle("Visita Obra - Obras");
      this.verifyParamsRoute();
      this.prepareBreadcrumb();
    } else {
      this.router.navigate(['/login']);
    }
  }

  verifyParamsRoute() {
    this.route.params.subscribe(params => {
      this.numeroExpediente = params['id'];
      this.getVisitas(params['id']);
      this.getPrincipalesPartidas();
    });
  }

  prepareBreadcrumb() {
    this.shared.activePage.emit("Visita de Obra");
    this.shared.parts.emit([
      { name: "Inicio", route: "#" },
      { name: "Detalle de Obras", route: '/obras/detalle' }
    ]);
  }

  getVisitas(expediente: string) {

    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 10,
      ordering: false,
      responsive: true,
      language: {
        'search': "Buscar",
        'paginate': {
          'first': 'Primero',
          'previous': 'Anterior',
          'next': 'Siguiente',
          'last': 'Ultimo'
        },
        'lengthMenu': 'Mostrar _MENU_ documentos',
        'info': 'Mostrando _PAGE_ de _PAGES_'
      }
    }

    this.pbrService.getVisitasObra(expediente)
      .subscribe(data => {
        console.log(data['ListaVisitaObra']);
        this.info = data['HeaderObraPublica'];
        this.lista = data['ListaVisitaObra'];
        this.app.loading = false;
      });
  }

  getPrincipalesPartidas() {
    this.pbrService.getPartidasPrincipales(this.numeroExpediente)
      .subscribe(data => {
        console.log(data);
        this.principalesPartidas = data;
      });
  }

  onFileChanged(event) {
    this.fileEvidencia = event.target.files[0];
  }

  registrarEvidenciaFotografica() {

    if (this.verifyEmpty(this.fileEvidencia) && this.verifyEmpty(this.tituloEvidencia) && this.verifyEmpty(this.descripcionEvidencia)) {

      let formData = new FormData();

      formData.append('ImageFile', this.fileEvidencia, this.fileEvidencia.name);
      formData.append('idExpediente', this.numeroExpediente);
      formData.append('idVisita', this.idVisita);
      formData.append('titulo', this.tituloEvidencia);
      formData.append('descripcion', this.descripcionEvidencia);
      
      this.pbrService.setFotografiasVisitaObra(formData)
        .subscribe(response => {
          this.getVisitas(this.numeroExpediente);
          this.toastr.success("Se registro exitosamente la evidencia fotografica", "Exito");
        });
    } else {
      this.toastr.info("Es necesario llenar todos los campos", "Campos Vacios");
    }
  }

  registrarVisitaObra() {

    if (this.verifyEmpty(this.fechaVisita) && 
    this.verifyEmpty(this.situacionActual) &&
    this.verifyEmpty(this.problematica) &&
        this.porcentajeFisico >= 0 &&
        this.porcentajeFinanciero >= 0) {

          const data = {
            'FechaVisita': this.datepipe.transform(this.fechaVisita, 'yyyy-MM-dd h:mm:ss'),
            'SituacionActual': this.situacionActual,
            'Problematica': this.problematica,
            'AvanceFisico': this.porcentajeFisico,
            'AvanceFinanciero': this.porcentajeFinanciero
          }
      
          this.pbrService.setNuevaVisitaObra(this.numeroExpediente, data)
            .subscribe(data => {
              this.getVisitas(this.numeroExpediente);
              if (data['success']) {
                Swal.fire(
                  'Exito!',
                  'Nueva visita agregada exitosamente!',
                  'success'
                );
              } else {
                Swal.fire(
                  'Error',
                  'Al parecer hubo un error al tratar de agregar la visita',
                  'error'
                );
              }
            });

        } else {
          this.toastr.info("Es necesario llenar todos los campos", "Campos Vacios");
        }
  }

  actualizarVisitaObra() {
    if (this.verifyEmpty(this.fechaVisitaEdicion) && 
        this.verifyEmpty(this.situacionActualEdicion) &&
        this.verifyEmpty(this.problematicaEdicion) &&
        this.porcentajeFisicoEdicion >= 0 &&
        this.porcentajeFinancieroEdicion >= 0) {

          const data = {
            'Id': this.idVisita,
            'FechaVisita': this.datepipe.transform(this.fechaVisitaEdicion, 'yyyy-MM-dd h:mm:ss'),
            'SituacionActual': this.situacionActualEdicion,
            'Problematica': this.problematicaEdicion,
            'AvanceFisico': this.porcentajeFisicoEdicion,
            'AvanceFinanciero': this.porcentajeFinancieroEdicion
          }

          console.log(data);
      
          this.pbrService.setUpdateVisitaObra(this.numeroExpediente, data)
            .subscribe(data => {
              console.log(data);
              this.getVisitas(this.numeroExpediente);
              if (data['success']) {
                Swal.fire(
                  'Exito!',
                  'Nueva visita agregada exitosamente!',
                  'success'
                );
              } else {
                Swal.fire(
                  'Error',
                  'Al parecer hubo un error al tratar de agregar la visita',
                  'error'
                );
              }
            });

        } else {
          this.toastr.info("Es necesario llenar todos los campos", "Campos Vacios");
        }
  }

  asignarDatosEdicion(idVisita: string, fecha: string, avanceFisico: string, avanceFinanciero: string, situacionActual: string, problematica: string) {
    this.idVisita = idVisita;
    this.fechaVisitaEdicion = this.datepipe.transform(fecha, 'yyyy-MM-ddTh:mm');
    this.porcentajeFisicoEdicion = +avanceFisico;
    this.porcentajeFinancieroEdicion = +avanceFinanciero;
    this.situacionActualEdicion = situacionActual;
    this.problematicaEdicion = problematica;
  }

  verMosaico(idVisita: string) {
    this.router.navigate(['/obra/evidencia-fotografica', idVisita, this.numeroExpediente]);
  }

  asignarIdVisita(idVisita: string) {
    this.idVisita = idVisita;
  }

  verifyEmpty(input: string) {
    return input != null && input != "";
  }

  registrarPrincipalPartida() {

    if (this.verifyEmpty(this.principalPartida)) {
      
      const data = {
        'idExpediente': this.numeroExpediente,
        'Nombre': this.principalPartida
      }

      this.pbrService.setNuevaPartidaPrincipal(data)
        .subscribe(data => {
          console.log(data);
          if (data['success']) {
            this.getPrincipalesPartidas();
            Swal.fire(
              'Exito!',
              'Nueva partida agregada exitosamente!',
              'success'
            );
          } else {
            Swal.fire(
              'Error',
              'Al parecer hubo un error al tratar de agregar la partida',
              'error'
            );
          }

          this.principalPartida = "";
        });

    } else {
      this.toastr.info("Es necesario llenar todos los campos", "Campos Vacios");
    }
  }

}
