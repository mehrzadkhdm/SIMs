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
        public List<endpointResponse> endPoints = new();
        public List<serviceProfileResponse> serviceProfiles = new();
        public Dictionary<int, string> idDict = new();
        public string newTime = "00:00:00";
        public Form loginForm;
        public RestClient client = new("https://cdn.emnify.net");
        public List<simResponse> sims = new();
        public string cs = @"server=rastreo911.com;userid=simsmx;password=Mexico2k20!!;database=em_sims";
        List<dataInfo> dataInfos = new();
        public DataGridViewCellStyle disabledSIMCellStyle = new ();
        
        public Form1(Form form)
        {
            InitializeComponent();
            loginForm = form;
        }

        private async void Form1_LoadAsync(object sender, EventArgs e)
        {
            comboBox1.SelectedIndexChanged += new EventHandler(async (sc, ev) => await comboBox1_SelectedIndexChanged(sc, ev));
            buttonOk.Click += new EventHandler(async (sc, ev) => await buttonOk_Click(sc, ev));
            disabledSIMCellStyle.ForeColor = Color.Red;
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

            client = new RestClient("https://cdn.emnify.net")
            {
                Timeout = -1
            };
            token = loginForm.Tag.ToString();

            //token = json["auth_token"].ToString();

            //RestSharp.Authenticators.OAuth
            //

            //var sus = sims.Where(x => (x.status.description == "Suspended")).Select(x => x.iccid).ToList();

            //
            //  Read endpoints information
            //
            Cursor = Cursors.WaitCursor;
            // x-count-per-page: 500 
            // x-current-page: 3 
            // x-total-count: 1092
            // x-total-pages: 3 
            // https://cdn.emnify.net/api/v1/endpoint?page=3&per_page=500
            //
            int itemsPerPage = 1000;
            int pages = 1;
            //RestRequest request = new RestRequest(string.Format("api/v1/endpoint?page={0}&per_page={1}", 1, itemsPerPage));
            RestRequest request = new RestRequest($"api/v1/endpoint?page=1&per_page={itemsPerPage}")
            {
                Method = Method.GET
            };
            //request.AddHeader("Content-type", "application/json");
            //request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("accept", "application/json, text/plain, */*");
            IRestResponse response = await client.ExecuteAsync(request);

            string content = response.Content;
            endPoints = JsonConvert.DeserializeObject<List<endpointResponse>>(content);
            //
            //int kk = int.Parse(response.Headers.Where(r => r.Name.ToLower() == "x-total-pages").Select(r => r.Value.ToString()).FirstOrDefault());

            foreach (var p in response.Headers)
            {
                if (p.Name.ToLower() == "x-total-pages")
                {
                    pages = int.Parse(p.Value.ToString());
                }
            }
            for (int page = 2; page <= pages; page++)
            {
                //request = new RestRequest(string.Format("api/v1/endpoint?page={0}&per_page={1}", page, itemsPerPage));
                request = new RestRequest($"api/v1/endpoint?page={page}&per_page={itemsPerPage}")
                {
                    Method = Method.GET
                };
                //request.AddHeader("Content-type", "application/json");
                //request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("accept", "application/json, text/plain, */*");
                response = await client.ExecuteAsync(request);
                content = response.Content;
                endPoints.AddRange(JsonConvert.DeserializeObject<List<endpointResponse>>(content));
            }
            //endpo = endPoints.Select(s => s.service_profile.name).Distinct().ToList();



            ;
            //  Read SIM information
            //
            request = new RestRequest($"api/v1/sim?page=1&per_page={itemsPerPage}")
            {
                Method = Method.GET
            };
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("aacept", "application/json, text/plain, */*");
            response = await client.ExecuteAsync(request);
            content = response.Content;
            sims = JsonConvert.DeserializeObject<List<simResponse>>(content);
            foreach (var p in response.Headers)
            {
                if (p.Name.ToLower() == "x-total-pages")
                {
                    pages = int.Parse(p.Value.ToString());
                }
            }
            for (int page = 2; page <= pages; page++)
            {
                request = new RestRequest($"api/v1/sim?page={page}&per_page={itemsPerPage}")
                {
                    Method = Method.GET
                };
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("accept", "application/json, text/plain, */*");
                response = await client.ExecuteAsync(request);
                content = response.Content;
                sims.AddRange(JsonConvert.DeserializeObject<List<simResponse>>(content));
            }




            //
            // Read Service Profile
            //
            request = new RestRequest("api/v1/service_profile")
            {
                Method = Method.GET
            };
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("aacept", "application/json, text/plain, */*");
            response = await client.ExecuteAsync(request);
            content = response.Content;
            serviceProfiles = JsonConvert.DeserializeObject<List<serviceProfileResponse>>(content);

            //
            //
            //  Read database
            //
            await ReadDatabase();
            Cursor = Cursors.Default;
            pictureBox2.Visible = false;

            comboBox1.Items.AddRange(serviceProfiles.Select(x => x.name).ToArray());


        }

        private async Task ReadDatabase()
        {
            dataInfos = new List<dataInfo>();
            using var con = new MySqlConnection(cs);
            con.Open();
            string sql = "SELECT * FROM em_sims.sims_data";
            using (var cmd = new MySqlCommand(sql, con))
            {
                cmd.ExecuteNonQuery();

                using MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                    dataInfos.Add(new dataInfo(rdr[3].ToString(), rdr[4].ToString(), DateTime.Parse(rdr[5].ToString())));
            }

        }

        //private async Task readDatabase2()
        //{
        //    dataGridView1.Rows.Clear();
        //    dataGridView1.Refresh();
        //    string service = comboBox1.SelectedItem.ToString();
        //    client = new RestClient("https://cdn.emnify.net");
        //    client.Timeout = -1;
        //    //?q = tags % 3Aarduino

        //    int id = endPoints.First(s => s.service_profile.name == service).service_profile.id;
        //    var request = new RestRequest("api/v1/endpoint?q=service_profile:" + id.ToString());
        //    request.Method = Method.GET;
        //    //request.AddHeader("Content-type", "application/json");
        //    //request.RequestFormat = DataFormat.Json;
        //    request.AddHeader("Authorization", "Bearer " + token);
        //    request.AddHeader("accept", "application/json, text/plain, */*");
        //    IRestResponse response = await client.ExecuteAsync(request);
        //    string content = response.Content;
        //    List<endpointResponse> endpoints = JsonConvert.DeserializeObject<List<endpointResponse>>(content);

        //    JArray clients = JArray.Parse(content);

        //    using (var con = new MySqlConnection(cs))
        //    {
        //        con.Open();
        //        string sql = "SELECT * FROM em_sims.sims_data WHERE serviceProfile_name ='" + comboBox1.SelectedItem.ToString() + "';";
        //        using (var cmd = new MySqlCommand(sql, con))
        //        {
        //            cmd.ExecuteNonQuery();

        //            using (MySqlDataReader rdr = cmd.ExecuteReader())
        //            {

        //                while (rdr.Read())
        //                {
        //                    dataGridView1.Rows.Add(rdr[0], rdr[3], rdr[4].ToString(), rdr[5].ToString());
        //                }
        //            }
        //        }
        //    }
        //}
        private async Task updateDataGridView(bool updateSIM)
        {
            dataGridView1.Rows.Clear();
            // update SIM if necessary
            //  Read SIM information
            //
            if (updateSIM)
            {
                RestRequest request;
                request = new RestRequest("api/v1/sim");
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("aacept", "application/json, text/plain, */*");
                IRestResponse response = await client.ExecuteAsync(request);
                string content = response.Content;
                sims = JsonConvert.DeserializeObject<List<simResponse>>(content);
            }
            //
            //   
            //
            if (comboBox1.Text == string.Empty) return;
            int id = serviceProfiles.Where(x => x.name == comboBox1.Text).Select(x => x.id).First();
            List<endpointResponse> points = endPoints.Where(x => x.service_profile.id == id).Select(x => x).ToList();
            List<string> newSims_Activated = new();
            List<string> newSims_Suspended = new();
            int order = 1;

            foreach (endpointResponse endpoint in points)
            {
                if (endpoint.sim != null)
                {
                    List<DateTime> dates = dataInfos.Where(x => x.ICCID == endpoint.sim.iccid).Select(x => x.expires).ToList();
                    string simStatus = sims.Where(x => x.id == endpoint.sim.id).Select(x => x.status.description).First();
                    string dateString = (dates.Count == 0 ? "" : dates.First().ToString());
                    if (dateString == "")
                    {
                        if (simStatus == "Activated")
                        {
                            dateString = nextDue(1).ToString();
                            newSims_Activated.Add(endpoint.sim.iccid);
                        }
                        if (simStatus == "Suspended")
                        {
                            dateString = nextDue(-1).ToString();
                            newSims_Suspended.Add(endpoint.sim.iccid);
                        }
                    }
                    var rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    //disabledSIMCellStyle.
                    row.Cells[0].Value = order++;
                    row.Cells[1].Value = endpoint.name;
                    row.Cells[2].Value = endpoint.sim.iccid;
                    row.Cells[3].Value = simStatus;
                    row.Cells[4].Value = dateString;

                    //dataGridView1.Rows.Add(row);
                    if(simStatus == "Suspended")
                    {
                       
                    }
                }
                else
                {
                    dataGridView1.Rows.Add(order++, endpoint.name, "",
                        "No SIM",
                        "");
                }
            }
            if (newSims_Activated.Count > 0)
            {
                using var con = new MySqlConnection(cs);
                con.Open();
                DateTime dt = nextDue(1);
                string newTimeDate = string.Format("{0}-{1:00}-{2:00} {3}", dt.Year, dt.Month, dt.Day, newTime);
                foreach (string iccid in newSims_Activated)
                {
                    //string sql = "INSERT INTO  em_sims.sims_data (), set expires = '" + newTimeDate + "' WHERE iccid IN ('" + ICCIDList + "')";
                    string sql = string.Format("INSERT INTO  em_sims.sims_data (ICCID, status, expires) VALUES ('{0}', 'Activated', '{1}')", iccid, newTimeDate);

                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        int numUpdate = cmd.ExecuteNonQuery();
                    }
                }
            }
            if (newSims_Suspended.Count > 0)
            {
                using (var con = new MySqlConnection(cs))
                {
                    con.Open();
                    DateTime dt = nextDue(-1);
                    string newTimeDate = string.Format("{0}-{1:00}-{2:00} {3}", dt.Year, dt.Month, dt.Day, newTime);
                    foreach (string iccid in newSims_Suspended)
                    {
                        string sql = string.Format("INSERT INTO  em_sims.sims_data (ICCID, status, expires) VALUES ('{0}', 'Suspended', '{1}')", iccid, newTimeDate);

                        using (var cmd = new MySqlCommand(sql, con))
                        {
                            int numUpdate = cmd.ExecuteNonQuery();
                        }
                    }

                }
            }
            //await readDatabase();
        }


        private async Task comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            await updateDataGridView(false);


            //dataGridView1.Rows.Clear();

            //int id = serviceProfiles.Where(x => x.name == comboBox1.Text).Select(x => x.id).First();
            //List<endpointResponse> points = endPoints.Where(x => x.service_profile.id == id).Select(x => x).ToList();
            //List<string> newSims_Activated = new List<string>();
            //List<string> newSims_Suspended = new List<string>();
            //foreach (endpointResponse endpoint in points)
            //{
            //    List<DateTime> dates = dataInfos.Where(x => x.ICCID == endpoint.sim.iccid).Select(x => x.expires).ToList();
            //    string simStatus = sims.Where(x => x.id == endpoint.sim.id).Select(x => x.status.description).First();
            //    string dateString = (dates.Count == 0 ? "" : dates.First().ToString());
            //    if (dateString == "")
            //    {
            //        if (simStatus == "Activated")
            //        {
            //            dateString = nextDue(1).ToString();
            //            newSims_Activated.Add(endpoint.sim.iccid);
            //        }
            //        if (simStatus == "Suspended")
            //        {
            //            dateString = nextDue(-1).ToString();
            //            newSims_Suspended.Add(endpoint.sim.iccid);
            //        }
            //    }

            //    dataGridView1.Rows.Add(endpoint.name, endpoint.sim.iccid,
            //        simStatus,
            //        dateString);
            //}
            //if (newSims_Activated.Count > 0)
            //{
            //    using (var con = new MySqlConnection(cs))
            //    {
            //        con.Open();
            //        DateTime dt = nextDue(1);
            //        string newTimeDate = string.Format("{0}-{1:00}-{2:00} {3}", dt.Year, dt.Month, dt.Day, newTime);
            //        foreach (string iccid in newSims_Activated)
            //        {
            //            //string sql = "INSERT INTO  em_sims.sims_data (), set expires = '" + newTimeDate + "' WHERE iccid IN ('" + ICCIDList + "')";
            //            string sql = string.Format("INSERT INTO  em_sims.sims_data (ICCID, status, expires) VALUES ('{0}', 'Activated', '{1}')", iccid, newTimeDate);

            //            using (var cmd = new MySqlCommand(sql, con))
            //            {
            //                int numUpdate = cmd.ExecuteNonQuery();
            //            }
            //        }

            //    }
            //}
            //if (newSims_Suspended.Count > 0)
            //{
            //    using (var con = new MySqlConnection(cs))
            //    {
            //        con.Open();
            //        DateTime dt = nextDue(-1);
            //        string newTimeDate = string.Format("{0}-{1:00}-{2:00} {3}", dt.Year, dt.Month, dt.Day, newTime);
            //        foreach (string iccid in newSims_Suspended)
            //        {
            //            string sql = string.Format("INSERT INTO  em_sims.sims_data (ICCID, status, expires) VALUES ('{0}', 'Suspended', '{1}')", iccid, newTimeDate);

            //            using (var cmd = new MySqlCommand(sql, con))
            //            {
            //                int numUpdate = cmd.ExecuteNonQuery();
            //            }
            //        }

            //    }
            //}
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
            //string cs = @"server=rastreo911.com;userid=simsmx;password=Mexico2k20!!;database=em_sims";
            Cursor = Cursors.WaitCursor;
            pictureBox2.Visible = true;
            groupBoxDueDate.Visible = false;
            List<string> activateList = new List<string>();
            List<string> suspendList = new List<string>();
            DateTime dt = dateTimePicker1.Value;
            DateTime today = DateTime.Now;
            string status = (dt < today ? "Enabled" : "Disabled");
            string newTimeDate = string.Format("{0}-{1:00}-{2:00} {3}", dt.Year, dt.Month, dt.Day, newTime);

            string iccid = "";
            List<string> selectedICCID = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                iccid = row.Cells[1].Value.ToString();
                selectedICCID.Add(iccid);
                if (dt < today)   // suspend
                {
                    if (row.Cells[2].Value.ToString() == "Activated")
                    {
                        suspendList.Add(iccid);
                    }
                }
                else // Activate
                {

                    if (row.Cells[2].Value.ToString() == "Suspended")
                    {
                        activateList.Add(iccid);
                    }
                }
            }

            string ICCIDList = string.Join("','", selectedICCID.ToArray());
            using (var con = new MySqlConnection(cs))
            {
                // update database
                con.Open();
                string sql = "UPDATE  em_sims.sims_data set expires = '" + newTimeDate + "' WHERE iccid IN ('" + ICCIDList + "')";

                //string sql = "SELECT * FROM em_sims.sims_data WHERE serviceProfile_name ='" + comboBox1.SelectedItem.ToString() + "';";
                using (var cmd = new MySqlCommand(sql, con))
                {
                    int numUpdate = cmd.ExecuteNonQuery();
                    //MessageBox.Show(this, string.Format("{0} records updated", numUpdate));
                }

                // update website
            }
            await ReadDatabase();
            //
            //  Update server
            //
            client.Timeout = -1;
            //
            //
            RestRequest request;
            foreach (string sim in activateList)
            {
                int ac_id = sims.Find(x => x.iccid == sim).id;
                request = new RestRequest("api/v1/sim")
                {
                    Method = Method.PATCH
                };
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("aacept", "application/json, text/plain, */*");
                request.RequestFormat = DataFormat.Json;

                request.Resource = string.Format("api/v1/sim/{0}", ac_id);
                request.AddJsonBody(
                    new
                    {
                        status = new { id = 1 }
                    });
                IRestResponse patchResponse = await client.ExecuteAsync(request);
                //string content = response.Content;

            }
            foreach (string sim in suspendList)
            {
                int su_id = sims.Find(x => x.iccid == sim).id;

                request = new RestRequest("api/v1/sim")
                {
                    Method = Method.PATCH
                };
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("aacept", "application/json, text/plain, */*");
                request.RequestFormat = DataFormat.Json;


                request.Resource = string.Format("api/v1/sim/{0}", su_id);
                request.AddJsonBody(
                    new
                    {
                        status = new { id = 2 }
                    });
                IRestResponse patchResponse = await client.ExecuteAsync(request);
                //string content = response.Content;


            }
            request = new RestRequest("api/v1/sim");
            request.Method = Method.GET;
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("aacept", "application/json, text/plain, */*");
            IRestResponse response = await client.ExecuteAsync(request);
            string content = response.Content;
            sims = JsonConvert.DeserializeObject<List<simResponse>>(content);
            //
            //  re-read databse and web information
            //
            dataGridView1.Rows.Clear();
            int id = serviceProfiles.Where(x => x.name == comboBox1.Text).Select(x => x.id).First();
            List<endpointResponse> points = endPoints.Where(x => x.service_profile.id == id).Select(x => x).ToList();
            List<string> newSims_Activated = new List<string>();
            List<string> newSims_Suspended = new List<string>();
            foreach (endpointResponse endpoint in points)
            {
                List<DateTime> dates = dataInfos.Where(x => x.ICCID == endpoint.sim.iccid).Select(x => x.expires).ToList();
                string simStatus = sims.Where(x => x.id == endpoint.sim.id).Select(x => x.status.description).First();
                string dateString = (dates.Count == 0 ? "" : dates.First().ToString());
                if (dateString == "")
                {
                    if (simStatus == "Activated")
                    {
                        dateString = nextDue(1).ToString();
                    }
                    if (simStatus == "Suspended")
                    {
                        dateString = nextDue(-1).ToString();
                    }
                }

                dataGridView1.Rows.Add(endpoint.name, endpoint.sim.iccid,
                    simStatus,
                    dateString);

            }

            dataGridView1.Enabled = true;
            comboBox1.Enabled = true;
            Cursor = Cursors.Default;
            pictureBox2.Visible = false;

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
        private DateTime nextDue(int flag)
        {
            DateTime today = DateTime.Now;
            DateTime date;
            if (flag == -1)
                return today.AddDays(-1);
            today = today.AddDays(1);
            DateTime nextDue = today.AddMonths(1);
            if (today.Day < 28)
                date = today.AddDays(28 - today.Day);
            else
                date = today.AddMonths(1).AddDays(28 - today.Day);

            return date;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            today = today.AddDays(1);
            DateTime nextDue = today.AddMonths(1);
            if (today.Day < 28)
                nextDue = today.AddDays(28 - today.Day);
            else
                nextDue = today.AddMonths(1).AddDays(28 - today.Day);
            dateTimePicker1.Value = nextDue;
        }
        public class dataInfo
        {
            //public string name { get; set; }
            //public int serviceProfile_id { get; set; }
            //public string serviceProfile_name { get; set; }
            public string ICCID { get; set; }
            public string status { get; set; }
            public DateTime expires { get; set; }
            //public int flag { get; set; }

            //public dataInfo(string n, int sid, string sn, string iccid, string st, DateTime exp)
            public dataInfo(string iccid, string st, DateTime exp)
            {
                //name = n;
                //serviceProfile_id = sid;
                //serviceProfile_name = sn;
                ICCID = iccid;
                status = st;
                expires = exp;
                //flag = 0;
            }





            //serviceProfile_name
            //serviceProfile_id
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to check the due date?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel) return;
            RestRequest request;
            Cursor = Cursors.WaitCursor;
            pictureBox2.Visible = true;

            List<dataInfo> expiredList = dataInfos.Where(x => x.expires < DateTime.Now).ToList();
            List<simResponse> goingToSuspend = sims.Where(x => expiredList.Any(y => x.iccid == y.ICCID && x.status.description == "Activated")).ToList();
            foreach (simResponse sim in goingToSuspend)
            {
                int su_id = sim.id;

                request = new RestRequest("api/v1/sim");
                request.Method = Method.PATCH;
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("aacept", "application/json, text/plain, */*");
                request.RequestFormat = DataFormat.Json;


                request.Resource = string.Format("api/v1/sim/{0}", su_id);
                request.AddJsonBody(
                    new
                    {
                        status = new { id = 2 }
                    });
                IRestResponse patchResponse = await client.ExecuteAsync(request);
            }
            //
            List<dataInfo> renewedList = dataInfos.Where(x => x.expires >= DateTime.Now).ToList();
            List<simResponse> goingToRenew = sims.Where(x => renewedList.Any(y => x.iccid == y.ICCID && x.status.description == "Suspended")).ToList();
            foreach (simResponse sim in goingToRenew)
            {
                int su_id = sim.id;

                request = new RestRequest("api/v1/sim");
                request.Method = Method.PATCH;
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("aacept", "application/json, text/plain, */*");
                request.RequestFormat = DataFormat.Json;


                request.Resource = string.Format("api/v1/sim/{0}", su_id);
                request.AddJsonBody(
                    new
                    {
                        status = new { id = 1 }
                    });
                IRestResponse patchResponse = await client.ExecuteAsync(request);
            }
            await updateDataGridView(true);
            Cursor = Cursors.Default;
            pictureBox2.Visible = false;


        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void DataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            DataGridView grd = sender as DataGridView;
            if (grd.Rows[e.RowIndex].Cells[3].Value.ToString() == "Activated")
            {
                grd.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
               
            }
            else
            {
                grd.Rows[e.RowIndex].Cells[1].Style = new DataGridViewCellStyle(disabledSIMCellStyle);
                grd.Rows[e.RowIndex].Cells[2].Style = new DataGridViewCellStyle(disabledSIMCellStyle);
                grd.Rows[e.RowIndex].Cells[3].Style = new DataGridViewCellStyle(disabledSIMCellStyle);
                
            }
            
        }
    }
}
