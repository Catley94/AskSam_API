using System.ComponentModel.DataAnnotations;

namespace AskSam.Dtos;

public record class CreateQuestionDto(
    [Required]Guid ClientGuid,
    [Required]bool Answered, 
    [Required][StringLength(200)]string Question, 
    [StringLength(200)]string Answer,
    [Required][StringLength(50)]string Type
    );
