using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

