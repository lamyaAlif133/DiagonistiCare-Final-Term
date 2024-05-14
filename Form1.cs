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
    public partial class Form1 : Form
    {
        private string loggedInUserID;
        
        public Form1()
        {
            InitializeComponent();
        }
        private SqlConnection GetDatabaseConnection()
        {
            return new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }
        //register button
        private void button2_Click(object sender, EventArgs e)
        {
            register form = new register();
            form.Show();
            this.Hide();
        }
        //login
        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = GetSelectedRole();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please enter username, password, and select a role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ValidateUser(username, password, GetRoleID(role)))
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HandleUserRedirect(role, username);
            }
            else
            {
                MessageBox.Show("Invalid username, password, or role. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    //checking username,pass,role
    private bool ValidateUser(string username, string password, int roleId)
    {
        try
        {
            using (var conn = GetDatabaseConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password=@Password AND RoleID = @RoleID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@RoleID", roleId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
    //role distribution
    private void HandleUserRedirect(string role, string username)
    {
        switch (role)
        {
            case "Admin":
                Admin_dashboard admin_form=new Admin_dashboard();
                    admin_form.Show();
                    this.Hide();
                break;
            case "Patient":
                loggedInUserID = GetUserID(username);
                if (!string.IsNullOrEmpty(loggedInUserID))
                    new Patient_dashboard(loggedInUserID).Show();
                else
                    MessageBox.Show("Unable to fetch user details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                break;
            case "Hospital":
                // Logic for opening the Doctor dashboard
                Hospital_dashboard hospital_Dashboard = new Hospital_dashboard();
                    hospital_Dashboard.Show();
                    this.Hide();
                break;
        }
        this.Hide();
    }
    //fetching user
    private string GetUserID(string username)
    {
        try
        {
            using (var conn = GetDatabaseConnection())
            {
                conn.Open();
                string query = "SELECT UserID FROM Users WHERE Username = @Username";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }
    //radio button checking method
    private string GetSelectedRole()
    {
        if (rbPatient.Checked)
            return "Patient";
        else if (rbAdmin.Checked)
            return "Admin";
        else if (rbHospital.Checked)
            return "Hospital";
        return "";
    }
    //getting roleID from role name method
    private int GetRoleID(string role)
    {
        switch (role)
        {
            case "Admin": return 1;
            case "Patient": return 2;
            case "Hospital": return 3;
            default: return -1;
        }
    }

    private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
