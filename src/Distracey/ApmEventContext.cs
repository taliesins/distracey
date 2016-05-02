namespace Distracey
{
    public class ApmEvent<T>
    {
        public T Event { get; set; }
        public IApmContext ApmContext { get; set; }
    }
}
