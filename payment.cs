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
    public partial class payment : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=LAPTOP-FCC0LMK6\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=True;TrustServerCertificate=True");
        string receiptNumber;
        string patientID;
        decimal testFee;
        int hospitalID;
        string hospitalName;
        private string testName;
        public payment(string loggedInUserID, decimal fee, int hospitalID, string hospitalName, string testName)
        {
            InitializeComponent();

            patientID = loggedInUserID;
            testFee = fee;
            GenerateReceiptNumber();
            this.hospitalID = hospitalID;
            txtTotalBill.Text = testFee.ToString("C2"); // Format as currency
            txtReceiptNo.Text = receiptNumber;
            this.hospitalID = hospitalID;
            this.hospitalName = hospitalName;
            txtHospitalName.Text = hospitalName;
            this.testName = testName;
            txtTestName.Text = testName;
        }
        //generationg random recieptnumber
        private void GenerateReceiptNumber()
        {
            Random rnd = new Random();
            receiptNumber = "RC-" + rnd.Next(10000, 99999);
        }



        private void payment_Load(object sender, EventArgs e)
        {

        }
        //pay button
        private void button4_Click(object sender, EventArgs e)
        {
            DateTime appointmentDate = dateTimePickerAppointment.Value;
            DateTime paymentDate = dateTimePickerPayment.Value;
            string paymentMethod = comboBoxPaymentMethod.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(paymentMethod))
            {
                MessageBox.Show("Please select a payment method.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            try
            {
                try
                {
                    conn.Open();

                    // Insert the appointment date and hospital name into the AppointmentSchedule table
                    string appointmentQuery = @"
                    INSERT INTO AppointmentSchedule (UserID,AppointmentDateTime, HospitalName )
                    VALUES (@UserID, @AppointmentDateTime, @HospitalName)";

                    using (SqlCommand appointmentCmd = new SqlCommand(appointmentQuery, conn))
                    {
                        appointmentCmd.Parameters.AddWithValue("@UserID", patientID);
                        appointmentCmd.Parameters.AddWithValue("@AppointmentDateTime", appointmentDate);
                        appointmentCmd.Parameters.AddWithValue("@HospitalName", hospitalName); // Assuming you have hospitalName available
                        

                        int appointmentRowsAffected = appointmentCmd.ExecuteNonQuery();

                        if (appointmentRowsAffected > 0)
                        {
                            MessageBox.Show("Appointment date scheduled successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to schedule appointment. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Insert payment information into the Payment table
                conn.Open();
                string paymentQuery = @"
                INSERT INTO Payment (UserID, Amount, PaymentDate, Receipt,TestName)
                VALUES (@UserID, @Amount, @PaymentDate, @Receipt, @TestName)";
                using (SqlCommand paymentCmd = new SqlCommand(paymentQuery, conn))
                {
                    paymentCmd.Parameters.AddWithValue("@UserID", patientID);
                    paymentCmd.Parameters.AddWithValue("@Amount", testFee);
                    paymentCmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                    paymentCmd.Parameters.AddWithValue("@Receipt", receiptNumber);
                    paymentCmd.Parameters.AddWithValue("@TestName", testName);

                    int paymentRowsAffected = paymentCmd.ExecuteNonQuery();

                    if (paymentRowsAffected > 0)
                    {
                        MessageBox.Show("Payment submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Patient_dashboard form= new Patient_dashboard(patientID);
                        form.Show();
                        this.Hide();

                    }
                    else
                    {
                        MessageBox.Show("Payment submission failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Patient_dashboard patientd =new Patient_dashboard(patientID);
            patientd.Show();
            this.Hide();
        }
    }
    

}
