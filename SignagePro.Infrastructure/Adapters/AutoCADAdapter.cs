using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using SignagePro.Core.Contracts;
using SignagePro.Core.Models;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace SignagePro.Infrastructure.Adapters
{
    public class AutoCADAdapter : IAutoCADAdapter
    {
        /// <summary>
        /// Implementación del contrato. Dibuja una señal en AutoCAD basada en sus datos.
        /// </summary>
        /// <param name="signalData">El objeto con los datos de la señal a dibujar.</param>
        public void DrawSignal(SignalData signalData)
        {
            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var db = doc.Database;
            var ed = doc.Editor;

            // CORRECCIÓN CRÍTICA: Bloquear el documento antes de modificarlo desde una paleta.
            // Esto evita el error "Unhandled Exception" al interactuar con la base de datos
            // desde una ventana que no es modal.
            using (doc.LockDocument())
            {
                // Iniciar una transacción para modificar la base de datos de AutoCAD
                using (var transaction = db.TransactionManager.StartTransaction())
                {
                    var blockTable = transaction.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    if (blockTable == null) return;

                    var modelSpaceId = blockTable[BlockTableRecord.ModelSpace];
                    var modelSpace = transaction.GetObject(modelSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                    if (modelSpace == null) return;

                    // Crear una polilínea para representar el rectángulo de la señal
                    using (var polyline = new Polyline())
                    {
                        polyline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                        polyline.AddVertexAt(1, new Point2d(signalData.Width, 0), 0, 0, 0);
                        polyline.AddVertexAt(2, new Point2d(signalData.Width, signalData.Height), 0, 0, 0);
                        polyline.AddVertexAt(3, new Point2d(0, signalData.Height), 0, 0, 0);
                        polyline.Closed = true;

                        // Establecer el color a rojo (índice de color 1 en AutoCAD)
                        polyline.ColorIndex = 1;

                        // Añadir la polilínea al espacio del modelo y a la transacción
                        modelSpace.AppendEntity(polyline);
                        transaction.AddNewlyCreatedDBObject(polyline, true);
                    }

                    // Confirmar la transacción para guardar los cambios
                    transaction.Commit();
                }
            }

            ed.Regen();
            ed.WriteMessage($"\n¡Señal '{signalData.Name}' creada exitosamente!");
        }
    }
}

