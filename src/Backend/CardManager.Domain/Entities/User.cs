namespace CardManager.Domain.Entities
{
    public class User : EntityBase
    {
        public string? Name { get; private set; }
        public string? Email { get; private set; }
        public string? PasswordHash { get; private set; }

        protected User() { }
        public User(string name, string email, string passwordHash)
        {
            SetName(name);
            SetEmail(email);
            SetPasswordHash(passwordHash);
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 6)
                throw new ArgumentException("Nome não pode ser nulo e tem que ter no minimo 6 caracteres.");

            Name = name;
        }
        private void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Email não pode ser nulo e deve conter @.");

            Email = email;
        }
        private void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Senha não pode ser nula.");

            PasswordHash = passwordHash;
        }
    }
}
