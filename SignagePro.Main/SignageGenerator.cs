using Autodesk.AutoCAD.ApplicationServices; // AÑADIR: Necesario para Document y DocumentLock
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: CommandClass(typeof(SignagePro.Main.SignageGenerator))]
namespace SignagePro.Main
{
    public class SignageGenerator
    {
        #region Data Tables (Sin cambios)
        private static readonly Dictionary<int, Dictionary<int, double>> SpaceTable = new Dictionary<int, Dictionary<int, double>>
        {
            { 1, new Dictionary<int, double> { { 200, 48 }, { 300, 71 }, { 400, 96 } } },
            { 2, new Dictionary<int, double> { { 200, 38 }, { 300, 57 }, { 400, 76 } } },
            { 3, new Dictionary<int, double> { { 200, 25 }, { 300, 38 }, { 400, 50 } } },
            { 4, new Dictionary<int, double> { { 200, 13 }, { 300, 19 }, { 400, 26 } } }
        };

        private static readonly Dictionary<char, Dictionary<int, double>> LetterWidthTable = new Dictionary<char, Dictionary<int, double>>
        {
            {'A', new Dictionary<int, double> { { 200, 170 }, { 300, 255 }, { 400, 340 } } },
            {'B', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'C', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'D', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'E', new Dictionary<int, double> { { 200, 124 }, { 300, 186 }, { 400, 248 } } },
            {'F', new Dictionary<int, double> { { 200, 124 }, { 300, 186 }, { 400, 248 } } },
            {'G', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'H', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'I', new Dictionary<int, double> { { 200, 32 }, { 300, 48 }, { 400, 64 } } },
            {'J', new Dictionary<int, double> { { 200, 127 }, { 300, 190 }, { 400, 254 } } },
            {'K', new Dictionary<int, double> { { 200, 140 }, { 300, 210 }, { 400, 280 } } },
            {'L', new Dictionary<int, double> { { 200, 124 }, { 300, 186 }, { 400, 248 } } },
            {'M', new Dictionary<int, double> { { 200, 157 }, { 300, 236 }, { 400, 314 } } },
            {'N', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'O', new Dictionary<int, double> { { 200, 143 }, { 300, 214 }, { 400, 286 } } },
            {'P', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'Q', new Dictionary<int, double> { { 200, 143 }, { 300, 214 }, { 400, 286 } } },
            {'R', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'S', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'T', new Dictionary<int, double> { { 200, 124 }, { 300, 186 }, { 400, 248 } } },
            {'U', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'V', new Dictionary<int, double> { { 200, 152 }, { 300, 229 }, { 400, 304 } } },
            {'W', new Dictionary<int, double> { { 200, 178 }, { 300, 267 }, { 400, 356 } } },
            {'X', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'Y', new Dictionary<int, double> { { 200, 171 }, { 300, 257 }, { 400, 342 } } },
            {'Z', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } }
        };

        private static readonly Dictionary<char, Dictionary<int, double>> NumeralWidthTable = new Dictionary<char, Dictionary<int, double>>
        {
            {'1', new Dictionary<int, double> { { 200, 50 }, { 300, 74 }, { 400, 98 } } },
            {'2', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'3', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'4', new Dictionary<int, double> { { 200, 149 }, { 300, 224 }, { 400, 298 } } },
            {'5', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'6', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'7', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'8', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'9', new Dictionary<int, double> { { 200, 137 }, { 300, 205 }, { 400, 274 } } },
            {'0', new Dictionary<int, double> { { 200, 143 }, { 300, 214 }, { 400, 286 } } }
        };
        #endregion

        private enum SignageType
        {
            Obligatorio,
            Direccion,
            Emplazamiento,
            Distancia
        }

        #region Helper Methods (sin cambios, renombrados a static)
        private static int GetCodeNumber(char preceding, char following)
        {
            if (char.IsLetter(preceding) != char.IsLetter(following) || char.IsWhiteSpace(preceding) || char.IsWhiteSpace(following)) return 1;
            if (char.IsLetter(preceding) && char.IsLetter(following))
            {
                string followingGroup1 = "BDEFHIKLMNPRU", followingGroup2 = "CGOSXZ", followingGroup3 = "AJTVWY";
                switch (preceding)
                {
                    case 'A': if (followingGroup3.Contains(following)) return 4; return 2;
                    case 'B': if (followingGroup1.Contains(following)) return 1; return 2;
                    case 'C': if (followingGroup3.Contains(following)) return 3; return 2;
                    case 'D': if (followingGroup1.Contains(following)) return 1; return 2;
                    case 'E': if (followingGroup3.Contains(following)) return 3; return 2;
                    case 'F': if (followingGroup3.Contains(following)) return 3; return 2;
                    case 'G': if (followingGroup1.Contains(following)) return 1; return 2;
                    case 'H': if (followingGroup1.Contains(following)) return 1; if (followingGroup2.Contains(following)) return 1; return 2;
                    case 'I': if (followingGroup1.Contains(following)) return 1; if (followingGroup2.Contains(following)) return 1; return 2;
                    case 'J': if (followingGroup1.Contains(following)) return 1; if (followingGroup2.Contains(following)) return 1; return 2;
                    case 'K': if (followingGroup3.Contains(following)) return 3; return 2;
                    case 'L': if (followingGroup3.Contains(following)) return 4; if (followingGroup2.Contains(following)) return 1; return 2;
                    case 'M': if (followingGroup1.Contains(following)) return 1; if (followingGroup2.Contains(following)) return 1; return 2;
                    case 'N': if (followingGroup1.Contains(following)) return 1; if (followingGroup2.Contains(following)) return 1; return 2;
                    case 'O': if (followingGroup1.Contains(following)) return 1; return 2;
                    case 'P': if (followingGroup1.Contains(following)) return 1; return 2;
                    case 'Q': if (followingGroup1.Contains(following)) return 1; return 2;
                    case 'R': if (followingGroup1.Contains(following)) return 1; return 2;
                    case 'S': if (followingGroup1.Contains(following)) return 1; return 2;
                    case 'T': if (followingGroup3.Contains(following)) return 4; return 2;
                    case 'U': if (followingGroup1.Contains(following)) return 1; if (followingGroup2.Contains(following)) return 1; return 2;
                    case 'V': if (followingGroup3.Contains(following)) return 4; return 2;
                    case 'W': if (followingGroup3.Contains(following)) return 4; return 2;
                    case 'X': if (followingGroup3.Contains(following)) return 3; return 2;
                    case 'Y': if (followingGroup3.Contains(following)) return 4; return 2;
                    case 'Z': if (followingGroup3.Contains(following)) return 3; return 2;
                }
            }
            if (char.IsDigit(preceding) && char.IsDigit(following))
            {
                string followingGroup1 = "15", followingGroup3 = "47";
                if (preceding == '1') { if (followingGroup3.Contains(following)) return 2; return 1; }
                if ("236890".Contains(preceding)) { if (followingGroup1.Contains(following)) return 1; return 2; }
                if ("47".Contains(preceding)) { if (followingGroup1.Contains(following)) return 2; return 4; }
                if (preceding == '5') { if (followingGroup1.Contains(following)) return 1; return 2; }
            }
            return 1;
        }

        private static bool TryGetCharacterWidth(char character, int heightKey, out double width)
        {
            character = char.ToUpper(character); Dictionary<int, double> widthDict = null;
            if (char.IsLetter(character) && LetterWidthTable.ContainsKey(character)) widthDict = LetterWidthTable[character];
            else if (char.IsDigit(character) && NumeralWidthTable.ContainsKey(character)) widthDict = NumeralWidthTable[character];
            if (widthDict != null && widthDict.ContainsKey(heightKey)) { width = widthDict[heightKey]; return true; }
            width = 0; return false;
        }

        private static double GetSpace(int codeNumber, int heightKey)
        {
            if (SpaceTable.ContainsKey(codeNumber) && SpaceTable[codeNumber].ContainsKey(heightKey)) return SpaceTable[codeNumber][heightKey];
            return 0;
        }
        #endregion

        [CommandMethod("CREAR_SENAL")]
        public void CreateSignage()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptStringOptions pso = new PromptStringOptions("\nIntroduzca el texto a generar: ");
            pso.AllowSpaces = true;
            PromptResult pr = ed.GetString(pso);
            if (pr.Status != PromptStatus.OK) return;
            string inputText = pr.StringResult.ToUpper().Trim();

            if (string.IsNullOrEmpty(inputText)) return;

            PromptKeywordOptions pkoType = new PromptKeywordOptions("\nSeleccione el tipo de letrero: [Obligatorio/Direccion/Emplazamiento/Distancia]: ");
            pkoType.Keywords.Add("Obligatorio"); pkoType.Keywords.Add("Direccion"); pkoType.Keywords.Add("Emplazamiento"); pkoType.Keywords.Add("Distancia");
            pkoType.Keywords.Default = "Direccion";
            PromptResult pkrType = ed.GetKeywords(pkoType);
            if (pkrType.Status != PromptStatus.OK) return;
            SignageType signageType = (SignageType)Enum.Parse(typeof(SignageType), pkrType.StringResult);

            short textColor = 256; short backgroundColor = 256;
            // Valores de offset automáticos: 
            double border = 0.15; // 15 cm de borde de contraste, según manual.

            switch (signageType)
            {
                case SignageType.Obligatorio:
                    textColor = 7;      // Blanco
                    backgroundColor = 1;  // Rojo
                    break;
                case SignageType.Emplazamiento:
                    textColor = 2;      // Amarillo
                    backgroundColor = 7;  // Negro
                    break;
                case SignageType.Direccion:
                    textColor = 7;      // Negro
                    backgroundColor = 2;  // Amarillo
                    break;
                case SignageType.Distancia:
                    textColor = 7;      // Blanco
                    backgroundColor = 7;  // Negro
                    break;
            }

            PromptKeywordOptions pkoHeight = new PromptKeywordOptions("\nSeleccione la altura de los caracteres [100mm/200mm/400mm/1m/2m/4m]: ");
            pkoHeight.Keywords.Add("100mm"); pkoHeight.Keywords.Add("200mm"); pkoHeight.Keywords.Add("400mm");
            pkoHeight.Keywords.Add("1m"); pkoHeight.Keywords.Add("2m"); pkoHeight.Keywords.Add("4m");
            pkoHeight.Keywords.Default = "4m";
            PromptResult pkrHeight = ed.GetKeywords(pkoHeight);
            if (pkrHeight.Status != PromptStatus.OK) return;
            string selectedHeight = pkrHeight.StringResult;

            double Hps = 0;
            const int baseHeightForTables = 400;
            string heightSuffix = "";

            switch (selectedHeight)
            {
                case "100mm": Hps = 100; heightSuffix = "(100 mm)"; break;
                case "200mm": Hps = 200; heightSuffix = "(200 mm)"; break;
                case "400mm": Hps = 400; heightSuffix = "(400 mm)"; break;
                case "1m": Hps = 1000; heightSuffix = "(1 m)"; break;
                case "2m": Hps = 2000; heightSuffix = "(2 m)"; break;
                case "4m": Hps = 4000; heightSuffix = "(4 m)"; break;
            }

            double Hes = Hps;
            if (Hps > 400)
            {
                Hes = Hps / 2.5;
            }

            // Calcular el "padding" (espacio entre el texto y el borde interior del fondo)
            // Se utiliza la proporción del alto del texto (Hps) para mantener el factor de proporcionalidad del diseño.
            double padding = Hps / 1000.0 * 0.5; // Aproximadamente 0.5m de padding por metro de altura de texto si lo escaláramos de 1m a 4m.

            PromptPointOptions ppo = new PromptPointOptions("\nSeleccione el punto de inserción inicial: ");
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status != PromptStatus.OK) return;

            Point3d initialPosition = ppr.Value;
            Point3d currentPosition = initialPosition;
            List<ObjectId> createdEntities = new List<ObjectId>();

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ms = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                double currentOffset = 0;
                for (int i = 0; i < inputText.Length; i++)
                {
                    char currentChar = inputText[i];

                    if (char.IsWhiteSpace(currentChar))
                    {
                        double spaceAdvanceInMillimeters = (Hps / 2.0);
                        currentPosition = new Point3d(currentPosition.X + spaceAdvanceInMillimeters / 1000.0, currentPosition.Y, currentPosition.Z);
                        continue;
                    }

                    string blockName = $"{currentChar} {heightSuffix}";

                    if (bt.Has(blockName))
                    {
                        BlockReference br = new BlockReference(currentPosition, bt[blockName]);
                        br.ColorIndex = textColor;

                        double scaleX = 1.0, scaleY = 1.0;
                        switch (selectedHeight)
                        {
                            case "1m":
                            case "2m":
                            case "4m":
                                scaleX = 1.0;
                                scaleY = 1.0;
                                break;
                            case "100mm":
                                scaleX = 0.25;
                                scaleY = 0.1;
                                break;
                            case "200mm":
                                scaleX = 0.5;
                                scaleY = 0.2;
                                break;
                            case "400mm":
                                scaleX = 1.0;
                                scaleY = 0.4;
                                break;
                        }

                        br.ScaleFactors = new Scale3d(scaleX, scaleY, 1.0);
                        ms.AppendEntity(br);
                        trans.AddNewlyCreatedDBObject(br, true);
                        createdEntities.Add(br.ObjectId);

                        TryGetCharacterWidth(currentChar, baseHeightForTables, out double charWidthBase);
                        double charWidthScaled = (Hes / baseHeightForTables) * charWidthBase;
                        currentOffset = charWidthScaled;
                    }
                    else
                    {
                        ed.WriteMessage($"\nAdvertencia: Bloque '{blockName}' no encontrado. Se usará texto simple como sustituto.");
                        DBText textFallback = new DBText
                        {
                            Position = currentPosition,
                            TextString = currentChar.ToString(),
                            Height = Hps / 1000.0,
                            HorizontalMode = TextHorizontalMode.TextLeft,
                            VerticalMode = TextVerticalMode.TextBase,
                            ColorIndex = textColor
                        };
                        ms.AppendEntity(textFallback);
                        trans.AddNewlyCreatedDBObject(textFallback, true);
                        createdEntities.Add(textFallback.ObjectId);

                        TryGetCharacterWidth(currentChar, baseHeightForTables, out double charWidthBase);
                        double charWidthScaled = (Hes / baseHeightForTables) * charWidthBase;
                        currentOffset = charWidthScaled;
                    }

                    if (i < inputText.Length - 1)
                    {
                        double spaceValueInMillimeters = 0;
                        char nextChar = inputText[i + 1];

                        if (!char.IsWhiteSpace(nextChar))
                        {
                            int codeNumber = GetCodeNumber(currentChar, nextChar);
                            double spaceBase = GetSpace(codeNumber, baseHeightForTables);
                            spaceValueInMillimeters = (Hes / baseHeightForTables) * spaceBase;
                        }

                        double advanceInMeters = (currentOffset + spaceValueInMillimeters) / 1000.0;
                        currentPosition = new Point3d(currentPosition.X + advanceInMeters, currentPosition.Y, currentPosition.Z);
                    }
                }

                double totalWidthInMeters = (currentPosition.X - initialPosition.X) + currentOffset / 1000.0;
                double totalHeightInMeters = Hps / 1000.0;

                Point3d p1 = initialPosition;
                Point3d p3 = new Point3d(initialPosition.X + totalWidthInMeters, initialPosition.Y + totalHeightInMeters, initialPosition.Z);

                // Caja de fondo (el color depende del tipo de señal: Rojo, Amarillo o Negro)
                Point3d p1_padded = new Point3d(p1.X - padding, p1.Y - padding, p1.Z);
                Point3d p3_padded = new Point3d(p3.X + padding, p3.Y + padding, p3.Z);

                Polyline backgroundBox = new Polyline(4);
                backgroundBox.AddVertexAt(0, new Point2d(p1_padded.X, p1_padded.Y), 0, 0, 0);
                backgroundBox.AddVertexAt(1, new Point2d(p3_padded.X, p1_padded.Y), 0, 0, 0);
                backgroundBox.AddVertexAt(2, new Point2d(p3_padded.X, p3_padded.Y), 0, 0, 0);
                backgroundBox.AddVertexAt(3, new Point2d(p1_padded.X, p3_padded.Y), 0, 0, 0);
                backgroundBox.Closed = true;
                ms.AppendEntity(backgroundBox);
                trans.AddNewlyCreatedDBObject(backgroundBox, true);

                Hatch backgroundHatch = new Hatch();
                backgroundHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                backgroundHatch.ColorIndex = backgroundColor;
                ms.AppendEntity(backgroundHatch);
                trans.AddNewlyCreatedDBObject(backgroundHatch, true);
                backgroundHatch.Associative = true;
                backgroundHatch.AppendLoop(HatchLoopTypes.Outermost, new ObjectIdCollection { backgroundBox.ObjectId });
                backgroundHatch.EvaluateHatch(true);

                // Lógica para el Marco de Contraste Negro (15 cm)
                if (signageType != SignageType.Obligatorio)
                {
                    Point3d p1_bordered = new Point3d(p1_padded.X - border, p1_padded.Y - border, p1_padded.Z);
                    Point3d p3_bordered = new Point3d(p3_padded.X + border, p3_padded.Y + border, p3_padded.Z);

                    Polyline borderBox = new Polyline(4);
                    borderBox.AddVertexAt(0, new Point2d(p1_bordered.X, p1_bordered.Y), 0, 0, 0);
                    borderBox.AddVertexAt(1, new Point2d(p3_bordered.X, p1_bordered.Y), 0, 0, 0);
                    borderBox.AddVertexAt(2, new Point2d(p3_bordered.X, p3_bordered.Y), 0, 0, 0);
                    borderBox.AddVertexAt(3, new Point2d(p1_bordered.X, p3_bordered.Y), 0, 0, 0);
                    borderBox.Closed = true;
                    ms.AppendEntity(borderBox);
                    trans.AddNewlyCreatedDBObject(borderBox, true);

                    Hatch borderHatch = new Hatch();
                    borderHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                    borderHatch.ColorIndex = 7; // Negro
                    ms.AppendEntity(borderHatch);
                    trans.AddNewlyCreatedDBObject(borderHatch, true);
                    borderHatch.Associative = true;
                    borderHatch.AppendLoop(HatchLoopTypes.Outermost, new ObjectIdCollection { borderBox.ObjectId });
                    borderHatch.AppendLoop(HatchLoopTypes.Default, new ObjectIdCollection { backgroundBox.ObjectId });
                    borderHatch.EvaluateHatch(true);

                    DrawOrderTable dot = trans.GetObject(ms.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;
                    dot.MoveToBottom(new ObjectIdCollection { backgroundHatch.ObjectId, borderBox.ObjectId });
                    dot.MoveToTop(new ObjectIdCollection(createdEntities.ToArray()));
                }
                else
                {
                    // Si es Obligatorio (Fondo Rojo), solo tiene el fondo (sin marco de contraste)
                    DrawOrderTable dot = trans.GetObject(ms.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;
                    dot.MoveToBottom(new ObjectIdCollection { backgroundHatch.ObjectId });
                    dot.MoveToTop(new ObjectIdCollection(createdEntities.ToArray()));
                }

                trans.Commit();
                ed.WriteMessage("\nGeneración de señal y marco finalizada.");
            }
        }
    }
}