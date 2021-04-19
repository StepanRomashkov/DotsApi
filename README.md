# DotsApi

## Overview

This project has been created mostly due to intention to fill my portfolio with relatively fresh stuff as well as for learning purpose and out of curiousity of course. Also I would be happy if anyone would learn something new and interesting from this one. Feel free to leave any comments, suggestions or questions. I'll try to be as social as possible.

The general idea is to build an app that allows user to make notices for him(her)self which would notify the user in given time that he or she needs something to accomplish. The main difference between this app and the ones that nearly every person already has is that the app supposed to be platform independent. I'm planning to build the frontend part with **Native Script** which is able to automatically translate the code for both Android and iOS so even if user switch between these two it's easy to install the app and get access to their cloud data. That's the target MVP. Currently it's ongoung project with no frontend yet and half-implemented REST API built using **ASP.NET Core 3.1** with **MongoDB** as a persistant data storage.

The project in its present state is deployed to Azure App Service and available at https://dotsapi2021.azurewebsites.net It uses MongoDB Atlas as datasource. Feel free to test it with an API testing tool like Postman or https://reqbin.com/ Unfortunately, the first API call to the live version is always getting timeout possibly due to free subscription on Azure or free tier on Atlas. Subsequent ones are good. Here is the list of [available endpoints](#endpoints) implemented at this point.

#### Dependencies used
- [Automapper](https://docs.automapper.org/en/latest/Getting-started.html)
- [Bcrypt](https://en.wikipedia.org/wiki/Bcrypt)
- [JSON Web Tokens](https://jwt.io/)
- [MongoDB drivers](https://www.mongodb.com/)

## Installation

#### Things you gonna need(probably) to get everything up and running
- [MongoDB Community Server](https://www.mongodb.com/try/download/community) - nice and free mongo server to hold our precious stuff.
- [Microsoft Visual Studio](https://visualstudio.microsoft.com/) - or your favorite IDE to work with C#.
- [Robo 3T](https://www.robomongo.org/download) - very handy and fast GUI tool to work with MongoDB servers.
- [httprepl](https://docs.microsoft.com/en-us/aspnet/core/web-api/http-repl/?view=aspnetcore-3.1&tabs=windows) - pretty simple command line tool for Web API testing or anything which works best for you like Postman.

Unfortunately, I cannot descrbe the setup process for Mac users because I don't have Mac so the next portion is valid only for Windows. Sorry, guys.

In order to minimize possible pain from the setup process I created a couple of files that configure the local instance of mongo database server and populate it with some test data. After cloning the repo look at the [localDbScripts](localDbScripts/) folder. 

If you don't want to change anything and finish the setup as soon as possible, simply create "C:\mongoDatabases\Dots" folder and move all files from "localDbScripts" into "C:\mongoDatabases". Then, having MongoDB server installed, type `mongod --config C:\mongoDatabases\mongodDots.cfg` in the command line. 

This should start an instance of mongo server writing logs to "C:\mongoDatabases\DotsDb.log" so you can connect to it with Robo 3T. Change the config file at your conveinience, just make sure that the folder specified in dbPath section exists or the magic won't work.

Let's connect to our db server with Robo 3T! I didn't do anything related to database security since it's running locally. Due to that all we need to specify in the connection settings is a name of the connection. If you connected successfully right click on the connection and choose "Open Shell" then type and execute `load('C://mongoDatabases//createDb.js')` command. Later on I suggest to have a little chain of commands like:
```javascript
db.Users.drop();
load('C://mongoDatabases//createDb.js');
db.getCollection('Users').find({})
```
to be able to bring your database to its initial state in one hit.

Now we have our database running with some test data so we're ready to start our app server. Open the solution in Visual Studio and run IIS Express. You'll be prompted to create a local certificate and use it so just agree a couple of times. If everything went good you should see your browser with "https://localhost:44329/" in the address line if you haven't changed the project properties.

## Test API with httprepl
At this point you can test the API with your favorite testing app or, in case you decided to give a try to httprepl I mentioned before you can run it with the following command: `httprepl https://localhost:44329/`

If it's the first time you run httprepl you need to specify a text editor for passing parameters to the endpoints. You can do it with something like this: `pref set editor.command.default 'C:\\Program Files\\Microsoft VS Code\\Code.exe'` or the one you prefer most.

Now, connected to the base address of https://localhost:44329/ you can navigate to the 'users' controller with `cd users` and finally test *authenticate* method by typing `post authenticate` 

The text editor should open. Mine usually asks me whether I want to create a new file with non-human readable name. Pass the object: 
```json
{"email": "let@me.in", "password": "test"}
```
Save file. Cose the text editor. If everything has been done correctly you should get something like that:
```powershell
(Disconnected)> connect https://localhost:44329
Using a base address of https://localhost:44329/
Unable to find an OpenAPI description
For detailed tool info, see https://aka.ms/http-repl-doc

https://localhost:44329/> cd users

https://localhost:44329/users> post authenticate
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 09 Apr 2021 01:50:50 GMT
Server: Microsoft-IIS/10.0
Transfer-Encoding: chunked
X-Powered-By: ASP.NET

{
  "id": "606df820d40c8faea767e2cf",
  "email": "let@me.in",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MDZkZjgyMGQ0MGM4ZmFlYTc2N2UyY2YiLCJuYmYiOjE2MTc5MzMwNTAsImV4cCI6MTYxODAxOTQ0OSwiaWF0IjoxNjE3OTMzMDUwfQ.WiqMKyCKhxzlN4ItvVFjRMZcaO0v_cyme26AYGapF1U"
}
```

Since only methods *register* and *authenticate* don't require authenticated user we need to set Authorization header for further testing. In httprepl we can do it executing the following command: `set header Authorization "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MDZkZjgyMGQ0MGM4ZmFlYTc2N2UyY2YiLCJuYmYiOjE2MTc5MzMwNTAsImV4cCI6MTYxODAxOTQ0OSwiaWF0IjoxNjE3OTMzMDUwfQ.WiqMKyCKhxzlN4ItvVFjRMZcaO0v_cyme26AYGapF1U"`

Then you can navigate to the notices controller with `cd /notices` and then send the command `get` which returns an array of notice objects belonging to the user currently authenticated in the system:
```powershell
https://localhost:44329/users> set header Authorization "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MDZkZjgyMGQ0MGM4ZmFlYTc2N2UyY2YiLCJuYmYiOjE2MTc5MzMwNTAsImV4cCI6MTYxODAxOTQ0OSwiaWF0IjoxNjE3OTMzMDUwfQ.WiqMKyCKhxzlN4ItvVFjRMZcaO0v_cyme26AYGapF1U"

https://localhost:44329/users> cd /notices

https://localhost:44329/notices> get
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 09 Apr 2021 02:12:35 GMT
Server: Microsoft-IIS/10.0
Transfer-Encoding: chunked
X-Powered-By: ASP.NET

[
  {
    "id": "606df820d40c8faea767e2d1",
    "userId": "606df820d40c8faea767e2cf",
    "name": "Test Notice 1",
    "timeCreated": 07.04.2021 18:21:20,
    "timeCompleted": 10.04.2021 18:21:20,
    "isCompleted": false
  },
  {
    "id": "606df820d40c8faea767e2d2",
    "userId": "606df820d40c8faea767e2cf",
    "name": "Test Notice 2",
    "timeCreated": 07.04.2021 18:21:20,
    "timeCompleted": 10.04.2021 18:21:20,
    "isCompleted": false
  }
]
```

## Endpoints

So far the app has minimalistic design so the User model has only Id, Email and a collection of notices. Of course, it has a hashed password as well but thanks to Automapper it's usually not exposed. The following endpoints are currently available:

- /users/register [POST] - Allows user to be registered. Takes {"email": "*string*", "password": "*string*"} object as an argument
- /users/authenticate [POST] - Authenticates user. Takes {"email": "*string*", "password": "*string*"} object as an argument. Returns an object containing serialized JWT.
- /users/{id} [PUT] - Allows user to change their data. Authentication is required. Takes an object with the new data as a parameter, e.g. {"email": "newEmail"}. Should pass resource-based authorization check so users are allowed to change only their own data, not anybody else's.
- /users/{id} [DELETE] - Deletes a user. Authentication is required. Due to a resource-based authorization check users are allowed to delete only themselves.
- /notices [GET] - Returns all incompleted notices that belong to currently authenticated user.
- /notices/add [POST] - Creates a new notice for the current user. Takes a notice DTO object, e.g. {"name": "Feed the cat", "timeCompleted": "2021-04-16T18:00:00.899Z"}
- /notices/{id} [DELETE] - Deletes a specified notice from the current user.