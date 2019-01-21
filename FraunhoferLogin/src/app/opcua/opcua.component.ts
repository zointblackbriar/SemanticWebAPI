
//when we make any HTPP request, the user's token will be attached automatically

import {HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse, HttpParams} from "@angular/common/http";
import {Response} from "@angular/http";
import {Component, OnDestroy, OnInit, Pipe, PipeTransform} from "@angular/core";
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

//@Pipe({name: 'valuesOfNode'})
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
    writingNode: any;
    subscriptionId: any;
    connectionString: string;
    subscriptionPath: string;
    public errorMsg: any;
    globalMaterialIcon: any;

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
        if(materialIcon != null)
            this.globalMaterialIcon = materialIcon;

            return this.http.get<any>(this.connectUrl + '/api/serverconf/' + this.serverInfo + '/allnodes/' + this.globalMaterialIcon)
                .subscribe( (res:Response) => {
                    this.data = res;
                    this.temp_var = true;
                    this.arrayNodes = JSON.parse(JSON.stringify(this.data));
                    // this.arrayNodes = JSON.parse(this.arrayNodes);
                    this.valuesOfNode = this.arrayNodes['value'];
                    this.statusOfNode = this.arrayNodes['status'];
                    console.log('this.valuesOfNode', this.valuesOfNode);
                    console.log('this.statusOfNode', this.statusOfNode);

                    console.log(this.data);
                },   (err: HttpErrorResponse) => {
                        this.errorMsg = err;
                        console.log(err.message);
                }
            );
    }

    //To control your form
    //get f() { return this.opcuaForm.controls; }

    setOpcUA() {
        console.log("setOpcUA");
        console.log("addressInfo" + this.addressInfo);
        this.addressInfo = "0-85";
        this.globalMaterialIcon = this.addressInfo;
    }

    serverConf(param_string: any)
    {
        console.log("hello SendCustomize");
        console.log("param_string", param_string);
        this.serverInfo = param_string;
        console.log("serverinfo" + this.serverInfo);
        return this.serverInfo;
    }

    public postData()
    {
        console.log("Post Request");
        this.connectionString = this.connectUrl + '/api/serverconf/' + this.serverInfo + '/allnodes/' + this.globalMaterialIcon;
        console.log("connection test string", this.connectionString);
        console.log("The value of writeable node", this.writingNode);
        return this.http.post(this.connectionString, this.writingNode, httpOptions)

            .subscribe( data => {
                console.log("POST Request is successful", data);
            },
                error => this.errorMsg = error
            );
    }

    public subscribtionOPCUA()
    {
        console.log("Subscribe Request");
        this.subscriptionPath = this.connectUrl + '/api/serverconf/' + this.serverInfo + '/subscribeNodes/' + this.subscriptionId;
        console.log("Test Subscription Path:", this.subscriptionPath);
        console.log("Subscription ID:", this.subscriptionId);
        return this.http.post(this.subscriptionPath, this.subscriptionId, httpOptions)
            .subscribe( data => {
                console.log("POST Request of Subscription Request is attempted", data);
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





