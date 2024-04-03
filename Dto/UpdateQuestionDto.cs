using System.ComponentModel.DataAnnotations;

namespace AskSam.Dtos;

public record class UpdateQuestionDto(
    [Required]int Id, 
    [Required]Guid Guid,
    [Required]bool Answered, 
    [Required][StringLength(200)]string Question, 
    [Required][StringLength(200)]string Answer,
    [Required][StringLength(50)]string Type,
    [Required]DateOnly DateCreated
    );
