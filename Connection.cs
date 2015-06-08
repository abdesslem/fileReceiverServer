using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace NetworkFileReceiver
{
    public class Connection
    {
        /*public IPAddress ipAdress;
        public TcpListener myList;
        public Socket s;
        private NetworkStream strRemote;
        private TcpListener tlsServer;
        private Stream strLocal;
         * 
         */
        public static byte[] key;
        public static byte[] IV;
        public IPAddress ipAdress;
        public TcpListener myList;
        public NetworkStream strRemote;
        public TcpClient tclServer;
        public void connection(Form1 fr)
        {
            try {
            fr.AppendText("Server running - Port: 5050\r\n");
            ipAdress = IPAddress.Parse("127.0.0.1");
            myList = new TcpListener(ipAdress, 5050);
            myList.Start();
            fr.AppendText("Local end point:" + myList.LocalEndpoint + "\r\n");
            fr.AppendText("Waiting for connections...\r\n");
            }
             catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);
                }
            while (true)
            {
            
                    //Tuple<string, string> tuple = CreateKeyPair();
                    tclServer = myList.AcceptTcpClient();
                    Thread thrclient = new Thread(() => HandleClient(tclServer, fr));
                    thrclient.Start();

                }
               
           }
       

        private void HandleClient(TcpClient tclServer, Form1 fr)
    {
               IPEndPoint endPoint = (IPEndPoint) tclServer.Client.RemoteEndPoint;
               IPAddress ipend = endPoint.Address;
               string sip = ipend.ToString();
               IPHostEntry hostEntry = Dns.GetHostEntry(ipend);
               string hostName = hostEntry.HostName;
               strRemote = tclServer.GetStream();
              //  s = myList.AcceptSocket();
              fr.AppendText("Connection accepted from IP :\r" +sip+ "\r\n HostName : \r" +hostName+ "\r\n");
                byte[] b = new byte[364];
               // int k = s.Receive(b);
               int k=strRemote.Read(b, 0, 364);
               fr.AppendText("Recieved the client public key...\r\n");
                /*for (int i = 0; i < k; i++)
                {
                    Console.Write(Convert.ToChar(b[i]));
                }*/
                Tuple<string, string> secret = AES.GenerateSecretKey();
                ASCIIEncoding asen = new ASCIIEncoding();
                key = asen.GetBytes(secret.Item1);
                IV = asen.GetBytes(secret.Item2);
                //  ASCIIEncoding asen = new ASCIIEncoding();
                string clientPub = System.Text.Encoding.Default.GetString(b);
                string data = "key=" + secret.Item1 + "#IV=" + secret.Item2;
                //Console.WriteLine(data);
                byte[] sharedKey = RSA.Encrypt(clientPub, data);
                strRemote.Write(sharedKey, 0, sharedKey.Length);
             
                fr.AppendText("Client authentification...\r\n");
                ClassConnectionSQL authen = new ClassConnectionSQL(tclServer);
               authen.Authentication();
               fr.AppendText("C Bon ...\r\n");
                    memDumpReceiver mem = new memDumpReceiver();
                    Thread thrDownload = new Thread(mem.DumpReceiver);
                    thrDownload.Start();
                    fr.AppendText("Can receive memory dump \r\n");
                    // fr.StartReceiving(strRemote);
                    //   Thread thrtransfert = new Thread(() => fr.StartReceiving(strRemote));
                    //  thrtransfert.Start();
                    fr.AppendText("Can receive suspicious files\r\n");
                    Thread receivefile = new Thread(() => fr.ReceiveFile(strRemote));
                    receivefile.Start();
                    fr.AppendText("Can receive collected evidence \r\n");
              

}
    }
}
