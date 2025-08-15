using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos
{
    public class dtoId<T>
    {
        /// <summary>
        /// id
        /// </summary>
        [Required(ErrorMessage = "id不可以为空")]
        public T Id { get; set; }
    }
}
