using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Pamtim.Models
{
    public class ScheduleViewModel
    {
        public string ID_Event { get; set; }
        public string Activity_ID { get; set; }



        public TimeSpan Start { get; set; }


        public TimeSpan Finish { get; set; }

        public string Day { get; set; }


        public string Location { get; set; }

        public string CourseName { get; set; }
        public string ActivityName { get; set; }
        public int AllowedAbsence { get; set; }
        public int CountAbsence { get; set; }
    }



    public class ListofScheduleViewModel
    {
        public List<ScheduleViewModel> Items { get; set; }
    }

}