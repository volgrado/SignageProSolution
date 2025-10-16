using Autodesk.AutoCAD.Runtime;
using SignagePro.Core;
using SignagePro.Infrastructure.Adapters;
using SignagePro.Infrastructure.Repositories;
using SignagePro.Main.UI;

[assembly: CommandClass(typeof(SignagePro.Main.Commands))]
[assembly: CommandClass(typeof(SignagePro.Main.SignageGenerator))]
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
                // 1. Creamos la implementación que sabe leer JSON.
                var signalRepository = new JsonSignalRepository();

                // 2. Se la pasamos al servicio del Core.
                var calculationService = new SignageCalculationService(signalRepository);

                var autocadAdapter = new AutoCADAdapter();

                // 3. Creamos el PaletteManager con el servicio ya configurado.
                _paletteManager = new PaletteManager(calculationService, autocadAdapter);
            }
            _paletteManager.Show();
        }
    }
}
