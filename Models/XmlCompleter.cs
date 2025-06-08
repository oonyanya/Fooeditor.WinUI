using System.Text;
using FooEditEngine;
using FooEditEngine.WinUI;

namespace FooEditor.WinUI.Models
{
    class XmlCompleteItem : CompleteWord
    {
        public bool Attribute
        {
            get;
            set;
        }

        public XmlCompleteItem ParentTag
        {
            get;
            set;
        }

        public XmlCompleteItem(string word, bool attr, XmlCompleteItem parent = null)
            : base(word)
        {
            this.Attribute = attr;
            this.ParentTag = parent;
        }
    }

    class XmlCompleter
    {
        static bool inputedTag = false;

        public static void LoadKeywords(AutoCompleteBoxBase CompleteBox, SyntaxDefnition syntax_def)
        {
            CompleteBox.Items.Clear();
            CompleteBox.Operators = syntax_def.Operators;
            CompleteBox.ShowingCompleteBox = (s, e) => { };

            foreach (string word in syntax_def.Keywords)
                CompleteBox.Items.Add(new XmlCompleteItem(word, false));
            foreach (string word in syntax_def.Keywords2)
                CompleteBox.Items.Add(new XmlCompleteItem(word, true));

            CompleteBox.ShowingCompleteBox = ShowingComplete;
            CompleteBox.SelectItem = SelectItem;
            CompleteBox.CollectItems = (s,e)=> { };
        }

        static void SelectItem(object sender,SelectItemEventArgs e)
        {
            string inputing_word = e.inputing_word;
            string word = e.item.word;

            XmlCompleteItem item = (XmlCompleteItem)e.item;
            if (!item.Attribute)
                word = e.item.word + "></" + word + ">";  //string.format()だとなぜかうまくいかない & '<'は既に入力している

            var doc = e.textbox;
            //キャレットは入力された文字の後ろにあるので、一致する分だけ選択して置き換える
            long caretIndex = doc.LayoutLines.GetLongIndexFromTextPoint(e.textbox.CaretPostion);
            long start = caretIndex - inputing_word.Length;
            if (start < 0)
                start = 0;
            doc.Replace(start, inputing_word.Length, word);
            doc.RequestRedraw();
        }

        static void ShowingComplete(object sender, ShowingCompleteBoxEventArgs e)
        {
            AutoCompleteBoxBase box = (AutoCompleteBoxBase)sender;

            var doc = e.textbox;
            long caretIndex = doc.LayoutLines.GetLongIndexFromTextPoint(e.textbox.CaretPostion);
            long inputingIndex = caretIndex - 1;
            if (inputingIndex < 0)
                inputingIndex = 0;

            string parentTag;
            inputedTag = IsInputedTag(e.textbox, inputingIndex, out parentTag);

            e.inputedWord = CompleteHelper.GetWord(doc, inputingIndex, box.Operators) + e.KeyChar;

            if (e.inputedWord == null)
                return;

            for (int i = 0; i < box.Items.Count; i++)
            {
                XmlCompleteItem item = box.Items[i] as XmlCompleteItem;
                if (item == null)
                    continue;
                if (item.word.StartsWith(e.inputedWord))
                {
                    if (inputedTag && !item.Attribute)
                        continue;
                    if (item.ParentTag != null && item.ParentTag.word != parentTag)
                        continue;
                    e.foundIndex = i;
                    break;
                }
            }
        }

        static bool IsInputedTag(Document doc, long index, out string tag)
        {
            bool hasSpace = false;
            StringBuilder word = new StringBuilder();

            if (doc.Length == 0)
            {
                tag = null;
                return false;
            }

            for (long i = index; i >= 0; i--)
            {
                if (doc[i] == ' ')
                {
                    hasSpace = true;
                    word.Clear();
                    continue;
                }else if (doc[i] == '<'){
                    if (hasSpace)
                    {
                        tag = word.ToString();
                        return true;
                    }else{
                        break;
                    }
                }
                word.Insert(0, doc[i]);
            }
            tag = null;
            return false;
        }

    }
}
