using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_Models
{
    public  class Product
    {
        public int id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description{ get; set; }
        [Required]
        public string Author{ get; set; }
        [Required]
        public string Isbn { get; set; }
        [Required]
        [Range(1, 1000)]
        public double Listprice { get; set; }
        [Required]
        [Range (1, 1000)]
        public double price { get; set; }
        [Required, Range(1, 1000)]
        public double price50 { get; set; }
        [Required]
        [Range(1, 1000)]
        public double  price100 { get; set; }
        
        [Display(Name ="Imageurl")]
        public string Imageurl { get; set; }//image path
        [Required]
        [Display(Name ="Category")]
        public int categoryid { get; set; }
        public Category category { get; set; }//fk
        [Required]
        [Display(Name ="CoverType")]
        public int covertypeid { get; set; }
        public CoverType coverType { get; set; }//fk
    }
}
