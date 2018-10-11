
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
    public data : Object;
    public temp_var : Object;
    //opcuaForm: FormGroup;
    addressInfo: any ;
    serverInfo: number;
    bodyOfRequest: any;
    connectionString: string;
    public errorMsg: any;





    constructor(public http: HttpClient){}
        ngAfterViewInit() : void
        {
                //this.fetchData();
                console.log("ngAfterViewInit");
                this.dtOptions = {
                    pagingType: 'full_numbers',
                    pageLength: 2,
                    dom : 'Bfrtip',
                };

        }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
    }
    public fetchData()
    {
        //{responseType:'json'})
        return this.http.get<any>(this.connectUrl + '/api/serverconf/' + this.serverInfo + '/allnodes/' + this.addressInfo)
            .subscribe( (res:Response) => {
                this.data = res;
                this.temp_var = true;
                this.arrayNodes = JSON.stringify(this.data);
                console.log(this.data);
                this.arrayNodes = JSON.parse(this.arrayNodes);
                // console.log(this.arrayNodes);
                // console.log(this.arrayNodes['node-id']);
                // console.log(this.arrayNodes['name']);
                // console.log(this.arrayNodes['type']);
                // console.log(this.arrayNodes['references'][0]);
                // console.log(this.arrayNodes['references'][1]);
                // console.log(this.arrayNodes['references'][2]);
                //
                // console.log(this.arrayNodes['references'][3]);
                //
                // console.log(this.arrayNodes['references'][4]);
                },
                (err: HttpErrorResponse) => {
                    this.errorMsg = err;
                    console.log(err.message);
                }
            );
    }

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
        return this.http.post(this.connectionString, this.bodyOfRequest, httpOptions)

            .subscribe( data => {
                console.log("POST Request is successful", data);
            },
                error => this.errorMsg = error
            );
    }




    ngOnInit(): void {

    }
}





