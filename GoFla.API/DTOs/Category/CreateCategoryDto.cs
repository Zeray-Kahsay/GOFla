using System;

namespace GoFla.API.DTOs.Category;

public record CreateCategoryDto
{
    public string  Name  { get; set; } = string.Empty;
    public int  SortOrder  { get; set; }
}
