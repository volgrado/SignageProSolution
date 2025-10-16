using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using SignagePro.Core;
using SignagePro.Core.Contracts;
using SignagePro.Main.ViewModels;
using System;

namespace SignagePro.Main.UI
{
    /// <summary>
    /// Gestiona la creación y visualización de la paleta principal de la aplicación.
    /// </summary>
    public class PaletteManager
    {
        private static PaletteSet? _paletteSet;
        private readonly SignageCalculationService _calculationService;
        private readonly IAutoCADAdapter _autocadAdapter;

        public PaletteManager(SignageCalculationService calculationService, IAutoCADAdapter autocadAdapter)
        {
            _calculationService = calculationService;
            _autocadAdapter = autocadAdapter;
        }

        public void Show()
        {
            if (_paletteSet == null)
            {
                // El GUID debe ser único para esta paleta para que AutoCAD recuerde su posición y estado.
                _paletteSet = new PaletteSet("Panel de Señalización Profesional", new Guid("F5AEC42A-2B5C-473A-9387-5A5E3E72E06A"));

                // Creamos la vista y su ViewModel
                var viewModel = new SignageViewModel(_calculationService, _autocadAdapter);
                var paletteView = new SignagePalette(viewModel);

                // CORRECCIÓN: Usamos AddVisual() para añadir un control WPF.
                _paletteSet.AddVisual("Señalización", paletteView);
            }

            // Hacemos visible la paleta.
            _paletteSet.Visible = true;
        }
    }
}

