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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Net.Mail;
using System.Net;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace CSCDWPF
{
    /// <summary>
    /// Interaction logic for EmailSender.xaml
    /// Rcranston
    /// CSCD 371 Final Project
    /// this software will hold a database and allow interactions then send mass emails, the current down side is this application does not work with 2-factor authentification.
    /// </summary>
    /// 
    public partial class EmailSender : Window
    {
        
        String sendTo="";
        SQLiteConnection sql_con;
        SQLiteCommand sql_cmd;
        SQLiteDataReader sql_red;

        public EmailSender()
        {
            sql_con = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True;");
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            InitializeComponent();
        }

        private void EMSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                buildTolist();
                if (sendTo.Length == 0 || EMservice.Text.Equals("Select One"))
                {
                    Console.WriteLine(sendTo + EMservice.Text);
                    MessageBox.Show("Check your entries");
                    return;
                }
                Console.WriteLine("to be sent to:"+sendTo.TrimEnd(','));
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(EMuser.Text),
                    Subject = EMHead.Text,
                    Body = EMmessage.Text
                };
                mailMessage.To.Add(sendTo.TrimEnd(','));
                if (!EMattatchment.Text.Contains("Want to attatch"))
                {
                    Console.WriteLine("Attempting attachment");
                    var attachment = new Attachment(EMattatchment.Text);
                    mailMessage.Attachments.Add(attachment);
                    Console.WriteLine("attachment Complete");
                }


                Console.WriteLine("Attempting to send");
                using (var smtpClient = new SmtpClient())
                {
                    switch (EMservice.Text)
                    {
                        case "Gmail":
                            smtpClient.Connect("smtp.gmail.com", 465, true);
                            break;
                        case "ICloud":
                            smtpClient.Connect("smtp.mail.me.com", 587, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
                            break;
                        case "Office365":
                            smtpClient.Connect("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
                            break;
                        case "Outlook":
                            smtpClient.Connect("smtp.outlook.com", 587, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
                            break;
                    }
                    smtpClient.Authenticate(EMuser.Text, EMpass.Text);
                    smtpClient.Send((MimeKit.MimeMessage)mailMessage);
                    smtpClient.Disconnect(true);
                }
                MessageBox.Show("Emails sent!");
            }
            catch (Exception x)
            {
                MessageBox.Show($"Uh oh something went wrong\n {x.ToString()}","UGH");
            }
        }
        private void buildTolist()
        {
            try
            {
                sendTo = "";
                Console.WriteLine("Beginning Coonnection");
                sql_cmd.CommandText = "SELECT * FROM FSW ORDER BY LastName";
                sql_red = sql_cmd.ExecuteReader();
                Console.WriteLine("Beginning Read");
                while (sql_red.Read()) // Read() returns true if there is still a result line to read
                {
                    Console.WriteLine(EMToSelct.Text);
                    if (EMToSelct.Text.Equals("All"))
                    {
                        sendTo = sendTo + $" " + sql_red["Email"] + ",";
                        Console.WriteLine(sendTo.TrimEnd(','));
                    }
                    else if (EMToSelct.Text.Equals("Those Who thought abot you Last Year") && (Boolean)sql_red["Lmail"])
                    {
                        sendTo = sendTo + $" " + sql_red["Email"] + ",";
                        Console.WriteLine(sendTo.TrimEnd(','));
                    }
                    else if (EMToSelct.Text.Equals("Those Who thought abot you Last Year") && !(Boolean)sql_red["Lmail"]) { }
                    else if (EMToSelct.Text.Equals("Select One"))
                    {
                        MessageBox.Show($"Please select and option and try again", "UGH");
                        break;
                    }
                    else if (EMToSelct.Text.Length > 0)
                    {
                        string x = EMToSelct.Text;
                        sendTo = sendTo + $" " + x + " ,";
                        Console.WriteLine(sendTo.TrimEnd(','));
                        break;
                    }
                }
                sql_red.Close();
                Console.WriteLine("Database read done");
            }
            catch (Exception x)
            {
                MessageBox.Show($"Invalid Entry, Try again\n {x.ToString()}");
                sql_red.Close();
            }
        }       
    }
}

