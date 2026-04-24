import { HttpClient, HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export function HttpLoaderFactory(): TranslateHttpLoader {
  return new TranslateHttpLoader();
}

export function initTranslate(translate: TranslateService) {
  return () => {
    translate.setDefaultLang('fr');
    return translate.use('fr').toPromise();
  };
}

@NgModule({
  exports: [
    TranslateModule,
    FormsModule,
    HttpClientModule
  ],
  imports: [
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory
      }
    })
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initTranslate,
      deps: [TranslateService],
      multi: true
    }
  ]
})
export class AppTranslateModule { }