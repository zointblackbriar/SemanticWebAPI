
//when we make any HTPP request, the user's token will be attached automatically

import {HttpClient, HttpErrorResponse, HttpResponse} from "@angular/common/http";
import {Response} from "@angular/http";
import {Component, OnDestroy, OnInit} from "@angular/core";
import {DataTablesModule} from "angular-datatables";
import {Subject} from 'rxjs';
import {map} from 'rxjs/operators';
import {Observable} from "rxjs/internal/Observable";
//import {Router, ActivatedRoute, ParamMap} from "@angular/router";

// export interface OPCNodes {
//     opcuanodeId:number;
//     opcuanodeType:String;
//     opcuanodeObjectType:String;
//     opcuanodeReferences: String [];
// }


@Component({templateUrl: 'opcua.component.html', styleUrls: ['opcua.component.css']})
export class OpcuaComponent implements OnInit, OnDestroy {
    title = 'app';
    dtOptions : DataTables.Settings = {};
    //nodes: Nodes[];
    dtTrigger: Subject<any> =  new Subject();

    connectUrl: string = 'http://localhost:4000';
    //arrayNodes : OPCNodes[];
    arrayNodes : any = [];
    //arrayNodes: json [];
    newNodes : string [];
    public data : Object;
    public temp_var : Object;



    constructor(public http: HttpClient){}
        ngAfterViewInit() : void
        {
            // this.fetchData().subscribe(data=> {
            //     this.arrayNodes.push(data);
            //     this.dtTrigger.next();
            // });
            // this.http.get<NodesInf[]>('http://localhost:4000/api/data-sets/1/nodes', {responseType:'json'})
            //     .subscribe(data => {
            //         // console.log(data);
            //         this.arrayNodes = data;
            //         // Calling the DT trigger to manually render the table
            //         this.dtTrigger.next();
            //     });

                this.fetchData();
                console.log("ngAfterViewInit");
                this.dtOptions = {
                    pagingType: 'full_numbers',
                    pageLength: 2
                };

        }

        ngOnDestroy(): void {
            this.dtTrigger.unsubscribe();
        }

     public fetchData()
    {
        //{responseType:'json'})
        //bir sonraki satirda node address space denemeleri yapilacak
        return this.http.get<any>(this.connectUrl + '/api/data-sets/1/nodes/')
            //.pipe(map(response => <OPCNodes>response.json()))
            .subscribe( (res:Response) => {
                this.data = res;
                this.temp_var = true
                this.arrayNodes = JSON.stringify(this.data);
                console.log(this.data);
                this.arrayNodes = JSON.parse(this.arrayNodes);
                console.log(this.arrayNodes);
                console.log(this.arrayNodes['node-id']);
                console.log(this.arrayNodes['name']);
                console.log(this.arrayNodes['type']);
                console.log(this.arrayNodes['references'][0]);
                console.log(this.arrayNodes['references'][1]);
                console.log(this.arrayNodes['references'][2]);

                console.log(this.arrayNodes['references'][3]);

                console.log(this.arrayNodes['references'][4]);


                    //console.log(JSON.stringify(data.node-id));
                //console.log(JSON.stringify(data.name));
                //console.log(JSON.stringify(data.type));
                },
                (err: HttpErrorResponse) => {
                    console.log(err.message);
                }
            );


        // this.http.get('http://localhost:4000/api/data-sets/1/nodes', {responseType:'json'})
        //     //.map(res: Response)
        //     .subscribe(response => {
        //         this.newNodes = response;
        //         this.dtTrigger.next();
        //     }, (err: HttpErrorResponse) => {
        //         console.log(err.message);
        //     });
        // console.log("fetch data");
        // const url = 'http://localhost:4000/api/data-sets/1/nodes';
        // return this.http.get(url, {responseType:'json'});

    }
    // private extractData(res: Response) {
    //     let body = <OPCNodes[]>res.json().afdelingen;    // return array from json file
    //     return body || [];     // also return empty array if there is no data
    // }


    ngOnInit(): void {
    }



}




