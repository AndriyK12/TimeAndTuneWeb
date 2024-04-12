namespace EFCore;

public partial class Task
{
    public int Taskid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly Dateofcreation { get; set; }

    public DateOnly Expectedfinishtime { get; set; }

    public DateOnly? Finishtime { get; set; }

    public int Priority { get; set; }

    public bool? Completed { get; set; }

    public TimeSpan? Executiontime { get; set; }

    public int? Useridref { get; set; }

    public virtual User? UseridrefNavigation { get; set; }
}
