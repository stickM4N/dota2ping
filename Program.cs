using System.Net.NetworkInformation;
using System.Text;
using dota2ping;


var clusters = new Dota2Clusters();

var buffer = Encoding.ASCII.GetBytes(new string('\0', 32));
const int timeout = 2;

List<Tuple<string, string, string, List<long>>> list = new();

foreach (var cluster in clusters)
{
    foreach (var relay in cluster.Relays)
    {
        list.Add(Tuple.Create<string, string, string, List<long>>(
            cluster.Id, cluster.Name, relay, new List<long>())
        );
    }
}

Tuple<long, int> getStats(List<long> pings)
{
    short lost = 0;
    foreach (var ping in pings)
    {
        if (ping == 0)
            lost += 1;
    }

    var div = pings.Count - lost;
    return Tuple.Create(pings.Sum() / (div != 0 ? div : 1), lost / pings.Count * 100);
}


while (true)
{
    Parallel.ForEach(list, e =>
    {
        var pingSender = new Ping();
        var reply = pingSender.Send(e.Item3, timeout, buffer);
        e.Item4.Add(reply.Status == IPStatus.Success ? reply.RoundtripTime : 0);
    });

    Console.Clear();
    foreach (var l in list)
    {
        var stats = getStats(l.Item4);
        Console.WriteLine("{1} [{0}] @ {2} ---> {3:F2}ms [{4}% loss]", l.Item1, l.Item2, l.Item3, stats.Item1, stats.Item2);
    }
}