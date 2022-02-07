import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-table-documents',
  templateUrl: './table-documents.component.html',
  styleUrls: ['./table-documents.component.css']
})
export class TableDocumentsComponent implements OnInit {

  @Input() expedientes: any;
  @Input() router: any;

  constructor() { }

  ngOnInit(): void {
  }

}
