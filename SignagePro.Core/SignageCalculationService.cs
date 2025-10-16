using SignagePro.Core.Contracts;
using SignagePro.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace SignagePro.Core
{
    public class SignageCalculationService
    {
        private readonly ISignalRepository _signalRepository;

        // Inyectamos la dependencia a través del constructor.
        public SignageCalculationService(ISignalRepository signalRepository)
        {
            _signalRepository = signalRepository;
        }

        public SignalData? GetSignalData(string signalCode)
        {
            return _signalRepository.GetAllSignals().FirstOrDefault(s => s.Code == signalCode);
        }

        public List<SignalData> GetAllSignals()
        {
            return _signalRepository.GetAllSignals();
        }
    }
}