using Microsoft.Extensions.Logging;
using Project_StudentManagement.DAL;
using Project_StudentManagement.DTO;

namespace Project_StudentManagement.BLL;

/// <summary>Quy tắc nghiệp vụ độc lập với WinForms và SQL.</summary>
public sealed class StudentBLL
{
    private readonly StudentDAL _dal;
    private readonly ILogger<StudentBLL> _logger;
    public StudentBLL(StudentDAL dal, ILogger<StudentBLL> logger) { _dal = dal; _logger = logger; }

    public List<StudentDTO> GetStudents() => _dal.GetAll();
    public List<StudentDTO> SearchStudents(string? keyword)
    {
        var normalized = (keyword ?? "").Trim();
        _logger.LogDebug(new EventId(2001, "SearchRequested"), "Search requested with keyword {SearchKeyword}", normalized);
        return _dal.Search(normalized);
    }
    public void AddStudent(StudentDTO s) { ValidateStudent(s); _dal.Insert(s); }
    public bool UpdateStudent(StudentDTO s) { ValidateStudent(s); if (string.IsNullOrWhiteSpace(s.StudentCode)) throw new ArgumentException("Hãy chọn sinh viên cần sửa."); return _dal.Update(s) > 0; }
    public bool DeleteStudent(string code) { if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Hãy chọn sinh viên cần xóa."); return _dal.Delete(code.Trim()) > 0; }

    public static void ValidateStudent(StudentDTO s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        s.StudentCode = (s.StudentCode ?? "").Trim(); s.FullName = (s.FullName ?? "").Trim();
        s.ClassName = (s.ClassName ?? "").Trim(); s.Phone = (s.Phone ?? "").Trim(); s.Email = (s.Email ?? "").Trim();
        if (s.StudentCode.Length == 0) throw new ArgumentException("Mã sinh viên không được để trống.");
        if (s.FullName.Length == 0) throw new ArgumentException("Họ tên không được để trống.");
        if (s.StudentCode.Length > 50 || s.FullName.Length > 200 || s.ClassName.Length > 100 || s.Phone.Length > 30 || s.Email.Length > 200)
            throw new ArgumentException("Thông tin vượt quá độ dài cho phép.");
        if (s.Email.Length > 0 && !s.Email.Contains('@')) throw new ArgumentException("Email không hợp lệ.");
    }

    public static bool IsValidGpa(double gpa) => !double.IsNaN(gpa) && !double.IsInfinity(gpa) && gpa >= 0.0 && gpa <= 4.0;
    public static void ValidateGpa(double gpa)
    {
        if (!IsValidGpa(gpa)) throw new ArgumentOutOfRangeException(nameof(gpa), "GPA phải nằm trong khoảng từ 0 đến 4.");
    }
    public static bool IsValidSearchKeyword(string? keyword) => !string.IsNullOrWhiteSpace(keyword);
}
