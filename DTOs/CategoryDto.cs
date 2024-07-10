namespace ProductManagement.DTOs;

public class CategoryDto
{
    public class CategoryData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class CategoryCreate
    {
        public string Name { get; set; }
    }
    
    public class CategoryUpdate
    {
        public string Name { get; set; }
    }

}