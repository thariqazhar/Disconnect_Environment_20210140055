using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Act6
{
    public partial class Status : Form
    {
        private string stringConnection = "data source=LAPTOP-9VURLJFC\\THARIQ_AZHAR;" + "database=Act6;User ID=sa;Password=hurricane95";
        private SqlConnection koneksi;
        public Status()
        {
            InitializeComponent();
            koneksi = new SqlConnection(stringConnection);
            refreshform();
        }

        private void Status_Load(object sender, EventArgs e)
        {

        }

        private void refreshform()
        {
            cbxNama.Enabled = false;
            cbxStatus.Enabled = false;
            cbxTahun.Enabled = false;
            cbxNama.SelectedIndex = -1;
            cbxStatus.SelectedIndex = -1;
            cbxTahun.SelectedIndex = -1;
            txtNIM.Enabled = false;
            btnAdd.Enabled = true;
            btnSave.Enabled = false;
            btnClear.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string nim = txtNIM.Text;
            string status = cbxStatus.Text;
            string tahun = cbxTahun.Text;
            int count = 0;
            string tempKodeStatus = "";
            string KodeStatus = "";
            koneksi.Open();

            string str = "select count (*) from dbo.status_mahasiswa";
            SqlCommand cm = new SqlCommand(str, koneksi);
            count = (int)cm.ExecuteScalar();
            if (count == 0)
            {
                KodeStatus = "1";
            }
            else
            {
                string querymax = "select Max(id_status) from dbo.status_mahasiswa";
                SqlCommand cmstatusSum = new SqlCommand(querymax, koneksi);
                int totalstatus = (int)cmstatusSum.ExecuteScalar();
                int finalKodeStatusInt = totalstatus + 1;
                KodeStatus = Convert.ToString(finalKodeStatusInt);
            }
            string queryString = "insert into dbo.status_mahasiswa (id_status, nim " +
                "status_mahasiswa, tahun_masuk)" + "values(@ids, @NIM, @sm, @tm)";
            SqlCommand cmd = new SqlCommand(queryString, koneksi);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add(new SqlParameter("ids", KodeStatus));
            cmd.Parameters.Add(new SqlParameter("NIM", nim));
            cmd.Parameters.Add(new SqlParameter("sm", status));
            cmd.Parameters.Add(new SqlParameter("tm", tahun));
            cmd.ExecuteNonQuery();
            koneksi.Close();

            MessageBox.Show("Data berhasil disimpan", "Sukses", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            refreshform();
            dataGridView();
        }

        private void dataGridView()
        {
            koneksi.Open();
            string str = "select * from dbo.status_mahasiswa";
            SqlDataAdapter da = new SqlDataAdapter(str, koneksi);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            koneksi.Close();
        }

        private void cbNama()
        {
            koneksi.Open();
            string str = "select nama_mahasiswa from dbo.mahasiswa where " +
                "not EXISTS(select id_status from dbo.status_mahasiswa where " +
                "status_mahasiswa.nim = mahasiswa.nim)";
            SqlCommand cmd = new SqlCommand(str, koneksi);
            SqlDataAdapter da = new SqlDataAdapter(str, koneksi);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.ExecuteReader();
            koneksi.Close();

            cbxNama.DisplayMember = "nama_mahasiswa";
            cbxNama.ValueMember = "NIM";
            cbxNama.DataSource = ds.Tables[0];
        }

        private void cbTahun()
        {
            int y = DateTime.Now.Year - 2010;
            string[] type = new string[y]; 
            int i = 0;
            for (i = 0; i < type.Length; i++)
            {
                if (i == 0)
                {
                    cbxTahun.Items.Add("2010");
                }
                else
                {
                    int l = 2010 + i;
                    cbxTahun.Items.Add(l.ToString());
                }
            }
        }

        private void cbxNama_SelectedIndexChanged(object sender, EventArgs e)
        {
            koneksi.Open();
            string nim = "";
            string strs = "select NIM from dbo.mahasiswa where nama_mahasiswa = @nm";
            SqlCommand cm = new SqlCommand(strs, koneksi);
            cm.CommandType = CommandType.Text;
            cm.Parameters.Add(new SqlParameter("@nm", cbxNama.Text));
            SqlDataReader dr = cm.ExecuteReader();
            while (dr.Read())
            {
                nim = dr["NIM"].ToString();
            }
            dr.Close();
            koneksi.Close();

            txtNIM.Text = nim;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            dataGridView();
            btnOpen.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            cbxNama.Enabled = true;
            cbxTahun.Enabled = true;
            cbxStatus.Enabled = true;
            txtNIM.Visible = true;
            cbTahun();
            cbNama();
            btnClear.Enabled = true;
            btnSave.Enabled = true;
            btnAdd.Enabled = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            refreshform();
        }

        private void Status_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 fm = new Form1();
            fm.Show();
            this.Hide();
        }
    }
}
