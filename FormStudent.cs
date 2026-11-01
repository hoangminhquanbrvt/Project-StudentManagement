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
        string connStr = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=StudentDB;Integrated Security=True;TrustServerCertificate=True";
        
        public FormStudent()
        {
            InitializeComponent();
        }
        private string selectedMSSV = ""; // MSSV gốc của sinh viên đang chọn, rỗng = chưa chọn
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
        
        private void TimKiemSinhVien()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string keyword = txtTimKiem.Text.Trim();
                    if (string.IsNullOrWhiteSpace(keyword) ||
                        keyword == "Nhập MSSV hoặc tên để tìm kiếm...")
                    {
                        LoadSinhVien();
                        return;
                    }

                    string sql = @"
                SELECT
                    StudentCode AS MSSV,
                    Phone AS SoDienThoai,
                    Class AS Lop,
                    FullName AS HoTen,
                    DateOfBirth AS NgaySinh,
                    Email
                FROM Students
                WHERE StudentCode LIKE @TuKhoa
                   OR FullName LIKE @TuKhoa";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuKhoa", "%" + keyword + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();

                        adapter.Fill(dt);

                        dgvStudents.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void ClearInputs()
        {
            txtMSV.Clear();
            txtHoTen.Clear();
            txtSDT.Clear();
            txtLop.Clear();
            txtEmail.Clear();
            dtpNgaySinh.Value = DateTime.Today;
            txtMSV.Focus();
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TimKiemSinhVien();
                e.SuppressKeyPress = true;
            }
        }
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            TimKiemSinhVien();
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
            // Kiểm tra dữ liệu bắt buộc
            if (string.IsNullOrWhiteSpace(txtMSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtLop.Text))
            {
                MessageBox.Show("Vui lòng nhập MSSV, họ tên và lớp!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string sql = @"
                INSERT INTO Students
                (StudentCode, FullName, Phone, Class, DateOfBirth, Email)
                VALUES
                (@MSSV, @HoTen, @SDT, @Lop, @NgaySinh, @Email)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MSSV", txtMSV.Text.Trim());
                        cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                        cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                        cmd.Parameters.AddWithValue("@Lop", txtLop.Text.Trim());
                        cmd.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value.Date);
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm sinh viên thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Tải lại danh sách
                LoadSinhVien();

                // Xóa dữ liệu nhập
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm sinh viên: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = "";
            txtTimKiem_Leave(null, EventArgs.Empty); // Đặt lại chữ gợi ý trong ô tìm kiếm
            ClearInputs();
            LoadSinhVien();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMSSV))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa trong danh sách!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa sinh viên {txtHoTen.Text} ({selectedMSSV})?\nHành động này không thể hoàn tác.",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Students WHERE StudentCode = @MSSV", conn))
                {
                    cmd.Parameters.AddWithValue("@MSSV", selectedMSSV);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Xóa sinh viên thành công!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadSinhVien();
                ClearInputs();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("Không thể xóa vì sinh viên này đang có dữ liệu liên quan ở bảng khác.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa sinh viên: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMSSV))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa trong danh sách!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtLop.Text))
            {
                MessageBox.Show("Vui lòng nhập MSSV, họ tên và lớp!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn cập nhật thông tin sinh viên này?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(@"
            UPDATE Students
            SET StudentCode = @MSSVMoi,
                FullName    = @HoTen,
                Phone       = @SDT,
                Class       = @Lop,
                DateOfBirth = @NgaySinh,
                Email       = @Email
            WHERE StudentCode = @MSSVCu", conn))
                {
                    cmd.Parameters.AddWithValue("@MSSVCu", selectedMSSV);       // MSSV gốc
                    cmd.Parameters.AddWithValue("@MSSVMoi", txtMSV.Text.Trim()); // MSSV mới (có thể đã sửa)
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                    cmd.Parameters.AddWithValue("@Lop", txtLop.Text.Trim());
                    cmd.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value.Date);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên này, có thể đã bị xóa.",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                MessageBox.Show("Cập nhật sinh viên thành công!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadSinhVien();
                ClearInputs();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("MSSV này đã tồn tại!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật sinh viên: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataRowView row = dgvStudents.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;

            selectedMSSV = row["MSSV"].ToString();
            txtMSV.Text = selectedMSSV;
            txtHoTen.Text = row["HoTen"].ToString();
            txtSDT.Text = row["SoDienThoai"].ToString();
            txtLop.Text = row["Lop"].ToString();
            txtEmail.Text = row["Email"].ToString();

            if (row["NgaySinh"] != DBNull.Value)
                dtpNgaySinh.Value = Convert.ToDateTime(row["NgaySinh"]);

        }
    }
}
