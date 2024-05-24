namespace API.DTOs;

public class CardDto
{
    
    public uint CardId { get; set; }
    public string CardName { get; set; }
    
    public uint OwnerId { get; set; }

    public ulong IsPublic { get; set; }

    public ulong IsTemplate { get; set; }

    public ulong IsArchived { get; set; }

    public double? EstimatedPrice { get; set; }

    public ColorDto Color { get; set; }
}