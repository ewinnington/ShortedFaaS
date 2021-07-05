using System.Collections.Generic;
using System.Collections.Concurrent; 

namespace ewinnington.ShortedFaas {
/* for later 

    public class FunctionsRegister 
    {
        public ConcurrentDictionary<string, HostedFunction> HostedFunctions; 
    }
*/

    public record HostedFunction(string Name, string Cli, List<string> Arguments, Dictionary<string, string> Environment); 
}