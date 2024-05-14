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
    public partial class Admin_dashboard : Form

    {
        private SqlConnection conn;

        public Admin_dashboard()
        {
            InitializeComponent();
            conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");
            LoadUsers();
            LoadRoles();
            comboBoxRoles.SelectedIndexChanged += comboBoxRoles_SelectedIndexChanged;
            btnSearch.Click += btnSearch_Click;
        }
        private void LoadUsers()
        {
            try
                {
                    using (var conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True"))
                    {
                        conn.Open();
                        string query = @"SELECT u.UserID, u.Name, u.Username, u.Age, u.Gender, u.Birthdate, u.Phone, u.Password, r.RoleName 
                             FROM Users u
                             JOIN UserRoles r ON u.RoleID = r.RoleID";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridViewUsers.DataSource = dataTable;
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

        private void LoadRoles()
        {
            try
            {
                using (var conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();
                    string query = "SELECT RoleName FROM UserRoles";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            comboBoxRoles.Items.Clear();  
                            comboBoxRoles.Items.Add("All");  // Add All option at the top of the list
                            while (reader.Read())
                            {
                                comboBoxRoles.Items.Add(reader["RoleName"].ToString());
                            }
                            comboBoxRoles.SelectedIndex = 0;  // Set All as the default selected item
                        }
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
        //filering via roles
        private void comboBoxRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterUsersByRole();
        }
        private void FilterUsersByRole()
        {
            string selectedRole = comboBoxRoles.SelectedItem.ToString();
            DataView dv = ((DataTable)dataGridViewUsers.DataSource).DefaultView;

            if (selectedRole != "All")
            {
                dv.RowFilter = $"RoleName = '{selectedRole}'"; // Filter by role name directly
            }
            else
            {
                dv.RowFilter = ""; // Clear the filter for 'All'
            }
        }


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
            //update
            private void update_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int userID = Convert.ToInt32(dataGridViewUsers.SelectedRows[0].Cells["UserID"].Value);
            string name = txtName.Text.Trim();
            string username = txtUsername.Text.Trim();
            int age = (int)numericUpDownAge.Value;
            string gender = comboBoxGender.SelectedItem.ToString();
            DateTime birthdate = dateTimePickerBirthdate.Value;
            string password = txtPassword.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string role = comboBoxRoles.SelectedItem.ToString();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill in all the required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();
                    string query = "UPDATE Users SET Name = @Name, Username = @Username, Age = @Age, Gender = @Gender, Birthdate = @Birthdate, Password = @Password, Phone = @Phone, RoleID = (SELECT RoleID FROM UserRoles WHERE RoleName = @RoleName) WHERE UserID = @UserID";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Birthdate", birthdate);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@RoleName", role);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadUsers(); // Refresh user data
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("User update failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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
        //insert
        private void insert_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string username = txtUsername.Text.Trim();
            int age = (int)numericUpDownAge.Value;
            string gender = comboBoxGender.SelectedItem.ToString();
            DateTime birthdate = dateTimePickerBirthdate.Value;
            string password = txtPassword.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string role = txtRole.Text.Trim(); // TextBox for role input

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill in all the required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Name, Username, Age, Gender, Birthdate, Password, Phone, RoleID) VALUES (@Name, @Username, @Age, @Gender, @Birthdate, @Password, @Phone, (SELECT RoleID FROM UserRoles WHERE RoleName = @RoleName))";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Birthdate", birthdate);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@RoleName", role); // Pass the role name to fetch the RoleID

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadUsers(); // Refresh user data
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("User insertion failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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
        //delete
        private void delete_Click(object sender, EventArgs e)
        {
            // Get selected user ID from the DataGridView
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int userID = Convert.ToInt32(dataGridViewUsers.SelectedRows[0].Cells["UserID"].Value);

                try
                {
                    using (var conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True"))
                    {
                        conn.Open();
                        string query = "DELETE FROM Users WHERE UserID = @UserID";
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserID", userID);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("User deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadUsers(); // Refresh user data
                                ClearFields();
                            }
                            else
                            {
                                MessageBox.Show("User deletion failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
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
        //load
        private void search_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }
        private void ClearFields()
        {
            txtName.Clear();
            txtUsername.Clear();
            numericUpDownAge.Value = 18;
            comboBoxGender.SelectedIndex = 0;
            dateTimePickerBirthdate.Value = DateTime.Now;
            txtPassword.Clear();
            txtPhone.Clear();
        }
        private void SearchUsers()
        {
            string searchTerm = txtSearch.Text.Trim();
            try
            {
                using (var conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();
                    string query = @"SELECT u.UserID, u.Name, u.Username, u.Age, u.Gender, u.Birthdate, u.Phone, u.Password, r.RoleName
                             FROM Users u
                             JOIN UserRoles r ON u.RoleID = r.RoleID
                             WHERE u.Name LIKE @SearchTerm OR u.Username LIKE @SearchTerm OR u.Phone LIKE @SearchTerm";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridViewUsers.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //search button
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchUsers();
        }
        private void comboBoxGender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        //logout button
        private void button1_Click(object sender, EventArgs e)
        {
            Form1 Login = new Form1();
            Login.Show();
            this.Hide();
        }

        private void comboBoxRoles_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
