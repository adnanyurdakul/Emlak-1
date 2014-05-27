using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendSms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {       
            string message = txtMessage.Text;

            SMSPaketi sms = new SMSPaketi("teknobilsoft", "klavye38", "aliozturk");
            sms.addSMS(message, numara);
            sms.gonder();            
        }
       
        string[] numara = new string[5];
        int sayac = 0;
        private void btnAddNumber_Click(object sender, EventArgs e)
        {
            numara[sayac] = txtPhone.Text;
            sayac++;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }
    }
}
