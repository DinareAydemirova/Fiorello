using System.Collections;
using System.Collections.Generic;

namespace Fiorello.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeactive { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
