using System;

namespace GoFla.API.DTOs.Category;

public record UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
