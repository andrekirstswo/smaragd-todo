namespace Core.Models;

public class TaskItem
{
    public string Id { get; }
    public string Name { get; }
    public string SectionId { get; set; }

    public TaskItem(string id, string name, string sectionId)
    {
        Id = id;
        Name = name;
        SectionId = sectionId;
    }
}