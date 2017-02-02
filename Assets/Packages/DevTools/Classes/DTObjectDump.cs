// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Reflection;
using FluffyUnderware.DevTools.Extensions;

namespace FluffyUnderware.DevTools
{

    public class DTObjectDump
    {
        const int INDENTSPACES = 5;
        string mIndent;
        StringBuilder mSB;
        object mObject;

        public DTObjectDump(object o, int indent=0)
        {
            mIndent=new string(' ',indent);
            mSB=new StringBuilder();
            mObject=o;

            Type T = o.GetType();
            var fia = T.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (fia.Length>0)
                AppendHeader("Fields");
            foreach (FieldInfo fi in fia)
                AppendMember(fi);
            
            var pia = T.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
            if (pia.Length>0)
                AppendHeader("Properties");
            foreach (PropertyInfo pi in pia)
                AppendMember(pi);
        }

        public override string ToString()
        {
            return mSB.ToString();
        }

        void AppendHeader(string name)
        {
            mSB.AppendLine(mIndent+"<b>---===| "+name+" |===---</b>\n");
        }

        void AppendMember(MemberInfo info)
        {
            Type type;
            string typeName;
            object value;

            var fi=info as FieldInfo;
            if (fi != null)
            {
                type = fi.FieldType;
                typeName = type.Name;
                value = fi.GetValue(mObject);
            }
            else
            {
                var pi = info as PropertyInfo;
                type = pi.PropertyType;
                typeName = type.Name;
                value = pi.GetValue(mObject, null);
            }

            if (value != null)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    string tmp = mIndent;
                    int count = 0;
                    var ie = value as IEnumerable;
                    if (ie != null)
                    {
                        if (type.GetEnumerableType().BaseType == typeof(ValueType))
                        {
                            foreach (var e in ie)
                                tmp = string.Concat(tmp, string.Format("<b>{0}</b>: {1} ", (count++).ToString(), e.ToString()));
                        }
                        else
                        {
                            if (typeof(IList).IsAssignableFrom(type))
                                typeName = "IList<" + type.GetEnumerableType() + ">";
                            tmp += "\n";
                            foreach (var e in ie)
                                tmp = string.Concat(tmp, string.Format("<b>{0}</b>: {1} ", (count++).ToString(), new DTObjectDump(e, mIndent.Length + INDENTSPACES).ToString()));
                        }
                    }
                    mSB.Append(mIndent);
                    mSB.AppendFormat("(<i>{0}</i>) <b>{1}[{2}]</b> = ", typeName, info.Name, count);
                    mSB.AppendLine(tmp);
                }
                else
                {
                    mSB.Append(mIndent);
                    mSB.AppendFormat("(<i>{0}</i>) <b>{1}</b> = ", typeName, info.Name);
                    mSB.AppendLine(mIndent + value.ToString());
                }
            }
        }

    
       
        }

}
