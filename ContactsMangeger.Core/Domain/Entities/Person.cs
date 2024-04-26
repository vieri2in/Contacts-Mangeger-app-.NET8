using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; }
        [StringLength(40)]
        public string? PersonName { get; set; }
        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)]
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }
        public bool? ReceiveNewLetters { get; set; }
        public string? TIN { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country? Country { get; set; }
        public override string ToString()
        {
            return $"PersonId:{PersonId}, PersonName:{PersonName},Email:{Email}, DateTime:{DateOfBirth.ToString()}, Gender:{Gender}, Country: {Country.CountryName}, Adress: {Address}, TIN: {TIN}, ReceiveNewLetters: {ReceiveNewLetters}";
        }
    }
}
