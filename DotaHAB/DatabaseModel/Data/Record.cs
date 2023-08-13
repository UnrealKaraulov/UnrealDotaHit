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
using DotaHIT.Core;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Jass.Native.Types;

namespace DotaHIT.DatabaseModel.Data
{
    public interface IRecord
    {
        /// <summary>
        /// определяется какие поля были объявлены в структуре
        /// и заносит их в массив units, предварительно указав их имена и атрибуты (через конструктор DbUnit).
        /// предполагается что все объявленные поля (критерий поиска полей указан в коде) типа DbUnit
        /// </summary>            
        FieldCollection GetFieldSamples();

        FieldCollection Fields
        {
            get;
        }

        string Title
        {
            get;
        }

        IRecord Clone();

        /// <summary>
        /// копирует значения полей этой структуры в Record
        /// </summary>
        /// <param name="Record"></param>
        void CopyTo(IRecord Record);

        object this[IField field]
        {
            get;
            set;
        }

        void ApplyProps(HabProperties hps);
        void ApplyPropsBySubstitute(HabProperties hps);
        void ApplyPropsExBySubstitute(HabProperties hps);

        void refresh();

        string ToString(params string[] unitNames);
    }    

    /// <summary>
    /// коллекция структур
    /// </summary>
    public class RecordCollection : List<IRecord>
    {
        public void AddEx(IRecord record)
        {
            if (Contains(record) == false)
                base.Add(record);
        }
        public void AddType(IRecord record)
        {
            if (ContainsType(record.GetType()) == false)
                base.Add(record);
        }
        public bool ContainsUnit(string UnitName, object Value)
        {
            foreach (IRecord r in this)
            {
                IField sdu;
                if (r.Fields.TryGetByName(UnitName, out sdu) && sdu.ValueEquals(Value))
                    return true;
            }
            return false;
        }
        public bool ContainsType(Type t)
        {
            foreach (IRecord sdr in this)
                if (sdr.GetType().Equals(t))
                    return true;
            return false;
        }
        public int IndexOfType(Type t)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].GetType().Equals(t))
                    return i;
            return -1;
        }
        /// <summary>
        /// находит элемент коллекции по его типу и возвращает ссылку на найденный элемент, иначе null
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IRecord GetByType(Type t)
        {
            int index = IndexOfType(t);
            return (index == -1) ? null : this[index];
        }
        /// <summary>
        /// находит элемент коллекции по значению указанного поля и возвращает ссылку на найденный элемент, иначе null
        /// </summary>
        /// <param name="UnitName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public IRecord GetByUnit(string UnitName, object Value)
        {
            foreach (IRecord r in this)
            {
                IField sdu;
                if (r.Fields.TryGetByName(UnitName, out sdu) && sdu.ValueEquals(Value))
                    return r;
            }
            return null;
        }
        public RecordCollection GetRangeByUnit(string UnitName, object Value)
        {
            RecordCollection range = new RecordCollection();

            foreach (IRecord r in this)
            {
                IField sdu;
                if (r.Fields.TryGetByName(UnitName, out sdu) && sdu.ValueEquals(Value))
                    range.Add(r);
            }
            return range;
        }
        public RecordCollection GetRangeByUnitEx(string UnitName, object Value)
        {
            RecordCollection range = new RecordCollection();

            foreach (IRecord r in this)
            {
                IField sdu;
                if (r.Fields.TryGetByName(UnitName, out sdu) && sdu.ValueEquals(Value))
                    range.AddEx(r);
            }
            return range;
        }
        public RecordCollection GetRangeByMethod(string methodName, object desiredReturnValue, params object[] args)
        {
            RecordCollection range = new RecordCollection();

            foreach (IRecord r in this)
            {
                object result = null;
                try
                {
                    result = r.GetType().InvokeMember(methodName,
                                BindingFlags.Public | BindingFlags.Instance
                                | BindingFlags.InvokeMethod,
                                null, r, args);
                }
                catch { }

                if (desiredReturnValue == result)
                    range.Add(r);
            }
            return range;
        }

        /// <summary>
        /// находит элемент коллекции по значению указанного поля и возвращает ссылку на найденный элемент, иначе null
        /// </summary>
        /// <param name="UnitName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool RemoveByUnit(string UnitName, object Value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                IRecord r = this[i];
                IField sdu;
                if (r.Fields.TryGetByName(UnitName, out sdu) && sdu.ValueEquals(Value))
                {
                    this.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public IRecord ItemAt(int index)
        {
            if (index >= Count || index < 0) return null;
            return this[index];
        }

        public override string ToString()
        {
            string text = "";
            foreach (IRecord r in this)
                text += r.ToString();
            return text;
        }

        public string ToString(params string[] unitNames)
        {
            string str = "";

            foreach (IRecord hr in this)
                str += hr.ToString(unitNames) + "\n";

            return str;
        }

        /// <summary>
        /// создает полностью независимую копию этой коллекции (ссылки на одинаковые значения разные)
        /// </summary>
        /// <returns></returns>
        public RecordCollection Clone()
        {
            RecordCollection Collection = new RecordCollection();

            foreach (IRecord r in this)
                Collection.Add(r.Clone());

            return Collection;
        }

        public void SortByTaverns()
        {
            /*
            ////////////////////
            // get all taverns  
            ////////////////////

            ArrayList tavernList = new ArrayList();

            for (int i = 0; i < this.Count; i++)
            {
                foreach (string tavern in (this[i] as IRecord).tavernNames)
                    if (tavernList.Contains(tavern) == false)
                        tavernList.Add(tavern);                    
            }

            /////////////////////////////////
            // get records for each tavern  
            /////////////////////////////////

            RecordCollection[] recordsByTaverns = new RecordCollection[tavernList.Count];

            for (int i = 0; i < tavernList.Count; i++)                                    
                recordsByTaverns[i] = this.GetRangeByUnit("tavernNames", tavernList[i]);
               

            this.Clear(); // clear it, to refill with each tavern's sorted records in the cycle below

            ////////////////////////////////
            // sort records in each tavern 
            ////////////////////////////////

            foreach (RecordCollection hrc in recordsByTaverns)
            {
                //////////////////////////////////////////////
                // get max slot for current tavern's records
                //////////////////////////////////////////////

                int maxSlot = 0;

                foreach (HabRecord record in hrc)
                    if (record.tavernSlotX.IsNull == false && record.tavernSlotY.IsNull == false)
                        maxSlot = Math.Max(maxSlot,
                                            HabRecordSlotComparer.get_slot(record.tavernSlotX, record.tavernSlotY));

                ///////////////////////////////////
                // sort records in current tavern
                ///////////////////////////////////

                ArrayList al = new ArrayList();

                for (int currentSlot = 0; currentSlot < maxSlot; currentSlot++)
                {
                    bool usedEmpty = false;

                    for (int i = 0; i < hrc.Count; i++)
                    {
                        HabRecord record = hrc[i] as HabRecord;

                        if (record.tavernSlotX.IsNull == false && record.tavernSlotY.IsNull == false)
                        {
                            int slot = HabRecordSlotComparer.get_slot(record.tavernSlotX, record.tavernSlotY);

                            if (currentSlot == slot)
                            {
                                al.Add(record);
                                hrc.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                        else
                            if (usedEmpty == false)
                            {
                                al.Add(record);
                                hrc.RemoveAt(i);
                                i--;

                                usedEmpty = true;
                            }
                    }
                }

                if (hrc.Count != 0)
                    al.AddRange(hrc);

                hrc.Clear();
                hrc.AddRange(al);

                this.AddRange(hrc);
            }  */
        }

    }
    /// <summary>
    /// используется для сравнения структур
    /// указываеются поля которые будут критерием сравнения
    /// если указано несколько полей то первое в списке поле имеет высший приоритет и т.д. по убыванию
    /// </summary>
    public class RecordComparer : IComparer<IRecord>
    {
        private string[] UnitNames = { };
        private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

        public RecordComparer(params string[] UnitNames)
        {
            this.UnitNames = UnitNames;
        }
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer<IRecord>.Compare(IRecord x, IRecord y)
        {
            return Compare(x, y, UnitNames, 0);
        }
        public int Compare(IRecord r1, IRecord r2)
        {
            return Compare(r1, r2, UnitNames, 0);
        }
        private int Compare(IRecord r1, IRecord r2, string[] UnitNames, int index)
        {
            if (index >= UnitNames.Length) return 0;
            else
            {
                string unitName = UnitNames[index];

                IField sdu1; r1.Fields.TryGetByName(unitName, out sdu1);
                IField sdu2; r2.Fields.TryGetByName(unitName, out sdu2);

                int result = cic.Compare(sdu1.Value, sdu2.Value);

                if (result == 0)
                    return Compare(r1, r2, UnitNames, index + 1);
                else
                    return result;
            }
        }
    }

    /// <summary>
    /// используется для сравнения структур
    /// указываеются поля которые будут критерием сравнения
    /// если указано несколько полей то первое в списке поле имеет высший приоритет и т.д. по убыванию
    /// </summary>
    public class RecordSlotComparer : IComparer<IRecord>
    {
        private string[] UnitNames = { };
        private int slotX;
        private int slotY;
        private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

        public RecordSlotComparer()
        {
            slotX = 0;
            slotY = 0;
        }

        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer<IRecord>.Compare(IRecord x, IRecord y)
        {
            widget r1 = x as widget;
            widget r2 = y as widget;

            // record 1 pos

            int r1_x;
            int r1_y;

            if (r1.buttonPos.Size == 0)
                r1_x = slotX;
            else
                r1_x = r1.buttonPos[0];

            if (r1.buttonPos.Size < 2)
                r1_y = slotY;
            else
                r1_y = r1.buttonPos[1];

            // record 2 pos

            int r2_x;
            int r2_y;

            if (r2.buttonPos.Size == 0)
                r2_x = slotX;
            else
                r2_x = r2.buttonPos[0];

            if (r2.buttonPos.Size < 2)
                r2_y = slotY;
            else
                r2_y = r2.buttonPos[1];

            return cic.Compare(get_slot(r1_x, r1_y),
                           get_slot(r1_x, r1_y));
        }

        public static int get_slot(int x, int y)
        {
            return (y * 4 + x);
        }
        public static int get_slot(string buttonpos)
        {
            if (string.IsNullOrEmpty(buttonpos)) return -1;

            string[] pos = buttonpos.Split(DBSTRINGCOLLECTION.comma_separator);
            int x;
            int y;
            int.TryParse(pos[0], out x);
            if (pos.Length > 1) int.TryParse(pos[1], out y);
            else y = 0;
            return get_slot(x, y);
        }
        public static void set_slot(int slot, out int x, out int y)
        {
            x = slot % 4;
            y = slot / 4;
        }

        protected void next_slot()
        {
            slotX += 1;
            if (slotX >= 4)
            {
                slotY += 1;
                slotX = slotX % 4;
            }
        }
    }    
}


