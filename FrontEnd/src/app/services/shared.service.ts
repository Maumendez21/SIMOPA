import { Injectable, Output, EventEmitter } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  @Output() activePage: EventEmitter<string> = new EventEmitter();
  @Output() parts: EventEmitter<any> = new EventEmitter();

  public update: BehaviorSubject<boolean> = new BehaviorSubject(true);
  public showSelectAuditores: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public showSelectEjercicio: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public ejercicio: BehaviorSubject<number> = new BehaviorSubject(2021);
  public auditor: BehaviorSubject<string> = new BehaviorSubject("auditorestodosdq37iq84burwwjdbsd3");
  public username: BehaviorSubject<string> = new BehaviorSubject("");
  public name: BehaviorSubject<string> = new BehaviorSubject("");
  public role: BehaviorSubject<string> = new BehaviorSubject("");
  public municipio: BehaviorSubject<string> = new BehaviorSubject("");
  public permisos: BehaviorSubject<string> = new BehaviorSubject("");

  public updateStream$ = this.update.asObservable();
  public showSelectAuditoresStream$ = this.showSelectAuditores.asObservable();
  public showSelectEjercicioStream$ = this.showSelectEjercicio.asObservable();
  public ejercicioStream$ = this.ejercicio.asObservable();
  public auditorStream$ = this.auditor.asObservable();
  public usernameStream$ = this.username.asObservable();
  public nameStream$ = this.name.asObservable();
  public roleStream$ = this.role.asObservable();
  public municipioStream$ = this.municipio.asObservable();
  public permisosStream$ = this.permisos.asObservable();

  constructor() { }

  public broadcastUpdateStream(update: boolean) {
    this.update.next(update);
  }

  public broadcastShowSelectAuditores(showSelectAuditores: boolean) {
    this.showSelectAuditores.next(showSelectAuditores);
  }

  public broadcastShowSelectEjercicio(showSelectEjercicio: boolean) {
    this.showSelectEjercicio.next(showSelectEjercicio);
  }

  public broadcastEjercicioStream(ejercicio: number) {
    this.ejercicio.next(ejercicio);
  }

  public broadcastAuditorStream(auditor: string) {
    this.auditor.next(auditor);
  }

  public broadcastUsernameStream(username: string) {
    this.username.next(username);
  }

  public broadcastNameStream(name: string) {
    this.name.next(name);
  }
  public broadcastRoleStream(role: string) {
    this.role.next(role);
  }
  public broadcastMunicipioStream(municipio: string) {
    this.municipio.next(municipio);
  }

  public broadcastPermisosStream(permisos: string) {
    this.permisos.next(permisos);
  }

  public clearSharedSession() {

    localStorage.clear();

    this.username.next("");
    this.name.next("");
    this.role.next("");
    this.municipio.next("");
    this.permisos.next("");
  }
}
