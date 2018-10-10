import {Component, NgModule} from '@angular/core';
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {OpcuaInterceptor} from './opcua/opcua.interceptor';


@NgModule({
    bootstrap: [AppComponent],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: OpcuaInterceptor,
            multi: true
        }
    ]
})

@Component({
    selector: 'app',
    templateUrl: 'app.component.html'
})

// Style url should be added into Component as below
// styleUrls: ['./app.component.css']

export class AppComponent {
    name = 'Angular'
}