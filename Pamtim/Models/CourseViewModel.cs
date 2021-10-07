using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pamtim.Models
{
    public class CourseViewModel
    {
        public string ID_Course { get; set; }
        public string Name { get; set; }
        public string Semester_ID { get; set; }
    }

    public class ListofCourseViewModel
    {
        public List<CourseViewModel> Items { get; set; }
    }

    public class EditCourseViewModel
    {

        public string Semester_ID { get; set; }

        public int ID_Course { get; set; }

        [Required]
        [Display(Name = "Naziv")]
        public string Name { get; set; }
    }
}