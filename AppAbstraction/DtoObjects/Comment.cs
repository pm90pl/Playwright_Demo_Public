namespace BBlog.Tests.AppAbstraction.DtoObjects;

public class Comment
{
    public string Body { get; set; }
    public string Id { get; set; }
    public Author Author { get; set; }
}