namespace RatePokemonApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<PokemonCategory> PokemonCategories { get; set; }
    }
}
