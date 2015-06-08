using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Net.Sockets; // Ajouter cet espace de noms qui permet d'apporter

namespace NetworkFileReceiver
{
    class ClassConnectionSQL
    {
        private SqlConnection con; //pour la connection
        private string strcon; //pour la chaine de connection
        public string req; //faire les requêtes
        public SqlDataReader rdr; //transités les données(traitement)
        public bool auth;
        NetworkStream RemoteStream;
        string Username, Userpass, passphrase;
        byte[] Npass;
        int i;
        public ClassConnectionSQL(TcpClient tcpClient)  //constructeur des classes
        {
            RemoteStream = tcpClient.GetStream();
            strcon = @"Data Source=.\SQLEXPRESS;AttachDbFilename=""C:\Users\slouma\Documents\SQLEXPRESS.mdf"";Integrated Security=True;Connect Timeout=30;User Instance=True";
            con = new SqlConnection(strcon);
            con.Open();
            auth = false;
        }
            public void Authentication()  //constructeur des classes
        {
            Npass = new byte[1024];
           while(auth==false)
            {
                i = RemoteStream.Read(Npass, 0, 1024);
                passphrase = System.Text.Encoding.Default.GetString(Npass);
               // passphrase = AES.AuthDecrypt(Npass, Connection.key, Connection.IV);
                char[] delimiterChars = { '&' };
                string[] result = passphrase.Split(delimiterChars);
                Username = result[0];
                Userpass = result[1];
                this.req = String.Format("Select * from Users where password='" + Userpass + "'and Nom='" + Username + "'");
                this.ExecuterDB();
                if (this.rdr.Read())
                {
                    MessageBox.Show("yes good");
                    Array.Clear(Npass, 0, Npass.Length);
                    auth = true;
                   // j = 3;
                    rdr.Close();
                   //this.FermerDB();
                }
                else
                {
                    MessageBox.Show("NOooooooooo");
                    Array.Clear(Npass, 0, Npass.Length);
                   // RemoteStream.Flush();
                    rdr.Close();
                  // this.FermerDB();
                }
            }
             }
        public void ExecuterDB()
        {
            SqlCommand cmd = new SqlCommand(req, con);
            rdr = cmd.ExecuteReader();
        }
        public void FermerDB()
        {
            rdr.Dispose();
            con.Dispose();
        }
    }
}