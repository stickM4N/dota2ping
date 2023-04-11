using System.Net.NetworkInformation;
using System.Text;
using dota2ping;


var clusters = new Dota2Clusters();

var pingSender = new Ping();

var buffer = Encoding.ASCII.GetBytes(new string('\0', 32));
const int timeout = 2;

foreach (var cluster in clusters)
{
    Console.WriteLine("{0} [{1}]", cluster.Name, cluster.Id);

    foreach (var relay in cluster.Relays)
    {
        var reply = pingSender.Send(relay, timeout, buffer);

        if (reply.Status == IPStatus.Success)
        {
            Console.WriteLine("{0} -> {1}ms", reply.Address, reply.RoundtripTime);
        }
        else
        {
            Console.WriteLine("{0} -> failure", reply.Address);
        }
    }

    Console.WriteLine("-------------------------------------");
}