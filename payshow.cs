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

namespace DiagnostiCare
{
    public partial class payshow : Form
    {
        private string userId;
        private string username;
        private SqlConnection conn;
        public payshow(string userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");
            LoadPaymentData();
            this.username = username;
            labelUsername.Text = "Username: " + this.username;
        }

        private void payshow_Load(object sender, EventArgs e)
        {

        }
        private void LoadPaymentData()
        {
            try
            {
                conn.Open();
                string query = @"
                SELECT PaymentID, UserID, Amount, PaymentDate, Receipt 
                FROM Payment
                WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridViewPayments.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load payment data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
        //back button
        private void button1_Click(object sender, EventArgs e)
        {
            Patient_dashboard patientd = new Patient_dashboard(userId);
            patientd.Show();
            this.Hide();
        }
    }
}