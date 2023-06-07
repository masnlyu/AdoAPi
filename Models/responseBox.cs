using Reference;
public class ResponseBox<T>
{
    public StateReference Status { get; set; }
    public T? Result { get; set; }
    public string Text { get; set; } = "";
}