using De01.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace De01
{
    public partial class frmSinhVien : Form
    {
        // Context để truy cập vào cơ sở dữ liệu
        private ModelsDBSinhVien context;

        public frmSinhVien()
        {
            InitializeComponent();
            context = new ModelsDBSinhVien();
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            // Tải danh sách sinh viên và lớp
            LoadData();
            LoadComboBoxLop();
        }

        // Phương thức để tải dữ liệu sinh viên vào DataGridView
        private void LoadData()
        {
            try
            {
                // Lấy danh sách sinh viên từ cơ sở dữ liệu
                var sinhvienList = context.Sinhviens
                    .Select(sv => new
                    {
                        sv.MaSV,
                        sv.HotenSV,
                        sv.NgaySinh,
                        TenLop = sv.Lop.TenLop // Lấy tên lớp từ bảng Lop
                    })
                    .ToList();

                // Đảm bảo xóa các cột có sẵn trước khi thêm mới
                lvSinhvien.Columns.Clear();

                // Thêm các cột cho DataGridView
                lvSinhvien.Columns.Add("MaSV", "Mã Sinh Viên");
                lvSinhvien.Columns.Add("HotenSV", "Họ và Tên");
                lvSinhvien.Columns.Add("NgaySinh", "Ngày Sinh");
                lvSinhvien.Columns.Add("TenLop", "Tên Lớp");

                // Tắt chế độ tự động sinh cột (AutoGenerateColumns)
                lvSinhvien.AutoGenerateColumns = false;

                // Gán danh sách sinh viên vào DataSource của DataGridView
                lvSinhvien.DataSource = sinhvienList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Phương thức để load dữ liệu lớp vào combobox
        private void LoadComboBoxLop()
        {
            try
            {
                var lopList = context.Lops.ToList();
                cboLop.DataSource = lopList;
                cboLop.DisplayMember = "TenLop";  // Tên lớp hiển thị trong ComboBox
                cboLop.ValueMember = "MaLop";    // Mã lớp được chọn
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu lớp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện thêm sinh viên
        private void btThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (ModelsDBSinhVien context = new ModelsDBSinhVien())
                {
                    Sinhvien sv = new Sinhvien
                    {
                        MaSV = txtMaSV.Text,
                        HotenSV = txtHotenSV.Text,
                        NgaySinh = dtNgaysinh.Value,
                        MaLop = cboLop.SelectedValue.ToString()
                    };
                    context.Sinhviens.Add(sv);
                    context.SaveChanges();
                    MessageBox.Show("Thêm sinh viên thành công!");
                    LoadData(); // Reload dữ liệu để hiển thị sinh viên mới thêm
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sinh viên: {ex.Message}");
            }
        }

        // Sự kiện sửa thông tin sinh viên
        private void btSua_Click(object sender, EventArgs e)
        {
            try
            {
                using (ModelsDBSinhVien context = new ModelsDBSinhVien())
                {
                    string maSV = txtMaSV.Text;
                    Sinhvien sv = context.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (sv != null)
                    {
                        sv.HotenSV = txtHotenSV.Text;
                        sv.NgaySinh = dtNgaysinh.Value;
                        sv.MaLop = cboLop.SelectedValue.ToString();
                        context.SaveChanges();
                        MessageBox.Show("Cập nhật thông tin sinh viên thành công!");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để sửa!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa sinh viên: {ex.Message}");
            }
        }

        // Sự kiện xóa sinh viên
        private void btXoa_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (ModelsDBSinhVien context = new ModelsDBSinhVien())
                    {
                        string maSV = txtMaSV.Text;
                        Sinhvien sv = context.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                        if (sv != null)
                        {
                            context.Sinhviens.Remove(sv);
                            context.SaveChanges();
                            MessageBox.Show("Xóa sinh viên thành công!");
                            LoadData(); // Reload dữ liệu sau khi xóa
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sinh viên để xóa!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}");
                }
            }
        }

        // Sự kiện đóng form với xác nhận
        private void btThoat_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn đóng form?", "Xác nhận", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                this.Close();
            }
        }

        // Sự kiện khi nhấn vào cell trong DataGridView để hiển thị dữ liệu vào form
        private void lvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = lvSinhvien.Rows[e.RowIndex];
                txtMaSV.Text = row.Cells["MaSV"].Value.ToString();
                txtHotenSV.Text = row.Cells["HotenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
                cboLop.Text = row.Cells["TenLop"].Value.ToString();
            }
        }
    }
}
