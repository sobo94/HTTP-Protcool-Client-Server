# HTTP-Protcool-Client-Server

## HTTP Tool (client)
HTTP Tool is a WPF Application written in C#. It generates some prebuilt HTTP request's and also has the ability to Issue Custom HTTP Request.
It take's in user input of Port Number, IP Address and supports sending both GET and POST request to the server. It can request Image files,
PHP or ASP Files. 

![01](https://user-images.githubusercontent.com/16788406/147656751-da9e2d35-9f30-4111-8c9a-bba0341ee504.PNG)

The client then displays the incoming message sent by the server.The tool has a functionality built in
to count the characters of the message sent from the server. This is to verify the Response request field of 
Content Lenght.

![03](https://user-images.githubusercontent.com/16788406/147657296-00362032-1790-4b8c-b4f2-131ca83a4fc8.PNG)

Note: The HTTP Tool is primarily a testing tool to display the various HTTP Code which the Server can render. 

---
## HTTP Server 
HTTP Server is a Console Application written in C#. It takes in 3 command line argument's.
 
| Command Line Arg  | Explaination                                                                                |
| :---------------: | :-----------------------------------------------------------------------------------------: |
|      WebRoot      |     which will be set to the root folder for the website data (html, jpg and gif files)     | 
|      WebIp        |     which will be set to the IP address of the server                                       |   
|      WebPort      |     which will be set to the Port number the server will be listening on                    | 

### Example of running from command line
myOwnWebServer –webRoot=C:\localWebSite –webIP=192.168.100.23 –webPort=5300

### Functionality

- The server support the returned content types of plain text (specifically the .txt extension),
HTML files (and their various extensions), JPG images (and their various extensions) and GIF.

- The server Supports the following HTTP Status Codes.

| HTTP Status Code  |       Explaination                  |
| :---------------: | :---------------------------------: |
|      200          |     Status Ok                       | 
|      400          |     Bad error request               |   
|      404          |     File Not Found                  | 
|      405          |     Method Not Allowed              |   
|      415          |     Unsupported Media Type          | 

- Application Logs HTTP Request and Response into a log file. Contained in the Bin Folder.
