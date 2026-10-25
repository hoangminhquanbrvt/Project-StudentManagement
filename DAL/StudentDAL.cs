using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Project_StudentManagement.DTO;
using System.Data;

namespace Project_StudentManagement.DAL;

/// <summary>Chỉ chịu trách nhiệm truy cập SQL Server.</summary>
public sealed class StudentDAL
{
    private readonly string _connectionString;
    private readonly ILogger<StudentDAL> _logger;

    public StudentDAL(ILogger<StudentDAL> logger, string? connectionString = null)
    {
        _logger = logger;
        _connectionString = connectionString ?? Environment.GetEnvironmentVariable("STUDENTDB_CONNECTION_STRING")
            ?? @"Server=localhost\SQLEXPRESS;Database=StudentDB;Integrated Security=True;TrustServerCertificate=True;";
    }

    public List<StudentDTO> GetAll()
    {
        const string sql = @"SELECT StudentCode, Phone, Class, FullName, DateOfBirth, Email
                             FROM Students ORDER BY StudentCode";
        var result = new List<StudentDTO>();
        try
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, cn);
            cn.Open(); using var reader = cmd.ExecuteReader();
            while (reader.Read()) result.Add(Map(reader));
            _logger.LogDebug(new EventId(1001, "GetAll"), "Loaded {StudentCount} student records", result.Count);
            return result;
        }
        catch (Exception ex) { _logger.LogError(new EventId(1901, "GetAllFailed"), ex, "Failed to load student list"); throw; }
    }

    public List<StudentDTO> Search(string keyword)
    {
        const string sql = @"SELECT StudentCode, Phone, Class, FullName, DateOfBirth, Email FROM Students
                             WHERE StudentCode LIKE @Keyword OR FullName LIKE @Keyword ORDER BY StudentCode";
        var result = new List<StudentDTO>();
        try
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 200).Value = "%" + keyword + "%";
            cn.Open(); using var reader = cmd.ExecuteReader();
            while (reader.Read()) result.Add(Map(reader));
            _logger.LogInformation(new EventId(1002, "Search"), "Search completed for keyword {SearchKeyword}; found {StudentCount} records", keyword, result.Count);
            return result;
        }
        catch (Exception ex) { _logger.LogError(new EventId(1902, "SearchFailed"), ex, "Search failed for keyword {SearchKeyword}", keyword); throw; }
    }

    public int Insert(StudentDTO s)
    {
        const string sql = @"INSERT INTO Students (StudentCode, Phone, Class, FullName, DateOfBirth, Email)
                             VALUES (@Code, @Phone, @Class, @Name, @Dob, @Email)";
        return ExecuteWrite(sql, cmd => AddParameters(cmd, s), "Insert", s.StudentCode);
    }
    public int Update(StudentDTO s)
    {
        const string sql = @"UPDATE Students SET Phone=@Phone, Class=@Class, FullName=@Name, DateOfBirth=@Dob, Email=@Email WHERE StudentCode=@Code";
        return ExecuteWrite(sql, cmd => AddParameters(cmd, s), "Update", s.StudentCode);
    }
    public int Delete(string code)
    {
        const string sql = "DELETE FROM Students WHERE StudentCode=@Code";
        return ExecuteWrite(sql, cmd => cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = code, "Delete", code);
    }

    private int ExecuteWrite(string sql, Action<SqlCommand> configure, string operation, string code)
    {
        try
        {
            using var cn = new SqlConnection(_connectionString); using var cmd = new SqlCommand(sql, cn);
            configure(cmd); cn.Open(); var affected = cmd.ExecuteNonQuery();
            if (affected > 0) _logger.LogInformation(new EventId(1100, operation), "{Operation} succeeded for StudentCode {StudentCode}; affected {AffectedRows} row(s)", operation, code, affected);
            else _logger.LogWarning(new EventId(1101, operation + "NotFound"), "{Operation} found no student for StudentCode {StudentCode}", operation, code);
            return affected;
        }
        catch (Exception ex) { _logger.LogError(new EventId(1999, operation + "Failed"), ex, "{Operation} failed for StudentCode {StudentCode}", operation, code); throw; }
    }
    private static void AddParameters(SqlCommand cmd, StudentDTO s)
    {
        cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = s.StudentCode.Trim();
        cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 30).Value = DbValue(s.Phone);
        cmd.Parameters.Add("@Class", SqlDbType.NVarChar, 100).Value = DbValue(s.ClassName);
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = s.FullName.Trim();
        cmd.Parameters.Add("@Dob", SqlDbType.Date).Value = s.DateOfBirth.Date;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = DbValue(s.Email);
    }
    private static object DbValue(string? value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
    private static StudentDTO Map(SqlDataReader r) => new()
    {
        StudentCode = r["StudentCode"]?.ToString() ?? "", Phone = r["Phone"]?.ToString() ?? "",
        ClassName = r["Class"]?.ToString() ?? "", FullName = r["FullName"]?.ToString() ?? "",
        DateOfBirth = r["DateOfBirth"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(r["DateOfBirth"]),
        Email = r["Email"]?.ToString() ?? ""
    };
}
