using DukcapilWS.ApiServices.Entity;
using System.Collections.Generic;

namespace DukcapilWS.ApiServices.Entity
{
    public class BaseEntity
    {

        public List<KTP> content { get; set; }

        public bool lastPage { get; set; }

        public int numberOfElements { get; set; }

        public string sort { get; set; }

        public int totalElements { get; set; }

        public bool firstPage { get; set; }

        public int number { get; set; }

        public int size { get; set; }
    }
}
