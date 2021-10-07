using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pamtim.Models
{
    public class AbsenceViewModel
    {
        public string ID_Absence { get; set; }
        public string Activity_ID { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime AbsenceDate { get; set; }
        public string Description { get; set; }
        public string CourseName { get; set; }
        public string ActivityName { get; set; }
    }

    public class ListofAbsenceViewModel
    {
        public List<AbsenceViewModel> Items { get; set; }
    }



    public class EditAbsenceViewModel
    {


        public string ID_Absence { get; set; }

        public string Activity_ID { get; set; }

        [Required]
        [Display(Name = "Datum kreiranja")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Required]
        [Display(Name = "Datum izostanka")]
        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AbsenceDate { get; set; }

        [Required]
        [Display(Name = "Opis")]
        public string Description { get; set; }



    }
}