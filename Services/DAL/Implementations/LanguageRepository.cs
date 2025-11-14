using Services.DAL.Implementations.Contracts;
using Services.DomainModel.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.DAL.Implementations
{
    public sealed class LanguageRepository: ILanguageRepository
    {
        private string basePath;
        
        public LanguageRepository(NameValueCollection app)
        {
            basePath = app["LanguagePath"];
        }

        public string Find(string word)
        {
            using (var sr = new StreamReader(basePath + "." + Thread.CurrentThread.CurrentCulture.Name))
            {
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split('=');

                    if (line[0] == word)
                    {
                        if (String.IsNullOrEmpty(line[1]))
                            return line[0];

                        return line[1];
                    }
                }
            }

            throw new WordNotFoundException();
        }

        public void WriteNewWord(string word, string value)
        {
        }

        public Dictionary<string, string> FindAll()
        {
            return null;
        }

        /// <summary>
        /// Obtiene las culturas soportadas por la aplicación desde los archivos de traducción
        /// </summary>
        public List<string> GetCurrentCultures()
        {
            return new List<string>();
        }
    }
}
