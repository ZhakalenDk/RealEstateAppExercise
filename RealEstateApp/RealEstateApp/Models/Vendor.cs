using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Vendor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
