using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;



namespace Project_StudentManagement
{
    public partial class FormStudent : Form
    {
        string connStr = @"Data Source=LAPTOP-RPV7OC8J\MSSQLSERVER04;Initial Catalog=StudentDB;Integrated Security=True;TrustServerCertificate=True";
        
        public FormStudent()
        {
            InitializeComponent();
        }
        private void LoadSinhVien()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string sql = @"
                SELECT
                    StudentCode AS MSSV,
                    Phone AS SoDienThoai,
                    Class AS Lop,
                    FullName AS HoTen,
                    DateOfBirth AS NgaySinh,
                    Email
                FROM Students";

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvStudents.AutoGenerateColumns = false;

                    dgvStudents.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }
        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Nhập MSSV hoặc tên để tìm kiếm...")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }
        private void txtTimKiem_Leave(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "")
            {
                txtTimKiem.Text = "Nhập MSSV hoặc tên để tìm kiếm...";
                txtTimKiem.ForeColor = Color.Gray;
            }
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
            txtTimKiem.Text = "Nhập MSSV hoặc tên để tìm kiếm...";
            txtTimKiem.ForeColor = Color.Gray;
            LoadSinhVien();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }
    }
}
