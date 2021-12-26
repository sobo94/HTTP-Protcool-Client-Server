using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace myOwnWebServer
{
    public class MyWebServer
    {
        private readonly TcpListener _myListener;
        private readonly string _webRoot;

        /// <summary>
        /// Constructor of web server
        /// </summary>
        /// <param name="webRoot"> web root path </param>
        /// <param name="webIp"> ip of web </param>
        /// <param name="webPort"> port of web </param>
        public MyWebServer(string webRoot = "C:\\localWebSite", string webIp = "127.0.0.1", int webPort = 5300)
        {
            _webRoot = webRoot;

            try
            {
                //start listing on the given _webPort  
                _myListener = new TcpListener(IPAddress.Parse(webIp), webPort);
                _myListener.Start();
                MyOwnLogger.Log("SERVER STARTED", $"WebRoot: {webRoot} , WebIP : {webIp} , WebPort : {webPort}");
                //start the thread which calls the method 'ServerListener'  
                Thread th = new Thread(ServerListener);
                th.Start();
            }
            catch (Exception)
            {
                // ignored
            }
        }


        /// <summary>
        /// Getting mime type by own my type class
        /// </summary>
        /// <param name="serverRequestedFile"> requested file type for mime </param>
        /// <returns></returns>
        public string GetOwnMimeType(string serverRequestedFile)
        {
            // Convert to lowercase  
            serverRequestedFile = serverRequestedFile.ToLower();
            int iStartPos = serverRequestedFile.IndexOf(".", StringComparison.Ordinal);
            var sFileExt = serverRequestedFile.Substring(iStartPos);
            try
            {
                var sMimeType = MyOwnMimeGet.GetMimeType(sFileExt);
                return sMimeType;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// Sending http header
        /// </summary>
        /// <param name="sHttpVersion"> version of http </param>
        /// <param name="sMimeHeader"> mime header </param>
        /// <param name="iTotBytes"> total bytes </param>
        /// <param name="sStatusCode"> http status code </param>
        /// <param name="mySocket"> socket </param>
        public void SetHttpHeader(string sHttpVersion, string sMimeHeader, int iTotBytes, string sStatusCode, ref Socket mySocket)
        {
            string dateFormat = $"{DateTime.Now:ddd,' 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' 'K}";

            String sBuffer = "";
            // if Mime type is not provided set default to text/html  
            if (sMimeHeader.Length == 0)
            {
                sMimeHeader = "text/html";// Default Mime Type is text/html  
            }
            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: MyOwnWebServer\r\n";
            sBuffer = sBuffer + "Content-Type: " + sMimeHeader + "\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Date: " + dateFormat + " \r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";
            Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);
            SetBrowser(bSendData, ref mySocket);

            MyOwnLogger.Log("Response",
                sStatusCode.Contains("200")
                    ? $" {sStatusCode} ,Content-Type: {sMimeHeader} , Content-Length: {iTotBytes} , Server: MyOwnWebServer , Date : {dateFormat}"
                    : sStatusCode);
        }


        /// <summary>
        /// Calling send to browser by string data
        /// </summary>
        /// <param name="sData">Data</param>
        /// <param name="mySocket">socket</param>
        public void SetBrowser(String sData, ref Socket mySocket)
        {
            SetBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }
        
        
        
        /// <summary>
        ///  send to browser 
        /// </summary>
        /// <param name="sData">Data in bytes</param>
        /// <param name="mySocket">socket</param>
        public void SetBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            try
            {
                if (mySocket.Connected)
                {
                    int numBytes;
                    if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                        Console.WriteLine("Socket Error cannot Send Packet");
                    //else
                    //{
                    //    Console.WriteLine("No. of bytes send {0}", numBytes);
                    //}
                }
                else Console.WriteLine("Connection Dropped....");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occurred : {0} ", e);
            }
        }




        /// <summary>
        /// Server listening function
        /// </summary>
        public void ServerListener()
        {
            while (true)
            {
                //Accept a new connection  
                Socket mySocket = _myListener.AcceptSocket();

                if (mySocket.Connected)
                {
                   
                    //make a byte array and receive data from the client   
                    Byte[] bReceive = new Byte[1024];
                    mySocket.Receive(bReceive, bReceive.Length, 0);
                    //Convert Byte to String  
                    string sBuffer = Encoding.ASCII.GetString(bReceive);

                    // Look for HTTP request  
                    var iStartPos = sBuffer.IndexOf("HTTP", 1, StringComparison.Ordinal);
                    // Get the HTTP text and version e.g. it will return "HTTP/1.1"  
                    string sHttpVersion = sBuffer.Substring(iStartPos, 8);
                    // Extract the Requested Type and Requested file/directory  
                    var sRequest = sBuffer.Substring(0, iStartPos - 1);
                    //Replace backslash with Forward Slash, if Any  
                    sRequest = sRequest.Replace("\\", "/");
                    //If file name is not supplied add forward slash to indicate   
                    //that it is a directory and then we will look for the   
                    //default file name..  
                    if (sRequest.IndexOf(".", StringComparison.Ordinal) < 1 && (!sRequest.EndsWith("/")))
                    {
                        sRequest = sRequest + "/";
                    }
                    //Extract the requested file name  
                    iStartPos = sRequest.LastIndexOf("/", StringComparison.Ordinal) + 1;
                    var sRequestedFile = sRequest.Substring(iStartPos);
                    //Extract The directory Name  
                    var sDirName = sRequest.Substring(sRequest.IndexOf("/", StringComparison.Ordinal), sRequest.LastIndexOf("/", StringComparison.Ordinal) - 3);

                    MyOwnLogger.Log("Request", sRequest);

                    string verb = sBuffer.Substring(0, 3);
                    //At present we will only deal with GET type  
                    if (verb != "GET")
                    {
                        string ErrorMessage = "<H2>Error!! Requested method is not supported</H2><Br>";
                        //Format The Message  
                        SetHttpHeader(sHttpVersion, "", ErrorMessage.Length, " 405 Method Not Allowed", ref mySocket);
                        //Send to the browser  
                        SetBrowser(ErrorMessage, ref mySocket);
                        mySocket.Close();
                        continue;
                        //return;
                    }

                    /////////////////////////////////////////////////////////////////////  
                    // Identify the Physical Directory  
                    /////////////////////////////////////////////////////////////////////  
                    String sLocalDir;
                    if (sDirName == "/")
                        sLocalDir = _webRoot;
                    else
                    {
                        //Get the Virtual Directory  
                        sLocalDir = _webRoot + sDirName;
                    }
             
                    //If the physical directory does not exists then  
                    // display the error message  
                    String sErrorMessage;
                    if (sLocalDir.Length == 0)
                    {
                        sErrorMessage = "<H2>Error!! Requested Directory does not exists</H2><Br>";
                        //Format The Message  
                        SetHttpHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                        //Send to the browser  
                        SetBrowser(sErrorMessage, ref mySocket);
                        mySocket.Close();
                        continue;
                    }

                    /////////////////////////////////////////////////////////////////////  
                    // Identify the File Name  
                    /////////////////////////////////////////////////////////////////////  
                    //If The file name is not supplied then look in the default file list  
                    if (sRequestedFile.Length == 0)
                    {
                        // Get the default filename  
                        sRequestedFile = "index.html";
                    }

                    /////////////////////////////////////////////////////////////////////  
                    // Get TheMime Type  
                    /////////////////////////////////////////////////////////////////////  
                    String sMimeType = GetOwnMimeType(sRequestedFile);

                    if (sMimeType == "")
                    {
                        sErrorMessage = "<H2>File Type Not Supported...</H2>";
                        SetHttpHeader(sHttpVersion, "", sErrorMessage.Length, " 415 Unsupported Media Type", ref mySocket);
                        SetBrowser(sErrorMessage, ref mySocket);
                    }

                    //Build the physical path  
                    var sPhysicalFilePath = Path.Combine(sLocalDir, sRequestedFile);


                    if (File.Exists(sPhysicalFilePath) == false)
                    {
                        sErrorMessage = "<H2>404 Error! File Does Not Exists...</H2>";
                        SetHttpHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                        SetBrowser(sErrorMessage, ref mySocket);
                    }
                    else
                    {
                        int iTotBytes = 0;
                        var sResponse = "";
                        FileStream fs = new FileStream(sPhysicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        // Create a reader that can read bytes from the FileStream.  
                        BinaryReader reader = new BinaryReader(fs);
                        byte[] bytes = new byte[fs.Length];
                        int read;
                        while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Read from the file and write the data to the network  
                            sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);
                            iTotBytes = iTotBytes + read;
                        }
                        reader.Close();
                        fs.Close();
                        SetHttpHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                        SetBrowser(bytes, ref mySocket);
                        //mySocket.Send(bytes, bytes.Length,0);  
                    }
                    mySocket.Close();

                }
            }

        }


    }
}