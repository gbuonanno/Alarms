using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Collections.Generic;





// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Try1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        //MySqlConnection sqlConn = new MySqlConnection();
        //MySqlCommand sqlCmd = new MySqlCommand();
        //DataTable sqlDt = new DataTable();
        //String sqlQuery;
        //MySqlDataAdapter Dta = new MySqlDataAdapter();
        //DataSet Ds = new DataSet();
        //MySqlDataReader sqlRd;

        //String server = "localhost";
        //String username = "root";
        //String password = "SUNRISE";
        //String database = "test";
        //String table = "test.error";
        string serverDB = "localhost";
        string idDB = "root";
        string passDB = "SUNRISE";
        string nameDB = "test";
        string tableDB = "test.error";

        public MainWindow()
        {
            this.InitializeComponent();
            upLoadData();
            AnalizeData();


        }

        //private void uploaddata()
        //{
        //    sqlconn.connectionstring = "server=" + server + ";" + "user id=" + username + ";" +
        //        "password=" + password + ";" + "database=" + database;
        //    sqlconn.open();
        //    sqlcmd.connection = sqlconn;
        //    sqlcmd.commandtext = "select * from " + table;
        //    sqlrd = sqlcmd.executereader();
        //    sqldt.load(sqlrd);
        //    sqlrd.close();
        //    sqlconn.close();


        //}

       

        public void upLoadData()
        {
            System.Diagnostics.Debug.WriteLine("llamada a la función");
            //var item = (sender as ListView).SelectedItem as Error;
            List<Error> Alarms_list = new List<Error>();
            try
            {
                // conexión con el servidor local de mysql. el puerto es el 3360.
                // connection with the local server of mysql. port 3360

                var connstr = "server=" + serverDB + ";uid=" + idDB + ";pwd=" + passDB + ";database=" + nameDB;

                using (var conn = new MySqlConnection(connstr))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                       
                        cmd.CommandText = "select * from " + tableDB;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                    
                                Error error = new Error();
                                error.ID = reader.GetString(0);
                                error.Message = reader.GetString(1);
                                error.Level = reader.GetString(2);
                                error.Action = reader.GetString(3);
                                error.Time = reader.GetDouble(4);
                                error.Date = reader.GetDateTime(5);
                                Alarms_list.Add(error);

                                
                                System.Diagnostics.Debug.WriteLine("column" + error.ID + error.Message + error.Level + error.Action);
                                
                            }
                            ListViewError.ItemsSource = Alarms_list;



                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("error en la lectura de crankshaft de mysql");
            }
        }

        public void AnalizeData()
        {
            System.Diagnostics.Debug.WriteLine("llamada a la función");
            List<Error_list> error_Lists = new List<Error_list>();
            int Code_count = 0;
            
            try
            {
                // conexión con el servidor local de mysql. el puerto es el 3360.
                // connection with the local server of mysql. port 3360

                var connstr = "server=" + serverDB + ";uid=" + idDB + ";pwd=" + passDB + ";database=" + nameDB;

                using (var conn = new MySqlConnection(connstr))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                     

                        
                        cmd.CommandText = "select * from " + tableDB;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                               
                               
                                //error_Lists.Add(new Error_list() { Message = reader.GetString(1), Time = reader.GetDouble(4), Count = 1 });
                                int i = 0;
                               // error_Lists.Add(new Error_list() { Time = reader.GetDouble(4) });
                                //System.Diagnostics.Debug.WriteLine("column" + error_Lists[i].Time);
                                i++;
                                String Message_Entry = reader.GetString(1);

                                bool found = false;
                                
                                int size = error_Lists.Count;
                                for (int k = 0; k < size; k++)
                                {
                                    if (error_Lists[k].Message == Message_Entry)

                                    {
                                        System.Diagnostics.Debug.WriteLine("Entrada al if");
                                        error_Lists[k].Count++;
                                        //error_Lists.Find(x => x.Message.Contains(Message_Entry)).Count = error_Lists.Find(x => x.Message.Contains(Message_Entry)).Count + 1;
                                        found = true;
                                    }
                                }
                                if( found == false)
                                {
                                    // Temporary code counter, should be added from the SQL data base
                                 Code_count++;
                                 error_Lists.Add(new Error_list() { Message = reader.GetString(1), Time = reader.GetDouble(4), Count = 1 , Level =1 ,Code= Code_count});
                                }
                                  

                                ;
                            }
                            ListViewErrors.ItemsSource = error_Lists;
                            


                            // se introducen en la listview crankshaftlist todas los items (crnks) generados (tantos items como filas en la tabla anterior).
                            // in the listview crankshaftlist we add all of the items (crnks) generated (as many row as the previous table).
                            Console.WriteLine();
                            foreach (Error_list aPart in error_Lists)
                            {
                                System.Diagnostics.Debug.WriteLine(aPart.Count);
                            }

                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("error en la lectura de crankshaft de mysql");
            }

            // Calculo de Volumen de tiempo de para
            double Stop_Time = 0;
            foreach (Error_list aPart in error_Lists)
            {
                
                System.Diagnostics.Debug.WriteLine(aPart.Total_Stop());
                Stop_Time = Stop_Time + aPart.Total_Stop();
                aPart.Total = aPart.Total_Stop();
            }
            //1440 Minutes a day 

            double Availability = (1440 - Stop_Time)/1440;
            
            System.Diagnostics.Debug.WriteLine("Availability =" + Availability + "%");
            Availability_TextBlock.Text = Availability.ToString(".##"+"%");
           
            //Availability_Text.Text=DateTime.Today.ToString(); 

            // Sort by Total 
            error_Lists.Sort(
                delegate (Error_list p1, Error_list p2)
                {
                    int compareDate = p2.Total_Stop().CompareTo(p1.Total_Stop());
                    if (compareDate == 0)
                    {
                        return p1.Time.CompareTo(p2.Time);
                    }
                    return compareDate;
                }
            );

            // Test Order print 
            System.Diagnostics.Debug.WriteLine("Testing Order of List");
            foreach (Error_list aPart in error_Lists)
            {
                System.Diagnostics.Debug.WriteLine(aPart.Total_Stop());
            }

            //Level of errors
            int Per_Level = error_Lists.Count/3;
            int Total_errors3 = 0;


            for ( int i = 0; i< Per_Level; i++)
            {
                error_Lists[i].Level = 3;
                Total_errors3++;
                
            }

            for (int i = Per_Level; i < (2 * Per_Level); i++)
            {
                error_Lists[i].Level = 2;

            }
            // Test Level print 
            System.Diagnostics.Debug.WriteLine("Testing Level of error");
            foreach (Error_list aPart in error_Lists)
            {
                System.Diagnostics.Debug.WriteLine(aPart.Level);
            }
            // Implementation of Email
            string Mail_from = "linqtestemail2022@gmail.com";
            string Mail_password = "SUNRISE2022";
            string Mail_to= "linqtestemail2022@gmail.com";
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(Mail_from);
                mail.To.Add(Mail_to);
                mail.Subject = "Test Sending mail";
                mail.Body = "<h1>  Informe de alarmas </h1> <br> <h2> El total de errores nivel 3 es " + Total_errors3 + "<h2>"; 
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com",587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(Mail_from,Mail_password);
                    smtp.Send(mail);
                   
                }
            }


        }
       
    }
}



