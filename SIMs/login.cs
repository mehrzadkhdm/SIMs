﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SIMs
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //string source = "Mexico123!";
            
            string hash = "";
            using (SHA1 sha1Hash = SHA1.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(textBoxPass.Text.Trim());
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToLower();

                //Console.WriteLine("The SHA1 hash of " + source + " is: " + hash);
            }
            var client = new RestClient("https://cdn.emnify.net");
            client.Timeout = -1;
            var request = new RestRequest("api/v1/authenticate", DataFormat.Json);
            request.Method = Method.POST;
            //request.AddHeader("Content-type", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "Bearer null");
            request.AddHeader("aacept", "application/json, text/plain, */*");
            request.AddJsonBody(
                new
                {
                    username = textBoxUser.Text,
                    //username = "neidy.mtto@gmail.com",
                    password = hash,
                }); ; // AddJsonBody serializes the object automatically
            //request.AddJsonBody(
            //    new
            //    {
            //        username = "mehrzadkhdm@gmail.com",
            //        password = hash // x5Va5fVNv!Ptvut
            //    }); // AddJsonBody serializes the object automatically

            IRestResponse response = await client.ExecuteAsync(request);
            var content = response.Content;
            if (response.StatusCode != System.Net.HttpStatusCode.OK ) return;
            var json = JsonConvert.DeserializeObject<JObject>(content);
            string token = json["auth_token"].ToString();

            this.Hide();
            this.Tag = token;
            Form1 myForm = new Form1(this);
            myForm.Closed += (s, args) => this.Close();
            myForm.Show();
        }

        private void login_Load(object sender, EventArgs e)
        {

        }
    }
}
