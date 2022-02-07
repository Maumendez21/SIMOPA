import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import { PbrService } from 'src/app/services/pbr.service';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { SharedService } from 'src/app/services/shared.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-usuarios-general',
  templateUrl: './usuarios-general.component.html',
  styleUrls: ['./usuarios-general.component.css']
})
export class UsuariosGeneralComponent implements OnInit {

  users: any;
  username: string;
  password: string;
  status: boolean;

  linkDB: string = "https://apipbrdevelolop.azurewebsites.net/api/report/obra/general/adquisiciones/obra/zip/"+localStorage.getItem('municipio');

  constructor(private app: AppComponent, private pbrService: PbrService, private shared: SharedService, private router: Router, private toastr: ToastrService) {
    if (this.app.auth) {
      this.app.setTitle("Usuarios - Configuración");
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);
      this.prepareBreadcrumb();
      this.getListUsers();
    } else {
      this.router.navigate(['/login']);
    }
  }

  ngOnInit(): void {}

  prepareBreadcrumb() {
    this.shared.activePage.emit("Usuarios");
    this.shared.parts.emit([
      { name: "Configuración", route: "configuracion/usuarios-general" }
    ]);
  }

  getListUsers() {
    this.pbrService.getListGeneralUsers()
      .subscribe(response => {
        this.users = response;
      });
  }

  setUsername(username: string) {
    this.username = username;
  }

  changeStatus(id: string, status: boolean, index: number) {
    const data = {
      "Id": id,
      "Active": !status
    };

    this.pbrService.setUpdateGeneralUserStatus(data)
      .subscribe(response => {
        this.users[index].Active = !status;
      });
  }

  changePassword() {
    const data = {
      "UserName": this.username,
      "Password": this.password
    };

    this.pbrService.setUpdateGeneralUserPassword(data)
      .subscribe(response => {
        if (response['success']) {
          this.toastr.success("La contraseña fue actualizada exitosamente", "Exito!");
        } else {
          this.toastr.error(response['messages'][0], "Error!");
        }

        this.password = "";
      });
  }

}
