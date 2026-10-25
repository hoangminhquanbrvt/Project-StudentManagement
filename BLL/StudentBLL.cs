
﻿using System;
using System.Collections.Generic;
using StudentManagement.DAL;
using StudentManagement.DTO;

namespace StudentManagement.BLL
{
    // BLL xử lý quy tắc nghiệp vụ.
    // Không viết SQL và không điều khiển giao diện WinForms.
    public class StudentBLL
    {
        private readonly StudentDAL dal = new StudentDAL();

        public List<StudentDTO> GetStudents()
        {
            return dal.GetAll();
        }

        public List<StudentDTO> SearchStudents(string code)
        {
            return dal.Search((code ?? "").Trim());
        }

        public void AddStudent(StudentDTO student)
        {
            CheckStudent(student);
            dal.Insert(student);
        }

        public bool UpdateStudent(StudentDTO student)
        {
            if (student == null)
                throw new ArgumentNullException("student");

            if (student.Id <= 0)
                throw new ArgumentException("Hãy chọn sinh viên cần sửa.");

            CheckStudent(student);

            return dal.Update(student) == 1;
        }

        public bool DeleteStudent(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Hãy chọn sinh viên cần xóa.");

            return dal.Delete(id) == 1;
        }

        private static void CheckStudent(StudentDTO s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            s.Code = (s.Code ?? "").Trim();
            s.FullName = (s.FullName ?? "").Trim();
            s.ClassName = (s.ClassName ?? "").Trim();
            s.Major = (s.Major ?? "").Trim();

            if (string.IsNullOrWhiteSpace(s.Code) ||
                string.IsNullOrWhiteSpace(s.FullName))
            {
                throw new ArgumentException(
                    "Mã sinh viên và họ tên không được để trống.");
            }

            if (!s.Code.StartsWith("SV", StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "Mã sinh viên phải bắt đầu bằng SV.");
            }

            if (s.Code.Length > 20 ||
                s.FullName.Length > 100 ||
                s.ClassName.Length > 50 ||
                s.Major.Length > 100)
            {
                throw new ArgumentException(
                    "Thông tin vượt quá độ dài cho phép.");
            }
        }
    }
}
