using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer.DataAbstraction;

namespace WhiteMagic.Tests
{
    public class InMemoryLanguageBranchRepository : ILanguageBranchRepository
    {
        public LanguageBranch Load(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public LanguageBranch Load(int id)
        {
            throw new NotImplementedException();
        }

        public IList<LanguageBranch> ListAll()
        {
            throw new NotImplementedException();
        }

        public IList<LanguageBranch> ListEnabled()
        {
            throw new NotImplementedException();
        }

        public LanguageBranch LoadFirstEnabledBranch()
        {
            throw new NotImplementedException();
        }

        public void Delete(int languageBranchId)
        {
            throw new NotImplementedException();
        }

        public void Save(LanguageBranch languageBranch)
        {
            throw new NotImplementedException();
        }
    }
}