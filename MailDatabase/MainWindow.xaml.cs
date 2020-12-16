using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Security.Permissions;
using System.Data.SQLite;
using System.Security.AccessControl;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace CSCDWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Rcranston
    /// CSCD 371 Final Project
    /// this software will hold a database and allow interactions then send mass emails, the current down side is this application does not work with 2-factor authentification.
    /// </summary>
   [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class MainWindow : Window
    {
        private DBWindow dbWindow;
        private EmailSender esWindow;
        private int num = 0;

        SQLiteConnection sql_con;
        SQLiteCommand sql_cmd;
        SQLiteDataReader sql_red;



        public MainWindow()
        {
            sql_con = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True;");
            sql_con.Open();
            sql_cmd= sql_con.CreateCommand();
            sql_cmd.CommandText = "CREATE TABLE if not exists FSW (id integer primary key, LastName varchar(100), FirstName varchar(100), Email varchar(100), Lmail boolean);";
            sql_cmd.ExecuteNonQuery();

            
            InitializeComponent();
            TableRefresh();
        }

        private void mnuFileExit_Click(object sender, RoutedEventArgs e)
        {   
                const string message ="Are you sure you want to Quit?";
                const string caption = "Howdy!";
                var result = MessageBox.Show(message, caption,
                                             (MessageBoxButton)MessageBoxButtons.YesNo,
                                             (MessageBoxImage)MessageBoxIcon.Question);
                if ((int)result != 7)
                {
                    sql_con.Close();
                    Environment.Exit(0);
                }
        }

        private void mnuHelpAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mail Database is a adress holder that will send mass emails, \nBased on the .Net Framework and developed using WPF \nConstructed by Ryan Cranston");
        }

        private void mnuFileDBWindow_Click(object sender, RoutedEventArgs e)
        {
            this.dbWindow = new DBWindow();
            this.dbWindow.ShowDialog();
        }


        private void StDButton_Click(object sender, RoutedEventArgs e)
        {
            for(int z=0;z<num;z++)
            {
                Object x = FSEtable.Items.GetItemAt(z);
                sql_cmd.CommandText = $"INSERT OR REPLACE INTO FSW (LastName, FirstName, Email, Lmail) VALUES ('{x.ToString().Split(',')[0].Substring(10)}','{x.ToString().Split(',')[1].Substring(10)}','{x.ToString().Split(',')[2].Substring(9)}','{x.ToString().Split(',')[3].Substring(8, (x.ToString().Split(',')[3].Substring(8).Length) - 1)}');";
                sql_cmd.ExecuteNonQuery();
            }
        }

        private void mnuClearDB(object sender, RoutedEventArgs e)
        {
            const string message = "Are you sure youd like to clear the database?";
            const string caption = "WARNING";
            var result = MessageBox.Show(message, caption,
                                         (MessageBoxButton)MessageBoxButtons.YesNo,
                                         (MessageBoxImage)MessageBoxIcon.Question);
            if ((int)result != 7)
            {
                sql_cmd.CommandText = "drop table if exists FSW";
                sql_cmd.ExecuteNonQuery();
                sql_cmd.CommandText = "CREATE TABLE if not exists FSW (id integer primary key, LastName varchar(100), FirstName varchar(100), Email varchar(100), Lmail boolean);";
                sql_cmd.ExecuteNonQuery();
            }
            TableRefresh();
        }

        private void NAddressButton_Click(object sender, RoutedEventArgs e)
        {
            this.dbWindow = new DBWindow();
            this.dbWindow.ShowDialog();
            TableRefresh();
        }

        public void TableRefresh()
        {
            FSEtable.Items.Clear();
            try
            {
                Console.WriteLine("Beginning Coonnection");
                sql_cmd.CommandText = "SELECT * FROM FSW ORDER BY LastName";
                sql_red = sql_cmd.ExecuteReader();

                Console.WriteLine("Beginning Read");
                while (sql_red.Read()) // Read() returns true if there is still a result line to read
                {
                    FSEtable.Items.Add(new { First = sql_red["LastName"], Second = sql_red["FirstName"], Third = sql_red["Email"], Four = sql_red["Lmail"] });
                }
                sql_red.Close();
                SRbox.Text = "Search by Last Name";
            }
            catch (Exception x)
            {
                MessageBox.Show($"Invalid Entry, Try again\n {x.ToString()}");
                sql_red.Close();
            }
        }
        private void mnuRefresh_Click(object sender, RoutedEventArgs e)
        {
            TableRefresh();
        }

        private void SRbutton_Click(object sender, RoutedEventArgs e)
        {
            if (SRbox.Text.Length == 0)
                TableRefresh();
            FSEtable.Items.Clear();
            try
            {
                Console.WriteLine("Beginning Coonnection");
                sql_cmd.CommandText = $"SELECT * FROM FSW WHERE LastName LIKE('{SRbox.Text}%') ORDER BY LastName";
                sql_red = sql_cmd.ExecuteReader();

                Console.WriteLine("Beginning Read");
                while (sql_red.Read()) // Read() returns true if there is still a result line to read
                {
                    FSEtable.Items.Add(new { First = sql_red["LastName"], Second = sql_red["FirstName"], Third = sql_red["Email"], Four = sql_red["Lmail"] });
                }
                sql_red.Close();
            }
            catch (Exception x)
            {
                MessageBox.Show($"Invalid Entry, Try again\n {x.ToString()}");
                sql_red.Close();
            }
        }

        private void nEmailButton_Click(object sender, RoutedEventArgs e)
        {
            this.esWindow = new EmailSender();
            this.esWindow.ShowDialog();
            TableRefresh();
        }
    }
}

