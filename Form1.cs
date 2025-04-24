using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions; // Thêm để validate email

namespace StudentManagement
{
    public partial class Form1 : Form
    {
        private List<Department> departments = new List<Department>();

        public Form1()
        {
            InitializeComponent();
            InitializeData(); // Khởi tạo dữ liệu mẫu
            PopulateTreeView(); // Đổ dữ liệu vào TreeView
            InitializeDataGridView(); // Khởi tạo DataGridView
        }

        // Cấu trúc dữ liệu mới
        public class Department
        {
            public string Name { get; set; }
            public List<Class> Classes { get; set; } = new List<Class>();
        }

        public class Class
        {
            public string Name { get; set; }
            public BindingList<Student> Students { get; set; } = new BindingList<Student>();
        }

        public class Student
        {
            public string StudentID { get; set; } // ID
            public string FullName { get; set; }  // Name
            public string Email { get; set; }     // Email
        }

        private void InitializeData()
        {
            // Tạo dữ liệu mẫu
            var dept1 = new Department { Name = "Khoa CNTT Kinh doanh" };
            var class1_1 = new Class { Name = "Class 111-22-111" };
            class1_1.Students.Add(new Student { StudentID = "105", FullName = "Full name #5", Email = "email5@ueh.edu.vn" });
            class1_1.Students.Add(new Student { StudentID = "106", FullName = "Full name #6", Email = "email6@ueh.edu.vn" });
            var class1_2 = new Class { Name = "Class 111-22-222" };
            class1_2.Students.Add(new Student { StudentID = "107", FullName = "Full name #7", Email = "email7@ueh.edu.vn" });
            dept1.Classes.Add(class1_1);
            dept1.Classes.Add(class1_2);

            var dept2 = new Department { Name = "Khoa Kế toán" };
            var class2_1 = new Class { Name = "class 222-33-111" };
            class2_1.Students.Add(new Student { StudentID = "108", FullName = "Full name #8", Email = "email8@ueh.edu.vn" });
            var class2_2 = new Class { Name = "class 222-33-222" };
            class2_2.Students.Add(new Student { StudentID = "109", FullName = "Full name #9", Email = "email9@ueh.edu.vn" });
            var class2_3 = new Class { Name = "class 222-33-333" }; // Lớp rỗng
            dept2.Classes.Add(class2_1);
            dept2.Classes.Add(class2_2);
            dept2.Classes.Add(class2_3);

            departments.Add(dept1);
            departments.Add(dept2);
        }

        private void PopulateTreeView()
        {
            treeView1.Nodes.Clear();
            foreach (var dept in departments)
            {
                TreeNode deptNode = new TreeNode(dept.Name);
                deptNode.Tag = dept; // Lưu đối tượng Department vào Tag
                foreach (var cls in dept.Classes)
                {
                    TreeNode classNode = new TreeNode(cls.Name);
                    classNode.Tag = cls; // Lưu đối tượng Class vào Tag
                    deptNode.Nodes.Add(classNode);
                }
                treeView1.Nodes.Add(deptNode);
            }
            // Mở rộng tất cả các node
            treeView1.ExpandAll(); 
        }

        private void InitializeDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false; // Không tự tạo cột

            // Thêm cột ID
            DataGridViewTextBoxColumn idCol = new DataGridViewTextBoxColumn();
            idCol.DataPropertyName = "StudentID"; // Liên kết với thuộc tính StudentID
            idCol.HeaderText = "ID";
            idCol.Name = "StudentID";
            idCol.Width = 50;
            dataGridView1.Columns.Add(idCol);

            // Thêm cột Name
            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.DataPropertyName = "FullName"; // Liên kết với thuộc tính FullName
            nameCol.HeaderText = "Name";
            nameCol.Name = "FullName";
            nameCol.Width = 150;
            dataGridView1.Columns.Add(nameCol);

            // Thêm cột Email
            DataGridViewTextBoxColumn emailCol = new DataGridViewTextBoxColumn();
            emailCol.DataPropertyName = "Email"; // Liên kết với thuộc tính Email
            emailCol.HeaderText = "Email";
            emailCol.Name = "Email";
            emailCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Tự động co giãn
            dataGridView1.Columns.Add(emailCol);
        }

        private void RefreshDataGridView(BindingList<Student> studentsToShow)
        {
            dataGridView1.DataSource = null; // Xóa nguồn dữ liệu cũ
            if (studentsToShow != null && studentsToShow.Any())
            {
                // Gán danh sách sinh viên làm nguồn dữ liệu
                // Cần tạo một BindingSource để DataGridView cập nhật thay đổi (nếu có)
                var source = new BindingSource(studentsToShow, null);
                dataGridView1.DataSource = source; 
            }
            else
            {
                dataGridView1.DataSource = null; // Hoặc gán danh sách rỗng
                // dataGridView1.Rows.Clear(); // Cách khác nếu không dùng DataSource
            }
            // Xóa lựa chọn mặc định
            dataGridView1.ClearSelection();
            // Xóa text box
            ClearSelectedStudentDisplay();
        }

        private void ClearSelectedStudentDisplay()
        {
            txtSelectedStudentID.Text = string.Empty;
            txtSelectedFullName.Text = string.Empty;
        }

        private void ClearInputFields()
        {
            txtNewStudentID.Text = string.Empty;
            txtNewFullName.Text = string.Empty;
            txtNewEmail.Text = string.Empty;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                // Sử dụng Regex để kiểm tra định dạng email cơ bản
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is Class selectedClass)
            {
                RefreshDataGridView(selectedClass.Students);
            }
            else
            {
                // Nếu chọn Khoa hoặc không chọn gì, hiển thị DataGridView rỗng
                RefreshDataGridView(new BindingList<Student>());
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Lấy đối tượng Student từ dòng được chọn
                // Do dùng DataSource, có thể lấy trực tiếp từ DataBoundItem
                if (dataGridView1.SelectedRows[0].DataBoundItem is Student selectedStudent)
                {
                    txtSelectedStudentID.Text = selectedStudent.StudentID;
                    txtSelectedFullName.Text = selectedStudent.FullName;
                }
                else
                {
                    ClearSelectedStudentDisplay();
                }
            }
            else
            {
                ClearSelectedStudentDisplay();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1. Lấy lớp đang được chọn từ TreeView
            if (!(treeView1.SelectedNode?.Tag is Class selectedClass))
            {
                MessageBox.Show("Vui lòng chọn một lớp trong danh sách bên trái trước khi thêm.", "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lấy và kiểm tra dữ liệu nhập
            string newID = txtNewStudentID.Text.Trim();
            string newName = txtNewFullName.Text.Trim();
            string newEmail = txtNewEmail.Text.Trim();

            if (string.IsNullOrEmpty(newID) || string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(newEmail))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ ID, Họ tên và Email.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Kiểm tra định dạng Email
            if (!IsValidEmail(newEmail))
            {
                MessageBox.Show("Định dạng Email không hợp lệ.", "Sai định dạng Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Kiểm tra ID trùng lặp trong lớp hiện tại
            if (selectedClass.Students.Any(s => s.StudentID.Equals(newID, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"ID sinh viên '{newID}' đã tồn tại trong lớp này.", "Trùng ID sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5. Tạo và thêm sinh viên mới vào BindingList của lớp
            Student newStudent = new Student
            {
                StudentID = newID,
                FullName = newName,
                Email = newEmail
            };
            selectedClass.Students.Add(newStudent);

            // 6. Xóa các ô nhập liệu
            ClearInputFields();

            // 7. Thông báo thành công (tùy chọn)
            // MessageBox.Show("Đã thêm sinh viên thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // DataGridView sẽ tự cập nhật vì dùng BindingList
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            // 1. Lấy lớp đang được chọn từ TreeView
            if (!(treeView1.SelectedNode?.Tag is Class selectedClass))
            {
                MessageBox.Show("Vui lòng chọn một lớp trước khi xóa sinh viên.", "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lấy sinh viên đang được chọn từ DataGridView
            if (dataGridView1.SelectedRows.Count == 0 || !(dataGridView1.SelectedRows[0].DataBoundItem is Student studentToRemove))
            {
                MessageBox.Show("Vui lòng chọn một sinh viên trong bảng để xóa.", "Chưa chọn sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Xác nhận xóa
            DialogResult confirmResult = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên '{studentToRemove.FullName}' (ID: {studentToRemove.StudentID})?",
                                                     "Xác nhận xóa",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                // 4. Xóa sinh viên khỏi BindingList
                selectedClass.Students.Remove(studentToRemove);
                // DataGridView sẽ tự cập nhật
            }
        }
    }
}
