namespace PMSCore.ViewModel;

public class AreaDetails
{
    public List<SectionDetails> sections { get; set; }
    public List<TableDetails> tables { get; set; }
}

public class TableDetails
{
    public int TableId { get; set; } = 0;
    public string TableName { get; set; } = null!;
    public int SectionId { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = null!;
    public int editorId { get; set; } = 0;
}


public class SectionDetails
{
    public int SectionId { get; set; }
    public string SectionName { get; set; } = null!;
    public string? Description { get; set; }
    public int editorId { get; set; } = 0;
    
}