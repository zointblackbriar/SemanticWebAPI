import { Injectable } from '@angular/core';
//import decode from 'jwt-decode';
import {JwtHelperService} from '@auth0/angular-jwt'
import {catchError} from "rxjs/operators";
import {HttpHeaders} from "@angular/common/http";



@Injectable()
export class OpcuaService {
    //here is token service
    connectUrl: string = 'http://localhost:4000';

    constructor(public jwtHelper : JwtHelperService){}
    public getToken() : string {
        return localStorage.getItem('token')
    }


    public isAuthenticated() : boolean {
        //get the token
        const token = this.getToken();
        //return a boolean reflecting
        //whether or not the token is expired
        console.log("Token is expired or not", this.jwtHelper.isTokenExpired());
        return this.jwtHelper.isTokenExpired();
    }




}
