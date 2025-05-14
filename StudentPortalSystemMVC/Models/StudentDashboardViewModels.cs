using System;
using System.Collections.Generic;

namespace StudentPortalSystemMVC.Models
{
    public class CourseViewModel
    {
        public int COURSE_ID { get; set; }
        public string COURSE_NAME { get; set; }
        public string TeacherName { get; set; }
        public int CREDITS { get; set; }
    }

    public class AttendanceViewModel
    {
        public string COURSE_NAME { get; set; }
        public DateTime ATTENDANCE_DATE { get; set; }
        public string Status { get; set; }
    }

    public class GradeViewModel
    {
        public string COURSE_NAME { get; set; }
        public double? MARKS { get; set; }
        public string GRADE { get; set; }
    }

    public class FeeViewModel
    {
        public string COURSE_NAME { get; set; }
        public double AMOUNT { get; set; }
        public string Status { get; set; }
    }

    public class TranscriptViewModel
    {
        public string COURSE_NAME { get; set; }
        public double? MARKS { get; set; }
        public string GRADE { get; set; }
        public double SGPA { get; set; }
        public double CGPA { get; set; }
    }

    // Teacher-specific view models
    public class TeacherDashboardViewModel
    {
        public USER Teacher { get; set; }
        public string Department { get; set; }
        public List<CourseViewModel> Courses { get; set; }
    }

    public class StudentAttendanceViewModel
    {
        public int ATTENDANCE_ID { get; set; }
        public int ENROLLMENT_ID { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; }
    }

    public class StudentGradeViewModel
    {
        public int GRADE_ID { get; set; }
        public int ENROLLMENT_ID { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public int? Marks { get; set; }
        public string Grade { get; set; }
    }

    public class CourseReportViewModel
    {
        public string CourseName { get; set; }
        public int TotalStudents { get; set; }
        public double AverageAttendance { get; set; }
        public double AverageMarks { get; set; }
    }
}