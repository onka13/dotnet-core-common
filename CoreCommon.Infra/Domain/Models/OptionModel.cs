namespace CoreCommon.Infra.Domain.Models
{
    public class OptionModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class IdNameModel<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }

        public IdNameModel()
        {

        }

        public IdNameModel(T id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class IdTitleModel<T>
    {
        public T Id { get; set; }
        public string Title { get; set; }

        public IdTitleModel()
        {

        }

        public IdTitleModel(T id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
