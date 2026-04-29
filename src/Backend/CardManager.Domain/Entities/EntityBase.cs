namespace CardManager.Domain.Entities
{
    public class EntityBase
    {
        public long Id { get; protected set; }
        public bool Active { get; protected set; }
        public DateTime CreatedOn { get; protected set; }

        protected EntityBase()
        {
            Active = true;
            CreatedOn = DateTime.UtcNow;
        }
        public void Activated() => Active = true;
        public void Deactivated() => Active = false;
    }
}
