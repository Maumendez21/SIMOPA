import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-toast-upload',
  templateUrl: './toast-upload.component.html',
  styleUrls: ['./toast-upload.component.css']
})
export class ToastUploadComponent implements OnInit {

  @Input() title: string;
  @Input() description: string;
  @Input() show: boolean;
  @Input() uploadSuccess: boolean;

  constructor() { }

  ngOnInit(): void {
  }

  ngOnChanges(): void {
    console.log([this.title, this.description, this.show, this.uploadSuccess]);
  }

}
