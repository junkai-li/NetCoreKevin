using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos
{
    public class dtoId<T>
    {
        /// <summary>
        /// id
        /// </summary>
        [Required(ErrorMessage = "id不可以为空")]
        public T Id { get; set; } = default!;
    }
}
