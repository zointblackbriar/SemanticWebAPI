import {Component, OnDestroy, OnInit} from "@angular/core";
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor, HttpErrorResponse, HttpHeaders, HttpClient
} from '@angular/common/http'
import {NgForOf} from "@angular/common";

const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type':  'text/plain, application/json, application/x-www-form-urlencoded',
        'Access-Control-Allow-Origin' : 'http://127.0.0.1:5000',
        'Access-Control-Allow-Methods' : 'GET, POST, PATCH, PUT, DELETE, OPTIONS',
        'Access-Control-Allow-Headers': 'Content-Type',
    })
};



@Component({templateUrl: 'questionanswering.component.html'})
export class QuestionansweringComponent implements OnInit, OnDestroy {

    bodyOfRequest: any;
    htmlShowData: any;
    htmlDynamicData: any;
    htmlShowError: any;
    constructor(private http: HttpClient) {

    }
    ngOnDestroy(): void {
    }

    ngOnInit(): void {
    }

    questionUrl: string = 'http://localhost:5000/integratedstaticmessage';
    questionDynamicUrl: string = 'http://localhost:5000/integrateddynamicmessage';
    public errorMsgQuestion:  any;

    //Send a POST Request
    public questionSender()
    {
            console.log("test Url", this.questionUrl);
            console.log("body of Request", this.bodyOfRequest);

            return this.http.post(this.questionUrl, JSON.stringify(this.bodyOfRequest), httpOptions )
                .subscribe(
                     data => {
                        this.htmlShowData = data;
                        console.log("POST call successful value returned in body",
                            data);

                    }, (error) => {
                         this.htmlShowError = error;
                        console.log("Error case", error);
                    });
    }

    public questionDynamicSender()
    {
        console.log("Dynamic URI", this.questionDynamicUrl);
        console.log("Body of Request in Dynamic URI", this.bodyOfRequest);

        return this.http.post(this.questionDynamicUrl, JSON.stringify(this.bodyOfRequest), httpOptions )
            .subscribe(
                data => {
                    this.htmlShowData = data;
                    console.log("POST call successful value returned in body",
                        data);

                }, (error) => {
                    this.htmlShowError = error;
                    console.log("Error case", error);
                });
    }

    mySortingFunction = (a:any, b:any) => {
        return a.key > b.key ? -1 : 1;
    }



}