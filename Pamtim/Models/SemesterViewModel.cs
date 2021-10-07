using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Pamtim.Models
{
    public class SemesterViewModel
    {
        public string ID_Semester { get; set; }
        public string Name { get; set; }
    }

    public class ListofSemesterViewModel
    {
        public List<SemesterViewModel> Items { get; set; }
    }

    public class EditSemesterViewModel
    {
        public string User_ID { get; set; }
        public int ID_Semester { get; set; }

        [Required]
        [Display(Name = "Naziv")]
        public string Name { get; set; }
    }
}