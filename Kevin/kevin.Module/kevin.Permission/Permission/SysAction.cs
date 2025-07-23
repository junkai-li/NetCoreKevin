using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kevin.Permission.Permisson
{
    /// <summary>
    /// FrameworkAction
    /// </summary>
    public class SysAction
    {
        [Display(Name = "ActionName")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Required(ErrorMessage = "{0}required")]
        public string ActionName { get; set; }

        [Display(Name = "ActionName")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Required(ErrorMessage = "{0}required")]
        public string Action { get; set; }

        [Required(ErrorMessage = "{0}required")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Display(Name = "MethodName")]
        public string MethodName { get; set; }
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Display(Name = "MethodName")]
        public string Method { get; set; }

        public string HttpMethod { get; set; }


        [Display(Name = "Module")]
        public SysCtrl Controller { get; set; }

        [Display(Name = "Parameter")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string Parameter { get; set; }

        [NotMapped]
        public List<string> ParasToRunTest { get; set; }

        [NotMapped]
        public bool IgnorePrivillege { get; set; }

        [NotMapped]
        private string _url;
        [NotMapped]
        public string Url
        {
            get
            {
                if (_url == null)
                {
                    if (this.Controller.Area != null)
                    {
                        _url = "/" + this.Controller.Area.Prefix + "/" + this.Controller.ClassName + "/" + this.MethodName;
                    }
                    else
                    {
                        _url = "/" + this.Controller.ClassName + "/" + this.MethodName;
                    }
                }
                return _url;
            }
            set
            {
                _url = value;
            }
        }
    }
}
