import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { PbrService } from 'src/app/services/pbr.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-anexos-modal-adq',
  templateUrl: './anexos-modal-adq.component.html',
  styleUrls: ['./anexos-modal-adq.component.css']
})
export class AnexosModalAdqComponent implements OnInit {

  @ViewChild('fileAnexo')
  fileAnexo: ElementRef;

  @Input() idExpediente: number;
  @Input() anexos: any;
  @Input() cargarAnexos: any;

  tituloAnexo: string;
  descripcionAnexo: string;
  fileAnexoForm: any;

  constructor(private pbrService: PbrService) { }

  ngOnInit(): void {
  }

  onFileChangedAnexo(event) {
    this.fileAnexoForm = event.target.files[0];
  }

  cargarAnexo() {

    if (this.tituloAnexo != "" && this.descripcionAnexo != "") {
      let formData = new FormData();

      formData.append('idExpediente', this.idExpediente.toString());
      formData.append('titulo', this.tituloAnexo);
      formData.append('descripcion', this.descripcionAnexo);
      formData.append('File', this.fileAnexoForm, this.fileAnexoForm.name);

      this.pbrService.setCargarAnexo("adquisiciones", formData)
        .subscribe(response => {

          this.cargarAnexos();

          if (response['success']) {

            Swal.fire(
              'Carga exitosa!',
              'El archivo se ha subido correctamente.',
              'success'
            );
            
            this.tituloAnexo = "";
            this.descripcionAnexo = "";
            this.fileAnexo.nativeElement.value = "";
          }
        });
    }
  }

  eliminarAnexo(id: string) {
    console.log(id);

    Swal.fire({
      title: '¿Seguro que quieres eliminar el Anexo?',
      text: "Esta acción no se puede revertir",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Si, estoy seguro',
      cancelButtonText: 'Cancelar',
    }).then((result) => {
      if (result.value) {
        this.pbrService.deleteAnexo(id)
          .subscribe(data => {
            console.log(data);

            this.cargarAnexos();

            Swal.fire(
              'Eliminado exitosamente!',
              'El anexo se ha eliminado correctamente.',
              'success'
            );
          });
      }
    });
  }

}
