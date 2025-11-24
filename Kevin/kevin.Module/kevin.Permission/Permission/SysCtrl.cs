 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kevin.Permission.Permisson
{
    /// <summary>
    /// FrameworkModule
    /// </summary> 
    public class SysCtrl
    {

        public   string? _name;

        public   string? Module { get; set; }
        public string ModuleName
        {
            get
            {
                string rv = "";
                //if (ActionDes?._localizer != null && string.IsNullOrEmpty(ActionDes?.Description) == false)
                //{
                //    if (ActionDes._localizer[ActionDes.Description].ResourceNotFound == true)
                //    {
                //        rv =  Core.Program._localizer[ActionDes.Description];
                //    }
                //    else
                //    {
                //        rv = ActionDes._localizer[ActionDes.Description];
                //    }
                //}
                //else
                //{
                rv = _name ?? "";
                //}

                if (IsApi == true)
                {
                    //rv += "(api)";
                }
                return rv;

            }
            set
            {
                _name = value;
            }
        }



        [Required(ErrorMessage = "{0}required")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Display(Name = "ClassName")]
        public   string? ClassName { get; set; }

        [Display(Name = "Action")]
        public   List<SysAction>? Actions { get; set; }

        [Display(Name = "Area")]
        public   SysArea? Area { get; set; }

        public   string? FullName { get; set; }

        [NotMapped]
        public bool IgnorePrivillege { get; set; }

        [NotMapped]
        public bool IsApi { get; set; }
    }
}
