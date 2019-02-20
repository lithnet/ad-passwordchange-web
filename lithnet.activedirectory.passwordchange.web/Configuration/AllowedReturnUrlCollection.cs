using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class AllowedReturnUrlCollection : ConfigurationElementCollection
    {
        public AllowedReturnUrlElement this[int index]
        {
            get => (AllowedReturnUrlElement)this.BaseGet(index);
            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        public void Add(AllowedReturnUrlElement reader)
        {
            this.BaseAdd(reader);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AllowedReturnUrlElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AllowedReturnUrlElement)element).Value;
        }

        public void Remove(AllowedReturnUrlElement reader)
        {
            this.BaseRemove(reader.Value);
        }

        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            this.BaseRemove(name);
        }
    }
}