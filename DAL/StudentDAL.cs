
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using StudentManagement.DTO;

namespace StudentManagement.DAL
{
    // DAL chỉ làm việc với SQL Server, không chứa code giao diện.
    public class StudentDAL
    {
        // Đổi tên server nếu SQL Server trên máy có tên khác.
        private readonly string connectionString =
            @"Server=Data Source=localhost\SQLEXPRESS;Database=StudentDB;Integrated Security=True;";

        public List<StudentDTO> GetAll()
        {
            List<StudentDTO> list = new List<StudentDTO>();

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                const string sql =
                    "SELECT Id, Code, FullName, ClassName, Major " +
                    "FROM Students ORDER BY Id";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new StudentDTO
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Code = Convert.ToString(reader["Code"]) ?? "",
                                FullName = Convert.ToString(reader["FullName"]) ?? "",
                                ClassName = Convert.ToString(reader["ClassName"]) ?? "",
                                Major = Convert.ToString(reader["Major"]) ?? ""
                            });
                        }
                    }
                }
            }

            return list;
        }

        public List<StudentDTO> Search(string code)
        {
            List<StudentDTO> list = new List<StudentDTO>();

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                const string sql =
                    "SELECT Id, Code, FullName, ClassName, Major " +
                    "FROM Students WHERE Code LIKE @Code ORDER BY Id";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Code", SqlDbType.NVarChar, 20)
                        .Value = (code ?? "") + "%";

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new StudentDTO
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Code = Convert.ToString(reader["Code"]) ?? "",
                                FullName = Convert.ToString(reader["FullName"]) ?? "",
                                ClassName = Convert.ToString(reader["ClassName"]) ?? "",
                                Major = Convert.ToString(reader["Major"]) ?? ""
                            });
                        }
                    }
                }
            }

            return list;
        }

        public int Insert(StudentDTO s)
        {
            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                const string sql =
                    "INSERT INTO Students (Code, FullName, ClassName, Major) " +
                    "VALUES (@Code, @Name, @Class, @Major)";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    AddParameters(command, s);

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Update(StudentDTO s)
        {
            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                const string sql =
                    "UPDATE Students SET FullName=@Name, " +
                    "ClassName=@Class, Major=@Major WHERE Id=@Id";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int)
                        .Value = s.Id;

                    command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
                        .Value = s.FullName;

                    command.Parameters.Add("@Class", SqlDbType.NVarChar, 50)
                        .Value = DbValue(s.ClassName);

                    command.Parameters.Add("@Major", SqlDbType.NVarChar, 100)
                        .Value = DbValue(s.Major);

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Delete(int id)
        {
            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                const string sql =
                    "DELETE FROM Students WHERE Id=@Id";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int)
                        .Value = id;

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        private static void AddParameters(
            SqlCommand command, StudentDTO s)
        {
            command.Parameters.Add("@Code", SqlDbType.NVarChar, 20)
                .Value = s.Code;

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
                .Value = s.FullName;

            command.Parameters.Add("@Class", SqlDbType.NVarChar, 50)
                .Value = DbValue(s.ClassName);

            command.Parameters.Add("@Major", SqlDbType.NVarChar, 100)
                .Value = DbValue(s.Major);
        }

        private static object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? (object)DBNull.Value
                : value;
        }
    }
}
