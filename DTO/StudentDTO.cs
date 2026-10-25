namespace Project_StudentManagement.DTO;

/// <summary>Đối tượng dữ liệu sinh viên dùng để trao đổi giữa UI, BLL và DAL.</summary>
public sealed class StudentDTO
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = "";
    public string FullName { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime DateOfBirth { get; set; } = DateTime.Today;

    // GPA dùng cho quy tắc nghiệp vụ và Unit Test của bài thực hành.
    // Giao diện cũ và database cũ không có cột GPA nên trường này chưa được lưu vào DB.
    public double GPA { get; set; }
}
