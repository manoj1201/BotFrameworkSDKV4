using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Search
{
    public partial class ContentSearchLuisRecognizer
    {
        public string DocumentNameEntities
        {
            get
            {
                var documentName = Entities?._instance?.DocumentName?.FirstOrDefault()?.Text;
                return documentName;
            }
        }
        public string DesignationEntities
        {
            get
            {
                var designationValue = Entities?._instance?.Designation?.FirstOrDefault()?.Text;
                return designationValue;
            }
        }
        public string PersonNameEntities
        {
            get
            {
                var personName = Entities?._instance?.personName?.FirstOrDefault()?.Text;
                return personName;
            }
        }
        public string DepartmentEntities
        {
            get
            {
                var departmentValue = Entities?._instance?.Department?.FirstOrDefault()?.Text;
                return departmentValue;
            }
        }
    }
}
