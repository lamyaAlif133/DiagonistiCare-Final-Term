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
    public partial class Patient_dashboard : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");
        private string loggedInUserID;
        private string patientName;
        
        public Patient_dashboard(string userID)
        {
            InitializeComponent();
            loggedInUserID = userID;
            LoadPatientName();
            LoadHospitals();
            comboBoxHospitals.SelectedIndexChanged += comboBoxHospitals_SelectedIndexChanged;
        }

        private void LoadPatientName()
        {
            try
            {
                conn.Open();
                string query = "SELECT Name FROM Users WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", loggedInUserID);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    patientName = result.ToString();
                    labelPatientName.Text = "Patient: " + patientName; 
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

        private void LoadHospitals()
        {
            try
            {
                conn.Open();
                string query = "SELECT HospitalName FROM Hospitals";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                comboBoxHospitals.Items.Clear();
                while (reader.Read())
                {
                    comboBoxHospitals.Items.Add(reader["HospitalName"].ToString());
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

        private void LoadTests(string hospitalName = null)
        {
            try
            {
                conn.Open();
                string query = @"SELECT TestName FROM Tests WHERE HospitalID = (SELECT HospitalID FROM Hospitals WHERE HospitalName = @HospitalName)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HospitalName", hospitalName);
                SqlDataReader reader = cmd.ExecuteReader();
                comboBoxTests.Items.Clear();
                while (reader.Read())
                {
                    comboBoxTests.Items.Add(reader["TestName"].ToString());
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
        
        
        
        private void Patient_dashboard_Load(object sender, EventArgs e)
        {
            
        }
        
        //Generate fee button
        private void button7_Click(object sender, EventArgs e)
        {
            string selectedHospital = comboBoxHospitals.SelectedItem.ToString();
            string selectedTest = comboBoxTests.SelectedItem.ToString();

            if (string.IsNullOrEmpty(selectedHospital) || string.IsNullOrEmpty(selectedTest))
            {
                MessageBox.Show("Please select a hospital and a test.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal fee = FetchTestFee(selectedHospital, selectedTest);
            textBoxFee.Text = fee.ToString(); 
             
        }
        //method to fetch fee from hospital and test table
        private decimal FetchTestFee(string selectedHospital, string selectedTest)
        {
            decimal fee = 0;
            try
            {
                conn.Open();
                string query = "SELECT Fee FROM Tests WHERE TestName = @TestName AND HospitalID = (SELECT HospitalID FROM Hospitals WHERE HospitalName = @HospitalName)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TestName", selectedTest);
                cmd.Parameters.AddWithValue("@HospitalName", selectedHospital);
                object result = cmd.ExecuteScalar();

                if (result != null && decimal.TryParse(result.ToString(), out fee))
                {
                    return fee;
                }
                else
                {
                    MessageBox.Show("Test fee not found for the selected hospital and test.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            return fee;
        }
       
        //proceed button
        private void button6_Click(object sender, EventArgs e)
        {
            string selectedHospital = comboBoxHospitals.SelectedItem.ToString();
            string selectedTest = comboBoxTests.SelectedItem.ToString();

            if (string.IsNullOrEmpty(selectedHospital) || string.IsNullOrEmpty(selectedTest))
            {
                MessageBox.Show("Please select a hospital and a test.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal fee = FetchTestFee(selectedHospital, selectedTest);
            int selectedHospitalID = Convert.ToInt32(comboBoxHospitals.SelectedValue);
            payment paymentForm = new payment(loggedInUserID, fee, selectedHospitalID, selectedHospital, selectedTest);
            paymentForm.Show();
            this.Hide(); ;
        }
        //Signout button
        private void button5_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }

        private void comboBoxHospitals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxHospitals.SelectedItem != null)
            {
                LoadTests(comboBoxHospitals.SelectedItem.ToString());
            }
        }
        //complain button
        private void button3_Click(object sender, EventArgs e)
        {
            Complain complain = new Complain(loggedInUserID);
            complain.Show();
            this.Hide();
        }
        //payment and billing button
        private void button4_Click(object sender, EventArgs e)
        {
            payshow paymentInfoForm = new payshow(loggedInUserID, patientName);
            paymentInfoForm.Show();
            this.Hide();
        }
        //test result button
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is under Development", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        //Message button
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is under Development", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
