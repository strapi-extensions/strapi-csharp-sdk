namespace Todo.Proxy.Contracts.API
{
    public class CreateTodoForUserRequest
    {
        public string Title { get; set; }
        public bool Completed { get; set; }
        public DateTime Due { get; set; }
    }
}
