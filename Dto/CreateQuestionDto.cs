using System.ComponentModel.DataAnnotations;

namespace EmptyDotNetWebAPI2.Dtos;

public record class CreateQuestionDto(
    [Required]bool Answered, 
    [Required][StringLength(200)]string Question, 
    [StringLength(200)]string Answer,
    [Required][StringLength(50)]string Type
    );
