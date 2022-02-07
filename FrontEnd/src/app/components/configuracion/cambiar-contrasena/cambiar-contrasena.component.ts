import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { SharedService } from 'src/app/services/shared.service';
import { Router } from '@angular/router';
import { PbrService } from 'src/app/services/pbr.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cambiar-contrasena',
  templateUrl: './cambiar-contrasena.component.html',
  styleUrls: ['./cambiar-contrasena.component.css']
})
export class CambiarContrasenaComponent implements OnInit {

  contrasenaActual: string;
  nuevaContrasena: string;
  repetirContrasena: string;

  constructor(private app: AppComponent, private shared: SharedService, private router: Router, private pbrService: PbrService, private toastr: ToastrService) {
    if (this.app.auth) {

      this.app.setTitle("Cambiar Contraseña");
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.prepareBreadcrumb();
    } else {
      this.router.navigate(['/login']);
    }
  }

  ngOnInit(): void {}

  prepareBreadcrumb() {
    this.shared.activePage.emit("Cambiar Contraseña");
  }

  actualizarContrasena() {

    if (this.contrasenaActual != "" && this.nuevaContrasena != "" && this.repetirContrasena != "") {
      
      if (this.nuevaContrasena == this.repetirContrasena) {
        
        const data = {
          Password: this.contrasenaActual,
          NewPassword: this.nuevaContrasena
        };
    
        this.pbrService.setActualizarContrasena(data)
          .subscribe(data => {
            if (data['success']) {
              this.toastr.success("Contraseña actualizada exitosamente", "Exito!");
              this.router.navigate(["/dashboard"]);
            } else {
              this.toastr.error(data['messages'], "Error");
            }
          });
      } else {
        this.toastr.warning("Las contraseñas no coinciden", "Advertencia");
      }
    } else {
      this.toastr.warning("Es necesario llenar todos los campos", "Campos Vacios");
    }

  }

}
