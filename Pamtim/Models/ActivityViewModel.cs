using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pamtim.Models
{
    public class ActivityViewModel
    {
        public string ID_Activity { get; set; }
        public string Name { get; set; }
        public string Course_ID { get; set; }
        public int AllowedAbsence { get; set; }
        public int CountAbsence { get; set; }
    }

    public class ListofActivityViewModel
    {
        public List<ActivityViewModel> Items { get; set; }
    }

   

    public class EditActivityViewModel
    {

        public string Course_ID { get; set; }

        public int ID_Activity { get; set; }

        public int CountAbsence { get; set; }

        [Required]
        [Display(Name = "Naziv")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Dozvoljeno izostanaka")]
        public int AllowedAbsence { get; set; }
    }
}