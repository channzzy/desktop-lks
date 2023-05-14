using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoodXYZ.kasir
{
    public partial class KasirNavigation : Form
    {
        koneksi con = new koneksi();
        string userid, tipeuser, nama;
        int totalakhir, kembalian = 0;
        string idtransaksi;
        public KasirNavigation(String userid, String tipeuser, String nama)
        {
            InitializeComponent();
            this.userid = userid;
            this.tipeuser = tipeuser;
            this.nama = nama;
            InitiateRefresh();
            lblname.Text = nama;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void cmbx_menu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbx_menu.SelectedIndex != -1)
            {
                Txt_Harga.Text = (cmbx_menu.SelectedItem as dynamic).Harga;
                Btn_Tambah.Enabled = true;
            }
            else
            {
                Txt_Harga.Text = "";
            }
        }

        private void Txt_Qty_TextChanged(object sender, EventArgs e)
        {
            hitungtotal();
        }

        private void Btn_Tambah_Click(object sender, EventArgs e)
        {
            var barang = (cmbx_menu.SelectedItem as dynamic);
            int newID = (dataGridView1.Rows.Count + 1);
            string idtrx = "TR" + newID.ToString().PadLeft(3, '0');

            dataGridView1.Rows.Add(idtrx, barang.Value, barang.Kode, barang.Nama, barang.Harga, Txt_Qty.Text, Txt_Total.Text);

            totalakhir += int.Parse(Txt_Total.Text);
            lbltotal.Text = totalakhir.ToString();
            Btn_Bayar.Enabled = true;
        }

        private void Btn_Bayar_Click(object sender, EventArgs e)
        {
            int bayar = int.Parse(Txt_Bayar.Text.ToString());
            int total = int.Parse(lbltotal.Text.ToString());

            if(bayar < total)
            {
                MessageBox.Show("Uang Anda Kurang");
            }else
            {
                MessageBox.Show("Pembayaran Berhasil");
                kembalian = bayar - total;
                lblkembalian.Text = kembalian.ToString();
                Btn_Save.Enabled = true;
            }
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            con.cud("insert into tbl_transaksi(no_transaksi,tgl_transaksi,total_bayar,id_user) values('" + DateTime.Now.ToString("yyyyMMddHHmmss") + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Txt_Total.Text.ToString() + "','" + userid + "')");

            con.select("select max(id_transaksi) from tbl_transaksi");
            foreach(DataRow dtr in dt.Rows){
                idtransaksi = dtr[0].ToString();
            }

            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                con.cud("insert into tbl_transaksidetail(id_transaksi,id_barang,qty,harga_satuan,subtotal) values('" + idtransaksi + "','" + row.Cells[1].Value.ToString() + "','" + row.Cells[5].Value.ToString() + "','" + row.Cells[4].Value.ToString() + "','" + row.Cells[6].Value.ToString() + "')");
                MessageBox.Show("Data Berhasil Disimpan");
            }
            Btn_Print.Enabled = true;
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

        private void KasirNavigation_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        private void KasirNavigation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.R)
            {
                Btn_Reset.PerformClick();
            }
            else if (e.Control == true && e.KeyCode == Keys.T)
            {
                Btn_Tambah.PerformClick();
            }
            else if (e.Control == true && e.KeyCode == Keys.S)
            {
                Btn_Save.PerformClick();
            }
            else if (e.Control == true && e.KeyCode == Keys.U)
            {
                Btn_Logout.PerformClick();
            }
            else if (e.Control == true && e.KeyCode == Keys.P)
            {
                Btn_Print.PerformClick();
            }
        }

        private void Txt_Qty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                errorProvider1.SetError(Txt_Qty, "Hanya boleh angka");
            }
        }

        private void Btn_Reset_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            lbltotal.Text = "";
            Txt_Qty.Clear();
        }

        private void Btn_Print_Click(object sender, EventArgs e)
        {
            FormReport fr = new FormReport();
            fr.ShowDialog();
        }

        private void cmbx_menu_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        public void InitiateRefresh()
        {
            Btn_Tambah.Enabled = false;
            Btn_Bayar.Enabled = false;
            Btn_Print.Enabled = false;
            Btn_Save.Enabled = false;
            Txt_Total.Enabled = false;
            Txt_Harga.Enabled = false;
            loadCB();
        }
        public void loadCB()
        {
            cmbx_menu.DisplayMember = "Text";
            cmbx_menu.ValueMember = "Value";
            DataTable cb = new DataTable();
            con.select("select id_barang,kode_barang,nama_barang,harga_satuan from tbl_barang");
            con.adp.Fill(cb);

            foreach(DataRow dtr in cb.Rows)
            {
                cmbx_menu.Items.Add(new
                {
                    Value = dtr[0].ToString(),
                    Text = dtr[1].ToString() + " - " + dtr[2].ToString(),
                    Kode = dtr[1].ToString(),
                    Nama = dtr[2].ToString(),
                    Harga = dtr[3].ToString()
                });
            }
            hitungtotal();
        }
        public void hitungtotal()
        {
            int harga = 0;
            if (cmbx_menu.SelectedIndex != -1)
            {
                harga = int.Parse(Txt_Harga.Text.ToString());
            }
            int qty = 0;
            if (Txt_Qty.Text != "")
            {
                qty = int.Parse(Txt_Qty.Text.ToString());
            }
            int total;
            total = harga * qty;
            Txt_Total.Text = total.ToString();
        }
    }
}
