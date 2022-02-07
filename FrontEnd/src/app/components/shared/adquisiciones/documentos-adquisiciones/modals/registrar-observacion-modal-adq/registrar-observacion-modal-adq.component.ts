import { Component, OnInit, Input } from '@angular/core';
import { PbrService } from 'src/app/services/pbr.service';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registrar-observacion-modal-adq',
  templateUrl: './registrar-observacion-modal-adq.component.html',
  styleUrls: ['./registrar-observacion-modal-adq.component.css']
})
export class RegistrarObservacionModalAdqComponent implements OnInit {

  @Input() idExpediente: number;
  @Input() observacionGeneral: string;
  @Input() info: any[] = [];

  constructor(private pbrService: PbrService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  registrarObservacionGeneral() {
    if (this.observacionGeneral) {
      const data = {
        'observacionGeneral': this.observacionGeneral
      }

      this.pbrService.setObservacionGeneral(this.idExpediente, "adquisiciones", data)
        .subscribe(data => {

          this.info['ObservacionesGenerales'] = this.observacionGeneral;

          Swal.fire(
            'Exito!',
            'Observación registrada exitosamente!',
            'success'
          );
        });

    } else {
      this.toastr.error("Es necesario llenar los campos", "Error");
    }
  }

}
