import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'notext'
})
export class NotextPipe implements PipeTransform {

  transform(text: string): string {
    if (text != null) {
      return text;
    } else {
      return 'N/D';
    }
  }

}
