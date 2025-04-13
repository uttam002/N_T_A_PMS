using Microsoft.AspNetCore.Http;

namespace PMSCore.ViewModel
{
    public class MenuDetails
    {
        public List<CategoryDetails> categories { get; set; }
        public List<ItemDetails> items { get; set; }
        public List<ModifierGropDetails> modifierGrops { get; set; }
        public List<ModifierDetails> modifiers { get; set; }

    }

    public class ModifierDetails
    {
        public int id { get; set; }
        public int groupId { get; set;}
        public string modifierName { get; set; } = null!;
        public decimal unitPrice { get; set; }
        public int quantity { get; set; }
        public string unitType { get; set; } =null!;
        public string description {get;set;}

    }


    public class ModifierGropDetails
    {
        public int id { get; set; }
        public string modifierGroupName { get; set; } = null!;
        public string? description { get; set;}
    }


    public class CategoryDetails
    {
        public int id { get; set; }=0;
        public string categoryName { get; set; } =  null!;
        public string? description { get; set;}
        public int editorId { get; set; }=0;
    }


    public class ItemDetails
    {
        public int id { get; set; }
        public string itemName { get; set; } = null!;
        public string itemType { get; set; } = null!;
        public decimal unitPrice { get; set; }
        public int quantity { get; set; }
        public string unitType { get; set; } = null!;
        public bool isAvailable { get; set; }
        // public IFormFile? photo { get; set; }
        public string? photo { get; set; } 
    }
}