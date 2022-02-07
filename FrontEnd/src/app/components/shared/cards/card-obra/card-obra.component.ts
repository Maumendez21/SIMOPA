import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { PbrService } from 'src/app/services/pbr.service';

@Component({
  selector: 'app-card-obra',
  templateUrl: './card-obra.component.html',
  styleUrls: ['./card-obra.component.css']
})
export class CardObraComponent implements OnInit {

  @Input() obra:any;
  estatusColor: string;
  tepeaca: boolean;

  constructor(private router: Router, private pbrService: PbrService) {
    
    if (localStorage.getItem("municipio") === "TEPEACA") {
      this.tepeaca = true;
    } else {
      this.tepeaca = false;
    }
    
  }

  ngOnInit(): void {
    this.verificarEstatusColor();
  }

  verificarEstatusColor() {

    switch (this.obra.estatusExpediente) {
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
    this.router.navigate(['/obras/expediente', id]);
  }

}
