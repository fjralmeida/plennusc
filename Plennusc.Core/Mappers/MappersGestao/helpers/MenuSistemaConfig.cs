using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Plennusc.Core.Mappers.MappersGestao.helpers
{
    public class MenuSistemaConfig
    {
        private static string _configFilePath;
        private Dictionary<int, int> _config; // CodMenu -> CodSistema

        public MenuSistemaConfig()
        {
            // Usa o HttpContext.Current para obter o caminho físico
            if (HttpContext.Current != null)
            {
                _configFilePath = HttpContext.Current.Server.MapPath("~/App_Data/menu_sistema_config.json");
            }
            else
            {
                // Fallback para quando não está em contexto web (testes, etc)
                _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "menu_sistema_config.json");
            }

            // Garante que a pasta App_Data existe
            var directory = Path.GetDirectoryName(_configFilePath);
            if (!Directory.Exists(directory) && directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    string json = File.ReadAllText(_configFilePath);
                    _config = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
                }
                else
                {
                    _config = new Dictionary<int, int>();
                    // Salva um arquivo vazio
                    SaveConfig();
                }
            }
            catch
            {
                _config = new Dictionary<int, int>();
            }
        }

        public int? GetSistemaDoMenu(int codMenu)
        {
            if (_config.ContainsKey(codMenu))
                return _config[codMenu];
            return null;
        }

        public void SetSistemaDoMenu(int codMenu, int codSistema)
        {
            _config[codMenu] = codSistema;
            SaveConfig();
        }

        public void RemoveConfig(int codMenu)
        {
            if (_config.ContainsKey(codMenu))
            {
                _config.Remove(codMenu);
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar configurações: {ex.Message}", ex);
            }
        }

        public Dictionary<int, int> GetAll()
        {
            return _config;
        }

        public List<int> GetMenusPorSistema(int codSistema)
        {
            var menus = new List<int>();
            foreach (var kvp in _config)
            {
                if (kvp.Value == codSistema)
                {
                    menus.Add(kvp.Key);
                }
            }
            return menus;
        }
    }
}