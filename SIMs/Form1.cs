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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using MySql.Data.MySqlClient;




namespace SIMs
{
    public partial class Form1 : Form
    {
        public string token = "";
        //public List<string> serviceProfiles = new List<string>();
        public List<endpointResponse> endPoints = new List<endpointResponse>();
        public List<serviceProfileResponse> serviceProfiles = new List<serviceProfileResponse>();
        public Dictionary<int, string> idDict = new Dictionary<int, string>();
        public string newTime = "09:00:00";
        public Form loginForm;
        public List<simResponse> sims = new List<simResponse>();
        public string cs = @"server=rastreo911.com;userid=simsmx;password=Mexico2k20!!;database=em_sims";
        List<dataInfo> dataInfos = new List<dataInfo>();
        public Form1(Form form)
        {
            InitializeComponent();
            loginForm = form;
        }

        private async void Form1_LoadAsync(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(async (sc, ev) => await this.comboBox1_SelectedIndexChanged(sc, ev));
            this.buttonOk.Click += new System.EventHandler(async (sc, ev) => await this.buttonOk_Click(sc, ev));
            //string source = "x5Va5fVNv!Ptvut";
            //source = "Mexico123!";
            //string hash = "";
            //using (SHA1 sha1Hash = SHA1.Create())
            //{
            //    //From String to byte array
            //    byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            //    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
            //    hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToLower();

            //    //Console.WriteLine("The SHA1 hash of " + source + " is: " + hash);
            //}
            var client = new RestClient("https://cdn.emnify.net");
            client.Timeout = -1;
            token = loginForm.Tag.ToString();
            //token = json["auth_token"].ToString();

            //RestSharp.Authenticators.OAuth
            //

            //var sus = sims.Where(x => (x.status.description == "Suspended")).Select(x => x.iccid).ToList();

            //
            //  Read endpoints information
            //
            Cursor = Cursors.WaitCursor;
            RestRequest request = new RestRequest("api/v1/endpoint");
            request.Method = Method.GET;
            //request.AddHeader("Content-type", "application/json");
            //request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("accept", "application/json, text/plain, */*");
            IRestResponse response = await client.ExecuteAsync(request);
            string content = response.Content;
            endPoints = JsonConvert.DeserializeObject<List<endpointResponse>>(content);
            //endpo = endPoints.Select(s => s.service_profile.name).Distinct().ToList();


            
                ;
            //  Read SIM information
            //
            request = new RestRequest("api/v1/sim");
            request.Method = Method.GET;
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("aacept", "application/json, text/plain, */*");
            response = await client.ExecuteAsync(request);
            content = response.Content;
            sims = JsonConvert.DeserializeObject<List<simResponse>>(content);
            //
            // Read Service Profile
            //
            request = new RestRequest("api/v1/service_profile");
            request.Method = Method.GET;
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("aacept", "application/json, text/plain, */*");
            response = await client.ExecuteAsync(request);
            content = response.Content;
            serviceProfiles  = JsonConvert.DeserializeObject<List<serviceProfileResponse>>(content);
            //
            //
            //  Read database
            //
            await readDatabase();
            Cursor = Cursors.Default;
            //foreach (JObject myclient in clients)
            //{
            //    names.Add(myclient["service_profile"]["name"].ToString());
            //    //namesAdd(myclient["service_profile"]);
            //}
            //comboBox1.Items.AddRange(names.Distinct().ToArray());
            //JObject jObject = J
            //List<simResponse> sims = JsonConvert.DeserializeObject<List<simResponse>>(content);

        }

        private async Task readDatabase()
        {
            
            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                string sql = "SELECT * FROM em_sims.sims_data";
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                            dataInfos.Add(new dataInfo(rdr[0].ToString(), int.Parse(rdr[1].ToString()),
                                rdr[2].ToString(), rdr[3].ToString(), int.Parse(rdr[4].ToString()), DateTime.Parse(rdr[5].ToString())));
                    }
                }
            }
            
        }
        private async Task readDatabase2()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            string service = comboBox1.SelectedItem.ToString();
            var client = new RestClient("https://cdn.emnify.net");
            client.Timeout = -1;
            //?q = tags % 3Aarduino

            int id = endPoints.First(s => s.service_profile.name == service).service_profile.id;
            var request = new RestRequest("api/v1/endpoint?q=service_profile:" + id.ToString());
            request.Method = Method.GET;
            //request.AddHeader("Content-type", "application/json");
            //request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("accept", "application/json, text/plain, */*");
            IRestResponse response = await client.ExecuteAsync(request);
            string content = response.Content;
            List<endpointResponse> endpoints = JsonConvert.DeserializeObject<List<endpointResponse>>(content);

            JArray clients = JArray.Parse(content);





            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                string sql = "SELECT * FROM em_sims.sims_data WHERE serviceProfile_name ='" + comboBox1.SelectedItem.ToString() + "';";
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            dataGridView1.Rows.Add(rdr[0], rdr[3], (int.Parse(rdr[4].ToString()) == 1 ? "Activated" : "Suspended"), rdr[5].ToString());
                        }
                    }
                }



            }



        }
        private async Task comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            //await readDatabase();
        }

        private void toolStripMenuItemDate_Click(object sender, EventArgs e)
        {
            groupBoxDueDate.Visible = true;
            dataGridView1.Enabled = false;
            comboBox1.Enabled = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            groupBoxDueDate.Visible = false;
            dataGridView1.Enabled = true;
            comboBox1.Enabled = true;

        }

        private async Task buttonOk_Click(object sender, EventArgs e)
        {
            string cs = @"server=rastreo911.com;userid=simsmx;password=Mexico2k20!!;database=em_sims";

            groupBoxDueDate.Visible = false;
            List<string> selectedICCID = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                selectedICCID.Add(row.Cells[1].Value.ToString());
            }

            string ICCIDList = string.Join("','", selectedICCID.ToArray());
            using (var con = new MySqlConnection(cs))
            {
                // update database
                con.Open();
                DateTime dt = dateTimePicker1.Value;
                DateTime today = DateTime.Now;
                string status = (dt < today ? "Enabled" : "Disabled");
                string newTimeDate = string.Format("{0}-{1:00}-{2:00} {3}", dt.Year, dt.Month, dt.Day, newTime);
                string sql = "UPDATE  em_sims.sims_data set expires = '" + newTimeDate + "' WHERE iccid IN ('" + ICCIDList + "')";

                //string sql = "SELECT * FROM em_sims.sims_data WHERE serviceProfile_name ='" + comboBox1.SelectedItem.ToString() + "';";
                using (var cmd = new MySqlCommand(sql, con))
                {
                    int numUpdate = cmd.ExecuteNonQuery();
                    MessageBox.Show(this, string.Format("{0} records updated", numUpdate));
                    //using (MySqlDataReader rdr = cmd.ExecuteReader())
                    //{
                    //    while (rdr.Read())
                    //    {
                    //        dataGridView1.Rows.Add(rdr[0], rdr[3], (int.Parse(rdr[4].ToString()) == 1 ? "Enabled" : "Disabled"), rdr[5].ToString());
                    //        //Console.WriteLine(rdr[0] + " -- " + rdr[1]);
                    //    }
                    //}
                }
                //}

                // update website
            }

            await readDatabase2();



            dataGridView1.Enabled = true;
            comboBox1.Enabled = true;

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) newTime = radioButton1.Tag.ToString();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) newTime = radioButton2.Tag.ToString();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked) newTime = radioButton3.Tag.ToString();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked) newTime = radioButton4.Tag.ToString();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked) newTime = radioButton5.Tag.ToString();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked) newTime = radioButton6.Tag.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            today = today.AddDays(1);
            DateTime nextDue = today.AddMonths(1);
            if (today.Day < 25)
                nextDue = today.AddDays(25 - today.Day);
            else
                nextDue = today.AddMonths(1).AddDays(25 - today.Day);
            dateTimePicker1.Value = nextDue;
        }
        public class dataInfo
        {
            public string name { get; set; }
            public int serviceProfile_id { get; set; }
            public string serviceProfile_name { get; set; }
            public string ICCID { get; set; }
            public int status { get; set; }
            public DateTime expires { get; set; }
            public int flag { get; set; }

            public dataInfo(string n, int sid, string sn, string iccid, int st, DateTime exp)
            {
                name = n;
                serviceProfile_id = sid;
                serviceProfile_name = sn;
                ICCID = iccid;
                status = st;
                expires = exp;
                flag = 0;
            }





            //serviceProfile_name
            //serviceProfile_id
        }
    }
}
