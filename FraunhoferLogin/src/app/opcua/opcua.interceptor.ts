import {Component, Injectable, OnInit} from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor
} from '@angular/common/http'
import {OpcuaService} from './opcua.service';
import { Observable } from 'rxjs';
import {JwtInterceptor} from "../_helpers/jwt.interceptor";

//Creating an interceptor
//The goal is to include the JWT which is in local storage as the Authorization
//header in any HTTP request that is sent

@Injectable()
export class OpcuaInterceptor  implements HttpInterceptor{
    constructor(public auth : OpcuaService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler):
    Observable<HttpEvent<any>> {

        //let currentUser = JSON.parse(localStorage.getItem('currentUser'));

        request = request.clone({
        setHeaders:     {
            Authorization : 'Bearer ${this.auth.getToken()}'
            }
        });

    console.log("request is", request);

    return next.handle(request);
    }
}
