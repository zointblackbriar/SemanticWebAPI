import {Component, Injectable, OnInit} from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor, HttpErrorResponse
} from '@angular/common/http'
import {OpcuaService} from './opcua.service';
import { Observable } from 'rxjs';
import {tap, map, dematerialize, delay, materialize} from 'rxjs/operators';
import {JwtInterceptor} from "../_helpers/jwt.interceptor";
import {Router} from "@angular/router";

//Creating an interceptor
//The goal is to include the JWT which is in local storage as the Authorization
//header in any HTTP request that is sent

@Injectable()
export class OpcuaInterceptor  implements HttpInterceptor{
    constructor(public auth : OpcuaService, private router: Router) {}
    returnLoginURL: string;
    intercept(request: HttpRequest<any>, next: HttpHandler):
    Observable<HttpEvent<any>> {

        //let currentUser = JSON.parse(localStorage.getItem('currentUser'));

        request = request.clone({
        setHeaders:     {
            Authorization : 'Bearer ${this.auth.getToken()}'
            }
        });

    console.log("request is", request);

    //return next.handle(request);
        // if the token is invalid, route into login page
        return next.handle(request).pipe(tap((event: HttpEvent<any>) => {
            if(event instanceof HttpErrorResponse) {
                //Do something
            }
        }, (err : any) => {
            if(err instanceof HttpErrorResponse) {
                if(err.status === 401){
                    //redirection to login page
                    console.log("There is an authentication error. Token is not valid");
                    this.returnLoginURL = '/login';
                    this.router.navigate([this.returnLoginURL]);
                }

            }
        }))
        .pipe(materialize())
        .pipe(delay(500))
        .pipe(dematerialize());

    }
}
