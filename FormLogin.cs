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

namespace Project_StudentManagement
{
    public partial class FormLogin : Form
    {
        string connectionString = @"Data Source=LAPTOP-RPV7OC8J\MSSQLSERVER04;Initial Catalog=StudentDB;Integrated Security=True;";
        public FormLogin()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
            btnLogin.Click += btnLogin_Click;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (username == "" || password == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Username và Password!");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT COUNT(*)
                        FROM Users
                        WHERE Username = @username
                          AND Password = @password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        conn.Open();

                        int result = (int)cmd.ExecuteScalar();

                        if (result > 0)
                        {
                            MessageBox.Show("Đăng nhập thành công!");

                            FormStudent frm = new FormStudent();
                            frm.Show();

                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Username hoặc Password không đúng!");
                            txtPassword.Clear();
                            txtPassword.Focus();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi kết nối SQL Server: " + ex.Message);
            }
        }
        private void FormLogin_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.Login;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
