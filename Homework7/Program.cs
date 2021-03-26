using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using Blog;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Homework7
{
    class Program
    {
        public class SearchJson
        {
            public int place_id { get; set; }
            public string licence { get; set; }
            public string osm_type { get; set; }
            //public int osm_id { get; set; }
            public List<string> boundingbox { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }
            public string display_name { get; set; }
            public string @class { get; set; }
            public string type { get; set; }
            public double importance { get; set; }
            public string icon { get; set; }
        }

        static async Task Main(string[] args)
        {
            using (Context context = new Context())
            {
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                Console.Write("Enter fullname: ");
                string fullname = Console.ReadLine();
                Console.Write("Enter Country: ");
                string country = Console.ReadLine();
                Console.Write("Enter City: ");
                string city = Console.ReadLine();
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    HttpResponseMessage pointsResponse =
                        await client.GetAsync("https://nominatim.openstreetmap.org/search?country=" + country + "&city="
                                              + city + "&format=jsonv2");
                    if (pointsResponse.IsSuccessStatusCode)
                    {
                        if (pointsResponse.Content.Headers.ContentLength != 2)
                        {
                            List<SearchJson> roots = await pointsResponse.Content.ReadFromJsonAsync<List<SearchJson>>();

                            if (roots.Count() > 0 && roots.First().type == "city")
                            {
                                IQueryable<UserData> users = from user in context.Users
                                    where (user.FullName == fullname)
                                    select user;
                                if (users.Count() > 0)
                                {
                                    Console.WriteLine("User is exists");
                                }
                                else
                                {
                                    IDbContextTransaction transaction =
                                        await context.Database.BeginTransactionAsync();
                                    try
                                    {
                                        Console.WriteLine($"Registering user");
                                        UserData newUser = new UserData()
                                            {FullName = fullname, Country = country, City = city};
                                        context.Users.Add(newUser);
                                        await transaction.CommitAsync();
                                        await context.SaveChangesAsync();
                                        Console.WriteLine("SUCCESS");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("FAILED");
                                    }

                                }
                            }
                            else
                            {
                                Console.WriteLine("Bad city");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bad country");
                        }
                    }
                    else
                    {
                        string resp = await pointsResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resp);
                    }
                }
            }
        }
    }
}