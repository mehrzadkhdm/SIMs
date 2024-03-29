using Newtonsoft.Json;
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

            
            string hash = "";
            using (SHA1 sha1Hash = SHA1.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(textBoxPass.Text.Trim());
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToLower();

            }
            var client = new RestClient("https://cdn.emnify.net")
            {
                Timeout = -1
            };
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
                    password = hash,
                }); ; // AddJsonBody serializes the object automatically
            // ""
            

            IRestResponse response = await client.ExecuteAsync(request);
            var content = response.Content;
            if (response.StatusCode != System.Net.HttpStatusCode.OK ) return;
            var json = JsonConvert.DeserializeObject<JObject>(content);
            string token = json["auth_token"].ToString();

            Hide();
            Tag = token;
            Form1 myForm = new Form1(this);
            myForm.Closed += (s, args) => Close();
            myForm.Show();
        }

        private void login_Load(object sender, EventArgs e)
        {

        }
    }
}
