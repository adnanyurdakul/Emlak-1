using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SendSms
{
    class SmsSend
    {
        // main metodu         
        [STAThread]
        static void Main1(string[] args)
        {
            try
            {
                // Kullanici adi, parola ve Originator kullanilarak bir sms paketi olusturulur.
                SMSPaketi smspak = new SMSPaketi("username", "parolam", "MUTLUCELL");

                // eger ileri tarihli sms gonderilecekse tarh parametreli asagidai Consturctor kullanilabilir
                // ornek: 2066-11-20 saat 19:30:00 'da gonder
                //SMSPaketi smspak = new SMSPaketi("user","123456","MUTLUCELL", new DateTime(2006,11,20,19,30,0));

                // mesajin gidecegi numaralar bir array'e doldurulur
                // numara formati onemli degildir, bosluklu parantezli, sifirli, sifirsiz, +90li vs olabilir
                String[] numaralar = { "533 444 55 66", "05324445555", "+90505 7156789" };

                // gidecek mesaj metni ve numaralar pakaete eklenir. 
                // bu sekilde bir sms paketine birden fazla mesaj eklenebilir
                smspak.addSMS("Merhaba. Bu bir denemedir.", numaralar);

                // sonuc eger mesaj basarili ise # ile baslayan bir mesaj ID'dir. 
                // bir hata olusmussa XML dokumaninda belirtilen hata kodlarindan biri doner
                String sonuc = smspak.gonder();
                MessageBox.Show(sonuc);

                //Raporun cekilmesi
                // rapor kullanici adi, parola ve mesaj gonderme isleminde sonuc olarak donen 
                // message ID ile cekilir. XML dokumaninda belirtilen formatta doner
                String rapor = SMSPaketi.rapor("username", "password", 156675);

                MessageBox.Show(rapor);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex);
            }
        }
    }

    public class SMSPaketi
    {
        public SMSPaketi(String ka, String parola, String org)
        {
            start += "<smspack ka=\"" + xmlEncode(ka) + "\" pwd=\"" + xmlEncode(parola)
                    + "\" org=\"" + xmlEncode(org) + "\">";

        }

        public SMSPaketi(String ka, String parola, String org, DateTime tarih)
        {
            start += "<smspack ka=\"" + xmlEncode(ka) + "\" pwd=\"" + xmlEncode(parola) +
                    "\" org=\"" + xmlEncode(org) + "\" tarih=\"" + tarihStr(tarih) + "\">";

        }

        public void addSMS(String mesaj, String[] numaralar)
        {
            body += "<mesaj><metin>";
            body += xmlEncode(mesaj);
            body += "</metin><nums>";
            foreach (String s in numaralar)
            {
                body += xmlEncode(s) + ",";
            }
            body += "</nums></mesaj>";
        }

        public String xml()
        {
            if (body.Length == 0)
                throw new ArgumentException("SMS paketinede sms yok!");
            return start + body + end;
        }

        public String gonder()
        {
            WebClient wc = new WebClient();

            string postData = xml();
            wc.Headers.Add("Content-Type", "text/xml; charset=UTF-8");
            // Apply ASCII Encoding to obtain the string as a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            byte[] responseArray = wc.UploadData("https://smsgw.mutlucell.com/smsgw-ws/sndblkex", "POST", byteArray);
            String response = Encoding.UTF8.GetString(responseArray);
            return response;
        }

        /**
         * ka = kullanici adi
         * parola
         * id: gonderim sonucu donen paket ID
         * 
         */
        public static String rapor(String ka, String parola, long id)
        {
            String xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<smsrapor ka=\"" + ka + "\" pwd=\"" + parola + "\" id=\"" + id + "\" />";
            WebClient wc = new WebClient();
            MessageBox.Show(xml);
            string postData = xml;
            wc.Headers.Add("Content-Type", "text/xml; charset=UTF-8");
            // Apply ASCII Encoding to obtain the string as a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            byte[] responseArray = wc.UploadData("https://smsgw.mutlucell.com/smsgw-ws/gtblkrprtex", "POST", byteArray);
            String response = Encoding.UTF8.GetString(responseArray);
            return response;
        }


        private static String tarihStr(DateTime d)
        {
            return xmlEncode(d.ToString("yyyy-MM-dd HH:mm"));
        }

        private static String xmlEncode(String s)
        {
            s = s.Replace("&", "&amp;");
            s = s.Replace("<", "&lt;");
            s = s.Replace(">", "&gt;");
            s = s.Replace("'", "&apos;");
            s = s.Replace("\"", "&quot;");
            return s;
        }

        private String start = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
        private String end = "</smspack>";
        private String body = "";
    }
}

 