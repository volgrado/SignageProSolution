using SignagePro.Core.Models;

namespace SignagePro.Core.Contracts
{
    /// <summary>
    /// Define el contrato para las operaciones que interactúan con AutoCAD.
    /// </summary>
    public interface IAutoCADAdapter
    {
        /// <summary>
        /// Dibuja una señal en AutoCAD basada en sus datos.
        /// </summary>
        /// <param name="signal">El objeto de datos de la señal a dibujar.</param>
        void DrawSignal(SignalData signal);
    }
}

