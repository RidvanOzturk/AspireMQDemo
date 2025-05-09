using Shared.Enums;

namespace Shared.Models;

public class QueueMessageModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public OperationType Operation { get; set; }
}
