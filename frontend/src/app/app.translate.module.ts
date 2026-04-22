import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export function HttpLoaderFactory() {
  return new TranslateHttpLoader();
}

@NgModule({
  exports: [
    TranslateModule,
    FormsModule
  ],
  imports: [
    TranslateModule.forRoot({
      defaultLanguage: 'fr',
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: []
      }
    })
  ]
})
export class AppTranslateModule { }