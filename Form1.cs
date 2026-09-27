using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_StudentManagement
{
    public partial class frmStudent : Form
    {
        public frmStudent()
        {
            InitializeComponent();
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
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
