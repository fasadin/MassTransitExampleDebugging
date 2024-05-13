using System.ComponentModel.DataAnnotations;

namespace MassTransitExample;

public class User
{
    [Key]
    public Guid Id { get; set; }
}
