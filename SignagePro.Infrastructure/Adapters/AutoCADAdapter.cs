using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using SignagePro.Core.Contracts;
using SignagePro.Core.Models;

// CORRECCIÓN: Se especifica el 'using' para resolver la ambigüedad de 'Application'
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace SignagePro.Infrastructure.Adapters
{
    public class AutoCADAdapter : IAutoCADAdapter
    {
        /// <summary>
        /// Implementación del contrato de la interfaz. Dibuja una señal en AutoCAD.
        /// </summary>
        /// <param name="signalData">El objeto con los datos de la señal a dibujar.</param>
        public void DrawSignal(SignalData signalData)
        {
            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var db = doc.Database;
            var ed = doc.Editor;

            // Iniciar una transacción
            using (var transaction = db.TransactionManager.StartTransaction())
            {
                // Abrir la tabla de bloques para escritura
                var blockTable = transaction.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (blockTable == null) return;

                var modelSpaceId = blockTable[BlockTableRecord.ModelSpace];
                var modelSpace = transaction.GetObject(modelSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                if (modelSpace == null) return;

                // Crear una polilínea para representar el rectángulo
                using (var polyline = new Polyline())
                {
                    polyline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                    polyline.AddVertexAt(1, new Point2d(signalData.Width, 0), 0, 0, 0);
                    polyline.AddVertexAt(2, new Point2d(signalData.Width, signalData.Height), 0, 0, 0);
                    polyline.AddVertexAt(3, new Point2d(0, signalData.Height), 0, 0, 0);
                    polyline.Closed = true;

                    // Establecer el color a rojo
                    polyline.ColorIndex = 1; // 1 es el índice para el color rojo en AutoCAD

                    // Añadir la polilínea al espacio del modelo
                    modelSpace.AppendEntity(polyline);
                    transaction.AddNewlyCreatedDBObject(polyline, true);
                }

                // Confirmar la transacción
                transaction.Commit();
            }

            // Opcional: Zoom a la entidad creada
            ed.Regen();
            ed.WriteMessage($"\n¡Señal '{signalData.Name}' creada exitosamente con una altura de {signalData.Height} unidades!");
        }
    }
}

