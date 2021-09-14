using System.ComponentModel.DataAnnotations;
using RepoDb.Attributes;

namespace EasyInsur.Models
{
    public class Person
    {
        public Person()
        {
        }

        public Person(string firstName, string lastName, string type, string personId, string imagePath,
            string regDate, string mobile, string address = null!, string email = null!)
        {
            FirstName = firstName;
            LastName = lastName;
            Type = type;
            PersonID = personId;
            ImagePath = imagePath;
            RegDate = regDate;
            Mobile = mobile;
            Address = address;
            Email = email;
        }
        public Person(string firstName, string lastName, string type, string personId, double balance, string imagePath,
            string regDate, string mobile, string address = null!, string email = null!)
        {
            FirstName = firstName;
            LastName = lastName;
            Type = type;
            PersonID = personId;
            Balance = balance;
            ImagePath = imagePath;
            RegDate = regDate;
            Mobile = mobile;
            Address = address;
            Email = email;
        }

        [Identity] // Identity decoration
        public long? Id { get; set; } = null;
        public string Type { get; set; }
        public string PersonID { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        [Display(GroupName = "Sales Details")]
        public string FirstName { get; set; }
        [Display(GroupName = "Sales Details")]
        public string LastName { get; set; }
        public double Balance { get; set; }
        public string ImagePath { get; set; }
        public string Mobile { get; set; }
        public string RegDate { get; set; }
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
