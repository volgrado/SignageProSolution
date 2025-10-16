using Autodesk.AutoCAD.Runtime;
using SignagePro.Core;
using SignagePro.Infrastructure.Adapters;
using SignagePro.Main.UI;

// Se añade un alias para resolver la ambigüedad con System.Windows.Forms.Application
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(SignagePro.Main.Commands))]

namespace SignagePro.Main
{
    public class Commands
    {
        private static PaletteManager? _paletteManager;

        [CommandMethod("ABRIR_PANEL_SIGNAGEPRO")]
        public void ShowSignageProPalette()
        {
            if (_paletteManager == null)
            {
                // CORRECCIÓN: Se instancian los servicios necesarios.
                var calculationService = new SignageCalculationService();
                var autocadAdapter = new AutoCADAdapter();

                // CORRECCIÓN: Se pasan los servicios al constructor del PaletteManager.
                _paletteManager = new PaletteManager(calculationService, autocadAdapter);
            }

            // CORRECCIÓN: El método para mostrar la paleta se llama 'Show'.
            _paletteManager.Show();
        }

        [CommandMethod("CREAR_SENAL_PRO_TEST")]
        public void CreateProfessionalSignalTest()
        {
            var doc = AcadApplication.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var editor = doc.Editor;

            editor.WriteMessage("\nIniciando la creación de señal profesional con la nueva arquitectura...");

            var calculationService = new SignageCalculationService();
            var autocadAdapter = new AutoCADAdapter();
            var signalData = calculationService.GetSignalData("S-01");

            if (signalData != null)
            {
                // CORRECCIÓN: Se utiliza el método 'DrawSignal' definido en la interfaz IAutoCADAdapter.
                autocadAdapter.DrawSignal(signalData);
                editor.WriteMessage($"\n¡Señal '{signalData.Name}' creada exitosamente con una altura de {signalData.Height} unidades!");
            }
            else
            {
                editor.WriteMessage("\nNo se encontraron datos para la señal S-01.");
            }
        }
    }
}
