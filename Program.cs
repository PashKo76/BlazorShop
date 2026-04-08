using BlazorShop.Components;
using System.Text.Json;

namespace BlazorShop
{
    public class Program
    {
        static string? Dirr = null;
        public static List<WebProduct> products = new();
        public static LinkedList<string> categories = new LinkedList<string>();
        public static readonly WebProduct PlaceHolder = new WebProduct()
        {
            Id = -1,
            Reviews = new(),
            Product = new Product()
            {
                Name = "PlaceHolder",
                ShortDescription = "No sh desc",
                Description = "no desc",
                Price = -1,
                Categories = new string[0]
            }
        };
        static Queue<(string review, int prodId)> unChecked = new();
        public static void SendToCheck(string reviewText, int prodId)
        {
            unChecked.Enqueue((reviewText, prodId));
        }
        static void LoadProducts()
        {
            Dirr = Directory.GetCurrentDirectory() + "/wwwroot/PreloadedProducts";
            foreach (var item in Directory.GetDirectories(Dirr))
            {
                Product? pr = null;
                try
                {
                    pr = JsonSerializer.Deserialize<Product>(File.ReadAllText(item + "/data.json"));
                }
                catch
                {
                    pr = null;
                }
                if (pr == null) continue;
                products.Add(new WebProduct() { Id = products.Count, Product = pr, Reviews = new() });
                foreach (var cat in pr.Categories)
                {
                    if (categories.Contains(cat)) continue;
                    categories.AddLast(cat);
                }
            }
        }
        public static void Main(string[] args)
        {
            try
            {
                LoadProducts();
            }
            catch(Exception ex)
            {
                Console.WriteLine(Directory.GetCurrentDirectory());
                Console.WriteLine(ex.Message);
            }
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
            
        }
    }
    public record class Product
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string[] Categories { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
    public record class WebProduct
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public LinkedList<string> Reviews { get; set; }
    }
}
