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
using Microsoft.Office.Interop.Excel;
using System.Timers;
using Windows.Foundation;





// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Try1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
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
        string tableAlarms = "test.alarms";
        string tableErrors = "test.newerror";
        private static Timer aTimer = new System.Timers.Timer(2 * 1000); // How often will the tasks be updated
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
        //private void CheckAlarms()
        //{
        //    aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

        //    aTimer.Start();
        //}

        //private void OnTimedEvent(object source, ElapsedEventArgs e)
        //{
        //    DateTimeOffset horaLectura = DateTimeOffset.Now;



        //    DispatcherQueue.TryEnqueue(() =>
        //    {
        //        upLoadData();
        //        AnalizeData();

        //    });
        //}
        static List<Errors_Excel> ReadExcel()
        {
            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            List<Errors_Excel> Excel_List = new List<Errors_Excel>();
            if (ExcelApp != null) {

                string path_excel = @"C:\Users\DataBox\source\repos\Error_List_Template.xlsx";
                
                Microsoft.Office.Interop.Excel.Workbook wb = ExcelApp.Workbooks.Open(path_excel
                    , 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet) wb.Sheets[1];
                Microsoft.Office.Interop.Excel.Range range = ws.UsedRange;
                int rowCount = range.Rows.Count;
                int colCount = range.Columns.Count;

                string Key_string = "";
                string Message = "";
                string Action = "";
                string Stop_Time_string = "";
             

                for (int i = 2; i <= rowCount; i++)
                {
                    Microsoft.Office.Interop.Excel.Range ExcelKey = (ws.Cells[i, 1] as Microsoft.Office.Interop.Excel.Range);
                    Key_string = ExcelKey.Value.ToString();
                    int Key = Int32.Parse(Key_string);
                   
                    Microsoft.Office.Interop.Excel.Range ExcelMessage = (ws.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range);
                    Message = ExcelMessage.Value.ToString();
                    Microsoft.Office.Interop.Excel.Range ExcelAction = (ws.Cells[i, 3] as Microsoft.Office.Interop.Excel.Range);
                    Action = ExcelAction.Value.ToString();
                    Microsoft.Office.Interop.Excel.Range ExcelTime = (ws.Cells[i, 4] as Microsoft.Office.Interop.Excel.Range);
                    Stop_Time_string = ExcelTime.Value.ToString();
                    int Stop_Time = Int32.Parse(Stop_Time_string);

                    Excel_List.Add(new Errors_Excel() { Key = Key, Message = Message, Action = Action, Stop_Time = Stop_Time });
                }
            }


            return Excel_List;
        }

        public void upLoadData()
        {
            
            //var item = (sender as listview).selecteditem as error;
          

            List<Errors_Excel> Excel_List = ReadExcel();
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
                       
                        cmd.CommandText = "select * from " + tableAlarms;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                    
                                Error error = new Error();
                                error.ID = reader.GetInt32(0);
                                error.Key = reader.GetInt32(1);
                                error.Date= reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss");   
                                System.Diagnostics.Debug.WriteLine("FECHA:"+error.Date);

                                for (int i =0; i< Excel_List.Count; i++)
                                {

                                    if (error.Key == Excel_List[i].Key)
                                    {
                                        error.Message=Excel_List[i].Message;
                                        error.Action = Excel_List[i].Action;
                                        error.Time = Excel_List[i].Stop_Time;

                                    }
  
                                }

                                Alarms_list.Add(error);
  
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


           for( int j = 0; j< Alarms_list.Count; j++) 
            { 
            try
            {
               
                string MyConnection2 = "datasource=localhost;port=3306;username=root;password=SUNRISE";
                string query= "insert into test.newerror(iderror,message,level,action,time,Date,Code) " +
                    "values('" + Alarms_list[j].ID + "','" + Alarms_list[j].Message + "', '0','" + Alarms_list[j].Action + "','" + Alarms_list[j].Time + "','" + Alarms_list[j].Date + "','" + Alarms_list[j].Key + "');";
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
                MySqlDataReader MyReader2;

                MyConn2.Open();
                MyReader2 = MyCommand2.ExecuteReader();
                while (MyReader2.Read()) { System.Diagnostics.Debug.WriteLine("EL PROGRAMA ENTRA AL WHILE"); }
                MyConn2.Close();

            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("error en AÑADIR datos");
            }
            }
        }



        public void AnalizeData()
        {
            //List<Errors_Excel> Excel_List = ReadExcel();
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



                        cmd.CommandText = "select * from " + tableErrors;
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
                                if (found == false)
                                {
                                    // Temporary code counter, should be added from the SQL data base
                                    Code_count++;
                                    error_Lists.Add(new Error_list() { Message = reader.GetString(1), Time = reader.GetDouble(4), Count = 1, Level = 1, Code = reader.GetInt32(6) });
                                }


                                ;
                            }
                            ListViewErrors.ItemsSource = error_Lists;



                           
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
            (double x , double y) = CircunferencePoints(Availability);
            System.Diagnostics.Debug.WriteLine("Availability =" + Availability + "%");
            Availability_TextBlock.Text = Availability.ToString(".##"+"%");
            var loc = new Windows.Foundation.Point(x, y);
            testchartOEE.StartPoint = loc;
            


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
            SendMail(error_Lists);
        }

        private static void SendMail(List<Error_list> List)
        {
            // Implementation of Email using system.net.mail for google
            int Per_Level = List.Count/3;
            int Total_errors3 = 0;


            for ( int i = 0; i< Per_Level; i++)
            {
                List[i].Level = 3;
                Total_errors3++;
                
            }
            double Stop_Time = 0;
             foreach (Error_list aPart in List)
            {
                Stop_Time = Stop_Time + aPart.Total_Stop();
                aPart.Total = aPart.Total_Stop();
            }

            string Mail_from = "linqtestemail2022@gmail.com";
            string Mail_password = "ekodioyteoizjpju";
            string Mail_to = "linqtestemail2022@gmail.com";
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(Mail_from);
                mail.To.Add(Mail_to);
                mail.Subject = "Test Sending mail";
               
                mail.Body = "<h1 align='center'>  Informe de alarmas </h1>  <br> <h2> El total de errores nivel 3 es: " + Total_errors3 +" <h2> <h2> El tiempo de parada total es: "+Stop_Time+" mins </h2><br>";
                int counter = 1;
                   foreach (Error_list aPart in List)
                   {
                    mail.Body= mail.Body +"<h4>"+counter +". " + aPart.Message +", Tiempo de parada de maquina es : "+aPart.Total+" mins <h4>";   
                    counter++;
                   }
                mail.Body = mail.Body + "<br> <h2> Saludos,<h2> <h2>Linq Case <h2>";
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(Mail_from, Mail_password);
                    smtp.Send(mail);

                }
            }
        }

        private double Quadratic(double a, double b, double c, Boolean pos)
        {
            //Function to calculate quadratic equations
            double x = 0;
            if (pos)
                x = ((-b + (double)(Math.Sqrt((b * b) - (4 * a * c)))) / (2 * a));
            else
                x = ((-(b) - (double)(Math.Sqrt(Math.Pow(b, 2) - (4 * a * c)))) / (2 * a));

            return x;
        }
        private (double, double) CircunferencePoints(double Percentage)
        {

            
            double x = 0;
            double y = 0;
            double angle = 0;
            double b = 0;
            double c = 0;
            double a = 0;
            bool pos = false;
            var adjust = new Size(200, 200);
            angle = Percentage * 2 * 3.1415;
            if (Percentage <= 0) { Percentage = 0; }
            if (Percentage > 1) { Percentage = 0; }

            if (Percentage <= 0.25)
            {
                x = (Math.Sin(angle) * 200) + 1250;
                c = (Math.Pow(x, 2)) - 2500 * x + 1562500 + 62500 - 40000;
                b = -500;
                a = 1;
                pos = false;
                y = Quadratic(a, b, c, pos);
                DirectionOEE.IsLargeArc = false;


            }
            else if (Percentage <= 0.50)
            {
                x = (Math.Sin(angle) * 200) + 1250;
                c = (Math.Pow(x, 2)) - 2500 * x + 1562500 + 62500 - 40000;
                b = -500;
                a = 1;
                pos = true;
                y = Quadratic(a, b, c, pos);
                DirectionOEE.IsLargeArc = false;
                if (Percentage > 0.45)
                {
                    adjust = new Size(200.5, 200.5);
                }

                //Direction1.Size = adjust;

            }
            else if (Percentage <= 0.75)
            {
                x = (Math.Sin(angle) * 200) + 1250;
                c = (Math.Pow(x, 2)) - 2500 * x + 1562500 + 62500 - 40000;
                b = -500;
                a = 1;
                pos = true;
                y = Quadratic(a, b, c, pos);
                DirectionOEE.IsLargeArc = true;
                adjust = new Size(148, 148);
                if (Percentage < 0.55)
                {
                    adjust = new Size(200.5, 200.5);
                }
                //Direction1.Size = adjust;

            }
            else
            {
                x = (Math.Sin(angle) * 200) + 1250;
                c = (Math.Pow(x, 2)) - 2500 * x + 1562500 + 62500 - 40000;
                b = -500;
                a = 1;
                pos = false;
                y = Quadratic(a, b, c, pos);
                DirectionOEE.IsLargeArc = true;

            }

            return (x, y);
        }

    }
}



