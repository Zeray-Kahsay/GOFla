using System;

namespace GoFla.API.DTOs.Category;

public record CategoryDto
{
    public int  Id  { get; set; }
    public string  Name  { get; set; } = string.Empty;
    public int  SortOrder  { get; set; } 
}
