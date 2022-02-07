import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponent } from 'src/app/app.component';
import { LoginModel } from 'src/app/models/login/login.model';
import { PbrService } from 'src/app/services/pbr.service';
import { SharedService } from 'src/app/services/shared.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent {

  login: LoginModel;
  error: boolean = false;
  message: string;
  btnLoading: any = {
    text: "Entrar",
    loading: false
  }

  constructor(private app: AppComponent, private pbrService: PbrService, private route: Router, private shared: SharedService) {

    if (this.app.auth) {

      this.route.navigate(['/adquisiciones/detalle']);

    } else {

      this.app.setTitle("Login");
      this.app.loading = false;
      this.shared.broadcastShowSelectAuditores(false);
      this.shared.broadcastShowSelectEjercicio(false);

      this.login = new LoginModel();
      this.customLogin();
    }
  }

  onSubmit() {

    this.app.loading = true;

    this.btnLoading.text = "Cargando...";
    this.btnLoading.loading = true;

    const data:any = {
      'Username': this.login.user,
      'Password': this.login.pass
    };

    if (this.login.user != null && this.login.pass != null) {

      this.logIn(data);

    } else {
      this.error = true;
      this.btnLoading.text = "Entrar";
      this.btnLoading.loading = false;
      this.app.loading = false;
    }

  }

  logIn(data: any) {
    this.pbrService.login(data)
      .subscribe(response => {

        if (response['success']) {

          localStorage.setItem("token", response['access_token']);
          localStorage.setItem("tokenRefresh", response['refresh_token']);
          localStorage.setItem("permits", response['listAcces']);

          this.shared.broadcastPermisosStream(response['listAcces']);

          this.app.auth = true;

          this.setInfoLocalStorage();
        } else {

          this.message = response['messages'][0];
          this.errorForm();
        }
      },
      error => {
        this.errorForm();
      });
  }

  setInfoLocalStorage() {
    this.pbrService.getInfoUser()
      .subscribe(response => {

        localStorage.setItem("username", response['username']);
        localStorage.setItem("name", response['name']);
        localStorage.setItem("role", response['role']);
        localStorage.setItem("municipio", response['municipio']);

        this.shared.broadcastUsernameStream(response['username']);
        this.shared.broadcastNameStream(response['name']);
        this.shared.broadcastRoleStream(response['role']);
        this.shared.broadcastMunicipioStream(response['municipio']);

        this.customLogin();

        this.app.loading = false;

        if (response['role'] === 'Ejecutivo') {
          this.route.navigate(['/dashboard']);
        } else {
          this.route.navigate(['/home']);
        }
      });
  }

  errorForm() {
    this.error = true;
    this.btnLoading.text = "Entrar";
    this.btnLoading.loading = false;
    this.app.loading = false;
  }

  customLogin() {


    if (this.app.auth) {
      document.getElementById("sidebarl").classList.remove('none');
      document.getElementById("navbarl").classList.remove('none');
      document.getElementById("breadcrumb").classList.remove('none');
      document.getElementById("select1").classList.remove('none');
      document.getElementById("select2").classList.remove('none');
    } else {
      document.getElementById("sidebarl").classList.add('none');
      document.getElementById("navbarl").classList.add('none');
      document.getElementById("breadcrumb").classList.add('none');
      document.getElementById("select1").classList.add('none');
      document.getElementById("select2").classList.add('none');
    }
  }

}
