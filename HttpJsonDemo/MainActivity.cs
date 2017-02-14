using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Net;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Java.Security.Cert;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace HttpJsonDemo
{
    [Activity(Label = "HttpJsonDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Button bt1;
        TextView tv1;
        TextView tv2;
        TextView tv3;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            bt1 = FindViewById<Button>(Resource.Id.button1);
            tv1 = FindViewById<TextView>(Resource.Id.textView1);
            tv2 = FindViewById<TextView>(Resource.Id.textView2);
            tv3 = FindViewById<TextView>(Resource.Id.textView3);
            bt1.Click += Bt1_Click;
        }

        private async void Bt1_Click(object sender, EventArgs e)
        {
            await FetchErrAsync("http://121.242.223.199/SEZOnlineWebService/SezOnlineWebService.svc/FetchNumberOfSEZandUnits/1");
        }
        public bool MyRemoteCertificateValidationCallback(System.Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
        private async Task FetchErrAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    //JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                    //return jsonDoc;
                    StreamReader reader = new StreamReader(stream);
                    string text = reader.ReadToEnd();
                    tv1.Text = text;
                    var myFetchNumberOfSEZandUnitsResultguage = JsonConvert.DeserializeObject<MyFetchNumberOfSEZandUnitsResultguage>(text);
                    tv2.Text = myFetchNumberOfSEZandUnitsResultguage.FetchNumberOfSEZandUnitsResult[0].Key;
                    tv3.Text = myFetchNumberOfSEZandUnitsResultguage.FetchNumberOfSEZandUnitsResult[0].Value;
                }
            }
        }


    }
    public class MyFetchNumberOfSEZandUnitsResultguage
    {
        public List<MyKeyValue> FetchNumberOfSEZandUnitsResult { get; set; }
    }

    public class MyKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

