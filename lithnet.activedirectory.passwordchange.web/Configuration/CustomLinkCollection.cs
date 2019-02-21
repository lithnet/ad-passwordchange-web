using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class CustomLinkCollection : ConfigurationElementCollection
    {
        public CustomLinkElement this[int index]
        {
            get => (CustomLinkElement)this.BaseGet(index);
            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        public new CustomLinkElement this[string id] => (CustomLinkElement)this.BaseGet(id);

        public CustomLinkElement GetHelpLink => (CustomLinkElement) this.BaseGet("get-help-button");

        public CustomLinkElement SuccessButton => (CustomLinkElement)this.BaseGet("success-button");

        public void Add(CustomLinkElement reader)
        {
            this.BaseAdd(reader);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomLinkElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomLinkElement)element).ID;
        }

        public void Remove(CustomLinkElement reader)
        {
            this.BaseRemove(reader.ID);
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