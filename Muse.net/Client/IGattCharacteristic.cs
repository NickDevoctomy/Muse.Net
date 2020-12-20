using System.Threading.Tasks;

namespace Muse.Net.Client
{
    public interface IGattCharacteristic
    {
        Task<bool> WriteCommand(string command);
    }
}
