using Shared.Enums;

namespace Shared.Models;

public class QueueMessageModel<T>
{
    public Guid Id { get; set; }           
    public OperationType Operation { get; set; }

    /*use all kinds of data*/
    public T Data { get; set; }             
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
