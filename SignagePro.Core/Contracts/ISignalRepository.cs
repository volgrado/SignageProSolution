using SignagePro.Core.Models;
using System.Collections.Generic;

namespace SignagePro.Core.Contracts
{
    public interface ISignalRepository
    {
        List<SignalData> GetAllSignals();
    }
}