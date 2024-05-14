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
    public partial class Hospital_dashboard : Form
    {
        private SqlConnection conn;
        
        public Hospital_dashboard()
        {
            InitializeComponent();
            conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");
        }
       
            private void Hospital_dashboard_Load(object sender, EventArgs e)
        {
            LoadHospitalNames();
            LoadAppointments();
        }
       
            private void LoadHospitalNames()
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT HospitalName FROM Hospitals", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        comboBoxSort.Items.Add(reader["HospitalName"].ToString());
                    }
                    reader.Close();  
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load hospital names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        
        //loading information in datagridview
        private void LoadAppointments()
        {
            try
            {
             conn.Open();
             string query = @"
        SELECT 
            a.AppointmentID, 
            a.AppointmentDateTime, 
            u.UserID, 
            u.Name, 
            u.Phone,  
            u.Age,    
            u.Gender, 
            h.HospitalName 
        FROM AppointmentSchedule a
        JOIN Users u ON a.UserID = u.UserID
        JOIN Hospitals h ON a.HospitalName = h.HospitalName";  // Ensure that join condition is correct
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewAppointments.DataSource = table;
                }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load appointments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

        }
        //load
        private void button1_Click(object sender, EventArgs e)
        {
            LoadAppointments();
        }
        //logout
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }


        //hospital sorting via combobox
        private void comboBoxSort_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (dataGridViewAppointments.DataSource != null)
            {
                (dataGridViewAppointments.DataSource as DataTable).DefaultView.RowFilter = $"HospitalName = '{comboBoxSort.SelectedItem}'";
            }
        }
    }
}
