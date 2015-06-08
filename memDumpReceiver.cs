using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;


namespace NetworkFileReceiver
{
    class memDumpReceiver
    {
        public void DumpReceiver()
        {
            IPAddress aip = IPAddress.Parse("127.0.0.1");
            const int BufferSize = 1024;
            TcpListener mylist = new TcpListener(aip, 5555);
            mylist.Start();
            TcpClient clt = mylist.AcceptTcpClient();
            NetworkStream stm = clt.GetStream();
            string path = @"c:\xd\";
            byte[] buffer = new byte[BufferSize];
            byte[] headbuf = new byte[64];
            stm.Read(headbuf, 0, 64);
            /*
              int receivedBytesLen = st.Read(clientData, 0, clientData.Length);
              int fileNameLen = BitConverter.ToInt32(clientData, 0);
              fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
             */
            long receive = 0L, length = BitConverter.ToInt64(headbuf, 0);
            // long filenamelen = BitConverter.ToInt64(buffer);
            int fileNameLen = BitConverter.ToInt32(headbuf, 8);
            string fileName = Encoding.ASCII.GetString(headbuf,12, fileNameLen);
            //string fileName = Encoding.Default.GetString(buffer, 0,stm.Read(buffer,0,1024));
            using (FileStream writer = new FileStream(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int received;
                while (receive < length)
                {
                    received = stm.Read(buffer, 0, 1024);
                    writer.Write(buffer, 0, received);
                    writer.Flush();
                    receive += (long)received;

                }
            }
            Console.WriteLine(" Receive finish.");
            stm.Close();
            mylist.Stop();
            Console.ReadLine();
        }
    }
}