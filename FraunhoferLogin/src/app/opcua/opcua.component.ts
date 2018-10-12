
//when we make any HTPP request, the user's token will be attached automatically

import {HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse, HttpParams} from "@angular/common/http";
import {Response} from "@angular/http";
import {Component, OnDestroy, OnInit} from "@angular/core";
import {DataTablesModule} from "angular-datatables";
import {Subject} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {Observable} from "rxjs/internal/Observable";
import {FormGroup} from "@angular/forms";
//import {Router, ActivatedRoute, ParamMap} from "@angular/router";

const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type':  'application/json'
    })
};


@Component({templateUrl: 'opcua.component.html', styleUrls: ['opcua.component.css'], selector: 'input-form'})
export class OpcuaComponent implements OnInit, OnDestroy {
    title = 'app';
    //dtOptions : DataTables.Settings = {};
    dtOptions: any = {};
    //nodes: Nodes[];
    dtTrigger: Subject<any> =  new Subject();

    connectUrl: string = 'http://localhost:4000';
    arrayNodes : any = [];
    valuesOfNode: any = [];
    statusOfNode: any = [];
    //arrayNodes: any;
    public data : Object;
    public temp_var : Object;
    //opcuaForm: FormGroup;
    addressInfo: any ;
    serverInfo: number;
    bodyOfRequest: any;
    connectionString: string;
    public errorMsg: any;
    //materialIcon: any;

    constructor(public http: HttpClient){}
        ngAfterViewInit() : void
        {
                //this.fetchData();
                console.log("ngAfterViewInit");
        }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
    }
    public fetchData(materialIcon: any)
    {
        if(materialIcon == null)
        {
            console.log("addressinfo not null");
            //{responseType:'json'})
            return this.http.get<any>(this.connectUrl + '/api/serverconf/' + this.serverInfo + '/allnodes/' + this.addressInfo)
                .subscribe( (res:Response) => {
                        this.data = res;
                        this.temp_var = true;
                        this.arrayNodes = JSON.stringify(this.data);
                        console.log(this.data);
                        this.arrayNodes = JSON.parse(this.arrayNodes);
                        this.valuesOfNode = this.arrayNodes['value'];
                        this.statusOfNode = this.arrayNodes['status'];
                        console.log("valuesOfNode" + this.valuesOfNode);
                        console.log("statusOfNode" + this.statusOfNode);
                    },
                    (err: HttpErrorResponse) => {
                        this.errorMsg = err;
                        console.log(err.message);
                    }
                );
        } else
        {
            console.log("materialIcon not null");

            return this.http.get<any>(this.connectUrl + '/api/serverconf/' + this.serverInfo + '/allnodes/' + materialIcon)
                .subscribe( (res:Response) => {
                    this.data = res;
                    this.temp_var = true;
                    this.arrayNodes = JSON.stringify(this.data);
                    this.arrayNodes = JSON.parse(this.arrayNodes);
                    this.valuesOfNode = this.arrayNodes['value'];
                    this.statusOfNode = this.arrayNodes['status'];
                    console.log(this.data);
                },   (err: HttpErrorResponse) => {
                        this.errorMsg = err;
                        console.log(err.message);
                }
            );
        }

    }

    //To control your form
    //get f() { return this.opcuaForm.controls; }

    setOpcUA() {
        console.log("hello SendCustomize");
        console.log("addressInfo" + this.addressInfo);
        this.addressInfo = "0-85";
    }

    serverConf()
    {
        console.log("hello SendCustomize");
        console.log("serverinfo" + this.serverInfo);
        return this.serverInfo;
    }

    public postData()
    {
        console.log("Post Request");
        this.connectionString = this.connectUrl + '/api/serverconf/' + this.serverInfo + '/allnodes/' + this.addressInfo;
        console.log("test String", this.connectionString);
        console.log("body of Request", this.bodyOfRequest);
        return this.http.post(this.connectionString, this.bodyOfRequest, httpOptions)

            .subscribe( data => {
                console.log("POST Request is successful", data);
            },
                error => this.errorMsg = error
            );
    }

    ngOnInit(): void {
        this.dtOptions = {
            pagingType: 'full_numbers',
            paging : true,
            ordering : true,
            info : true,
            pageLength: 2,
            dom : 'Bfrtip',
        };

    }
}





