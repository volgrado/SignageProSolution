using SignagePro.Core.Models;

namespace SignagePro.Core
{
    public class SignageCalculationService
    {
        private readonly List<SignalData> _signalData = new List<SignalData>
        {
            new SignalData { Code = "S-01", Name = "Señal de Stop", Width = 60, Height = 60 },
            new SignalData { Code = "R-101", Name = "Velocidad Máxima 50", Width = 70, Height = 70 },
            new SignalData { Code = "P-21", Name = "Peligro Niños", Width = 80, Height = 80 }
        };

        public SignalData? GetSignalData(string signalCode)
        {
            return _signalData.FirstOrDefault(s => s.Code == signalCode);
        }

        // MÉTODO AÑADIDO: Devuelve la lista completa de señales.
        public List<SignalData> GetAllSignals()
        {
            return _signalData;
        }
    }
}

