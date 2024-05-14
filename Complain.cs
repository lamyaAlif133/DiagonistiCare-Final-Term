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
    public partial class Complain : Form
    {
        private string userId;
        SqlConnection conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");
        public Complain(string userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        private void Complain_Load(object sender, EventArgs e)
        {

        }
        //submit
        private void button1_Click(object sender, EventArgs e)
        {
            string complainText = txtComplaint.Text.Trim();
             

            if (string.IsNullOrEmpty(complainText))
            {
                MessageBox.Show("Please enter a complaint before submitting.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True"))
            {
                try
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO Complain (UserID, ComplainText) VALUES (@UserID, @ComplainText)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@ComplainText", complainText);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Your complaint has been submitted successfully.", "Complaint Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtComplaint.Clear();
                            Patient_dashboard patientd = new Patient_dashboard(userId);
                            patientd.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Failed to submit your complaint. Please try again.", "Submission Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }

            }
        }
        //back button
        private void button2_Click(object sender, EventArgs e)
        {
            Patient_dashboard patientd = new Patient_dashboard(userId);
            patientd.Show();
            this.Hide();
        }
    }
}
