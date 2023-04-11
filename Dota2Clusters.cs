using System.Text.Json;
using System.Text.Json.Nodes;

namespace dota2ping;

public class Cluster
{
    public string Id;
    public string Name;
    public List<string> Relays = new();


    public Cluster(string id, JsonNode data)
    {
        Id = id;
        Name = data.AsObject()["desc"]!.ToString();

        if (!data.AsObject().ContainsKey("relays"))
            return;

        foreach (var relay in data.AsObject()["relays"]?.AsArray()!)
        {
            Relays.Add(relay!.AsObject()["ipv4"]!.ToString());
        }
    }
}

public class Dota2Clusters : List<Cluster>
{
    private const string ClustersUri = "https://api.steampowered.com/ISteamApps/GetSDRConfig/v1?appid=570";

    public Dota2Clusters()
    {
        try
        {
            HttpClient client = new();

            var response = client.GetAsync(ClustersUri).Result;
            response.EnsureSuccessStatusCode();
            var responseBody = response.Content.ReadAsStringAsync().Result;

            var pops = JsonSerializer.Deserialize<JsonObject>(responseBody)!.AsObject()["pops"]!;
            foreach (var e in pops.AsObject())
                Add(new Cluster(e.Key, e.Value!));
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Exception: {0}", e.Message);
        }
    }
}