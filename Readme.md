Semantic Web Service is written against OPC UA Protocol Suite which can help to end-users who does not know any idea about underlying structure.

The Web Service consists of the following structurew

1) OPC UA  GUI
2) JWT Authentication Middleware
3) OPC UA Middleware 

When a web user requires to connect the system, a password and an username need to be provided which has been predefined.
The authentication schema is similar to OAuth authentication which named "JSON Web Token Authentication". In this schema, a token is created
by server and immediately is sent back to a client. As soon as a client takes a token, a connection could be set up with server with Bearer
HTTP Request.


You can see a sample view of the project's GUI to handle read Object, Folder and Variable in OPC UA Protocol and write against the Values.


<p align="center"> <a  href="https://imgflip.com/gif/2k4ub4"><img src="https://i.imgflip.com/2k4ub4.gif" title="made at imgflip.com"/></a> </p>


The architecture is depicted as below:


![Architecture Page](https://github.com/zointblackbriar/SemanticWebAPI/blob/master/img/OPCUAWebService.png)


<p align="center"> ## How to install </p>

After you install all the files from the repository, you should type 

`dotnet run` 

under the folder named "SemanticAPI"

Backend side of the system will be up and running. Then you need to install GUI side with npm commands

Please type under the folder name FraunhoferLogin consequently 

`npm install`

`npm start`

The system was tested with the following technologies:

`Ubuntu 16.04`

`Windows 10`

`npm version -- 5.6.0`

`node version -- v8.11.1`

`dotnet version -- 2.2.100-preview1-009349 (In windows, you should install exactly the preview version which is marked by me)`


## The following technologies has been used:

## ASP.Net Core
## Angular 6

<p align="center"> <b><strong>Reference of Code</strong></b> </p>

@References 1: https://github.com/OPCUAUniCT/OPCUAWebPlatformUniCT
@References 2: http://jasonwatmore.com/post/2018/06/26/aspnet-core-21-simple-api-for-authentication-registration-and-user-management
@References 3: https://medium.com/codingthesmartway-com-blog/angular-material-part-4-data-table-23874582f23a

<p align="center"> ## Reference of Papers </p>

The foundation ideas of OPC UA Web Platform are discussed in the following papers. The platform described in these papers 
may differs for some aspects but the concepts are quite similar:

- [Integration of OPC UA into a web-based platform to enhance interoperability (ISIE 2017)](https://ieeexplore.ieee.org/document/8001417/)
- [OPC UA integration into the web (IECON 2017)](https://ieeexplore.ieee.org/document/8216590/)
- [A web-based platform for OPC UA integration in IIoT environment (ETFA 2017)](https://ieeexplore.ieee.org/document/8247713/)
- [Integrating OPC UA with web technologies to enhance interoperability (Computer Standards & Interfaces 2018 - Elsevier)](https://doi.org/10.1016/j.csi.2018.04.004)

