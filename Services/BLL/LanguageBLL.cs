using Services.DAL.Factory;
using Services.DomainModel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BLL
{
    internal class LanguageBLL
    {
        ServiceFactory _serviceFactory;
        public LanguageBLL(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
        public string Translate(string word)
        {
            try
            {
                return _serviceFactory.languageRepository.Find(word);
            }
            catch (WordNotFoundException)
            {
                _serviceFactory.languageRepository.WriteNewWord(word, String.Empty);
                return word;
            }
        }
    }
}
