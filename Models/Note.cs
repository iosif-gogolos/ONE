using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ONE.Models;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
    public List<Note> ChildNotes { get; set; } = new();
    public Guid? ParentId { get; set; }
    
    [JsonIgnore]
    public int Level { get; set; } = 0;
}
