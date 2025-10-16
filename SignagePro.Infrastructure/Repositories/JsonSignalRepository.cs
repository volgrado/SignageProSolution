using Newtonsoft.Json;
using SignagePro.Core.Contracts;
using SignagePro.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SignagePro.Infrastructure.Repositories
{
    public class JsonSignalRepository : ISignalRepository
    {
        private readonly string _filePath;

        public JsonSignalRepository()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            _filePath = Path.Combine(assemblyDirectory, "signals.json");
        }

        public List<SignalData> GetAllSignals()
        {
            if (!File.Exists(_filePath))
            {
                // Aquí podrías loguear un error o notificar al usuario en AutoCAD.
                return new List<SignalData>();
            }

            string jsonContent = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<SignalData>>(jsonContent) ?? new List<SignalData>();
        }
    }
}
