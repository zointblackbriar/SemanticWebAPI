Semantic Web Service is written against OPC UA Protocol Suite which can help to end-users who does not know any idea about underlying structure.

The Web Service consists of the following structurew

1) Web Authentication GUI
2) JWT Authentication Middleware
3) OPC UA Middleware 

When a web user requires to connect the system, a password and an username need to be provided which has been predefined.
The authentication schema is similar to OAuth authentication which named "JSON Web Token Authentication". In this schema, a token is created
by server and immediately is sent back to a client. As soon as a client takes a token, a connection could be set up with server with Bearer
HTTP Request.


You can see a sample view of the project's GUI to handle read Object, Folder and Variable in OPC UA Protocol and write against the Values.


<a href="https://imgflip.com/gif/2k4ub4"><img src="https://i.imgflip.com/2k4ub4.gif" title="Intro Page"/></a>


To be continued