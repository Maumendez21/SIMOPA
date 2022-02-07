import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-ver-documento-modal-adq',
  templateUrl: './ver-documento-modal-adq.component.html',
  styleUrls: ['./ver-documento-modal-adq.component.css']
})
export class VerDocumentoModalAdqComponent implements OnInit {

  @Input() doc: string;

  constructor() { }

  ngOnInit(): void {
  }

}
