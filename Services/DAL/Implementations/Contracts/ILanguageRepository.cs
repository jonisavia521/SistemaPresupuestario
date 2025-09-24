using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Implementations.Contracts
{
    internal interface ILanguageRepository
    {
        string Find(string word);
        void WriteNewWord(string word, string value);
        Dictionary<string, string> FindAll();
        List<string> GetCurrentCultures();
    }
}
