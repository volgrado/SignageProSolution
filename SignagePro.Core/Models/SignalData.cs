namespace SignagePro.Core.Models
{
    /// <summary>
    /// Representa los datos de una señal de tráfico.
    /// Es una clase simple (POCO - Plain Old CLR Object).
    /// </summary>
    public class SignalData
    {
        // PROPIEDADES AÑADIDAS:
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public double Width { get; set; }
        public double Height { get; set; }
    }
}

