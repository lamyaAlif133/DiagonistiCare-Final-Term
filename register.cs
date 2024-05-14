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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;

namespace DiagnostiCare
{
    public partial class register : Form

    {
        SqlConnection conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");
        public register()
        {
            InitializeComponent();
        }
        //register
        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string username = txtUsername.Text.Trim();
            int age = (int)numericUpDownAge.Value;
            string gender = comboBoxGender.SelectedItem.ToString();
            DateTime birthdate = dateTimePickerBirthdate.Value;
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Please fill in all the fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Password and Confirm Password do not match. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Name, Username, Age, Gender, Birthdate, Password, Phone, RoleID) VALUES (@Name, @Username, @Age, @Gender, @Birthdate, @Password, @Phone, @RoleID)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Birthdate", birthdate);
                        cmd.Parameters.AddWithValue("@Password", password);  
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@RoleID", GetRoleID("Patient")); //only for patients
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            this.Hide();
                            Form1 Login = new Form1();
                            Login.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Registration failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //converting role string to roleID
        private int GetRoleID(string role)
        {
            switch (role)
            {
                case "Admin":
                    return 1;
                case "Patient":
                    return 2;
                case "Hospital":
                    return 3;
                default:
                    return -1;
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtUsername.Clear();
            numericUpDownAge.Value = 18;
            comboBoxGender.SelectedIndex = 0;
            dateTimePickerBirthdate.Value = DateTime.Now;
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            txtPhone.Clear();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
        //clear
        private void button2_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
    }
}
