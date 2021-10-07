using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Pamtim.Models
{
    public class EventViewModel
    {
        public string ID_Event { get; set; }
        public string Activity_ID { get; set; }

        
        public TimeSpan Start { get; set; }

        public string Location { get; set; }
        public TimeSpan Finish { get; set; }

        public string Day { get; set; }


        public string CourseName { get; set; }
        public string ActivityName { get; set; }
    }



    public class ListofEventViewModel
    {
        public List<EventViewModel> Items { get; set; }
    }

    public class EditEventViewModel
    {
        public string ID_Event { get; set; }

        public string Activity_ID { get; set; }

        [Display(Name = "Lokacija")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Početak")]
        [DataType(DataType.Time)]
        public TimeSpan Start { get; set; }

        [Required]
        [Display(Name = "Završetak")]
        [DataType(DataType.Time)]
        public TimeSpan Finish { get; set; }

        [Required]
        [Display(Name = "Dan")]
        public string Day { get; set; }

        public IEnumerable<SelectListItem> Days { get; set; }
    }

    public class RasporedViewModel
    {
        public List<ListofEventViewModel> Items { get; set; }
    }
}