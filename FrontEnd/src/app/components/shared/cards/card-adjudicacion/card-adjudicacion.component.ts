import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PbrService } from 'src/app/services/pbr.service';

@Component({
  selector: 'app-card-adjudicacion',
  templateUrl: './card-adjudicacion.component.html',
  styleUrls: ['./card-adjudicacion.component.css']
})
export class CardAdjudicacionComponent implements OnInit {

  @Input() adjudicacion:any;
  estatusColor: string;
  cedulaComprasUrgentes: string = "REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL";

  constructor(private router: Router, private pbrService: PbrService) {
    //(adjudicacion.objetoContrato.length>39)? (adjudicacion.objetoContrato | slice:0:39)+'...':(adjudicacion.objetoContrato)
  }

  ngOnInit(): void {
    this.verificarEstatusColor();
  }

  verificarEstatusColor() {
    switch (this.adjudicacion.estatusExpediente) {
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

  verCheckList(id:string) {
    this.router.navigate(['/adquisiciones/expediente', id]);
  }

}
