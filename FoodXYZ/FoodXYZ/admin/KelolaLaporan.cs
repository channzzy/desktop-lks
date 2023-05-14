using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoodXYZ.admin
{
    public partial class KelolaLaporan : Form
    {
        koneksi con = new koneksi();
        DataTable dt = new DataTable();
        string userid, tipeuser, nama;
        public KelolaLaporan(String userid, String tipeuser, String nama)
        {
            InitializeComponent();
            this.userid = userid;
            this.tipeuser = tipeuser;
            this.nama = nama;
            DateTime date = DateTime.Now;
            dtpicker1.Value = new DateTime(date.Year, date.Month,1);
            dtpicker2.Value = date;
            viewDG();
        }

        private void Btn_Filter_Click(object sender, EventArgs e)
        {
            fillData();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Btn_Generate_Click(object sender, EventArgs e)
        {
            DataTable dc = new DataTable();
            con.select("select convert(varchar(10),tgl_transaksi,111) as tanggal,sum(total_bayar) as bayar from tbl_transaksi where tgl_transaksi between '" + dtpicker1.Value.ToString("yyyy-MM-dd") + "' and '" + dtpicker2.Value.ToString("yyyy-MM-dd") + "' group by tgl_transaksi ");
            con.adp.Fill(dc);
            chart1.Series.Clear();
            chart1.Titles.Clear();

            chart1.Series.Add("Omset");

            foreach(DataRow dtr in dc.Rows)
            {
                chart1.Series["Omset"].Points.AddXY(dtr[0].ToString(), int.Parse(dtr[1].ToString()));
            }
             
        }

        private void KelolaLaporan_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        private void KelolaLaporan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.U)
            {
                Btn_KelolaUser.PerformClick();
            }
            else if (e.Control == true && e.KeyCode == Keys.L)
            {
                Btn_KelolaLaporan.PerformClick();
            }
            else if (e.Control == true && e.KeyCode == Keys.G)
            {
                Btn_Generate.PerformClick();
            }
            else if (e.Control == true && e.KeyCode == Keys.F)
            {
                Btn_Filter.PerformClick();
            }
            else if (e.Control == true && e.KeyCode == Keys.P)
            {
                Btn_Logout.PerformClick();
            }
        }

        private void Btn_Logout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Apakah anda yakin?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (DialogResult.Yes == result)
            {
                FormLogin fl = new FormLogin();
                this.Hide();
                con.cud("update tbl_log set aktivitas='Logout',waktu='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where id_user='" + userid + "'");
                MessageBox.Show("Anda berhasil logout");
                fl.ShowDialog();
                this.Close();
            }
        }

        private void Btn_KelolaUser_Click(object sender, EventArgs e)
        {
            admin.KelolaUser ku = new admin.KelolaUser(userid, tipeuser, nama);
            this.Hide();
            ku.ShowDialog();
            this.Close();
        }

        private void Btn_KelolaLaporan_Click(object sender, EventArgs e)
        {
            admin.KelolaLaporan kl = new admin.KelolaLaporan(userid, tipeuser, nama);
            this.Hide();
            kl.ShowDialog();
            this.Close();
        }

        public void viewDG()
        {
            dt.Clear();
            dataGridView1.Refresh();
            dataGridView1.DataSource = dt;
            fillData();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].HeaderText = "ID Transaksi";
            dataGridView1.Columns[1].HeaderText = "Tanggal Transaksi";
            dataGridView1.Columns[2].HeaderText = "Total Pembayaran";
            dataGridView1.Columns[3].HeaderText = "Nama Kasir";
        }
        public void fillData()
        {
            dt.Clear();
            con.select("select tt.id_transaksi,tt.tgl_transaksi,tt.total_bayar,tu.nama from tbl_transaksi as tt inner join tbl_user as tu on tt.id_user = tu.id_user where tgl_transaksi between '" + dtpicker1.Value.ToString("yyyy-MM-dd") + "' and '" + dtpicker2.Value.ToString("yyyy-MM-dd") + "'");
            con.adp.Fill(dt);
        }
    }
}
