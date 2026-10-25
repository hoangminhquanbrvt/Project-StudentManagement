
namespace StudentManagement.DTO
{
    // DTO chỉ dùng để chứa và chuyển dữ liệu sinh viên.
    public class StudentDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }

        public string ClassName { get; set; }

        public string Major { get; set; }

        public StudentDTO()
        {
            Code = "";
            FullName = "";
            ClassName = "";
            Major = "";
        }
    }
}
