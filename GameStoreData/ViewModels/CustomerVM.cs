using System.ComponentModel.DataAnnotations;

namespace GameStoreData.ViewModels
{
    public enum PaymentType
    {
        Card,
        Cash
    }

    public class CustomerVM
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public PaymentType PaymentType { get; set; }

        [MaxLength(600)]
        public string? Comment { get; set; }
    }
}

