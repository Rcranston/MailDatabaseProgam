using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data.SQLite;

namespace CSCDWPF
{
    /// <summary>
    /// Interaction logic for DBWindow.xaml
    /// Rcranston
    /// CSCD 371 Midterm
    /// this software will watch a selected directory and have the posiblity to save to a database
    /// </summary>

    public partial class DBWindow : Window
    {
        SQLiteConnection sql_con;
        SQLiteCommand sql_cmd;

        public DBWindow()
        {
            InitializeComponent();
            sql_con = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True;");
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
        }

        private void SubButtin_Click(object sender, RoutedEventArgs e)
        {
            if (USRFname.Text.Length == 0 && USRLname.Text.Length == 0 && USRemail.Text.Length == 0)
            { MessageBox.Show("Hmmm somthing doesnt look right, Check what you have entered...");
            }
            else
            {
                try
                {
                    sql_cmd.CommandText = $"INSERT OR REPLACE INTO FSW (LastName, FirstName, Email, Lmail) VALUES ('{USRLname.Text}','{USRFname.Text}','{USRemail.Text}','{USRprev.IsChecked}');";
                    sql_cmd.ExecuteNonQuery();
                    MessageBox.Show("Added!");
                }
                catch (Exception x)
                {
                    MessageBox.Show($"Invalid Entry, Try again\n { x.ToString()}");
                }
                USRLname.Text = "";
                USRFname.Text = "";
                USRemail.Text = "";
                USRprev.IsChecked = false;
            }
        }
    }
}
