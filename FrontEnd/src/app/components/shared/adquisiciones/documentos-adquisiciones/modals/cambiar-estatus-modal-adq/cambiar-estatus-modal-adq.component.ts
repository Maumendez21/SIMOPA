import { Component, OnInit, Input } from '@angular/core';
import { PbrService } from 'src/app/services/pbr.service';

@Component({
  selector: 'app-cambiar-estatus-modal-adq',
  templateUrl: './cambiar-estatus-modal-adq.component.html',
  styleUrls: ['./cambiar-estatus-modal-adq.component.css']
})
export class CambiarEstatusModalAdqComponent implements OnInit {

  @Input() idExpediente: number;
  @Input() claveExpediente: number;
  @Input() documentos: any;
  @Input() indexDocument: number;

  estatusChangeDocument: string = "N/A";
  role: string = localStorage.getItem("role");

  constructor(private pbrService: PbrService) { }

  ngOnInit(): void {
  }

  cambiarEstatusDocumento() {
    const data = {
      "estatusDocumento": this.estatusChangeDocument
    };

    console.log([data, this.idExpediente, this.claveExpediente]);

    this.pbrService.setUpdateEstatus("adquisiciones", this.idExpediente, this.claveExpediente.toString(), data)
      .subscribe(data => {
        console.log(data);
        this.documentos[this.indexDocument].estatus = this.estatusChangeDocument;
      });
  }

}
