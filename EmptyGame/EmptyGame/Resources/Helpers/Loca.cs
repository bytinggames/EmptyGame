using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    public class LocaF : Loca
    {
        protected Func<object>[] parameters;

        public LocaF(string _en = "")
            : base(_en)
        {
        }
        public LocaF(string _en, params Func<object>[] _parameters)
            : base(_en)
        {
            this.parameters = _parameters;
        }
        public LocaF(string _en, params object[] _parameters)
            : base()
        {
            object[] par = new object[_parameters.Length];
            List<Func<object>> parameterList = new List<Func<object>>();
            int j = 0;

            for (int i = 0; i < par.Length; i++)
            {
                if (_parameters[i] is Func<object> h)
                {
                    par[i] = "{" + j++ + "}";
                    parameterList.Add(h);
                }
                else
                    par[i] = _parameters[i];
            }

            parameters = parameterList.ToArray();

            if (L.language == 0)
                Initialize(string.Format(_en, par));
        }


        public override string GetText(params object[] customParams)
        {
            if (parameters != null)
                return string.Format(text, parameters.Select(f => f()).Concat(customParams).ToArray());
            else if (customParams.Length > 0)
                return string.Format(text, customParams);
            else
                return text;
        }
    }


    public class Loca
    {
        protected string text;

        public Loca()
        {

        }

        public Loca(string _en)
        {
            if (L.language == 0)
                Initialize(_en);
        }

        public Loca(string _en, params object[] _parameters)
        {
            if (L.language == 0)
                Initialize(string.Format(_en, _parameters));
        }

        protected static void FormatCodes(string _text)
        {

        }

        protected virtual void Initialize(string _text)
        {
            text = _text;

            //if (text.Length > 0 && !text.EndsWith(".") && !text.EndsWith("!"))
            //    Console.WriteLine("warning: . is mising in loca: " + text);

            int bracketArgumentsInTextCount = text.Count(f => f == '{');

            int count = bracketArgumentsInTextCount;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '[')
                {
                    int iStart = i;
                    do
                    {
                        i++;
                    }
                    while (text[i] != ']');

                    if (iStart + 1 < i) // if theres actually anything between the []
                    {
                        text = text.Remove(iStart) + "{" + count++ + "}" + text.Substring(i + 1);
                    }
                }
            }
        }
        public override string ToString() => GetText();
        public virtual string GetText()
        {
            return text;
        }
        public virtual string GetText(params object[] _customParams)
        {
            return string.Format(text, _customParams);
        }

        public static implicit operator Loca(string _text)
        {
            return new Loca(_text);
        }

        public static implicit operator string(Loca _loca)
        {
            return _loca.GetText();
        }

        public string this[params object[] _customParams]
        {
            get => GetText(_customParams);
        }
        public string U
        {
            get
            {
                string _text = GetText();
                if (_text.First() != JuliHelper.Drawer.openCode)
                    return UppercaseFirst(GetText());
                else
                {
                    for (int i = 1; i < _text.Length; i++)
                    {
                        if (_text[i - 1] == JuliHelper.Drawer.closeCode && _text[i] != JuliHelper.Drawer.openCode)
                        {
                            return _text.Substring(0, i) + _text[i].ToString().ToUpper() + _text.Substring(i + 1, _text.Length - i - 1);
                        }
                    }
                    return _text;
                }
            }
        }
        static string UppercaseFirst(string _s)
        {
            char[] a = _s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }

    public class LocaF<T> : LocaF
    {
        bool multiple;

        Func<T, object>[] funcs;
        public LocaF(string _en, params Func<T, object>[] _funcs/*{0}*/)
        {
            funcs = _funcs;

            if (L.language == 0)
            {
                //object[] par = new object[_parameters.Length];
                //List<Func<object>> parameterList = new List<Func<object>>();
                //int j = 0;

                //for (int i = 0; i < par.Length; i++)
                //{
                //    if (_parameters[i] is Func<object> h)
                //    {
                //        par[i] = "{" + j++ + "}";
                //        parameterList.Add(h);
                //    }
                //    else
                //        par[i] = _parameters[i];
                //}

                //parameters = parameterList.ToArray();

                if (L.language == 0)
                    Initialize(_en);
                
            }
        }

        public LocaF(bool _multiple, string _en, params Func<T, object>[] _funcs/*{0}*/)
        {
            multiple = _multiple;
            funcs = _funcs;

            if (L.language == 0)
            {
                //object[] par = new object[_parameters.Length];
                //List<Func<object>> parameterList = new List<Func<object>>();
                //int j = 0;

                //for (int i = 0; i < par.Length; i++)
                //{
                //    if (_parameters[i] is Func<object> h)
                //    {
                //        par[i] = "{" + j++ + "}";
                //        parameterList.Add(h);
                //    }
                //    else
                //        par[i] = _parameters[i];
                //}

                //parameters = parameterList.ToArray();

                if (L.language == 0)
                    Initialize(_en);

            }
        }

        public override string GetText(params object[] _customParams)
        {
            List<object> customFuncParams = new List<object>();
            if (_customParams.Length > 0)
            {
                for (int i = 0; i < funcs.Length; i++)
                {
                    customFuncParams.Add(funcs[i]((T)_customParams[multiple?i:0]));
                }
            }

            if (parameters != null)
                return string.Format(text, customFuncParams.Concat(parameters.Select(f => f())).Concat(_customParams).ToArray());
            else if (_customParams.Length > 0)
                return string.Format(text, customFuncParams.Concat(_customParams).ToArray());
            else
                return text;
        }
    }
}
