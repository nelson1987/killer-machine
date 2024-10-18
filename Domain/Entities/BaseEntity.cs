namespace Domain.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Ativo = true;
        }

        public int Id { get; set; }
        public bool Ativo { get; set; }
    }
}