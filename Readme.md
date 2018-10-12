Semantic Web Service is written against OPC UA Protocol Suite which can help to end-users who does not know any idea about underlying structure.

The Web Service consists of the following structurew

1) Web Authentication GUI
2) JWT Authentication Middleware
3) OPC UA Middleware 

When a web user requires to connect the system, a password and an username need to be provided which has been predefined.
The authentication schema is similar to OAuth authentication which named "JSON Web Token Authentication". In this schema, a token is created
by server and immediately is sent back to a client. As soon as a client takes a token, a connection could be set up with server with Bearer
HTTP Request.