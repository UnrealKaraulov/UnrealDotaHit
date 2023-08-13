using System;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DotaHIT.DatabaseModel.Data
{
    public abstract class FieldAttribute : Attribute
    {
        public virtual string Process(string value)
        {
            return value;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SubstituteAttribute : FieldAttribute
    {
        string name;
        bool isPattern = false;
        public Regex pattern = null;
        public SubstituteAttribute(string name)
        {
            this.name = name;
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public bool IsPattern
        {
            get
            {
                return isPattern;
            }
            set
            {
                isPattern = value;
                if (isPattern)
                    pattern = new Regex(name, RegexOptions.IgnoreCase);
            }
        }
        public bool IsMatch(string value)
        {
            if (isPattern)
                return pattern.IsMatch(value);
            else
                return String.Compare(name, value, true) == 0;
        }
    }
    public class EnumerationAttribute : FieldAttribute
    {
        bool enable;
        public char enum_char = ',';
        public EnumerationAttribute(bool enable)
        {
            this.enable = enable;
        }
        public bool Enable
        {
            get
            {
                return enable;
            }
        }

        public override string Process(string value)
        {
            if (enable)
                return value;
            else
                if (!string.IsNullOrEmpty(value))
                    return value.Split(new char[] { enum_char }, StringSplitOptions.RemoveEmptyEntries)[0];
                else
                    return value;
        }
    }
    public class RemoveColorTagAttribute : FieldAttribute
    {
        public override string Process(string value)
        {
            value = value.Replace("|r", "");
            int index = value.IndexOf("|c"); // |cffffcc00

            if (index != -1)
                value = value.Remove(index, 10);

            return value;
        }
    }
    public class TrimAttribute : FieldAttribute
    {
        public char[] trimChars = new char[] { ' ' };
        public TrimAttribute(params char[] trimChars)
        {
            if (trimChars.Length != 0)
                this.trimChars = trimChars;
        }
        public override string Process(string value)
        {
            return value.Trim(trimChars);
        }
    }
    public class ArtToGifAttribute : FieldAttribute
    {
        public ArtToGifAttribute()
        {
        }
        public override string Process(string value)
        {
            return Path.GetFileNameWithoutExtension(value);
        }
    }

    public class FieldAttributeCollection : List<FieldAttribute>
    {
        public FieldAttributeCollection() { }
        public FieldAttributeCollection(object[] attributes)
        {
            this.AddRange(attributes);
        }
        public void AddRange(object[] attributes)
        {
            foreach (object attribute in attributes)
                if (attribute is FieldAttribute)
                    this.Add(attribute as FieldAttribute);
        }
        public bool Contains(Type attribute_type)
        {
            foreach (object attribute in this)
                if (attribute.GetType() == attribute_type)
                    return true;
            return false;
        }
        public object Get(Type attribute_type)
        {
            foreach (object attribute in this)
                if (attribute.GetType() == attribute_type)
                    return attribute;
            return null;
        }
        public string Process(string value)
        {
            foreach (FieldAttribute dbua in this)
                value = dbua.Process(value);

            return value;
        }
    }    
}


