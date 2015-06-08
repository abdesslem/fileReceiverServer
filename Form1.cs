using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.IO.Compression;

namespace NetworkFileReceiver
{
    public  partial class Form1 : Form
    {
        public string fileName;
        public BinaryWriter bWrite;
        // The thread in which the file will be received
        public Thread thrSecure;
        public Thread thrtransfert;
        // The stream for writing the file to the hard-drive
        private Stream strLocal;
        // The network stream that receives the file
        private NetworkStream strRemote;
        // The TCP listener that will listen for connections
        private TcpListener tlsServer;
        // Delegate for updating the logging textbox
        public delegate void UpdateStatusCallback(string StatusMessage);
        // Delegate for updating the progressbar
        private delegate void UpdateProgressCallback(Int64 BytesRead, Int64 TotalBytes);
        // For storing the progress in percentages
        private static int PercentProgress;

        public Form1()
        {
            InitializeComponent();
            this.MaximizeBox = false;
        }


   
         public void ReceiveFile(NetworkStream st)
        {
            //string fileName;
            //BinaryWriter bWrite;
            byte[] clientData = new byte[1024 * 5000];
            string receivedPath = @"C:\xd\";
           // while (st.CanRead)
            try
            {
                int receivedBytesLen = st.Read(clientData, 0, clientData.Length);
                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
                // Console.WriteLine("Client:{0} connected & File {1} started received.", clientSock.RemoteEndPoint, fileName);
                bWrite = new BinaryWriter(File.Open(receivedPath + fileName, FileMode.Append));
                bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
                Console.WriteLine("File: {0} received & saved at path: {1}", fileName, receivedPath);
                bWrite.Close();
                st.Close();
               AES.DecryptFile(@"C:\xd\encfiles.zip","decfiles.zip",Connection.key,Connection.IV);
            }
            catch (IOException ex) { Console.WriteLine(ex); }
        }
        public void AppendText(String text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendText), new object[] { text });
                return;
            }
            this.txtLog.Text += text;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
          //  thrDownload = new Thread(StartReceiving);
            //thrDownload.Start();
            Connection cn = new Connection();
            Thread thread = new Thread(() => cn.connection(this));
            thread.Start();
        }



        public void StartReceiving(NetworkStream strRemote)
        {
            //string FileName = "";
            // There are many lines in here that can cause an exception
            try
            {
                 // For holding the number of bytes we are reading at one time from the stream
                    int bytesSize = 0;

                    // The buffer that holds the data received from the client
                    byte[] downBuffer = new byte[2048];
                    // Read the first buffer (2048 bytes) from the stream - which represents the file name
                    bytesSize = strRemote.Read(downBuffer, 0, 2048);
                    // Convert the stream to string and store the file name
                    string FileName = System.Text.Encoding.ASCII.GetString(downBuffer, 0, bytesSize);
                    // Set the file stream to the path C:\ plus the name of the file that was on the sender's computer
                this.AppendText(@"C:\xd\" + FileName);
                strLocal = new FileStream(@"C:\xd\" + FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                  

                    // The buffer that holds the data received from the client
                    downBuffer = new byte[2048];
                    // Read the next buffer (2048 bytes) from the stream - which represents the file size
                    bytesSize = strRemote.Read(downBuffer, 0, 2048);
                    // Convert the file size from bytes to string and then to long (Int64)
                    //long FileSize = Convert.ToInt64(System.Text.Encoding.ASCII.GetString(downBuffer, 0, bytesSize));

                    // Write the status to the log textbox on the form (txtLog)
                    //this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "Receiving file " + FileName + " (" + FileSize + " bytes)\r\n" });
                    this.AppendText("receiving file");
                    // The buffer size for receiving the file
                    downBuffer = new byte[2048];

                    // From now on we read everything that's in the stream's buffer because the file content has started
                    while ((bytesSize = strRemote.Read(downBuffer, 0, downBuffer.Length)) > 0)
                    {
                        // Write the data to the local file stream
                        strLocal.Write(downBuffer, 0, bytesSize);
                        // Update the progressbar by passing the file size and how much we downloaded so far to UpdateProgress()
                        //  this.Invoke(new UpdateProgressCallback(this.UpdateProgress), new object[] { strLocal.Length, FileSize });
                    }
                    // When this point is reached, the file has been received and stored successfuly
                }
            catch (IOException ex) 
            {
                this.AppendText("this {0} error occur when thransfering file"+ ex);
            }

           // finally
           // {
                // This part of the method will fire no matter wether an error occured in the above code or not

                // Write the status to the log textbox on the form (txtLog)
                //DecompressToDirectory(@"C:\NewImages\files.zip", "NEWFILE");
                //this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "The file was received. Closing streams.\r\n" });

                // Close the streams
          //      strLocal.Close();
            //    strRemote.Close();

                // Write the status to the log textbox on the form (txtLog)
              //  this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { "Streams are now closed.\r\n" });

                // Start the server (TCP listener) all over again
                //DecompressToDirectory(@"C:\NewImages\files.zip", @"C:\NewImages\files");
               // StartReceiving(strRemote);
                
            //}
        }

        public void UpdateStatus(string StatusMessage)
        {
            // Append the status to the log textbox text 
            txtLog.Text += StatusMessage;
        }

        private void UpdateProgress(Int64 BytesRead, Int64 TotalBytes)
        {
            if (TotalBytes > 0)
            {
                // Calculate the download progress in percentages
                PercentProgress = Convert.ToInt32((BytesRead * 100) / TotalBytes);
                // Make progress on the progress bar
                prgDownload.Value = PercentProgress;
            }
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            strLocal.Close();
            strRemote.Close();
            txtLog.Text += "Streams are now closed.\r\n";
        }

        private void btnStop_Click_1(object sender, EventArgs e)
        {
            //ClassConnectionSQL auth= new ClassConnectionSQL();
          //  strLocal.Close();
            //strRemote.Close();
           // DecompressToDirectory(@"C:\NewImages\files.zip", @"C:\NewImages\fi");
            
            // txtLog.Text += "Streams are now closed.\r\n";
        }
    }
}